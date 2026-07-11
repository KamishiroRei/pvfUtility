namespace SevenZip;

public struct ArchiveProperty
{
	public string Name { get; internal set; }

	public object Value { get; internal set; }

	public override bool Equals(object obj)
	{
		if (obj is ArchiveProperty afi)
		{
			return Equals(afi);
		}
		return false;
	}

	public bool Equals(ArchiveProperty afi)
	{
		if (afi.Name == Name)
		{
			return afi.Value == Value;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Name.GetHashCode() ^ Value.GetHashCode();
	}

	public override string ToString()
	{
		return Name + " = " + Value;
	}

	public static bool operator ==(ArchiveProperty afi1, ArchiveProperty afi2)
	{
		return afi1.Equals(afi2);
	}

	public static bool operator !=(ArchiveProperty afi1, ArchiveProperty afi2)
	{
		return !afi1.Equals(afi2);
	}
}
