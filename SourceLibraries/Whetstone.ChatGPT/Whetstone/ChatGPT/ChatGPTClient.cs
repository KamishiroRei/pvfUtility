using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Whetstone.ChatGPT.Models;

namespace Whetstone.ChatGPT;

public class ChatGPTClient : IChatGPTClient, IDisposable
{
	private const string ResponseLinePrefix = "data: ";

	private readonly HttpClient _client;

	private readonly bool _isHttpClientProvided = true;

	private ChatGPTCredentials? _chatCredentials;

	private bool _isDisposed;

	public ChatGPTCredentials? Credentials
	{
		set
		{
			_chatCredentials = value;
		}
	}

	public ChatGPTClient(string apiKey)
		: this(new ChatGPTCredentials(apiKey), new HttpClient())
	{
	}

	public ChatGPTClient(string apiKey, string organization)
		: this(new ChatGPTCredentials(apiKey, organization), new HttpClient())
	{
	}

	public ChatGPTClient(ChatGPTCredentials credentials)
		: this(credentials, new HttpClient())
	{
	}

	public ChatGPTClient(IOptions<ChatGPTCredentials> credentialsOptions)
		: this(credentialsOptions.Value, new HttpClient())
	{
	}

	public ChatGPTClient(IOptions<ChatGPTCredentials> credentialsOptions, HttpClient httpClient)
		: this(credentialsOptions.Value, httpClient)
	{
	}

	private ChatGPTClient(ChatGPTCredentials credentials, HttpClient httpClient)
	{
		_chatCredentials = credentials;
		if (httpClient == null)
		{
			_client = new HttpClient();
			_isHttpClientProvided = false;
		}
		else
		{
			_client = httpClient;
			_isHttpClientProvided = true;
		}
		InitializeClient(_client);
	}

	private void InitializeClient(HttpClient client)
	{
		client.BaseAddress = new Uri("https://api.openai.com/v1/");
		if (!string.IsNullOrWhiteSpace(_client.DefaultRequestHeaders.Authorization?.Parameter))
		{
			throw new ArgumentException("HttpClient already has authorization token.", "client");
		}
	}

	public async Task<ChatGPTChatCompletionResponse?> CreateChatCompletionAsync(ChatGPTChatCompletionRequest completionRequest, CancellationToken? cancellationToken = null)
	{
		if (completionRequest == null)
		{
			throw new ArgumentNullException("completionRequest");
		}
		if (string.IsNullOrWhiteSpace(completionRequest.Model))
		{
			throw new ArgumentException("Model is required", "completionRequest");
		}
		if (completionRequest.Messages == null || completionRequest.Messages.Count == 0)
		{
			throw new ArgumentException("Message is required", "completionRequest");
		}
		completionRequest.Stream = false;
		return await SendRequestAsync<ChatGPTChatCompletionRequest, ChatGPTChatCompletionResponse>(HttpMethod.Post, "chat/completions", completionRequest, cancellationToken);
	}

