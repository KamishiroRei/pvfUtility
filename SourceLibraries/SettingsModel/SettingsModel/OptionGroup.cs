using System;
using System.Collections.Generic;
using SettingsModel.Interfaces;
using SettingsModel.Models;

namespace SettingsModel;

internal class OptionGroup : IOptionGroup
{
	public string Name { get; private set; }

	public bool IsDirty { get; private set; }

	private Dictionary<string, OptionsSchema> OptionDefinitions { get; set; }

	public OptionGroup(string name)
	{
		Name = name;
		OptionDefinitions = new Dictionary<string, OptionsSchema>();
		IsDirty = false;
	}

	public IEnumerable<IOptionsSchema> GetOptionDefinitions()
	{
		foreach (KeyValuePair<string, OptionsSchema> optionDefinition in OptionDefinitions)
		{
			yield return optionDefinition.Value;
		}
	}

	public IOptionsSchema GetOptionDefinition(string optionName)
	{
		if (OptionDefinitions.TryGetValue(optionName, out var value))
		{
			return value;
		}
		return null;
	}

	public bool GetValue(string optionName, out object outresult)
	{
		outresult = null;
		if (!OptionDefinitions.TryGetValue(optionName, out var value))
		{
			return false;
		}
		outresult = value.Value;
		return true;
	}

	public object GetValue(string optionName)
	{
		if (!GetValue(optionName, out var outresult))
		{
			throw new Exception($"The application option {Name}-{optionName} cannot be located.");
		}
		return outresult;
	}

	public T GetValue<T>(string optionName)
	{
		object value = GetValue(optionName);
		if (!(value is T))
		{
			throw new Exception($"The requested option {Name}-{optionName} is not of requested type <T>.");
		}
		return (T)value;
	}

	public bool SetValue(string optionName, object newValue)
	{
		if (!OptionDefinitions.TryGetValue(optionName, out var value))
		{
			return false;
		}
		if (value.TypeOfValue != newValue.GetType())
		{
			throw new Exception($"Expected Type:'{value.TypeOfValue}' of '{Name}-{value.OptionName}' was not supplied '{newValue.GetType()}'");
		}
		bool flag = value.SetValue(newValue);
		IsDirty |= flag;
		return flag;
	}

	public bool List_AddValue(string optionName, string keyName, object value)
	{
		return GetOptionDefinition(optionName)?.List_AddValue(keyName, value) ?? false;
	}

	public bool List_Clear(string optionName)
	{
		if (!OptionDefinitions.TryGetValue(optionName, out var value))
		{
			return false;
		}
		value.List_Clear();
		return true;
	}

	public IOptionsSchema List_CreateOption<T>(string optionName, Type type, bool isOptional, List<T> list)
	{
		OptionsSchema optionsSchema = OptionsSchema.CreateOptionsSchema(optionName, type, isOptional, list);
		OptionDefinitions.Add(optionName, optionsSchema);
		IsDirty = true;
		return optionsSchema;
	}

	public IEnumerable<object> List_GetListOfValues(string optionName)
	{
		return GetOptionDefinition(optionName)?.List_GetListOfValues();
	}

	public IEnumerable<KeyValuePair<object, object>> List_GetListOfKeyValues(string optionName)
	{
		return GetOptionDefinition(optionName)?.List_GetListOfKeyValues();
	}

	public void SetUndirty(bool isDirty)
	{
		IsDirty = isDirty;
	}

	internal IOptionsSchema AddOption(string optionName, Type type, bool IsOptional, object value)
	{
		OptionsSchema optionsSchema = new OptionsSchema(optionName, type, IsOptional, value);
		OptionDefinitions.Add(optionName, optionsSchema);
		IsDirty = true;
		return optionsSchema;
	}

	internal bool RemoveOptionDefinition(string optionName)
	{
		return OptionDefinitions.Remove(optionName);
	}
}
