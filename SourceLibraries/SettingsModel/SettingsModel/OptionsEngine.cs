using System;
using System.Collections.Generic;
using System.IO;
using SettingsModel.Interfaces;
using SettingsModel.Models.XML;

namespace SettingsModel;

internal class OptionsEngine : IEngine
{
	private readonly Dictionary<string, OptionGroup> mOptionGroups = new Dictionary<string, OptionGroup>();

	private bool mIsDirty;

	public bool IsDirty
	{
		get
		{
			if (mOptionGroups != null && mOptionGroups.Count > 0)
			{
				bool flag = mIsDirty;
				{
					foreach (KeyValuePair<string, OptionGroup> mOptionGroup in mOptionGroups)
					{
						flag |= mOptionGroup.Value.IsDirty;
					}
					return flag;
				}
			}
			return mIsDirty;
		}
		private set
		{
			if (mIsDirty != value)
			{
				mIsDirty = value;
			}
			if (mOptionGroups == null)
			{
				return;
			}
			foreach (KeyValuePair<string, OptionGroup> mOptionGroup in mOptionGroups)
			{
				mOptionGroup.Value.SetUndirty(value);
			}
		}
	}

	public override int GetHashCode()
	{
		return base.GetHashCode() | mOptionGroups.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is IEngine engine))
		{
			return false;
		}
		string strA = engine.WriteXML();
		string strB = WriteXML();
		return string.Compare(strA, strB, ignoreCase: false) == 0;
	}

	public IOptionsSchema AddOption(string nameOfOptionGroup, string optionName, Type type, bool isOptional, object value)
	{
		string message;
		if (!string.IsNullOrEmpty(message = CheckForValidName(nameOfOptionGroup)))
		{
			throw new Exception(message);
		}
		if (!string.IsNullOrEmpty(message = CheckForValidName(optionName)))
		{
			throw new Exception(message);
		}
		mOptionGroups.TryGetValue(nameOfOptionGroup, out var value2);
		if (value2 == null)
		{
			value2 = new OptionGroup(nameOfOptionGroup);
			mOptionGroups.Add(nameOfOptionGroup, value2);
			IsDirty = true;
		}
		return value2.AddOption(optionName, type, isOptional, value);
	}

	public IOptionsSchema AddListOption<T>(string nameOfOptionGroup, string optionName, Type type, bool isOptional, List<T> list)
	{
		mOptionGroups.TryGetValue(nameOfOptionGroup, out var value);
		if (value == null)
		{
			value = new OptionGroup(nameOfOptionGroup);
			mOptionGroups.Add(nameOfOptionGroup, value);
			IsDirty = true;
		}
		return value.List_CreateOption(optionName, type, isOptional, list);
	}

	public bool GetOptionValue(string nameOfOptionGroup, string optionName, out object optValue)
	{
		optValue = null;
		return GetOptionGroup(nameOfOptionGroup)?.GetValue(optionName, out optValue) ?? false;
	}

	public object GetOptionValue(string nameOfOptionGroup, string optionName)
	{
		if (!GetOptionValue(nameOfOptionGroup, optionName, out var optValue))
		{
			throw new Exception($"The application option {nameOfOptionGroup}-{optionName} cannot be located.");
		}
		return optValue;
	}

	public T GetOptionValue<T>(string nameOfOptionGroup, string optionName)
	{
		object optionValue = GetOptionValue(nameOfOptionGroup, optionName);
		if (!(optionValue is T))
		{
			throw new Exception($"The requested option {nameOfOptionGroup}-{optionName} is not of requested type <T>.");
		}
		return (T)optionValue;
	}

	public bool SetOptionValue(string nameOfOptionGroup, string optionName, object newOptValue)
	{
		return GetOptionGroup(nameOfOptionGroup)?.SetValue(optionName, newOptValue) ?? false;
	}

	public IEnumerable<IOptionGroup> GetOptionGroups()
	{
		if (mOptionGroups == null)
		{
			yield break;
		}
		foreach (KeyValuePair<string, OptionGroup> mOptionGroup in mOptionGroups)
		{
			yield return mOptionGroup.Value;
		}
	}

	public IOptionGroup GetOptionGroup(string nameOfOptionGroup)
	{
		OptionGroup value = null;
		if (!mOptionGroups.TryGetValue(nameOfOptionGroup, out value))
		{
			return null;
		}
		return value;
	}

	public void SetUndirty()
	{
		if (mOptionGroups == null)
		{
			return;
		}
		foreach (KeyValuePair<string, OptionGroup> mOptionGroup in mOptionGroups)
		{
			mOptionGroup.Value.SetUndirty(isDirty: false);
		}
		IsDirty = false;
	}

	bool IEngine.RemoveOptionsGroup(string nameOfOptionGroup)
	{
		if (!mOptionGroups.TryGetValue(nameOfOptionGroup, out var _))
		{
			return false;
		}
		mOptionGroups.Remove(nameOfOptionGroup);
		return true;
	}

	bool IEngine.RemoveOption(string nameOfOptionGroup, string optionName)
	{
		if (!mOptionGroups.TryGetValue(nameOfOptionGroup, out var value))
		{
			return false;
		}
		return value.RemoveOptionDefinition(optionName);
	}

	void IEngine.RemoveAllOptions()
	{
		mOptionGroups.Clear();
	}

	void IEngine.WriteXML(string fileName)
	{
		new XMLLayer().WriteXML(fileName, this);
		IsDirty = false;
	}

	public string WriteXML()
	{
		string result = new XMLLayer().WriteXML(this);
		IsDirty = false;
		return result;
	}

	void IEngine.ReadXML(string fileName)
	{
		try
		{
			new XMLLayer().ReadXML(fileName, this);
			IsDirty = false;
		}
		catch
		{
		}
	}

	void IEngine.ReadXML(TextReader reader)
	{
		new XMLLayer().ReadXML(reader, this);
		IsDirty = false;
	}

	private string CheckForValidName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return $"The '{name}' name cannot be empty or null";
		}
		char[] resvedOptionListCharacters = XMLLayer.ResvedOptionListCharacters;
		foreach (char c in resvedOptionListCharacters)
		{
			if (name.Contains($"{c}"))
			{
				return $"The '{name}' name is not valid since it contains the '{c}' character\n(this character is reserved for internal usage only).";
			}
		}
		return null;
	}
}
