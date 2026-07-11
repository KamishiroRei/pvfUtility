using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;

namespace PvfCode.ViewModels.DocumentFolder.VisualLineElementGenerators;

public class LstSectionCommentView : Button, IComponentConnector
{
	private readonly string FilePath;

	private Action<TextSegment> Ob25EatPqg;

	private TextSegment YGA5OnZjBo;

	internal LstSectionCommentView TestBlock;

	private bool pYH5K2visH;

	public LstSectionCommentView(PvfFile lstFile, string lstRightPath, Action<TextSegment> act, TextSegment textSeg)
	{
		Ob25EatPqg = act;
		InitializeComponent();
		AppCore.ViewModelBase.PVF.GetLstFullPath(lstFile, lstRightPath, out string filePath);
		FilePath = filePath;
		if (AppCore.ViewModelBase.PVF.FileAny(filePath))
		{
			string text = AppCore.ViewModelBase.PVF.GetItemName(filePath);
			if (string.IsNullOrEmpty(text))
			{
				text = AppSetting.Instance.GetIlogger().GetStr("LstFileRightItemNameBtn_CannotFindItemName");
			}
			TestBlock.Content = text.Replace("\r\n", string.Empty);
			if (AppSetting.Instance.EditConfig.LstDocumentNameGoFileNeedCtrl)
			{
				TestBlock.ToolTip = AppSetting.Instance.GetIlogger()?.GetStr("LstFileRightItemNameBtnTooltipInCtrl");
			}
			else
			{
				TestBlock.ToolTip = AppSetting.Instance.GetIlogger()?.GetStr("LstFileRightItemNameBtnTooltip");
			}
		}
		else
		{
			TestBlock.Content = AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExistTitle") + "...";
			TestBlock.Foreground = new SolidColorBrush(Colors.Red);
		}
		YGA5OnZjBo = textSeg;
	}

	protected override void OnClick()
	{
		Ob25EatPqg(YGA5OnZjBo);
		bool flag = ((int)Keyboard.Modifiers & 2) == 2;
		if (AppSetting.Instance.EditConfig.LstDocumentNameGoFileNeedCtrl && flag)
		{
			DhG5IKsYj3();
		}
		else if (!AppSetting.Instance.EditConfig.LstDocumentNameGoFileNeedCtrl)
		{
			DhG5IKsYj3();
		}
	}

	private void gtH5bMNJQ5()
	{
		Ob25EatPqg(YGA5OnZjBo);
		if (((int)Keyboard.Modifiers & 2) == 2)
		{
			DhG5IKsYj3();
		}
	}

	private void DhG5IKsYj3()
	{
		AppCore.ViewModelBase.RootDocument.AddDocument(FilePath, gotoNode: true);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!pYH5K2visH)
		{
			pYH5K2visH = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/viewmodels/documentfolder/visuallineelementgenerators/lstsectioncommentview.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			TestBlock = (LstSectionCommentView)target;
		}
		else
		{
			pYH5K2visH = true;
		}
	}
}
