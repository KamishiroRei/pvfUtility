using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.ViewModels;
using PvfCode.ViewModels.DocumentFolder.CodeCompletion;
using PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip;
using PvfCode.ViewModels.DocumentFolder.Foldings;
using PvfCode.ViewModels.DocumentFolder.VisualLineElementGenerators;
using TextEditLib;

namespace PvfCode.Controls.TextEditorFolder;

public class TextEditorBase : TextEdit, IComponentConnector
{
	private delegate Task VfCpNbOF2aZ7tZoelHH();

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass13_0
	{
		public int offset;

		public HighlightingType BscOW8Yxue;

		public _003C_003Ec__DisplayClass13_0()
		{
		}

		internal bool ganOrOUAY8(HighlightedSection s)
		{
			if (s.Offset <= offset && s.Offset + s.Length >= offset)
			{
				return s.Color.Name == BscOW8Yxue.ToString();
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass14_0
	{
		public int offset;

		public HighlightingType feQO2Al1lh;

		public _003C_003Ec__DisplayClass14_0()
		{
		}

		internal bool OkLOmCY6ht(HighlightedSection s)
		{
			if (s.Offset <= offset && s.Offset + s.Length >= offset)
			{
				return s.Color.Name == feQO2Al1lh.ToString();
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass15_0
	{
		public int offset;

		public HighlightingType Nj1O563b46;

		public _003C_003Ec__DisplayClass15_0()
		{
		}

		internal bool hkpOf7HoBT(HighlightedSection s)
		{
			if (s.Offset <= offset && s.Offset + s.Length >= offset)
			{
				return s.Color.Name == Nj1O563b46.ToString();
			}
			return false;
		}
	}

	[CompilerGenerated]
	private bool XDqgkYIa5e;

	[CompilerGenerated]
	private SectionCommentElementGenerator A7hg0O73Xy;

	private ScriptCodeCompletionBase codeCompletion;

	private new bool IsLoaded;

	[CompilerGenerated]
	private bool NFkgXji2sH;

	private EditorHoverTooltipManager? MxqgpWMI0q;

	private PeriodicTimer xIpgUUn2C1;

	private object omTgcZvJwL;

	public FoldingStrategyBase FoldingStrategyBaseHelper;

	[CompilerGenerated]
	private bool SHrg81k72M;

	public static readonly DependencyProperty TextIsChangedProperty;

	public static readonly DependencyProperty AllowCompletionProperty;

	public static readonly DependencyProperty AllowFoldingProperty;

	public static readonly DependencyProperty FocusCaretProperty;

	private bool kc2gMsb3hf;

	public static readonly DependencyProperty VerticalOffsetExProperty;

	public static readonly DependencyProperty MaxNumExProperty;

	public static readonly DependencyProperty ViewportSizeExProperty;

	public static readonly DependencyProperty ScrollViewerProperty;

	public static readonly DependencyProperty IsActiveProperty;

	public static readonly DependencyProperty FileTypeProperty;

	public static readonly DependencyProperty PvfFileProperty;

	private bool WH7gVLLxBG;

	public bool CodeCompletionIsOpen
	{
		[CompilerGenerated]
		get
		{
			return XDqgkYIa5e;
		}
		[CompilerGenerated]
		set
		{
			XDqgkYIa5e = value;
		}
	}

	public bool AllowUpdateFolding
	{
		[CompilerGenerated]
		get
		{
			return SHrg81k72M;
		}
		[CompilerGenerated]
		set
		{
			SHrg81k72M = value;
		}
	}

	public DocumentHighlighter GetDocumentHighlighter
	{
		get
		{
			try
			{
				if (base.SyntaxHighlighting == null)
				{
					return null;
				}
				return new DocumentHighlighter(base.Document, base.SyntaxHighlighting);
			}
			catch (Exception e)
			{
				AppCore.Logger.ErrorUploadDialog(e, "TextEditor.GetDocumentHighlighter");
				return null;
			}
		}
	}

	public bool TextIsChanged
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(TextIsChangedProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TextIsChangedProperty, (object)value);
		}
	}

	public bool AllowCompletion
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(AllowCompletionProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(AllowCompletionProperty, (object)value);
		}
	}

	public bool AllowFolding
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(AllowFoldingProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(AllowFoldingProperty, (object)value);
		}
	}

	public int FocusCaretLine
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(FocusCaretProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FocusCaretProperty, (object)value);
		}
	}

