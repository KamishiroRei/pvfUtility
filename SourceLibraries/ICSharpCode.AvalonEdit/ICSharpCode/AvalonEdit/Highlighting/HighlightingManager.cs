using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting;

public class HighlightingManager : IHighlightingDefinitionReferenceResolver
{
	private sealed class DelayLoadedHighlightingDefinition : IHighlightingDefinition
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

	internal sealed class DefaultHighlightingManager : HighlightingManager
	{
		public new static readonly DefaultHighlightingManager Instance = new DefaultHighlightingManager();

		public DefaultHighlightingManager()
		{
			Resources.RegisterBuiltInHighlightings(this);
		}

		internal void RegisterHighlighting(string name, string[] extensions, string resourceName)
		{
			try
			{
				RegisterHighlighting(name, extensions, LoadHighlighting(resourceName));
			}
			catch (HighlightingDefinitionInvalidException innerException)
			{
				throw new InvalidOperationException("The built-in highlighting '" + name + "' is invalid.", innerException);
			}
		}

		private Func<IHighlightingDefinition> LoadHighlighting(string resourceName)
		{
			return delegate
			{
				XshdSyntaxDefinition syntaxDefinition;
				using (Stream input = Resources.OpenStream(resourceName))
				{
					using XmlTextReader reader = new XmlTextReader(input);
					syntaxDefinition = HighlightingLoader.LoadXshd(reader, skipValidation: true);
				}
				return HighlightingLoader.Load(syntaxDefinition, this);
			};
		}
	}

	private readonly object lockObj = new object();

	private Dictionary<string, IHighlightingDefinition> highlightingsByName = new Dictionary<string, IHighlightingDefinition>();

	private Dictionary<string, IHighlightingDefinition> highlightingsByExtension = new Dictionary<string, IHighlightingDefinition>(StringComparer.OrdinalIgnoreCase);

	private List<IHighlightingDefinition> allHighlightings = new List<IHighlightingDefinition>();

	public ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions
	{
		get
		{
			lock (lockObj)
			{
				return Array.AsReadOnly(allHighlightings.ToArray());
			}
		}
	}

	public static HighlightingManager Instance => DefaultHighlightingManager.Instance;

	public IHighlightingDefinition GetDefinition(string name)
	{
		lock (lockObj)
		{
			if (highlightingsByName.TryGetValue(name, out var value))
			{
				return value;
			}
			return null;
		}
	}

	public IHighlightingDefinition GetDefinitionByExtension(string extension)
	{
		lock (lockObj)
		{
			if (highlightingsByExtension.TryGetValue(extension, out var value))
			{
				return value;
			}
			return null;
		}
	}

	public void RegisterHighlighting(string name, string[] extensions, IHighlightingDefinition highlighting)
	{
		if (highlighting == null)
		{
			throw new ArgumentNullException("highlighting");
		}
		lock (lockObj)
		{
			allHighlightings.Add(highlighting);
			if (name != null)
			{
				highlightingsByName[name] = highlighting;
			}
			if (extensions != null)
			{
				foreach (string key in extensions)
				{
					highlightingsByExtension[key] = highlighting;
				}
			}
		}
	}

	public void RegisterHighlighting(string name, string[] extensions, Func<IHighlightingDefinition> lazyLoadedHighlighting)
	{
		if (lazyLoadedHighlighting == null)
		{
			throw new ArgumentNullException("lazyLoadedHighlighting");
		}
		RegisterHighlighting(name, extensions, new DelayLoadedHighlightingDefinition(name, lazyLoadedHighlighting));
	}
}
