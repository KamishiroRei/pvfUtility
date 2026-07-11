using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using UnitComboLib.Command;
using UnitComboLib.Local;
using UnitComboLib.Models;
using UnitComboLib.Models.Unit;

namespace UnitComboLib.ViewModels;

public class UnitViewModel : BaseViewModel, IDataErrorInfo, IUnitViewModel
{
	public delegate void ScreenPointsChangedDelegate();

	private string _ValueTip = string.Empty;

	private double _Value;

	private string _StrValue = "0.0";

	private Converter _UnitConverter;

	private RelayCommand<Itemkey> _SetSelectedItemCommand;

	private string _MaxStringLengthValue = "#####";

	private const double MinFontSizeValue = 2.0;

	private const double MaxFontSizeValue = 399.0;

	private const double MinPercentageSizeValue = 24.0;

	private const double MaxPercentageSizeValue = 3325.0;

	private ListItem _SelectedItem { get; set; }

	private ObservableCollection<ListItem> _UnitList { get; set; }

	public int ScreenPoints
	{
		get
		{
			if (SelectedItem != null)
			{
				return (int)_UnitConverter.Convert(SelectedItem.Key, _Value, Itemkey.ScreenFontPoints);
			}
			return 12;
		}
		set
		{
			if (SelectedItem == null)
			{
				return;
			}
			if (SelectedItem.Key == Itemkey.ScreenFontPoints)
			{
				if ((double)value != Value)
				{
					Value = value;
				}
			}
			else if (value != (int)_UnitConverter.Convert(SelectedItem.Key, _Value, Itemkey.ScreenFontPoints))
			{
				Value = (int)_UnitConverter.Convert(Itemkey.ScreenFontPoints, value, SelectedItem.Key);
			}
			this.EventScreenPointsChanged?.Invoke();
		}
	}

	public ObservableCollection<ListItem> UnitList
	{
		get
		{
			if (_UnitList != null && _UnitList.Count > 1)
			{
				_UnitList.RemoveAt(1);
			}
			return _UnitList;
		}
	}

	public ListItem SelectedItem
	{
		get
		{
			return _SelectedItem;
		}
		set
		{
			if (_SelectedItem != value)
			{
				_SelectedItem = value;
				RaisePropertyChanged(() => SelectedItem);
				RaisePropertyChanged(() => ScreenPoints);
				RaisePropertyChanged(() => MinValue);
				RaisePropertyChanged(() => MaxValue);
			}
		}
	}

	public string Error => null;

	public string this[string propertyName]
	{
		get
		{
			if (propertyName == "StringValue")
			{
				if (string.IsNullOrEmpty(_StrValue))
				{
					return SetToolTip(Strings.Integer_Contain_ErrorMessage);
				}
				if (double.TryParse(_StrValue, out var result))
				{
					if (IsDoubleWithinRange(result, SelectedItem.Key, out var message))
					{
						Value = result;
						return SetToolTip(null);
					}
					return SetToolTip(message);
				}
				return SetToolTip(Strings.Integer_Conversion_ErrorMessage);
			}
			return SetToolTip(null);
		}
	}

	public string ValueTip
	{
		get
		{
			return _ValueTip;
		}
		protected set
		{
			if (_ValueTip != value)
			{
				_ValueTip = value;
				RaisePropertyChanged(() => ValueTip);
			}
		}
	}

	public string MaxStringLengthValue
	{
		get
		{
			return _MaxStringLengthValue;
		}
		set
		{
			if (_MaxStringLengthValue != value)
			{
				_MaxStringLengthValue = value;
				RaisePropertyChanged(() => MaxStringLengthValue);
			}
		}
	}

	public string StringValue
	{
		get
		{
			return _StrValue;
		}
		set
		{
			if (_StrValue != value)
			{
				_StrValue = value;
				RaisePropertyChanged(() => StringValue);
				this.EventScreenPointsChanged?.Invoke();
			}
		}
	}

	public double Value
	{
		get
		{
			return _Value;
		}
		set
		{
			if (_Value != value)
			{
				_Value = value;
				_StrValue = $"{_Value:0}";
				RaisePropertyChanged(() => Value);
				RaisePropertyChanged(() => StringValue);
				RaisePropertyChanged(() => ScreenPoints);
			}
		}
	}

	public double MinValue => GetMinValue(SelectedItem.Key);

