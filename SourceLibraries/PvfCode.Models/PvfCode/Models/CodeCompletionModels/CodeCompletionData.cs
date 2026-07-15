using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Newtonsoft.Json;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Dot.Desktop.interfaces;
using Utools;

namespace PvfCode.Models.CodeCompletionModels;

[JsonObject(MemberSerialization.OptOut)]
public class CodeCompletionData : ModelBase, ICodeCompletionData
{
	private string text;

	public string _CompleteText;

	private CodeCompletScriptType codeCompletScriptType;

	private bool isShare;

	private string nickNames;

	[JsonIgnore]
	public ImageSource Image
	{
		get
		{
			if (HighlightingType == HighlightingType.String)
			{
				return AppSetting.Instance.GetRes()?.MethodSealed;
			}
			return AppSetting.Instance.GetRes()?.HigSectionIcon;
		}
		set
		{
		}
	}

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
			DoNotify(nameof(Text));
		}
	}

	public string Description { get; set; }

	public double Priority { get; set; }

	public bool HaveEndSection { get; set; }

	public HighlightingType HighlightingType { get; set; }

	public string CompleteText
	{
		get
		{
			if (string.IsNullOrEmpty(_CompleteText))
			{
				if (HaveEndSection)
				{
					return Text + "\r\n\r\n" + GetEndSectionText();
				}
				return Text + "\r\n";
			}
			return _CompleteText;
		}
		set
		{
			_CompleteText = value;
			DoNotify(nameof(CompleteText));
		}
	}

	public CodeCompletScriptType CodeCompletScriptType
	{
		get
		{
			return codeCompletScriptType;
		}
		set
		{
			codeCompletScriptType = value;
			DoNotify(nameof(CodeCompletScriptType));
		}
	}

	public bool IsShare
	{
		get
		{
			return isShare;
		}
		set
		{
			isShare = value;
			DoNotify(nameof(IsShare));
		}
	}

	public PvfFileType PvfFileType { get; set; }

	public string NickNames
	{
		get
		{
			if (nickNames == null)
			{
				nickNames = string.Empty;
			}
			return nickNames;
		}
		set
		{
			nickNames = value;
			DoNotify(nameof(NickNames));
		}
	}

	private string GetEndSectionText()
	{
		string text = Text;
		if (!string.IsNullOrEmpty(Text) && Text.Length > 1)
		{
			text = text.Insert(1, "/");
		}
		return text;
	}

	public void Complete(TextEditor editor, TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
	{
		textArea.Document.Replace(completionSegment, CompleteText);
		string text = textArea.Document.GetText(completionSegment);
		if (!CompleteText.Contains("$光标"))
		{
			return;
		}
		string text2 = StrHelper.Between(CompleteText, "$光标(", ")");
		if (!string.IsNullOrEmpty(text2))
		{
			int num = text.IndexOf("$光标(");
			if (num != -1)
			{
				textArea.Document.Replace(completionSegment.Offset + num, ("$光标(" + text2 + ")").Length, text2);
				textArea.Caret.Offset = completionSegment.Offset + num + text2.Length;
				editor.Select(completionSegment.Offset + num, text2.Length);
			}
		}
		else
		{
			int num2 = text.IndexOf("$光标");
			if (num2 != -1)
			{
				textArea.Document.Replace(completionSegment.Offset + num2, "$光标".Length, text2);
				textArea.Caret.Offset = completionSegment.Offset + num2;
			}
		}
	}

	public static CodeCompletionData Create(PvfFileType pvfFileType)
	{
		return new CodeCompletionData
		{
			PvfFileType = pvfFileType,
			CodeCompletScriptType = ((pvfFileType == PvfFileType.nut) ? CodeCompletScriptType.Nut : CodeCompletScriptType.Script),
			IsShare = true,
			CompleteText = AppSetting.Instance.GetIlogger().GetStr("mess_InsertContentWhenPressTabOrEnter")
		};
	}
}
