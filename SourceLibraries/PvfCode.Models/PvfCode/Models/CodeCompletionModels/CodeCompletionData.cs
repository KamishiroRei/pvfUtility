using System;
using System.Runtime.CompilerServices;
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
	private string hSeZaKOp38;

	[CompilerGenerated]
	private string QIQZIXKkpU;

	[CompilerGenerated]
	private double ybUZUeUtnu;

	[CompilerGenerated]
	private bool EY5Zl1JrmD;

	[CompilerGenerated]
	private HighlightingType bJOZfcumsE;

	public string _CompleteText;

	private CodeCompletScriptType ENVZhDntbe;

	private bool L87ZT1VM75;

	[CompilerGenerated]
	private PvfFileType ucHZ0yrMWs;

	private string WnBZsqUuuf;

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
			return hSeZaKOp38;
		}
		set
		{
			hSeZaKOp38 = value;
			DoNotify("Text");
		}
	}

	public string Description
	{
		[CompilerGenerated]
		get
		{
			return QIQZIXKkpU;
		}
		[CompilerGenerated]
		set
		{
			QIQZIXKkpU = value;
		}
	}

	public double Priority
	{
		[CompilerGenerated]
		get
		{
			return ybUZUeUtnu;
		}
		[CompilerGenerated]
		set
		{
			ybUZUeUtnu = value;
		}
	}

	public bool HaveEndSection
	{
		[CompilerGenerated]
		get
		{
			return EY5Zl1JrmD;
		}
		[CompilerGenerated]
		set
		{
			EY5Zl1JrmD = value;
		}
	}

	public HighlightingType HighlightingType
	{
		[CompilerGenerated]
		get
		{
			return bJOZfcumsE;
		}
		[CompilerGenerated]
		set
		{
			bJOZfcumsE = value;
		}
	}

	public string CompleteText
	{
		get
		{
			if (string.IsNullOrEmpty(_CompleteText))
			{
				if (HaveEndSection)
				{
					return Text + "\r\n\r\n" + wDvZMlKGV3();
				}
				return Text + "\r\n";
			}
			return _CompleteText;
		}
		set
		{
			_CompleteText = value;
			DoNotify("CompleteText");
		}
	}

	public CodeCompletScriptType CodeCompletScriptType
	{
		get
		{
			return ENVZhDntbe;
		}
		set
		{
			ENVZhDntbe = value;
			DoNotify("CodeCompletScriptType");
		}
	}

	public bool IsShare
	{
		get
		{
			return L87ZT1VM75;
		}
		set
		{
			L87ZT1VM75 = value;
			DoNotify("IsShare");
		}
	}

	public PvfFileType PvfFileType
	{
		[CompilerGenerated]
		get
		{
			return ucHZ0yrMWs;
		}
		[CompilerGenerated]
		set
		{
			ucHZ0yrMWs = value;
		}
	}

	public string NickNames
	{
		get
		{
			if (WnBZsqUuuf == null)
			{
				WnBZsqUuuf = string.Empty;
			}
			return WnBZsqUuuf;
		}
		set
		{
			WnBZsqUuuf = value;
			DoNotify("NickNames");
		}
	}

	private string wDvZMlKGV3()
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

	public CodeCompletionData()
	{
	}
}
