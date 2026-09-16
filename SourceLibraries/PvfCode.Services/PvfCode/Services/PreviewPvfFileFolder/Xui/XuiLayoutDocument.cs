using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace PvfCode.Services.PreviewPvfFileFolder.Xui;

/// <summary>
/// PVF 内 .xui 布局文件（UTF-16LE XML）的解析模型。
/// 格式依据：90CN 提取原件实测（ROOT→TOOL→CONTROL 平铺控件、STR:/NUM:/VECTOR:/BOOL:/COLOR: 类型前缀、
/// 根闭合后存在重复冗余闭合标签与尾随空白）。解析读到根元素闭合即停，不消费尾部冗余内容。
/// </summary>
public sealed class XuiLayoutDocument
{
	public const double DefaultCanvasWidth = 800;

	public const double DefaultCanvasHeight = 600;

	public string ScriptVersion { get; private set; }

	public string ToolVersion { get; private set; }

	/// <summary>顶层控件（CONTROL 容器内或根下的直接子控件），保持文档顺序。</summary>
	public List<XuiControl> Controls { get; } = new();

	/// <summary>全部控件（含嵌套子控件），文档顺序即层内绘制顺序。</summary>
	public List<XuiControl> AllControls { get; } = new();

	public string ParseError { get; private set; }

	public int TotalCount => AllControls.Count;

	public int ImageControlCount { get; private set; }

	public int AnimationControlCount { get; private set; }

	public int TextControlCount { get; private set; }

	public int ContainerCount { get; private set; }

	public int HiddenCount { get; private set; }

	public int NoPosCount { get; private set; }

	/// <summary>按文档顺序去重的 ImagePack 引用（已剥离 STR: 前缀）。</summary>
	public List<string> ImagePackReferences { get; } = new();

	public static XuiLayoutDocument Parse(string xmlText)
	{
		XuiLayoutDocument document = new();
		if (string.IsNullOrWhiteSpace(xmlText))
		{
			document.ParseError = "文件内容为空。";
			return document;
		}
		string text = xmlText.TrimStart('\uFEFF');
		XmlReaderSettings settings = new()
		{
			DtdProcessing = DtdProcessing.Ignore,
			XmlResolver = null,
			IgnoreComments = true,
			IgnoreProcessingInstructions = true,
			CloseInput = true,
			CheckCharacters = false
		};
		try
		{
			using XmlReader reader = XmlReader.Create(new StringReader(text), settings);
			IXmlLineInfo lineInfo = (IXmlLineInfo)reader;
			Stack<XuiControl> containerStack = new();
			bool seenRoot = false;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Depth == 0)
				{
					// 根元素闭合：后面的 </CONTROL></ROOT> 冗余尾注不再消费。
					break;
				}
				if (reader.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				string name = reader.LocalName;
				if (!seenRoot)
				{
					seenRoot = true;
					if (string.Equals(name, "ROOT", StringComparison.OrdinalIgnoreCase) && reader.HasAttributes)
					{
						while (reader.MoveToNextAttribute())
						{
							if (string.Equals(reader.Name, "ScriptVer", StringComparison.OrdinalIgnoreCase))
							{
								document.ScriptVersion = reader.Value;
							}
						}
					}
					continue;
				}
				if (string.Equals(name, "TOOL", StringComparison.OrdinalIgnoreCase))
				{
					if (reader.HasAttributes)
					{
						while (reader.MoveToNextAttribute())
						{
							if (string.Equals(reader.Name, "ToolVer", StringComparison.OrdinalIgnoreCase))
							{
								document.ToolVersion = reader.Value;
							}
						}
					}
					continue;
				}
				bool isEmpty = reader.IsEmptyElement;
				bool isBareControlContainer = string.Equals(name, "CONTROL", StringComparison.OrdinalIgnoreCase);
				XuiControl control = null;
				if (isBareControlContainer && !HasAnyAttribute(reader))
				{
					// 结构性 <CONTROL> 容器：不入控件列表，仅作为层级通道。
					containerStack.Push(null);
					if (!isEmpty)
					{
						continue;
					}
					containerStack.Pop();
					continue;
				}
				control = new XuiControl(name, lineInfo?.LineNumber ?? 0, lineInfo?.LinePosition ?? 0, containerStack.Count);
				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						control.Attributes[reader.LocalName] = reader.Value;
					}
					reader.MoveToElement();
				}
				control.Parent = containerStack.Count > 0 ? containerStack.Peek() : null;
				document.RegisterControl(control);
				if (control.Parent != null)
				{
					control.Parent.Children.Add(control);
				}
				else
				{
					document.Controls.Add(control);
				}
				if (!isEmpty)
				{
					containerStack.Push(control);
				}
			}
			document.RebuildStatistics();
			if (document.TotalCount == 0)
			{
				document.ParseError = "没有解析到 XUI 控件。";
			}
			return document;
		}
		catch (Exception exception)
		{
			document.ParseError = "XUI 解析失败：" + exception.Message;
			return document;
		}
	}

	private static bool HasAnyAttribute(XmlReader reader)
	{
		if (!reader.HasAttributes)
		{
			return false;
		}
		bool hasAttribute = false;
		while (reader.MoveToNextAttribute())
		{
			hasAttribute = true;
			break;
		}
		reader.MoveToElement();
		return hasAttribute;
	}

	private void RegisterControl(XuiControl control)
	{
		AllControls.Add(control);
		switch (control.Kind)
		{
		case XuiControlKind.Image:
			ImageControlCount++;
			break;
		case XuiControlKind.Animation:
			AnimationControlCount++;
			break;
		case XuiControlKind.Text:
			TextControlCount++;
			break;
		case XuiControlKind.Container:
			ContainerCount++;
			break;
		}
		string imagePack = control.ImagePack;
		if (!string.IsNullOrEmpty(imagePack) && control.ImageIndex.HasValue &&
			!ImagePackReferences.Contains(imagePack, StringComparer.OrdinalIgnoreCase))
		{
			ImagePackReferences.Add(imagePack);
		}
	}

	private void RebuildStatistics()
	{
		HiddenCount = AllControls.Count(control => !control.IsVisible);
		NoPosCount = AllControls.Count(control => !control.HasPos && control.Kind != XuiControlKind.Container);
	}
}

