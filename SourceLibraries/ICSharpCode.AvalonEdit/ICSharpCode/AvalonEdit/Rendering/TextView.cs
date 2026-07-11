using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering;

public class TextView : FrameworkElement, IScrollInfo, IWeakEventListener, ITextEditorComponent, IServiceProvider
{
	private sealed class LayerCollection : UIElementCollection
	{
		private readonly TextView textView;

		public LayerCollection(TextView textView)
			: base(textView, textView)
		{
			this.textView = textView;
		}

		public override void Clear()
		{
			base.Clear();
			textView.LayersChanged();
		}

		public override int Add(UIElement element)
		{
			int result = base.Add(element);
			textView.LayersChanged();
			return result;
		}

		public override void RemoveAt(int index)
		{
			base.RemoveAt(index);
			textView.LayersChanged();
		}

		public override void RemoveRange(int index, int count)
		{
			base.RemoveRange(index, count);
			textView.LayersChanged();
		}
	}

	private ColumnRulerRenderer columnRulerRenderer;

	private CurrentLineHighlightRenderer currentLineHighlighRenderer;

	public static readonly DependencyProperty DocumentProperty;

	private TextDocument document;

	private HeightTree heightTree;

	public static readonly DependencyProperty OptionsProperty;

	private readonly ObserveAddRemoveCollection<VisualLineElementGenerator> elementGenerators;

	private readonly ObserveAddRemoveCollection<IVisualLineTransformer> lineTransformers;

	private SingleCharacterElementGenerator singleCharacterElementGenerator;

	private LinkElementGenerator linkElementGenerator;

	private MailLinkElementGenerator mailLinkElementGenerator;

	internal readonly TextLayer textLayer;

	private readonly LayerCollection layers;

	private List<InlineObjectRun> inlineObjects = new List<InlineObjectRun>();

	private List<VisualLine> visualLinesWithOutstandingInlineObjects = new List<VisualLine>();

	public static readonly DependencyProperty NonPrintableCharacterBrushProperty;

	public static readonly DependencyProperty LinkTextForegroundBrushProperty;

	public static readonly DependencyProperty LinkTextBackgroundBrushProperty;

	public static readonly DependencyProperty LinkTextUnderlineProperty;

	private DispatcherOperation invalidateMeasureOperation;

	private List<VisualLine> allVisualLines = new List<VisualLine>();

	private ReadOnlyCollection<VisualLine> visibleVisualLines;

	private double clippedPixelsOnTop;

	private List<VisualLine> newVisualLines;

	private const double AdditionalHorizontalScrollAmount = 3.0;

	private Size lastAvailableSize;

	private bool inMeasure;

	private TextFormatter formatter;

	internal TextViewCachedElements cachedElements;

	private readonly ObserveAddRemoveCollection<IBackgroundRenderer> backgroundRenderers;

	private Size scrollExtent;

	private Vector scrollOffset;

	private Size scrollViewport;

	private bool canVerticallyScroll;

	private bool canHorizontallyScroll;

	private bool defaultTextMetricsValid;

	private double wideSpaceWidth;

	private double defaultLineHeight;

	private double defaultBaseline;

	[ThreadStatic]
	private static bool invalidCursor;

	private readonly ServiceContainer services = new ServiceContainer();

	public static readonly RoutedEvent PreviewMouseHoverEvent;

	public static readonly RoutedEvent MouseHoverEvent;

	public static readonly RoutedEvent PreviewMouseHoverStoppedEvent;

	public static readonly RoutedEvent MouseHoverStoppedEvent;

	private MouseHoverLogic hoverLogic;

	public static readonly DependencyProperty ColumnRulerPenProperty;

	public static readonly DependencyProperty CurrentLineBackgroundProperty;

	public static readonly DependencyProperty CurrentLineBorderProperty;

	public TextDocument Document
	{
		get
		{
			return (TextDocument)GetValue(DocumentProperty);
		}
		set
		{
			SetValue(DocumentProperty, value);
		}
	}

	internal double FontSize => (double)GetValue(TextBlock.FontSizeProperty);

	public TextEditorOptions Options
	{
		get
		{
			return (TextEditorOptions)GetValue(OptionsProperty);
		}
		set
		{
			SetValue(OptionsProperty, value);
		}
	}

	public IList<VisualLineElementGenerator> ElementGenerators => elementGenerators;

	public IList<IVisualLineTransformer> LineTransformers => lineTransformers;

	public UIElementCollection Layers => layers;

	protected override int VisualChildrenCount => layers.Count + inlineObjects.Count;

	protected override IEnumerator LogicalChildren => inlineObjects.Select((InlineObjectRun io) => io.Element).Concat(layers.Cast<UIElement>()).GetEnumerator();

	public Brush NonPrintableCharacterBrush
	{
		get
		{
			return (Brush)GetValue(NonPrintableCharacterBrushProperty);
		}
		set
		{
			SetValue(NonPrintableCharacterBrushProperty, value);
		}
	}

	public Brush LinkTextForegroundBrush
	{
		get
		{
			return (Brush)GetValue(LinkTextForegroundBrushProperty);
		}
		set
		{
			SetValue(LinkTextForegroundBrushProperty, value);
		}
	}

	public Brush LinkTextBackgroundBrush
	{
		get
		{
			return (Brush)GetValue(LinkTextBackgroundBrushProperty);
		}
		set
		{
			SetValue(LinkTextBackgroundBrushProperty, value);
		}
	}

	public bool LinkTextUnderline
	{
		get
		{
			return (bool)GetValue(LinkTextUnderlineProperty);
		}
		set
		{
			SetValue(LinkTextUnderlineProperty, value);
		}
	}

	public ReadOnlyCollection<VisualLine> VisualLines
	{
		get
		{
			if (visibleVisualLines == null)
			{
				throw new VisualLinesInvalidException();
			}
			return visibleVisualLines;
		}
	}

	public bool VisualLinesValid => visibleVisualLines != null;

	public IList<IBackgroundRenderer> BackgroundRenderers => backgroundRenderers;

	bool IScrollInfo.CanVerticallyScroll
	{
		get
		{
			return canVerticallyScroll;
		}
		set
		{
			if (canVerticallyScroll != value)
			{
				canVerticallyScroll = value;
				InvalidateMeasure(DispatcherPriority.Normal);
			}
		}
	}

	bool IScrollInfo.CanHorizontallyScroll
	{
		get
		{
			return canHorizontallyScroll;
		}
		set
		{
			if (canHorizontallyScroll != value)
			{
				canHorizontallyScroll = value;
				ClearVisualLines();
				InvalidateMeasure(DispatcherPriority.Normal);
			}
		}
	}

	double IScrollInfo.ExtentWidth => scrollExtent.Width;

	double IScrollInfo.ExtentHeight => scrollExtent.Height;

	double IScrollInfo.ViewportWidth => scrollViewport.Width;

	double IScrollInfo.ViewportHeight => scrollViewport.Height;

	public double HorizontalOffset => scrollOffset.X;

	public double VerticalOffset => scrollOffset.Y;

	public Vector ScrollOffset => scrollOffset;

