using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Highlighting;

[Serializable]
internal sealed class SystemColorHighlightingBrush : HighlightingBrush, ISerializable
{
	private readonly PropertyInfo property;

	public SystemColorHighlightingBrush(PropertyInfo property)
	{
		this.property = property;
	}

	public override Brush GetBrush(ITextRunConstructionContext context)
	{
		return (Brush)property.GetValue(null, null);
	}

	public override string ToString()
	{
		return property.Name;
	}

	private SystemColorHighlightingBrush(SerializationInfo info, StreamingContext context)
	{
		property = typeof(SystemColors).GetProperty(info.GetString("propertyName"));
		if (property == null)
		{
			throw new ArgumentException("Error deserializing SystemColorHighlightingBrush");
		}
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("propertyName", property.Name);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is SystemColorHighlightingBrush systemColorHighlightingBrush))
		{
			return false;
		}
		return object.Equals(property, systemColorHighlightingBrush.property);
	}

	public override int GetHashCode()
	{
		return property.GetHashCode();
	}
}
