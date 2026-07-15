using System;
using HL.Manager;
using ICSharpCode.AvalonEdit.Highlighting;

namespace PvfCode.Controls;

internal static class PvfCodeHighlightingSource
{
	internal static event Action Changed;

	internal static IHighlightingDefinition Script => ThemedHighlightingManager.Instance.GetDefinition("Script");

	internal static void NotifyChanged()
	{
		Changed?.Invoke();
	}
}