	public double MaxValue => GetMaxValue(SelectedItem.Key);

	public ICommand SetSelectedItemCommand
	{
		get
		{
			if (_SetSelectedItemCommand == null)
			{
				_SetSelectedItemCommand = new RelayCommand<Itemkey>(delegate(Itemkey p)
				{
					SetSelectedItemExecuted(p);
				}, (Itemkey p) => true);
			}
			return _SetSelectedItemCommand;
		}
	}

	public event ScreenPointsChangedDelegate EventScreenPointsChanged;

	public UnitViewModel(IList<ListItem> list, Converter unitConverter, int defaultIndex = 0, double defaultValue = 100.0)
	{
		_UnitList = new ObservableCollection<ListItem>(list);
		_SelectedItem = _UnitList[defaultIndex];
		_UnitConverter = unitConverter;
		_Value = defaultValue;
		_StrValue = $"{_Value:0}";
	}

	protected UnitViewModel()
	{
	}

	private object SetSelectedItemExecuted(Itemkey unitKey)
	{
		ListItem listItem = _UnitList.SingleOrDefault((ListItem i) => i.Key == unitKey);
		if (listItem != null)
		{
			if (double.TryParse(_StrValue, out var result))
			{
				double num = _UnitConverter.Convert(SelectedItem.Key, result, listItem.Key);
				if (num < GetMinValue(unitKey))
				{
					num = GetMinValue(unitKey);
				}
				else if (num > GetMaxValue(unitKey))
				{
					num = GetMaxValue(unitKey);
				}
				Value = num;
				_StrValue = $"{_Value:0}";
				SelectedItem = listItem;
				ValueTip = SetUnitRangeMessage(unitKey);
			}
			RaisePropertyChanged(() => Value);
			RaisePropertyChanged(() => MinValue);
			RaisePropertyChanged(() => MaxValue);
			RaisePropertyChanged(() => StringValue);
			RaisePropertyChanged(() => SelectedItem);
		}
		return null;
	}

	private bool IsDoubleWithinRange(double doubleValue, Itemkey unitToConvert, out string message)
	{
		message = SetUnitRangeMessage(unitToConvert);
		switch (unitToConvert)
		{
		case Itemkey.ScreenFontPoints:
			if (doubleValue < 2.0)
			{
				return false;
			}
			if (doubleValue > 399.0)
			{
				return false;
			}
			return true;
		case Itemkey.ScreenPercent:
			if (doubleValue < 24.0)
			{
				return false;
			}
			if (doubleValue > 3325.0)
			{
				return false;
			}
			return true;
		default:
			return false;
		}
	}

	private string SetUnitRangeMessage(Itemkey unit)
	{
		return unit switch
		{
			Itemkey.ScreenFontPoints => FontSizeErrorTip(), 
			Itemkey.ScreenPercent => PercentSizeErrorTip(), 
			_ => throw new NotSupportedException(unit.ToString()), 
		};
	}

	private string FontSizeErrorTip()
	{
		return string.Format(Strings.Enter_Font_Size_InRange_Message, $"{2.0:0}", $"{399.0:0}");
	}

	private string PercentSizeErrorTip()
	{
		return string.Format(Strings.Enter_Percent_Size_InRange_Message, $"{24.0:0}", $"{3325.0:0}");
	}

	private string SetToolTip(string strError)
	{
		string valueTip = string.Format(Strings.Enter_Percent_Font_Size_InRange_Message, $"{24.0:0}", $"{3325.0:0}", $"{2.0:0}", $"{399.0:0}");
		if (strError == null)
		{
			if (SelectedItem != null)
			{
				switch (SelectedItem.Key)
				{
				case Itemkey.ScreenFontPoints:
					ValueTip = FontSizeErrorTip();
					break;
				case Itemkey.ScreenPercent:
					ValueTip = PercentSizeErrorTip();
					break;
				default:
					ValueTip = valueTip;
					break;
				}
			}
			else
			{
				ValueTip = valueTip;
			}
		}
		else
		{
			ValueTip = strError;
		}
		return strError;
	}

	private double GetMinValue(Itemkey key)
	{
		if (key == Itemkey.ScreenFontPoints)
		{
			return 2.0;
		}
		return 24.0;
	}

	private double GetMaxValue(Itemkey key)
	{
		if (key == Itemkey.ScreenFontPoints)
		{
			return 399.0;
		}
		return 3325.0;
	}
}
