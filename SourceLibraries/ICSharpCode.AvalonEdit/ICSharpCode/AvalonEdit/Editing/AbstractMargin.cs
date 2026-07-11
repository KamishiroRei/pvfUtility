using System;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Editing;

public abstract class AbstractMargin : FrameworkElement, ITextViewConnect
{
	public static readonly DependencyProperty TextViewProperty = DependencyProperty.Register("TextView", typeof(TextView), typeof(AbstractMargin), new FrameworkPropertyMetadata(OnTextViewChanged));

	private bool wasAutoAddedToTextView;

	private TextDocument document;

	public TextView TextView
	{
		get
		{
			return (TextView)GetValue(TextViewProperty);
		}
		set
		{
			SetValue(TextViewProperty, value);
		}
	}

	public TextDocument Document => document;

	private static void OnTextViewChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
	{
		AbstractMargin obj = (AbstractMargin)dp;
		obj.wasAutoAddedToTextView = false;
		obj.OnTextViewChanged((TextView)e.OldValue, (TextView)e.NewValue);
	}

	void ITextViewConnect.AddToTextView(TextView textView)
	{
		if (TextView == null)
		{
			TextView = textView;
			wasAutoAddedToTextView = true;
		}
		else if (TextView != textView)
		{
			throw new InvalidOperationException("This margin belongs to a different TextView.");
		}
	}

	void ITextViewConnect.RemoveFromTextView(TextView textView)
	{
		if (wasAutoAddedToTextView && TextView == textView)
		{
			TextView = null;
		}
	}

	protected virtual void OnTextViewChanged(TextView oldTextView, TextView newTextView)
	{
		if (oldTextView != null)
		{
			oldTextView.DocumentChanged -= TextViewDocumentChanged;
		}
		if (newTextView != null)
		{
			newTextView.DocumentChanged += TextViewDocumentChanged;
		}
		TextViewDocumentChanged(null, null);
	}

	private void TextViewDocumentChanged(object sender, EventArgs e)
	{
		OnDocumentChanged(document, (TextView != null) ? TextView.Document : null);
	}

	protected virtual void OnDocumentChanged(TextDocument oldDocument, TextDocument newDocument)
	{
		document = newDocument;
	}
}
