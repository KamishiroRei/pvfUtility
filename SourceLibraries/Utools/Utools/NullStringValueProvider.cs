using System.Data;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Utools;

internal sealed class NullStringValueProvider : IValueProvider
{
	private readonly PropertyInfo property;

	public NullStringValueProvider(PropertyInfo property)
	{
		this.property = property;
	}

	public void SetValue(object target, object value)
	{
		property.SetValue(target, value);
	}

	public object GetValue(object target)
	{
		object value = property.GetValue(target);
		if (property.PropertyType == typeof(string) && value == null)
		{
			value = string.Empty;
		}
		if (property.PropertyType == typeof(DataTable))
		{
			DataTable dataTable = (DataTable)value;
			foreach (DataRow row in dataTable.Rows)
			{
				foreach (DataColumn column in dataTable.Columns)
				{
					if (column.DataType == typeof(string))
					{
						row[column.ColumnName] = (row[column.ColumnName] as string) ?? string.Empty;
					}
				}
			}
			value = dataTable;
		}
		return value;
	}
}