	public double VerticalOffsetEx
	{
		get
		{
			return base.VerticalOffset;
		}
		set
		{
			((DependencyObject)this).SetValue(VerticalOffsetExProperty, (object)value);
		}
	}

	public double MaxNumEx
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(MaxNumExProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MaxNumExProperty, (object)value);
		}
	}

	public double ViewportSizeEx
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ViewportSizeExProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ViewportSizeExProperty, (object)value);
		}
	}

	public ScrollViewer ScrollViewer
	{
		get
		{
			return (ScrollViewer)((DependencyObject)this).GetValue(ScrollViewerProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ScrollViewerProperty, (object)value);
		}
	}

	public bool IsActive
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsActiveProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsActiveProperty, (object)value);
		}
	}

	public PvfFileType? FileType
	{
		get
		{
			return (PvfFileType?)((DependencyObject)this).GetValue(FileTypeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FileTypeProperty, (object)value);
		}
	}

	public PvfFile PvfFile
	{
		get
		{
			return (PvfFile)((DependencyObject)this).GetValue(PvfFileProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PvfFileProperty, (object)value);
		}
	}

	[SpecialName]
	[CompilerGenerated]
	private SectionCommentElementGenerator PTjgbEXWL4()
	{
		return A7hg0O73Xy;
	}

	[SpecialName]
	[CompilerGenerated]
	private void Xi9gIQurEA(SectionCommentElementGenerator P_0)
	{
		A7hg0O73Xy = P_0;
	}

	public TextEditorBase()
	{
		omTgcZvJwL = new object();
		SHrg81k72M = true;
		InitializeComponent();
		base.Encoding = Encoding.GetEncoding((int)AppSetting.Instance.PvfConfig.DefaultEncoding);
		base.TextArea.TextView.ElementGenerators.Add(new TruncateLongLines());
		base.Loaded += CK0gQEN48X;
		base.Unloaded += TextEditorBase_Unloaded;
	}

	public bool OffSetIsHiglig(HighlightingType type, int offset)
	{
		_003C_003Ec__DisplayClass13_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass13_0();
		CS_0024_003C_003E8__locals6.offset = offset;
		CS_0024_003C_003E8__locals6.BscOW8Yxue = type;
		try
		{
			DocumentLine lineByOffset = base.Document.GetLineByOffset(CS_0024_003C_003E8__locals6.offset);
			return GetDocumentHighlighter.HighlightLine(lineByOffset.LineNumber).Sections.Any((HighlightedSection s) => s.Offset <= CS_0024_003C_003E8__locals6.offset && s.Offset + s.Length >= CS_0024_003C_003E8__locals6.offset && s.Color.Name == CS_0024_003C_003E8__locals6.BscOW8Yxue.ToString());
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorBase.OffSetIsHiglig");
			return false;
		}
	}

	public bool OffSetIsHiglig(HighlightingType type, IEnumerable<HighlightedSection> sections, int offset)
	{
		_003C_003Ec__DisplayClass14_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass14_0();
		CS_0024_003C_003E8__locals5.offset = offset;
		CS_0024_003C_003E8__locals5.feQO2Al1lh = type;
		try
		{
			return sections?.Any((HighlightedSection s) => s.Offset <= CS_0024_003C_003E8__locals5.offset && s.Offset + s.Length >= CS_0024_003C_003E8__locals5.offset && s.Color.Name == CS_0024_003C_003E8__locals5.feQO2Al1lh.ToString()) ?? false;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorBase.OffSetIsHiglig");
			return false;
		}
	}

	public HighlightedSection GetHigSection(HighlightingType type, int offset)
	{
		_003C_003Ec__DisplayClass15_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass15_0();
		CS_0024_003C_003E8__locals6.offset = offset;
		CS_0024_003C_003E8__locals6.Nj1O563b46 = type;
		try
		{
			DocumentLine lineByOffset = base.Document.GetLineByOffset(CS_0024_003C_003E8__locals6.offset);
			return GetDocumentHighlighter.HighlightLine(lineByOffset.LineNumber).Sections.FirstOrDefault((HighlightedSection s) => s.Offset <= CS_0024_003C_003E8__locals6.offset && s.Offset + s.Length >= CS_0024_003C_003E8__locals6.offset && s.Color.Name == CS_0024_003C_003E8__locals6.Nj1O563b46.ToString());
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorBase.GetHigSection");
			return null;
		}
	}

	public HighlightedSection GetHigSectionAndIndex(HighlightingType type, DocumentLine line, int offset, out int index)
	{
		try
		{
			HighlightedLine highlightedLine = GetDocumentHighlighter?.HighlightLine(line.LineNumber);
			if (highlightedLine == null)
			{
				index = -1;
				return null;
			}
			int num = 0;
			HighlightedSection result = null;
			foreach (HighlightedSection section in highlightedLine.Sections)
			{
				if (section.Offset <= offset && section.Offset + section.Length >= offset && section.Color.Name == type.ToString())
				{
					result = section;
					break;
				}
				num++;
			}
			index = num;
			return result;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorBase.GetHigSectionAndIndex");
			index = -1;
			return null;
		}
	}

	public IEnumerable<HighlightedSection> GetHigSections(int offset)
	{
		try
		{
			if (offset < 0 || offset > base.Document.TextLength)
			{
				return new List<HighlightedSection>();
			}
			DocumentLine lineByOffset = base.Document.GetLineByOffset(offset);
			return GetDocumentHighlighter.HighlightLine(lineByOffset.LineNumber).Sections;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorBase.GetHigSections");
			return null;
		}
	}

	public IEnumerable<HighlightedSection> GetHigSectionsByLineNumber(int lineNumber)
	{
		try
		{
			return GetDocumentHighlighter.HighlightLine(lineNumber).Sections;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorBase.GetHigSectionsByLineNumber");
			return null;
		}
	}

	public int? GetMouseOffset()
	{
		try
		{
			TextViewPosition? textViewPosition = oufgxu6L5T();
			if (!textViewPosition.HasValue)
			{
				return null;
			}
			return base.Document.GetOffset(textViewPosition.Value.Location);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorBase.GetMouseOffset");
			return null;
		}
	}

	internal TextViewPosition? oufgxu6L5T()
	{
		try
		{
			TextViewPosition? position = base.TextArea.TextView.GetPosition(Mouse.GetPosition(base.TextArea.TextView) + base.TextArea.TextView.ScrollOffset);
			if (!position.HasValue)
			{
				return null;
			}
			int num = base.Document.GetLineByNumber(position.Value.Line).Length + 1;
			if (position.Value.Column == num)
			{
				return null;
			}
			return position;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorBase.GetMouseOffset");
			return null;
		}
	}

	[SpecialName]
	[CompilerGenerated]
	private bool fKygP4hgAj()
	{
		return NFkgXji2sH;
	}

	[SpecialName]
	[CompilerGenerated]
	private void TaZgZRheUT(bool P_0)
	{
		NFkgXji2sH = P_0;
	}

	private async void CK0gQEN48X(object P_0, RoutedEventArgs P_1)
	{
		try
		{
			if (!IsLoaded)
			{
				IsLoaded = true;
				if (base.Document != null)
				{
					base.Document.TextChanged += SmGgLfSInl;
				}
				if (base.EditorType == TextEditorType.Diff)
				{
					base.TextArea.Caret.PositionChanged += X93gnL7Pqh;
				}
				if (PvfFile != null && FileType == PvfFileType.lst && !AppSetting.Instance.EditConfig.NotUseFileListTooltip.Contains(PvfFile.FileName))
				{
					Xi9gIQurEA(new SectionCommentElementGenerator(this, PvfFile));
					base.TextArea.TextView.ElementGenerators.Add(PTjgbEXWL4());
				}
				if (AllowCompletion && FileType.HasValue)
				{
					switch (FileType.Value)
					{
					case PvfFileType.nut:
						codeCompletion = new NutCodeCompletion(this, FileType);
						break;
					default:
						codeCompletion = new ScriptCodeCompletion(this, FileType);
						break;
					case PvfFileType.lst:
					case PvfFileType.kor:
					case PvfFileType.str:
					case PvfFileType.bin:
						break;
					}
				}
				switch (FileType)
				{
				default:
					if ((PvfFile != null && PvfFile.FileName != "stringtable.bin") || (PvfFile != null && FileType == PvfFileType.lst && AppSetting.Instance.PvfConfig.LstFileUseScriptFile.Contains(PvfFile.FileName)))
					{
						MxqgpWMI0q = new EditorHoverTooltipManager(this, PvfFile);
					}
					break;
				case PvfFileType.txt:
				case PvfFileType.str:
				case PvfFileType.bin:
					break;
				}
				if (AllowFolding)
				{
					kFLg6a8Ffy();
					UpdateFolding();
				}
			}
			if (!fKygP4hgAj())
			{
				ScrollViewer getScrollViewer = base.GetScrollViewer;
				if (getScrollViewer != null)
				{
					kc2gMsb3hf = true;
					getScrollViewer.ScrollChanged += fIcgdKj2iI;
					TaZgZRheUT(true);
				}
			}
			await o5CgoPaD3B();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorBase.TextEditorBase_Loaded");
		}
	}

	public TextSegment? GetVisibleOffset()
	{
		try
		{
			TextView textView = base.TextArea.TextView;
			TextViewPosition? position = textView.GetPosition(new Point(0.0, 0.0) + textView.ScrollOffset);
			TextViewPosition? position2 = textView.GetPosition(new Point(textView.ActualWidth, textView.ActualHeight) + textView.ScrollOffset);
			int num = (position.HasValue ? base.Document.GetOffset(position.Value.Location) : base.Document.TextLength);
			int num2 = (position2.HasValue ? base.Document.GetOffset(position2.Value.Location) : base.Document.TextLength);
			if (num2 <= num)
			{
				return null;
			}
			return new TextSegment
			{
				StartOffset = num,
				EndOffset = num2,
				Length = num2 - num
			};
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorBase.GetVisibleOffset");
			return null;
		}
	}

	public void TextEditorBase_Unloaded(object sender, RoutedEventArgs e)
	{
		KVqgwmoMK4();
	}

	private Point Q8hgg5wbOq(MouseEventArgs P_0)
	{
		Point position = P_0.GetPosition(this);
		TextViewPosition? positionFromPoint = GetPositionFromPoint(position);
		Point point;
		if (positionFromPoint.HasValue)
		{
			TextView textView = base.TextArea.TextView;
			point = textView.PointToScreen(textView.GetVisualPosition(positionFromPoint.Value, VisualYPosition.LineBottom) - textView.ScrollOffset);
			point.X = point.X - 4.0;
		}
		else
		{
			point = PointToScreen(position + new Vector(-4.0, 6.0));
		}
		return point.TransformFromDevice(this);
	}

	private void kFLg6a8Ffy()
	{
		try
		{
			if (FileType.HasValue)
			{
				switch (FileType.Value)
				{
				case PvfFileType.txt:
				case PvfFileType.lst:
				case PvfFileType.kor:
				case PvfFileType.str:
				case PvfFileType.bin:
					return;
				}
				FoldingStrategyBaseHelper = new FoldingStrategyBase(this, FileType);
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEdtiroBase.InitFoldings");
		}
	}

	private async Task lr6g1uqW5C()
	{
		int errId = 0;
		try
		{
			if (xIpgUUn2C1 == null)
			{
				return;
			}
			try
			{
				while (true)
				{
					bool flag = xIpgUUn2C1 != null;
					if (flag)
					{
						flag = await xIpgUUn2C1.WaitForNextTickAsync();
					}
					if (flag)
					{
						errId = 0;
						if (IsActive && AllowUpdateFolding)
						{
							errId = 1;
							await UpdateFolding();
							errId = 2;
						}
						continue;
					}
					break;
				}
			}
			catch (Exception)
			{
			}
		}
		catch (Exception e)
		{
			LoggerViewModel logger = AppCore.Logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
			defaultInterpolatedStringHandler.AppendLiteral("折叠定时任务 ErrId:");
			defaultInterpolatedStringHandler.AppendFormatted(errId);
			logger.ErrorUploadDialog(e, defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}

	public async Task UpdateFolding()
	{
		try
		{
			if (AllowFolding)
			{
				AllowUpdateFolding = false;
				if (FoldingStrategyBaseHelper == null)
				{
					AllowUpdateFolding = false;
					return;
				}
				await (FoldingStrategyBaseHelper?.UpdateFoldings());
				AllowUpdateFolding = false;
			}
		}
		catch (Exception e)
		{
			AllowUpdateFolding = false;
			AppCore.Logger.ErrorUploadDialog(e, "更新折叠策略");
		}
	}

	private void KVqgwmoMK4()
	{
		lock (omTgcZvJwL)
		{
			if (xIpgUUn2C1 != null)
			{
				try
				{
					xIpgUUn2C1?.Dispose();
					xIpgUUn2C1 = null;
					return;
				}
				catch (Exception e)
				{
					AppCore.Logger.ErrorUploadDialog(e, "TextEditor.UnloadedFoldingTimer");
					return;
				}
			}
		}
	}

	private async Task o5CgoPaD3B()
	{
		if (xIpgUUn2C1 == null)
		{
			xIpgUUn2C1 = new PeriodicTimer(TimeSpan.FromSeconds(3.0));
			await lr6g1uqW5C();
		}
	}

	private static void lgBgs4DB2P(DependencyObject P_0, DependencyPropertyChangedEventArgs P_1)
	{
		TextEditorBase textEditorBase = (TextEditorBase)(object)P_0;
		if (textEditorBase != null && (bool)P_1.NewValue)
		{
			textEditorBase.AllowUpdateFolding = true;
		}
	}

	private void SmGgLfSInl(object? sender, EventArgs P_1)
	{
		TextIsChanged = true;
		AllowUpdateFolding = true;
	}

	private void X93gnL7Pqh(object? sender, EventArgs P_1)
	{
		if (base.TextArea != null)
		{
			FocusCaretLine = base.TextArea.Caret.Line;
		}
	}

	private static void Ux5gqfBVZV(DependencyObject P_0, DependencyPropertyChangedEventArgs P_1)
	{
		TextEditorBase textEditorBase = (TextEditorBase)(object)P_0;
		if (textEditorBase != null && P_1.NewValue != null)
		{
			int line = (int)P_1.NewValue;
			textEditorBase.TextArea.Caret.Line = line;
		}
	}

	private void fIcgdKj2iI(object P_0, ScrollChangedEventArgs P_1)
	{
		try
		{
			ScrollViewer getScrollViewer = base.GetScrollViewer;
			if (base.EditorType == TextEditorType.Diff || base.EditorType == TextEditorType.Editor)
			{
				MaxNumEx = getScrollViewer.ScrollableHeight;
				ViewportSizeEx = getScrollViewer.ViewportHeight;
				VerticalOffsetEx = base.VerticalOffset;
			}
			_ = kc2gMsb3hf;
			ScrollViewer = getScrollViewer;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "滚动条改变事件：Bar_ScrollChanged");
		}
	}

	private static void qWvgePBRGk(DependencyObject P_0, DependencyPropertyChangedEventArgs P_1)
	{
		TextEditorBase textEditorBase = (TextEditorBase)(object)P_0;
		if (textEditorBase != null && P_1.NewValue != null)
		{
			double offset = (double)P_1.NewValue;
			textEditorBase.ScrollToVerticalOffset(offset);
		}
	}

	private static void FYwgtShVSk(DependencyObject P_0, DependencyPropertyChangedEventArgs P_1)
	{
		TextEditorBase textEditorBase = (TextEditorBase)(object)P_0;
		if (textEditorBase != null && !Convert.ToBoolean(P_1.NewValue) && textEditorBase.codeCompletion != null)
		{
			textEditorBase.codeCompletion.CloseWindow();
		}
	}

	public void Dispose2()
	{
		try
		{
			base.GetScrollViewer.ScrollChanged -= fIcgdKj2iI;
			base.TextArea.Caret.PositionChanged += X93gnL7Pqh;
			base.Loaded -= CK0gQEN48X;
			base.Unloaded -= TextEditorBase_Unloaded;
			base.Document.TextChanged -= SmGgLfSInl;
			KVqgwmoMK4();
			FoldingStrategyBaseHelper?.Dispose();
			codeCompletion?.Dispose();
			MxqgpWMI0q?.Dispose();
			base.TextArea.TextView.ElementGenerators?.Clear();
			base.TextArea.TextView.BackgroundRenderers.Clear();
			Dispose();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditor.Dispose2");
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!WH7gVLLxBG)
		{
			WH7gVLLxBG = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/texteditorfolder/texteditorbase.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		WH7gVLLxBG = true;
	}

	static TextEditorBase()
	{
		TextIsChangedProperty = DependencyProperty.Register("TextIsChanged", typeof(bool), typeof(TextEditorBase), new PropertyMetadata((object)false, new PropertyChangedCallback(lgBgs4DB2P)));
		AllowCompletionProperty = DependencyProperty.Register("AllowCompletion", typeof(bool), typeof(TextEditorBase), new PropertyMetadata((object)false));
		AllowFoldingProperty = DependencyProperty.Register("AllowFolding", typeof(bool), typeof(TextEditorBase), new PropertyMetadata((object)true));
		FocusCaretProperty = DependencyProperty.Register("FocusCaretLine", typeof(int), typeof(TextEditorBase), new PropertyMetadata((object)0, new PropertyChangedCallback(Ux5gqfBVZV)));
		VerticalOffsetExProperty = DependencyProperty.Register("VerticalOffsetEx", typeof(double), typeof(TextEditorBase), new PropertyMetadata((object)0.0, new PropertyChangedCallback(qWvgePBRGk)));
		MaxNumExProperty = DependencyProperty.Register("MaxNumEx", typeof(double), typeof(TextEditorBase), new PropertyMetadata((object)0.0));
		ViewportSizeExProperty = DependencyProperty.Register("ViewportSizeEx", typeof(double), typeof(TextEditorBase), new PropertyMetadata((object)0.0));
		ScrollViewerProperty = DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(TextEditorBase), new PropertyMetadata((PropertyChangedCallback)null));
		IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(TextEditorBase), new PropertyMetadata((object)false, new PropertyChangedCallback(FYwgtShVSk)));
		FileTypeProperty = DependencyProperty.Register("FileType", typeof(PvfFileType?), typeof(TextEditorBase), new PropertyMetadata((PropertyChangedCallback)null));
		PvfFileProperty = DependencyProperty.Register("PvfFile", typeof(PvfFile), typeof(TextEditorBase), new PropertyMetadata((PropertyChangedCallback)null));
	}
}
