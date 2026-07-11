using System.Runtime.InteropServices;

namespace SevenZip;

[ComImport]
[Guid("23170F69-40C1-278A-0000-000600300000")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IArchiveOpenVolumeCallback
{
	[PreserveSig]
	int GetProperty(ItemPropId propId, ref PropVariant value);

	[PreserveSig]
	int GetStream([MarshalAs(UnmanagedType.LPWStr)] string name, [MarshalAs(UnmanagedType.Interface)] out IInStream inStream);
}
