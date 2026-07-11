using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Folding;

public class FoldingManager : IWeakEventListener
{
	private sealed class FoldingManagerInstallation : FoldingManager
	{
		private TextArea textArea;

		private FoldingMargin margin;

		private FoldingElementGenerator generator;

		public FoldingManagerInstallation(TextArea textArea)
			: base(textArea.Document)
		{
			this.textArea = textArea;
			margin = new FoldingMargin
			{
				FoldingManager = this
			};
			generator = new FoldingElementGenerator
			{
				FoldingManager = this
			};
			textArea.LeftMargins.Add(margin);
			textArea.TextView.Services.AddService(typeof(FoldingManager), this);
			textArea.TextView.ElementGenerators.Insert(0, generator);
			textArea.Caret.PositionChanged += textArea_Caret_PositionChanged;
		}

		public void Uninstall()
		{
			Clear();
			if (textArea != null)
			{
				textArea.Caret.PositionChanged -= textArea_Caret_PositionChanged;
				textArea.LeftMargins.Remove(margin);
				textArea.TextView.ElementGenerators.Remove(generator);
				textArea.TextView.Services.RemoveService(typeof(FoldingManager));
				margin = null;
				generator = null;
				textArea = null;
			}
		}

		private void textArea_Caret_PositionChanged(object sender, EventArgs e)
		{
			int offset = textArea.Caret.Offset;
			foreach (FoldingSection item in GetFoldingsContaining(offset))
			{
				if (item.IsFolded && item.StartOffset < offset && offset < item.EndOffset)
				{
					item.IsFolded = false;
				}
			}
		}
	}

	internal readonly TextDocument document;

	internal readonly List<TextView> textViews = new List<TextView>();

	private readonly TextSegmentCollection<FoldingSection> foldings;

	private bool isFirstUpdate = true;

	public IEnumerable<FoldingSection> AllFoldings => foldings;

	public FoldingManager(TextDocument document)
	{
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		this.document = document;
		foldings = new TextSegmentCollection<FoldingSection>();
		document.VerifyAccess();
		WeakEventManagerBase<TextDocumentWeakEventManager.Changed, TextDocument>.AddListener(document, this);
	}

	protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		if (managerType == typeof(TextDocumentWeakEventManager.Changed))
		{
			OnDocumentChanged((DocumentChangeEventArgs)e);
			return true;
		}
		return false;
	}

	bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		return ReceiveWeakEvent(managerType, sender, e);
	}

	private void OnDocumentChanged(DocumentChangeEventArgs e)
	{
		foldings.UpdateOffsets(e);
		int offset = e.Offset + e.InsertionLength;
		DocumentLine lineByOffset = document.GetLineByOffset(offset);
		offset = lineByOffset.Offset + lineByOffset.TotalLength;
		foreach (FoldingSection item in foldings.FindOverlappingSegments(e.Offset, offset - e.Offset))
		{
			if (item.Length == 0)
			{
				RemoveFolding(item);
			}
			else
			{
				item.ValidateCollapsedLineSections();
			}
		}
	}

	internal void AddToTextView(TextView textView)
	{
		if (textView == null || textViews.Contains(textView))
		{
			throw new ArgumentException();
		}
		textViews.Add(textView);
		foreach (FoldingSection folding in foldings)
		{
			if (folding.collapsedSections != null)
			{
				Array.Resize(ref folding.collapsedSections, textViews.Count);
				folding.ValidateCollapsedLineSections();
			}
		}
	}

	internal void RemoveFromTextView(TextView textView)
	{
		int num = textViews.IndexOf(textView);
		if (num < 0)
		{
			throw new ArgumentException();
		}
		textViews.RemoveAt(num);
		foreach (FoldingSection folding in foldings)
		{
			if (folding.collapsedSections != null)
			{
				CollapsedLineSection[] array = new CollapsedLineSection[textViews.Count];
				Array.Copy(folding.collapsedSections, 0, array, 0, num);
				folding.collapsedSections[num].Uncollapse();
				Array.Copy(folding.collapsedSections, num + 1, array, num, array.Length - num);
				folding.collapsedSections = array;
			}
		}
	}

	internal void Redraw()
	{
		foreach (TextView textView in textViews)
		{
			textView.Redraw();
		}
	}

	internal void Redraw(FoldingSection fs)
	{
		foreach (TextView textView in textViews)
		{
			textView.Redraw(fs);
		}
	}

	public FoldingSection CreateFolding(int startOffset, int endOffset)
	{
		if (startOffset >= endOffset)
		{
			throw new ArgumentException("startOffset must be less than endOffset");
		}
		if (startOffset < 0 || endOffset > document.TextLength)
		{
			throw new ArgumentException("Folding must be within document boundary");
		}
		FoldingSection foldingSection = new FoldingSection(this, startOffset, endOffset);
		foldings.Add(foldingSection);
		Redraw(foldingSection);
		return foldingSection;
	}

	public void RemoveFolding(FoldingSection fs)
	{
		if (fs == null)
		{
			throw new ArgumentNullException("fs");
		}
		fs.IsFolded = false;
		foldings.Remove(fs);
		Redraw(fs);
	}

	public void Clear()
	{
		document.VerifyAccess();
		foreach (FoldingSection folding in foldings)
		{
			folding.IsFolded = false;
		}
		foldings.Clear();
		Redraw();
	}

	public int GetNextFoldedFoldingStart(int startOffset)
	{
		FoldingSection foldingSection = foldings.FindFirstSegmentWithStartAfter(startOffset);
		while (foldingSection != null && !foldingSection.IsFolded)
		{
			foldingSection = foldings.GetNextSegment(foldingSection);
		}
		return foldingSection?.StartOffset ?? (-1);
	}

	public FoldingSection GetNextFolding(int startOffset)
	{
		return foldings.FindFirstSegmentWithStartAfter(startOffset);
	}

	public ReadOnlyCollection<FoldingSection> GetFoldingsAt(int startOffset)
	{
		List<FoldingSection> list = new List<FoldingSection>();
		FoldingSection foldingSection = foldings.FindFirstSegmentWithStartAfter(startOffset);
		while (foldingSection != null && foldingSection.StartOffset == startOffset)
		{
			list.Add(foldingSection);
			foldingSection = foldings.GetNextSegment(foldingSection);
		}
		return list.AsReadOnly();
	}

	public ReadOnlyCollection<FoldingSection> GetFoldingsContaining(int offset)
	{
		return foldings.FindSegmentsContaining(offset);
	}

	public void UpdateFoldings(IEnumerable<NewFolding> newFoldings, int firstErrorOffset)
	{
		if (newFoldings == null)
		{
			throw new ArgumentNullException("newFoldings");
		}
		if (firstErrorOffset < 0)
		{
			firstErrorOffset = int.MaxValue;
		}
		FoldingSection[] array = AllFoldings.ToArray();
		int num = 0;
		int num2 = 0;
		foreach (NewFolding newFolding in newFoldings)
		{
			if (newFolding.StartOffset < num2)
			{
				throw new ArgumentException("newFoldings must be sorted by start offset");
			}
			num2 = newFolding.StartOffset;
			newFolding.StartOffset.CoerceValue(0, document.TextLength);
			newFolding.EndOffset.CoerceValue(0, document.TextLength);
			if (newFolding.StartOffset == newFolding.EndOffset)
			{
				continue;
			}
			while (num < array.Length && newFolding.StartOffset > array[num].StartOffset)
			{
				RemoveFolding(array[num++]);
			}
			FoldingSection foldingSection;
			if (num < array.Length && newFolding.StartOffset == array[num].StartOffset)
			{
				foldingSection = array[num++];
				foldingSection.Length = newFolding.EndOffset - newFolding.StartOffset;
			}
			else
			{
				foldingSection = CreateFolding(newFolding.StartOffset, newFolding.EndOffset);
				if (isFirstUpdate)
				{
					foldingSection.IsFolded = newFolding.DefaultClosed;
				}
				foldingSection.Tag = newFolding;
			}
			foldingSection.Title = newFolding.Name;
		}
		isFirstUpdate = false;
		while (num < array.Length)
		{
			FoldingSection foldingSection2 = array[num++];
			if (foldingSection2.StartOffset < firstErrorOffset)
			{
				RemoveFolding(foldingSection2);
				continue;
			}
			break;
		}
	}

	public static FoldingManager Install(TextArea textArea)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		return new FoldingManagerInstallation(textArea);
	}

	public static void Uninstall(FoldingManager manager)
	{
		if (manager == null)
		{
			throw new ArgumentNullException("manager");
		}
		if (manager is FoldingManagerInstallation foldingManagerInstallation)
		{
			foldingManagerInstallation.Uninstall();
			return;
		}
		throw new ArgumentException("FoldingManager was not created using FoldingManager.Install");
	}
}
