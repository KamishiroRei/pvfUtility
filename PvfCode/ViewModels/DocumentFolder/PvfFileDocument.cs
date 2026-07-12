using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.Win32;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.Dot;
using PvfCode.Models.CodeCompletionModels;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services.PreviewPvfFileFolder;
using PvfCode.ViewModels.DocumentFolder.AniNpkLineElement;
using PvfCode.ViewModels.DocumentFolder.BackgroundRenderers;
using PvfCode.ViewModels.DocumentFolder.CodeCompletion;
using PvfCode.ViewModels.DocumentFolder.Enums;
using PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys;
using PvfCode.ViewModels.DocumentFolder.KorElementGenerator;
using PvfCode.ViewModels.DocumentFolder.PreviewControls;
using PvfCode.ViewModels.DocumentFolder.SearchViewModels;
using PvfCode.ViewModels.DocumentFolder.TextMarker;
using PvfCode.ViewModels.DocumentFolder.VerticalScrollBarHighlighted;
using PvfCode.ViewModels.DocumentFolder.VisualLineElementGenerators;
using TextEditLib;
using Utools;

namespace PvfCode.ViewModels.DocumentFolder;

public class PvfFileDocument : DocumentBase
{
	public event EventHandler PreviewContentChanged;

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass51_0
	{
		public string QHPelojyTg;

		public PvfFileDocument D2WejrvI73;

		public _003C_003Ec__DisplayClass51_0()
		{
		}

		internal void tcHeDCUgrV()
		{
			D2WejrvI73.Document.Text = QHPelojyTg;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass69_0
	{
		public string KqZeCTxUbb;

		public string ljreH6ilvr;

		public HashSet<int> Ss3eh96r4k;

		public List<VerticalScrollBarHighlightedData> fkVev7f78h;

		public double MOkeByij0B;

		public _003C_003Ec__DisplayClass69_0()
		{
		}

		internal void xUIeTGTnjS()
		{
			TextDocument textDocument = new TextDocument
			{
				Text = ljreH6ilvr
			};
			foreach (DocumentLine line in textDocument.Lines)
			{
				if (line.Length > 0 && textDocument.GetText(line).IndexOf(KqZeCTxUbb, 0, StringComparison.Ordinal) != -1)
				{
					Ss3eh96r4k.Add(line.LineNumber);
				}
			}
			foreach (int item in Ss3eh96r4k)
			{
				fkVev7f78h.Add(new VerticalScrollBarHighlightedData((double)(item - 1) * MOkeByij0B, 3, VerticalScrollBarHighlightedType.同音词));
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass72_0
	{
		public PvfFileDocument HEherawOGs;

		public HighlightedSection V0seW7VQWi;

		public _003C_003Ec__DisplayClass72_0()
		{
		}

		internal void sLfeFupwHY()
		{
			HEherawOGs.Editor.Select(V0seW7VQWi.Offset, V0seW7VQWi.Length);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass76_0
	{
		public PvfFileDocument TRke25VixJ;

		public Dictionary<int, string> K3RefcuG1X;

		public _003C_003Ec__DisplayClass76_0()
		{
		}

		internal Task? VAjemW3doK()
		{
			return AppCore.ViewModelBase.PVF.Strtable.DocumentSave(K3RefcuG1X, AppCore.ViewModelBase.PVF);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass76_1
	{
		public string vvpeSQWnWF;

		public _003C_003Ec__DisplayClass76_0 FsOeAxHtvW;

		public _003C_003Ec__DisplayClass76_1()
		{
		}

		internal void HH0e5Ptgta()
		{
			TextDocument textDocument = new TextDocument
			{
				Text = vvpeSQWnWF
			};
			DocumentHighlighter documentHighlighter = new DocumentHighlighter(textDocument, FsOeAxHtvW.TRke25VixJ.Highlighting);
			foreach (DocumentLine line in textDocument.Lines)
			{
				if (line.Length <= 0)
				{
					continue;
				}
				HighlightedLine highlightedLine = documentHighlighter.HighlightLine(line.LineNumber);
				if (highlightedLine != null && highlightedLine.Sections != null && highlightedLine.Sections.Count == 2)
				{
					IList<HighlightedSection> sections = highlightedLine.Sections;
					if (sections[0].Color.Name != "Section")
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
						defaultInterpolatedStringHandler.AppendLiteral("第：");
						defaultInterpolatedStringHandler.AppendFormatted(line.LineNumber);
						defaultInterpolatedStringHandler.AppendLiteral("行 起始类型不是字符串索引 请检查！");
						AppCore.ShowMsg(defaultInterpolatedStringHandler.ToStringAndClear(), isError: true);
						break;
					}
					if (sections[1].Color.Name != "String")
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(19, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("第：");
						defaultInterpolatedStringHandler2.AppendFormatted(line.LineNumber);
						defaultInterpolatedStringHandler2.AppendLiteral("行 第二个类型不是字符串 请检查！");
						AppCore.ShowMsg(defaultInterpolatedStringHandler2.ToStringAndClear(), isError: true);
						break;
					}
					if (sections[0].Length == 2)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(15, 1);
						defaultInterpolatedStringHandler3.AppendLiteral("第：");
						defaultInterpolatedStringHandler3.AppendFormatted(line.LineNumber);
						defaultInterpolatedStringHandler3.AppendLiteral("行 索引中没有编号 请检查");
						AppCore.ShowMsg(defaultInterpolatedStringHandler3.ToStringAndClear(), isError: true);
						break;
					}
					string text = textDocument.GetText(sections[0]);
					if (!int.TryParse(text.Substring(1, text.Length - 2), out var result))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(17, 1);
						defaultInterpolatedStringHandler4.AppendLiteral("第：");
						defaultInterpolatedStringHandler4.AppendFormatted(line.LineNumber);
						defaultInterpolatedStringHandler4.AppendLiteral("行 索引中不是int型 请检查");
						AppCore.ShowMsg(defaultInterpolatedStringHandler4.ToStringAndClear(), isError: true);
						break;
					}
					if (result < 0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(16, 1);
						defaultInterpolatedStringHandler5.AppendLiteral("第：");
						defaultInterpolatedStringHandler5.AppendFormatted(line.LineNumber);
						defaultInterpolatedStringHandler5.AppendLiteral("行 索引值不能小于0 请检查");
						AppCore.ShowMsg(defaultInterpolatedStringHandler5.ToStringAndClear(), isError: true);
						break;
					}
					if (FsOeAxHtvW.K3RefcuG1X.ContainsKey(result))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(18, 2);
						defaultInterpolatedStringHandler6.AppendLiteral("第：");
						defaultInterpolatedStringHandler6.AppendFormatted(line.LineNumber);
						defaultInterpolatedStringHandler6.AppendLiteral("行 与索引：[");
						defaultInterpolatedStringHandler6.AppendFormatted(result);
						defaultInterpolatedStringHandler6.AppendLiteral("] 重复请检查！ ");
						AppCore.ShowMsg(defaultInterpolatedStringHandler6.ToStringAndClear(), isError: true);
						break;
					}
					FsOeAxHtvW.K3RefcuG1X.Add(result, textDocument.GetText(sections[1]).Replace("`", string.Empty).Replace("\\n", "\r\n"));
				}
			}
		}
	}

	[CompilerGenerated]
	private PvfFile T7L5m9WtNU;

	private bool IsLoaded;

	public EncodingType NowEncoding
	{
		get
		{
			return GetProperty(() => NowEncoding);
		}
		set
		{
			SetProperty(() => NowEncoding, value, qg0fNntIPb);
			RaisePropertyChanged("EditorEdcoding");
		}
	}

	public Encoding EditorEdcoding => Encoding.GetEncoding((int)NowEncoding);

	public SearchViewModel SearchPanel
	{
		get
		{
			return GetProperty(() => SearchPanel);
		}
		set
		{
			SetProperty<SearchViewModel>(() => SearchPanel, value);
		}
	}

	public override string FileName
	{
		get
		{
			if (base.DocumentType == PvfFileDocumentType.PVF文档)
			{
				return File?.ShortName;
			}
			return base.DocumentType.ToString();
		}
	}

	public override int? Rarity
	{
		get
		{
			if (File == null || !File.IsScriptFile)
			{
				return null;
			}
			if (!File.GetRarity((PvfPack)AppCore.ViewModelBase.PVF, out int rarity))
			{
				return null;
			}
			return rarity;
		}
	}

	public PvfFile File
	{
		[CompilerGenerated]
		get
		{
			return T7L5m9WtNU;
		}
		[CompilerGenerated]
		set
		{
			T7L5m9WtNU = value;
		}
	}

	public string FullPath => File?.FileName;

	public string ToolBarFullPath
	{
		get
		{
			return GetProperty(() => ToolBarFullPath);
		}
		set
		{
			SetProperty<string>(() => ToolBarFullPath, value);
		}
	}

	public TextDocument Document
	{
		get
		{
			return GetProperty(() => Document);
		}
		set
		{
			SetProperty<TextDocument>(() => Document, value);
		}
	}

	public bool TextIsChanged
	{
		get
		{
			return GetProperty(() => TextIsChanged);
		}
		set
		{
			SetProperty(() => TextIsChanged, value);
			Sy2fzpfUqV();
		}
	}

	public IHighlightingDefinition Highlighting
	{
		get
		{
			return GetProperty(() => Highlighting);
		}
		set
		{
			SetProperty<IHighlightingDefinition>(() => Highlighting, value);
		}
	}

	public VerticalScrollBarHighlightedMagager VerticalScrollBarHighlightedMagager
	{
		get
		{
			return GetProperty(() => VerticalScrollBarHighlightedMagager);
		}
		set
		{
			SetProperty<VerticalScrollBarHighlightedMagager>(() => VerticalScrollBarHighlightedMagager, value);
		}
	}

	public TextEditorPreviewViewModelBase TextEditorPreviewViewModelBase
	{
		get
		{
			return GetProperty(() => TextEditorPreviewViewModelBase);
		}
		set
		{
			SetProperty<TextEditorPreviewViewModelBase>(() => TextEditorPreviewViewModelBase, value);
		}
	}

	public bool TextEditorPrivewViewVisibility
	{
		get
		{
			return GetProperty(() => TextEditorPrivewViewVisibility);
		}
		set
		{
			SetProperty(() => TextEditorPrivewViewVisibility, value);
		}
	}

	public int FocusCaretLine
	{
		get
		{
			return GetProperty(() => FocusCaretLine);
		}
		set
		{
			SetProperty(() => FocusCaretLine, value, Ju3f3ldpmP);
		}
	}

	public FilePreviewDataBase? ItemPreviewData
	{
		get
		{
			if (AppSetting.Instance.PvfFilePreviewOptions.ForbidShowToolTip)
			{
				return null;
			}
			if (File != null && AppCore.ViewModelBase.PVF != null)
			{
				return FilePreviewDataBase.Create(AppCore.ViewModelBase.PVF, File, base.Icon);
			}
			return null;
		}
	}

	public bool PreviewItemVisibility
	{
		get
		{
			if (File != null && AppCore.ViewModelBase.PVF != null)
			{
				switch (File.FileType)
				{
				case PvfFileType.equ:
					return File.IsScriptFile;
				case PvfFileType.stk:
					return File.IsScriptFile;
				case PvfFileType.shp:
					return File.IsScriptFile;
				}
			}
			return false;
		}
	}

	public bool SupportsPreview => PvfPreviewDocument.Supports(File);

	public TextEditorBase Editor
	{
		get
		{
			return GetProperty(() => Editor);
		}
		set
		{
			SetProperty<TextEditorBase>(() => Editor, value);
		}
	}

	public AttachType? AttachType
	{
		get
		{
			if (base.Icon == null || base.Icon == Res.Instance.Editor.HighImportance)
			{
				return null;
			}
			PvfFile file = File;
			if (file == null || file.FileType != PvfFileType.equ)
			{
				return null;
			}
			PvfFile file2 = File;
			if (file2 != null && file2.GetAttachType(AppCore.ViewModelBase.PVF, out var attachType))
			{
				return attachType;
			}
			return null;
		}
	}

	private void w3MfVLPjEt()
	{
		RaisePropertyChanged("Rarity");
	}

	[SpecialName]
	private IHighlighter BCo5rRKg8l()
	{
		return new DocumentHighlighter(Document, Highlighting);
	}

	public PvfFileDocument(PvfFile file)
		: base(file.FileName)
	{
		base.DocumentType = PvfFileDocumentType.PVF文档;
		Document = new TextDocument();
		File = file;
		if (file != null)
		{
			if (file.FileType == PvfFileType.nut)
			{
				NowEncoding = AppSetting.Instance.EditConfig.NutDefaultEncodingType;
			}
			else
			{
				NowEncoding = AppSetting.Instance.PvfConfig.DefaultEncoding;
			}
			ToolBarFullPath = file.FileName;
		}
		else
		{
			NowEncoding = AppSetting.Instance.PvfConfig.DefaultEncoding;
		}
		SetIHighlighting();
		TextIsChanged = false;
	}

	private void Ju3f3ldpmP()
	{
		if (Document != null && Editor != null && VerticalScrollBarHighlightedMagager != null)
		{
			double num = Editor.MaxNumEx / (double)Document.LineCount;
			double position = (double)(FocusCaretLine - 1) * num;
			VerticalScrollBarHighlightedMagager.RefreshSelectedRow(position);
		}
	}

	[Command]
	public async void RefDocumentText()
	{
		try
		{
			base.IsLoading = true;
			if (Editor != null && Editor.FoldingStrategyBaseHelper != null)
			{
				Editor.FoldingStrategyBaseHelper?.Clear();
			}
			if (Document == null)
			{
				AppCore.Logger.ErrorUploadDialog(new Exception("DocumentIsNull"), "PvfFileDocument.RefDocumentText");
				return;
			}
			await Task.Run(async delegate
			{
				_003C_003Ec__DisplayClass51_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass51_0();
				CS_0024_003C_003E8__locals4.D2WejrvI73 = this;
				PvfGroup pvf = AppCore.ViewModelBase.PVF;
				if (FullPath == null)
				{
					AppCore.Logger.ErrorUploadDialog(new Exception("Document文件路径不能为Null"), "PvfFileDocument.RefDocumentText");
				}
				else if (pvf == null)
				{
					AppCore.Logger.ErrorUploadDialog(new Exception("pvf已关闭"), "PvfFileDocument.RefDocumentText");
				}
				else
				{
					string qHPelojyTg = ((!(FullPath == "stringtable.bin")) ? pvf.GetFileText(File, NowEncoding) : (await AppCore.ViewModelBase.PVF.Strtable.GetDocumentText()));
					CS_0024_003C_003E8__locals4.QHPelojyTg = qHPelojyTg;
					if (FullPath == "stringtable.bin")
					{
						await (pvf.Strtable?.LoadQuote(pvf));
					}
					else if (FullPath.Contains("kor.str"))
					{
						await (pvf.Strview?.InitStringViewQuote(pvf, FullPath));
					}
					await ((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
					{
						CS_0024_003C_003E8__locals4.D2WejrvI73.Document.Text = CS_0024_003C_003E8__locals4.QHPelojyTg;
					}, Array.Empty<object>());
				}
			});
			TextIsChanged = false;
			base.IsLoading = false;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfFileDocument.RefDocumentText");
		}
	}

	private void eoDfRKFuDC()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		if (Editor == null || Editor.TextArea.TextView.BackgroundRenderers == null || File.FileType != PvfFileType.lst)
		{
			return;
		}
		IHighlightingDefinition syntaxHighlighting = Editor.SyntaxHighlighting;
		string text = Document.Text;
		if (syntaxHighlighting == null || string.IsNullOrEmpty(text))
		{
			return;
		}
		TextMarkerService textMarkerService = new TextMarkerService(Document);
		TextView textView = Editor.TextArea.TextView;
		IBackgroundRenderer[] array = textView.BackgroundRenderers.ToArray();
		foreach (IBackgroundRenderer backgroundRenderer in array)
		{
			if (backgroundRenderer is TextMarkerService)
			{
				textView.BackgroundRenderers.Remove(backgroundRenderer);
			}
		}
		IVisualLineTransformer[] array2 = textView.LineTransformers.ToArray();
		foreach (IVisualLineTransformer visualLineTransformer in array2)
		{
			if (visualLineTransformer is TextMarkerService)
			{
				textView.LineTransformers.Remove(visualLineTransformer);
			}
		}
		textView.Services.RemoveService(typeof(TextMarkerService));
		textView.Services.RemoveService(typeof(EnhancedScrollBar));
		TextDocument textDocument = new TextDocument();
		textDocument.Text = text;
		DocumentHighlighter documentHighlighter = new DocumentHighlighter(textDocument, syntaxHighlighting);
		if (documentHighlighter == null)
		{
			return;
		}
		HashSet<int> hashSet = new HashSet<int>();
		HashSet<string> hashSet2 = new HashSet<string>();
		foreach (DocumentLine line in textDocument.Lines)
		{
			if (line.Length == 0)
			{
				continue;
			}
			HighlightedLine highlightedLine = documentHighlighter.HighlightLine(line.LineNumber);
			if (highlightedLine == null || highlightedLine.Sections == null)
			{
				continue;
			}
			IList<HighlightedSection> sections = highlightedLine.Sections;
			if (sections.Count != 2)
			{
				textMarkerService.Create(line.Offset, line.Length);
				continue;
			}
			if (sections[0].Color.Name != "Digits" || sections[1].Color.Name != "String")
			{
				textMarkerService.Create(line.Offset, line.Length);
				continue;
			}
			if (int.TryParse(textDocument.GetText(sections[0]), out var result))
			{
				if (hashSet.Contains(result))
				{
					ITextMarker textMarker = textMarkerService.Create(sections[0].Offset, sections[0].Length);
					textMarker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
					textMarker.ForegroundColor = Colors.Red;
				}
				else
				{
					hashSet.Add(result);
				}
			}
			else
			{
				textMarkerService.Create(sections[0].Offset, sections[0].Length);
			}
			HighlightedSection highlightedSection = sections[1];
			string text2 = textDocument.GetText(highlightedSection);
			if (hashSet2.Contains(text2))
			{
				textMarkerService.Create(highlightedSection.Offset, highlightedSection.Length);
			}
			else
			{
				hashSet2.Add(text2);
			}
		}
		stopwatch.Stop();
		TimeSpan elapsed = stopwatch.Elapsed;
		textView.BackgroundRenderers.Add(textMarkerService);
		textView.LineTransformers.Add(textMarkerService);
		textView.Services.AddService(typeof(TextMarkerService), textMarkerService);
		LoggerViewModel logger = AppCore.Logger;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
		defaultInterpolatedStringHandler.AppendLiteral("程序耗时:'");
		defaultInterpolatedStringHandler.AppendFormatted(elapsed);
		defaultInterpolatedStringHandler.AppendLiteral("'秒");
		logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
	}

	[Command]
	public void OnClose()
	{
		AppCore.ViewModelBase.RootDocument.RemoveDocument(FullPath, isShowDialog: true);
	}

	[Command]
	public void OpenWindowAddCodeCompletionData()
	{
		if (File != null)
		{
			WindowAddCodeCompletionData windowAddCodeCompletionData = new WindowAddCodeCompletionData(CodeCompletionData.Create(File.FileType));
			windowAddCodeCompletionData.Owner = Application.Current.MainWindow;
			windowAddCodeCompletionData.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			windowAddCodeCompletionData.ShowDialog();
		}
	}

	public void SetIHighlighting()
	{
		if (File != null)
		{
			if (AppSetting.Instance.PvfConfig.LstFileUseScriptFile.Contains(FullPath))
			{
				Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.equ);
			}
			else
			{
				Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(File.FileType);
			}
		}
	}

	private void qg0fNntIPb()
	{
		RefDocumentText();
	}

	[Command]
	public void TextEditLoad(object sender)
	{
		if (!IsLoaded)
		{
			PvfFileType fileType = File.FileType;
			VerticalScrollBarHighlightedMagager = new VerticalScrollBarHighlightedMagager();
			IsLoaded = true;
			Editor = (TextEditorBase)sender;
			SearchPanel = new SearchViewModel(Editor, FullPath);
			Editor.TextArea.SelectionChanged += iPd5jDePU5;
			Editor.TextArea.Caret.PositionChanged += j3Z5l8Bm4b;
			Editor.TextArea.TextView.Drop += llF5vRk3y2;
			Editor.TextArea.MouseDoubleClick += Q795T3kTJ8;
			Sy2fzpfUqV();
			switch (fileType)
			{
			case PvfFileType.ani:
				TextEditorPreviewViewModelBase = new TextEditorPreviewViewModelAni(Editor, File);
				Editor.TextArea.TextView.ElementGenerators.Add(new AniNpkElementGenerator(BCo5rRKg8l()));
				break;
			case PvfFileType.str:
				Editor.TextArea.TextView.ElementGenerators.Add(new KorFileElementGenerator(Editor.GetDocumentHighlighter, File));
				break;
			}
			if (FullPath == "stringtable.bin")
			{
				Editor.TextArea.TextView.ElementGenerators.Add(new StringQuoteElementGenerator(Editor.GetDocumentHighlighter));
			}
			if (fileType != PvfFileType.str && fileType != PvfFileType.txt && fileType != PvfFileType.bin)
			{
				ScriptCommentLineGenerator item = new ScriptCommentLineGenerator(Editor, Editor.GetDocumentHighlighter, File);
				Editor.TextArea.TextView.ElementGenerators.Add(item);
			}
			TextEditorPrivewViewVisibility = TextEditorPreviewViewModelBase != null;
			_ = Editor.TextArea.TextView;
		}
		if (base.DocumentType == PvfFileDocumentType.PVF文档)
		{
			DocumentNavigationService.Instance.Add(new NavigationData
			{
				FilePath = FullPath,
				Text = ((Editor.Line < 1) ? "" : Document.GetText(Document.GetLineByNumber(Editor.Line))),
				Line = Editor.Line,
				Column = Editor.Column
			});
			Editor.Focus();
		}
	}

	[Command]
	public void OnPrivewAni()
	{
		if (AppSetting.Instance.EditConfig.ShowAniPreviewPanel)
		{
			if (File.FileType == PvfFileType.ani)
			{
				TextEditorPreviewViewModelBase = new TextEditorPreviewViewModelAni(Editor, File);
				if (base.IsActive)
				{
					TextEditorPreviewViewModelBase.LoadData(Document.Text);
				}
			}
		}
		else
		{
			TextEditorPreviewViewModelBase.Dispose();
		}
	}

	[Command]
	public void OnOpenPreview()
	{
		AppCore.ViewModelBase.RootDocument.OpenPreview(this);
	}

	public void NavigateToTag(int offset, int length)
	{
		base.IsActive = true;
		if (Application.Current == null)
		{
			return;
		}
		Application.Current.Dispatcher.BeginInvoke((Action)delegate
		{
			if (Editor == null || Document == null || Document.TextLength == 0)
			{
				return;
			}
			int safeOffset = Math.Max(0, Math.Min(offset, Document.TextLength - 1));
			int safeLength = Math.Max(0, Math.Min(length, Document.TextLength - safeOffset));
			Editor.Select(safeOffset, safeLength);
			Editor.TextArea.Caret.Offset = safeOffset;
			Editor.ScrollToLine(Document.GetLineByOffset(safeOffset).LineNumber);
			Editor.Focus();
		}, DispatcherPriority.Background);
	}

	private async void Sy2fzpfUqV()
	{
		try
		{
			if (TextIsChanged)
			{
				base.Icon = Res.Instance.Editor.HighImportance;
				return;
			}
			ResultData<ImageSource> resultData = await Task.Run(() => AppCore.ViewModelBase.PVF.GetScriptIconSource(File));
			if (resultData.IsError)
			{
				AppCore.Logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_GetIconError"), resultData.Msg));
			}
			else if (resultData.Data != null)
			{
				base.Icon = resultData.Data;
			}
			else
			{
				base.Icon = null;
			}
			RaisePropertyChanged("AttachType");
		}
		catch (Exception)
		{
		}
	}

	private void j3Z5l8Bm4b(object? sender, EventArgs P_1)
	{
		try
		{
			DocumentNavigationService.Instance.Add(new NavigationData
			{
				FilePath = FullPath,
				DocumentOffset = Editor.TextArea.Caret.Offset,
				Line = Editor.Line,
				Column = Editor.Column,
				Text = ((Editor.Line < 1) ? "" : Document.GetText(Document.GetLineByNumber(Editor.Line)))
			});
		}
		catch (Exception)
		{
		}
	}

	public TextEdit GetEditor()
	{
		return Editor;
	}

	private async void iPd5jDePU5(object? sender, EventArgs P_1)
	{
		_003C_003Ec__DisplayClass69_0 CS_0024_003C_003E8__locals15 = new _003C_003Ec__DisplayClass69_0();
		VerticalScrollBarHighlightedMagager.Remove(VerticalScrollBarHighlightedType.同音词);
		CS_0024_003C_003E8__locals15.KqZeCTxUbb = Editor.SelectedText;
		if (string.IsNullOrEmpty(CS_0024_003C_003E8__locals15.KqZeCTxUbb) || CS_0024_003C_003E8__locals15.KqZeCTxUbb.Length >= 1000 || Document.LineCount > 5000)
		{
			return;
		}
		CS_0024_003C_003E8__locals15.ljreH6ilvr = Document.Text;
		CS_0024_003C_003E8__locals15.fkVev7f78h = new List<VerticalScrollBarHighlightedData>();
		CS_0024_003C_003E8__locals15.Ss3eh96r4k = new HashSet<int>();
		CS_0024_003C_003E8__locals15.MOkeByij0B = Editor.MaxNumEx / (double)Document.LineCount;
		lock (this)
		{
		}
		await Task.Run(delegate
		{
			TextDocument textDocument = new TextDocument
			{
				Text = CS_0024_003C_003E8__locals15.ljreH6ilvr
			};
			foreach (DocumentLine line in textDocument.Lines)
			{
				if (line.Length > 0 && textDocument.GetText(line).IndexOf(CS_0024_003C_003E8__locals15.KqZeCTxUbb, 0, StringComparison.Ordinal) != -1)
				{
					CS_0024_003C_003E8__locals15.Ss3eh96r4k.Add(line.LineNumber);
				}
			}
			foreach (int item in CS_0024_003C_003E8__locals15.Ss3eh96r4k)
			{
				CS_0024_003C_003E8__locals15.fkVev7f78h.Add(new VerticalScrollBarHighlightedData((double)(item - 1) * CS_0024_003C_003E8__locals15.MOkeByij0B, 3, VerticalScrollBarHighlightedType.同音词));
			}
		});
		if (CS_0024_003C_003E8__locals15.fkVev7f78h.Count <= 1000)
		{
			VerticalScrollBarHighlightedMagager.Items.AddRange(CS_0024_003C_003E8__locals15.fkVev7f78h);
		}
	}

	public void BarStaticItem_KeyDown(object sender, KeyEventArgs e)
	{
		if ((int)e.Key == 6)
		{
			AppCore.ViewModelBase.PvfFileTreeViewModel.GoToNode(File.FileName);
		}
	}

	private void Q795T3kTJ8(object P_0, MouseButtonEventArgs P_1)
	{
		S625CmQKDe(P_1);
	}

	private void S625CmQKDe(MouseButtonEventArgs P_0)
	{
		_003C_003Ec__DisplayClass72_0 CS_0024_003C_003E8__locals11 = new _003C_003Ec__DisplayClass72_0();
		CS_0024_003C_003E8__locals11.HEherawOGs = this;
		if (Editor.GetMouseOffset().HasValue)
		{
			return;
		}
		int caretOffset = Editor.CaretOffset;
		DocumentLine lineByOffset = Document.GetLineByOffset(caretOffset);
		HighlightedLine highlightedLine = BCo5rRKg8l().HighlightLine(lineByOffset.LineNumber);
		if (highlightedLine == null || highlightedLine.Sections == null || highlightedLine.Sections.Count <= 0)
		{
			return;
		}
		CS_0024_003C_003E8__locals11.V0seW7VQWi = highlightedLine.Sections[highlightedLine.Sections.Count - 1];
		if (Document.GetCharAt(CS_0024_003C_003E8__locals11.V0seW7VQWi.Offset) == '`' && Document.GetCharAt(CS_0024_003C_003E8__locals11.V0seW7VQWi.EndOffset) == '`')
		{
			if (CS_0024_003C_003E8__locals11.V0seW7VQWi.Length == 2)
			{
				return;
			}
			CS_0024_003C_003E8__locals11.V0seW7VQWi = new HighlightedSection
			{
				Offset = CS_0024_003C_003E8__locals11.V0seW7VQWi.Offset + 1,
				Length = CS_0024_003C_003E8__locals11.V0seW7VQWi.Length - 1
			};
		}
		((DispatcherObject)Editor).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			CS_0024_003C_003E8__locals11.HEherawOGs.Editor.Select(CS_0024_003C_003E8__locals11.V0seW7VQWi.Offset, CS_0024_003C_003E8__locals11.V0seW7VQWi.Length);
		}, Array.Empty<object>());
		P_0.Handled = true;
	}

	[Command]
	public void OnOpenNewDocument()
	{
		try
		{
			if (string.IsNullOrEmpty(ToolBarFullPath))
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_FilePathCannotBeEmpty"));
				ToolBarFullPath = FullPath;
			}
			else if (!AppCore.ViewModelBase.PVF.FileAny(ToolBarFullPath))
			{
				AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FilePathNotExist"), ToolBarFullPath));
				ToolBarFullPath = FullPath;
			}
			else
			{
				AppCore.ViewModelBase.RootDocument.AddDocument(ToolBarFullPath, gotoNode: true);
				ToolBarFullPath = FullPath;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfFileDocument.OnOpenNewDocument");
		}
	}

	[Command]
	public void GoToNodeNowFile()
	{
		AppCore.ViewModelBase.PvfFileTreeViewModel.GoToNode(File.FileName);
	}

	[Command]
	public void OnSave()
	{
		try
		{
			if (File == null)
			{
				AppCore.ShowMsg("PvfFileDcoument.OnSave PvfFileIsNull", isError: true);
				return;
			}
			if (FullPath == "stringtable.bin")
			{
				TnA5hQnSic();
				return;
			}
			if (File.FileType == PvfFileType.nut && AppSetting.Instance.EditConfig.SaveBeautifyNutCode)
			{
				BeautifyCode(isSelected: false);
			}
			string fileText = AppCore.ViewModelBase.PVF.GetFileText(File, NowEncoding);
			bool isUpdated = File.IsUpdated;
			if (AppCore.ViewModelBase.PVF.SaveFileText(File, Document.Text, NowEncoding))
			{
				TnA5hQnSic();
				PreviewContentChanged?.Invoke(this, EventArgs.Empty);
			}
			if (fileText == AppCore.ViewModelBase.PVF.GetFileText(File, NowEncoding) && !isUpdated)
			{
				File.IsUpdated = false;
			}
			w3MfVLPjEt();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfFileDocument.OnSave");
		}
	}

	private async Task Vgy5H0kDYW()
	{
		_003C_003Ec__DisplayClass76_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass76_0();
		CS_0024_003C_003E8__locals5.TRke25VixJ = this;
		base.IsLoading = true;
		CS_0024_003C_003E8__locals5.K3RefcuG1X = new Dictionary<int, string>();
		if (!string.IsNullOrEmpty(Document.Text))
		{
			_003C_003Ec__DisplayClass76_1 obj = new _003C_003Ec__DisplayClass76_1();
			obj.FsOeAxHtvW = CS_0024_003C_003E8__locals5;
			obj.vvpeSQWnWF = Document.Text;
			await Task.Run(delegate
			{
				TextDocument textDocument = new TextDocument
				{
					Text = obj.vvpeSQWnWF
				};
				DocumentHighlighter documentHighlighter = new DocumentHighlighter(textDocument, obj.FsOeAxHtvW.TRke25VixJ.Highlighting);
				foreach (DocumentLine line in textDocument.Lines)
				{
					if (line.Length > 0)
					{
						HighlightedLine highlightedLine = documentHighlighter.HighlightLine(line.LineNumber);
						if (highlightedLine != null && highlightedLine.Sections != null && highlightedLine.Sections.Count == 2)
						{
							IList<HighlightedSection> sections = highlightedLine.Sections;
							if (sections[0].Color.Name != "Section")
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
								defaultInterpolatedStringHandler.AppendLiteral("第：");
								defaultInterpolatedStringHandler.AppendFormatted(line.LineNumber);
								defaultInterpolatedStringHandler.AppendLiteral("行 起始类型不是字符串索引 请检查！");
								AppCore.ShowMsg(defaultInterpolatedStringHandler.ToStringAndClear(), isError: true);
								break;
							}
							if (sections[1].Color.Name != "String")
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(19, 1);
								defaultInterpolatedStringHandler2.AppendLiteral("第：");
								defaultInterpolatedStringHandler2.AppendFormatted(line.LineNumber);
								defaultInterpolatedStringHandler2.AppendLiteral("行 第二个类型不是字符串 请检查！");
								AppCore.ShowMsg(defaultInterpolatedStringHandler2.ToStringAndClear(), isError: true);
								break;
							}
							if (sections[0].Length == 2)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(15, 1);
								defaultInterpolatedStringHandler3.AppendLiteral("第：");
								defaultInterpolatedStringHandler3.AppendFormatted(line.LineNumber);
								defaultInterpolatedStringHandler3.AppendLiteral("行 索引中没有编号 请检查");
								AppCore.ShowMsg(defaultInterpolatedStringHandler3.ToStringAndClear(), isError: true);
								break;
							}
							string text = textDocument.GetText(sections[0]);
							if (!int.TryParse(text.Substring(1, text.Length - 2), out var result))
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(17, 1);
								defaultInterpolatedStringHandler4.AppendLiteral("第：");
								defaultInterpolatedStringHandler4.AppendFormatted(line.LineNumber);
								defaultInterpolatedStringHandler4.AppendLiteral("行 索引中不是int型 请检查");
								AppCore.ShowMsg(defaultInterpolatedStringHandler4.ToStringAndClear(), isError: true);
								break;
							}
							if (result < 0)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(16, 1);
								defaultInterpolatedStringHandler5.AppendLiteral("第：");
								defaultInterpolatedStringHandler5.AppendFormatted(line.LineNumber);
								defaultInterpolatedStringHandler5.AppendLiteral("行 索引值不能小于0 请检查");
								AppCore.ShowMsg(defaultInterpolatedStringHandler5.ToStringAndClear(), isError: true);
								break;
							}
							if (obj.FsOeAxHtvW.K3RefcuG1X.ContainsKey(result))
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(18, 2);
								defaultInterpolatedStringHandler6.AppendLiteral("第：");
								defaultInterpolatedStringHandler6.AppendFormatted(line.LineNumber);
								defaultInterpolatedStringHandler6.AppendLiteral("行 与索引：[");
								defaultInterpolatedStringHandler6.AppendFormatted(result);
								defaultInterpolatedStringHandler6.AppendLiteral("] 重复请检查！ ");
								AppCore.ShowMsg(defaultInterpolatedStringHandler6.ToStringAndClear(), isError: true);
								break;
							}
							obj.FsOeAxHtvW.K3RefcuG1X.Add(result, textDocument.GetText(sections[1]).Replace("`", string.Empty).Replace("\\n", "\r\n"));
						}
					}
				}
			});
		}
		else if (CS_0024_003C_003E8__locals5.K3RefcuG1X.Count == 0 && AppCore.Logger.ShowDialog("当前字符串表为空 确定要继续保存？") != MessageResult.Yes)
		{
			return;
		}
		await Task.Run(() => AppCore.ViewModelBase.PVF.Strtable.DocumentSave(CS_0024_003C_003E8__locals5.K3RefcuG1X, AppCore.ViewModelBase.PVF));
		base.IsLoading = false;
	}

	private void TnA5hQnSic()
	{
		try
		{
			TextIsChanged = false;
			KeyValuePair<string, PvfTreeFileBase>? keyValuePair = AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.FilePathGetTreeNode(File.FileName);
			if (keyValuePair.HasValue)
			{
				keyValuePair.Value.Value.FileNameDoNotify();
			}
			keyValuePair = AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.FilePathGetTreeNode(File.FileName);
			if (keyValuePair.HasValue)
			{
				keyValuePair.Value.Value.FileNameDoNotify();
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfFileDocument.SaveSuccessChangeData");
		}
	}

	[Command]
	public void OnSaveAllDocument()
	{
		AppCore.ViewModelBase.RootDocument.SaveAllDocument();
	}

	[Command]
	public void OnSwitchFolding(bool expand)
	{
		try
		{
			if (Editor.FoldingStrategyBaseHelper == null || Editor.FoldingStrategyBaseHelper.Manager.AllFoldings == null)
			{
				return;
			}
			foreach (FoldingSection allFolding in Editor.FoldingStrategyBaseHelper.Manager.AllFoldings)
			{
				allFolding.IsFolded = !expand;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfFileDocument.OnSwitchFolding");
		}
	}

	[Command]
	public void OnMouseUp()
	{
		int? num = Editor?.GetMouseOffset();
		if (num.HasValue)
		{
			Editor.CaretOffset = num.Value;
		}
	}

	[Command]
	public void OnSwitchFoldingCurrentRange()
	{
		if (Editor.FoldingStrategyBaseHelper == null || Editor.FoldingStrategyBaseHelper.Manager.AllFoldings == null)
		{
			return;
		}
		int caretOffset = Editor.CaretOffset;
		DocumentLine lineByOffset = Document.GetLineByOffset(caretOffset);
		if (lineByOffset == null)
		{
			return;
		}
		FoldingSection foldingSection = null;
		foreach (FoldingSection allFolding in Editor.FoldingStrategyBaseHelper.Manager.AllFoldings)
		{
			if (!allFolding.IsFolded && ((allFolding.StartOffset >= lineByOffset.Offset && allFolding.StartOffset <= lineByOffset.EndOffset) || (allFolding.EndOffset >= lineByOffset.Offset && allFolding.StartOffset <= lineByOffset.EndOffset)))
			{
				foldingSection = allFolding;
			}
		}
		if (foldingSection != null)
		{
			foldingSection.IsFolded = true;
		}
	}

	[Command]
	public void OnCommentCode()
	{
		try
		{
			if (File.FileType == PvfFileType.nut)
			{
				new ScriptCommentController(BCo5rRKg8l(), Editor).CommentCode();
			}
		}
		catch (Exception ex)
		{
			AppCore.Logger.Error("代码注释错误：" + ex.Message);
		}
	}

	[Command]
	public void OnUncommentCode()
	{
		try
		{
			if (File.FileType == PvfFileType.nut)
			{
				new ScriptCommentController(BCo5rRKg8l(), Editor).ClearComments();
			}
		}
		catch (Exception ex)
		{
			AppCore.Logger.Error("代码取消注释错误：" + ex.Message);
		}
	}

	[Command]
	public void OnGoToTreeFileView()
	{
		AppCore.ViewModelBase.PvfFileTreeViewModel.GoToNode(File.FileName);
	}

	[Command]
	public void OnCopy()
	{
		try
		{
			string selectedText = Editor.SelectedText;
			if (!string.IsNullOrEmpty(selectedText))
			{
				AppCore.CopyString(selectedText);
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfFileDocument.OnCopy");
		}
	}

	[Command]
	public void OnCopyFilePath()
	{
		try
		{
			AppCore.CopyString(File.FileName);
		}
		catch (Exception ex)
		{
			AppCore.Logger.Error(ex.Message);
		}
	}

	[Command]
	public async void DocumentTextSaveAs()
	{
		try
		{
			string extension = File.Extension;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 3);
			defaultInterpolatedStringHandler.AppendFormatted(AppSetting.Instance.GetIlogger()?.GetStr("PvfFileScriptName"));
			defaultInterpolatedStringHandler.AppendLiteral(" (*");
			defaultInterpolatedStringHandler.AppendFormatted(extension);
			defaultInterpolatedStringHandler.AppendLiteral(")|*");
			defaultInterpolatedStringHandler.AppendFormatted(extension);
			string filter = defaultInterpolatedStringHandler.ToStringAndClear();
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = filter,
				FileName = FileName
			};
			bool? flag = saveFileDialog.ShowDialog();
			if (flag.HasValue && flag.Value)
			{
				await System.IO.File.WriteAllTextAsync(saveFileDialog.FileName, Document.Text);
			}
		}
		catch (Exception ex)
		{
			AppCore.Logger.Error(ex.Message);
			AppCore.ShowMsg(ex.Message);
		}
	}

	[Command]
	public void OnSaveToAniFile()
	{
		if (AppCore.ViewModelBase.PVF.SaveFileAsBinaryAni(File, Document.Text))
		{
			TnA5hQnSic();
		}
	}

	[Command]
	public void OnSaveToScriptFile()
	{
		if (AppCore.ViewModelBase.PVF.SaveFileAsScript(File, Document.Text))
		{
			TnA5hQnSic();
		}
	}

	[Command]
	public void OnGoToLine()
	{
		try
		{
			WindowGotoLine windowGotoLine = new WindowGotoLine
			{
				Offset = Editor.CaretOffset,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				Owner = Application.Current.MainWindow
			};
			if (windowGotoLine.ShowDialog().Value)
			{
				Editor.Focus();
				Editor.CaretOffset = Math.Clamp(windowGotoLine.Offset, 0, Document.TextLength);
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfFileDocument.OnGoToLine");
		}
	}

	[Command]
	public void OnCopyNowLinePasteNextLine()
	{
		try
		{
			int offset = Editor.TextArea.Caret.Offset;
			DocumentLine lineByOffset = Document.GetLineByOffset(offset);
			if (lineByOffset.Length > 0)
			{
				string text = Document.GetText(lineByOffset);
				Document.Insert(lineByOffset.EndOffset, "\r\n" + text);
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfFileDocument.OnCopyNowLinePasteNextLine");
		}
	}

	[Command]
	public void BeautifyCode(bool isSelected)
	{
		try
		{
			int offset = Editor.TextArea.Caret.Offset;
			Document.Text = new JSBeautify(Document.Text, new JSBeautifyOptions()).GetResult();
			if (offset <= Document.TextLength)
			{
				Editor.TextArea.Caret.Offset = offset;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfFileDocument.BeautifyCode");
		}
	}

	[Command]
	public void OnInsertLstNullLine()
	{
		try
		{
			if (File != null)
			{
				int lstNumMax = AppCore.ViewModelBase.PVF.ListFileTable.GetLstNumMax(File.FileName);
				TextDocument document = Document;
				int textLength = Document.TextLength;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 1);
				defaultInterpolatedStringHandler.AppendLiteral("\r\n");
				defaultInterpolatedStringHandler.AppendFormatted(lstNumMax + 1);
				defaultInterpolatedStringHandler.AppendLiteral("\t``");
				document.Insert(textLength, defaultInterpolatedStringHandler.ToStringAndClear());
				Editor.TextArea.Caret.Offset = Document.TextLength - 1;
				Editor.ScrollToLine(Document.LineCount);
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfFileDocument.OnInsertLstNullLine");
		}
	}

	[Command]
	public void OnCopyItemCode()
	{
		try
		{
			if (File != null)
			{
				int? itemCode = File.ItemCode;
				if (itemCode.HasValue)
				{
					AppCore.CopyString(itemCode.ToString());
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfFileDocument.OnCopyItemCode");
		}
	}

	[Command]
	public void OnMovingCurrentRowUpAndDownCommand(bool isUp)
	{
		try
		{
			if (File == null)
			{
				return;
			}
			int offset = Editor.TextArea.Caret.Offset;
			DocumentLine lineByOffset = Document.GetLineByOffset(offset);
			if (lineByOffset == null)
			{
				return;
			}
			if (isUp)
			{
				if (lineByOffset.PreviousLine != null)
				{
					string text = Document.GetText(lineByOffset);
					string text2 = Document.GetText(lineByOffset.PreviousLine);
					Document.Replace(lineByOffset, text2);
					Document.Replace(lineByOffset.PreviousLine, text);
					Editor.TextArea.Caret.Offset = lineByOffset.PreviousLine.Offset + lineByOffset.PreviousLine.Length;
				}
			}
			else if (lineByOffset.NextLine != null)
			{
				string text3 = Document.GetText(lineByOffset);
				string text4 = Document.GetText(lineByOffset.NextLine);
				Document.Replace(lineByOffset, text4);
				Document.Replace(lineByOffset.NextLine, text3);
				Editor.TextArea.Caret.Offset = lineByOffset.NextLine.Offset + lineByOffset.NextLine.Length;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfFileDocument.OnMovingCurrentRowUpAndDownCommand");
		}
	}

	private void llF5vRk3y2(object P_0, DragEventArgs P_1)
	{
		string text = (string)P_1.Data.GetData(typeof(string));
		if (text != null && text == "TreeListDropGroup：15427586-86B6-5410-5D88-7F139C0C1E9E")
		{
			P_1.Handled = true;
			TreeListDropGroup.Instance.DropDocument(TreeListDropGroup.Instance.GetFilePaths(), Editor, this);
		}
	}

	public override void Dispose()
	{
		if (Document != null)
		{
			SearchPanel?.Dispose();
			if (Editor != null)
			{
				Editor.TextArea.SelectionChanged -= iPd5jDePU5;
				Editor.TextArea.Caret.PositionChanged -= j3Z5l8Bm4b;
				Editor.TextArea.MouseDoubleClick -= Q795T3kTJ8;
			}
			if (SearchPanel != null)
			{
			}
			SearchPanel = null;
			VerticalScrollBarHighlightedMagager?.Dispose();
			TextEditorPreviewViewModelBase?.Dispose();
			if (Editor != null)
			{
				Editor.Dispose2();
			}
			Document.Text = string.Empty;
			Document = null;
			base.Icon = null;
			Editor = null;
			File = null;
		}
	}

	[CompilerGenerated]
	private async Task? f1X5BRv7xh()
	{
		_003C_003Ec__DisplayClass51_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass51_0();
		CS_0024_003C_003E8__locals4.D2WejrvI73 = this;
		PvfGroup pvf = AppCore.ViewModelBase.PVF;
		if (FullPath == null)
		{
			AppCore.Logger.ErrorUploadDialog(new Exception("Document文件路径不能为Null"), "PvfFileDocument.RefDocumentText");
			return;
		}
		if (pvf == null)
		{
			AppCore.Logger.ErrorUploadDialog(new Exception("pvf已关闭"), "PvfFileDocument.RefDocumentText");
			return;
		}
		string qHPelojyTg = ((!(FullPath == "stringtable.bin")) ? pvf.GetFileText(File, NowEncoding) : (await AppCore.ViewModelBase.PVF.Strtable.GetDocumentText()));
		CS_0024_003C_003E8__locals4.QHPelojyTg = qHPelojyTg;
		if (FullPath == "stringtable.bin")
		{
			await (pvf.Strtable?.LoadQuote(pvf));
		}
		else if (FullPath.Contains("kor.str"))
		{
			await (pvf.Strview?.InitStringViewQuote(pvf, FullPath));
		}
		await ((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			CS_0024_003C_003E8__locals4.D2WejrvI73.Document.Text = CS_0024_003C_003E8__locals4.QHPelojyTg;
		}, Array.Empty<object>());
	}

	[CompilerGenerated]
	private ResultData<ImageSource> mdt5FFKA0b()
	{
		return AppCore.ViewModelBase.PVF.GetScriptIconSource(File);
	}
}
