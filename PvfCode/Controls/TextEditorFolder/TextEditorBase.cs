using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
	private ScriptCodeCompletionBase codeCompletion;

	private new bool IsLoaded;

	private EditorHoverTooltipManager? hoverTooltipManager;

	private PeriodicTimer foldingTimer;

	private object foldingTimerLock;

	public FoldingStrategyBase FoldingStrategyBaseHelper;

	public static readonly DependencyProperty TextIsChangedProperty;

	public static readonly DependencyProperty AllowCompletionProperty;

	public static readonly DependencyProperty AllowFoldingProperty;

	public static readonly DependencyProperty FocusCaretProperty;

	private bool scrollViewerInitialized;

	public static readonly DependencyProperty VerticalOffsetExProperty;

	public static readonly DependencyProperty MaxNumExProperty;

	public static readonly DependencyProperty ViewportSizeExProperty;

	public static readonly DependencyProperty ScrollViewerProperty;

	public static readonly DependencyProperty IsActiveProperty;

	public static readonly DependencyProperty FileTypeProperty;

	public static readonly DependencyProperty PvfFileProperty;

	private bool _contentLoaded;

	private SectionCommentElementGenerator SectionCommentElementGenerator { get; set; }

	private bool ScrollChangedSubscribed { get; set; }

	public bool CodeCompletionIsOpen { get; set; }

	public bool AllowUpdateFolding { get; set; }

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

	public TextEditorBase()
	{
		foldingTimerLock = new object();
		AllowUpdateFolding = true;
		InitializeComponent();
		base.Encoding = Encoding.GetEncoding((int)AppSetting.Instance.PvfConfig.DefaultEncoding);
		base.TextArea.TextView.ElementGenerators.Add(new TruncateLongLines());
		base.Loaded += OnLoaded;
		base.Unloaded += TextEditorBase_Unloaded;
	}

	public bool OffSetIsHiglig(HighlightingType type, int offset)
	{
		try
		{
			DocumentLine lineByOffset = base.Document.GetLineByOffset(offset);
			return GetDocumentHighlighter.HighlightLine(lineByOffset.LineNumber).Sections.Any(s => s.Offset <= offset && s.Offset + s.Length >= offset && s.Color.Name == type.ToString());
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorBase.OffSetIsHiglig");
			return false;
		}
	}

	public bool OffSetIsHiglig(HighlightingType type, IEnumerable<HighlightedSection> sections, int offset)
	{
		try
		{
			return sections?.Any(s => s.Offset <= offset && s.Offset + s.Length >= offset && s.Color.Name == type.ToString()) ?? false;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorBase.OffSetIsHiglig");
			return false;
		}
	}

	public HighlightedSection GetHigSection(HighlightingType type, int offset)
	{
		try
		{
			DocumentLine lineByOffset = base.Document.GetLineByOffset(offset);
			return GetDocumentHighlighter.HighlightLine(lineByOffset.LineNumber).Sections.FirstOrDefault(s => s.Offset <= offset && s.Offset + s.Length >= offset && s.Color.Name == type.ToString());
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
			TextViewPosition? textViewPosition = GetMousePosition();
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

	private TextViewPosition? GetMousePosition()
	{
		return GetCurrentMousePosition();
	}

	internal TextViewPosition? oufgxu6L5T()
	{
		return GetCurrentMousePosition();
	}

	private TextViewPosition? GetCurrentMousePosition()
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

	private async void OnLoaded(object sender, RoutedEventArgs eventArgs)
	{
		try
		{
			if (!IsLoaded)
			{
				IsLoaded = true;
				if (base.Document != null)
				{
					base.Document.TextChanged += OnDocumentTextChanged;
				}
				if (base.EditorType == TextEditorType.Diff)
				{
					base.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
				}
				if (PvfFile != null && FileType == PvfFileType.lst && !AppSetting.Instance.EditConfig.NotUseFileListTooltip.Contains(PvfFile.FileName))
				{
					SectionCommentElementGenerator = new SectionCommentElementGenerator(this, PvfFile);
					base.TextArea.TextView.ElementGenerators.Add(SectionCommentElementGenerator);
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
						hoverTooltipManager = new EditorHoverTooltipManager(this, PvfFile);
					}
					break;
				case PvfFileType.txt:
				case PvfFileType.str:
				case PvfFileType.bin:
					break;
				}
				if (AllowFolding)
				{
					InitializeFolding();
					UpdateFolding();
				}
			}
			if (!ScrollChangedSubscribed)
			{
				ScrollViewer getScrollViewer = base.GetScrollViewer;
				if (getScrollViewer != null)
				{
					scrollViewerInitialized = true;
					getScrollViewer.ScrollChanged += OnScrollChanged;
					ScrollChangedSubscribed = true;
				}
			}
			await StartFoldingTimerAsync();
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
		StopFoldingTimer();
	}

	private Point GetPopupPosition(MouseEventArgs e)
	{
		Point position = e.GetPosition(this);
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

	private void InitializeFolding()
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

	private async Task RunFoldingTimerAsync()
	{
		int errId = 0;
		try
		{
			if (foldingTimer == null)
			{
				return;
			}
			try
			{
				while (true)
				{
					bool flag = foldingTimer != null;
					if (flag)
					{
						flag = await foldingTimer.WaitForNextTickAsync();
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
			logger.ErrorUploadDialog(e, $"折叠定时任务 ErrId:{errId}");
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

	private void StopFoldingTimer()
	{
		lock (foldingTimerLock)
		{
			if (foldingTimer != null)
			{
				try
				{
					foldingTimer?.Dispose();
					foldingTimer = null;
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

	private async Task StartFoldingTimerAsync()
	{
		if (foldingTimer == null)
		{
			foldingTimer = new PeriodicTimer(TimeSpan.FromSeconds(3.0));
			await RunFoldingTimerAsync();
		}
	}

	private static void OnTextIsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	{
		TextEditorBase textEditorBase = (TextEditorBase)(object)dependencyObject;
		if (textEditorBase != null && (bool)e.NewValue)
		{
			textEditorBase.AllowUpdateFolding = true;
		}
	}

	private void OnDocumentTextChanged(object? sender, EventArgs e)
	{
		TextIsChanged = true;
		AllowUpdateFolding = true;
	}

	private void OnCaretPositionChanged(object? sender, EventArgs e)
	{
		if (base.TextArea != null)
		{
			FocusCaretLine = base.TextArea.Caret.Line;
		}
	}

	private static void OnFocusCaretLineChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	{
		TextEditorBase textEditorBase = (TextEditorBase)(object)dependencyObject;
		if (textEditorBase != null && e.NewValue != null)
		{
			int line = (int)e.NewValue;
			textEditorBase.TextArea.Caret.Line = line;
		}
	}

	private void OnScrollChanged(object sender, ScrollChangedEventArgs eventArgs)
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
			_ = scrollViewerInitialized;
			ScrollViewer = getScrollViewer;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "滚动条改变事件：Bar_ScrollChanged");
		}
	}

	private static void OnVerticalOffsetChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	{
		TextEditorBase textEditorBase = (TextEditorBase)(object)dependencyObject;
		if (textEditorBase != null && e.NewValue != null)
		{
			double offset = (double)e.NewValue;
			textEditorBase.ScrollToVerticalOffset(offset);
		}
	}

	private static void OnIsActiveChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	{
		TextEditorBase textEditorBase = (TextEditorBase)(object)dependencyObject;
		if (textEditorBase != null && !Convert.ToBoolean(e.NewValue) && textEditorBase.codeCompletion != null)
		{
			textEditorBase.codeCompletion.CloseWindow();
		}
	}

	public void Dispose2()
	{
		try
		{
			base.GetScrollViewer.ScrollChanged -= OnScrollChanged;
			base.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
			base.Loaded -= OnLoaded;
			base.Unloaded -= TextEditorBase_Unloaded;
			base.Document.TextChanged -= OnDocumentTextChanged;
			StopFoldingTimer();
			FoldingStrategyBaseHelper?.Dispose();
			codeCompletion?.Dispose();
			hoverTooltipManager?.Dispose();
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
		if (!_contentLoaded)
		{
			_contentLoaded = true;
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
		_contentLoaded = true;
	}

	static TextEditorBase()
	{
		TextIsChangedProperty = DependencyProperty.Register("TextIsChanged", typeof(bool), typeof(TextEditorBase), new PropertyMetadata((object)false, new PropertyChangedCallback(OnTextIsChanged)));
		AllowCompletionProperty = DependencyProperty.Register("AllowCompletion", typeof(bool), typeof(TextEditorBase), new PropertyMetadata((object)false));
		AllowFoldingProperty = DependencyProperty.Register("AllowFolding", typeof(bool), typeof(TextEditorBase), new PropertyMetadata((object)true));
		FocusCaretProperty = DependencyProperty.Register("FocusCaretLine", typeof(int), typeof(TextEditorBase), new PropertyMetadata((object)0, new PropertyChangedCallback(OnFocusCaretLineChanged)));
		VerticalOffsetExProperty = DependencyProperty.Register("VerticalOffsetEx", typeof(double), typeof(TextEditorBase), new PropertyMetadata((object)0.0, new PropertyChangedCallback(OnVerticalOffsetChanged)));
		MaxNumExProperty = DependencyProperty.Register("MaxNumEx", typeof(double), typeof(TextEditorBase), new PropertyMetadata((object)0.0));
		ViewportSizeExProperty = DependencyProperty.Register("ViewportSizeEx", typeof(double), typeof(TextEditorBase), new PropertyMetadata((object)0.0));
		ScrollViewerProperty = DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(TextEditorBase), new PropertyMetadata((PropertyChangedCallback)null));
		IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(TextEditorBase), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsActiveChanged)));
		FileTypeProperty = DependencyProperty.Register("FileType", typeof(PvfFileType?), typeof(TextEditorBase), new PropertyMetadata((PropertyChangedCallback)null));
		PvfFileProperty = DependencyProperty.Register("PvfFile", typeof(PvfFile), typeof(TextEditorBase), new PropertyMetadata((PropertyChangedCallback)null));
	}
}
