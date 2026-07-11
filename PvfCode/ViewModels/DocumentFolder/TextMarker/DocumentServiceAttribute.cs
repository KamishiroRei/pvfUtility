using System;

namespace PvfCode.ViewModels.DocumentFolder.TextMarker;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class DocumentServiceAttribute : Attribute
{
	public DocumentServiceAttribute()
	{
	}
}
