using System;
using System.Collections.Generic;
using SettingsModel.Interfaces;

namespace SettingsModel.Models;

internal class OptionsSchema : IOptionsSchema
{
	private Dictionary<object, object> mValues;

	public OptionSchemaType SchemaType { get; private set; }

	public string OptionName { get; private set; }

	public Type TypeOfValue { get; private set; }

	public bool IsOptional { get; private set; }

	public object Value { get; private set; }

	public object DefaultValue { get; private set; }

	public OptionsSchema(string optionName, Type typeOfOptionValue, bool isOptional, object defaultValue, OptionSchemaType schemaType = OptionSchemaType.SingleValue)
	{
		SchemaType = schemaType;
		OptionName = optionName;
		TypeOfValue = typeOfOptionValue;
		IsOptional = isOptional;
		if ((uint)schemaType <= 1u)
		{
			Value = (DefaultValue = defaultValue);
			return;
		}
		throw new NotSupportedException(schemaType.ToString());
	}

	public static OptionsSchema CreateOptionsSchema<T>(string optionName, Type typeOfOptionValue, bool isOptional, List<T> values)
	{
		OptionsSchema optionsSchema = new OptionsSchema(optionName, typeOfOptionValue, isOptional, null, OptionSchemaType.List);
		optionsSchema.InitializeValueList((values != null) ? values : new List<T>());
		return optionsSchema;
	}

	private void InitializeValueList<T>(List<T> values)
	{
		if (mValues != null)
		{
			mValues.Clear();
		}
		else
		{
			mValues = new Dictionary<object, object>();
		}
		foreach (T value in values)
		{
			mValues.Add(value, value);
		}
	}

	public bool List_Remove(string key)
	{
		if (mValues != null)
		{
			return mValues.Remove(key);
		}
		return false;
	}

	public bool List_TryGetValue(string key, out object value)
	{
		value = null;
		if (mValues != null)
		{
			return mValues.TryGetValue(key, out value);
		}
		return false;
	}

	public bool SetValue(object newValue)
	{
		if (SchemaType == OptionSchemaType.List)
		{
			if (mValues.TryGetValue(newValue, out var _))
			{
				return false;
			}
			mValues.Add(newValue, newValue);
			return true;
		}
		if (Value != newValue)
		{
			Value = newValue;
			return true;
		}
		return false;
	}

	public bool List_AddValue(string name, object value)
	{
		if (SchemaType == OptionSchemaType.List)
		{
			if (mValues.TryGetValue(name, out var _))
			{
				mValues.Remove(name);
			}
			mValues.Add(name, value);
			return true;
		}
		return false;
	}

	public bool List_Clear()
	{
		if (SchemaType == OptionSchemaType.List)
		{
			mValues.Clear();
			return true;
		}
		return false;
	}

	public IEnumerable<object> List_GetListOfValues()
	{
		if (SchemaType == OptionSchemaType.List)
		{
			foreach (KeyValuePair<object, object> mValue in mValues)
			{
				yield return mValue.Value;
			}
		}
		else
		{
			yield return Value;
		}
	}

	public IEnumerable<KeyValuePair<object, object>> List_GetListOfKeyValues()
	{
		if (SchemaType != OptionSchemaType.List)
		{
			yield break;
		}
		foreach (KeyValuePair<object, object> mValue in mValues)
		{
			yield return mValue;
		}
	}
}
