using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Collections.Pooled;
using PvfCode.Dot;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf;
using PvfCode.NPK.Utils.AniModel;
using PvfCode.NPK.Utils.AniModel.Enums;
using PvfCode.Services.PvfParsingNew.EditorPrivew;
using WinCopies.Util;

namespace PvfCode.Services;

public static class BinaryAniCompiler
{
	private class ffP2dGOSCUMNUS9ggmI
	{
		[CompilerGenerated]
		private string KQC0ueRWoG;

		[CompilerGenerated]
		private List<string> mmc0I78JSr;

		[CompilerGenerated]
		private List<ffP2dGOSCUMNUS9ggmI> wUF0e1WfCV;

		[CompilerGenerated]
		private bool bC70CmZIbM;

		public string SectionName
		{
			[CompilerGenerated]
			get
			{
				return KQC0ueRWoG;
			}
			[CompilerGenerated]
			set
			{
				KQC0ueRWoG = value;
			}
		}

		public ffP2dGOSCUMNUS9ggmI()
		{
			eEtOd4gXr1(new List<ffP2dGOSCUMNUS9ggmI>());
			qUTOTdL0vu(new List<string>());
		}

		[SpecialName]
		[CompilerGenerated]
		public List<string> TtJO7DF1PT()
		{
			return mmc0I78JSr;
		}

		[SpecialName]
		[CompilerGenerated]
		public void qUTOTdL0vu(List<string> P_0)
		{
			mmc0I78JSr = P_0;
		}

		[SpecialName]
		[CompilerGenerated]
		public List<ffP2dGOSCUMNUS9ggmI> BxiOhMIJuP()
		{
			return wUF0e1WfCV;
		}

		[SpecialName]
		[CompilerGenerated]
		public void eEtOd4gXr1(List<ffP2dGOSCUMNUS9ggmI> P_0)
		{
			wUF0e1WfCV = P_0;
		}

		[SpecialName]
		[CompilerGenerated]
		public bool tldOqUdS1c()
		{
			return bC70CmZIbM;
		}

		[SpecialName]
		[CompilerGenerated]
		public void mIwOgnH2HO(bool P_0)
		{
			bC70CmZIbM = P_0;
		}

		public ResultData uMtObuuZ1N(StringBuilder P_0, List<string> P_1)
		{
			ResultData resultData = new ResultData();
			if (tldOqUdS1c())
			{
				P_0.AppendLine(SectionName);
				foreach (ffP2dGOSCUMNUS9ggmI item in BxiOhMIJuP())
				{
					ResultData resultData2 = item.uMtObuuZ1N(P_0, P_1);
					if (resultData2.IsError)
					{
						return resultData2;
					}
				}
			}
			else if (AppSetting.Instance.PvfConfig.AniSectionNames.Contains(SectionName))
			{
				if (SectionName == "[IMAGE POS]")
				{
					P_0.AppendLine(SectionName);
					List<int> list = new List<int>();
					foreach (string item2 in TtJO7DF1PT())
					{
						if (double.TryParse(item2, out var result))
						{
							try
							{
								list.Add((int)result);
							}
							catch (Exception)
							{
								resultData.Msg = "转换失败 [IMAGE POS] 下的值：" + item2 + "不正确";
								return resultData;
							}
							continue;
						}
						resultData.Msg = "转换失败 [IMAGE POS] 下的值：" + item2 + "不正确";
						return resultData;
					}
					if (list.Count != 2)
					{
						resultData.Msg = "转换失败 [IMAGE POS] 下的值应为2个一组";
						return resultData;
					}
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, P_0);
					handler.AppendFormatted(list[0].ToString());
					handler.AppendLiteral("\t");
					handler.AppendFormatted(list[1].ToString());
					P_0.AppendLine(ref handler);
				}
				else
				{
					P_0.AppendLine(SectionName);
					P_0.AppendLine(string.Join("\r\n", TtJO7DF1PT()));
				}
			}
			else if (SectionName == "[IMAGE EX]")
			{
				if (TtJO7DF1PT().Count == 0)
				{
					resultData.Msg = "[IMAGE EX]下的值不正确 应为 2个一组";
					return resultData;
				}
				P_0.AppendLine("[IMAGE]");
				if (TtJO7DF1PT()[0] == "-1")
				{
					P_0.AppendLine("``");
					P_0.AppendLine("0");
					return resultData;
				}
				if (TtJO7DF1PT().Count != 2)
				{
					resultData.Msg = "[IMAGE EX]下的值不正确 应为 2个一组";
					return resultData;
				}
				string text = TtJO7DF1PT()[0];
				ResultData resultData3 = A2EOVE8Wky(P_1, text, P_0);
				if (resultData3.IsError)
				{
					return resultData3;
				}
				P_0.AppendLine((int.TryParse(TtJO7DF1PT()[1], out var result2) ? result2 : 0).ToString());
			}
			return resultData;
		}

