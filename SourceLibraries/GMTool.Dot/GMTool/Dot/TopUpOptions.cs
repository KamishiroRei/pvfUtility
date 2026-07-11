using DevExpress.Mvvm;
using GMTool.Dot.taiwan_billing;

namespace GMTool.Dot;

public class TopUpOptions : ViewModelBase
{
	private int _RechargeOption_Ratio;

	private int _Value;

	public int RechargeOption_Ratio
	{
		get
		{
			if (_RechargeOption_Ratio <= 0)
			{
				_RechargeOption_Ratio = 1000;
			}
			return _RechargeOption_Ratio;
		}
		set
		{
			_RechargeOption_Ratio = value;
			RaisePropertyChanged("RechargeOption_Ratio");
		}
	}

	public int Value
	{
		get
		{
			if (_Value <= 0)
			{
				_Value = 100;
			}
			return _Value;
		}
		set
		{
			_Value = value;
			RaisePropertyChanged("Value");
		}
	}

	public TopUpValueType TopUpValueType
	{
		get
		{
			return GetProperty(() => TopUpValueType);
		}
		set
		{
			SetProperty(() => TopUpValueType, value);
		}
	}

	public TopUpType TopUpType
	{
		get
		{
			return GetProperty(() => TopUpType);
		}
		set
		{
			SetProperty(() => TopUpType, value);
		}
	}

	public TopUpType ClearTopUpType
	{
		get
		{
			return GetProperty(() => ClearTopUpType);
		}
		set
		{
			SetProperty(() => ClearTopUpType, value);
		}
	}
}
