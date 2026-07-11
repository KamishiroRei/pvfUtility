using System;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

[Serializable]
public struct XshdReference<T> : IEquatable<XshdReference<T>> where T : XshdElement
{
	private string referencedDefinition;

	private string referencedElement;

	private T inlineElement;

	public string ReferencedDefinition => referencedDefinition;

	public string ReferencedElement => referencedElement;

	public T InlineElement => inlineElement;

	public XshdReference(string referencedDefinition, string referencedElement)
	{
		if (referencedElement == null)
		{
			throw new ArgumentNullException("referencedElement");
		}
		this.referencedDefinition = referencedDefinition;
		this.referencedElement = referencedElement;
		inlineElement = null;
	}

	public XshdReference(T inlineElement)
	{
		if (inlineElement == null)
		{
			throw new ArgumentNullException("inlineElement");
		}
		referencedDefinition = null;
		referencedElement = null;
		this.inlineElement = inlineElement;
	}

	public object AcceptVisitor(IXshdVisitor visitor)
	{
		if (inlineElement != null)
		{
			return inlineElement.AcceptVisitor(visitor);
		}
		return null;
	}

	public override bool Equals(object obj)
	{
		if (obj is XshdReference<T>)
		{
			return Equals((XshdReference<T>)obj);
		}
		return false;
	}

	public bool Equals(XshdReference<T> other)
	{
		if (referencedDefinition == other.referencedDefinition && referencedElement == other.referencedElement)
		{
			return inlineElement == other.inlineElement;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return GetHashCode(referencedDefinition) ^ GetHashCode(referencedElement) ^ GetHashCode(inlineElement);
	}

	private static int GetHashCode(object o)
	{
		return o?.GetHashCode() ?? 0;
	}

	public static bool operator ==(XshdReference<T> left, XshdReference<T> right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(XshdReference<T> left, XshdReference<T> right)
	{
		return !left.Equals(right);
	}
}
