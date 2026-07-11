using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace SevenZip;

[StructLayout(LayoutKind.Explicit)]
internal struct PropVariant
{
	[FieldOffset(0)]
	private ushort _vt;

	[FieldOffset(8)]
	private readonly FILETIME _fileTime;

	[FieldOffset(8)]
	private readonly PropArray _propArray;

	[FieldOffset(8)]
	private IntPtr _value;

	[FieldOffset(8)]
	private uint _uInt32Value;

	[FieldOffset(8)]
	private int _int32Value;

	[FieldOffset(8)]
	private long _int64Value;

	[FieldOffset(8)]
	private ulong _uInt64Value;

	public VarEnum VarType
	{
		private get
		{
			return (VarEnum)_vt;
		}
		set
		{
			_vt = (ushort)value;
		}
	}

	public IntPtr Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
		}
	}

	public uint UInt32Value
	{
		get
		{
			return _uInt32Value;
		}
		set
		{
			_uInt32Value = value;
		}
	}

	public int Int32Value
	{
		get
		{
			return _int32Value;
		}
		set
		{
			_int32Value = value;
		}
	}

	public long Int64Value
	{
		get
		{
			return _int64Value;
		}
		set
		{
			_int64Value = value;
		}
	}

	public ulong UInt64Value
	{
		get
		{
			return _uInt64Value;
		}
		set
		{
			_uInt64Value = value;
		}
	}

	public object Object
	{
		get
		{
			switch (VarType)
			{
			case VarEnum.VT_BSTR:
				return Marshal.PtrToStringBSTR(Value);
			case VarEnum.VT_EMPTY:
				return null;
			case VarEnum.VT_FILETIME:
				try
				{
					return DateTime.FromFileTime(Int64Value);
				}
				catch (ArgumentOutOfRangeException)
				{
					return DateTime.MinValue;
				}
			default:
			{
				GCHandle gCHandle = GCHandle.Alloc(this, GCHandleType.Pinned);
				try
				{
					return Marshal.GetObjectForNativeVariant(gCHandle.AddrOfPinnedObject());
				}
				catch (NotSupportedException)
				{
					return VarType switch
					{
						VarEnum.VT_UI8 => UInt64Value, 
						VarEnum.VT_UI4 => UInt32Value, 
						VarEnum.VT_I8 => Int64Value, 
						VarEnum.VT_I4 => Int32Value, 
						_ => 0, 
					};
				}
				finally
				{
					gCHandle.Free();
				}
			}
			}
		}
	}

	public override bool Equals(object obj)
	{
		if (obj is PropVariant afi)
		{
			return Equals(afi);
		}
		return false;
	}

	private bool Equals(PropVariant afi)
	{
		if (afi.VarType != VarType)
		{
			return false;
		}
		if (VarType != VarEnum.VT_BSTR)
		{
			return afi.Int64Value == Int64Value;
		}
		return afi.Value == Value;
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}

	public override string ToString()
	{
		return "[" + Value + "] " + Int64Value.ToString(CultureInfo.CurrentCulture);
	}

	public static bool operator ==(PropVariant afi1, PropVariant afi2)
	{
		return afi1.Equals(afi2);
	}

	public static bool operator !=(PropVariant afi1, PropVariant afi2)
	{
		return !afi1.Equals(afi2);
	}
}
