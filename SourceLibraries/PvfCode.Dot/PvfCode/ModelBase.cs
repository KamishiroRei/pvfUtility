using System.ComponentModel;

namespace PvfCode;

public class ModelBase : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;

	public void DoNotify(string properName = "")
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(properName));
	}
}
