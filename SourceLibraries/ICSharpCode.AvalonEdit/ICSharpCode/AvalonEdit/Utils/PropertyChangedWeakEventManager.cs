using System.ComponentModel;

namespace ICSharpCode.AvalonEdit.Utils;

public sealed class PropertyChangedWeakEventManager : WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>
{
	protected override void StartListening(INotifyPropertyChanged source)
	{
		source.PropertyChanged += base.DeliverEvent;
	}

	protected override void StopListening(INotifyPropertyChanged source)
	{
		source.PropertyChanged -= base.DeliverEvent;
	}
}
