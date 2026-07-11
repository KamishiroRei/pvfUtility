using System.Windows;

namespace TextEditLib.Themes;

public static class ResourceKeys
{
	public static readonly ComponentResourceKey ControlAccentColorKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentColorKey");

	public static readonly ComponentResourceKey ControlAccentBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentBrushKey");

	public static readonly ComponentResourceKey EditorBackground = new ComponentResourceKey(typeof(ResourceKeys), "EditorBackground");

	public static readonly ComponentResourceKey EditorForeground = new ComponentResourceKey(typeof(ResourceKeys), "EditorForeground");

	public static readonly ComponentResourceKey EditorLineNumbersForeground = new ComponentResourceKey(typeof(ResourceKeys), "EditorLineNumbersForeground");

	public static readonly ComponentResourceKey EditorSelectionBrush = new ComponentResourceKey(typeof(ResourceKeys), "EditorSelectionBrush");

	public static readonly ComponentResourceKey EditorSelectionBorder = new ComponentResourceKey(typeof(ResourceKeys), "EditorSelectionBorder");

	public static readonly ComponentResourceKey EditorNonPrintableCharacterBrush = new ComponentResourceKey(typeof(ResourceKeys), "EditorNonPrintableCharacterBrush");

	public static readonly ComponentResourceKey EditorLinkTextForegroundBrush = new ComponentResourceKey(typeof(ResourceKeys), "EditorLinkTextForegroundBrush");

	public static readonly ComponentResourceKey EditorLinkTextBackgroundBrush = new ComponentResourceKey(typeof(ResourceKeys), "EditorLinkTextBackgroundBrush");

	public static readonly ComponentResourceKey EditorCurrentLineBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "EditorCurrentLineBackgroundBrushKey");

	public static readonly ComponentResourceKey EditorCurrentLineBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "EditorCurrentLineBorderBrushKey");

	public static readonly ComponentResourceKey EditorCurrentLineBorderThicknessKey = new ComponentResourceKey(typeof(ResourceKeys), "EditorCurrentLineBorderThicknessKey");
}
