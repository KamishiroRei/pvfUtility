using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf;

namespace PvfCode.Services;

public class ScriptFileCompilerOl
{
	private Ilogger logger;

	private readonly PvfGroup pvf;

	private Ilogger GetLogger()
	{
		if (logger == null)
		{
			logger = AppSetting.Instance.GetService<Ilogger>();
		}
		return logger;
	}

	public ScriptFileCompilerOl(PvfGroup pack)
	{
		pvf = pack;
	}

	public string Decompile(PvfFile file)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder("#PVF_File\r\n");
			DecompileData(file.Data, file.DataLen, file.FileName, stringBuilder);
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			return AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_ReadScriptException") + "\r\n" + ex.Message;
		}
	}

	public string Decompile(byte[] scriptdata)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			DecompileData(scriptdata, scriptdata.Length, "", stringBuilder);
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			return AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_ReadScriptException") + "\r\n" + ex.Message;
		}
	}

	private void DecompileData(byte[] data, int dataLength, string fileName, StringBuilder output)
	{
		if (data != null && dataLength >= 7)
		{
			for (int i = 2; i < dataLength - 4; i += 5)
			{
				byte b = data[i];
				int num = BitConverter.ToInt32(data, i + 1);
				switch (b)
				{
				case 5:
				{
					output.Append($"\r\n{pvf.Strtable.GetStringItem(num)}\r\n");
					break;
				}
				case 10:
				{
					int strid = BitConverter.ToInt32(data, i - 4);
					string stringItem = pvf.Strtable.GetStringItem(num);
					if (AppSetting.Instance.PvfConfig.AutoConvertStringLink)
					{
						output.Append("`" + pvf.Strview.GetStrText(strid, stringItem, autoConvertStr: true).Replace("\\n", "\r\n") + "`\r\n");
						break;
					}
					output.Append("<" + strid + "::" + stringItem + "`" + pvf.Strview.GetStrText(strid, stringItem, autoConvertStr: true) + "`>\r\n");
					break;
				}
				case 7:
				{
					output.Append($"`{pvf.Strtable.GetStringItem(num, autoConvertStr: true)}`\r\n");
					break;
				}
				case 6:
				case 8:
					output.Append("{" + b + "=`" + pvf.Strtable.GetStringItem(num, autoConvertStr: true) + "`}\r\n");
					break;
				case 3:
					output.Append("{" + b.ToString() + "=" + num + "}\t");
					break;
				case 4:
					output.Append(DataHelper.FormatFloat(BitConverter.ToSingle(data, i + 1)) + "\t");
					break;
				case 2:
					output.Append(num + "\t");
					break;
				}
			}
		}
		output.Append("\r\n");
	}

	public Dictionary<string, string> DecompileDic(PvfFile file)
	{
		return DecompileSections(file.Data, file.DataLen);
	}

	private Dictionary<string, string> DecompileSections(byte[] data, int dataLength)
	{
		Dictionary<string, StringBuilder> dictionary = new Dictionary<string, StringBuilder>();
		bool flag = false;
		string text = null;
		if (data != null && dataLength >= 7)
		{
			for (int i = 2; i < dataLength - 4; i += 5)
			{
				byte b = data[i];
				int num = BitConverter.ToInt32(data, i + 1);
				switch (b)
				{
				case 5:
				{
					string sectionToken = pvf.Strtable.GetStringItem(num);
					if (sectionToken.Length > 2)
					{
						if (sectionToken[0] == '[' && sectionToken[1] == '/')
						{
							flag = false;
							text = null;
						}
						else if (sectionToken[0] == '[')
						{
							flag = true;
							if (!dictionary.ContainsKey(sectionToken))
							{
								dictionary.Add(sectionToken, new StringBuilder());
							}
							text = sectionToken;
						}
						else
						{
							text = null;
							flag = false;
						}
					}
					else
					{
						text = null;
						flag = false;
					}
					break;
				}
				case 10:
				{
					int strid = BitConverter.ToInt32(data, i - 4);
					string stringItem = pvf.Strtable.GetStringItem(num);
					if (flag)
					{
						if (AppSetting.Instance.PvfConfig.AutoConvertStringLink)
						{
							AppendSectionValue(dictionary, text, "`" + pvf.Strview.GetStrText(strid, stringItem, autoConvertStr: true).Replace("\\n", "\r\n") + "`");
							break;
						}
						AppendSectionValue(dictionary, text, "<" + strid + "::" + stringItem + "`" + pvf.Strview.GetStrText(strid, stringItem, autoConvertStr: true) + "`>");
					}
					break;
				}
				case 7:
					if (flag)
					{
						AppendSectionValue(dictionary, text, "`" + pvf.Strtable.GetStringItem(num, autoConvertStr: true) + "`");
					}
					break;
				case 6:
				case 8:
					if (flag)
					{
						AppendSectionValue(dictionary, text, "{" + b + "=`" + pvf.Strtable.GetStringItem(num, autoConvertStr: true) + "`}\r\n");
					}
					break;
				case 3:
					AppendSectionValue(dictionary, text, "{" + b.ToString() + "=" + num + "}\t");
					break;
				case 4:
					AppendSectionValue(dictionary, text, DataHelper.FormatFloat(BitConverter.ToSingle(data, i + 1)) + "\t");
					break;
				case 2:
					AppendSectionValue(dictionary, text, num + "\t");
					break;
				}
			}
		}
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, StringBuilder> item in dictionary)
		{
			dictionary2.Add(item.Key, item.Value.ToString());
			stringBuilder.AppendLine(item.Key + "\r\n" + item.Value?.ToString() + "\r\n");
		}
		return dictionary2;
	}

	private void AppendSectionValue(Dictionary<string, StringBuilder> sections, string sectionName, string valueText)
	{
		if (sectionName != null && sections.TryGetValue(sectionName, out StringBuilder value))
		{
			value.Append(valueText);
		}
	}

	public byte[] Compile(PvfFile obj, string scriptText, bool compileChinaScriptFile = false)
	{
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteByte(176);
		memoryStream.WriteByte(208);
		List<ErrorItem> list = CompileScript(obj.FileName, scriptText, false, memoryStream, compileChinaScriptFile);
		Ilogger? logger = GetLogger();
		string unknownDataFormat = logger?.GetStrNoReplace("mess_UnknownData") ?? "Unknown data: {0}";
		foreach (ErrorItem item in list)
		{
			item.Description = string.Format(unknownDataFormat, item.Input);
		}
		if (list.Count == 0)
		{
			return memoryStream.ToArray();
		}
		string compilerErrorFormat = logger?.GetStrNoReplace("mess_ScriptCompilerError") ?? "Script compiler errors: {0}";
		logger?.Error(string.Format(compilerErrorFormat, list.Count));
		logger?.Error(list);
		return null;
	}

	public (bool success, byte[] data) EncryptScriptText(string scriptText, bool readOnly, bool notShowError = false)
	{
		MemoryStream memoryStream = new MemoryStream();
		List<ErrorItem> list = CompileScript("", scriptText, readOnly, memoryStream, false);
		if (list.Count > 0 && !notShowError)
		{
			GetLogger().Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_ScriptCompilerError2"), list.Count));
			GetLogger().Error(list);
		}
		return (success: list.Count == 0, data: memoryStream.ToArray());
	}

	private static string NormalizeScriptContent(string scriptContent)
	{
		if (scriptContent == null)
		{
			return string.Empty;
		}
		try
		{
			scriptContent = Regex.Replace(scriptContent, "//[^\\r\\n]*", "\r\n");
			scriptContent = Regex.Replace(scriptContent, "(\\[name\\])<", "$1\r\n<");
			scriptContent = Regex.Replace(scriptContent, "\\]\\s", "]\t\r\n");
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			StringBuilder stringBuilder = new StringBuilder(scriptContent.Length);
			string text = scriptContent;
			foreach (char c in text)
			{
				switch (c)
				{
				case '[':
					flag = true;
					flag3 = false;
					stringBuilder.Append(c);
					continue;
				case ']':
					flag = false;
					flag3 = false;
					stringBuilder.Append(c);
					continue;
				case '`':
					flag2 = !flag2;
					flag3 = false;
					stringBuilder.Append(c);
					continue;
				}
				if (!flag && !flag2)
				{
					switch (c)
					{
					case ' ':
						if (!flag3)
						{
							stringBuilder.Append('\t');
							flag3 = true;
						}
						break;
					case '\t':
						if (!flag3)
						{
							stringBuilder.Append('\t');
							flag3 = true;
						}
						break;
					default:
						stringBuilder.Append(c);
						flag3 = false;
						break;
					}
				}
				else
				{
					stringBuilder.Append(c);
					if (c == '\n' || c == '\r')
					{
						flag3 = false;
					}
				}
			}
			string[] array = stringBuilder.ToString().Split(new string[3]
			{
				"\r\n",
				"\n",
				"\r"
			}, StringSplitOptions.None);
			StringBuilder normalizedContent = new StringBuilder();
			string[] array2 = array;
			foreach (string value in array2)
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					normalizedContent.AppendLine(value);
				}
			}
			return normalizedContent.ToString();
		}
		catch (Exception ex)
		{
			throw new Exception("空洞HnadScriptContent error:" + ex.Message, ex);
		}
	}

	private List<ErrorItem> CompileScript(string fileName, string scriptText, bool readOnly, Stream output, bool compileChinaScriptFile)
	{
		scriptText = new Regex("<(\\d+::.+?)`.+?`>").Replace(scriptText, "<$1``>");
		string[] array = scriptText.Split(new string[2]
		{
			"\r\n",
			"\n"
		}, StringSplitOptions.RemoveEmptyEntries).ToArray();
		int num = array.Length;
		List<ErrorItem> list = new List<ErrorItem>();
		string text = "";
		for (int i = 0; i < num; i++)
		{
			string text2 = array[i].ToLower();
			if (text2 == "#pvf_file" || text2 == "#pvf_file_add" || array[i] == "")
			{
				continue;
			}
			string[] array2 = array[i].Split(new string[1] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text3 in array2)
			{
				text += text3;
				byte b;
				byte[] buffer;
				if (readOnly)
				{
					(b, buffer) = ParseReadOnlyToken(text);
				}
				else if (compileChinaScriptFile)
				{
					(b, buffer) = ParseChinaToken(text);
				}
				else
				{
					(b, buffer) = ParseToken(text);
				}
				switch (b)
				{
				case 10:
					if (compileChinaScriptFile)
					{
						WriteChinaStringLink(text, output, readOnly);
					}
					else
					{
						WriteStringLink(text, output, readOnly);
					}
					text = "";
					break;
				case 81:
					text += "\r\n";
					break;
				case byte.MaxValue:
					list.Add(new ErrorItem(text, i + 1, fileName));
					text = "";
					break;
				default:
					output.WriteByte(b);
					output.Write(buffer, 0, 4);
					text = "";
					break;
				case 0:
					break;
				}
			}
		}
		return list;
	}

	private (byte, byte[]) ParseToken(string token)
	{
		char c = token[0];
		byte result;
		byte[] bytes;
		if ((uint)c <= 91u)
		{
			if (c != '<')
			{
				if (c != '[' || token[token.Length - 1] != ']')
				{
					goto IL_0308;
				}
				result = 5;
				int stringTableId = pvf.Strtable.GetStringTableId(token);
				bytes = BitConverter.GetBytes((uint)((stringTableId == -1) ? pvf.Strtable.AddStringItem(token) : stringTableId));
			}
			else
			{
				if (token[token.Length - 1] != '>')
				{
					goto IL_0308;
				}
				result = 10;
				bytes = BitConverter.GetBytes(0);
			}
		}
		else if (c != '`')
		{
			if (c != '{' || token[token.Length - 1] != '}')
			{
				goto IL_0308;
			}
			string dataFromFormat = DataHelper.GetDataFromFormat(token, "{", "=");
			string valueText = DataHelper.GetDataFromFormat(token, "=", "}");
			byte.TryParse(dataFromFormat, out result);
			bytes = BitConverter.GetBytes(0);
			if (result == 0)
			{
				return (result, bytes);
			}
			if (valueText[0] != '`' || valueText[valueText.Length - 1] != '`')
			{
				int result2;
				bool num = int.TryParse(valueText, out result2);
				bytes = BitConverter.GetBytes(result2);
				if (!num)
				{
					result = byte.MaxValue;
				}
			}
			else
			{
				string stringValue = DataHelper.GetDataFromFormat(valueText, "`", "`");
				if (string.IsNullOrEmpty(stringValue))
				{
					stringValue = " ";
				}
				int stringTableId = pvf.Strtable.GetStringTableId(stringValue);
				bytes = BitConverter.GetBytes((uint)((stringTableId != -1) ? stringTableId : pvf.Strtable.AddStringItem(stringValue)));
			}
		}
		else if (token[token.Length - 1] == '`')
		{
			result = 7;
			string stringValue = DataHelper.GetDataFromFormat(token, "`", "`");
			int stringTableId = pvf.Strtable.GetStringTableId(stringValue);
			bytes = BitConverter.GetBytes((uint)((stringTableId != -1) ? stringTableId : pvf.Strtable.AddStringItem(stringValue)));
		}
		else
		{
			result = 81;
			bytes = BitConverter.GetBytes(0);
		}
		goto IL_0355;
		IL_0355:
		return (result, bytes);
		IL_0308:
		if (token.IndexOf('.') < 0)
		{
			result = 2;
			int result3;
			bool num2 = int.TryParse(token, out result3);
			bytes = BitConverter.GetBytes(result3);
			if (!num2)
			{
				result = byte.MaxValue;
			}
		}
		else
		{
			result = 4;
			float result4;
			bool num3 = float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out result4);
			bytes = BitConverter.GetBytes(result4);
			if (!num3)
			{
				result = byte.MaxValue;
			}
		}
		goto IL_0355;
	}

	private (byte, byte[]) ParseReadOnlyToken(string token)
	{
		byte result = byte.MaxValue;
		byte[] bytes = BitConverter.GetBytes(0);
		if (string.IsNullOrEmpty(token))
		{
			return (result, bytes);
		}
		char c = token[0];
		if ((uint)c <= 91u)
		{
			if (c != '<')
			{
				if (c != '[' || token[token.Length - 1] != ']')
				{
					goto IL_02a1;
				}
				int stringTableId = pvf.Strtable.GetStringTableId(token);
				if (stringTableId == -1)
				{
					return (result, bytes);
				}
				result = 5;
				bytes = BitConverter.GetBytes((uint)stringTableId);
			}
			else
			{
				if (token[token.Length - 1] != '>')
				{
					goto IL_02a1;
				}
				result = 10;
				bytes = BitConverter.GetBytes(0);
			}
		}
		else if (c != '`')
		{
			if (c != '{' || token[token.Length - 1] != '}')
			{
				goto IL_02a1;
			}
			string dataFromFormat = DataHelper.GetDataFromFormat(token, "{", "=");
			string valueText = DataHelper.GetDataFromFormat(token, "=", "}");
			byte.TryParse(dataFromFormat, out result);
			if (result == 0)
			{
				return (result, bytes);
			}
			if (valueText[0] != '`' || valueText[valueText.Length - 1] != '`')
			{
				int.TryParse(valueText, out var result2);
				bytes = BitConverter.GetBytes(result2);
			}
			else
			{
				string stringValue = DataHelper.GetDataFromFormat(valueText, "`", "`");
				int stringTableId = pvf.Strtable.GetStringTableId(stringValue);
				if (stringTableId != -1)
				{
					bytes = BitConverter.GetBytes((uint)stringTableId);
				}
			}
		}
		else
		{
			if (token[token.Length - 1] != '`')
			{
				goto IL_02a1;
			}
			string stringValue = DataHelper.GetDataFromFormat(token, "`", "`");
			int stringTableId = pvf.Strtable.GetStringTableId(stringValue);
			if (stringTableId == -1)
			{
				return (result, bytes);
			}
			result = 7;
			bytes = BitConverter.GetBytes((uint)stringTableId);
		}
		goto IL_02e4;
		IL_02a1:
		if (token.IndexOf('.') < 0)
		{
			result = 2;
			int.TryParse(token, out var result3);
			bytes = BitConverter.GetBytes(result3);
		}
		else
		{
			result = 4;
			float result4;
			bool num = float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out result4);
			bytes = BitConverter.GetBytes(result4);
			if (!num)
			{
				result = byte.MaxValue;
			}
		}
		goto IL_02e4;
		IL_02e4:
		return (result, bytes);
	}

	private void WriteStringLink(string token, Stream output, bool readOnly)
	{
		uint.TryParse(DataHelper.GetDataFromFormat(token, "<", "::"), out var result);
		string dataFromFormat = DataHelper.GetDataFromFormat(token, "::", "`");
		output.WriteByte(9);
		output.Write(BitConverter.GetBytes(result), 0, 4);
		output.WriteByte(10);
		int stringTableId = pvf.Strtable.GetStringTableId(dataFromFormat);
		if (stringTableId != -1)
		{
			output.Write(BitConverter.GetBytes((uint)stringTableId), 0, 4);
		}
		else if (!readOnly)
		{
			output.Write(BitConverter.GetBytes((uint)pvf.Strtable.AddStringItem(dataFromFormat)), 0, 4);
		}
		else
		{
			output.Write(BitConverter.GetBytes(0u), 0, 4);
		}
	}

	private void WriteChinaStringLink(string token, Stream output, bool readOnly)
	{
		uint.TryParse(DataHelper.GetDataFromFormat(token, "<", "::"), out var result);
		string dataFromFormat = DataHelper.GetDataFromFormat(token, "::", ">");
		output.WriteByte(9);
		output.Write(BitConverter.GetBytes(result), 0, 4);
		output.WriteByte(10);
		int stringTableId = pvf.Strtable.GetStringTableId(dataFromFormat);
		if (stringTableId != -1)
		{
			output.Write(BitConverter.GetBytes((uint)stringTableId), 0, 4);
		}
		else if (!readOnly)
		{
			output.Write(BitConverter.GetBytes((uint)pvf.Strtable.AddStringItem(dataFromFormat)), 0, 4);
		}
		else
		{
			output.Write(BitConverter.GetBytes(0u), 0, 4);
		}
	}

	private (byte, byte[]) ParseChinaToken(string token)
	{
		char c = token[0];
		byte result;
		byte[] bytes;
		if ((uint)c <= 91u)
		{
			if (c != '<')
			{
				if (c != '[' || token[token.Length - 1] != ']')
				{
					goto IL_0380;
				}
				result = 5;
				int stringTableId = pvf.Strtable.GetStringTableId(token);
				bytes = BitConverter.GetBytes((uint)((stringTableId == -1) ? pvf.Strtable.AddStringItem(token) : stringTableId));
			}
			else
			{
				if (token[token.Length - 1] != '>')
				{
					goto IL_0380;
				}
				result = 10;
				bytes = BitConverter.GetBytes(0);
			}
		}
		else if (c != '`')
		{
			if (c != '{' || token[token.Length - 1] != '}')
			{
				goto IL_0380;
			}
			string dataFromFormat = DataHelper.GetDataFromFormat(token, "{", "=");
			string valueText = DataHelper.GetDataFromFormat(token, "=", "}");
			byte.TryParse(dataFromFormat, out result);
			bytes = BitConverter.GetBytes(0);
			if (result == 0)
			{
				return (result, bytes);
			}
			if (valueText[0] != '`' || valueText[valueText.Length - 1] != '`')
			{
				int result2;
				bool num = int.TryParse(valueText, out result2);
				bytes = BitConverter.GetBytes(result2);
				if (!num)
				{
					result = byte.MaxValue;
				}
			}
			else
			{
				string stringValue = DataHelper.GetDataFromFormat(valueText, "`", "`");
				if (string.IsNullOrEmpty(stringValue))
				{
					stringValue = " ";
				}
				int stringTableId = pvf.Strtable.GetStringTableId(stringValue);
				bytes = BitConverter.GetBytes((uint)((stringTableId != -1) ? stringTableId : pvf.Strtable.AddStringItem(stringValue)));
			}
		}
		else if (token[token.Length - 1] == '`')
		{
			if (token.Length > 4 && token[1] == '<' && token[token.Length - 2] == '>')
			{
				token = token.Remove(token.Length - 2, 2).Remove(0, 2);
				result = 7;
				int stringTableId = pvf.Strtable.GetStringTableId(token);
				bytes = BitConverter.GetBytes((uint)((stringTableId != -1) ? stringTableId : pvf.Strtable.AddStringItem(token)));
			}
			else
			{
				result = 7;
				string stringValue = DataHelper.GetDataFromFormat(token, "`", "`");
				int stringTableId = pvf.Strtable.GetStringTableId(stringValue);
				bytes = BitConverter.GetBytes((uint)((stringTableId != -1) ? stringTableId : pvf.Strtable.AddStringItem(stringValue)));
			}
		}
		else
		{
			result = 81;
			bytes = BitConverter.GetBytes(0);
		}
		goto IL_03cd;
		IL_0380:
		if (token.IndexOf('.') < 0)
		{
			result = 2;
			int result3;
			bool num2 = int.TryParse(token, out result3);
			bytes = BitConverter.GetBytes(result3);
			if (!num2)
			{
				result = byte.MaxValue;
			}
		}
		else
		{
			result = 4;
			float result4;
			bool num3 = float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out result4);
			bytes = BitConverter.GetBytes(result4);
			if (!num3)
			{
				result = byte.MaxValue;
			}
		}
		goto IL_03cd;
		IL_03cd:
		return (result, bytes);
	}
}
