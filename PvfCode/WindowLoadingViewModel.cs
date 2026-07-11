using System.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;

namespace PvfCode;

public class WindowLoadingViewModel : ViewModelBase
{
	private readonly CancellationTokenSource _cancellationTokenSource;

	public bool ShowStopButton { get; set; }

	public string Text { get; set; }

	public WindowLoadingViewModel(string text)
	{
		Text = text;
	}

	public WindowLoadingViewModel(string text, CancellationTokenSource cancellationTokenSource)
	{
		ShowStopButton = true;
		Text = text;
		_cancellationTokenSource = cancellationTokenSource;
	}

	[Command]
	public void OnStop()
	{
		_cancellationTokenSource.Cancel();
	}
}