	ScrollViewer IScrollInfo.ScrollOwner { get; set; }

	public double WideSpaceWidth
	{
		get
		{
			CalculateDefaultTextMetrics();
			return wideSpaceWidth;
		}
	}

	public double DefaultLineHeight
	{
		get
		{
			CalculateDefaultTextMetrics();
			return defaultLineHeight;
		}
	}

	public double DefaultBaseline
	{
		get
		{
			CalculateDefaultTextMetrics();
			return defaultBaseline;
		}
	}

	public ServiceContainer Services => services;

	public double DocumentHeight
	{
		get
		{
			if (heightTree == null)
			{
				return 0.0;
			}
			return heightTree.TotalHeight;
		}
	}

	public Pen ColumnRulerPen
	{
		get
		{
			return (Pen)GetValue(ColumnRulerPenProperty);
		}
		set
		{
			SetValue(ColumnRulerPenProperty, value);
		}
	}

	public Brush CurrentLineBackground
	{
		get
		{
			return (Brush)GetValue(CurrentLineBackgroundProperty);
		}
		set
		{
			SetValue(CurrentLineBackgroundProperty, value);
		}
	}

	public Pen CurrentLineBorder
	{
		get
		{
			return (Pen)GetValue(CurrentLineBorderProperty);
		}
		set
		{
			SetValue(CurrentLineBorderProperty, value);
		}
	}

	public int HighlightedLine
	{
		get
		{
			return currentLineHighlighRenderer.Line;
		}
		set
		{
			currentLineHighlighRenderer.Line = value;
		}
	}

	public virtual double EmptyLineSelectionWidth => 1.0;

	public event EventHandler DocumentChanged;

	public event PropertyChangedEventHandler OptionChanged;

	public event EventHandler<VisualLineConstructionStartEventArgs> VisualLineConstructionStarting;

	public event EventHandler VisualLinesChanged;

	public event EventHandler ScrollOffsetChanged;

	public event MouseEventHandler PreviewMouseHover
	{
		add
		{
			AddHandler(PreviewMouseHoverEvent, value);
		}
		remove
		{
			RemoveHandler(PreviewMouseHoverEvent, value);
		}
	}

	public event MouseEventHandler MouseHover
	{
		add
		{
			AddHandler(MouseHoverEvent, value);
		}
		remove
		{
			RemoveHandler(MouseHoverEvent, value);
		}
	}

	public event MouseEventHandler PreviewMouseHoverStopped
	{
		add
		{
			AddHandler(PreviewMouseHoverStoppedEvent, value);
		}
		remove
		{
			RemoveHandler(PreviewMouseHoverStoppedEvent, value);
		}
	}

	public event MouseEventHandler MouseHoverStopped
	{
		add
		{
			AddHandler(MouseHoverStoppedEvent, value);
		}
		remove
		{
			RemoveHandler(MouseHoverStoppedEvent, value);
		}
	}

	static TextView()
	{
		DocumentProperty = DependencyProperty.Register("Document", typeof(TextDocument), typeof(TextView), new FrameworkPropertyMetadata(OnDocumentChanged));
		OptionsProperty = DependencyProperty.Register("Options", typeof(TextEditorOptions), typeof(TextView), new FrameworkPropertyMetadata(OnOptionsChanged));
		NonPrintableCharacterBrushProperty = DependencyProperty.Register("NonPrintableCharacterBrush", typeof(Brush), typeof(TextView), new FrameworkPropertyMetadata(Brushes.LightGray));
		LinkTextForegroundBrushProperty = DependencyProperty.Register("LinkTextForegroundBrush", typeof(Brush), typeof(TextView), new FrameworkPropertyMetadata(Brushes.Blue));
		LinkTextBackgroundBrushProperty = DependencyProperty.Register("LinkTextBackgroundBrush", typeof(Brush), typeof(TextView), new FrameworkPropertyMetadata(Brushes.Transparent));
		LinkTextUnderlineProperty = DependencyProperty.Register("LinkTextUnderline", typeof(bool), typeof(TextView), new FrameworkPropertyMetadata(true));
		PreviewMouseHoverEvent = EventManager.RegisterRoutedEvent("PreviewMouseHover", RoutingStrategy.Tunnel, typeof(MouseEventHandler), typeof(TextView));
		MouseHoverEvent = EventManager.RegisterRoutedEvent("MouseHover", RoutingStrategy.Bubble, typeof(MouseEventHandler), typeof(TextView));
		PreviewMouseHoverStoppedEvent = EventManager.RegisterRoutedEvent("PreviewMouseHoverStopped", RoutingStrategy.Tunnel, typeof(MouseEventHandler), typeof(TextView));
		MouseHoverStoppedEvent = EventManager.RegisterRoutedEvent("MouseHoverStopped", RoutingStrategy.Bubble, typeof(MouseEventHandler), typeof(TextView));
		ColumnRulerPenProperty = DependencyProperty.Register("ColumnRulerPen", typeof(Pen), typeof(TextView), new FrameworkPropertyMetadata(CreateFrozenPen(Brushes.LightGray)));
		CurrentLineBackgroundProperty = DependencyProperty.Register("CurrentLineBackground", typeof(Brush), typeof(TextView));
		CurrentLineBorderProperty = DependencyProperty.Register("CurrentLineBorder", typeof(Pen), typeof(TextView));
		UIElement.ClipToBoundsProperty.OverrideMetadata(typeof(TextView), new FrameworkPropertyMetadata(Boxes.True));
		UIElement.FocusableProperty.OverrideMetadata(typeof(TextView), new FrameworkPropertyMetadata(Boxes.False));
	}

	public TextView()
	{
		services.AddService(typeof(TextView), this);
		textLayer = new TextLayer(this);
		elementGenerators = new ObserveAddRemoveCollection<VisualLineElementGenerator>(ElementGenerator_Added, ElementGenerator_Removed);
		lineTransformers = new ObserveAddRemoveCollection<IVisualLineTransformer>(LineTransformer_Added, LineTransformer_Removed);
		backgroundRenderers = new ObserveAddRemoveCollection<IBackgroundRenderer>(BackgroundRenderer_Added, BackgroundRenderer_Removed);
		columnRulerRenderer = new ColumnRulerRenderer(this);
		currentLineHighlighRenderer = new CurrentLineHighlightRenderer(this);
		Options = new TextEditorOptions();
		layers = new LayerCollection(this);
		InsertLayer(textLayer, KnownLayer.Text, LayerInsertionPosition.Replace);
		hoverLogic = new MouseHoverLogic(this);
		hoverLogic.MouseHover += delegate(object? sender, MouseEventArgs e)
		{
			RaiseHoverEventPair(e, PreviewMouseHoverEvent, MouseHoverEvent);
		};
		hoverLogic.MouseHoverStopped += delegate(object? sender, MouseEventArgs e)
		{
			RaiseHoverEventPair(e, PreviewMouseHoverStoppedEvent, MouseHoverStoppedEvent);
		};
	}

	private static void OnDocumentChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
	{
		((TextView)dp).OnDocumentChanged((TextDocument)e.OldValue, (TextDocument)e.NewValue);
	}

