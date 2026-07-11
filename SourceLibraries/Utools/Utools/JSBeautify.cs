using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Utools;

public class JSBeautify
{
	private StringBuilder ql6VKtSXMQ;

	private string srWV6U3Pyx;

	private int IyuVAnxn6u;

	private string VdZVGnUQup;

	private Stack<string> rx5VFQmh8m;

	private string J3rVbEDK36;

	private int J83Vy0muso;

	private char L02VPm5PXV;

	private int RfwVtfVZod;

	private bool JdXVpjSecw;

	private bool OrCVQ7C2Fx;

	private bool zhKVrCqAiq;

	private string pluVqB15rV;

	private string IsWVjW2dDs;

	private string KbuVZNZ5sH;

	private int j0QVowN9V2;

	private string NMfVSRGKOD;

	private string zT2VDM6VEC;

	private string pWIV1WBbY3;

	private string[] iBfVsTImxi;

	private string ltrVigPLAN;

	private string PeYV2O5fcU;

	private bool zEHV5uBXv3;

	private bool nipVXKrZfg;

	private string[] kiDVWRMM96;

	private bool I9mVCcISdj;

	private string xwpVUNrIYZ;

	private bool ROnVLMYjD8;

	private void oZS83VyDW()
	{
		while (ql6VKtSXMQ.Length > 0 && (ql6VKtSXMQ[ql6VKtSXMQ.Length - 1] == ' ' || ql6VKtSXMQ[ql6VKtSXMQ.Length - 1].ToString() == srWV6U3Pyx))
		{
			ql6VKtSXMQ.Remove(ql6VKtSXMQ.Length - 1, 1);
		}
	}

	private void T7quAwrpN(bool? P_0)
	{
		P_0 = P_0 ?? true;
		OrCVQ7C2Fx = false;
		oZS83VyDW();
		if (ql6VKtSXMQ.Length != 0)
		{
			if (ql6VKtSXMQ[ql6VKtSXMQ.Length - 1] != '\n' || !P_0.Value)
			{
				ql6VKtSXMQ.Append(Environment.NewLine);
			}
			for (int i = 0; i < IyuVAnxn6u; i++)
			{
				ql6VKtSXMQ.Append(srWV6U3Pyx);
			}
		}
	}

	private void LRSh45Vay()
	{
		string text = " ";
		if (ql6VKtSXMQ.Length > 0)
		{
			text = ql6VKtSXMQ[ql6VKtSXMQ.Length - 1].ToString();
		}
		if (text != " " && text != "\n" && text != srWV6U3Pyx)
		{
			ql6VKtSXMQ.Append(' ');
		}
	}

	private void cpB4klULA()
	{
		ql6VKtSXMQ.Append(VdZVGnUQup);
	}

	private void XBKJcAiSp()
	{
		IyuVAnxn6u++;
	}

	private void Yx80e9NCr()
	{
		if (IyuVAnxn6u > 0)
		{
			IyuVAnxn6u--;
		}
	}

	private void jkmzyDPsG()
	{
		if (ql6VKtSXMQ.Length > 0 && ql6VKtSXMQ[ql6VKtSXMQ.Length - 1].ToString() == srWV6U3Pyx)
		{
			ql6VKtSXMQ.Remove(ql6VKtSXMQ.Length - 1, 1);
		}
	}

	private void t1EVYhhJp8(string P_0)
	{
		rx5VFQmh8m.Push(J3rVbEDK36);
		J3rVbEDK36 = P_0;
	}

	private void pv4VVOlxrM()
	{
		zhKVrCqAiq = J3rVbEDK36 == "DO_BLOCK";
		J3rVbEDK36 = rx5VFQmh8m.Pop();
	}

	private bool vY7V9fdpIk(object P_0, ArrayList P_1)
	{
		return P_1.Contains(P_0);
	}

	private bool lcGV7rNZa1()
	{
		int num = 0;
		int num2 = 0;
		for (int num3 = ql6VKtSXMQ.Length - 1; num3 >= 0; num3--)
		{
			switch (ql6VKtSXMQ[num3])
			{
			case ':':
				if (num == 0)
				{
					num2++;
				}
				break;
			case '?':
				if (num == 0)
				{
					if (num2 == 0)
					{
						return true;
					}
					num2--;
				}
				break;
			case '{':
				if (num == 0)
				{
					return false;
				}
				num--;
				break;
			case '(':
			case '[':
				num--;
				break;
			case ')':
			case ']':
			case '}':
				num++;
				break;
			}
		}
		return false;
	}

