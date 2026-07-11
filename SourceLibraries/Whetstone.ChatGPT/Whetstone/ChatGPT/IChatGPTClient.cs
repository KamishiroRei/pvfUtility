using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Whetstone.ChatGPT.Models;

namespace Whetstone.ChatGPT;

public interface IChatGPTClient : IDisposable
{
	ChatGPTCredentials? Credentials { set; }

	Task<ChatGPTChatCompletionResponse?> CreateChatCompletionAsync(ChatGPTChatCompletionRequest completionRequest, CancellationToken? cancellationToken = null);

	Task<ChatGPTCompletionResponse?> CreateCompletionAsync(ChatGPTCompletionRequest completionRequest, CancellationToken? cancellationToken = null);

	Task<ChatGPTCreateEditResponse?> CreateEditAsync(ChatGPTCreateEditRequest createEditRequest, CancellationToken? cancellationToken = null);

	Task<ChatGPTDeleteResponse?> DeleteFileAsync(string? fileId, CancellationToken? cancellationToken = null);

	Task<ChatGPTListResponse<ChatGPTFileInfo>?> ListFilesAsync(CancellationToken? cancellationToken = null);

	Task<ChatGPTListResponse<ChatGPTModel>?> ListModelsAsync(CancellationToken? cancellationToken = null);

	Task<ChatGPTFileInfo?> RetrieveFileAsync(string? fileId, CancellationToken? cancellationToken = null);

	Task<ChatGPTFileContent?> RetrieveFileContentAsync(string? fileId, CancellationToken? cancellationToken = null);

	Task<ChatGPTModel?> RetrieveModelAsync(string modelId, CancellationToken? cancellationToken = null);

	Task<ChatGPTDeleteResponse?> DeleteModelAsync(string? modelId, CancellationToken? cancellationToken = null);

	Task<ChatGPTFileInfo?> UploadFileAsync(ChatGPTUploadFileRequest? fileRequest, CancellationToken? cancellationToken = null);

	Task<ChatGPTFineTuneJob?> CreateFineTuneAsync(ChatGPTCreateFineTuneRequest? createFineTuneRequest, CancellationToken? cancellationToken = null);

	Task<ChatGPTListResponse<ChatGPTFineTuneJob>?> ListFineTunesAsync(CancellationToken? cancellationToken = null);

	Task<ChatGPTFineTuneJob?> RetrieveFineTuneAsync(string? fineTuneId, CancellationToken? cancellationToken = null);

	Task<ChatGPTFineTuneJob?> CancelFineTuneAsync(string? fineTuneId, CancellationToken? cancellationToken = null);

	Task<ChatGPTListResponse<ChatGPTEvent>?> ListFineTuneEventsAsync(string? fineTuneId, CancellationToken? cancellationToken = null);

	Task<ChatGPTCreateModerationResponse?> CreateModerationAsync(ChatGPTCreateModerationRequest? createModerationRequest, CancellationToken? cancellationToken = null);

	Task<ChatGPTCreateEmbeddingsResponse?> CreateEmbeddingsAsync(ChatGPTCreateEmbeddingsRequest? createEmbeddingsRequest, CancellationToken? cancellationToken = null);

	Task<ChatGPTImageResponse?> CreateImageAsync(ChatGPTCreateImageRequest? createImageRequest, CancellationToken? cancellationToken = null);

	Task<ChatGPTImageResponse?> CreateImageVariationAsync(ChatGPTCreateImageVariationRequest? imageVariationRequest, CancellationToken? cancellationToken = null);

	Task<ChatGPTImageResponse?> CreateImageEditAsync(ChatGPTCreateImageEditRequest? imageEditRequest, CancellationToken? cancellationToken = null);

	IAsyncEnumerable<ChatGPTCompletionStreamResponse?> StreamCompletionAsync(ChatGPTCompletionRequest completionRequest, CancellationToken? cancellationToken = null);

	IAsyncEnumerable<ChatGPTChatCompletionStreamResponse?> StreamChatCompletionAsync(ChatGPTChatCompletionRequest completionRequest, CancellationToken? cancellationToken = null);

	IAsyncEnumerable<ChatGPTEvent?> StreamFineTuneEventsAsync(string? fineTuneId, CancellationToken? cancellationToken = null);

	Task<byte[]?> DownloadImageAsync(GeneratedImage generatedImage, CancellationToken? cancellationToken = null);

	Task<ChatGPTAudioResponse?> CreateTranscriptionAsync(ChatGPTAudioTranscriptionRequest transcriptionRequest, bool verbose = false, CancellationToken? cancellationToken = null);

	Task<string?> CreateTranscriptionAsync(ChatGPTAudioTranscriptionRequest transcriptionRequest, AudioResponseFormatText textFormat = AudioResponseFormatText.Text, CancellationToken? cancellationToken = null);

	Task<ChatGPTAudioResponse?> CreateTranslationAsync(ChatGPTAudioTranslationRequest translationRequest, bool verbose = false, CancellationToken? cancellationToken = null);

	Task<string?> CreateTranslationAsync(ChatGPTAudioTranslationRequest translationRequest, AudioResponseFormatText textFormat = AudioResponseFormatText.Text, CancellationToken? cancellationToken = null);
}