	private void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
	{
		if (oldValue != null)
		{
			heightTree.Dispose();
			heightTree = null;
			formatter.Dispose();
			formatter = null;
			cachedElements.Dispose();
			cachedElements = null;
			WeakEventManagerBase<TextDocumentWeakEventManager.Changing, TextDocument>.RemoveListener(oldValue, this);
		}
		document = newValue;
		ClearScrollData();
		ClearVisualLines();
		if (newValue != null)
		{
			WeakEventManagerBase<TextDocumentWeakEventManager.Changing, TextDocument>.AddListener(newValue, this);
			formatter = TextFormatterFactory.Create(this);
			InvalidateDefaultTextMetrics();
			heightTree = new HeightTree(newValue, DefaultLineHeight);
			cachedElements = new TextViewCachedElements();
		}
		InvalidateMeasure(DispatcherPriority.Normal);
		if (this.DocumentChanged != null)
		{
			this.DocumentChanged(this, EventArgs.Empty);
		}
	}

	private void RecreateTextFormatter()
	{
		if (formatter != null)
		{
			formatter.Dispose();
			formatter = TextFormatterFactory.Create(this);
			Redraw();
		}
	}

	private void RecreateCachedElements()
	{
		if (cachedElements != null)
		{
			cachedElements.Dispose();
			cachedElements = new TextViewCachedElements();
		}
	}

	protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		if (managerType == typeof(TextDocumentWeakEventManager.Changing))
		{
			DocumentChangeEventArgs e2 = (DocumentChangeEventArgs)e;
			Redraw(e2.Offset, e2.RemovalLength);
			return true;
		}
		if (managerType == typeof(PropertyChangedWeakEventManager))
		{
			OnOptionChanged((PropertyChangedEventArgs)e);
			return true;
		}
		return false;
	}

	bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		return ReceiveWeakEvent(managerType, sender, e);
	}

	protected virtual void OnOptionChanged(PropertyChangedEventArgs e)
	{
		if (this.OptionChanged != null)
		{
			this.OptionChanged(this, e);
		}
		if (Options.ShowColumnRuler)
		{
			columnRulerRenderer.SetRuler(Options.ColumnRulerPosition, ColumnRulerPen);
		}
		else
		{
			columnRulerRenderer.SetRuler(-1, ColumnRulerPen);
		}
		UpdateBuiltinElementGeneratorsFromOptions();
		Redraw();
	}

	private static void OnOptionsChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
	{
		((TextView)dp).OnOptionsChanged((TextEditorOptions)e.OldValue, (TextEditorOptions)e.NewValue);
	}

	private void OnOptionsChanged(TextEditorOptions oldValue, TextEditorOptions newValue)
	{
		if (oldValue != null)
		{
			WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>.RemoveListener(oldValue, this);
		}
		if (newValue != null)
		{
			WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>.AddListener(newValue, this);
		}
		OnOptionChanged(new PropertyChangedEventArgs(null));
	}

	private void ElementGenerator_Added(VisualLineElementGenerator generator)
	{
		ConnectToTextView(generator);
		Redraw();
	}

	private void ElementGenerator_Removed(VisualLineElementGenerator generator)
	{
		DisconnectFromTextView(generator);
		Redraw();
	}

	private void LineTransformer_Added(IVisualLineTransformer lineTransformer)
	{
		ConnectToTextView(lineTransformer);
		Redraw();
	}

	private void LineTransformer_Removed(IVisualLineTransformer lineTransformer)
	{
		DisconnectFromTextView(lineTransformer);
		Redraw();
	}

	private void UpdateBuiltinElementGeneratorsFromOptions()
	{
		TextEditorOptions options = Options;
		AddRemoveDefaultElementGeneratorOnDemand(ref singleCharacterElementGenerator, options.ShowBoxForControlCharacters || options.ShowSpaces || options.ShowTabs);
		AddRemoveDefaultElementGeneratorOnDemand(ref linkElementGenerator, options.EnableHyperlinks);
		AddRemoveDefaultElementGeneratorOnDemand(ref mailLinkElementGenerator, options.EnableEmailHyperlinks);
	}

	private void AddRemoveDefaultElementGeneratorOnDemand<T>(ref T generator, bool demand) where T : VisualLineElementGenerator, IBuiltinElementGenerator, new()
	{
		if (generator != null != demand)
		{
			if (demand)
			{
				generator = new T();
				ElementGenerators.Add(generator);
			}
			else
			{
				ElementGenerators.Remove(generator);
				generator = null;
			}
		}
		if (generator != null)
		{
			generator.FetchOptions(Options);
		}
	}

	private void LayersChanged()
	{
		textLayer.index = layers.IndexOf(textLayer);
	}

	public void InsertLayer(UIElement layer, KnownLayer referencedLayer, LayerInsertionPosition position)
	{
		if (layer == null)
		{
			throw new ArgumentNullException("layer");
		}
		if (!Enum.IsDefined(typeof(KnownLayer), referencedLayer))
		{
			throw new InvalidEnumArgumentException("referencedLayer", (int)referencedLayer, typeof(KnownLayer));
		}
		if (!Enum.IsDefined(typeof(LayerInsertionPosition), position))
		{
			throw new InvalidEnumArgumentException("position", (int)position, typeof(LayerInsertionPosition));
		}
		if (referencedLayer == KnownLayer.Background && position != LayerInsertionPosition.Above)
		{
			throw new InvalidOperationException("Cannot replace or insert below the background layer.");
		}
		LayerPosition value = new LayerPosition(referencedLayer, position);
		LayerPosition.SetLayerPosition(layer, value);
		for (int i = 0; i < layers.Count; i++)
		{
			LayerPosition layerPosition = LayerPosition.GetLayerPosition(layers[i]);
			if (layerPosition == null)
			{
				continue;
			}
			if (layerPosition.KnownLayer == referencedLayer && layerPosition.Position == LayerInsertionPosition.Replace)
			{
				switch (position)
				{
				case LayerInsertionPosition.Below:
					layers.Insert(i, layer);
					return;
				case LayerInsertionPosition.Above:
					layers.Insert(i + 1, layer);
					return;
				case LayerInsertionPosition.Replace:
					layers[i] = layer;
					return;
				}
			}
			else if ((layerPosition.KnownLayer == referencedLayer && layerPosition.Position == LayerInsertionPosition.Above) || layerPosition.KnownLayer > referencedLayer)
			{
				layers.Insert(i, layer);
				return;
			}
		}
		layers.Add(layer);
	}

	protected override Visual GetVisualChild(int index)
	{
		int num = textLayer.index + 1;
		if (index < num)
		{
			return layers[index];
		}
		if (index < num + inlineObjects.Count)
		{
			return inlineObjects[index - num].Element;
		}
		return layers[index - inlineObjects.Count];
	}

	internal void AddInlineObject(InlineObjectRun inlineObject)
	{
		bool flag = false;
		for (int i = 0; i < inlineObjects.Count; i++)
		{
			if (inlineObjects[i].Element == inlineObject.Element)
			{
				RemoveInlineObjectRun(inlineObjects[i], keepElement: true);
				inlineObjects.RemoveAt(i);
				flag = true;
				break;
			}
		}
		inlineObjects.Add(inlineObject);
		if (!flag)
		{
			AddVisualChild(inlineObject.Element);
		}
		inlineObject.Element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
		inlineObject.desiredSize = inlineObject.Element.DesiredSize;
	}

	private void MeasureInlineObjects()
	{
		foreach (InlineObjectRun inlineObject in inlineObjects)
		{
			if (inlineObject.VisualLine.IsDisposed)
			{
				continue;
			}
			inlineObject.Element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			if (!inlineObject.Element.DesiredSize.IsClose(inlineObject.desiredSize))
			{
				inlineObject.desiredSize = inlineObject.Element.DesiredSize;
				if (allVisualLines.Remove(inlineObject.VisualLine))
				{
					DisposeVisualLine(inlineObject.VisualLine);
				}
			}
		}
	}

	private void RemoveInlineObjects(VisualLine visualLine)
	{
		if (visualLine.hasInlineObjects)
		{
			visualLinesWithOutstandingInlineObjects.Add(visualLine);
		}
	}

	private void RemoveInlineObjectsNow()
	{
		if (visualLinesWithOutstandingInlineObjects.Count == 0)
		{
			return;
		}
		inlineObjects.RemoveAll(delegate(InlineObjectRun ior)
		{
			if (visualLinesWithOutstandingInlineObjects.Contains(ior.VisualLine))
			{
				RemoveInlineObjectRun(ior, keepElement: false);
				return true;
			}
			return false;
		});
		visualLinesWithOutstandingInlineObjects.Clear();
	}

	private void RemoveInlineObjectRun(InlineObjectRun ior, bool keepElement)
	{
		if (!keepElement && ior.Element.IsKeyboardFocusWithin)
		{
			UIElement uIElement = this;
			while (uIElement != null && !uIElement.Focusable)
			{
				uIElement = VisualTreeHelper.GetParent(uIElement) as UIElement;
			}
			if (uIElement != null)
			{
				Keyboard.Focus(uIElement);
			}
		}
		ior.VisualLine = null;
		if (!keepElement)
		{
			RemoveVisualChild(ior.Element);
		}
	}

	public void Redraw()
	{
		Redraw(DispatcherPriority.Normal);
	}

	public void Redraw(DispatcherPriority redrawPriority)
	{
		VerifyAccess();
		ClearVisualLines();
		InvalidateMeasure(redrawPriority);
	}

	public void Redraw(VisualLine visualLine, DispatcherPriority redrawPriority = DispatcherPriority.Normal)
	{
		VerifyAccess();
		if (allVisualLines.Remove(visualLine))
		{
			DisposeVisualLine(visualLine);
			InvalidateMeasure(redrawPriority);
		}
	}

	public void Redraw(int offset, int length, DispatcherPriority redrawPriority = DispatcherPriority.Normal)
	{
		VerifyAccess();
		bool flag = false;
		for (int i = 0; i < allVisualLines.Count; i++)
		{
			VisualLine visualLine = allVisualLines[i];
			int offset2 = visualLine.FirstDocumentLine.Offset;
			int num = visualLine.LastDocumentLine.Offset + visualLine.LastDocumentLine.TotalLength;
			if (offset <= num)
			{
				flag = true;
				if (offset + length >= offset2)
				{
					allVisualLines.RemoveAt(i--);
					DisposeVisualLine(visualLine);
				}
			}
		}
		if (flag)
		{
			InvalidateMeasure(redrawPriority);
		}
	}

	public void InvalidateLayer(KnownLayer knownLayer)
	{
		InvalidateMeasure(DispatcherPriority.Normal);
	}

	public void InvalidateLayer(KnownLayer knownLayer, DispatcherPriority priority)
	{
		InvalidateMeasure(priority);
	}

	public void Redraw(ISegment segment, DispatcherPriority redrawPriority = DispatcherPriority.Normal)
	{
		if (segment != null)
		{
			Redraw(segment.Offset, segment.Length, redrawPriority);
		}
	}

	private void ClearVisualLines()
	{
		visibleVisualLines = null;
		if (allVisualLines.Count == 0)
		{
			return;
		}
		foreach (VisualLine allVisualLine in allVisualLines)
		{
			DisposeVisualLine(allVisualLine);
		}
		allVisualLines.Clear();
	}

	private void DisposeVisualLine(VisualLine visualLine)
	{
		if (newVisualLines != null && newVisualLines.Contains(visualLine))
		{
			throw new ArgumentException("Cannot dispose visual line because it is in construction!");
		}
		visibleVisualLines = null;
		visualLine.Dispose();
		RemoveInlineObjects(visualLine);
	}

	private void InvalidateMeasure(DispatcherPriority priority)
	{
		if (priority >= DispatcherPriority.Render)
		{
			if (invalidateMeasureOperation != null)
			{
				invalidateMeasureOperation.Abort();
				invalidateMeasureOperation = null;
			}
			InvalidateMeasure();
		}
		else if (invalidateMeasureOperation != null)
		{
			invalidateMeasureOperation.Priority = priority;
		}
		else
		{
			invalidateMeasureOperation = base.Dispatcher.BeginInvoke(priority, (Action)delegate
			{
				invalidateMeasureOperation = null;
				InvalidateMeasure();
			});
		}
	}

	public VisualLine GetVisualLine(int documentLineNumber)
	{
		foreach (VisualLine allVisualLine in allVisualLines)
		{
			int lineNumber = allVisualLine.FirstDocumentLine.LineNumber;
			int lineNumber2 = allVisualLine.LastDocumentLine.LineNumber;
			if (documentLineNumber >= lineNumber && documentLineNumber <= lineNumber2)
			{
				return allVisualLine;
			}
		}
		return null;
	}

	public VisualLine GetOrConstructVisualLine(DocumentLine documentLine)
	{
		if (documentLine == null)
		{
			throw new ArgumentNullException("documentLine");
		}
		if (!Document.Lines.Contains(documentLine))
		{
			throw new InvalidOperationException("Line belongs to wrong document");
		}
		VerifyAccess();
		VisualLine visualLine = GetVisualLine(documentLine.LineNumber);
		if (visualLine == null)
		{
			TextRunProperties textRunProperties = CreateGlobalTextRunProperties();
			VisualLineTextParagraphProperties paragraphProperties = CreateParagraphProperties(textRunProperties);
			while (heightTree.GetIsCollapsed(documentLine.LineNumber))
			{
				documentLine = documentLine.PreviousLine;
			}
			visualLine = BuildVisualLine(documentLine, textRunProperties, paragraphProperties, elementGenerators.ToArray(), lineTransformers.ToArray(), lastAvailableSize);
			allVisualLines.Add(visualLine);
			foreach (VisualLine allVisualLine in allVisualLines)
			{
				allVisualLine.VisualTop = heightTree.GetVisualPosition(allVisualLine.FirstDocumentLine);
			}
		}
		return visualLine;
	}

	public void EnsureVisualLines()
	{
		base.Dispatcher.VerifyAccess();
		if (inMeasure)
		{
			throw new InvalidOperationException("The visual line build process is already running! Cannot EnsureVisualLines() during Measure!");
		}
		if (!VisualLinesValid)
		{
			InvalidateMeasure(DispatcherPriority.Normal);
			UpdateLayout();
		}
		if (!VisualLinesValid)
		{
			MeasureOverride(lastAvailableSize);
		}
		if (!VisualLinesValid)
		{
			throw new VisualLinesInvalidException("Internal error: visual lines invalid after EnsureVisualLines call");
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (availableSize.Width > 32000.0)
		{
			availableSize.Width = 32000.0;
		}
		if (!canHorizontallyScroll && !availableSize.Width.IsClose(lastAvailableSize.Width))
		{
			ClearVisualLines();
		}
		lastAvailableSize = availableSize;
		foreach (UIElement layer in layers)
		{
			layer.Measure(availableSize);
		}
		MeasureInlineObjects();
		InvalidateVisual();
		double num;
		if (document == null)
		{
			allVisualLines = new List<VisualLine>();
			visibleVisualLines = allVisualLines.AsReadOnly();
			num = 0.0;
		}
		else
		{
			inMeasure = true;
			try
			{
				num = CreateAndMeasureVisualLines(availableSize);
			}
			finally
			{
				inMeasure = false;
			}
		}
		RemoveInlineObjectsNow();
		num += 3.0;
		double num2 = DocumentHeight;
		if (Options.AllowScrollBelowDocument && !double.IsInfinity(scrollViewport.Height))
		{
			double num3 = Math.Max(DefaultLineHeight, 30.0);
			double val = Math.Min(num2 - num3, scrollOffset.Y) + scrollViewport.Height;
			num2 = Math.Max(num2, val);
		}
		textLayer.SetVisualLines(visibleVisualLines);
		SetScrollData(availableSize, new Size(num, num2), scrollOffset);
		if (this.VisualLinesChanged != null)
		{
			this.VisualLinesChanged(this, EventArgs.Empty);
		}
		return new Size(Math.Min(availableSize.Width, num), Math.Min(availableSize.Height, num2));
	}

	private double CreateAndMeasureVisualLines(Size availableSize)
	{
		TextRunProperties textRunProperties = CreateGlobalTextRunProperties();
		VisualLineTextParagraphProperties paragraphProperties = CreateParagraphProperties(textRunProperties);
		DocumentLine lineByVisualPosition = heightTree.GetLineByVisualPosition(scrollOffset.Y);
		clippedPixelsOnTop = scrollOffset.Y - heightTree.GetVisualPosition(lineByVisualPosition);
		newVisualLines = new List<VisualLine>();
		if (this.VisualLineConstructionStarting != null)
		{
			this.VisualLineConstructionStarting(this, new VisualLineConstructionStartEventArgs(lineByVisualPosition));
		}
		VisualLineElementGenerator[] elementGeneratorsArray = elementGenerators.ToArray();
		IVisualLineTransformer[] lineTransformersArray = lineTransformers.ToArray();
		DocumentLine documentLine = lineByVisualPosition;
		double num = 0.0;
		double num2 = 0.0 - clippedPixelsOnTop;
		while (num2 < availableSize.Height && documentLine != null)
		{
			VisualLine visualLine = GetVisualLine(documentLine.LineNumber);
			if (visualLine == null)
			{
				visualLine = BuildVisualLine(documentLine, textRunProperties, paragraphProperties, elementGeneratorsArray, lineTransformersArray, availableSize);
			}
			visualLine.VisualTop = scrollOffset.Y + num2;
			documentLine = visualLine.LastDocumentLine.NextLine;
			num2 += visualLine.Height;
			foreach (TextLine textLine in visualLine.TextLines)
			{
				if (textLine.WidthIncludingTrailingWhitespace > num)
				{
					num = textLine.WidthIncludingTrailingWhitespace;
				}
			}
			newVisualLines.Add(visualLine);
		}
		foreach (VisualLine allVisualLine in allVisualLines)
		{
			if (!newVisualLines.Contains(allVisualLine))
			{
				DisposeVisualLine(allVisualLine);
			}
		}
		allVisualLines = newVisualLines;
		visibleVisualLines = new ReadOnlyCollection<VisualLine>(newVisualLines.ToArray());
		newVisualLines = null;
		if (allVisualLines.Any((VisualLine line) => line.IsDisposed))
		{
			throw new InvalidOperationException("A visual line was disposed even though it is still in use.\nThis can happen when Redraw() is called during measure for lines that are already constructed.");
		}
		return num;
	}

	private TextRunProperties CreateGlobalTextRunProperties()
	{
		return new GlobalTextRunProperties
		{
			typeface = this.CreateTypeface(),
			fontRenderingEmSize = FontSize,
			foregroundBrush = (Brush)GetValue(Control.ForegroundProperty),
			cultureInfo = CultureInfo.CurrentCulture
		};
	}

	private VisualLineTextParagraphProperties CreateParagraphProperties(TextRunProperties defaultTextRunProperties)
	{
		return new VisualLineTextParagraphProperties
		{
			defaultTextRunProperties = defaultTextRunProperties,
			textWrapping = (canHorizontallyScroll ? TextWrapping.NoWrap : TextWrapping.Wrap),
			tabSize = (double)Options.IndentationSize * WideSpaceWidth,
			flowDirection = base.FlowDirection
		};
	}

	public bool GetLineIsCollapsed(int lineNumber)
	{
		return heightTree.GetIsCollapsed(lineNumber);
	}

	private VisualLine BuildVisualLine(DocumentLine documentLine, TextRunProperties globalTextRunProperties, VisualLineTextParagraphProperties paragraphProperties, VisualLineElementGenerator[] elementGeneratorsArray, IVisualLineTransformer[] lineTransformersArray, Size availableSize)
	{
		if (heightTree.GetIsCollapsed(documentLine.LineNumber))
		{
			throw new InvalidOperationException("Trying to build visual line from collapsed line");
		}
		VisualLine visualLine = new VisualLine(this, documentLine);
		VisualLineTextSource visualLineTextSource = new VisualLineTextSource(visualLine)
		{
			Document = document,
			GlobalTextRunProperties = globalTextRunProperties,
			TextView = this
		};
		visualLine.ConstructVisualElements(visualLineTextSource, elementGeneratorsArray);
		if (visualLine.FirstDocumentLine != visualLine.LastDocumentLine)
		{
			double visualPosition = heightTree.GetVisualPosition(visualLine.FirstDocumentLine.NextLine);
			double visualPosition2 = heightTree.GetVisualPosition(visualLine.LastDocumentLine.NextLine ?? visualLine.LastDocumentLine);
			if (!visualPosition.IsClose(visualPosition2))
			{
				for (int i = visualLine.FirstDocumentLine.LineNumber + 1; i <= visualLine.LastDocumentLine.LineNumber; i++)
				{
					if (!heightTree.GetIsCollapsed(i))
					{
						heightTree.CollapseText(visualLine.FirstDocumentLine, visualLine.LastDocumentLine);
						throw new InvalidOperationException("Line " + i + " was skipped by a VisualLineElementGenerator, but it is not collapsed.");
					}
				}
				throw new InvalidOperationException("All lines collapsed but visual pos different - height tree inconsistency?");
			}
		}
		visualLine.RunTransformers(visualLineTextSource, lineTransformersArray);
		int num = 0;
		TextLineBreak previousLineBreak = null;
		List<TextLine> list = new List<TextLine>();
		paragraphProperties.indent = 0.0;
		paragraphProperties.firstLineInParagraph = true;
		while (num <= visualLine.VisualLengthWithEndOfLineMarker)
		{
			TextLine textLine = formatter.FormatLine(visualLineTextSource, num, availableSize.Width, paragraphProperties, previousLineBreak);
			list.Add(textLine);
			num += textLine.Length;
			if (num >= visualLine.VisualLengthWithEndOfLineMarker)
			{
				break;
			}
			if (paragraphProperties.firstLineInParagraph)
			{
				paragraphProperties.firstLineInParagraph = false;
				TextEditorOptions options = Options;
				double num2 = 0.0;
				if (options.InheritWordWrapIndentation)
				{
					int indentationVisualColumn = GetIndentationVisualColumn(visualLine);
					if (indentationVisualColumn > 0 && indentationVisualColumn < num)
					{
						num2 = textLine.GetDistanceFromCharacterHit(new CharacterHit(indentationVisualColumn, 0));
					}
				}
				num2 += options.WordWrapIndentation;
				if (num2 > 0.0 && num2 * 2.0 < availableSize.Width)
				{
					paragraphProperties.indent = num2;
				}
			}
			previousLineBreak = textLine.GetTextLineBreak();
		}
		visualLine.SetTextLines(list);
		heightTree.SetHeight(visualLine.FirstDocumentLine, visualLine.Height);
		return visualLine;
	}

	private static int GetIndentationVisualColumn(VisualLine visualLine)
	{
		if (visualLine.Elements.Count == 0)
		{
			return 0;
		}
		int num = 0;
		int num2 = 0;
		VisualLineElement visualLineElement = visualLine.Elements[num2];
		while (visualLineElement.IsWhitespace(num))
		{
			num++;
			if (num == visualLineElement.VisualColumn + visualLineElement.VisualLength)
			{
				num2++;
				if (num2 == visualLine.Elements.Count)
				{
					break;
				}
				visualLineElement = visualLine.Elements[num2];
			}
		}
		return num;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		EnsureVisualLines();
		foreach (UIElement layer in layers)
		{
			layer.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
		}
		if (document == null || allVisualLines.Count == 0)
		{
			return finalSize;
		}
		Vector offset = scrollOffset;
		if (scrollOffset.X + finalSize.Width > scrollExtent.Width)
		{
			offset.X = Math.Max(0.0, scrollExtent.Width - finalSize.Width);
		}
		if (scrollOffset.Y + finalSize.Height > scrollExtent.Height)
		{
			offset.Y = Math.Max(0.0, scrollExtent.Height - finalSize.Height);
		}
		if (SetScrollData(scrollViewport, scrollExtent, offset))
		{
			InvalidateMeasure(DispatcherPriority.Normal);
		}
		if (visibleVisualLines != null)
		{
			Point point = new Point(0.0 - scrollOffset.X, 0.0 - clippedPixelsOnTop);
			foreach (VisualLine visibleVisualLine in visibleVisualLines)
			{
				int num = 0;
				foreach (TextLine textLine in visibleVisualLine.TextLines)
				{
					foreach (TextSpan<TextRun> textRunSpan in textLine.GetTextRunSpans())
					{
						if (textRunSpan.Value is InlineObjectRun { VisualLine: not null } inlineObjectRun)
						{
							double distanceFromCharacterHit = textLine.GetDistanceFromCharacterHit(new CharacterHit(num, 0));
							inlineObjectRun.Element.Arrange(new Rect(new Point(point.X + distanceFromCharacterHit, point.Y), inlineObjectRun.Element.DesiredSize));
						}
						num += textRunSpan.Length;
					}
					point.Y += textLine.Height;
				}
			}
		}
		InvalidateCursorIfMouseWithinTextView();
		return finalSize;
	}

	private void BackgroundRenderer_Added(IBackgroundRenderer renderer)
	{
		ConnectToTextView(renderer);
		InvalidateLayer(renderer.Layer);
	}

	private void BackgroundRenderer_Removed(IBackgroundRenderer renderer)
	{
		DisconnectFromTextView(renderer);
		InvalidateLayer(renderer.Layer);
	}

	protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
	{
		base.OnRenderSizeChanged(sizeInfo);
		InvalidateLayer(KnownLayer.Selection);
	}

	protected override void OnRender(DrawingContext drawingContext)
	{
		RenderBackground(drawingContext, KnownLayer.Background);
		foreach (VisualLine visibleVisualLine in visibleVisualLines)
		{
			Brush brush = null;
			int num = 0;
			int num2 = 0;
			foreach (VisualLineElement element in visibleVisualLine.Elements)
			{
				if (brush == null || !brush.Equals(element.BackgroundBrush))
				{
					if (brush != null)
					{
						BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder();
						backgroundGeometryBuilder.AlignToWholePixels = true;
						backgroundGeometryBuilder.CornerRadius = 3.0;
						foreach (Rect item in BackgroundGeometryBuilder.GetRectsFromVisualSegment(this, visibleVisualLine, num, num + num2))
						{
							backgroundGeometryBuilder.AddRectangle(this, item);
						}
						Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
						if (geometry != null)
						{
							drawingContext.DrawGeometry(brush, null, geometry);
						}
					}
					num = element.VisualColumn;
					num2 = element.VisualLength;
					brush = element.BackgroundBrush;
				}
				else
				{
					num2 += element.VisualLength;
				}
			}
			if (brush == null)
			{
				continue;
			}
			BackgroundGeometryBuilder backgroundGeometryBuilder2 = new BackgroundGeometryBuilder();
			backgroundGeometryBuilder2.AlignToWholePixels = true;
			backgroundGeometryBuilder2.CornerRadius = 3.0;
			foreach (Rect item2 in BackgroundGeometryBuilder.GetRectsFromVisualSegment(this, visibleVisualLine, num, num + num2))
			{
				backgroundGeometryBuilder2.AddRectangle(this, item2);
			}
			Geometry geometry2 = backgroundGeometryBuilder2.CreateGeometry();
			if (geometry2 != null)
			{
				drawingContext.DrawGeometry(brush, null, geometry2);
			}
		}
	}

	internal void RenderBackground(DrawingContext drawingContext, KnownLayer layer)
	{
		foreach (IBackgroundRenderer backgroundRenderer in backgroundRenderers)
		{
			if (backgroundRenderer.Layer == layer)
			{
				backgroundRenderer.Draw(this, drawingContext);
			}
		}
	}

	internal void ArrangeTextLayer(IList<VisualLineDrawingVisual> visuals)
	{
		Point point = new Point(0.0 - scrollOffset.X, 0.0 - clippedPixelsOnTop);
		foreach (VisualLineDrawingVisual visual in visuals)
		{
			if (!(visual.Transform is TranslateTransform translateTransform) || translateTransform.X != point.X || translateTransform.Y != point.Y)
			{
				visual.Transform = new TranslateTransform(point.X, point.Y);
				visual.Transform.Freeze();
			}
			point.Y += visual.Height;
		}
	}

	private void ClearScrollData()
	{
		SetScrollData(default(Size), default(Size), default(Vector));
	}

	private bool SetScrollData(Size viewport, Size extent, Vector offset)
	{
		if (!viewport.IsClose(scrollViewport) || !extent.IsClose(scrollExtent) || !offset.IsClose(scrollOffset))
		{
			scrollViewport = viewport;
			scrollExtent = extent;
			SetScrollOffset(offset);
			OnScrollChange();
			return true;
		}
		return false;
	}

	private void OnScrollChange()
	{
		((IScrollInfo)this).ScrollOwner?.InvalidateScrollInfo();
	}

	private void SetScrollOffset(Vector vector)
	{
		if (!canHorizontallyScroll)
		{
			vector.X = 0.0;
		}
		if (!canVerticallyScroll)
		{
			vector.Y = 0.0;
		}
		if (!scrollOffset.IsClose(vector))
		{
			scrollOffset = vector;
			if (this.ScrollOffsetChanged != null)
			{
				this.ScrollOffsetChanged(this, EventArgs.Empty);
			}
		}
	}

	void IScrollInfo.LineUp()
	{
		((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y - DefaultLineHeight);
	}

	void IScrollInfo.LineDown()
	{
		((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y + DefaultLineHeight);
	}

	void IScrollInfo.LineLeft()
	{
		((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X - WideSpaceWidth);
	}

	void IScrollInfo.LineRight()
	{
		((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X + WideSpaceWidth);
	}

	void IScrollInfo.PageUp()
	{
		((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y - scrollViewport.Height);
	}

	void IScrollInfo.PageDown()
	{
		((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y + scrollViewport.Height);
	}

	void IScrollInfo.PageLeft()
	{
		((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X - scrollViewport.Width);
	}

	void IScrollInfo.PageRight()
	{
		((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X + scrollViewport.Width);
	}

	void IScrollInfo.MouseWheelUp()
	{
		((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y - (double)SystemParameters.WheelScrollLines * DefaultLineHeight);
		OnScrollChange();
	}

	void IScrollInfo.MouseWheelDown()
	{
		((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y + (double)SystemParameters.WheelScrollLines * DefaultLineHeight);
		OnScrollChange();
	}

	void IScrollInfo.MouseWheelLeft()
	{
		((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X - (double)SystemParameters.WheelScrollLines * WideSpaceWidth);
		OnScrollChange();
	}

	void IScrollInfo.MouseWheelRight()
	{
		((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X + (double)SystemParameters.WheelScrollLines * WideSpaceWidth);
		OnScrollChange();
	}

	private void InvalidateDefaultTextMetrics()
	{
		defaultTextMetricsValid = false;
		if (heightTree != null)
		{
			CalculateDefaultTextMetrics();
		}
	}

	private void CalculateDefaultTextMetrics()
	{
		if (defaultTextMetricsValid)
		{
			return;
		}
		defaultTextMetricsValid = true;
		if (formatter != null)
		{
			TextRunProperties textRunProperties = CreateGlobalTextRunProperties();
			using TextLine textLine = formatter.FormatLine(new SimpleTextSource("x", textRunProperties), 0, 32000.0, new VisualLineTextParagraphProperties
			{
				defaultTextRunProperties = textRunProperties,
				flowDirection = base.FlowDirection
			}, null);
			wideSpaceWidth = Math.Max(1.0, textLine.WidthIncludingTrailingWhitespace);
			defaultBaseline = Math.Max(1.0, textLine.Baseline);
			defaultLineHeight = Math.Max(1.0, textLine.Height);
		}
		else
		{
			wideSpaceWidth = FontSize / 2.0;
			defaultBaseline = FontSize;
			defaultLineHeight = FontSize + 3.0;
		}
		if (heightTree != null)
		{
			heightTree.DefaultLineHeight = defaultLineHeight;
		}
	}

	private static double ValidateVisualOffset(double offset)
	{
		if (double.IsNaN(offset))
		{
			throw new ArgumentException("offset must not be NaN");
		}
		if (offset < 0.0)
		{
			return 0.0;
		}
		return offset;
	}

	void IScrollInfo.SetHorizontalOffset(double offset)
	{
		offset = ValidateVisualOffset(offset);
		if (!scrollOffset.X.IsClose(offset))
		{
			SetScrollOffset(new Vector(offset, scrollOffset.Y));
			InvalidateVisual();
			textLayer.InvalidateVisual();
		}
	}

	void IScrollInfo.SetVerticalOffset(double offset)
	{
		offset = ValidateVisualOffset(offset);
		if (!scrollOffset.Y.IsClose(offset))
		{
			SetScrollOffset(new Vector(scrollOffset.X, offset));
			InvalidateMeasure(DispatcherPriority.Normal);
		}
	}

	Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle)
	{
		if (rectangle.IsEmpty || visual == null || visual == this || !IsAncestorOf(visual))
		{
			return Rect.Empty;
		}
		rectangle = visual.TransformToAncestor(this).TransformBounds(rectangle);
		MakeVisible(Rect.Offset(rectangle, scrollOffset));
		return rectangle;
	}

	public virtual void MakeVisible(Rect rectangle)
	{
		Rect rect = new Rect(scrollOffset.X, scrollOffset.Y, scrollViewport.Width, scrollViewport.Height);
		Vector d = scrollOffset;
		if (rectangle.Left < rect.Left)
		{
			if (rectangle.Right > rect.Right)
			{
				d.X = rectangle.Left + rectangle.Width / 2.0;
			}
			else
			{
				d.X = rectangle.Left;
			}
		}
		else if (rectangle.Right > rect.Right)
		{
			d.X = rectangle.Right - scrollViewport.Width;
		}
		if (rectangle.Top < rect.Top)
		{
			if (rectangle.Bottom > rect.Bottom)
			{
				d.Y = rectangle.Top + rectangle.Height / 2.0;
			}
			else
			{
				d.Y = rectangle.Top;
			}
		}
		else if (rectangle.Bottom > rect.Bottom)
		{
			d.Y = rectangle.Bottom - scrollViewport.Height;
		}
		d.X = ValidateVisualOffset(d.X);
		d.Y = ValidateVisualOffset(d.Y);
		if (!scrollOffset.IsClose(d))
		{
			SetScrollOffset(d);
			OnScrollChange();
			InvalidateMeasure(DispatcherPriority.Normal);
		}
	}

	protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
	{
		return new PointHitTestResult(this, hitTestParameters.HitPoint);
	}

	public static void InvalidateCursor()
	{
		if (!invalidCursor)
		{
			invalidCursor = true;
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (Action)delegate
			{
				invalidCursor = false;
				Mouse.UpdateCursor();
			});
		}
	}

	internal void InvalidateCursorIfMouseWithinTextView()
	{
		if (base.IsMouseOver)
		{
			InvalidateCursor();
		}
	}

	protected override void OnQueryCursor(QueryCursorEventArgs e)
	{
		GetVisualLineElementFromPosition(e.GetPosition(this) + scrollOffset)?.OnQueryCursor(e);
	}

	protected override void OnMouseDown(MouseButtonEventArgs e)
	{
		base.OnMouseDown(e);
		if (!e.Handled)
		{
			EnsureVisualLines();
			GetVisualLineElementFromPosition(e.GetPosition(this) + scrollOffset)?.OnMouseDown(e);
		}
	}

	protected override void OnMouseUp(MouseButtonEventArgs e)
	{
		base.OnMouseUp(e);
		if (!e.Handled)
		{
			EnsureVisualLines();
			GetVisualLineElementFromPosition(e.GetPosition(this) + scrollOffset)?.OnMouseUp(e);
		}
	}

	public virtual void PreviewUp(MouseButtonEventArgs e)
	{
		GetVisualLineElementFromPosition(e.GetPosition(this) + scrollOffset)?.OnPreviewUp(e);
	}

	public VisualLine GetVisualLineFromVisualTop(double visualTop)
	{
		EnsureVisualLines();
		foreach (VisualLine visualLine in VisualLines)
		{
			if (!(visualTop < visualLine.VisualTop) && visualTop < visualLine.VisualTop + visualLine.Height)
			{
				return visualLine;
			}
		}
		return null;
	}

	public double GetVisualTopByDocumentLine(int line)
	{
		VerifyAccess();
		if (heightTree == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		return heightTree.GetVisualPosition(heightTree.GetLineByNumber(line));
	}

	public VisualLineElement GetVisualLineElementFromPosition(Point visualPosition)
	{
		VisualLine visualLineFromVisualTop = GetVisualLineFromVisualTop(visualPosition.Y);
		if (visualLineFromVisualTop != null)
		{
			int visualColumnFloor = visualLineFromVisualTop.GetVisualColumnFloor(visualPosition);
			foreach (VisualLineElement element in visualLineFromVisualTop.Elements)
			{
				if (element.VisualColumn + element.VisualLength > visualColumnFloor)
				{
					return element;
				}
			}
		}
		return null;
	}

	public Point GetVisualPosition(TextViewPosition position, VisualYPosition yPositionMode)
	{
		VerifyAccess();
		if (Document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		DocumentLine lineByNumber = Document.GetLineByNumber(position.Line);
		VisualLine orConstructVisualLine = GetOrConstructVisualLine(lineByNumber);
		int visualColumn = position.VisualColumn;
		if (visualColumn < 0)
		{
			int num = lineByNumber.Offset + position.Column - 1;
			visualColumn = orConstructVisualLine.GetVisualColumn(num - orConstructVisualLine.FirstDocumentLine.Offset);
		}
		return orConstructVisualLine.GetVisualPosition(visualColumn, position.IsAtEndOfLine, yPositionMode);
	}

	public TextViewPosition? GetPosition(Point visualPosition)
	{
		VerifyAccess();
		if (Document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		return GetVisualLineFromVisualTop(visualPosition.Y)?.GetTextViewPosition(visualPosition, Options.EnableVirtualSpace);
	}

	public TextViewPosition? GetPositionFloor(Point visualPosition)
	{
		VerifyAccess();
		if (Document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		return GetVisualLineFromVisualTop(visualPosition.Y)?.GetTextViewPositionFloor(visualPosition, Options.EnableVirtualSpace);
	}

	public virtual object GetService(Type serviceType)
	{
		object service = services.GetService(serviceType);
		if (service == null && document != null)
		{
			service = document.ServiceProvider.GetService(serviceType);
		}
		return service;
	}

	private void ConnectToTextView(object obj)
	{
		if (obj is ITextViewConnect textViewConnect)
		{
			textViewConnect.AddToTextView(this);
		}
	}

	private void DisconnectFromTextView(object obj)
	{
		if (obj is ITextViewConnect textViewConnect)
		{
			textViewConnect.RemoveFromTextView(this);
		}
	}

	private void RaiseHoverEventPair(MouseEventArgs e, RoutedEvent tunnelingEvent, RoutedEvent bubblingEvent)
	{
		if (bubblingEvent.Name == "MouseHoverStopped")
		{
			GetVisualLineElementFromPosition(e.GetPosition(this) + scrollOffset)?.OnMouseHoverStopped(e);
		}
		MouseDevice mouseDevice = e.MouseDevice;
		StylusDevice stylusDevice = e.StylusDevice;
		int tickCount = Environment.TickCount;
		MouseEventArgs e2 = new MouseEventArgs(mouseDevice, tickCount, stylusDevice)
		{
			RoutedEvent = tunnelingEvent,
			Source = this
		};
		RaiseEvent(e2);
		MouseEventArgs e3 = new MouseEventArgs(mouseDevice, tickCount, stylusDevice)
		{
			RoutedEvent = bubblingEvent,
			Source = this,
			Handled = e2.Handled
		};
		RaiseEvent(e3);
	}

	public CollapsedLineSection CollapseLines(DocumentLine start, DocumentLine end)
	{
		VerifyAccess();
		if (heightTree == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		return heightTree.CollapseText(start, end);
	}

	public DocumentLine GetDocumentLineByVisualTop(double visualTop)
	{
		VerifyAccess();
		if (heightTree == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		return heightTree.GetLineByVisualPosition(visualTop);
	}

	protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		base.OnPropertyChanged(e);
		if (TextFormatterFactory.PropertyChangeAffectsTextFormatter(e.Property))
		{
			RecreateTextFormatter();
			RecreateCachedElements();
			InvalidateDefaultTextMetrics();
		}
		else if (e.Property == Control.ForegroundProperty || e.Property == NonPrintableCharacterBrushProperty || e.Property == LinkTextBackgroundBrushProperty || e.Property == LinkTextForegroundBrushProperty || e.Property == LinkTextUnderlineProperty)
		{
			RecreateCachedElements();
			Redraw();
		}
		if (e.Property == Control.FontFamilyProperty || e.Property == Control.FontSizeProperty || e.Property == Control.FontStretchProperty || e.Property == Control.FontStyleProperty || e.Property == Control.FontWeightProperty || e.Property == FrameworkElement.FlowDirectionProperty)
		{
			RecreateCachedElements();
			InvalidateDefaultTextMetrics();
			Redraw();
		}
		if (e.Property == ColumnRulerPenProperty)
		{
			columnRulerRenderer.SetRuler(Options.ColumnRulerPosition, ColumnRulerPen);
		}
		if (e.Property == CurrentLineBorderProperty)
		{
			currentLineHighlighRenderer.BorderPen = CurrentLineBorder;
		}
		if (e.Property == CurrentLineBackgroundProperty)
		{
			currentLineHighlighRenderer.BackgroundBrush = CurrentLineBackground;
		}
	}

	private static Pen CreateFrozenPen(SolidColorBrush brush)
	{
		Pen pen = new Pen(brush, 1.0);
		pen.Freeze();
		return pen;
	}
}