		private ResultData A2EOVE8Wky(List<string> P_0, string P_1, StringBuilder P_2)
		{
			ResultData resultData = new ResultData();
			if (!int.TryParse(P_1, out var result))
			{
				resultData.Msg = "[IMAGE EX]下的Img路径索引编号不是Int32请检查";
				return resultData;
			}
			if (result >= P_0.Count)
			{
				resultData.Msg = "[IMAGE EX]下的Img路径索引编号大于实际路径数量";
				return resultData;
			}
			P_2.AppendLine(P_0[result]);
			return resultData;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass9_0
	{
		public string aCj0PFm0jW;

		public _003C_003Ec__DisplayClass9_0()
		{
		}

		internal bool bCs03BYHgG(string item)
		{
			return item == aCj0PFm0jW;
		}
	}

	private static Ilogger dPVIwwJ1uu;

	[SpecialName]
	private static Ilogger PVUIL9AWaA()
	{
		if (dPVIwwJ1uu == null)
		{
			dPVIwwJ1uu = AppSetting.Instance.GetService<Ilogger>();
		}
		return dPVIwwJ1uu;
	}

	public static bool FileTextConvertAniFile(string text, string fileName, out AniFile anifile)
	{
		(bool, byte[], ErrorItem) tuple = CompileBinaryAni(text, fileName);
		if (!tuple.Item1)
		{
			anifile = new AniFile();
			PVUIL9AWaA().Error(new List<ErrorItem> { tuple.Item3 });
			return false;
		}
		if (tuple.Item2 == null || tuple.Item2.Length == 0)
		{
			anifile = new AniFile();
			return true;
		}
		return DecompileAniToModel(tuple.Item2, fileName, out anifile);
	}

	public static (bool success, string text) DecompileBinaryAni(PvfFile file)
	{
		if (file.Data == null || file.DataLen <= 0)
		{
			return (success: true, text: "");
		}
		return DecompileBinaryAni(file.Data, file.FileName);
	}

	public static (bool success, string text) DecompileBinaryAni(byte[] fileData, string fileName)
	{
		try
		{
			List<string> list = new List<string>();
			MemoryStream memoryStream = new MemoryStream(fileData);
			StringBuilder stringBuilder = new StringBuilder("#PVF_File\r\n\r\n");
			ushort num = LXiIx7GhpZ(memoryStream);
			ushort num2 = LXiIx7GhpZ(memoryStream);
			for (int i = 0; i < num2; i++)
			{
				list.Add(ocNIpPpqEv(TiAIswrodh(memoryStream), memoryStream));
			}
			ushort num3 = LXiIx7GhpZ(memoryStream);
			StringBuilder stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler;
			for (int j = 0; j < num3; j++)
			{
				ushort num4 = LXiIx7GhpZ(memoryStream);
				switch (num4)
				{
				case 0:
				case 1:
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder4 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(7, 2, stringBuilder2);
					handler.AppendLiteral("[");
					handler.AppendFormatted((ANIData)num4);
					handler.AppendLiteral("]\r\n\t");
					handler.AppendFormatted(zQ6IKfMB6v(memoryStream));
					handler.AppendLiteral("\r\n");
					stringBuilder4.Append(ref handler);
					break;
				}
				case 3:
				case 28:
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder3 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(7, 2, stringBuilder2);
					handler.AppendLiteral("[");
					handler.AppendFormatted((ANIData)num4);
					handler.AppendLiteral("]\r\n\t");
					handler.AppendFormatted(LXiIx7GhpZ(memoryStream));
					handler.AppendLiteral("\r\n");
					stringBuilder3.Append(ref handler);
					break;
				}
				case 18:
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder5 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(13, 1, stringBuilder2);
					handler.AppendLiteral("[SPECTRUM]\r\n\t");
					handler.AppendFormatted(zQ6IKfMB6v(memoryStream));
					stringBuilder5.Append(ref handler);
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder6 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(22, 1, stringBuilder2);
					handler.AppendLiteral("\r\n\t[SPECTRUM TERM]\r\n\t\t");
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					stringBuilder6.Append(ref handler);
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder7 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(27, 1, stringBuilder2);
					handler.AppendLiteral("\r\n\t[SPECTRUM LIFE TIME]\r\n\t\t");
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					stringBuilder7.Append(ref handler);
					stringBuilder.Append("\r\n\t[SPECTRUM COLOR]\r\n\t\t");
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder8 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(5, 4, stringBuilder2);
					handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
					handler.AppendLiteral("\r\n");
					stringBuilder8.Append(ref handler);
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder9 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(26, 1, stringBuilder2);
					handler.AppendLiteral("\t[SPECTRUM EFFECT]\r\n\t\t`");
					handler.AppendFormatted((Effect_Item)LXiIx7GhpZ(memoryStream));
					handler.AppendLiteral("`\r\n");
					stringBuilder9.Append(ref handler);
					break;
				}
				default:
				{
					ErrorItem item = new ErrorItem("", 0, fileName)
					{
						Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_BinaryAniReadGlobalError"), memoryStream.Position)
					};
					PVUIL9AWaA().Error(new List<ErrorItem> { item });
					return (success: false, text: string.Empty);
				}
				}
			}
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder10 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(16, 1, stringBuilder2);
			handler.AppendLiteral("[FRAME MAX]\r\n\t");
			handler.AppendFormatted(num);
			handler.AppendLiteral("\r\n");
			stringBuilder10.Append(ref handler);
			for (int k = 0; k < num; k++)
			{
				stringBuilder.Append("\r\n[FRAME" + k.ToString("D3") + "]\r\n");
				ushort num5 = LXiIx7GhpZ(memoryStream);
				StringBuilder stringBuilder11 = new StringBuilder();
				for (int l = 0; l < num5; l++)
				{
					switch (LXiIx7GhpZ(memoryStream))
					{
					case 15:
						stringBuilder11.Append("\t[ATTACK BOX]\r\n\t");
						break;
					case 14:
						stringBuilder11.Append("\t[DAMAGE BOX]\r\n\t");
						break;
					default:
						PVUIL9AWaA().Error(new List<ErrorItem>
						{
							new ErrorItem("", 0, fileName)
							{
								Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_BinaryAniReadFrameError"), k, memoryStream.Position)
							}
						});
						return (success: false, text: string.Empty);
					}
					stringBuilder2 = stringBuilder11;
					StringBuilder stringBuilder12 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(7, 6, stringBuilder2);
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					handler.AppendLiteral("\r\n");
					stringBuilder12.Append(ref handler);
				}
				stringBuilder.Append("\t[IMAGE]\r\n");
				int num6 = PSfIcBUNmT(memoryStream);
				if (num6 >= 0)
				{
					if (num6 > list.Count - 1)
					{
						PVUIL9AWaA().Error(new List<ErrorItem>
						{
							new ErrorItem("", 0, fileName)
							{
								Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadFrameImageError"), k, memoryStream.Position)
							}
						});
						return (success: false, text: string.Empty);
					}
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder13 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(10, 2, stringBuilder2);
					handler.AppendLiteral("\t\t`");
					handler.AppendFormatted(list[num6]);
					handler.AppendLiteral("`\r\n\t\t");
					handler.AppendFormatted(LXiIx7GhpZ(memoryStream));
					handler.AppendLiteral("\r\n");
					stringBuilder13.Append(ref handler);
				}
				else
				{
					stringBuilder.Append("\t\t``\r\n\t\t0\r\n");
				}
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder14 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(19, 2, stringBuilder2);
				handler.AppendLiteral("\t[IMAGE POS]\r\n\t\t");
				handler.AppendFormatted(TiAIswrodh(memoryStream));
				handler.AppendLiteral("\t");
				handler.AppendFormatted(TiAIswrodh(memoryStream));
				handler.AppendLiteral("\r\n");
				stringBuilder14.Append(ref handler);
				ushort num7 = LXiIx7GhpZ(memoryStream);
				for (int m = 0; m < num7; m++)
				{
					ushort num8 = LXiIx7GhpZ(memoryStream);
					switch (num8)
					{
					case 0:
					case 1:
					case 10:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder29 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(9, 2, stringBuilder2);
						handler.AppendLiteral("\t[");
						handler.AppendFormatted((ANIData)num8);
						handler.AppendLiteral("]\r\n\t\t");
						handler.AppendFormatted(zQ6IKfMB6v(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder29.Append(ref handler);
						break;
					}
					case 3:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder28 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(14, 1, stringBuilder2);
						handler.AppendLiteral("\t[COORD]\r\n\t\t");
						handler.AppendFormatted(LXiIx7GhpZ(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder28.Append(ref handler);
						break;
					}
					case 17:
						stringBuilder.Append("\t[PRELOAD]\r\n\t\t1\r\n");
						break;
					case 7:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder16 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(20, 2, stringBuilder2);
						handler.AppendLiteral("\t[IMAGE RATE]\r\n\t\t");
						handler.AppendFormatted(U0WI5U3YUU(memoryStream));
						handler.AppendLiteral("\t");
						handler.AppendFormatted(U0WI5U3YUU(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder16.Append(ref handler);
						break;
					}
					case 8:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder15 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(21, 1, stringBuilder2);
						handler.AppendLiteral("\t[IMAGE ROTATE]\r\n\t\t");
						handler.AppendFormatted(U0WI5U3YUU(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder15.Append(ref handler);
						break;
					}
					case 9:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder17 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(16, 4, stringBuilder2);
						handler.AppendLiteral("\t[RGBA]\r\n\t\t");
						handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
						handler.AppendLiteral("\t");
						handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
						handler.AppendLiteral("\t");
						handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
						handler.AppendLiteral("\t");
						handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder17.Append(ref handler);
						break;
					}
					case 11:
					{
						stringBuilder.Append("\t[GRAPHIC EFFECT]\r\n");
						ushort num9 = LXiIx7GhpZ(memoryStream);
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder25 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
						handler.AppendLiteral("\t\t`");
						handler.AppendFormatted((Effect_Item)num9);
						handler.AppendLiteral("`\r\n");
						stringBuilder25.Append(ref handler);
						if (num9 == 5)
						{
							stringBuilder2 = stringBuilder;
							StringBuilder stringBuilder26 = stringBuilder2;
							handler = new StringBuilder.AppendInterpolatedStringHandler(6, 3, stringBuilder2);
							handler.AppendLiteral("\t\t");
							handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
							handler.AppendLiteral("\t");
							handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
							handler.AppendLiteral("\t");
							handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
							handler.AppendLiteral("\r\n");
							stringBuilder26.Append(ref handler);
						}
						if (num9 == 6)
						{
							stringBuilder2 = stringBuilder;
							StringBuilder stringBuilder27 = stringBuilder2;
							handler = new StringBuilder.AppendInterpolatedStringHandler(5, 2, stringBuilder2);
							handler.AppendLiteral("\t\t");
							handler.AppendFormatted(PSfIcBUNmT(memoryStream));
							handler.AppendLiteral("\t");
							handler.AppendFormatted(PSfIcBUNmT(memoryStream));
							handler.AppendLiteral("\r\n");
							stringBuilder27.Append(ref handler);
						}
						break;
					}
					case 12:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder24 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(14, 1, stringBuilder2);
						handler.AppendLiteral("\t[DELAY]\r\n\t\t");
						handler.AppendFormatted(TiAIswrodh(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder24.Append(ref handler);
						break;
					}
					case 13:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder23 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(22, 1, stringBuilder2);
						handler.AppendLiteral("\t[DAMAGE TYPE]\r\n\t\t`");
						handler.AppendFormatted((DAMAGE_TYPE_Item)LXiIx7GhpZ(memoryStream));
						handler.AppendLiteral("`\r\n");
						stringBuilder23.Append(ref handler);
						break;
					}
					case 16:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder22 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(21, 1, stringBuilder2);
						handler.AppendLiteral("\t[PLAY SOUND]\r\n\t\t`");
						handler.AppendFormatted(ocNIpPpqEv(TiAIswrodh(memoryStream), memoryStream));
						handler.AppendLiteral("`\r\n");
						stringBuilder22.Append(ref handler);
						break;
					}
					case 23:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder21 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(17, 1, stringBuilder2);
						handler.AppendLiteral("\t[SET FLAG]\r\n\t\t");
						handler.AppendFormatted(TiAIswrodh(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder21.Append(ref handler);
						break;
					}
					case 24:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder20 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(20, 1, stringBuilder2);
						handler.AppendLiteral("\t[FLIP TYPE]\r\n\t\t`");
						handler.AppendFormatted((FLIP_TYPE_Item)LXiIx7GhpZ(memoryStream));
						handler.AppendLiteral("`\r\n");
						stringBuilder20.Append(ref handler);
						break;
					}
					case 25:
						stringBuilder.Append("\t[LOOP START]\r\n");
						break;
					case 26:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder19 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(17, 1, stringBuilder2);
						handler.AppendLiteral("\t[LOOP END]\r\n\t\t");
						handler.AppendFormatted(TiAIswrodh(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder19.Append(ref handler);
						break;
					}
					case 27:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder18 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(16, 4, stringBuilder2);
						handler.AppendLiteral("\t[CLIP]\r\n\t\t");
						handler.AppendFormatted(PSfIcBUNmT(memoryStream));
						handler.AppendLiteral("\t");
						handler.AppendFormatted(PSfIcBUNmT(memoryStream));
						handler.AppendLiteral("\t");
						handler.AppendFormatted(PSfIcBUNmT(memoryStream));
						handler.AppendLiteral("\t");
						handler.AppendFormatted(PSfIcBUNmT(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder18.Append(ref handler);
						break;
					}
					default:
					{
						ErrorItem item2 = new ErrorItem("", 0, fileName)
						{
							Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_BinaryAniReadFrameSubError"), k, memoryStream.Position)
						};
						PVUIL9AWaA().Error(new List<ErrorItem> { item2 });
						return (success: false, text: string.Empty);
					}
					}
				}
				stringBuilder.Append(stringBuilder11);
			}
			return (success: true, text: stringBuilder.ToString());
		}
		catch (Exception ex)
		{
			PVUIL9AWaA().Error(new List<ErrorItem>
			{
				new ErrorItem("", 0, fileName)
				{
					Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_BinaryAniReadError"), ex.Message)
				}
			});
			return (success: false, text: string.Empty);
		}
	}

	public static bool DecompileAniToModel(PvfFile file, out AniFile anifile)
	{
		if (file.Data == null || file.DataLen <= 0)
		{
			anifile = new AniFile();
			return true;
		}
		return DecompileAniToModel(file.Data, file.FileName, out anifile);
	}

	public static bool DecompileAniToModel(byte[] fileData, string fileName, out AniFile aniFile)
	{
		aniFile = new AniFile();
		try
		{
			List<string> list = new List<string>();
			MemoryStream memoryStream = new MemoryStream(fileData);
			ushort num = LXiIx7GhpZ(memoryStream);
			ushort num2 = LXiIx7GhpZ(memoryStream);
			for (int i = 0; i < num2; i++)
			{
				list.Add(ocNIpPpqEv(TiAIswrodh(memoryStream), memoryStream));
			}
			ushort num3 = LXiIx7GhpZ(memoryStream);
			for (int j = 0; j < num3; j++)
			{
				switch (LXiIx7GhpZ(memoryStream))
				{
				case 0:
				{
					aniFile.LOOP = bool.TryParse(zQ6IKfMB6v(memoryStream).ToString(), out var result2) && result2;
					break;
				}
				case 1:
				{
					aniFile.SHADOW = bool.TryParse(zQ6IKfMB6v(memoryStream).ToString(), out var result) && result;
					break;
				}
				case 3:
					aniFile.COORD = PSfIcBUNmT(memoryStream);
					break;
				case 28:
					aniFile.OPERATION = LXiIx7GhpZ(memoryStream);
					break;
				case 18:
				{
					aniFile.SPECTRUM = new SPECTRUM
					{
						SPECTRUM_ = zQ6IKfMB6v(memoryStream),
						SPECTRUM_TERM = TiAIswrodh(memoryStream),
						SPECTRUM_LIFE_TIME = TiAIswrodh(memoryStream)
					};
					byte b = (byte)Xs3IGVl5nD(memoryStream);
					byte b2 = (byte)Xs3IGVl5nD(memoryStream);
					byte b3 = (byte)Xs3IGVl5nD(memoryStream);
					byte b4 = (byte)Xs3IGVl5nD(memoryStream);
					aniFile.SPECTRUM.SPECTRUM_COLOR = new RGBA((int)b, (int)b2, (int)b3, (int)b4);
					aniFile.SPECTRUM.SPECTRUM_EFFECT = (Effect_Item)LXiIx7GhpZ(memoryStream);
					break;
				}
				default:
				{
					ErrorItem item = new ErrorItem("", 0, fileName)
					{
						Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_BinaryAniReadGlobalError"), memoryStream.Position)
					};
					PVUIL9AWaA().Error(new List<ErrorItem> { item });
					return false;
				}
				}
			}
			for (int k = 0; k < num; k++)
			{
				FRAMEModel fRAMEModel = new FRAMEModel(k);
				List<BOX_Base> list2 = new List<BOX_Base>();
				aniFile.Items.Add(fRAMEModel);
				ushort num4 = LXiIx7GhpZ(memoryStream);
				for (int l = 0; l < num4; l++)
				{
					switch (LXiIx7GhpZ(memoryStream))
					{
					case 15:
						list2.Add(new ATTACK_BOX(TiAIswrodh(memoryStream), TiAIswrodh(memoryStream), TiAIswrodh(memoryStream), TiAIswrodh(memoryStream), TiAIswrodh(memoryStream), TiAIswrodh(memoryStream)));
						continue;
					case 14:
						list2.Add(new DAMAGE_BOX(TiAIswrodh(memoryStream), TiAIswrodh(memoryStream), TiAIswrodh(memoryStream), TiAIswrodh(memoryStream), TiAIswrodh(memoryStream), TiAIswrodh(memoryStream)));
						continue;
					}
					PVUIL9AWaA().Error(new List<ErrorItem>
					{
						new ErrorItem("", 0, fileName)
						{
							Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_BinaryAniReadFrameError"), k, memoryStream.Position)
						}
					});
					return false;
				}
				int num5 = PSfIcBUNmT(memoryStream);
				if (num5 >= 0)
				{
					if (num5 > list.Count - 1)
					{
						PVUIL9AWaA().Error(new List<ErrorItem>
						{
							new ErrorItem("", 0, fileName)
							{
								Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadFrameImageError"), k, memoryStream.Position)
							}
						});
						return false;
					}
					fRAMEModel.Image = new AniImage(list[num5], LXiIx7GhpZ(memoryStream));
				}
				else
				{
					fRAMEModel.Image = new AniImage("", 0);
				}
				fRAMEModel.IMAGE_POS = new POINT(TiAIswrodh(memoryStream), TiAIswrodh(memoryStream));
				ushort num6 = LXiIx7GhpZ(memoryStream);
				for (int m = 0; m < num6; m++)
				{
					switch (LXiIx7GhpZ(memoryStream))
					{
					case 0:
					{
						fRAMEModel.LOOP = bool.TryParse(zQ6IKfMB6v(memoryStream).ToString(), out var result4) && result4;
						break;
					}
					case 1:
					{
						fRAMEModel.SHADOW = bool.TryParse(zQ6IKfMB6v(memoryStream).ToString(), out var result3) && result3;
						break;
					}
					case 10:
						fRAMEModel.INTERPOLATION = zQ6IKfMB6v(memoryStream);
						break;
					case 3:
						fRAMEModel.COORD = LXiIx7GhpZ(memoryStream);
						break;
					case 17:
						fRAMEModel.PRELOAD = true;
						break;
					case 7:
						fRAMEModel.IMAGE_RATE = new ImageRate(U0WI5U3YUU(memoryStream), U0WI5U3YUU(memoryStream));
						break;
					case 8:
						fRAMEModel.IMAGE_ROTATE = U0WI5U3YUU(memoryStream);
						break;
					case 9:
					{
						byte b5 = (byte)Xs3IGVl5nD(memoryStream);
						byte b6 = (byte)Xs3IGVl5nD(memoryStream);
						byte b7 = (byte)Xs3IGVl5nD(memoryStream);
						byte b8 = (byte)Xs3IGVl5nD(memoryStream);
						fRAMEModel.RGBA = new RGBA((int)b5, (int)b6, (int)b7, (int)b8);
						break;
					}
					case 11:
					{
						Effect_Item effect_Item = (Effect_Item)LXiIx7GhpZ(memoryStream);
						switch (effect_Item)
						{
						case Effect_Item.MONOCHROME:
							fRAMEModel.GRAPHIC_EFFECT = new GRAPHIC_EFFECT_Base
							{
								Type = effect_Item,
								RGB = new RGB(Xs3IGVl5nD(memoryStream), Xs3IGVl5nD(memoryStream), Xs3IGVl5nD(memoryStream))
							};
							break;
						case Effect_Item.SPACEDISTORT:
							fRAMEModel.GRAPHIC_EFFECT = new GRAPHIC_EFFECT_Base(PSfIcBUNmT(memoryStream), PSfIcBUNmT(memoryStream), effect_Item);
							break;
						default:
							fRAMEModel.GRAPHIC_EFFECT = new GRAPHIC_EFFECT_Base
							{
								Type = effect_Item
							};
							break;
						}
						break;
					}
					case 12:
						fRAMEModel.DELAY = TiAIswrodh(memoryStream);
						break;
					case 13:
						fRAMEModel.DAMAGE_TYPE = (DAMAGE_TYPE_Item)LXiIx7GhpZ(memoryStream);
						break;
					case 16:
						fRAMEModel.PLAY_SOUND = ocNIpPpqEv(TiAIswrodh(memoryStream), memoryStream);
						break;
					case 23:
						fRAMEModel.SET_FLAG = TiAIswrodh(memoryStream);
						break;
					case 24:
						fRAMEModel.FLIP_TYPE = (FLIP_TYPE_Item)LXiIx7GhpZ(memoryStream);
						break;
					case 25:
						fRAMEModel.LOOP_START = true;
						break;
					case 26:
						fRAMEModel.LOOP_END = TiAIswrodh(memoryStream);
						break;
					case 27:
						fRAMEModel.CLIP = new CLIP(PSfIcBUNmT(memoryStream), PSfIcBUNmT(memoryStream), PSfIcBUNmT(memoryStream), PSfIcBUNmT(memoryStream));
						break;
					default:
					{
						ErrorItem item2 = new ErrorItem("", 0, fileName)
						{
							Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_BinaryAniReadFrameSubError"), k, memoryStream.Position)
						};
						PVUIL9AWaA().Error(new List<ErrorItem> { item2 });
						return false;
					}
					}
				}
				if (list2.Count > 0)
				{
					fRAMEModel.BOX_List = list2;
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			PVUIL9AWaA().Error(new List<ErrorItem>
			{
				new ErrorItem("", 0, fileName)
				{
					Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_BinaryAniReadError"), ex.Message)
				}
			});
			return false;
		}
	}

	public static (bool success, PooledList<PrivewAniData>) GetAniPrivewData(PvfFile file)
	{
		PooledList<PrivewAniData> pooledList = new PooledList<PrivewAniData>();
		try
		{
			if (file == null)
			{
				return (success: true, pooledList);
			}
			if (file.DataLen <= 0)
			{
				return (success: true, pooledList);
			}
			List<string> list = new List<string>();
			MemoryStream memoryStream = new MemoryStream(file.Data);
			StringBuilder stringBuilder = new StringBuilder("#PVF_File\r\n");
			ushort num = LXiIx7GhpZ(memoryStream);
			ushort num2 = LXiIx7GhpZ(memoryStream);
			for (int i = 0; i < num2; i++)
			{
				list.Add(ocNIpPpqEv(TiAIswrodh(memoryStream), memoryStream));
			}
			ushort num3 = LXiIx7GhpZ(memoryStream);
			StringBuilder stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler;
			for (int j = 0; j < num3; j++)
			{
				ushort num4 = LXiIx7GhpZ(memoryStream);
				switch (num4)
				{
				case 0:
				case 1:
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder4 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(7, 2, stringBuilder2);
					handler.AppendLiteral("[");
					handler.AppendFormatted((ANIData)num4);
					handler.AppendLiteral("]\r\n\t");
					handler.AppendFormatted(zQ6IKfMB6v(memoryStream));
					handler.AppendLiteral("\r\n");
					stringBuilder4.Append(ref handler);
					break;
				}
				case 3:
				case 28:
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder3 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(7, 2, stringBuilder2);
					handler.AppendLiteral("[");
					handler.AppendFormatted((ANIData)num4);
					handler.AppendLiteral("]\r\n\t");
					handler.AppendFormatted(LXiIx7GhpZ(memoryStream));
					handler.AppendLiteral("\r\n");
					stringBuilder3.Append(ref handler);
					break;
				}
				case 18:
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder5 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(13, 1, stringBuilder2);
					handler.AppendLiteral("[SPECTRUM]\r\n\t");
					handler.AppendFormatted(zQ6IKfMB6v(memoryStream));
					stringBuilder5.Append(ref handler);
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder6 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(22, 1, stringBuilder2);
					handler.AppendLiteral("\r\n\t[SPECTRUM TERM]\r\n\t\t");
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					stringBuilder6.Append(ref handler);
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder7 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(27, 1, stringBuilder2);
					handler.AppendLiteral("\r\n\t[SPECTRUM LIFE TIME]\r\n\t\t");
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					stringBuilder7.Append(ref handler);
					stringBuilder.Append("\r\n\t[SPECTRUM COLOR]\r\n\t\t");
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder8 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(5, 4, stringBuilder2);
					handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
					handler.AppendLiteral("\r\n");
					stringBuilder8.Append(ref handler);
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder9 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(26, 1, stringBuilder2);
					handler.AppendLiteral("\t[SPECTRUM EFFECT]\r\n\t\t`");
					handler.AppendFormatted((Effect_Item)LXiIx7GhpZ(memoryStream));
					handler.AppendLiteral("`\r\n");
					stringBuilder9.Append(ref handler);
					break;
				}
				default:
				{
					ErrorItem item = new ErrorItem("", 0, file.FileName)
					{
						Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_BinaryAniReadGlobalError"), memoryStream.Position)
					};
					PVUIL9AWaA().Error(new List<ErrorItem> { item });
					return (success: false, null);
				}
				}
			}
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder10 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(16, 1, stringBuilder2);
			handler.AppendLiteral("[FRAME MAX]\r\n\t");
			handler.AppendFormatted(num);
			handler.AppendLiteral("\r\n");
			stringBuilder10.Append(ref handler);
			for (int k = 0; k < num; k++)
			{
				PrivewAniData privewAniData = new PrivewAniData();
				stringBuilder.Append("\r\n[FRAME" + k.ToString("D3") + "]\r\n");
				ushort num5 = LXiIx7GhpZ(memoryStream);
				StringBuilder stringBuilder11 = new StringBuilder();
				for (int l = 0; l < num5; l++)
				{
					switch (LXiIx7GhpZ(memoryStream))
					{
					case 15:
						stringBuilder11.Append("\t[ATTACK BOX]\r\n\t");
						break;
					case 14:
						stringBuilder11.Append("\t[DAMAGE BOX]\r\n\t");
						break;
					default:
						PVUIL9AWaA().Error(new List<ErrorItem>
						{
							new ErrorItem("", 0, file.FileName)
							{
								Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_BinaryAniReadFrameError"), k, memoryStream.Position)
							}
						});
						return (success: false, null);
					}
					stringBuilder2 = stringBuilder11;
					StringBuilder stringBuilder12 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(7, 6, stringBuilder2);
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					handler.AppendLiteral("\t");
					handler.AppendFormatted(TiAIswrodh(memoryStream));
					handler.AppendLiteral("\r\n");
					stringBuilder12.Append(ref handler);
				}
				stringBuilder.Append("\t[IMAGE]\r\n");
				int num6 = PSfIcBUNmT(memoryStream);
				if (num6 >= 0)
				{
					if (num6 > list.Count - 1)
					{
						PVUIL9AWaA().Error(new List<ErrorItem>
						{
							new ErrorItem("", 0, file.FileName)
							{
								Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_BinaryAniReadFrameImageError"), k, memoryStream.Position)
							}
						});
						return (success: false, null);
					}
					ushort num7 = LXiIx7GhpZ(memoryStream);
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder13 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(10, 2, stringBuilder2);
					handler.AppendLiteral("\t\t`");
					handler.AppendFormatted(list[num6]);
					handler.AppendLiteral("`\r\n\t\t");
					handler.AppendFormatted(num7);
					handler.AppendLiteral("\r\n");
					stringBuilder13.Append(ref handler);
					privewAniData.Icon = list[num6];
					privewAniData.IconIndex = num7;
				}
				else
				{
					stringBuilder.Append("\t\t``\r\n\t\t0\r\n");
				}
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder14 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(19, 2, stringBuilder2);
				handler.AppendLiteral("\t[IMAGE POS]\r\n\t\t");
				handler.AppendFormatted(TiAIswrodh(memoryStream));
				handler.AppendLiteral("\t");
				handler.AppendFormatted(TiAIswrodh(memoryStream));
				handler.AppendLiteral("\r\n");
				stringBuilder14.Append(ref handler);
				ushort num8 = LXiIx7GhpZ(memoryStream);
				for (int m = 0; m < num8; m++)
				{
					ushort num9 = LXiIx7GhpZ(memoryStream);
					switch (num9)
					{
					case 0:
					case 1:
					case 10:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder29 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(9, 2, stringBuilder2);
						handler.AppendLiteral("\t[");
						handler.AppendFormatted((ANIData)num9);
						handler.AppendLiteral("]\r\n\t\t");
						handler.AppendFormatted(zQ6IKfMB6v(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder29.Append(ref handler);
						break;
					}
					case 3:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder28 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(14, 1, stringBuilder2);
						handler.AppendLiteral("\t[COORD]\r\n\t\t");
						handler.AppendFormatted(LXiIx7GhpZ(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder28.Append(ref handler);
						break;
					}
					case 17:
						stringBuilder.Append("\t[PRELOAD]\r\n\t\t1\r\n");
						break;
					case 7:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder16 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(20, 2, stringBuilder2);
						handler.AppendLiteral("\t[IMAGE RATE]\r\n\t\t");
						handler.AppendFormatted(U0WI5U3YUU(memoryStream));
						handler.AppendLiteral("\t");
						handler.AppendFormatted(U0WI5U3YUU(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder16.Append(ref handler);
						break;
					}
					case 8:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder15 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(21, 1, stringBuilder2);
						handler.AppendLiteral("\t[IMAGE ROTATE]\r\n\t\t");
						handler.AppendFormatted(U0WI5U3YUU(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder15.Append(ref handler);
						break;
					}
					case 9:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder17 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(16, 4, stringBuilder2);
						handler.AppendLiteral("\t[RGBA]\r\n\t\t");
						handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
						handler.AppendLiteral("\t");
						handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
						handler.AppendLiteral("\t");
						handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
						handler.AppendLiteral("\t");
						handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder17.Append(ref handler);
						break;
					}
					case 11:
					{
						stringBuilder.Append("\t[GRAPHIC EFFECT]\r\n");
						ushort num11 = LXiIx7GhpZ(memoryStream);
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder25 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
						handler.AppendLiteral("\t\t`");
						handler.AppendFormatted((Effect_Item)num11);
						handler.AppendLiteral("`\r\n");
						stringBuilder25.Append(ref handler);
						if (num11 == 5)
						{
							stringBuilder2 = stringBuilder;
							StringBuilder stringBuilder26 = stringBuilder2;
							handler = new StringBuilder.AppendInterpolatedStringHandler(6, 3, stringBuilder2);
							handler.AppendLiteral("\t\t");
							handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
							handler.AppendLiteral("\t");
							handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
							handler.AppendLiteral("\t");
							handler.AppendFormatted(Xs3IGVl5nD(memoryStream));
							handler.AppendLiteral("\r\n");
							stringBuilder26.Append(ref handler);
						}
						if (num11 == 6)
						{
							stringBuilder2 = stringBuilder;
							StringBuilder stringBuilder27 = stringBuilder2;
							handler = new StringBuilder.AppendInterpolatedStringHandler(5, 2, stringBuilder2);
							handler.AppendLiteral("\t\t");
							handler.AppendFormatted(PSfIcBUNmT(memoryStream));
							handler.AppendLiteral("\t");
							handler.AppendFormatted(PSfIcBUNmT(memoryStream));
							handler.AppendLiteral("\r\n");
							stringBuilder27.Append(ref handler);
						}
						break;
					}
					case 12:
					{
						int num10 = TiAIswrodh(memoryStream);
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder24 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(14, 1, stringBuilder2);
						handler.AppendLiteral("\t[DELAY]\r\n\t\t");
						handler.AppendFormatted(num10);
						handler.AppendLiteral("\r\n");
						stringBuilder24.Append(ref handler);
						privewAniData.Delay = num10;
						break;
					}
					case 13:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder23 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(22, 1, stringBuilder2);
						handler.AppendLiteral("\t[DAMAGE TYPE]\r\n\t\t`");
						handler.AppendFormatted((DAMAGE_TYPE_Item)LXiIx7GhpZ(memoryStream));
						handler.AppendLiteral("`\r\n");
						stringBuilder23.Append(ref handler);
						break;
					}
					case 16:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder22 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(21, 1, stringBuilder2);
						handler.AppendLiteral("\t[PLAY SOUND]\r\n\t\t`");
						handler.AppendFormatted(ocNIpPpqEv(TiAIswrodh(memoryStream), memoryStream));
						handler.AppendLiteral("`\r\n");
						stringBuilder22.Append(ref handler);
						break;
					}
					case 23:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder21 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(17, 1, stringBuilder2);
						handler.AppendLiteral("\t[SET FLAG]\r\n\t\t");
						handler.AppendFormatted(TiAIswrodh(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder21.Append(ref handler);
						break;
					}
					case 24:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder20 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(20, 1, stringBuilder2);
						handler.AppendLiteral("\t[FLIP TYPE]\r\n\t\t`");
						handler.AppendFormatted((FLIP_TYPE_Item)LXiIx7GhpZ(memoryStream));
						handler.AppendLiteral("`\r\n");
						stringBuilder20.Append(ref handler);
						break;
					}
					case 25:
						stringBuilder.Append("\t[LOOP START]\r\n");
						break;
					case 26:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder19 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(17, 1, stringBuilder2);
						handler.AppendLiteral("\t[LOOP END]\r\n\t\t");
						handler.AppendFormatted(TiAIswrodh(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder19.Append(ref handler);
						break;
					}
					case 27:
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder18 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(16, 4, stringBuilder2);
						handler.AppendLiteral("\t[CLIP]\r\n\t\t");
						handler.AppendFormatted(PSfIcBUNmT(memoryStream));
						handler.AppendLiteral("\t");
						handler.AppendFormatted(PSfIcBUNmT(memoryStream));
						handler.AppendLiteral("\t");
						handler.AppendFormatted(PSfIcBUNmT(memoryStream));
						handler.AppendLiteral("\t");
						handler.AppendFormatted(PSfIcBUNmT(memoryStream));
						handler.AppendLiteral("\r\n");
						stringBuilder18.Append(ref handler);
						break;
					}
					default:
					{
						ErrorItem item2 = new ErrorItem("", 0, file.FileName)
						{
							Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_BinaryAniReadFrameSubError"), k, memoryStream.Position)
						};
						PVUIL9AWaA().Error(new List<ErrorItem> { item2 });
						return (success: false, null);
					}
					}
				}
				stringBuilder.Append(stringBuilder11);
				pooledList.Add(privewAniData);
			}
			return (success: true, pooledList);
		}
		catch (Exception ex)
		{
			PVUIL9AWaA().Error(new List<ErrorItem>
			{
				new ErrorItem("", 0, file.FileName)
				{
					Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadError"), ex.Message)
				}
			});
			return (success: false, pooledList);
		}
	}

	public static (bool success, byte[]? data, ErrorItem error) CompileBinaryAni(string text, string fileName, bool compile70PlusAni = false)
	{
		try
		{
			if (string.IsNullOrEmpty(text))
			{
				return (success: true, data: new byte[0], error: null);
			}
			if (compile70PlusAni)
			{
				ResultData<string> resultData = Convert85PlusAniTo70Ani(text);
				if (resultData.IsError)
				{
					ErrorItem item = new ErrorItem("", 0, fileName)
					{
						Description = resultData.Msg
					};
					return (success: false, data: null, error: item);
				}
				text = resultData.Data;
			}
			List<byte> list = new List<byte>();
			List<string> list2 = new List<string>();
			List<string> list3 = (from t in text.Split(new char[3] { '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)
				select t.TrimEnd() into text6
				where text6 != "" && text6 != "#PVF_File"
				select text6).ToList();
			int count = list3.Count;
			List<int> list4 = new List<int>();
			for (int num = 0; num < count; num++)
			{
				if (list3[num].Length > 6 && list3[num].Substring(0, 6) == "[FRAME" && list3[num] != "[FRAME MAX]")
				{
					list4.Add(num);
				}
			}
			list4.Add(count);
			list.AddRange(BitConverter.GetBytes((ushort)(list4.Count - 1)));
			for (int num2 = 0; num2 < count; num2++)
			{
				if (!(list3[num2] == "") && !(list3[num2] != "[IMAGE]"))
				{
					string text2 = list3[num2 + 1];
					if (!list2.Contains(text2) && text2 != "``")
					{
						list2.Add(text2);
					}
					if (text2 == "``")
					{
						list3[num2 + 1] = "-1";
					}
				}
			}
			list.AddRange(BitConverter.GetBytes((ushort)list2.Count));
			foreach (string item4 in list2)
			{
				string dataFromFormat = DataHelper.GetDataFromFormat(item4, "`", "`");
				list.AddRange(BitConverter.GetBytes(dataFromFormat.Length));
				list.AddRange(Encoding.ASCII.GetBytes(dataFromFormat));
			}
			List<byte> list5 = new List<byte>();
			int num3 = 0;
			for (int num4 = 0; num4 < list4[0]; num4++)
			{
				if (list3[num4] == "" || list3[num4][0] != '[' || (list3[num4].Length > 6 && list3[num4].Substring(0, 6) == "[FRAME"))
				{
					continue;
				}
				num3++;
				string text3 = list3[num4];
				if (!(text3 == "[LOOP]"))
				{
					if (!(text3 == "[SHADOW]"))
					{
						if (!(text3 == "[COORD]"))
						{
							if (!(text3 == "[OPERATION]"))
							{
								if (!(text3 == "[SPECTRUM]"))
								{
									goto IL_07d3;
								}
								list5.AddRange(BitConverter.GetBytes((ushort)18));
								list5.Add(byte.Parse(list3[num4 + 1]));
								if (list3[num4 + 2][0] != '[' || list3[num4 + 4][0] != '[' || list3[num4 + 6][0] != '[' || list3[num4 + 11][0] != '[')
								{
									continue;
								}
								list5.AddRange(BitConverter.GetBytes(uint.Parse(list3[num4 + 3])));
								list5.AddRange(BitConverter.GetBytes(uint.Parse(list3[num4 + 5])));
								list5.Add(byte.Parse(list3[num4 + 7]));
								list5.Add(byte.Parse(list3[num4 + 8]));
								list5.Add(byte.Parse(list3[num4 + 9]));
								list5.Add(byte.Parse(list3[num4 + 10]));
								string text4 = list3[num4 + 12];
								if (!(text4 == "`NONE`"))
								{
									if (!(text4 == "`DODGE`"))
									{
										if (!(text4 == "`LINEARDODGE`"))
										{
											if (!(text4 == "`DARK`"))
											{
												if (!(text4 == "`MONOCHROME`"))
												{
													goto IL_07d3;
												}
												list5.AddRange(BitConverter.GetBytes((ushort)5));
											}
											else
											{
												list5.AddRange(BitConverter.GetBytes((ushort)3));
											}
										}
										else
										{
											list5.AddRange(BitConverter.GetBytes((ushort)2));
										}
									}
									else
									{
										list5.AddRange(BitConverter.GetBytes((ushort)1));
									}
								}
								else
								{
									list5.AddRange(BitConverter.GetBytes((ushort)0));
								}
								list3[num4 + 2] = "";
								list3[num4 + 4] = "";
								list3[num4 + 6] = "";
								list3[num4 + 11] = "";
								continue;
							}
							list5.AddRange(BitConverter.GetBytes((ushort)28));
							list5.AddRange(BitConverter.GetBytes(ushort.Parse(list3[num4 + 1])));
							continue;
						}
						list5.AddRange(BitConverter.GetBytes((ushort)3));
						list5.AddRange(BitConverter.GetBytes(ushort.Parse(list3[num4 + 1])));
						continue;
					}
					list5.AddRange(BitConverter.GetBytes((ushort)1));
					list5.Add(byte.Parse(list3[num4 + 1]));
					continue;
				}
				list5.AddRange(BitConverter.GetBytes((ushort)0));
				list5.Add(byte.Parse(list3[num4 + 1]));
				continue;
				IL_07d3:
				ErrorItem item2 = new ErrorItem("", 0, fileName)
				{
					Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_BinaryAniReadGlobalError2"), list3[num4])
				};
				return (success: false, data: null, error: item2);
			}
			list.AddRange(BitConverter.GetBytes((ushort)num3));
			list.AddRange(list5);
			for (int num5 = 1; num5 < list4.Count; num5++)
			{
				_003C_003Ec__DisplayClass9_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass9_0();
				List<byte> list6 = new List<byte>();
				int num6 = 0;
				int num7 = list4[num5 - 1];
				int num8 = list4[num5];
				CS_0024_003C_003E8__locals5.aCj0PFm0jW = "-1";
				string s = "0";
				string s2 = "0";
				string s3 = "0";
				for (int num9 = num7; num9 < num8; num9++)
				{
					if (list3[num9] == "" || list3[num9][0] != '[')
					{
						continue;
					}
					if (list3[num9] == "[DAMAGE BOX]" || list3[num9] == "[ATTACK BOX]")
					{
						if (list3[num9] == "[DAMAGE BOX]")
						{
							list6.AddRange(BitConverter.GetBytes((ushort)14));
						}
						else if (list3[num9] == "[ATTACK BOX]")
						{
							list6.AddRange(BitConverter.GetBytes((ushort)15));
						}
						list6.AddRange(BitConverter.GetBytes(int.Parse(list3[num9 + 1])));
						list6.AddRange(BitConverter.GetBytes(int.Parse(list3[num9 + 2])));
						list6.AddRange(BitConverter.GetBytes(int.Parse(list3[num9 + 3])));
						list6.AddRange(BitConverter.GetBytes(int.Parse(list3[num9 + 4])));
						list6.AddRange(BitConverter.GetBytes(int.Parse(list3[num9 + 5])));
						list6.AddRange(BitConverter.GetBytes(int.Parse(list3[num9 + 6])));
						list3[num9] = "";
						num6++;
					}
					string text3 = list3[num9];
					if (!(text3 == "[IMAGE]"))
					{
						if (text3 == "[IMAGE POS]")
						{
							list3[num9] = "";
							s2 = list3[num9 + 1];
							s3 = list3[num9 + 2];
						}
					}
					else
					{
						list3[num9] = "";
						CS_0024_003C_003E8__locals5.aCj0PFm0jW = list3[num9 + 1];
						s = list3[num9 + 2];
					}
				}
				list.AddRange(BitConverter.GetBytes((ushort)num6));
				if (num6 > 0)
				{
					list.AddRange(list6);
				}
				if (CS_0024_003C_003E8__locals5.aCj0PFm0jW != "-1")
				{
					int value = list2.FindIndex((string text6) => text6 == CS_0024_003C_003E8__locals5.aCj0PFm0jW);
					list.AddRange(BitConverter.GetBytes(Convert.ToInt16(value)));
					list.AddRange(BitConverter.GetBytes(short.Parse(s)));
				}
				else
				{
					list.AddRange(BitConverter.GetBytes(short.Parse(CS_0024_003C_003E8__locals5.aCj0PFm0jW)));
				}
				list.AddRange(BitConverter.GetBytes(int.Parse(s2)));
				list.AddRange(BitConverter.GetBytes(int.Parse(s3)));
				List<byte> list7 = new List<byte>();
				int num10 = 0;
				for (int num11 = num7; num11 < num8; num11++)
				{
					if ((list3[num11].Length > 6 && list3[num11].Substring(0, 6) == "[FRAME") || list3[num11] == "" || list3[num11][0] != '[')
					{
						continue;
					}
					num10++;
					string text3 = list3[num11];
					if (text3 != null)
					{
						switch (text3.Length)
						{
						case 6:
							switch (text3[1])
							{
							case 'L':
								if (!(text3 == "[LOOP]"))
								{
									break;
								}
								list7.AddRange(BitConverter.GetBytes((ushort)0));
								list7.Add(byte.Parse(list3[num11 + 1]));
								continue;
							case 'C':
								if (!(text3 == "[CLIP]"))
								{
									break;
								}
								list7.AddRange(BitConverter.GetBytes((ushort)27));
								list7.AddRange(BitConverter.GetBytes(short.Parse(list3[num11 + 1])));
								list7.AddRange(BitConverter.GetBytes(short.Parse(list3[num11 + 2])));
								list7.AddRange(BitConverter.GetBytes(short.Parse(list3[num11 + 3])));
								list7.AddRange(BitConverter.GetBytes(short.Parse(list3[num11 + 4])));
								continue;
							case 'R':
								if (!(text3 == "[RGBA]"))
								{
									break;
								}
								list7.AddRange(BitConverter.GetBytes((ushort)9));
								list7.Add(byte.Parse(list3[num11 + 1]));
								list7.Add(byte.Parse(list3[num11 + 2]));
								list7.Add(byte.Parse(list3[num11 + 3]));
								list7.Add(byte.Parse(list3[num11 + 4]));
								continue;
							}
							break;
						case 7:
							switch (text3[1])
							{
							case 'C':
								if (!(text3 == "[COORD]"))
								{
									break;
								}
								list7.AddRange(BitConverter.GetBytes((ushort)3));
								list7.AddRange(BitConverter.GetBytes(ushort.Parse(list3[num11 + 1])));
								continue;
							case 'D':
								if (!(text3 == "[DELAY]"))
								{
									break;
								}
								list7.AddRange(BitConverter.GetBytes((ushort)12));
								list7.AddRange(BitConverter.GetBytes(Convert.ToUInt32(list3[num11 + 1])));
								continue;
							}
							break;
						case 12:
							switch (text3[1])
							{
							case 'I':
								if (!(text3 == "[IMAGE RATE]"))
								{
									break;
								}
								list7.AddRange(BitConverter.GetBytes((ushort)7));
								list7.AddRange(BitConverter.GetBytes(float.Parse(list3[num11 + 1])));
								list7.AddRange(BitConverter.GetBytes(float.Parse(list3[num11 + 2])));
								continue;
							case 'L':
								if (!(text3 == "[LOOP START]"))
								{
									break;
								}
								list7.AddRange(BitConverter.GetBytes((ushort)25));
								continue;
							case 'P':
							{
								if (!(text3 == "[PLAY SOUND]"))
								{
									break;
								}
								list7.AddRange(BitConverter.GetBytes((ushort)16));
								string text5 = list3[num11 + 1].Substring(1, list3[num11 + 1].Length - 2);
								list7.AddRange(BitConverter.GetBytes(text5.Length));
								list7.AddRange(Encoding.ASCII.GetBytes(text5));
								continue;
							}
							}
							break;
						case 10:
							switch (text3[1])
							{
							case 'S':
								if (!(text3 == "[SET FLAG]"))
								{
									break;
								}
								list7.AddRange(BitConverter.GetBytes((ushort)23));
								list7.AddRange(BitConverter.GetBytes(uint.Parse(list3[num11 + 1])));
								continue;
							case 'L':
								if (!(text3 == "[LOOP END]"))
								{
									break;
								}
								list7.AddRange(BitConverter.GetBytes((ushort)26));
								list7.AddRange(BitConverter.GetBytes(uint.Parse(list3[num11 + 1])));
								continue;
							}
							break;
						case 8:
							if (!(text3 == "[SHADOW]"))
							{
								break;
							}
							list7.AddRange(BitConverter.GetBytes((ushort)1));
							list7.Add(byte.Parse(list3[num11 + 1]));
							continue;
						case 9:
							if (!(text3 == "[PRELOAD]"))
							{
								break;
							}
							list7.AddRange(BitConverter.GetBytes((ushort)17));
							continue;
						case 14:
							if (!(text3 == "[IMAGE ROTATE]"))
							{
								break;
							}
							list7.AddRange(BitConverter.GetBytes((ushort)8));
							list7.AddRange(BitConverter.GetBytes(float.Parse(list3[num11 + 1])));
							continue;
						case 15:
							if (!(text3 == "[INTERPOLATION]"))
							{
								break;
							}
							list7.AddRange(BitConverter.GetBytes((ushort)10));
							list7.Add(byte.Parse(list3[num11 + 1]));
							continue;
						case 16:
						{
							if (!(text3 == "[GRAPHIC EFFECT]"))
							{
								break;
							}
							list7.AddRange(BitConverter.GetBytes((ushort)11));
							string text4 = list3[num11 + 1].ToUpper();
							if (text4 == null)
							{
								break;
							}
							switch (text4.Length)
							{
							case 6:
								switch (text4[1])
								{
								case 'N':
									if (!(text4 == "`NONE`"))
									{
										break;
									}
									list7.AddRange(BitConverter.GetBytes((ushort)0));
									continue;
								case 'D':
									if (!(text4 == "`DARK`"))
									{
										break;
									}
									list7.AddRange(BitConverter.GetBytes((ushort)3));
									continue;
								}
								break;
							case 7:
								if (!(text4 == "`DODGE`"))
								{
									break;
								}
								list7.AddRange(BitConverter.GetBytes((ushort)1));
								continue;
							case 13:
								if (!(text4 == "`LINEARDODGE`"))
								{
									break;
								}
								list7.AddRange(BitConverter.GetBytes((ushort)2));
								continue;
							case 5:
								if (!(text4 == "`XOR`"))
								{
									break;
								}
								list7.AddRange(BitConverter.GetBytes((ushort)4));
								continue;
							case 12:
								if (!(text4 == "`MONOCHROME`"))
								{
									break;
								}
								list7.AddRange(BitConverter.GetBytes((ushort)5));
								list7.Add(byte.Parse(list3[num11 + 2]));
								list7.Add(byte.Parse(list3[num11 + 3]));
								list7.Add(byte.Parse(list3[num11 + 4]));
								continue;
							case 14:
								if (!(text4 == "`SPACEDISTORT`"))
								{
									break;
								}
								list7.AddRange(BitConverter.GetBytes((ushort)6));
								list7.AddRange(BitConverter.GetBytes(short.Parse(list3[num11 + 2])));
								list7.AddRange(BitConverter.GetBytes(short.Parse(list3[num11 + 3])));
								continue;
							}
							break;
						}
						case 13:
							if (text3 == "[DAMAGE TYPE]")
							{
								list7.AddRange(BitConverter.GetBytes((ushort)13));
								string text4 = list3[num11 + 1];
								if (text4 == "`SUPERARMOR`")
								{
									list7.AddRange(BitConverter.GetBytes((ushort)1));
									continue;
								}
								if (text4 == "`NORMAL`")
								{
									list7.AddRange(BitConverter.GetBytes((ushort)0));
									continue;
								}
								if (text4 == "`UNBREAKABLE`")
								{
									list7.AddRange(BitConverter.GetBytes((ushort)2));
									continue;
								}
							}
							break;
						case 11:
							if (text3 == "[FLIP TYPE]")
							{
								list7.AddRange(BitConverter.GetBytes((ushort)24));
								string text4 = list3[num11 + 1];
								if (text4 == "`ALL`")
								{
									list7.AddRange(BitConverter.GetBytes((ushort)3));
									continue;
								}
								if (text4 == "`HORIZON`")
								{
									list7.AddRange(BitConverter.GetBytes((ushort)1));
									continue;
								}
								if (text4 == "`VERTICAL`")
								{
									list7.AddRange(BitConverter.GetBytes((ushort)2));
									continue;
								}
							}
							break;
						}
					}
					ErrorItem item3 = new ErrorItem("", 0, fileName)
					{
						Description = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadFrameError2"), num5, list3[num11])
					};
					return (success: false, data: null, error: item3);
				}
				list.AddRange(BitConverter.GetBytes((ushort)num10));
				list.AddRange(list7);
			}
			return (success: true, data: list.ToArray(), error: null);
		}
		catch (Exception ex)
		{
			PVUIL9AWaA().Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniSaveError"), fileName, ex.Message));
			return (success: false, data: null, error: null);
		}
	}

	public static ResultData<string> Convert85PlusAniTo70Ani(string text)
	{
		ResultData<string> resultData = new ResultData<string>();
		try
		{
			List<string> list = new List<string>();
			int num = text.IndexOf("[IMAGE PATH]");
			int num2 = text.IndexOf("[/IMAGE PATH]");
			string value = "FRAME";
			if (num != -1 && num2 != -1)
			{
				string text2 = text.Substring(num, num2 - num + "[/IMAGE PATH]".Length);
				text = text.Replace(text2, "\r\n");
				text2 = text2.Replace("[IMAGE PATH]", string.Empty).Replace("[/IMAGE PATH]", string.Empty);
				string[] array = text2.Split(new char[3] { '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
				if (array != null)
				{
					list = array.ToList();
				}
			}
			List<string> list2 = (from t in text.Replace("#PVF_File", string.Empty).Split(new char[3] { '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)
				select t.TrimEnd()).ToList();
			List<ffP2dGOSCUMNUS9ggmI> list3 = new List<ffP2dGOSCUMNUS9ggmI>();
			ffP2dGOSCUMNUS9ggmI ffP2dGOSCUMNUS9ggmI2 = null;
			foreach (string item in list2)
			{
				if (item.Length > 3 && item[0] == '[')
				{
					if (item.Contains(value) && item.Length > 7 && !item.Contains(" "))
					{
						break;
					}
					ffP2dGOSCUMNUS9ggmI2 = new ffP2dGOSCUMNUS9ggmI
					{
						SectionName = item
					};
					list3.Add(ffP2dGOSCUMNUS9ggmI2);
				}
				else
				{
					ffP2dGOSCUMNUS9ggmI2?.TtJO7DF1PT().Add(item);
				}
			}
			bool flag = false;
			ffP2dGOSCUMNUS9ggmI2 = null;
			ffP2dGOSCUMNUS9ggmI ffP2dGOSCUMNUS9ggmI3 = null;
			foreach (string item2 in list2)
			{
				if (item2.Contains(value) && item2.Length > 7 && !item2.Contains(" "))
				{
					ffP2dGOSCUMNUS9ggmI obj = new ffP2dGOSCUMNUS9ggmI();
					obj.SectionName = item2;
					obj.mIwOgnH2HO(true);
					ffP2dGOSCUMNUS9ggmI2 = obj;
					list3.Add(ffP2dGOSCUMNUS9ggmI2);
					flag = true;
				}
				else if (flag)
				{
					if (item2.Length > 3 && item2[0] == '[')
					{
						ffP2dGOSCUMNUS9ggmI3 = new ffP2dGOSCUMNUS9ggmI
						{
							SectionName = item2
						};
						ffP2dGOSCUMNUS9ggmI2.BxiOhMIJuP().Add(ffP2dGOSCUMNUS9ggmI3);
					}
					else
					{
						ffP2dGOSCUMNUS9ggmI3.TtJO7DF1PT().Add(item2);
					}
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ffP2dGOSCUMNUS9ggmI item3 in list3)
			{
				ResultData resultData2 = item3.uMtObuuZ1N(stringBuilder, list);
				if (resultData2.IsError)
				{
					return new ResultData<string>
					{
						Msg = resultData2.Msg
					};
				}
			}
			return new ResultData<string>
			{
				Data = stringBuilder.ToString()
			};
		}
		catch (Exception ex)
		{
			resultData.Msg = "70PlusAni转换失败：" + ex.Message;
			return resultData;
		}
	}

	private static byte zQ6IKfMB6v(Stream P_0)
	{
		return (byte)P_0.ReadByte();
	}

	private static ushort LXiIx7GhpZ(Stream P_0)
	{
		byte[] array = new byte[2];
		P_0.Read(array, 0, 2);
		return BitConverter.ToUInt16(array, 0);
	}

	private static short PSfIcBUNmT(Stream P_0)
	{
		byte[] array = new byte[2];
		P_0.Read(array, 0, 2);
		return BitConverter.ToInt16(array, 0);
	}

	private static int TiAIswrodh(Stream P_0)
	{
		byte[] array = new byte[4];
		P_0.Read(array, 0, 4);
		return BitConverter.ToInt32(array, 0);
	}

	private static float U0WI5U3YUU(Stream P_0)
	{
		byte[] array = new byte[4];
		P_0.Read(array, 0, 4);
		return BitConverter.ToSingle(array, 0);
	}

	private static double Xs3IGVl5nD(Stream P_0)
	{
		return (256.0 + (double)(int)zQ6IKfMB6v(P_0)) % 256.0;
	}

	private static string ocNIpPpqEv(int P_0, Stream P_1)
	{
		byte[] array = new byte[P_0];
		P_1.Read(array, 0, P_0);
		return Encoding.ASCII.GetString(array);
	}
}
