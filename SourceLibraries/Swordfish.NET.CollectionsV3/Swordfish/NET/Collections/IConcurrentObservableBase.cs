using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Swordfish.NET.Collections;

public interface IConcurrentObservableBase<T> : INotifyPropertyChanged, INotifyCollectionChanged
{
	IList<T> CollectionView { get; }

	int Count { get; }
}
