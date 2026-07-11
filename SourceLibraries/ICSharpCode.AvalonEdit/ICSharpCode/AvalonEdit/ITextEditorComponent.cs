using System;
using System.ComponentModel;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit;

public interface ITextEditorComponent : IServiceProvider
{
	TextDocument Document { get; }

	TextEditorOptions Options { get; }

	event EventHandler DocumentChanged;

	event PropertyChangedEventHandler OptionChanged;
}
