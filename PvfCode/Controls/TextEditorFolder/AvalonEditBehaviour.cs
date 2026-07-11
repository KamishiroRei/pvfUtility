using System;
using System.Windows;
using DevExpress.Mvvm.UI.Interactivity;
using ICSharpCode.AvalonEdit;

namespace PvfCode.Controls.TextEditorFolder;

public sealed class AvalonEditBehaviour : Behavior<TextEditor>
{
	public static readonly DependencyProperty GiveMeTheTextProperty;

	public string GiveMeTheText
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(GiveMeTheTextProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(GiveMeTheTextProperty, (object)value);
		}
	}

	public AvalonEditBehaviour()
	{
	}

	protected override void OnAttached()
	{
		base.OnAttached();
		if (base.AssociatedObject != null)
		{
			if (string.IsNullOrEmpty(GiveMeTheText))
			{
				base.AssociatedObject.Document.Text = "";
			}
			else
			{
				base.AssociatedObject.Document.Text = GiveMeTheText;
			}
			base.AssociatedObject.TextChanged += HDVgHT38XN;
		}
	}

	protected override void OnDetaching()
	{
		base.OnDetaching();
		if (base.AssociatedObject != null)
		{
			base.AssociatedObject.TextChanged -= HDVgHT38XN;
		}
	}

	private void HDVgHT38XN(object? sender, EventArgs P_1)
	{
		if (sender is TextEditor { Document: not null } textEditor)
		{
			GiveMeTheText = textEditor.Document.Text;
		}
	}

	private static void uU0ghhUdAS(DependencyObject P_0, DependencyPropertyChangedEventArgs P_1)
	{
		try
		{
			AvalonEditBehaviour avalonEditBehaviour = P_0 as AvalonEditBehaviour;
			if (avalonEditBehaviour.AssociatedObject == null)
			{
				return;
			}
			TextEditor textEditor = avalonEditBehaviour.AssociatedObject;
			if (textEditor.Document != null)
			{
				int caretOffset = textEditor.CaretOffset;
				if (P_1.NewValue == null || string.IsNullOrEmpty(P_1.NewValue.ToString()))
				{
					textEditor.Document.Text = string.Empty;
					textEditor.CaretOffset = 0;
				}
				else
				{
					textEditor.Document.Text = P_1.NewValue.ToString();
					textEditor.CaretOffset = caretOffset;
				}
			}
		}
		catch (Exception)
		{
		}
	}

	static AvalonEditBehaviour()
	{
		GiveMeTheTextProperty = DependencyProperty.Register("GiveMeTheText", typeof(string), typeof(AvalonEditBehaviour), (PropertyMetadata)(object)new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(uU0ghhUdAS)));
	}
}
