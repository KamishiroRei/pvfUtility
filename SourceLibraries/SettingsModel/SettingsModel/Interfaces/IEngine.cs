using System;
using System.Collections.Generic;
using System.IO;

namespace SettingsModel.Interfaces;

public interface IEngine
{
	bool IsDirty { get; }

	IOptionsSchema AddOption(string nameOfOptionGroup, string optionName, Type type, bool isOptional, object value);

	IOptionsSchema AddListOption<T>(string nameOfOptionGroup, string optionName, Type type, bool isOptional, List<T> list);

	bool GetOptionValue(string nameOfOptionGroup, string optionName, out object optValue);

	object GetOptionValue(string nameOfOptionGroup, string optionName);

	T GetOptionValue<T>(string nameOfOptionGroup, string optionName);

	bool SetOptionValue(string nameOfOptionGroup, string optionName, object newValue);

	IEnumerable<IOptionGroup> GetOptionGroups();

	IOptionGroup GetOptionGroup(string nameOfOptionGroup);

	void WriteXML(string fileName);

	string WriteXML();

	void ReadXML(string fileName);

	void ReadXML(TextReader reader);

	void SetUndirty();

	bool RemoveOptionsGroup(string nameOfOptionGroup);

	bool RemoveOption(string nameOfOptionGroup, string optionName);

	void RemoveAllOptions();
}
