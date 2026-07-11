using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Security;
using SettingsModel.Interfaces;
using SettingsModel.Models.XML.Converters;

namespace SettingsModel.Models.XML;

internal class XMLLayer
{
	public static readonly char[] ResvedOptionListCharacters = new char[3] { '$', '{', '}' };

	public void WriteXML(string fileName, IEngine engine)
	{
		try
		{
			DataSet dataSet = ConvertFromModelToDataSet(engine);
			try
			{
				dataSet.WriteXml(fileName);
			}
			finally
			{
				((IDisposable)dataSet)?.Dispose();
			}
		}
		catch (Exception)
		{
			throw;
		}
	}

	public string WriteXML(IEngine engine)
	{
		try
		{
			using StringWriter stringWriter = new StringWriter();
			DataSet dataSet = ConvertFromModelToDataSet(engine);
			try
			{
				dataSet.WriteXml(stringWriter);
			}
			finally
			{
				((IDisposable)dataSet)?.Dispose();
			}
			return stringWriter.ToString();
		}
		catch (Exception)
		{
			throw;
		}
	}

	public void ReadXML(string fileName, IEngine engine)
	{
		try
		{
			if (!File.Exists(fileName))
			{
				return;
			}
			DataSet dataSet = ConvertFromModelToDataSet(engine);
			try
			{
				dataSet.ReadXml(fileName);
				ConvertFromDataSetToModel(engine, dataSet);
			}
			finally
			{
				((IDisposable)dataSet)?.Dispose();
			}
		}
		catch (Exception)
		{
			throw;
		}
	}

	public void ReadXML(TextReader reader, IEngine engine)
	{
		try
		{
			DataSet dataSet = ConvertFromModelToDataSet(engine);
			try
			{
				dataSet.ReadXml(reader);
				ConvertFromDataSetToModel(engine, dataSet);
			}
			finally
			{
				((IDisposable)dataSet)?.Dispose();
			}
		}
		catch (Exception)
		{
			throw;
		}
	}

	private DataSet ConvertFromModelToDataSet(IEngine engine)
	{
		DataSet dataSet = new DataSet();
		AlternativeDataTypeHandler alternativeDataTypeHandler = new AlternativeDataTypeHandler();
		foreach (IOptionGroup optionGroup in engine.GetOptionGroups())
		{
			DataTable dataTable = CreateTable(optionGroup);
			dataSet.Tables.Add(dataTable);
			DataRow dataRow = dataTable.NewRow();
			foreach (IOptionsSchema optionDefinition in optionGroup.GetOptionDefinitions())
			{
				IAlternativeDataTypeHandler alternativeDataTypeHandler2 = alternativeDataTypeHandler.FindHandler(optionDefinition.TypeOfValue);
				if (optionDefinition.SchemaType == OptionSchemaType.SingleValue)
				{
					if (alternativeDataTypeHandler2 != null)
					{
						dataRow[optionDefinition.OptionName] = alternativeDataTypeHandler2.Convert(optionDefinition.Value as SecureString);
					}
					else
					{
						dataRow[optionDefinition.OptionName] = optionDefinition.Value;
					}
				}
				else
				{
					DataTable table = CreateListTable(CreateListItemTableName(optionGroup, optionDefinition), optionDefinition, alternativeDataTypeHandler2, dataRow);
					dataSet.Tables.Add(table);
				}
			}
			if (dataRow.ItemArray.Count() > 0)
			{
				dataTable.Rows.Add(dataRow);
			}
		}
		return dataSet;
	}

