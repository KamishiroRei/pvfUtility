using System.Collections.Generic;

namespace ViewModels.independent_drop;

public class Dungeon_drop_rate_balance
{
	private readonly List<int> oVG1ScHs9;

	public Dungeon_drop_rate_balance(List<int> items)
	{
		oVG1ScHs9 = items;
	}

	public string ToText()
	{
		return string.Join("\t", oVG1ScHs9);
	}
}
