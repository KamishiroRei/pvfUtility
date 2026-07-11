using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using DevExpress.Mvvm.UI;

namespace PvfCode;

[Guid("E343F8F2-CA68-4BF4-BB54-EEA4B3AC4A31")]
[ComVisible(true)]
public class MyNotificationActivator : ToastNotificationActivator
{
	public override void OnActivate(string arguments, Dictionary<string, string> data)
	{
		MessageBox.Show("Activate it!");
	}

	public MyNotificationActivator()
	{
	}
}
