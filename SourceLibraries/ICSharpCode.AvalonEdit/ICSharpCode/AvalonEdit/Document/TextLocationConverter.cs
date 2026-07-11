using System;
using System.ComponentModel;
using System.Globalization;

namespace ICSharpCode.AvalonEdit.Document;

public class TextLocationConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (!(sourceType == typeof(string)))
		{
			return base.CanConvertFrom(context, sourceType);
		}
		return true;
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (!(destinationType == typeof(TextLocation)))
		{
			return base.CanConvertTo(context, destinationType);
		}
		return true;
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value is string)
		{
			string[] array = ((string)value).Split(';', ',');
			if (array.Length == 2)
			{
				return new TextLocation(int.Parse(array[0], culture), int.Parse(array[1], culture));
			}
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is TextLocation && destinationType == typeof(string))
		{
			TextLocation textLocation = (TextLocation)value;
			return textLocation.Line.ToString(culture) + ";" + textLocation.Column.ToString(culture);
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}
}
