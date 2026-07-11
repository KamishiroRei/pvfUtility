using System.ComponentModel;

namespace PvfCode;

public struct ModelBaseStruct : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;
}
