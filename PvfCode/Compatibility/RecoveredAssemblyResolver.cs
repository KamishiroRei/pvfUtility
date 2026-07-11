using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace PvfCode.Compatibility;

internal static class RecoveredAssemblyResolver
{
	private static bool registered;

	internal static void Register()
	{
		if (registered)
		{
			return;
		}

		registered = true;
		AssemblyLoadContext.Default.Resolving += ResolveFromApplicationDirectory;
	}

	private static Assembly ResolveFromApplicationDirectory(AssemblyLoadContext context, AssemblyName requestedName)
	{
		if (string.IsNullOrWhiteSpace(requestedName.Name))
		{
			return null;
		}

		string candidatePath = Path.Combine(AppContext.BaseDirectory, requestedName.Name + ".dll");
		if (!File.Exists(candidatePath))
		{
			return null;
		}

		try
		{
			AssemblyName candidateName = AssemblyName.GetAssemblyName(candidatePath);
			if (!AssemblyName.ReferenceMatchesDefinition(requestedName, candidateName))
			{
				return null;
			}

			return context.LoadFromAssemblyPath(candidatePath);
		}
		catch (BadImageFormatException)
		{
			return null;
		}
	}
}
