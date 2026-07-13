using System.Collections.Generic;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.ViewModels.Store.Preview;

public class PreviewFileListCommentViewModel : ViewModelBase
{
	private readonly Dictionary<string, TreelistCommentRes> Source;

	public PvfTreeViewModel TreeViewModel { get; set; }

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
