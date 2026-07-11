using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

[Serializable]
public class XshdColor : XshdElement, ISerializable
{
	public string Name { get; set; }

	public FontFamily FontFamily { get; set; }

	public int? FontSize { get; set; }

	public HighlightingBrush Foreground { get; set; }

	public HighlightingBrush Background { get; set; }

	public FontWeight? FontWeight { get; set; }

	public bool? Underline { get; set; }

	public bool? Strikethrough { get; set; }

	public FontStyle? FontStyle { get; set; }

	public string ExampleText { get; set; }

	public XshdColor()
	{
	}

	protected XshdColor(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		Name = info.GetString("Name");
		Foreground = (HighlightingBrush)info.GetValue("Foreground", typeof(HighlightingBrush));
		Background = (HighlightingBrush)info.GetValue("Background", typeof(HighlightingBrush));
		if (info.GetBoolean("HasWeight"))
		{
			FontWeight = System.Windows.FontWeight.FromOpenTypeWeight(info.GetInt32("Weight"));
		}
		if (info.GetBoolean("HasStyle"))
		{
			FontStyle = (FontStyle?)new FontStyleConverter().ConvertFromInvariantString(info.GetString("Style"));
		}
		ExampleText = info.GetString("ExampleText");
		if (info.GetBoolean("HasUnderline"))
		{
			Underline = info.GetBoolean("Underline");
		}
		if (info.GetBoolean("HasStrikethrough"))
		{
			Strikethrough = info.GetBoolean("Strikethrough");
		}
		if (info.GetBoolean("HasFamily"))
		{
			FontFamily = new FontFamily(info.GetString("Family"));
		}
		if (info.GetBoolean("HasSize"))
		{
			FontSize = info.GetInt32("Size");
		}
	}

	[SecurityCritical]
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		info.AddValue("Name", Name);
		info.AddValue("Foreground", Foreground);
		info.AddValue("Background", Background);
		info.AddValue("HasUnderline", Underline.HasValue);
		if (Underline.HasValue)
		{
			info.AddValue("Underline", Underline.Value);
		}
		info.AddValue("HasStrikethrough", Strikethrough.HasValue);
		if (Strikethrough.HasValue)
		{
			info.AddValue("Strikethrough", Strikethrough.Value);
		}
		info.AddValue("HasWeight", FontWeight.HasValue);
		if (FontWeight.HasValue)
		{
			info.AddValue("Weight", FontWeight.Value.ToOpenTypeWeight());
		}
		info.AddValue("HasStyle", FontStyle.HasValue);
		if (FontStyle.HasValue)
		{
			info.AddValue("Style", FontStyle.Value.ToString());
		}
		info.AddValue("ExampleText", ExampleText);
		info.AddValue("HasFamily", FontFamily != null);
		if (FontFamily != null)
		{
			info.AddValue("Family", FontFamily.FamilyNames.FirstOrDefault());
		}
		info.AddValue("HasSize", FontSize.HasValue);
		if (FontSize.HasValue)
		{
			info.AddValue("Size", FontSize.Value.ToString());
		}
	}

	public override object AcceptVisitor(IXshdVisitor visitor)
	{
		return visitor.VisitColor(this);
	}
}
