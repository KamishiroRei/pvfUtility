using System;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Snippets;

[Serializable]
public class SnippetBoundElement : SnippetElement
{
	private SnippetReplaceableTextElement targetElement;

	public SnippetReplaceableTextElement TargetElement
	{
		get
		{
			return targetElement;
		}
		set
		{
			targetElement = value;
		}
	}

	public virtual string ConvertText(string input)
	{
		return input;
	}

	public override void Insert(InsertionContext context)
	{
		if (targetElement != null)
		{
			TextAnchor textAnchor = context.Document.CreateAnchor(context.InsertionPosition);
			textAnchor.MovementType = AnchorMovementType.BeforeInsertion;
			textAnchor.SurviveDeletion = true;
			string text = targetElement.Text;
			if (text != null)
			{
				context.InsertText(ConvertText(text));
			}
			TextAnchor textAnchor2 = context.Document.CreateAnchor(context.InsertionPosition);
			textAnchor2.MovementType = AnchorMovementType.BeforeInsertion;
			textAnchor2.SurviveDeletion = true;
			AnchorSegment segment = new AnchorSegment(textAnchor, textAnchor2);
			context.RegisterActiveElement(this, new BoundActiveElement(context, targetElement, this, segment));
		}
	}

	public override Inline ToTextRun()
	{
		if (targetElement != null)
		{
			string text = targetElement.Text;
			if (text != null)
			{
				return new Italic(new Run(ConvertText(text)));
			}
		}
		return base.ToTextRun();
	}
}