	private string[] fTkVMmpuf0(ref int P_0)
	{
		int num = 0;
		if (P_0 < pluVqB15rV.Length)
		{
			string text = pluVqB15rV[P_0].ToString();
			P_0++;
			while (IsWVjW2dDs.Contains(text))
			{
				if (P_0 >= pluVqB15rV.Length)
				{
					return new string[2]
					{
						"",
						"TK_EOF"
					};
				}
				if (text == "\n")
				{
					num++;
				}
				text = pluVqB15rV[P_0].ToString();
				P_0++;
			}
			bool flag = false;
			if (JdXVpjSecw)
			{
				if (num > 1)
				{
					for (int i = 0; i < 2; i++)
					{
						T7quAwrpN(i == 0);
					}
				}
				flag = num == 1;
			}
			if (KbuVZNZ5sH.Contains(text))
			{
				if (P_0 < pluVqB15rV.Length)
				{
					while (KbuVZNZ5sH.Contains(pluVqB15rV[P_0]))
					{
						text += pluVqB15rV[P_0];
						P_0++;
						if (P_0 == pluVqB15rV.Length)
						{
							break;
						}
					}
				}
				if (P_0 != pluVqB15rV.Length && Regex.IsMatch(text, "^[0-9]+[Ee]$") && (pluVqB15rV[P_0] == '-' || pluVqB15rV[P_0] == '+'))
				{
					char c = pluVqB15rV[P_0];
					P_0++;
					string[] array = fTkVMmpuf0(ref P_0);
					text = text + c + array[0];
					return new string[2]
					{
						text,
						"TK_WORD"
					};
				}
				if (!(text == "in"))
				{
					if (flag && NMfVSRGKOD != "TK_OPERATOR" && !OrCVQ7C2Fx)
					{
						T7quAwrpN(null);
					}
					return new string[2]
					{
						text,
						"TK_WORD"
					};
				}
				return new string[2]
				{
					text,
					"TK_OPERATOR"
				};
			}
			if (!(text == "(") && !(text == "["))
			{
				if (!(text == ")") && !(text == "]"))
				{
					if (!(text == "{"))
					{
						if (!(text == "}"))
						{
							if (!(text == ";"))
							{
								if (text == "/")
								{
									string text2 = "";
									if (pluVqB15rV[P_0] == '*')
									{
										P_0++;
										if (P_0 < pluVqB15rV.Length)
										{
											while (pluVqB15rV[P_0] != '*' || pluVqB15rV[P_0 + 1] <= '\0' || pluVqB15rV[P_0 + 1] != '/' || P_0 >= pluVqB15rV.Length)
											{
												text2 += pluVqB15rV[P_0];
												P_0++;
												if (P_0 >= pluVqB15rV.Length)
												{
													break;
												}
											}
										}
										P_0 += 2;
										return new string[2]
										{
											"/*" + text2 + "*/",
											"TK_BLOCK_COMMENT"
										};
									}
									if (pluVqB15rV[P_0] == '/')
									{
										text2 = text;
										while (pluVqB15rV[P_0] != '\r' && pluVqB15rV[P_0] != '\n')
										{
											text2 += pluVqB15rV[P_0];
											P_0++;
											if (P_0 >= pluVqB15rV.Length)
											{
												break;
											}
										}
										P_0++;
										if (flag)
										{
											T7quAwrpN(null);
										}
										return new string[2]
										{
											text2,
											"TK_COMMENT"
										};
									}
								}
								if (text == "'" || text == "\"" || (text == "/" && ((NMfVSRGKOD == "TK_WORD" && zT2VDM6VEC == "return") || NMfVSRGKOD == "TK_START_EXPR" || NMfVSRGKOD == "TK_START_BLOCK" || NMfVSRGKOD == "TK_END_BLOCK" || NMfVSRGKOD == "TK_OPERATOR" || NMfVSRGKOD == "TK_EOF" || NMfVSRGKOD == "TK_SEMICOLON")))
								{
									string text3 = text;
									bool flag2 = false;
									string text4 = text;
									if (P_0 < pluVqB15rV.Length)
									{
										if (text3 == "/")
										{
											bool flag3 = false;
											while (flag2 || flag3 || pluVqB15rV[P_0].ToString() != text3)
											{
												text4 += pluVqB15rV[P_0];
												if (!flag2)
												{
													flag2 = pluVqB15rV[P_0] == '\\';
													if (pluVqB15rV[P_0] == '[')
													{
														flag3 = true;
													}
													else if (pluVqB15rV[P_0] == ']')
													{
														flag3 = false;
													}
												}
												else
												{
													flag2 = false;
												}
												P_0++;
												if (P_0 >= pluVqB15rV.Length)
												{
													return new string[2]
													{
														text4,
														"TK_STRING"
													};
												}
											}
										}
										else
										{
											while (flag2 || pluVqB15rV[P_0].ToString() != text3)
											{
												text4 += pluVqB15rV[P_0];
												flag2 = !flag2 && pluVqB15rV[P_0] == '\\';
												P_0++;
												if (P_0 >= pluVqB15rV.Length)
												{
													return new string[2]
													{
														text4,
														"TK_STRING"
													};
												}
											}
										}
									}
									P_0++;
									text4 += text3;
									if (text3 == "/")
									{
										while (P_0 < pluVqB15rV.Length && KbuVZNZ5sH.Contains(pluVqB15rV[P_0]))
										{
											text4 += pluVqB15rV[P_0];
											P_0++;
										}
									}
									return new string[2]
									{
										text4,
										"TK_STRING"
									};
								}
								if (text == "#")
								{
									string text5 = "#";
									if (P_0 < pluVqB15rV.Length && pWIV1WBbY3.Contains(pluVqB15rV[P_0]))
									{
										do
										{
											text = pluVqB15rV[P_0].ToString();
											text5 += text;
											P_0++;
										}
										while (P_0 < pluVqB15rV.Length && text != "#" && text != "=");
										if (!(text == "#"))
										{
											return new string[2]
											{
												text5,
												"TK_OPERATOR"
											};
										}
										return new string[2]
										{
											text5,
											"TK_WORD"
										};
									}
								}
								if (text == "<" && pluVqB15rV.Substring(P_0 - 1, 3) == "<!--")
								{
									P_0 += 3;
									return new string[2]
									{
										"<!--",
										"TK_COMMENT"
									};
								}
								if (text == "-" && pluVqB15rV.Substring(P_0 - 1, 2) == "-->")
								{
									P_0 += 2;
									if (flag)
									{
										T7quAwrpN(null);
									}
									return new string[2]
									{
										"-->",
										"TK_COMMENT"
									};
								}
								if (Enumerable.Contains(iBfVsTImxi, text))
								{
									while (P_0 < pluVqB15rV.Length && Enumerable.Contains(iBfVsTImxi, text + pluVqB15rV[P_0]))
									{
										text += pluVqB15rV[P_0];
										P_0++;
										if (P_0 >= pluVqB15rV.Length)
										{
											break;
										}
									}
									return new string[2]
									{
										text,
										"TK_OPERATOR"
									};
								}
								return new string[2]
								{
									text,
									"TK_UNKNOWN"
								};
							}
							return new string[2]
							{
								text,
								"TK_SEMICOLON"
							};
						}
						return new string[2]
						{
							text,
							"TK_END_BLOCK"
						};
					}
					return new string[2]
					{
						text,
						"TK_START_BLOCK"
					};
				}
				return new string[2]
				{
					text,
					"TK_END_EXPR"
				};
			}
			return new string[2]
			{
				text,
				"TK_START_EXPR"
			};
		}
		return new string[2]
		{
			"",
			"TK_EOF"
		};
	}

