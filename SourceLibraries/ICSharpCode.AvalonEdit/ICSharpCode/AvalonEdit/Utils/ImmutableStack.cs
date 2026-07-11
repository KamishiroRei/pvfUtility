using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.AvalonEdit.Utils;

[Serializable]
public sealed class ImmutableStack<T> : IEnumerable<T>, IEnumerable
{
	public static readonly ImmutableStack<T> Empty = new ImmutableStack<T>();

	private readonly T value;

	private readonly ImmutableStack<T> next;

	public bool IsEmpty => next == null;

	private ImmutableStack()
	{
	}

	private ImmutableStack(T value, ImmutableStack<T> next)
	{
		this.value = value;
		this.next = next;
	}

	public ImmutableStack<T> Push(T item)
	{
		return new ImmutableStack<T>(item, this);
	}

	public T Peek()
	{
		if (IsEmpty)
		{
			throw new InvalidOperationException("Operation not valid on empty stack.");
		}
		return value;
	}

	public T PeekOrDefault()
	{
		return value;
	}

	public ImmutableStack<T> Pop()
	{
		if (IsEmpty)
		{
			throw new InvalidOperationException("Operation not valid on empty stack.");
		}
		return next;
	}

	public IEnumerator<T> GetEnumerator()
	{
		ImmutableStack<T> t = this;
		while (!t.IsEmpty)
		{
			yield return t.value;
			t = t.next;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder("[Stack");
		using (IEnumerator<T> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				stringBuilder.Append(' ');
				stringBuilder.Append(current);
			}
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}
}
