using System;
using DevExpress.Mvvm.UI;

namespace PvfCode.Converts;

public class EventArgsConverterSenderAndArgs : IEventArgsConverter
{
	public object Convert(object sender, object args)
	{
		return new Tuple<object, object>(sender, args);
	}

	public EventArgsConverterSenderAndArgs()
	{
	}
}
