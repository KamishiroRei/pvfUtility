using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Swordfish.NET.Collections;

public interface IConcurrentObservableList<T> : IConcurrentObservableBase<T>, INotifyPropertyChanged, INotifyCollectionChanged, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection
{
	ImmutableList<T> ImmutableList { get; }
}
