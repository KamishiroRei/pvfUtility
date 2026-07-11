using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Editors;

namespace Controls.TextEditorFolder;

public class TextBoxPasteHelper : DependencyObject
{
	public static readonly DependencyProperty CanPasteNewLineProperty;

	public static bool GetCanPasteNewLine(DependencyObject d)
	{
		return (bool)d.GetValue(CanPasteNewLineProperty);
	}

	public static void SetCanPasteNewLine(DependencyObject d, bool value)
	{
		d.SetValue(CanPasteNewLineProperty, (object)value);
	}

	private static void hjwXNam1F(DependencyObject P_0, DependencyPropertyChangedEventArgs P_1)
	{
		if (P_0 is AutoSuggestEdit autoSuggestEdit)
		{
			CommandBinding commandBinding = new CommandBinding();
			commandBinding.Command = ApplicationCommands.Paste;
			commandBinding.Executed += UIBpa6dBI;
			autoSuggestEdit.CommandBindings.Add(commandBinding);
			KeyBinding keyBinding = new KeyBinding();
			keyBinding.Key = (Key)65;
			keyBinding.Modifiers = (ModifierKeys)2;
			keyBinding.Command = ApplicationCommands.Paste;
			autoSuggestEdit.InputBindings.Add(keyBinding);
		}
	}

	private static void UIBpa6dBI(object P_0, ExecutedRoutedEventArgs P_1)
	{
		string text = Clipboard.GetText();
		if (!string.IsNullOrEmpty(text) && P_0 is TextBox textBox)
		{
			if (textBox.SelectedText == textBox.Text)
			{
				textBox.Text = text;
				return;
			}
			if (!string.IsNullOrEmpty(textBox.SelectedText))
			{
				textBox.SelectedText = text;
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(textBox.Text);
			stringBuilder.Insert(textBox.CaretIndex, text);
			textBox.Text = stringBuilder.ToString();
		}
	}

	public TextBoxPasteHelper()
	{
	}

	static TextBoxPasteHelper()
	{
		CanPasteNewLineProperty = DependencyProperty.RegisterAttached("CanPasteNewLine", typeof(bool), typeof(TextBoxPasteHelper), new PropertyMetadata((object)false, new PropertyChangedCallback(hjwXNam1F)));
	}
}
