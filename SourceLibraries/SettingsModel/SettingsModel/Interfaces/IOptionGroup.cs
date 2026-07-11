using System;
using System.Collections.Generic;

namespace SettingsModel.Interfaces;

public interface IOptionGroup
{
	string Name { get; }

	IEnumerable<IOptionsSchema> GetOptionDefinitions();

	IOptionsSchema GetOptionDefinition(string optionName);

	bool GetValue(string optionName, out object optValue);

	object GetValue(string optionName);

	T GetValue<T>(string optionName);

	bool SetValue(string optionName, object newValue);

	bool List_AddValue(string optionName, string keyName, object value);

	bool List_Clear(string optionName);

	IOptionsSchema List_CreateOption<T>(string optionName, Type type, bool isOptional, List<T> list);

	IEnumerable<object> List_GetListOfValues(string optionName);

	IEnumerable<KeyValuePair<object, object>> List_GetListOfKeyValues(string optionName);

	void SetUndirty(bool isDirty);
}