public enum XuiControlKind
{
	Image,
	Animation,
	Text,
	Container,
	Other
}

/// <summary>XUI 单个控件节点：类型名 + 全部原始属性（保留原文，类型化读取按需剥离前缀）。</summary>
public sealed class XuiControl
{
	public XuiControl(string typeName, int lineNumber, int linePosition, int depth)
	{
		TypeName = typeName;
		LineNumber = lineNumber;
		LinePosition = linePosition;
		Depth = depth;
	}

	public string TypeName { get; }

	public int LineNumber { get; }

	/// <summary>元素在行内的 1 基列位置（即 '&lt;' 所在列）。</summary>
	public int LinePosition { get; }

	public int Depth { get; }

	public XuiControl Parent { get; internal set; }

	public List<XuiControl> Children { get; } = new();

	/// <summary>原始属性表（键不区分大小写，值为含类型前缀的原文）。</summary>
	public Dictionary<string, string> Attributes { get; } = new(StringComparer.OrdinalIgnoreCase);

	public XuiControlKind Kind
	{
		get
		{
			if (!string.IsNullOrEmpty(AnimationPath))
			{
				return XuiControlKind.Animation;
			}
			switch (TypeName ?? string.Empty)
			{
			case "CNUIControlImage":
			case "CNBoardImage":
			case "CNScreenImage":
				return ImagePack != null ? XuiControlKind.Image : XuiControlKind.Other;
			case "CNUIControlText":
				return XuiControlKind.Text;
			case "CNUIControlAnimationForRDAR":
				return XuiControlKind.Animation;
			case "IControl":
			case "POPUPWINDOW":
				return Children.Count > 0 || string.IsNullOrEmpty(ImagePack) ? XuiControlKind.Container : XuiControlKind.Image;
			default:
				return ImagePack != null ? XuiControlKind.Image : XuiControlKind.Other;
			}
		}
	}

	public string RawId => GetRaw("ID");

	public string Id => StripPrefix(RawId);

	public string ImagePack => StripPrefix(GetRaw("ImagePack"));

	public int? ImageIndex => GetInt("imageIndex");

	public bool HasPos => PosX.HasValue || PosY.HasValue;

