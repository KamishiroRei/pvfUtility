using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Collections.Pooled;
using DevExpress.Mvvm;
using Swordfish.NET.Collections;

namespace PvfCode.LoggerBase;

public interface Ilogger
{
	CancellationTokenSource TaskCancellationTokenSource { get; set; }

	bool TaskIsWork { get; set; }

	ConcurrentObservableCollection<ErrorItem> ErrorItems { get; set; }

	void TaskTokenStart();

	void TaskTokenStop();

	void ProgressUpdate(double value);

	void Debug(string msg);

	void Debug(object obj);

	void Warning(string msg);

	void Success(string msg);

	void Error(string msg);

	void ErrorUploadDialog(Exception e, string caption);

	void Error(List<ErrorItem> errs);

	void ShowMsg(string msg, bool isError = false, string? caption = null, bool loggerError = false);

	MessageResult ShowDialog(string msg, string? caption = null);

	MessageResult ShowDialogResult(IMessageBoxService service, string msg, string? caption = null);

	Task ShowNotification(NotificationViewModel vm);

	Task ShowNotification<TIcommandParm>(NotificationViewModel<TIcommandParm> vm);

	void ClearMessage();

	Task TreeListAddFiles(PooledList<string> fileList);

	Window CreateLoadingWindow(string title, Window? owner = null);

	void ShowLoadingWindow(Window win);

	void CloseLoadingWindow(Window win);

	void ShowSavePvfPackOptionsDialog();

	void AddFileListToSearchPanel(IEnumerable<string> filleList);

	Task AddFileListToCurrentSearchPanel(IEnumerable<string> filleList, bool expandAllNodes = true);

	void AddFileListToNewSearchPanel(IEnumerable<string> filleList, string title);

	void GoToTreeListNode(string filePath);

	string GetStr(string key);

	string GetStrNoReplace(string key);

	void OpenPvfFileDocument(string filePath, bool gotoNode = false);

	void SetDocumentFocused(string filePath, bool goToNode = false);

	void OpenNpcShopEditor(string filePath);

	Dispatcher GetAppDispatcher();
}
