using System;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;

namespace PvfCode.ViewModels.TreeFolder;

public class ViewEditTreeCommentViewModel : ViewModelBase, IWindowBindingBase
{
	[CompilerGenerated]
	private TreelistCommentRes NSRrZyIPpU;

	[CompilerGenerated]
	private Action CIDrJ416kR;

	private readonly PvfTreeFileBase TreeFile;

	public TreelistCommentRes TreeListComment
	{
		[CompilerGenerated]
		get
		{
			return NSRrZyIPpU;
		}
		[CompilerGenerated]
		set
		{
			NSRrZyIPpU = value;
		}
	}

	public Action CloseAction
	{
		[CompilerGenerated]
		get
		{
			return CIDrJ416kR;
		}
		[CompilerGenerated]
		set
		{
			CIDrJ416kR = value;
		}
	}

	public ViewEditTreeCommentViewModel(PvfTreeFileBase treeFile, Action closeAction)
	{
		CloseAction = closeAction;
		TreeFile = treeFile;
		AppSetting.Instance.PvfConfig.TreelistCommentDic.TryGetValue(treeFile.FullPath, out TreelistCommentRes value);
		TreeListComment = ((value == null) ? new TreelistCommentRes
		{
			FilePath = treeFile.FullPath
		} : value);
		TreeListComment.MasterIsShare = false;
	}

	[Command]
	public async void OnSave()
	{
		TreeListComment.Create = DateTime.Now;
		AppSetting.Instance.PvfConfig.TreelistCommentDic[TreeListComment.FilePath] = TreeListComment;
		await AppSetting.Instance.SaveSetting();
		TreeFile.CommentDoNotify();
		CloseAction();
	}
}
