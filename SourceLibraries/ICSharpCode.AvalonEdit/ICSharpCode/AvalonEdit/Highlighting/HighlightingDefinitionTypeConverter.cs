using System;
using System.ComponentModel;
using System.Globalization;

namespace ICSharpCode.AvalonEdit.Highlighting;

public sealed class HighlightingDefinitionTypeConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (sourceType == typeof(string))
		{
			return true;
		}
		return base.CanConvertFrom(context, sourceType);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value is string name)
		{
			return HighlightingManager.Instance.GetDefinition(name);
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (destinationType == typeof(string))
		{
			return true;
		}
		return base.CanConvertTo(context, destinationType);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is IHighlightingDefinition highlightingDefinition && destinationType == typeof(string))
		{
			return highlightingDefinition.Name;
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}
}
