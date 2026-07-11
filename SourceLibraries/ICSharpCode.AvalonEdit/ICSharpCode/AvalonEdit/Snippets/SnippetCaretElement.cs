using System;
using System.Runtime.Serialization;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Snippets;

[Serializable]
public class SnippetCaretElement : SnippetElement
{
	[OptionalField]
	private bool setCaretOnlyIfTextIsSelected;

	public SnippetCaretElement()
	{
	}

	public SnippetCaretElement(bool setCaretOnlyIfTextIsSelected)
	{
		this.setCaretOnlyIfTextIsSelected = setCaretOnlyIfTextIsSelected;
	}

	public override void Insert(InsertionContext context)
	{
		if (!setCaretOnlyIfTextIsSelected || !string.IsNullOrEmpty(context.SelectedText))
		{
			SetCaret(context);
		}
	}

	internal static void SetCaret(InsertionContext context)
	{
		TextAnchor pos = context.Document.CreateAnchor(context.InsertionPosition);
		pos.MovementType = AnchorMovementType.BeforeInsertion;
		pos.SurviveDeletion = true;
		context.Deactivated += delegate(object? sender, SnippetEventArgs e)
		{
			if (e.Reason == DeactivateReason.ReturnPressed || e.Reason == DeactivateReason.NoActiveElements)
			{
				context.TextArea.Caret.Offset = pos.Offset;
			}
		};
	}
}