	public async IAsyncEnumerable<ChatGPTChatCompletionStreamResponse?> StreamChatCompletionAsync(ChatGPTChatCompletionRequest completionRequest, CancellationToken? cancellationToken = null)
	{
		if (completionRequest == null)
		{
			throw new ArgumentNullException("completionRequest");
		}
		if (string.IsNullOrWhiteSpace(completionRequest.Model))
		{
			throw new ArgumentException("Model is required", "completionRequest");
		}
		if (completionRequest.Messages == null || completionRequest.Messages.Count == 0)
		{
			throw new ArgumentException("Message is required", "completionRequest");
		}
		completionRequest.Stream = true;
		using HttpRequestMessage httpReq = CreateRequestMessage(HttpMethod.Post, "chat/completions");
		CancellationToken cancelToken = cancellationToken ?? CancellationToken.None;
		string content = JsonSerializer.Serialize(completionRequest);
		httpReq.Content = new StringContent(content, Encoding.UTF8, "application/json");
		HttpResponseMessage responseMessage = await _client.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		if (responseMessage.IsSuccessStatusCode)
		{
			using (Stream responseStream = await responseMessage.Content.ReadAsStreamAsync(cancelToken).ConfigureAwait(continueOnCapturedContext: false))
			{
				using StreamReader reader = new StreamReader(responseStream);
				string text;
				while ((text = await reader.ReadLineAsync()) != null)
				{
					if (!text.StartsWith("data: ", StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					text = text.Substring("data: ".Length).Trim();
					if (!string.IsNullOrWhiteSpace(text) && text != "[DONE]")
					{
						ChatGPTChatCompletionStreamResponse chatGPTChatCompletionStreamResponse;
						try
						{
							chatGPTChatCompletionStreamResponse = JsonSerializer.Deserialize<ChatGPTChatCompletionStreamResponse>(text);
						}
						catch (JsonException innerEx)
						{
							throw new ChatGPTException("Error deserializing ChatGPT streamed chat response: " + text, innerEx);
						}
						ChatGPTStreamedChatChoice chatGPTStreamedChatChoice = chatGPTChatCompletionStreamResponse?.GetChoice();
						if (chatGPTStreamedChatChoice != null && !string.Equals(chatGPTStreamedChatChoice.FinishReason, "stop", StringComparison.OrdinalIgnoreCase) && chatGPTStreamedChatChoice.Delta != null && chatGPTStreamedChatChoice.Delta.Content != null)
						{
							yield return chatGPTChatCompletionStreamResponse;
						}
					}
				}
			}
			yield break;
		}
		throw new ChatGPTException(JsonSerializer.Deserialize<ChatGPTErrorResponse>(await responseMessage.Content.ReadAsStringAsync(cancelToken).ConfigureAwait(continueOnCapturedContext: false))?.Error, responseMessage.StatusCode);
	}

	public async Task<ChatGPTCompletionResponse?> CreateCompletionAsync(ChatGPTCompletionRequest completionRequest, CancellationToken? cancellationToken = null)
	{
		if (completionRequest == null)
		{
			throw new ArgumentNullException("completionRequest");
		}
		if (string.IsNullOrWhiteSpace(completionRequest.Model))
		{
			throw new ArgumentException("Model is required", "completionRequest");
		}
		if (string.IsNullOrWhiteSpace(completionRequest.Prompt))
		{
			throw new ArgumentException("Prompt is required", "completionRequest");
		}
		completionRequest.Stream = false;
		return await SendRequestAsync<ChatGPTCompletionRequest, ChatGPTCompletionResponse>(HttpMethod.Post, "completions", completionRequest, cancellationToken);
	}

	public async Task<ChatGPTListResponse<ChatGPTModel>?> ListModelsAsync(CancellationToken? cancellationToken = null)
	{
		return await SendRequestAsync<ChatGPTListResponse<ChatGPTModel>>(HttpMethod.Get, "models", cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<ChatGPTModel?> RetrieveModelAsync(string modelId, CancellationToken? cancellationToken = null)
	{
		if (string.IsNullOrWhiteSpace(modelId))
		{
			throw new ArgumentException("Cannot be null or whitespace.", "modelId");
		}
		return await SendRequestAsync<ChatGPTModel>(HttpMethod.Get, "models/" + modelId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<ChatGPTDeleteResponse?> DeleteModelAsync(string? modelId, CancellationToken? cancellationToken = null)
	{
		if (string.IsNullOrWhiteSpace(modelId))
		{
			throw new ArgumentException("Cannot be null or whitespace.", "modelId");
		}
		return await SendRequestAsync<ChatGPTDeleteResponse>(HttpMethod.Delete, "models/" + modelId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async IAsyncEnumerable<ChatGPTCompletionStreamResponse?> StreamCompletionAsync(ChatGPTCompletionRequest completionRequest, CancellationToken? cancellationToken = null)
	{
		if (completionRequest == null)
		{
			throw new ArgumentNullException("completionRequest");
		}
		if (string.IsNullOrWhiteSpace(completionRequest.Model))
		{
			throw new ArgumentException("Model is required", "completionRequest");
		}
		if (string.IsNullOrWhiteSpace(completionRequest.Prompt))
		{
			throw new ArgumentException("Prompt is required", "completionRequest");
		}
		completionRequest.Stream = true;
		using HttpRequestMessage httpReq = CreateRequestMessage(HttpMethod.Post, "completions");
		CancellationToken cancelToken = cancellationToken ?? CancellationToken.None;
		string content = JsonSerializer.Serialize(completionRequest);
		httpReq.Content = new StringContent(content, Encoding.UTF8, "application/json");
		HttpResponseMessage responseMessage = await _client.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		if (responseMessage.IsSuccessStatusCode)
		{
			using (Stream responseStream = await responseMessage.Content.ReadAsStreamAsync(cancelToken).ConfigureAwait(continueOnCapturedContext: false))
			{
				using StreamReader reader = new StreamReader(responseStream);
				string text;
				while ((text = await reader.ReadLineAsync()) != null)
				{
					if (text.StartsWith("data: ", StringComparison.OrdinalIgnoreCase))
					{
						text = text.Substring("data: ".Length);
					}
					if (!string.IsNullOrWhiteSpace(text) && text != "[DONE]")
					{
						yield return JsonSerializer.Deserialize<ChatGPTCompletionStreamResponse>(text.Trim());
					}
				}
			}
			yield break;
		}
		throw new ChatGPTException(JsonSerializer.Deserialize<ChatGPTErrorResponse>(await responseMessage.Content.ReadAsStringAsync(cancelToken).ConfigureAwait(continueOnCapturedContext: false))?.Error, responseMessage.StatusCode);
	}

	public async Task<ChatGPTCreateEditResponse?> CreateEditAsync(ChatGPTCreateEditRequest createEditRequest, CancellationToken? cancellationToken = null)
	{
		if (createEditRequest == null)
		{
			throw new ArgumentNullException("createEditRequest");
		}
		if (string.IsNullOrWhiteSpace(createEditRequest.Model))
		{
			createEditRequest.Model = ChatGPTEditModels.Davinci;
		}
		if (string.IsNullOrWhiteSpace(createEditRequest.Instruction))
		{
			throw new ArgumentException("Instruction is required", "createEditRequest");
		}
		return await SendRequestAsync<ChatGPTCreateEditRequest, ChatGPTCreateEditResponse>(HttpMethod.Post, "edits", createEditRequest, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<ChatGPTFileInfo?> UploadFileAsync(ChatGPTUploadFileRequest? fileRequest, CancellationToken? cancellationToken = null)
	{
		if (fileRequest == null)
		{
			throw new ArgumentNullException("fileRequest");
		}
		if (fileRequest.File == null)
		{
			throw new ArgumentNullException("fileRequest", "File is required");
		}
		if (string.IsNullOrWhiteSpace(fileRequest.File.FileName))
		{
			throw new ArgumentException("File.FileName is required", "fileRequest");
		}
		if (fileRequest.File.Content == null)
		{
			throw new ArgumentException("File.Content is required", "fileRequest");
		}
		if (string.IsNullOrWhiteSpace(fileRequest.Purpose))
		{
			throw new ArgumentException("Purpose is required", "fileRequest");
		}
		MultipartFormDataContent content = new MultipartFormDataContent
		{
			{
				new StringContent(fileRequest.Purpose),
				"purpose"
			},
			{
				new ByteArrayContent(fileRequest.File.Content),
				"file",
				fileRequest.File.FileName
			}
		};
		using HttpRequestMessage httpReq = CreateRequestMessage(HttpMethod.Post, "files");
		httpReq.Content = content;
		HttpResponseMessage httpResponseMessage = (cancellationToken.HasValue ? (await _client.SendAsync(httpReq, cancellationToken.Value).ConfigureAwait(continueOnCapturedContext: false)) : (await _client.SendAsync(httpReq).ConfigureAwait(continueOnCapturedContext: false)));
		using HttpResponseMessage httpResponse = httpResponseMessage;
		return await ProcessResponseAsync<ChatGPTFileInfo>(httpResponse, cancellationToken);
	}

	public async Task<ChatGPTListResponse<ChatGPTFileInfo>?> ListFilesAsync(CancellationToken? cancellationToken = null)
	{
		return await SendRequestAsync<ChatGPTListResponse<ChatGPTFileInfo>>(HttpMethod.Get, "files", cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<ChatGPTDeleteResponse?> DeleteFileAsync(string? fileId, CancellationToken? cancellationToken = null)
	{
		if (string.IsNullOrWhiteSpace(fileId))
		{
			throw new ArgumentException("Cannot be null or whitespace.", "fileId");
		}
		return await SendRequestAsync<ChatGPTDeleteResponse>(HttpMethod.Delete, "files/" + fileId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<ChatGPTFileInfo?> RetrieveFileAsync(string? fileId, CancellationToken? cancellationToken = null)
	{
		if (string.IsNullOrWhiteSpace(fileId))
		{
			throw new ArgumentException("Cannot be null or whitespace.", "fileId");
		}
		return await SendRequestAsync<ChatGPTFileInfo>(HttpMethod.Get, "files/" + fileId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<ChatGPTFileContent?> RetrieveFileContentAsync(string? fileId, CancellationToken? cancellationToken = null)
	{
		if (string.IsNullOrWhiteSpace(fileId))
		{
			throw new ArgumentException("Cannot be null or whitespace.", "fileId");
		}
		CancellationToken cancelToken = cancellationToken ?? CancellationToken.None;
		using HttpRequestMessage httpReq = CreateRequestMessage(HttpMethod.Get, "files/" + fileId + "/content");
		using HttpResponseMessage httpResponse = await _client.SendAsync(httpReq, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		if (httpResponse.IsSuccessStatusCode)
		{
			ChatGPTFileContent fileContent = new ChatGPTFileContent();
			ChatGPTFileContent chatGPTFileContent = fileContent;
			chatGPTFileContent.Content = await httpResponse.Content.ReadAsByteArrayAsync(cancelToken).ConfigureAwait(continueOnCapturedContext: false);
			fileContent.FileName = httpResponse.Content?.Headers?.ContentDisposition?.FileName?.Replace("\"", "");
			return fileContent;
		}
		throw new ChatGPTException(JsonSerializer.Deserialize<ChatGPTErrorResponse>(await GetResponseStringAsync(httpResponse, cancelToken).ConfigureAwait(continueOnCapturedContext: false))?.Error, httpResponse.StatusCode);
	}

	public async Task<ChatGPTFineTuneJob?> CreateFineTuneAsync(ChatGPTCreateFineTuneRequest? createFineTuneRequest, CancellationToken? cancellationToken = null)
	{
		if (createFineTuneRequest == null)
		{
			throw new ArgumentNullException("createFineTuneRequest");
		}
		if (string.IsNullOrWhiteSpace(createFineTuneRequest.Model))
		{
			createFineTuneRequest.Model = "ada";
		}
		if (string.IsNullOrWhiteSpace(createFineTuneRequest.TrainingFileId))
		{
			throw new ArgumentException("TrainingFileId cannot be null or whitespace.", "createFineTuneRequest");
		}
		return await SendRequestAsync<ChatGPTCreateFineTuneRequest, ChatGPTFineTuneJob>(HttpMethod.Post, "fine-tunes", createFineTuneRequest, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<ChatGPTListResponse<ChatGPTFineTuneJob>?> ListFineTunesAsync(CancellationToken? cancellationToken = null)
	{
		return await SendRequestAsync<ChatGPTListResponse<ChatGPTFineTuneJob>>(HttpMethod.Get, "fine-tunes", cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<ChatGPTFineTuneJob?> RetrieveFineTuneAsync(string? fineTuneId, CancellationToken? cancellationToken = null)
	{
		if (string.IsNullOrWhiteSpace(fineTuneId))
		{
			throw new ArgumentException("Cannot be null or whitespace.", "fineTuneId");
		}
		return await SendRequestAsync<ChatGPTFineTuneJob>(HttpMethod.Get, "fine-tunes/" + fineTuneId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<ChatGPTFineTuneJob?> CancelFineTuneAsync(string? fineTuneId, CancellationToken? cancellationToken = null)
	{
		if (string.IsNullOrWhiteSpace(fineTuneId))
		{
			throw new ArgumentException("Cannot be null or whitespace.", "fineTuneId");
		}
		return await SendRequestAsync<ChatGPTFineTuneJob>(HttpMethod.Post, "fine-tunes/" + fineTuneId + "/cancel", cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<ChatGPTListResponse<ChatGPTEvent>?> ListFineTuneEventsAsync(string? fineTuneId, CancellationToken? cancellationToken = null)
	{
		if (string.IsNullOrWhiteSpace(fineTuneId))
		{
			throw new ArgumentException("Cannot be null or whitespace.", "fineTuneId");
		}
		return await SendRequestAsync<ChatGPTListResponse<ChatGPTEvent>>(HttpMethod.Get, "fine-tunes/" + fineTuneId + "/events", cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async IAsyncEnumerable<ChatGPTEvent?> StreamFineTuneEventsAsync(string? fineTuneId, CancellationToken? cancellationToken = null)
	{
		if (string.IsNullOrWhiteSpace(fineTuneId))
		{
			throw new ArgumentException("Cannot be null or whitespace.", "fineTuneId");
		}
		CancellationToken cancelToken = cancellationToken ?? CancellationToken.None;
		using HttpRequestMessage httpReq = CreateRequestMessage(HttpMethod.Get, "fine-tunes/" + fineTuneId + "/events?stream=true");
		HttpResponseMessage responseMessage = await _client.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		if (responseMessage.IsSuccessStatusCode)
		{
			using (Stream responseStream = await responseMessage.Content.ReadAsStreamAsync(cancelToken).ConfigureAwait(continueOnCapturedContext: false))
			{
				using StreamReader reader = new StreamReader(responseStream);
				string text;
				while ((text = await reader.ReadLineAsync()) != null)
				{
					if (text.StartsWith("data: ", StringComparison.OrdinalIgnoreCase))
					{
						text = text.Substring("data: ".Length);
					}
					if (!string.IsNullOrWhiteSpace(text) && text != "[DONE]")
					{
						yield return JsonSerializer.Deserialize<ChatGPTEvent>(text.Trim());
					}
				}
			}
			yield break;
		}
		throw new ChatGPTException(JsonSerializer.Deserialize<ChatGPTErrorResponse>(await responseMessage.Content.ReadAsStringAsync(cancelToken).ConfigureAwait(continueOnCapturedContext: false))?.Error, responseMessage.StatusCode);
	}

	public async Task<ChatGPTCreateModerationResponse?> CreateModerationAsync(ChatGPTCreateModerationRequest? createModerationRequest, CancellationToken? cancellationToken = null)
	{
		if (createModerationRequest == null)
		{
			throw new ArgumentNullException("createModerationRequest");
		}
		if (createModerationRequest.Inputs == null)
		{
			throw new ArgumentException("Inputs cannot be null", "createModerationRequest");
		}
		if (!createModerationRequest.Inputs.Any())
		{
			throw new ArgumentException("Inputs must have one or more items", "createModerationRequest");
		}
		return await SendRequestAsync<ChatGPTCreateModerationRequest, ChatGPTCreateModerationResponse>(HttpMethod.Post, "moderations", createModerationRequest, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<ChatGPTCreateEmbeddingsResponse?> CreateEmbeddingsAsync(ChatGPTCreateEmbeddingsRequest? createEmbeddingsRequest, CancellationToken? cancellationToken = null)
	{
		if (createEmbeddingsRequest == null)
		{
			throw new ArgumentNullException("createEmbeddingsRequest");
		}
		if (createEmbeddingsRequest.Inputs == null)
		{
			throw new ArgumentException("Inputs cannot be null", "createEmbeddingsRequest");
		}
		if (string.IsNullOrWhiteSpace(createEmbeddingsRequest.Model))
		{
			throw new ArgumentException("Model cannot be null or empty", "createEmbeddingsRequest");
		}
		return await SendRequestAsync<ChatGPTCreateEmbeddingsRequest, ChatGPTCreateEmbeddingsResponse>(HttpMethod.Post, "embeddings", createEmbeddingsRequest, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<ChatGPTImageResponse?> CreateImageAsync(ChatGPTCreateImageRequest? createImageRequest, CancellationToken? cancellationToken = null)
	{
		if (createImageRequest == null)
		{
			throw new ArgumentNullException("createImageRequest");
		}
		if (string.IsNullOrWhiteSpace(createImageRequest.Prompt))
		{
			throw new ArgumentException("Prompt cannot be null or empty", "createImageRequest");
		}
		if (createImageRequest.Prompt.Length > 1000)
		{
			throw new ArgumentException("Prompt cannot be longer than 1000 characters", "createImageRequest");
		}
		if (createImageRequest.NumberOfImagesToGenerate < 0)
		{
			throw new ArgumentException("NumberOfImagesToGenerate must be between 1 and 10", "createImageRequest");
		}
		if (createImageRequest.NumberOfImagesToGenerate > 10)
		{
			throw new ArgumentException("NumberOfImagesToGenerate must be between 1 and 10", "createImageRequest");
		}
		return await SendRequestAsync<ChatGPTCreateImageRequest, ChatGPTImageResponse>(HttpMethod.Post, "images/generations", createImageRequest, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<ChatGPTImageResponse?> CreateImageVariationAsync(ChatGPTCreateImageVariationRequest? imageVariationRequest, CancellationToken? cancellationToken = null)
	{
		if (imageVariationRequest == null)
		{
			throw new ArgumentNullException("imageVariationRequest");
		}
		if (imageVariationRequest.NumberOfImagesToGenerate < 0)
		{
			throw new ArgumentException("NumberOfImagesToGenerate must be between 1 and 10", "imageVariationRequest");
		}
		if (imageVariationRequest.NumberOfImagesToGenerate > 10)
		{
			throw new ArgumentException("NumberOfImagesToGenerate must be between 1 and 10", "imageVariationRequest");
		}
		if (imageVariationRequest.Image == null)
		{
			throw new ArgumentException("Image cannot be null", "imageVariationRequest");
		}
		if (string.IsNullOrWhiteSpace(imageVariationRequest.Image.FileName))
		{
			throw new ArgumentException("Image.FileName cannot be null or empty", "imageVariationRequest");
		}
		if (imageVariationRequest.Image.Content == null)
		{
			throw new ArgumentException("Image.Content cannot be null", "imageVariationRequest");
		}
		if (imageVariationRequest.Image.Content.Length == 0)
		{
			throw new ArgumentException("Image.Content.Length cannot be 0", "imageVariationRequest");
		}
		MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent
		{
			{
				new ByteArrayContent(imageVariationRequest.Image.Content),
				"image",
				imageVariationRequest.Image.FileName
			},
			{
				new StringContent(imageVariationRequest.Size.GetDescriptionFromEnumValue()),
				"size"
			},
			{
				new StringContent(imageVariationRequest.ResponseFormat.GetDescriptionFromEnumValue()),
				"response_format"
			}
		};
		if (imageVariationRequest.NumberOfImagesToGenerate != 1)
		{
			multipartFormDataContent.Add(new StringContent(imageVariationRequest.NumberOfImagesToGenerate.ToString(CultureInfo.InvariantCulture)), "n");
		}
		if (!string.IsNullOrWhiteSpace(imageVariationRequest.User))
		{
			multipartFormDataContent.Add(new StringContent(imageVariationRequest.User), "user");
		}
		HttpRequestMessage httpRequestMessage = CreateRequestMessage(HttpMethod.Post, "images/variations");
		httpRequestMessage.Content = multipartFormDataContent;
		HttpResponseMessage httpResponseMessage = (cancellationToken.HasValue ? (await _client.SendAsync(httpRequestMessage, cancellationToken.Value).ConfigureAwait(continueOnCapturedContext: false)) : (await _client.SendAsync(httpRequestMessage).ConfigureAwait(continueOnCapturedContext: false)));
		using HttpResponseMessage httpResponse = httpResponseMessage;
		return await ProcessResponseAsync<ChatGPTImageResponse>(httpResponse, cancellationToken);
	}

	public async Task<ChatGPTImageResponse?> CreateImageEditAsync(ChatGPTCreateImageEditRequest? imageEditRequest, CancellationToken? cancellationToken = null)
	{
		if (imageEditRequest == null)
		{
			throw new ArgumentNullException("imageEditRequest");
		}
		if (string.IsNullOrWhiteSpace(imageEditRequest.Prompt))
		{
			throw new ArgumentException("Prompt cannot be null or empty", "imageEditRequest");
		}
		if (imageEditRequest.Prompt.Length > 1000)
		{
			throw new ArgumentException("Prompt cannot be longer than 1000 characters", "imageEditRequest");
		}
		if (imageEditRequest.NumberOfImagesToGenerate < 0)
		{
			throw new ArgumentException("NumberOfImagesToGenerate must be between 1 and 10", "imageEditRequest");
		}
		if (imageEditRequest.NumberOfImagesToGenerate > 10)
		{
			throw new ArgumentException("NumberOfImagesToGenerate must be between 1 and 10", "imageEditRequest");
		}
		if (imageEditRequest.Image == null)
		{
			throw new ArgumentException("Image cannot be null", "imageEditRequest");
		}
		if (string.IsNullOrWhiteSpace(imageEditRequest.Image.FileName))
		{
			throw new ArgumentException("Image.FileName cannot be null or empty", "imageEditRequest");
		}
		if (imageEditRequest.Image.Content == null)
		{
			throw new ArgumentException("Image.Content cannot be null", "imageEditRequest");
		}
		if (imageEditRequest.Image.Content.Length == 0)
		{
			throw new ArgumentException("Image.Content.Length cannot be 0", "imageEditRequest");
		}
		ByteArrayContent byteArrayContent = null;
		if (imageEditRequest.Mask != null)
		{
			if (imageEditRequest.Mask.Content == null)
			{
				throw new ArgumentException("If Mask is provided, then Mask.Content cannot be null", "imageEditRequest");
			}
			if (imageEditRequest.Mask.Content.Length == 0)
			{
				throw new ArgumentException("If Mask is provided, then Mask.Content.Length cannot be 0", "imageEditRequest");
			}
			if (string.IsNullOrWhiteSpace(imageEditRequest.Mask.FileName))
			{
				throw new ArgumentException("If Mask is provided, then Mask.FileName cannot be null or empty", "imageEditRequest");
			}
			byteArrayContent = new ByteArrayContent(imageEditRequest.Mask.Content);
		}
		MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent
		{
			{
				new ByteArrayContent(imageEditRequest.Image.Content),
				"image",
				imageEditRequest.Image.FileName
			},
			{
				new StringContent(imageEditRequest.Prompt),
				"prompt"
			},
			{
				new StringContent(imageEditRequest.Size.GetDescriptionFromEnumValue()),
				"size"
			},
			{
				new StringContent(imageEditRequest.ResponseFormat.GetDescriptionFromEnumValue()),
				"response_format"
			}
		};
		if (byteArrayContent != null)
		{
			multipartFormDataContent.Add(byteArrayContent, "mask", imageEditRequest.Mask.FileName);
		}
		if (imageEditRequest.NumberOfImagesToGenerate != 1)
		{
			multipartFormDataContent.Add(new StringContent(imageEditRequest.NumberOfImagesToGenerate.ToString(CultureInfo.InvariantCulture)), "n");
		}
		if (!string.IsNullOrWhiteSpace(imageEditRequest.User))
		{
			multipartFormDataContent.Add(new StringContent(imageEditRequest.User), "user");
		}
		using HttpRequestMessage httpReq = CreateRequestMessage(HttpMethod.Post, "images/edits");
		httpReq.Content = multipartFormDataContent;
		using HttpResponseMessage httpResponse = await _client.SendAsync(httpReq, (!cancellationToken.HasValue) ? CancellationToken.None : cancellationToken.Value).ConfigureAwait(continueOnCapturedContext: false);
		return await ProcessResponseAsync<ChatGPTImageResponse>(httpResponse, cancellationToken);
	}

	public async Task<byte[]?> DownloadImageAsync(GeneratedImage generatedImage, CancellationToken? cancellationToken = null)
	{
		byte[] retVal = null;
		CancellationToken cancelToken = ((!cancellationToken.HasValue) ? CancellationToken.None : cancellationToken.Value);
		if (generatedImage == null)
		{
			throw new ArgumentNullException("generatedImage");
		}
		if (string.IsNullOrWhiteSpace(generatedImage.Url) && string.IsNullOrWhiteSpace(generatedImage.Base64))
		{
			throw new ArgumentException("Either Url or Base64 properties must be provided", "generatedImage");
		}
		if (!string.IsNullOrWhiteSpace(generatedImage.Base64))
		{
			retVal = Convert.FromBase64String(generatedImage.Base64);
		}
		else
		{
			if (!Uri.TryCreate(generatedImage.Url, UriKind.Absolute, out Uri result))
			{
				throw new ArgumentException("Url is not a valid Uri", "generatedImage");
			}
			using HttpRequestMessage requestMessage = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = result
			};
			using HttpResponseMessage httpResponse = await _client.SendAsync(requestMessage, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
			if (httpResponse != null)
			{
				if (!httpResponse.IsSuccessStatusCode)
				{
					throw new ChatGPTException("Error downloading image " + generatedImage.Url + ": " + httpResponse.ReasonPhrase, httpResponse.StatusCode);
				}
				retVal = await httpResponse.Content.ReadAsByteArrayAsync(cancelToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		return retVal;
	}

	public async Task<ChatGPTAudioResponse?> CreateTranscriptionAsync(ChatGPTAudioTranscriptionRequest transcriptionRequest, bool verbose = false, CancellationToken? cancellationToken = null)
	{
		MultipartFormDataContent multipartFormDataContent = BuildTranscriptionRequest(transcriptionRequest);
		multipartFormDataContent.Add(new StringContent(verbose ? "verbose_json" : "json"), "response_format");
		using HttpRequestMessage httpReq = CreateRequestMessage(HttpMethod.Post, "audio/transcriptions");
		httpReq.Content = multipartFormDataContent;
		HttpResponseMessage httpResponseMessage = (cancellationToken.HasValue ? (await _client.SendAsync(httpReq, cancellationToken.Value).ConfigureAwait(continueOnCapturedContext: false)) : (await _client.SendAsync(httpReq).ConfigureAwait(continueOnCapturedContext: false)));
		using HttpResponseMessage httpResponse = httpResponseMessage;
		return await ProcessResponseAsync<ChatGPTAudioResponse>(httpResponse, cancellationToken);
	}

	public async Task<string?> CreateTranscriptionAsync(ChatGPTAudioTranscriptionRequest transcriptionRequest, AudioResponseFormatText textFormat = AudioResponseFormatText.Text, CancellationToken? cancellationToken = null)
	{
		MultipartFormDataContent multipartFormDataContent = BuildTranscriptionRequest(transcriptionRequest);
		multipartFormDataContent.Add(new StringContent(textFormat.GetDescriptionFromEnumValue()), "response_format");
		using HttpRequestMessage httpReq = CreateRequestMessage(HttpMethod.Post, "audio/transcriptions");
		httpReq.Content = multipartFormDataContent;
		HttpResponseMessage httpResponseMessage = (cancellationToken.HasValue ? (await _client.SendAsync(httpReq, cancellationToken.Value).ConfigureAwait(continueOnCapturedContext: false)) : (await _client.SendAsync(httpReq).ConfigureAwait(continueOnCapturedContext: false)));
		using HttpResponseMessage httpResponse = httpResponseMessage;
		return await GetResponseStringAsync(httpResponse, cancellationToken);
	}

	private static MultipartFormDataContent BuildTranscriptionRequest(ChatGPTAudioTranscriptionRequest transcriptionRequest)
	{
		if (transcriptionRequest == null)
		{
			throw new ArgumentNullException("transcriptionRequest");
		}
		if (transcriptionRequest.File == null)
		{
			throw new ArgumentNullException("transcriptionRequest", "File is required");
		}
		if (string.IsNullOrWhiteSpace(transcriptionRequest.File.FileName))
		{
			throw new ArgumentException("File.FileName is required", "transcriptionRequest");
		}
		if (transcriptionRequest.File.Content == null)
		{
			throw new ArgumentException("File.Content is required", "transcriptionRequest");
		}
		if (string.IsNullOrWhiteSpace(transcriptionRequest.Model))
		{
			throw new ArgumentException("Model is required", "transcriptionRequest");
		}
		MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent
		{
			{
				new StringContent(transcriptionRequest.Model),
				"model"
			},
			{
				new ByteArrayContent(transcriptionRequest.File.Content),
				"file",
				transcriptionRequest.File.FileName
			},
			{
				new StringContent(transcriptionRequest.Temperature.ToString("0.0", CultureInfo.InvariantCulture)),
				"temperature"
			}
		};
		if (!string.IsNullOrEmpty(transcriptionRequest.Language))
		{
			multipartFormDataContent.Add(new StringContent(transcriptionRequest.Language), "language");
		}
		if (!string.IsNullOrEmpty(transcriptionRequest.Prompt))
		{
			multipartFormDataContent.Add(new StringContent(transcriptionRequest.Prompt), "prompt");
		}
		return multipartFormDataContent;
	}

	public async Task<ChatGPTAudioResponse?> CreateTranslationAsync(ChatGPTAudioTranslationRequest translationRequest, bool verbose = false, CancellationToken? cancellationToken = null)
	{
		MultipartFormDataContent multipartFormDataContent = BuildTranslationRequest(translationRequest);
		multipartFormDataContent.Add(new StringContent(verbose ? "verbose_json" : "json"), "response_format");
		using HttpRequestMessage httpReq = CreateRequestMessage(HttpMethod.Post, "audio/translations");
		httpReq.Content = multipartFormDataContent;
		HttpResponseMessage httpResponseMessage = (cancellationToken.HasValue ? (await _client.SendAsync(httpReq, cancellationToken.Value).ConfigureAwait(continueOnCapturedContext: false)) : (await _client.SendAsync(httpReq).ConfigureAwait(continueOnCapturedContext: false)));
		using HttpResponseMessage httpResponse = httpResponseMessage;
		return await ProcessResponseAsync<ChatGPTAudioResponse>(httpResponse, cancellationToken);
	}

	public async Task<string?> CreateTranslationAsync(ChatGPTAudioTranslationRequest translationRequest, AudioResponseFormatText textFormat = AudioResponseFormatText.Text, CancellationToken? cancellationToken = null)
	{
		MultipartFormDataContent multipartFormDataContent = BuildTranslationRequest(translationRequest);
		multipartFormDataContent.Add(new StringContent(textFormat.GetDescriptionFromEnumValue()), "response_format");
		using HttpRequestMessage httpReq = CreateRequestMessage(HttpMethod.Post, "audio/translations");
		httpReq.Content = multipartFormDataContent;
		HttpResponseMessage httpResponseMessage = (cancellationToken.HasValue ? (await _client.SendAsync(httpReq, cancellationToken.Value).ConfigureAwait(continueOnCapturedContext: false)) : (await _client.SendAsync(httpReq).ConfigureAwait(continueOnCapturedContext: false)));
		using HttpResponseMessage httpResponse = httpResponseMessage;
		return await GetResponseStringAsync(httpResponse, cancellationToken);
	}

	private static MultipartFormDataContent BuildTranslationRequest(ChatGPTAudioTranslationRequest translationRequest)
	{
		if (translationRequest == null)
		{
			throw new ArgumentNullException("translationRequest");
		}
		if (translationRequest.File == null)
		{
			throw new ArgumentNullException("translationRequest", "File is required");
		}
		if (string.IsNullOrWhiteSpace(translationRequest.File.FileName))
		{
			throw new ArgumentException("File.FileName is required", "translationRequest");
		}
		if (translationRequest.File.Content == null)
		{
			throw new ArgumentException("File.Content is required", "translationRequest");
		}
		if (string.IsNullOrWhiteSpace(translationRequest.Model))
		{
			throw new ArgumentException("Model is required", "translationRequest");
		}
		MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent
		{
			{
				new StringContent(translationRequest.Model),
				"model"
			},
			{
				new ByteArrayContent(translationRequest.File.Content),
				"file",
				translationRequest.File.FileName
			},
			{
				new StringContent(translationRequest.Temperature.ToString("0.0", CultureInfo.InvariantCulture)),
				"temperature"
			}
		};
		if (!string.IsNullOrEmpty(translationRequest.Prompt))
		{
			multipartFormDataContent.Add(new StringContent(translationRequest.Prompt), "prompt");
		}
		return multipartFormDataContent;
	}

	private async Task<TR?> SendRequestAsync<T, TR>(HttpMethod method, string url, T requestMessage, CancellationToken? cancellationToken) where T : class where TR : class
	{
		using HttpRequestMessage httpReq = CreateRequestMessage(method, url);
		string content = JsonSerializer.Serialize(requestMessage);
		httpReq.Content = new StringContent(content, Encoding.UTF8, "application/json");
		HttpResponseMessage httpResponseMessage = (cancellationToken.HasValue ? (await _client.SendAsync(httpReq, cancellationToken.Value).ConfigureAwait(continueOnCapturedContext: false)) : (await _client.SendAsync(httpReq).ConfigureAwait(continueOnCapturedContext: false)));
		using HttpResponseMessage httpResponse = httpResponseMessage;
		return await ProcessResponseAsync<TR>(httpResponse, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	private async Task<T?> SendRequestAsync<T>(HttpMethod method, string url, CancellationToken? cancellationToken) where T : class
	{
		using HttpRequestMessage request = CreateRequestMessage(method, url);
		HttpResponseMessage httpResponseMessage = (cancellationToken.HasValue ? (await _client.SendAsync(request, cancellationToken.Value).ConfigureAwait(continueOnCapturedContext: false)) : (await _client.SendAsync(request).ConfigureAwait(continueOnCapturedContext: false)));
		using HttpResponseMessage httpResponse = httpResponseMessage;
		return await ProcessResponseAsync<T>(httpResponse, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	private static async Task<T?> ProcessResponseAsync<T>(HttpResponseMessage responseMessage, CancellationToken? cancellationToken) where T : class
	{
		string text = await GetResponseStringAsync(responseMessage, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (responseMessage.IsSuccessStatusCode)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				T val = JsonSerializer.Deserialize<T>(text);
				if (val == null)
				{
					throw new ChatGPTException("Unable to deserialize response.");
				}
				return val;
			}
			return null;
		}
		throw new ChatGPTException(JsonSerializer.Deserialize<ChatGPTErrorResponse>(text)?.Error, responseMessage.StatusCode);
	}

	private static async Task<string> GetResponseStringAsync(HttpResponseMessage responseMessage, CancellationToken? cancellationToken)
	{
		return cancellationToken.HasValue ? (await responseMessage.Content.ReadAsStringAsync(cancellationToken.Value).ConfigureAwait(continueOnCapturedContext: false)) : (await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false));
	}

	private HttpRequestMessage CreateRequestMessage(HttpMethod method, string url)
	{
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, url);
		if (_chatCredentials == null)
		{
			throw new ChatGPTException("ChatGPTCredentials are null.");
		}
		if (string.IsNullOrWhiteSpace(_chatCredentials.ApiKey))
		{
			throw new ChatGPTException("ApiKey property cannot be null or whitespace.");
		}
		httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _chatCredentials.ApiKey);
		if (!string.IsNullOrWhiteSpace(_chatCredentials.Organization))
		{
			httpRequestMessage.Headers.Add("OpenAI-Organization", _chatCredentials.Organization);
		}
		return httpRequestMessage;
	}

	~ChatGPTClient()
	{
		Dispose(disposing: true);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (!_isDisposed)
		{
			if (!_isHttpClientProvided)
			{
				_client.Dispose();
			}
			_isDisposed = true;
		}
	}
}