	public string GetResult()
	{
		if (ROnVLMYjD8)
		{
			ql6VKtSXMQ.AppendLine().AppendLine("</script>");
		}
		return ql6VKtSXMQ.ToString();
	}

	public JSBeautify(string js_source_text, JSBeautifyOptions options)
	{
		J83Vy0muso = options.indent_size ?? 1;
		L02VPm5PXV = options.indent_char ?? '\t';
		RfwVtfVZod = options.indent_level.GetValueOrDefault();
		JdXVpjSecw = options.preserve_newlines ?? true;
		ql6VKtSXMQ = new StringBuilder();
		rx5VFQmh8m = new Stack<string>();
		for (srWV6U3Pyx = ""; J83Vy0muso > 0; J83Vy0muso--)
		{
			srWV6U3Pyx += L02VPm5PXV;
		}
		IyuVAnxn6u = RfwVtfVZod;
		pluVqB15rV = js_source_text.Replace("<script type=\"text/javascript\">", "").Replace("</script>", "");
		if (pluVqB15rV.Length != js_source_text.Length)
		{
			ql6VKtSXMQ.AppendLine("<script type=\"text/javascript\">");
			ROnVLMYjD8 = true;
		}
		PeYV2O5fcU = "";
		NMfVSRGKOD = "TK_START_EXPR";
		zT2VDM6VEC = "";
		zhKVrCqAiq = false;
		zEHV5uBXv3 = false;
		nipVXKrZfg = false;
		IsWVjW2dDs = "\n\r\t ";
		KbuVZNZ5sH = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_$";
		pWIV1WBbY3 = "0123456789";
		iBfVsTImxi = "+ - * / % & ++ -- = += -= *= /= %= == === != !== > < >= <- <= >> << >>> >>>= >>= <<= && &= | || ! !! , : ? ^ ^= |= ::".Split(' ');
		kiDVWRMM96 = "continue,try,throw,return,if,switch,case,default,for,while,break,function".Split(',');
		J3rVbEDK36 = "BLOCK";
		rx5VFQmh8m.Push(J3rVbEDK36);
		j0QVowN9V2 = 0;
		I9mVCcISdj = false;
		while (true)
		{
			string[] array = fTkVMmpuf0(ref j0QVowN9V2);
			VdZVGnUQup = array[0];
			xwpVUNrIYZ = array[1];
			if (xwpVUNrIYZ == "TK_EOF")
			{
				break;
			}
			string text = xwpVUNrIYZ;
			if (text != null)
			{
				switch (text.Length)
				{
				case 11:
					switch (text[3])
					{
					case 'E':
						if (text == "TK_END_EXPR")
						{
							cpB4klULA();
							pv4VVOlxrM();
						}
						break;
					case 'O':
					{
						if (!(text == "TK_OPERATOR"))
						{
							break;
						}
						bool flag = true;
						bool flag2 = true;
						if (zEHV5uBXv3 && VdZVGnUQup != ",")
						{
							nipVXKrZfg = true;
							if (VdZVGnUQup == ":")
							{
								zEHV5uBXv3 = false;
							}
						}
						if (zEHV5uBXv3 && VdZVGnUQup == "," && J3rVbEDK36 == "EXPRESSION")
						{
							nipVXKrZfg = false;
						}
						if (VdZVGnUQup == ":" && I9mVCcISdj)
						{
							cpB4klULA();
							T7quAwrpN(null);
							I9mVCcISdj = false;
							break;
						}
						if (VdZVGnUQup == "::")
						{
							cpB4klULA();
							break;
						}
						if (VdZVGnUQup == ",")
						{
							if (zEHV5uBXv3)
							{
								if (nipVXKrZfg)
								{
									cpB4klULA();
									T7quAwrpN(null);
									nipVXKrZfg = false;
								}
								else
								{
									cpB4klULA();
									LRSh45Vay();
								}
							}
							else if (NMfVSRGKOD == "TK_END_BLOCK")
							{
								cpB4klULA();
								T7quAwrpN(null);
							}
							else if (J3rVbEDK36 == "BLOCK")
							{
								cpB4klULA();
								T7quAwrpN(null);
							}
							else
							{
								cpB4klULA();
								LRSh45Vay();
							}
							break;
						}
						if (VdZVGnUQup == "--" || VdZVGnUQup == "++")
						{
							if (zT2VDM6VEC == ";")
							{
								if (J3rVbEDK36 == "BLOCK")
								{
									T7quAwrpN(null);
									flag = true;
									flag2 = false;
								}
								else
								{
									flag = true;
									flag2 = false;
								}
							}
							else
							{
								if (zT2VDM6VEC == "{")
								{
									T7quAwrpN(null);
								}
								flag = false;
								flag2 = false;
							}
						}
						else if ((VdZVGnUQup == "!" || VdZVGnUQup == "+" || VdZVGnUQup == "-") && (zT2VDM6VEC == "return" || zT2VDM6VEC == "case"))
						{
							flag = true;
							flag2 = false;
						}
						else if ((VdZVGnUQup == "!" || VdZVGnUQup == "+" || VdZVGnUQup == "-") && NMfVSRGKOD == "TK_START_EXPR")
						{
							flag = false;
							flag2 = false;
						}
						else if (NMfVSRGKOD == "TK_OPERATOR")
						{
							flag = false;
							flag2 = false;
						}
						else if (NMfVSRGKOD == "TK_END_EXPR")
						{
							flag = true;
							flag2 = true;
						}
						else if (VdZVGnUQup == ".")
						{
							flag = false;
							flag2 = false;
						}
						else if (VdZVGnUQup == ":")
						{
							flag = (lcGV7rNZa1() ? true : false);
						}
						if (flag)
						{
							LRSh45Vay();
						}
						cpB4klULA();
						if (flag2)
						{
							LRSh45Vay();
						}
						break;
					}
					}
					break;
				case 12:
					switch (text[3])
					{
					case 'E':
						if (text == "TK_END_BLOCK")
						{
							if (NMfVSRGKOD == "TK_START_BLOCK")
							{
								oZS83VyDW();
								Yx80e9NCr();
							}
							else
							{
								Yx80e9NCr();
								T7quAwrpN(null);
							}
							cpB4klULA();
							pv4VVOlxrM();
						}
						break;
					case 'S':
						if (text == "TK_SEMICOLON")
						{
							cpB4klULA();
							zEHV5uBXv3 = false;
						}
						break;
					}
					break;
				case 10:
					switch (text[3])
					{
					case 'C':
						if (text == "TK_COMMENT")
						{
							LRSh45Vay();
							cpB4klULA();
							T7quAwrpN(null);
						}
						break;
					case 'U':
						if (text == "TK_UNKNOWN")
						{
							cpB4klULA();
						}
						break;
					}
					break;
				case 13:
					if (!(text == "TK_START_EXPR"))
					{
						break;
					}
					zEHV5uBXv3 = false;
					t1EVYhhJp8("EXPRESSION");
					if (zT2VDM6VEC == ";" || NMfVSRGKOD == "TK_START_BLOCK")
					{
						T7quAwrpN(null);
					}
					else if (!(NMfVSRGKOD == "TK_END_EXPR") && !(NMfVSRGKOD == "TK_START_EXPR"))
					{
						if (NMfVSRGKOD != "TK_WORD" && NMfVSRGKOD != "TK_OPERATOR")
						{
							LRSh45Vay();
						}
						else if (Enumerable.Contains(kiDVWRMM96, PeYV2O5fcU))
						{
							LRSh45Vay();
						}
					}
					cpB4klULA();
					break;
				case 14:
					if (!(text == "TK_START_BLOCK"))
					{
						break;
					}
					if (PeYV2O5fcU == "do")
					{
						t1EVYhhJp8("DO_BLOCK");
					}
					else
					{
						t1EVYhhJp8("BLOCK");
					}
					if (NMfVSRGKOD != "TK_OPERATOR" && NMfVSRGKOD != "TK_START_EXPR")
					{
						if (NMfVSRGKOD == "TK_START_BLOCK")
						{
							T7quAwrpN(null);
						}
						else
						{
							LRSh45Vay();
						}
					}
					cpB4klULA();
					XBKJcAiSp();
					break;
				case 7:
					if (!(text == "TK_WORD"))
					{
						break;
					}
					if (zhKVrCqAiq)
					{
						LRSh45Vay();
						cpB4klULA();
						LRSh45Vay();
						zhKVrCqAiq = false;
						break;
					}
					if (VdZVGnUQup == "case" || VdZVGnUQup == "default")
					{
						if (zT2VDM6VEC == ":")
						{
							jkmzyDPsG();
						}
						else
						{
							Yx80e9NCr();
							T7quAwrpN(null);
							XBKJcAiSp();
						}
						cpB4klULA();
						I9mVCcISdj = true;
						break;
					}
					ltrVigPLAN = "NONE";
					if (NMfVSRGKOD == "TK_END_BLOCK")
					{
						if (!Enumerable.Contains(new string[3]
						{
							"else",
							"catch",
							"finally"
						}, VdZVGnUQup.ToLower()))
						{
							ltrVigPLAN = "NEWLINE";
						}
						else
						{
							ltrVigPLAN = "SPACE";
							LRSh45Vay();
						}
					}
					else if (NMfVSRGKOD == "TK_SEMICOLON" && (J3rVbEDK36 == "BLOCK" || J3rVbEDK36 == "DO_BLOCK"))
					{
						ltrVigPLAN = "NEWLINE";
					}
					else if (NMfVSRGKOD == "TK_SEMICOLON" && J3rVbEDK36 == "EXPRESSION")
					{
						ltrVigPLAN = "SPACE";
					}
					else if (NMfVSRGKOD == "TK_STRING")
					{
						ltrVigPLAN = "NEWLINE";
					}
					else if (NMfVSRGKOD == "TK_WORD")
					{
						ltrVigPLAN = "SPACE";
					}
					else if (NMfVSRGKOD == "TK_START_BLOCK")
					{
						ltrVigPLAN = "NEWLINE";
					}
					else if (NMfVSRGKOD == "TK_END_EXPR")
					{
						LRSh45Vay();
						ltrVigPLAN = "NEWLINE";
					}
					if (NMfVSRGKOD != "TK_END_BLOCK" && Enumerable.Contains(new string[3]
					{
						"else",
						"catch",
						"finally"
					}, VdZVGnUQup.ToLower()))
					{
						T7quAwrpN(null);
					}
					else if (Enumerable.Contains(kiDVWRMM96, VdZVGnUQup) || ltrVigPLAN == "NEWLINE")
					{
						if (zT2VDM6VEC == "else")
						{
							LRSh45Vay();
						}
						else if ((!(NMfVSRGKOD == "TK_START_EXPR") && !(zT2VDM6VEC == "=") && !(zT2VDM6VEC == ",")) || !(VdZVGnUQup == "function"))
						{
							if (NMfVSRGKOD == "TK_WORD" && (zT2VDM6VEC == "return" || zT2VDM6VEC == "throw"))
							{
								LRSh45Vay();
							}
							else if (NMfVSRGKOD != "TK_END_EXPR")
							{
								if ((NMfVSRGKOD != "TK_START_EXPR" || VdZVGnUQup != "var") && zT2VDM6VEC != ":")
								{
									if (VdZVGnUQup == "if" && NMfVSRGKOD == "TK_WORD" && PeYV2O5fcU == "else")
									{
										LRSh45Vay();
									}
									else
									{
										T7quAwrpN(null);
									}
								}
							}
							else if (Enumerable.Contains(kiDVWRMM96, VdZVGnUQup) && zT2VDM6VEC != ")")
							{
								T7quAwrpN(null);
							}
						}
					}
					else if (ltrVigPLAN == "SPACE")
					{
						LRSh45Vay();
					}
					cpB4klULA();
					PeYV2O5fcU = VdZVGnUQup;
					if (VdZVGnUQup == "var")
					{
						zEHV5uBXv3 = true;
						nipVXKrZfg = false;
					}
					if (VdZVGnUQup == "if" || VdZVGnUQup == "else")
					{
						OrCVQ7C2Fx = true;
					}
					break;
				case 9:
					if (text == "TK_STRING")
					{
						if (NMfVSRGKOD == "TK_START_BLOCK" || NMfVSRGKOD == "TK_END_BLOCK" || NMfVSRGKOD == "TK_SEMICOLON")
						{
							T7quAwrpN(null);
						}
						else if (NMfVSRGKOD == "TK_WORD")
						{
							LRSh45Vay();
						}
						cpB4klULA();
					}
					break;
				case 16:
					if (text == "TK_BLOCK_COMMENT")
					{
						T7quAwrpN(null);
						cpB4klULA();
						T7quAwrpN(null);
					}
					break;
				}
			}
			NMfVSRGKOD = xwpVUNrIYZ;
			zT2VDM6VEC = VdZVGnUQup;
		}
	}
}
