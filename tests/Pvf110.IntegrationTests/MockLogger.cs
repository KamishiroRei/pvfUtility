using System.Windows;
using System.Windows.Threading;
using Collections.Pooled;
using DevExpress.Mvvm;
using PvfCode.LoggerBase;
using Swordfish.NET.Collections;

namespace Pvf110.IntegrationTests;

/// <summary>最小 Ilogger 实现：控制台输出，供 110JP 集成测试使用。</summary>
public sealed class MockLogger : Ilogger
{
    public CancellationTokenSource TaskCancellationTokenSource { get; set; } = new();
    public bool TaskIsWork { get; set; }
    public ConcurrentObservableCollection<ErrorItem> ErrorItems { get; set; } = new();

    public void TaskTokenStart() { }
    public void TaskTokenStop() { }
    public void ProgressUpdate(double value) { }
    public void Debug(string msg) => Console.WriteLine($"[D] {msg}");
    public void Debug(object obj) => Console.WriteLine($"[D] {obj}");
    public void Warning(string msg) => Console.WriteLine($"[W] {msg}");
    public void Success(string msg) => Console.WriteLine($"[S] {msg}");
    public void Error(string msg) => Console.WriteLine($"[E] {msg}");
    public void ErrorUploadDialog(Exception e, string caption) => Console.WriteLine($"[E-DLG] {caption}: {e.Message}");
    public void Error(List<ErrorItem> errs)
    {
        foreach (var e in errs) Console.WriteLine($"[E-LIST] line={e.Line} input=<{e.Input}> desc={e.Description}");
    }
    public void ShowMsg(string msg, bool isError = false, string? caption = null, bool loggerError = false)
        => Console.WriteLine($"{(isError ? "[E-MSG]" : "[MSG]")} {msg}");
    public MessageResult ShowDialog(string msg, string? caption = null)
    {
        Console.WriteLine($"[DLG] {caption}: {msg}");
        return MessageResult.OK;
    }
    public MessageResult ShowDialogResult(IMessageBoxService service, string msg, string? caption = null)
        => ShowDialog(msg, caption);
    public Task ShowNotification(NotificationViewModel vm) => Task.CompletedTask;
    public Task ShowNotification<TIcommandParm>(NotificationViewModel<TIcommandParm> vm) => Task.CompletedTask;
    public void ClearMessage() { }
    public Task TreeListAddFiles(PooledList<string> fileList) => Task.CompletedTask;
    public Window CreateLoadingWindow(string title, Window? owner = null) => new Window { Title = title };
    public void ShowLoadingWindow(Window win) { }
    public void CloseLoadingWindow(Window win) { }
    public void ShowSavePvfPackOptionsDialog() { }
    public void AddFileListToSearchPanel(IEnumerable<string> filleList) { }
    public Task AddFileListToCurrentSearchPanel(IEnumerable<string> filleList, bool expandAllNodes = true) => Task.CompletedTask;
    public void AddFileListToNewSearchPanel(IEnumerable<string> filleList, string title) { }
    public void GoToTreeListNode(string filePath) { }
    public string GetStr(string key) => key;
    public string GetStrNoReplace(string key) => key;
    public void OpenPvfFileDocument(string filePath, bool gotoNode = false) { }
    public void SetDocumentFocused(string filePath, bool goToNode = false) { }
    public void OpenNpcShopEditor(string filePath) { }
    public Dispatcher GetAppDispatcher() => Dispatcher.CurrentDispatcher;
}
