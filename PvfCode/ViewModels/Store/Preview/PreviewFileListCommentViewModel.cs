using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.ViewModels.Store.Preview;

public class PreviewFileListCommentViewModel : ViewModelBase
{
	[CompilerGenerated]
	private PvfTreeViewModel FSuWfGlYSI;

	private readonly Dictionary<string, TreelistCommentRes> Source;

	public PvfTreeViewModel TreeViewModel
	{
		[CompilerGenerated]
		get
		{
			return FSuWfGlYSI;
		}
		[CompilerGenerated]
		set
		{
			FSuWfGlYSI = value;
		}
	}

	public PreviewFileListCommentViewModel(Dictionary<string, TreelistCommentRes> source)
	{
		TreeViewModel = new PvfTreeViewModel(TreeViewType.FileListDescription);
		Source = source;
	}

	[Command]
	public async void Loaded()
	{
		await TreeViewModel.TreeGroupData.CreateFileListDescriptionTrees(Source.Keys, Source);
	}

	[Command]
	public void Closed()
	{
		Application.Current.MainWindow.Activate();
	}
}
