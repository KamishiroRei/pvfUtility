using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf;

namespace PvfCode.Services;

public class ScriptFileCompilerOl
{
	private Ilogger rffImLL3WH;

	private readonly PvfGroup FxcI41eDZ0;

	[SpecialName]
	private Ilogger jk8IFnVWb6()
	{
		if (rffImLL3WH == null)
		{
			rffImLL3WH = AppSetting.Instance.GetService<Ilogger>();
		}
		return rffImLL3WH;
	}

	public ScriptFileCompilerOl(PvfGroup pack)
	{
		FxcI41eDZ0 = pack;
	}

	public string Decompile(PvfFile file)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder("#PVF_File\r\n");
			xvaI6aesKU(file.Data, file.DataLen, file.FileName, stringBuilder);
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
			xvaI6aesKU(scriptdata, scriptdata.Length, "", stringBuilder);
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			return AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_ReadScriptException") + "\r\n" + ex.Message;
		}
	}

	private void xvaI6aesKU(byte[] P_0, int P_1, string P_2, StringBuilder P_3)
	{
		if (P_0 != null && P_1 >= 7)
		{
			for (int i = 2; i < P_1 - 4; i += 5)
			{
				byte b = P_0[i];
				int num = BitConverter.ToInt32(P_0, i + 1);
				switch (b)
				{
				case 5:
				{
					StringBuilder stringBuilder = P_3;
					StringBuilder stringBuilder3 = stringBuilder;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(4, 1, stringBuilder);
					handler.AppendLiteral("\r\n");
					handler.AppendFormatted(FxcI41eDZ0.Strtable.GetStringItem(num));
					handler.AppendLiteral("\r\n");
					stringBuilder3.Append(ref handler);
					break;
				}
				case 10:
				{
					int strid = BitConverter.ToInt32(P_0, i - 4);
					string stringItem = FxcI41eDZ0.Strtable.GetStringItem(num);
					if (AppSetting.Instance.PvfConfig.AutoConvertStringLink)
					{
						P_3.Append("`" + FxcI41eDZ0.Strview.GetStrText(strid, stringItem, autoConvertStr: true).Replace("\\n", "\r\n") + "`\r\n");
						break;
					}
					P_3.Append("<" + strid + "::" + stringItem + "`" + FxcI41eDZ0.Strview.GetStrText(strid, stringItem, autoConvertStr: true) + "`>\r\n");
					break;
				}
				case 7:
				{
					StringBuilder stringBuilder = P_3;
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(4, 1, stringBuilder);
					handler.AppendLiteral("`");
					handler.AppendFormatted(FxcI41eDZ0.Strtable.GetStringItem(num, autoConvertStr: true));
					handler.AppendLiteral("`\r\n");
					stringBuilder2.Append(ref handler);
					break;
				}
				case 6:
				case 8:
					P_3.Append("{" + b + "=`" + FxcI41eDZ0.Strtable.GetStringItem(num, autoConvertStr: true) + "`}\r\n");
					break;
				case 3:
					P_3.Append("{" + b.ToString() + "=" + num + "}\t");
					break;
				case 4:
					P_3.Append(DataHelper.FormatFloat(BitConverter.ToSingle(P_0, i + 1)) + "\t");
					break;
				case 2:
					P_3.Append(num + "\t");
					break;
				}
			}
		}
		P_3.Append("\r\n");
	}

	public Dictionary<string, string> DecompileDic(PvfFile file)
	{
		return z5TI2BW9YD(file.Data, file.DataLen);
	}

	private Dictionary<string, string> z5TI2BW9YD(byte[] P_0, int P_1)
	{
		Dictionary<string, StringBuilder> dictionary = new Dictionary<string, StringBuilder>();
		bool flag = false;
		string text = null;
		if (P_0 != null && P_1 >= 7)
		{
			for (int i = 2; i < P_1 - 4; i += 5)
			{
				byte b = P_0[i];
				int num = BitConverter.ToInt32(P_0, i + 1);
				switch (b)
				{
				case 5:
				{
					string stringItem2 = FxcI41eDZ0.Strtable.GetStringItem(num);
					if (stringItem2.Length > 2)
					{
						if (stringItem2[0] == '[' && stringItem2[1] == '/')
						{
							flag = false;
							text = null;
						}
						else if (stringItem2[0] == '[')
						{
							flag = true;
							if (!dictionary.ContainsKey(stringItem2))
							{
								dictionary.Add(stringItem2, new StringBuilder());
							}
							text = stringItem2;
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
					int strid = BitConverter.ToInt32(P_0, i - 4);
					string stringItem = FxcI41eDZ0.Strtable.GetStringItem(num);
					if (flag)
					{
						if (AppSetting.Instance.PvfConfig.AutoConvertStringLink)
						{
							AhkIBY2Cjl(dictionary, text, "`" + FxcI41eDZ0.Strview.GetStrText(strid, stringItem, autoConvertStr: true).Replace("\\n", "\r\n") + "`");
							break;
						}
						AhkIBY2Cjl(dictionary, text, "<" + strid + "::" + stringItem + "`" + FxcI41eDZ0.Strview.GetStrText(strid, stringItem, autoConvertStr: true) + "`>");
					}
					break;
				}
				case 7:
					if (flag)
					{
						AhkIBY2Cjl(dictionary, text, "`" + FxcI41eDZ0.Strtable.GetStringItem(num, autoConvertStr: true) + "`");
					}
					break;
				case 6:
				case 8:
					if (flag)
					{
						AhkIBY2Cjl(dictionary, text, "{" + b + "=`" + FxcI41eDZ0.Strtable.GetStringItem(num, autoConvertStr: true) + "`}\r\n");
					}
					break;
				case 3:
					AhkIBY2Cjl(dictionary, text, "{" + b.ToString() + "=" + num + "}\t");
					break;
				case 4:
					AhkIBY2Cjl(dictionary, text, DataHelper.FormatFloat(BitConverter.ToSingle(P_0, i + 1)) + "\t");
					break;
				case 2:
					AhkIBY2Cjl(dictionary, text, num + "\t");
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

	private void AhkIBY2Cjl(Dictionary<string, StringBuilder> P_0, string P_1, string P_2)
	{
		if (P_1 != null && P_0.TryGetValue(P_1, out StringBuilder value))
		{
			value.Append(P_2);
		}
	}

	public byte[] Compile(PvfFile obj, string scriptText, bool compileChinaScriptFile = false)
	{
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteByte(176);
		memoryStream.WriteByte(208);
		List<ErrorItem> list = lmvIvCLGxk(obj.FileName, scriptText, false, memoryStream, compileChinaScriptFile);
		List<ErrorItem> list2 = new List<ErrorItem>();
		foreach (ErrorItem item in list)
		{
			item.Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_UnknownData"), item.Input);
		}
		if (list2.Count == 0)
		{
			return memoryStream.ToArray();
		}
		jk8IFnVWb6().Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_ScriptCompilerError"), list.Count));
		jk8IFnVWb6().Error(list);
		return null;
	}

	public (bool success, byte[] data) EncryptScriptText(string scriptText, bool readOnly, bool notShowError = false)
	{
		MemoryStream memoryStream = new MemoryStream();
		List<ErrorItem> list = lmvIvCLGxk("", scriptText, readOnly, memoryStream, false);
		if (list.Count > 0 && !notShowError)
		{
			jk8IFnVWb6().Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_ScriptCompilerError2"), list.Count));
			jk8IFnVWb6().Error(list);
		}
		return (success: list.Count == 0, data: memoryStream.ToArray());
	}

	private static string jFZIUsKJfK(string P_0)
	{
		if (P_0 == null)
		{
			return string.Empty;
		}
		try
		{
			P_0 = Regex.Replace(P_0, "//[^\\r\\n]*", "\r\n");
			P_0 = Regex.Replace(P_0, "(\\[name\\])<", "$1\r\n<");
			P_0 = Regex.Replace(P_0, "\\]\\s", "]\t\r\n");
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			StringBuilder stringBuilder = new StringBuilder(P_0.Length);
			string text = P_0;
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
			StringBuilder stringBuilder2 = new StringBuilder();
			string[] array2 = array;
			foreach (string value in array2)
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					stringBuilder2.AppendLine(value);
				}
			}
			return stringBuilder2.ToString();
		}
		catch (Exception ex)
		{
			throw new Exception("空洞HnadScriptContent error:" + ex.Message, ex);
		}
	}

	private List<ErrorItem> lmvIvCLGxk(string P_0, string P_1, bool P_2, Stream P_3, bool P_4)
	{
		P_1 = new Regex("<(\\d+::.+?)`.+?`>").Replace(P_1, "<$1``>");
		string[] array = P_1.Split(new string[2]
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
				if (P_2)
				{
					(b, buffer) = T4nIifindq(text);
				}
				else if (P_4)
				{
					(b, buffer) = MhcIXQhNUJ(text);
				}
				else
				{
					(b, buffer) = zh1IWMZuxg(text);
				}
				switch (b)
				{
				case 10:
					if (P_4)
					{
						FVgIo2MPim(text, P_3, P_2);
					}
					else
					{
						FKPItt2lX3(text, P_3, P_2);
					}
					text = "";
					break;
				case 81:
					text += "\r\n";
					break;
				case byte.MaxValue:
					list.Add(new ErrorItem(text, i + 1, P_0));
					text = "";
					break;
				default:
					P_3.WriteByte(b);
					P_3.Write(buffer, 0, 4);
					text = "";
					break;
				case 0:
					break;
				}
			}
		}
		return list;
	}

	private (byte, byte[]) zh1IWMZuxg(string P_0)
	{
		char c = P_0[0];
		byte result;
		byte[] bytes;
		if ((uint)c <= 91u)
		{
			if (c != '<')
			{
				if (c != '[' || P_0[P_0.Length - 1] != ']')
				{
					goto IL_0308;
				}
				result = 5;
				int stringTableId = FxcI41eDZ0.Strtable.GetStringTableId(P_0);
				bytes = BitConverter.GetBytes((uint)((stringTableId == -1) ? FxcI41eDZ0.Strtable.AddStringItem(P_0) : stringTableId));
			}
			else
			{
				if (P_0[P_0.Length - 1] != '>')
				{
					goto IL_0308;
				}
				result = 10;
				bytes = BitConverter.GetBytes(0);
			}
		}
		else if (c != '`')
		{
			if (c != '{' || P_0[P_0.Length - 1] != '}')
			{
				goto IL_0308;
			}
			string dataFromFormat = DataHelper.GetDataFromFormat(P_0, "{", "=");
			string dataFromFormat2 = DataHelper.GetDataFromFormat(P_0, "=", "}");
			byte.TryParse(dataFromFormat, out result);
			bytes = BitConverter.GetBytes(0);
			if (result == 0)
			{
				return (result, bytes);
			}
			if (dataFromFormat2[0] != '`' || dataFromFormat2[dataFromFormat2.Length - 1] != '`')
			{
				int result2;
				bool num = int.TryParse(dataFromFormat2, out result2);
				bytes = BitConverter.GetBytes(result2);
				if (!num)
				{
					result = byte.MaxValue;
				}
			}
			else
			{
				string text = DataHelper.GetDataFromFormat(dataFromFormat2, "`", "`");
				if (string.IsNullOrEmpty(text))
				{
					text = " ";
				}
				int stringTableId2 = FxcI41eDZ0.Strtable.GetStringTableId(text);
				bytes = BitConverter.GetBytes((uint)((stringTableId2 != -1) ? stringTableId2 : FxcI41eDZ0.Strtable.AddStringItem(text)));
			}
		}
		else if (P_0[P_0.Length - 1] == '`')
		{
			result = 7;
			string dataFromFormat3 = DataHelper.GetDataFromFormat(P_0, "`", "`");
			int stringTableId3 = FxcI41eDZ0.Strtable.GetStringTableId(dataFromFormat3);
			bytes = BitConverter.GetBytes((uint)((stringTableId3 != -1) ? stringTableId3 : FxcI41eDZ0.Strtable.AddStringItem(dataFromFormat3)));
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
		if (P_0.IndexOf('.') < 0)
		{
			result = 2;
			int result3;
			bool num2 = int.TryParse(P_0, out result3);
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
			bool num3 = float.TryParse(P_0, out result4);
			bytes = BitConverter.GetBytes(result4);
			if (!num3)
			{
				result = byte.MaxValue;
			}
		}
		goto IL_0355;
	}

	private (byte, byte[]) T4nIifindq(string P_0)
	{
		byte result = byte.MaxValue;
		byte[] bytes = BitConverter.GetBytes(0);
		if (string.IsNullOrEmpty(P_0))
		{
			return (result, bytes);
		}
		char c = P_0[0];
		if ((uint)c <= 91u)
		{
			if (c != '<')
			{
				if (c != '[' || P_0[P_0.Length - 1] != ']')
				{
					goto IL_02a1;
				}
				int stringTableId = FxcI41eDZ0.Strtable.GetStringTableId(P_0);
				if (stringTableId == -1)
				{
					return (result, bytes);
				}
				result = 5;
				bytes = BitConverter.GetBytes((uint)stringTableId);
			}
			else
			{
				if (P_0[P_0.Length - 1] != '>')
				{
					goto IL_02a1;
				}
				result = 10;
				bytes = BitConverter.GetBytes(0);
			}
		}
		else if (c != '`')
		{
			if (c != '{' || P_0[P_0.Length - 1] != '}')
			{
				goto IL_02a1;
			}
			string dataFromFormat = DataHelper.GetDataFromFormat(P_0, "{", "=");
			string dataFromFormat2 = DataHelper.GetDataFromFormat(P_0, "=", "}");
			byte.TryParse(dataFromFormat, out result);
			if (result == 0)
			{
				return (result, bytes);
			}
			if (dataFromFormat2[0] != '`' || dataFromFormat2[dataFromFormat2.Length - 1] != '`')
			{
				int.TryParse(dataFromFormat2, out var result2);
				bytes = BitConverter.GetBytes(result2);
			}
			else
			{
				string dataFromFormat3 = DataHelper.GetDataFromFormat(dataFromFormat2, "`", "`");
				int stringTableId2 = FxcI41eDZ0.Strtable.GetStringTableId(dataFromFormat3);
				if (stringTableId2 != -1)
				{
					bytes = BitConverter.GetBytes((uint)stringTableId2);
				}
			}
		}
		else
		{
			if (P_0[P_0.Length - 1] != '`')
			{
				goto IL_02a1;
			}
			string dataFromFormat4 = DataHelper.GetDataFromFormat(P_0, "`", "`");
			int stringTableId3 = FxcI41eDZ0.Strtable.GetStringTableId(dataFromFormat4);
			if (stringTableId3 == -1)
			{
				return (result, bytes);
			}
			result = 7;
			bytes = BitConverter.GetBytes((uint)stringTableId3);
		}
		goto IL_02e4;
		IL_02a1:
		if (P_0.IndexOf('.') < 0)
		{
			result = 2;
			int.TryParse(P_0, out var result3);
			bytes = BitConverter.GetBytes(result3);
		}
		else
		{
			result = 4;
			float result4;
			bool num = float.TryParse(P_0, out result4);
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

	private void FKPItt2lX3(string P_0, Stream P_1, bool P_2)
	{
		uint.TryParse(DataHelper.GetDataFromFormat(P_0, "<", "::"), out var result);
		string dataFromFormat = DataHelper.GetDataFromFormat(P_0, "::", "`");
		P_1.WriteByte(9);
		P_1.Write(BitConverter.GetBytes(result), 0, 4);
		P_1.WriteByte(10);
		int stringTableId = FxcI41eDZ0.Strtable.GetStringTableId(dataFromFormat);
		if (stringTableId != -1)
		{
			P_1.Write(BitConverter.GetBytes((uint)stringTableId), 0, 4);
		}
		else if (!P_2)
		{
			P_1.Write(BitConverter.GetBytes((uint)FxcI41eDZ0.Strtable.AddStringItem(dataFromFormat)), 0, 4);
		}
		else
		{
			P_1.Write(BitConverter.GetBytes(0u), 0, 4);
		}
	}

	private void FVgIo2MPim(string P_0, Stream P_1, bool P_2)
	{
		uint.TryParse(DataHelper.GetDataFromFormat(P_0, "<", "::"), out var result);
		string dataFromFormat = DataHelper.GetDataFromFormat(P_0, "::", ">");
		P_1.WriteByte(9);
		P_1.Write(BitConverter.GetBytes(result), 0, 4);
		P_1.WriteByte(10);
		int stringTableId = FxcI41eDZ0.Strtable.GetStringTableId(dataFromFormat);
		if (stringTableId != -1)
		{
			P_1.Write(BitConverter.GetBytes((uint)stringTableId), 0, 4);
		}
		else if (!P_2)
		{
			P_1.Write(BitConverter.GetBytes((uint)FxcI41eDZ0.Strtable.AddStringItem(dataFromFormat)), 0, 4);
		}
		else
		{
			P_1.Write(BitConverter.GetBytes(0u), 0, 4);
		}
	}

	private (byte, byte[]) MhcIXQhNUJ(string P_0)
	{
		char c = P_0[0];
		byte result;
		byte[] bytes;
		if ((uint)c <= 91u)
		{
			if (c != '<')
			{
				if (c != '[' || P_0[P_0.Length - 1] != ']')
				{
					goto IL_0380;
				}
				result = 5;
				int stringTableId = FxcI41eDZ0.Strtable.GetStringTableId(P_0);
				bytes = BitConverter.GetBytes((uint)((stringTableId == -1) ? FxcI41eDZ0.Strtable.AddStringItem(P_0) : stringTableId));
			}
			else
			{
				if (P_0[P_0.Length - 1] != '>')
				{
					goto IL_0380;
				}
				result = 10;
				bytes = BitConverter.GetBytes(0);
			}
		}
		else if (c != '`')
		{
			if (c != '{' || P_0[P_0.Length - 1] != '}')
			{
				goto IL_0380;
			}
			string dataFromFormat = DataHelper.GetDataFromFormat(P_0, "{", "=");
			string dataFromFormat2 = DataHelper.GetDataFromFormat(P_0, "=", "}");
			byte.TryParse(dataFromFormat, out result);
			bytes = BitConverter.GetBytes(0);
			if (result == 0)
			{
				return (result, bytes);
			}
			if (dataFromFormat2[0] != '`' || dataFromFormat2[dataFromFormat2.Length - 1] != '`')
			{
				int result2;
				bool num = int.TryParse(dataFromFormat2, out result2);
				bytes = BitConverter.GetBytes(result2);
				if (!num)
				{
					result = byte.MaxValue;
				}
			}
			else
			{
				string text = DataHelper.GetDataFromFormat(dataFromFormat2, "`", "`");
				if (string.IsNullOrEmpty(text))
				{
					text = " ";
				}
				int stringTableId2 = FxcI41eDZ0.Strtable.GetStringTableId(text);
				bytes = BitConverter.GetBytes((uint)((stringTableId2 != -1) ? stringTableId2 : FxcI41eDZ0.Strtable.AddStringItem(text)));
			}
		}
		else if (P_0[P_0.Length - 1] == '`')
		{
			if (P_0.Length > 4 && P_0[1] == '<' && P_0[P_0.Length - 2] == '>')
			{
				P_0 = P_0.Remove(P_0.Length - 2, 2).Remove(0, 2);
				result = 7;
				int stringTableId3 = FxcI41eDZ0.Strtable.GetStringTableId(P_0);
				bytes = BitConverter.GetBytes((uint)((stringTableId3 != -1) ? stringTableId3 : FxcI41eDZ0.Strtable.AddStringItem(P_0)));
			}
			else
			{
				result = 7;
				string dataFromFormat3 = DataHelper.GetDataFromFormat(P_0, "`", "`");
				int stringTableId4 = FxcI41eDZ0.Strtable.GetStringTableId(dataFromFormat3);
				bytes = BitConverter.GetBytes((uint)((stringTableId4 != -1) ? stringTableId4 : FxcI41eDZ0.Strtable.AddStringItem(dataFromFormat3)));
			}
		}
		else
		{
			result = 81;
			bytes = BitConverter.GetBytes(0);
		}
		goto IL_03cd;
		IL_0380:
		if (P_0.IndexOf('.') < 0)
		{
			result = 2;
			int result3;
			bool num2 = int.TryParse(P_0, out result3);
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
			bool num3 = float.TryParse(P_0, out result4);
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
