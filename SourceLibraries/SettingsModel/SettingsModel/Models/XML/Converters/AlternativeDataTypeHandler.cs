using System;
using System.Collections.Generic;
using System.Security;

namespace SettingsModel.Models.XML.Converters;

internal class AlternativeDataTypeHandler
{
	private readonly Dictionary<Type, IAlternativeDataTypeHandler> converters;

	public AlternativeDataTypeHandler()
	{
		converters = new Dictionary<Type, IAlternativeDataTypeHandler>();
		converters.Add(typeof(SecureString), new SecureStringHandler());
	}

	public IAlternativeDataTypeHandler FindHandler(Type typeOfDataType2Handle)
	{
		IAlternativeDataTypeHandler value = null;
		try
		{
			converters.TryGetValue(typeOfDataType2Handle, out value);
		}
		catch
		{
		}
		return value;
	}
}
