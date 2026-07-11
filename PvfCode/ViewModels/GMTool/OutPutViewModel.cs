#define TRACE
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using GMTool.SqlModel;
using HL.Interfaces;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Utools;

namespace PvfCode.ViewModels.GMTool;

public class OutPutViewModel : ViewModelBase, GMToolLogger
{
	private IHighlightingDefinition highlighting;

	public IHighlightingDefinition Highlighting
	{
		get
		{
			return highlighting;
		}
		set
		{
			highlighting = value;
			RaisePropertyChanged("Highlighting");
		}
	}

	public TextDocument Document { get; set; }

	public OutPutViewModel()
	{
		Document = new TextDocument();
		IThemedHighlightingManager service = AppSetting.Instance.GetService<IThemedHighlightingManager>();
		Highlighting = service.GetDefinition("LOG");
	}

	public void Success(string msg)
	{
		AppendMessage(msg);
	}

	public void Error(string msg)
	{
		AppendMessage(msg);
	}

	private void AppendMessage(string message)
	{
		try
		{
			Trace.WriteLine(message);
			((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)(() =>
				Document.Insert(Document.TextLength, DateTime.Now.ToString("HH:mm:ss") + " " + message + "\r\n")),
				Array.Empty<object>());
		}
		catch (Exception)
		{
		}
	}

	public void Debug(string msg)
	{
		AppendMessage(msg);
	}

	public void Debug(object obj)
	{
		AppendMessage(obj.ToJson());
	}

	public void Warning(string msg)
	{
		AppendMessage(msg);
	}

	[Command]
	public void OnClear()
	{
		Document.Text = string.Empty;
	}
}
