using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Highlighting;

namespace HL.Manager;

internal sealed class DelayLoadedHighlightingDefinition : IHighlightingDefinition
{
	private readonly object lockObj = new object();

	private readonly string name;

	private Func<IHighlightingDefinition> lazyLoadingFunction;

	private IHighlightingDefinition definition;

	private Exception storedException;

	public string Name
	{
		get
		{
			if (name != null)
			{
				return name;
			}
			return GetDefinition().Name;
		}
	}

	public HighlightingRuleSet MainRuleSet => GetDefinition().MainRuleSet;

	public IEnumerable<HighlightingColor> NamedHighlightingColors => GetDefinition().NamedHighlightingColors;

	public IDictionary<string, string> Properties => GetDefinition().Properties;

	public DelayLoadedHighlightingDefinition(string name, Func<IHighlightingDefinition> lazyLoadingFunction)
	{
		this.name = name;
		this.lazyLoadingFunction = lazyLoadingFunction;
	}

	private IHighlightingDefinition GetDefinition()
	{
		Func<IHighlightingDefinition> func;
		lock (lockObj)
		{
			if (definition != null)
			{
				return definition;
			}
			func = lazyLoadingFunction;
		}
		Exception ex = null;
		IHighlightingDefinition highlightingDefinition = null;
		try
		{
			using (BusyManager.BusyLock busyLock = BusyManager.Enter(this))
			{
				if (!busyLock.Success)
				{
					throw new InvalidOperationException("Tried to create delay-loaded highlighting definition recursively. Make sure the are no cyclic references between the highlighting definitions.");
				}
				highlightingDefinition = func();
			}
			if (highlightingDefinition == null)
			{
				throw new InvalidOperationException("Function for delay-loading highlighting definition returned null");
			}
		}
		catch (Exception ex2)
		{
			ex = ex2;
		}
		lock (lockObj)
		{
			lazyLoadingFunction = null;
			if (definition == null && storedException == null)
			{
				definition = highlightingDefinition;
				storedException = ex;
			}
			if (storedException != null)
			{
				throw new HighlightingDefinitionInvalidException("Error delay-loading highlighting definition", storedException);
			}
			return definition;
		}
	}

	public HighlightingRuleSet GetNamedRuleSet(string name)
	{
		return GetDefinition().GetNamedRuleSet(name);
	}

	public HighlightingColor GetNamedColor(string name)
	{
		return GetDefinition().GetNamedColor(name);
	}

	public override string ToString()
	{
		return Name;
	}
}
