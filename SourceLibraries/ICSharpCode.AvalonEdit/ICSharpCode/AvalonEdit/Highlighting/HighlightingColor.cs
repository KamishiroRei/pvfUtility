using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting;

[Serializable]
public class HighlightingColor : ISerializable, IFreezable, ICloneable, IEquatable<HighlightingColor>
{
	internal static readonly HighlightingColor Empty = FreezableHelper.FreezeAndReturn(new HighlightingColor());

	private string name;

	private FontFamily fontFamily;

	private int? fontSize;

	private FontWeight? fontWeight;

	private FontStyle? fontStyle;

	private bool? underline;

	private bool? strikethrough;

	private HighlightingBrush foreground;

	private HighlightingBrush background;

	private bool frozen;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			if (frozen)
			{
				throw new InvalidOperationException();
			}
			name = value;
		}
	}

	public FontFamily FontFamily
	{
		get
		{
			return fontFamily;
		}
		set
		{
			if (frozen)
			{
				throw new InvalidOperationException();
			}
			fontFamily = value;
		}
	}

	public int? FontSize
	{
		get
		{
			return fontSize;
		}
		set
		{
			if (frozen)
			{
				throw new InvalidOperationException();
			}
			fontSize = value;
		}
	}

	public FontWeight? FontWeight
	{
		get
		{
			return fontWeight;
		}
		set
		{
			if (frozen)
			{
				throw new InvalidOperationException();
			}
			fontWeight = value;
		}
	}

	public FontStyle? FontStyle
	{
		get
		{
			return fontStyle;
		}
		set
		{
			if (frozen)
			{
				throw new InvalidOperationException();
			}
			fontStyle = value;
		}
	}

	public bool? Underline
	{
		get
		{
			return underline;
		}
		set
		{
			if (frozen)
			{
				throw new InvalidOperationException();
			}
			underline = value;
		}
	}

	public bool? Strikethrough
	{
		get
		{
			return strikethrough;
		}
		set
		{
			if (frozen)
			{
				throw new InvalidOperationException();
			}
			strikethrough = value;
		}
	}

	public HighlightingBrush Foreground
	{
		get
		{
			return foreground;
		}
		set
		{
			if (frozen)
			{
				throw new InvalidOperationException();
			}
			foreground = value;
		}
	}

	public HighlightingBrush Background
	{
		get
		{
			return background;
		}
		set
		{
			if (frozen)
			{
				throw new InvalidOperationException();
			}
			background = value;
		}
	}

	public bool IsFrozen => frozen;

	internal bool IsEmptyForMerge
	{
		get
		{
			if (!fontWeight.HasValue && !fontStyle.HasValue && !underline.HasValue && !strikethrough.HasValue && foreground == null && background == null && fontFamily == null)
			{
				return !fontSize.HasValue;
			}
			return false;
		}
	}

	public HighlightingColor()
	{
	}

	protected HighlightingColor(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		Name = info.GetString("Name");
		if (info.GetBoolean("HasWeight"))
		{
			FontWeight = System.Windows.FontWeight.FromOpenTypeWeight(info.GetInt32("Weight"));
		}
		if (info.GetBoolean("HasStyle"))
		{
			FontStyle = (FontStyle?)new FontStyleConverter().ConvertFromInvariantString(info.GetString("Style"));
		}
		if (info.GetBoolean("HasUnderline"))
		{
			Underline = info.GetBoolean("Underline");
		}
		if (info.GetBoolean("HasStrikethrough"))
		{
			Strikethrough = info.GetBoolean("Strikethrough");
		}
		Foreground = (HighlightingBrush)info.GetValue("Foreground", typeof(SimpleHighlightingBrush));
		Background = (HighlightingBrush)info.GetValue("Background", typeof(SimpleHighlightingBrush));
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
		info.AddValue("Foreground", Foreground);
		info.AddValue("Background", Background);
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

	public virtual string ToCss()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (Foreground != null)
		{
			Color? color = Foreground.GetColor(null);
			if (color.HasValue)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "color: #{0:x2}{1:x2}{2:x2}; ", color.Value.R, color.Value.G, color.Value.B);
			}
		}
		if (Background != null)
		{
			Color? color2 = Background.GetColor(null);
			if (color2.HasValue)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "background-color: #{0:x2}{1:x2}{2:x2}; ", color2.Value.R, color2.Value.G, color2.Value.B);
			}
		}
		if (FontWeight.HasValue)
		{
			stringBuilder.Append("font-weight: ");
			stringBuilder.Append(FontWeight.Value.ToString().ToLowerInvariant());
			stringBuilder.Append("; ");
		}
		if (FontStyle.HasValue)
		{
			stringBuilder.Append("font-style: ");
			stringBuilder.Append(FontStyle.Value.ToString().ToLowerInvariant());
			stringBuilder.Append("; ");
		}
		if (Underline.HasValue)
		{
			stringBuilder.Append("text-decoration: ");
			stringBuilder.Append(Underline.Value ? "underline" : "none");
			stringBuilder.Append("; ");
		}
		if (Strikethrough.HasValue)
		{
			if (!Underline.HasValue)
			{
				stringBuilder.Append("text-decoration:  ");
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder.Append(Strikethrough.Value ? " line-through" : " none");
			stringBuilder.Append("; ");
		}
		return stringBuilder.ToString();
	}

	public override string ToString()
	{
		return "[" + GetType().Name + " " + (string.IsNullOrEmpty(Name) ? ToCss() : Name) + "]";
	}

	public virtual void Freeze()
	{
		frozen = true;
	}

	public virtual HighlightingColor Clone()
	{
		HighlightingColor obj = (HighlightingColor)MemberwiseClone();
		obj.frozen = false;
		return obj;
	}

	object ICloneable.Clone()
	{
		return Clone();
	}

	public sealed override bool Equals(object obj)
	{
		return Equals(obj as HighlightingColor);
	}

	public virtual bool Equals(HighlightingColor other)
	{
		if (other == null)
		{
			return false;
		}
		if (name == other.name && fontWeight == other.fontWeight)
		{
			FontStyle? fontStyle = this.fontStyle;
			FontStyle? fontStyle2 = other.fontStyle;
			if (fontStyle.HasValue == fontStyle2.HasValue && (!fontStyle.HasValue || fontStyle.GetValueOrDefault() == fontStyle2.GetValueOrDefault()) && underline == other.underline && strikethrough == other.strikethrough && object.Equals(foreground, other.foreground) && object.Equals(background, other.background) && object.Equals(fontFamily, other.fontFamily))
			{
				return object.Equals(FontSize, other.FontSize);
			}
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = 0;
		if (name != null)
		{
			num += 1000000007 * name.GetHashCode();
		}
		num += 1000000009 * fontWeight.GetHashCode();
		num += 1000000021 * fontStyle.GetHashCode();
		if (foreground != null)
		{
			num += 1000000033 * foreground.GetHashCode();
		}
		if (background != null)
		{
			num += 1000000087 * background.GetHashCode();
		}
		if (fontFamily != null)
		{
			num += 1000000123 * fontFamily.GetHashCode();
		}
		if (fontSize.HasValue)
		{
			num += 1000000167 * fontSize.GetHashCode();
		}
		return num;
	}

	public void MergeWith(HighlightingColor color)
	{
		FreezableHelper.ThrowIfFrozen(this);
		if (color.fontWeight.HasValue)
		{
			fontWeight = color.fontWeight;
		}
		if (color.fontStyle.HasValue)
		{
			fontStyle = color.fontStyle;
		}
		if (color.foreground != null)
		{
			foreground = color.foreground;
		}
		if (color.background != null)
		{
			background = color.background;
		}
		if (color.underline.HasValue)
		{
			underline = color.underline;
		}
		if (color.strikethrough.HasValue)
		{
			strikethrough = color.strikethrough;
		}
		if (color.fontFamily != null)
		{
			fontFamily = color.fontFamily;
		}
		if (color.fontSize.HasValue)
		{
			fontSize = color.fontSize;
		}
	}
}