	public double? PosX => GetVector("Pos", 0);

	public double? PosY => GetVector("Pos", 1);

	public double? Width => GetVector("Size", 0);

	public double? Height => GetVector("Size", 1);

	public bool IsVisible => !string.Equals(StripPrefix(GetRaw("IsVisible")), "false", StringComparison.OrdinalIgnoreCase);

	public bool IsEnable => !string.Equals(StripPrefix(GetRaw("IsEnable")), "false", StringComparison.OrdinalIgnoreCase);

	public int? Alpha => GetInt("Alpha");

	/// <summary>文本颜色 COLOR:r, g, b 的红色分量（存在 COLOR: 前缀时）。</summary>
	public string TextColor => StripPrefix(GetRaw("TextColor"));

	public string Font => StripPrefix(GetRaw("font"));

	public string AnimationPath => StripPrefix(GetRaw("AnimationPath"));

	public string EffectType => StripPrefix(GetRaw("effectType"));

	public string StringContent => StripPrefix(GetRaw("string"));

	/// <summary>九宫格帧索引（LeftTop/Top/.../RightBottomImgIndex，属性名存在 RightBottomIndex 变体）。</summary>
	public IReadOnlyDictionary<string, int> BoardIndexes
	{
		get
		{
			Dictionary<string, int> indexes = new(StringComparer.OrdinalIgnoreCase);
			foreach (KeyValuePair<string, string> attribute in Attributes)
			{
				if (attribute.Key.EndsWith("ImgIndex", StringComparison.OrdinalIgnoreCase) ||
					string.Equals(attribute.Key, "RightBottomIndex", StringComparison.OrdinalIgnoreCase))
				{
					if (int.TryParse(StripPrefix(attribute.Value), NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
					{
						indexes[attribute.Key] = index;
					}
				}
			}
			return indexes;
		}
	}

	public bool IsNinePatch => TypeName == "CNBoardImage" || string.Equals(StripPrefix(GetRaw("IsNinePatch")), "true", StringComparison.OrdinalIgnoreCase);

	public string GetRaw(string name)
	{
		return Attributes.TryGetValue(name, out string value) ? value : null;
	}

	public string GetDisplay(string name)
	{
		return StripPrefix(GetRaw(name));
	}

	private static string StripPrefix(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		foreach (string prefix in new[] { "STR:", "NUM:", "BOOL:", "VECTOR:", "COLOR:" })
		{
			if (value.StartsWith(prefix, StringComparison.Ordinal))
			{
				return value.Substring(prefix.Length).Trim();
			}
		}
		return value.Trim();
	}

	public int? GetInt(string name)
	{
		string value = StripPrefix(GetRaw(name));
		return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : null;
	}

	public double? GetVector(string name, int component)
	{
		string value = StripPrefix(GetRaw(name));
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		string[] parts = value.Split(',');
		if (component >= parts.Length)
		{
			return null;
		}
		return double.TryParse(parts[component].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double result) ? result : null;
	}

	public string ToolTipSummary
	{
		get
		{
			StringBuilder builder = new();
			builder.Append(TypeName);
			if (!string.IsNullOrEmpty(Id))
			{
				builder.Append("  ID=").Append(Id);
			}
			builder.AppendLine().Append("第 ").Append(LineNumber).Append(" 行");
			if (HasPos)
			{
				builder.Append("   Pos=(").Append(PosX?.ToString("0.#", CultureInfo.InvariantCulture)).Append(", ")
					.Append(PosY?.ToString("0.#", CultureInfo.InvariantCulture)).Append(')');
			}
			if (Width.HasValue || Height.HasValue)
			{
				builder.Append("   Size=(").Append(Width?.ToString("0.#", CultureInfo.InvariantCulture)).Append(", ")
					.Append(Height?.ToString("0.#", CultureInfo.InvariantCulture)).Append(')');
			}
			if (ImagePack != null)
			{
				builder.AppendLine().Append(ImagePack).Append("  #").Append(ImageIndex?.ToString(CultureInfo.InvariantCulture) ?? "?");
			}
			if (!string.IsNullOrEmpty(AnimationPath))
			{
				builder.AppendLine().Append(AnimationPath);
			}
			return builder.ToString();
		}
	}
}
