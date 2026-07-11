using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using TextEditLib;
using ViewModels.DocumentFolder.OffsetColorizers;

namespace PvfCode.ViewModels.DocumentFolder.LinkFolder;

public class FilePathLinkVisualLine : VisualLineText
{
	private readonly FilePathLinkGoToDocumentPathDelegate UVH4qUYivR;

	private readonly PvfFile File;

	private readonly PvfFileType FileType;

	private readonly string kaW4dJCjd1;

	private readonly string Y214ecD9FE;

	public FilePathLinkVisualLine(PvfFile file, FilePathLinkGoToDocumentPathDelegate selectTextDelegate, VisualLine parentVisualLine, int length)
		: base(parentVisualLine, length)
	{
		kaW4dJCjd1 = file.FilePathHeader;
		File = file;
		Y214ecD9FE = file.FileName;
		FileType = file.FileType;
		UVH4qUYivR = selectTextDelegate;
	}

	public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
	{
		qLs4wV339J();
		return base.CreateTextRun(startVisualColumn, context);
	}

	private void KLn41fDTfa()
	{
		base.TextRunProperties.SetTextDecorations(TextDecorations.Underline);
	}

	private void qLs4wV339J()
	{
		if (!HaC4oLixPr(out string _))
		{
			StreamGeometry streamGeometry = new StreamGeometry();
			using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
			{
				streamGeometryContext.BeginFigure(new Point(0.0, 1.0), isFilled: false, isClosed: false);
				streamGeometryContext.PolyLineTo((IList<Point>)(object)new Point[6]
				{
					new Point(1.5, 0.0),
					new Point(3.0, 3.0),
					new Point(4.5, 1.5),
					new Point(6.0, 0.0),
					new Point(7.5, 3.0),
					new Point(9.0, 1.5)
				}, isStroked: true, isSmoothJoin: true);
			}
			Pen pen = new Pen(new DrawingBrush(new GeometryDrawing
			{
				Pen = new Pen(Brushes.Red, 1.0),
				Geometry = streamGeometry
			})
			{
				TileMode = TileMode.Tile,
				Viewport = new Rect(0.0, 0.0, 9.0, 4.0),
				ViewportUnits = BrushMappingMode.Absolute,
				Viewbox = new Rect(0.0, 0.0, 9.0, 4.0),
				ViewboxUnits = BrushMappingMode.Absolute,
				Stretch = Stretch.Fill
			}, 3.0);
			TextDecorationCollection textDecorationCollection = new TextDecorationCollection();
			TextDecoration value = new TextDecoration
			{
				Pen = pen
			};
			textDecorationCollection.Add(value);
			base.TextRunProperties.SetTextDecorations(textDecorationCollection);
		}
	}