	private void ConvertFromDataSetToModel(IEngine engine, DataSet dataSet)
	{
		AlternativeDataTypeHandler alternativeDataTypeHandler = new AlternativeDataTypeHandler();
		foreach (DataTable table in dataSet.Tables)
		{
			string optionName;
			IOptionGroup optionGroup = FindOptionsGroup(engine, table.TableName, out optionName);
			if (!string.IsNullOrEmpty(optionName))
			{
				if (table.Rows.Count > 0)
				{
					optionGroup.List_Clear(optionName);
					for (int i = 0; i < table.Rows.Count; i++)
					{
						optionGroup.SetValue(optionName, table.Rows[i].ItemArray[0]);
					}
				}
			}
			else
			{
				if (table.Rows.Count < 2)
				{
					continue;
				}
				for (int j = 0; j < table.Columns.Count; j++)
				{
					IOptionsSchema optionDefinition = optionGroup.GetOptionDefinition(table.Columns[j].ColumnName);
					IAlternativeDataTypeHandler alternativeDataTypeHandler2 = alternativeDataTypeHandler.FindHandler(optionDefinition.TypeOfValue);
					if (alternativeDataTypeHandler2 != null)
					{
						object newValue = alternativeDataTypeHandler2.ConvertBack(table.Rows[1].ItemArray[j] as string);
						optionGroup.SetValue(table.Columns[j].ColumnName, newValue);
					}
					else
					{
						optionGroup.SetValue(table.Columns[j].ColumnName, table.Rows[1].ItemArray[j]);
					}
				}
			}
		}
		dataSet = null;
	}

	private static DataTable CreateTable(IOptionGroup tableSchema)
	{
		DataTable dataTable = new DataTable(tableSchema.Name);
		foreach (IOptionsSchema optionDefinition in tableSchema.GetOptionDefinitions())
		{
			DataColumn dataColumn = new DataColumn(optionDefinition.OptionName, (optionDefinition.TypeOfValue == typeof(SecureString)) ? typeof(string) : optionDefinition.TypeOfValue);
			dataColumn.AllowDBNull = optionDefinition.IsOptional;
			dataTable.Columns.Add(dataColumn);
		}
		return dataTable;
	}

	private static DataTable CreateListTable(string tableName, IOptionsSchema columnSchema, IAlternativeDataTypeHandler handler, DataRow masterRow)
	{
		DataTable dataTable = new DataTable(tableName);
		DataColumn dataColumn = new DataColumn(columnSchema.OptionName, (columnSchema.TypeOfValue == typeof(SecureString)) ? typeof(string) : columnSchema.TypeOfValue);
		dataColumn.AllowDBNull = columnSchema.IsOptional;
		dataTable.Columns.Add(dataColumn);
		bool flag = true;
		foreach (object item in columnSchema.List_GetListOfValues())
		{
			if (flag)
			{
				flag = false;
				if (handler != null)
				{
					masterRow[columnSchema.OptionName] = handler.Convert(item as SecureString);
				}
				else
				{
					masterRow[columnSchema.OptionName] = item;
				}
			}
			DataRow dataRow = dataTable.NewRow();
			if (handler != null)
			{
				dataRow[columnSchema.OptionName] = handler.Convert(item as SecureString);
			}
			else
			{
				dataRow[columnSchema.OptionName] = item;
			}
			dataTable.Rows.Add(dataRow);
		}
		return dataTable;
	}

	private IOptionGroup FindOptionsGroup(IEngine engine, string tableName, out string optionName)
	{
		string groupName = string.Empty;
		optionName = string.Empty;
		IOptionGroup optionGroup = engine.GetOptionGroup(tableName);
		if (optionGroup == null)
		{
			if (TryResolveOptionsGroupAndOptionName(tableName, out groupName, out optionName))
			{
				throw new Exception($"Unknown item detected. Cannot resolve {tableName}.");
			}
			optionGroup = engine.GetOptionGroup(groupName);
		}
		if (optionGroup == null)
		{
			throw new Exception($"Cannot resolve table name: {tableName}");
		}
		return optionGroup;
	}

	private string CreateListItemTableName(IOptionGroup optionsGroup, IOptionsSchema optionSchema)
	{
		return "${" + optionsGroup.Name + "}${" + optionSchema.OptionName + "}";
	}

	private bool TryResolveOptionsGroupAndOptionName(string name, out string groupName, out string optionName)
	{
		groupName = string.Empty;
		optionName = string.Empty;
		string[] array = name.Split(ResvedOptionListCharacters);
		foreach (string text in array)
		{
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			if (string.IsNullOrEmpty(groupName))
			{
				groupName = text;
				continue;
			}
			if (!string.IsNullOrEmpty(optionName))
			{
				throw new NotSupportedException(name);
			}
			optionName = text;
		}
		if (string.IsNullOrEmpty(optionName) || string.IsNullOrEmpty(groupName))
		{
			throw new NotSupportedException(name);
		}
		return false;
	}
}
