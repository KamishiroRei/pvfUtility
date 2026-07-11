using System;

namespace SettingsModel.Models.XML.Converters;

internal interface IAlternativeDataTypeHandler
{
	Type SourceDataType { get; }

	Type TargetDataType { get; }

	object Convert(object objectInput);

	object ConvertBack(object objectEncryptedData);
}