	private bool HaC4oLixPr(out string? P_0)
	{
		if (FileType == PvfFileType.nut)
		{
			KLn41fDTfa();
		}
		string text = GetFilePath().Replace("`", string.Empty).Replace("\"", string.Empty).ToLower();
		if (text.Contains("%s/"))
		{
			P_0 = text;
			return true;
		}
		P_0 = null;
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		bool flag;
		if (FileType == PvfFileType.lst)
		{
			KLn41fDTfa();
			flag = pVF.GetLstFullPath(File, text.ToLower(), out P_0);
		}
		else
		{
			string text2 = Path.GetExtension(text).ToLower();
			if (text2 == ".img")
			{
				KLn41fDTfa();
				return true;
			}
			if (!AppSetting.Instance.PvfConfig.FileTypes.Contains(text2))
			{
				return true;
			}
			if (text == ".ani")
			{
				return true;
			}
			P_0 = text;
			flag = AppCore.ViewModelBase.PVF.FileAny(text);
			if (!flag)
			{
				string directoryName = Path.GetDirectoryName(Y214ecD9FE);
				P_0 = Path.Combine(directoryName, text).Replace("\\", "/");
				flag = pVF.FileAny(P_0);
				if (!flag)
				{
					try
					{
						bool flag2 = false;
						while (text.Length > 3 && text.Substring(0, 3) == "../")
						{
							directoryName = Path.GetDirectoryName(directoryName);
							text = text.Remove(0, 3);
							flag2 = true;
						}
						if (flag2)
						{
							P_0 = Path.Combine(directoryName, text).Replace("\\", "/");
							return pVF.FileAny(P_0);
						}
					}
					catch (Exception)
					{
						return false;
					}
					if (new Regex("\\.\\./", RegexOptions.Compiled).Matches(text).Count > 0)
					{
						int num = text.IndexOf("../", 0);
						try
						{
							while (text.Length > 3 && num != -1)
							{
								text = text.Remove(num, 3);
								if (text == null)
								{
									return false;
								}
								string text3 = text.Substring(0, num);
								if (text3 == null)
								{
									return false;
								}
								string text4 = text.Substring(num, text.Length - text3.Length);
								if (text4 == null)
								{
									return false;
								}
								text = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(text3)), text4).Replace("\\", "/");
								if (text == null)
								{
									return false;
								}
								num = text.IndexOf("../", 0);
							}
						}
						catch (Exception)
						{
							return false;
						}
						P_0 = text;
						flag = pVF.FileAny(text);
					}
					else if (Y214ecD9FE == "etc/equipmentpartset.etc")
					{
						P_0 = "equipment/" + text;
						flag = pVF.FileAny(P_0);
					}
					else if (FileType == PvfFileType.dgn)
					{
						P_0 = Path.Combine(directoryName, text).Replace("\\", "/");
						flag = pVF.FileAny(P_0);
					}
					else if (FileType == PvfFileType.aic)
					{
						if (text2 == ".ai")
						{
							P_0 = Path.Combine(Path.Combine(directoryName, "ai"), text).Replace("\\", "/");
							flag = pVF.FileAny(P_0);
						}
						else if (text2 == ".key")
						{
							P_0 = Path.Combine(Path.Combine(directoryName, "key"), text).Replace("\\", "/");
							flag = pVF.FileAny(P_0);
						}
						else if (text2 == ".act")
						{
							P_0 = Path.Combine(directoryName, text).Replace("\\", "/");
							flag = pVF.FileAny(P_0);
						}
					}
					else if (FileType == PvfFileType.ora)
					{
						P_0 = "aura/equipment" + text;
						flag = pVF.FileAny(P_0);
					}
					else if (FileType == PvfFileType.mob)
					{
						if (text2 == ".equ")
						{
							P_0 = "equipment/" + text;
							flag = pVF.FileAny(P_0);
						}
					}
					else if (FileType == PvfFileType.chr)
					{
						P_0 = Path.Combine(directoryName, text).Replace("\\", "/");
						flag = pVF.FileAny(P_0);
					}
					else
					{
						if (FileType == PvfFileType.nut)
						{
							return true;
						}
						if (FileType == PvfFileType.twn)
						{
							if (text2 == ".map")
							{
								P_0 = text;
								flag = pVF.FileAny(P_0);
								if (!flag)
								{
									P_0 = "map/" + text;
									flag = pVF.FileAny(P_0);
									if (!flag)
									{
										string fileName = Path.GetFileName(P_0);
										P_0 = Path.Combine(Path.GetDirectoryName(P_0), "(r)" + fileName).Replace("\\", "/").ToLower();
										flag = pVF.FileAny(P_0);
									}
								}
							}
						}
						else if (FileType == PvfFileType.etc)
						{
							if (Y214ecD9FE == "etc/titleballooninfo.etc")
							{
								P_0 = Path.Combine("equipment/character/common/title/etcanimation", text).Replace("\\", "/").ToLower();
								flag = pVF.FileAny(P_0);
							}
							else if (Y214ecD9FE == "etc/equipmenteffectset.etc")
							{
								P_0 = Path.Combine("equipment", text).Replace("\\", "/").ToLower();
								flag = pVF.FileAny(P_0);
							}
							else if (kaW4dJCjd1 == "character" && text2 == ".equ")
							{
								P_0 = Path.Combine("equipment", text).ToLower().Replace("\\", "/");
								flag = pVF.FileAny(P_0);
							}
							else if (Y214ecD9FE == "etc/skillpreload.etc" && !pVF.FileAny(P_0) && P_0.Contains("//") && P_0.Length > 4 && P_0.Substring(0, 4) == "etc/")
							{
								P_0 = P_0.Replace("//", "/").Substring(4, P_0.Length - 5);
								flag = pVF.FileAny(P_0);
							}
						}
					}
				}
			}
		}
		if (flag)
		{
			KLn41fDTfa();
		}
		return flag;
	}

	public override void OnQueryCursor(QueryCursorEventArgs e)
	{
		try
		{
			DocumentBase document = AppCore.ViewModelBase.RootDocument.GetDocument(File.FileName);
			if (document == null || !(document is PvfFileDocument pvfFileDocument))
			{
				return;
			}
			TextEdit textEdit = pvfFileDocument?.GetEditor();
			if (textEdit == null)
			{
				return;
			}
			if (((int)Keyboard.Modifiers & 2) == 2)
			{
				e.Cursor = Cursors.Hand;
				if (textEdit != null)
				{
					AWR4LvNe4U(textEdit);
				}
			}
			else
			{
				e.Cursor = Cursors.IBeam;
			}
			((TextView)e.Source).Redraw(base.ParentVisualLine, (DispatcherPriority)5);
			e.Handled = true;
		}
		catch (Exception e2)
		{
			AppCore.Logger.ErrorUploadDialog(e2, "VisualLineReferenceText2.OnQueryCursor");
		}
	}

	public override void OnPreviewUp(MouseButtonEventArgs e)
	{
		string fullPath;
		if (FileType == PvfFileType.nut)
		{
			UVH4qUYivR?.Invoke(kVc4sSfCgH());
		}
		else if (HaC4oLixPr(out fullPath))
		{
			UVH4qUYivR?.Invoke(kVc4sSfCgH(), fullPath);
		}
		else
		{
			UVH4qUYivR?.Invoke(kVc4sSfCgH(), string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), GetFilePath()));
		}
	}

	public override void OnMouseHoverStopped(MouseEventArgs e)
	{
		DocumentBase document = AppCore.ViewModelBase.RootDocument.GetDocument(File.FileName);
		if (document != null && document is PvfFileDocument pvfFileDocument)
		{
			TextEdit textEdit = pvfFileDocument?.GetEditor();
			if (textEdit != null)
			{
				aXx4nAs87t(textEdit);
			}
		}
	}

	public string GetFilePath()
	{
		return base.ParentVisualLine.Document.GetText(kVc4sSfCgH());
	}

	private TextSegment kVc4sSfCgH()
	{
		int num = GetRelativeOffset(base.VisualColumn) - base.VisualColumn;
		return new TextSegment
		{
			StartOffset = base.ParentVisualLine.FirstDocumentLine.Offset + base.RelativeTextOffset + num,
			Length = base.DocumentLength - num
		};
	}

	private void AWR4LvNe4U(TextEdit P_0)
	{
		if (P_0 == null)
		{
			return;
		}
		TextSegment textSegment = kVc4sSfCgH();
		bool flag = true;
		IVisualLineTransformer[] array = P_0.TextArea.TextView.LineTransformers.ToArray();
		foreach (IVisualLineTransformer visualLineTransformer in array)
		{
			if (visualLineTransformer is LinkHoverStyle linkHoverStyle)
			{
				if (linkHoverStyle.TextSeg.StartOffset == textSegment.StartOffset && linkHoverStyle.TextSeg.Length == textSegment.Length)
				{
					flag = false;
				}
				else
				{
					P_0.TextArea.TextView.LineTransformers.Remove(visualLineTransformer);
				}
			}
		}
		if (flag)
		{
			P_0.TextArea.TextView.LineTransformers.Add(new LinkHoverStyle(textSegment));
		}
	}

	private void aXx4nAs87t(TextEdit P_0)
	{
		IVisualLineTransformer[] array = P_0.TextArea.TextView.LineTransformers.ToArray();
		foreach (IVisualLineTransformer visualLineTransformer in array)
		{
			if (visualLineTransformer is LinkHoverStyle)
			{
				P_0.TextArea.TextView.LineTransformers.Remove(visualLineTransformer);
			}
		}
	}
}
