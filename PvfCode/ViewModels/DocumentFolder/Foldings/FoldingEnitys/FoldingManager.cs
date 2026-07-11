using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys;

public class FoldingManager : IWeakEventListener
{
	private sealed class gZ93VZbtAdtOiDmmhBs : FoldingManager
	{
		private TextArea pFqbEkWHHD;

		private FoldingMargin OWVbO4IK1V;

		private FoldingElementGenerator mZ5bKcOiR1;

		public gZ93VZbtAdtOiDmmhBs(TextArea P_0)
			: base(P_0.Document)
		{
			pFqbEkWHHD = P_0;
			OWVbO4IK1V = new FoldingMargin
			{
				FoldingManager = this
			};
			mZ5bKcOiR1 = new FoldingElementGenerator
			{
				FoldingManager = this
			};
			P_0.LeftMargins.Add(OWVbO4IK1V);
			P_0.TextView.Services.AddService(typeof(FoldingManager), this);
			P_0.TextView.ElementGenerators.Insert(0, mZ5bKcOiR1);
			P_0.Caret.PositionChanged += Rk1bI9W4Bf;
		}

		public void HOubbbGiZZ()
		{
			Clear();
			if (pFqbEkWHHD != null)
			{
				pFqbEkWHHD.Caret.PositionChanged -= Rk1bI9W4Bf;
				pFqbEkWHHD.LeftMargins.Remove(OWVbO4IK1V);
				pFqbEkWHHD.TextView.ElementGenerators.Remove(mZ5bKcOiR1);
				pFqbEkWHHD.TextView.Services.RemoveService(typeof(FoldingManager));
				OWVbO4IK1V = null;
				mZ5bKcOiR1 = null;
				pFqbEkWHHD = null;
			}
		}

		private void Rk1bI9W4Bf(object P_0, EventArgs P_1)
		{
			int offset = pFqbEkWHHD.Caret.Offset;
			foreach (FoldingSection item in GetFoldingsContaining(offset))
			{
				if (item.IsFolded && item.StartOffset < offset && offset < item.EndOffset)
				{
					item.IsFolded = false;
				}
			}
		}
	}

	internal readonly TextDocument d6MYOknguE;

	internal readonly List<TextView> mIhYKQMGji;

	internal readonly TextSegmentCollection<FoldingSection> hBGY9EMuL6;

	private bool JeXYPRuXlN;

	public IEnumerable<FoldingSection> AllFoldings => hBGY9EMuL6;

	public FoldingManager(TextDocument document)
	{
		mIhYKQMGji = new List<TextView>();
		JeXYPRuXlN = true;
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		d6MYOknguE = document;
		hBGY9EMuL6 = new TextSegmentCollection<FoldingSection>();
		document.VerifyAccess();
		WeakEventManagerBase<TextDocumentWeakEventManager.Changed, TextDocument>.AddListener(document, (IWeakEventListener)(object)this);
	}

	protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		if (managerType == typeof(TextDocumentWeakEventManager.Changed))
		{
			FPQYeXrAAb((DocumentChangeEventArgs)e);
			return true;
		}
		return false;
	}

	bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		return ReceiveWeakEvent(managerType, sender, e);
	}

	private void FPQYeXrAAb(DocumentChangeEventArgs P_0)
	{
		hBGY9EMuL6.UpdateOffsets(P_0);
		int offset = P_0.Offset + P_0.InsertionLength;
		DocumentLine lineByOffset = d6MYOknguE.GetLineByOffset(offset);
		offset = lineByOffset.Offset + lineByOffset.TotalLength;
		foreach (FoldingSection item in hBGY9EMuL6.FindOverlappingSegments(P_0.Offset, offset - P_0.Offset))
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

	internal void vGQYtHRdp6(TextView P_0)
	{
		if (P_0 == null || mIhYKQMGji.Contains(P_0))
		{
			throw new ArgumentException();
		}
		mIhYKQMGji.Add(P_0);
		foreach (FoldingSection item in hBGY9EMuL6)
		{
			if (item.collapsedSections != null)
			{
				Array.Resize(ref item.collapsedSections, mIhYKQMGji.Count);
				item.ValidateCollapsedLineSections();
			}
		}
	}

	internal void NHyYbHHCqJ(TextView P_0)
	{
		int num = mIhYKQMGji.IndexOf(P_0);
		if (num < 0)
		{
			throw new ArgumentException();
		}
		mIhYKQMGji.RemoveAt(num);
		foreach (FoldingSection item in hBGY9EMuL6)
		{
			if (item.collapsedSections != null)
			{
				CollapsedLineSection[] array = new CollapsedLineSection[mIhYKQMGji.Count];
				Array.Copy(item.collapsedSections, 0, array, 0, num);
				item.collapsedSections[num].Uncollapse();
				Array.Copy(item.collapsedSections, num + 1, array, num, array.Length - num);
				item.collapsedSections = array;
			}
		}
	}

	internal void O30YI9ut90()
	{
		foreach (TextView item in mIhYKQMGji)
		{
			item.Redraw();
		}
	}

	internal void xhMYEsYpfO(FoldingSection P_0)
	{
		foreach (TextView item in mIhYKQMGji)
		{
			item.Redraw(P_0, (DispatcherPriority)9);
		}
	}

	public FoldingSection CreateFolding(int startOffset, int endOffset)
	{
		if (startOffset >= endOffset)
		{
			return null;
		}
		if (startOffset < 0 || endOffset > d6MYOknguE.TextLength)
		{
			return null;
		}
		FoldingSection foldingSection = new FoldingSection(this, startOffset, endOffset);
		hBGY9EMuL6.Add(foldingSection);
		xhMYEsYpfO(foldingSection);
		return foldingSection;
	}

	public void RemoveFolding(FoldingSection fs)
	{
		if (fs == null)
		{
			throw new ArgumentNullException("fs");
		}
		fs.IsFolded = false;
		hBGY9EMuL6.Remove(fs);
		xhMYEsYpfO(fs);
	}

	public void Clear()
	{
		d6MYOknguE.VerifyAccess();
		foreach (FoldingSection item in hBGY9EMuL6)
		{
			item.IsFolded = false;
		}
		hBGY9EMuL6.Clear();
		O30YI9ut90();
	}

	public int GetNextFoldedFoldingStart(int startOffset)
	{
		FoldingSection foldingSection = hBGY9EMuL6.FindFirstSegmentWithStartAfter(startOffset);
		while (foldingSection != null && !foldingSection.IsFolded)
		{
			foldingSection = hBGY9EMuL6.GetNextSegment(foldingSection);
		}
		return foldingSection?.StartOffset ?? (-1);
	}

	public FoldingSection GetNextFolding(int startOffset)
	{
		return hBGY9EMuL6.FindFirstSegmentWithStartAfter(startOffset);
	}

	public ReadOnlyCollection<FoldingSection> GetFoldingsAt(int startOffset)
	{
		List<FoldingSection> list = new List<FoldingSection>();
		FoldingSection foldingSection = hBGY9EMuL6.FindFirstSegmentWithStartAfter(startOffset);
		while (foldingSection != null && foldingSection.StartOffset == startOffset)
		{
			list.Add(foldingSection);
			foldingSection = hBGY9EMuL6.GetNextSegment(foldingSection);
		}
		return list.AsReadOnly();
	}

	public ReadOnlyCollection<FoldingSection> GetFoldingsContaining(int offset)
	{
		return hBGY9EMuL6.FindSegmentsContaining(offset);
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
			newFolding.StartOffset.CoerceValue(0, d6MYOknguE.TextLength);
			newFolding.EndOffset.CoerceValue(0, d6MYOknguE.TextLength);
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
				if (foldingSection == null)
				{
					return;
				}
				if (JeXYPRuXlN)
				{
					foldingSection.IsFolded = newFolding.DefaultClosed;
				}
				foldingSection.Tag = newFolding;
			}
			foldingSection.Title = newFolding.Name;
		}
		JeXYPRuXlN = false;
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
		return new gZ93VZbtAdtOiDmmhBs(textArea);
	}

	public static void Uninstall(FoldingManager manager)
	{
		if (manager == null)
		{
			throw new ArgumentNullException("manager");
		}
		if (manager is gZ93VZbtAdtOiDmmhBs gZ93VZbtAdtOiDmmhBs2)
		{
			gZ93VZbtAdtOiDmmhBs2.HOubbbGiZZ();
			return;
		}
		throw new ArgumentException("FoldingManager was not created using FoldingManager.Install");
	}
}
