using System;
using System.Collections.Generic;
using SettingsModel.Models;

namespace SettingsModel.Interfaces;

public interface IOptionsSchema
{
	OptionSchemaType SchemaType { get; }

	string OptionName { get; }

	Type TypeOfValue { get; }

	bool IsOptional { get; }

	object Value { get; }

	object DefaultValue { get; }

	bool List_Remove(string key);

	bool List_TryGetValue(string key, out object value);

	bool SetValue(object newValue);

	bool List_AddValue(string name, object value);

	bool List_Clear();

	IEnumerable<object> List_GetListOfValues();

	IEnumerable<KeyValuePair<object, object>> List_GetListOfKeyValues();
}
