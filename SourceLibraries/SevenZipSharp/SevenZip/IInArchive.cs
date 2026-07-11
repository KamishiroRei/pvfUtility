using System.Runtime.InteropServices;

namespace SevenZip;

[ComImport]
[Guid("23170F69-40C1-278A-0000-000600600000")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IInArchive
{
	[PreserveSig]
	int Open(IInStream stream, [In] ref ulong maxCheckStartPosition, [MarshalAs(UnmanagedType.Interface)] IArchiveOpenCallback openArchiveCallback);

	void Close();

	uint GetNumberOfItems();

	void GetProperty(uint index, ItemPropId propId, ref PropVariant value);

	[PreserveSig]
	int Extract([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] uint[] indexes, uint numItems, int testMode, [MarshalAs(UnmanagedType.Interface)] IArchiveExtractCallback extractCallback);

	void GetArchiveProperty(ItemPropId propId, ref PropVariant value);

	uint GetNumberOfProperties();

	void GetPropertyInfo(uint index, [MarshalAs(UnmanagedType.BStr)] out string name, out ItemPropId propId, out ushort varType);

	uint GetNumberOfArchiveProperties();

	void GetArchivePropertyInfo(uint index, [MarshalAs(UnmanagedType.BStr)] out string name, out ItemPropId propId, out ushort varType);
}
