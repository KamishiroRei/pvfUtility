using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Windows;
using ICSharpCode.AvalonEdit.Highlighting;

namespace HL.Xshtd;

[Serializable]
public class XshtdColor : XshtdElement, ISerializable
{
	private readonly XshtdSyntaxDefinition _syntax;

	public string Name { get; set; }

	public HighlightingBrush Foreground { get; set; }

	public HighlightingBrush Background { get; set; }

	public FontWeight? FontWeight { get; set; }

	public bool? Underline { get; set; }

	public FontStyle? FontStyle { get; set; }

	public string ExampleText { get; set; }

	public XshtdColor(XshtdSyntaxDefinition syntax)
	{
		_syntax = syntax;
	}

	protected XshtdColor(SerializationInfo info, StreamingContext context)
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
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
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
	}

	public override object AcceptVisitor(IXshtdVisitor visitor)
	{
		return visitor.VisitColor(_syntax, this);
	}
}
