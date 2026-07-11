using System;
using System.Windows;

namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class LayerPosition : IComparable<LayerPosition>
{
	internal static readonly DependencyProperty LayerPositionProperty = DependencyProperty.RegisterAttached("LayerPosition", typeof(LayerPosition), typeof(LayerPosition));

	internal readonly KnownLayer KnownLayer;

	internal readonly LayerInsertionPosition Position;

	public static void SetLayerPosition(UIElement layer, LayerPosition value)
	{
		layer.SetValue(LayerPositionProperty, value);
	}

	public static LayerPosition GetLayerPosition(UIElement layer)
	{
		return (LayerPosition)layer.GetValue(LayerPositionProperty);
	}

	public LayerPosition(KnownLayer knownLayer, LayerInsertionPosition position)
	{
		KnownLayer = knownLayer;
		Position = position;
	}

	public int CompareTo(LayerPosition other)
	{
		int num = KnownLayer.CompareTo(other.KnownLayer);
		if (num != 0)
		{
			return num;
		}
		return Position.CompareTo(other.Position);
	}
}
