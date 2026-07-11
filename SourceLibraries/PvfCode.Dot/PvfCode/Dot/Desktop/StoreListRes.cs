using System;
using PvfCode.Dot.Desktop.Enums;

namespace PvfCode.Dot.Desktop;

public class StoreListRes : ModelBase, ICloneable
{
	private StoreType _Type;

	private string _Title;

	private string _Description;

	private string _DetailedInstructions;

	public StoreType Type
	{
		get
		{
			return _Type;
		}
		set
		{
			_Type = value;
			DoNotify("Type");
		}
	}

	public MacroType? MacroType { get; set; }

	public string Title
	{
		get
		{
			return _Title;
		}
		set
		{
			_Title = value;
			DoNotify("Title");
		}
	}

	public object Data { get; set; }

	public string Description
	{
		get
		{
			return _Description;
		}
		set
		{
			_Description = value;
			DoNotify("Description");
		}
	}

	public string DetailedInstructions
	{
		get
		{
			if (string.IsNullOrEmpty(_DetailedInstructions))
			{
				_DetailedInstructions = "//您的书签详细说明\r\n//为鼓励作者共享 该处允许适当投放广告";
			}
			return _DetailedInstructions;
		}
		set
		{
			_DetailedInstructions = value;
			DoNotify("DetailedInstructions");
		}
	}

	public object Clone()
	{
		return MemberwiseClone();
	}

	public StoreListRes CloneData()
	{
		return (StoreListRes)Clone();
	}

	public StoreListRes CloneData(object data)
	{
		StoreListRes obj = (StoreListRes)Clone();
		obj.Data = data;
		return obj;
	}
}
