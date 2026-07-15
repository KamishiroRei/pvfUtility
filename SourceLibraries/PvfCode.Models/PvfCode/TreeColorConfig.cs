using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm;
using Newtonsoft.Json;
using PvfCode.Models.Enums;

namespace PvfCode;

[JsonObject(MemberSerialization.OptOut)]
public class TreeColorConfig : BindableBase
{
	private SolidColorBrush newFileFlagBrush;

	private SolidColorBrush fileNameBrush;

	private SolidColorBrush itemNameBrush;

	private SolidColorBrush itemCodeBrush;

	private SolidColorBrush commentBrush;

	private SolidColorBrush? selectedBackBrush;

	private SolidColorBrush? focusedBackBrush;

	private SolidColorBrush? rarityColor0;

	private SolidColorBrush? rarityColor1;

	private SolidColorBrush? rarityColor2;

	private SolidColorBrush? rarityColor3;

	private SolidColorBrush? rarityColor4;

	private SolidColorBrush? rarityColor5;

	private ICommand resetCommand;

	public ThemeType ThemeTypeChina { get; set; }

	public SolidColorBrush NewFileFlagBrush
	{
		get
		{
			if (newFileFlagBrush == null)
			{
				newFileFlagBrush = CreateBrush("Red");
			}
			return newFileFlagBrush;
		}
		set
		{
			newFileFlagBrush = value;
			RaisePropertyChanged("NewFileFlagBrush");
		}
	}

	public SolidColorBrush FileNameBrush
	{
		get
		{
			if (fileNameBrush == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					fileNameBrush = CreateBrush("black");
					break;
				case ThemeType.VS2019Dark:
					fileNameBrush = CreateBrush("White");
					break;
				case ThemeType.VS2019Light:
					fileNameBrush = CreateBrush("black");
					break;
				}
			}
			return fileNameBrush;
		}
		set
		{
			fileNameBrush = value;
			RaisePropertyChanged("FileNameBrush");
		}
	}

	public SolidColorBrush ItemNameBrush
	{
		get
		{
			if (itemNameBrush == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					itemNameBrush = CreateBrush("#a31515");
					break;
				case ThemeType.VS2019Dark:
					itemNameBrush = CreateBrush("#af7a66");
					break;
				case ThemeType.VS2019Light:
					itemNameBrush = CreateBrush("#a31515");
					break;
				}
			}
			return itemNameBrush;
		}
		set
		{
			itemNameBrush = value;
			RaisePropertyChanged("ItemNameBrush");
		}
	}

	public SolidColorBrush ItemCodeBrush
	{
		get
		{
			if (itemCodeBrush == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					itemCodeBrush = CreateBrush("#5b2da8");
					break;
				case ThemeType.VS2019Dark:
					itemCodeBrush = CreateBrush("#b5cea8");
					break;
				case ThemeType.VS2019Light:
					itemCodeBrush = CreateBrush("#5b2da8");
					break;
				}
			}
			return itemCodeBrush;
		}
		set
		{
			itemCodeBrush = value;
			RaisePropertyChanged("ItemCodeBrush");
		}
	}

	public SolidColorBrush CommentBrush
	{
		get
		{
			if (commentBrush == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					commentBrush = CreateBrush("gray");
					break;
				case ThemeType.VS2019Dark:
					commentBrush = CreateBrush("gray");
					break;
				case ThemeType.VS2019Light:
					commentBrush = CreateBrush("gray");
					break;
				}
			}
			return commentBrush;
		}
		set
		{
			commentBrush = value;
			RaisePropertyChanged("CommentBrush");
		}
	}

	public SolidColorBrush SelectedBackBrush
	{
		get
		{
			if (selectedBackBrush == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					selectedBackBrush = CreateBrush("#d8daeb");
					break;
				case ThemeType.VS2019Dark:
					selectedBackBrush = CreateBrush("#404040");
					break;
				case ThemeType.VS2019Light:
					selectedBackBrush = CreateBrush("#c9def5");
					break;
				}
			}
			return selectedBackBrush;
		}
		set
		{
			selectedBackBrush = value;
			RaisePropertyChanged("SelectedBackBrush");
		}
	}

	public SolidColorBrush FocusedBackBrush
	{
		get
		{
			if (focusedBackBrush == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					focusedBackBrush = CreateBrush("#d8daeb");
					break;
				case ThemeType.VS2019Dark:
					focusedBackBrush = CreateBrush("#404040");
					break;
				case ThemeType.VS2019Light:
					focusedBackBrush = CreateBrush("#c9def5");
					break;
				}
			}
			return focusedBackBrush;
		}
		set
		{
			focusedBackBrush = value;
			RaisePropertyChanged("FocusedBackBrush");
		}
	}

	public SolidColorBrush RarityColor0
	{
		get
		{
			if (rarityColor0 == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					rarityColor0 = FileNameBrush;
					break;
				case ThemeType.VS2019Dark:
					rarityColor0 = FileNameBrush;
					break;
				case ThemeType.VS2019Light:
					rarityColor0 = FileNameBrush;
					break;
				}
			}
			return rarityColor0;
		}
		set
		{
			rarityColor0 = value;
			RaisePropertyChanged("RarityColor0");
		}
	}

	public SolidColorBrush RarityColor1
	{
		get
		{
			if (rarityColor1 == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					rarityColor1 = CreateBrush("#68D5ED");
					break;
				case ThemeType.VS2019Dark:
					rarityColor1 = CreateBrush("#68D5ED");
					break;
				case ThemeType.VS2019Light:
					rarityColor1 = CreateBrush("#68D5ED");
					break;
				}
			}
			return rarityColor1;
		}
		set
		{
			rarityColor1 = value;
			RaisePropertyChanged("RarityColor1");
		}
	}

	public SolidColorBrush RarityColor2
	{
		get
		{
			if (rarityColor2 == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					rarityColor2 = CreateBrush("#B36BFF");
					break;
				case ThemeType.VS2019Dark:
					rarityColor2 = CreateBrush("#B36BFF");
					break;
				case ThemeType.VS2019Light:
					rarityColor2 = CreateBrush("#B36BFF");
					break;
				}
			}
			return rarityColor2;
		}
		set
		{
			rarityColor2 = value;
			RaisePropertyChanged("RarityColor2");
		}
	}

	public SolidColorBrush RarityColor3
	{
		get
		{
			if (rarityColor3 == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					rarityColor3 = CreateBrush("#DC007E");
					break;
				case ThemeType.VS2019Dark:
					rarityColor3 = CreateBrush("#DC007E");
					break;
				case ThemeType.VS2019Light:
					rarityColor3 = CreateBrush("#DC007E");
					break;
				}
			}
			return rarityColor3;
		}
		set
		{
			rarityColor3 = value;
			RaisePropertyChanged("RarityColor3");
		}
	}

	public SolidColorBrush RarityColor4
	{
		get
		{
			if (rarityColor4 == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					rarityColor4 = CreateBrush("#FFB100");
					break;
				case ThemeType.VS2019Dark:
					rarityColor4 = CreateBrush("#FFB100");
					break;
				case ThemeType.VS2019Light:
					rarityColor4 = CreateBrush("#FFB100");
					break;
				}
			}
			return rarityColor4;
		}
		set
		{
			rarityColor4 = value;
			RaisePropertyChanged("RarityColor4");
		}
	}

	public SolidColorBrush RarityColor5
	{
		get
		{
			if (rarityColor5 == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					rarityColor5 = CreateBrush("#FF6666");
					break;
				case ThemeType.VS2019Dark:
					rarityColor5 = CreateBrush("#FF6666");
					break;
				case ThemeType.VS2019Light:
					rarityColor5 = CreateBrush("#FF6666");
					break;
				}
			}
			return rarityColor5;
		}
		set
		{
			rarityColor5 = value;
			RaisePropertyChanged("RarityColor5");
		}
	}

	[JsonIgnore]
	public ICommand ResetCommand
	{
		get
		{
			if (resetCommand == null)
			{
				resetCommand = new DelegateCommand(Reset);
			}
			return resetCommand;
		}
	}

	public TreeColorConfig(ThemeType themeTypeChina)
	{
		ThemeTypeChina = themeTypeChina;
	}

	private SolidColorBrush CreateBrush(string color)
	{
		return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
	}

	public void Reset()
	{
		NewFileFlagBrush = null;
		FileNameBrush = null;
		ItemNameBrush = null;
		ItemCodeBrush = null;
		CommentBrush = null;
		FocusedBackBrush = null;
		SelectedBackBrush = null;
		RarityColor0 = null;
		RarityColor1 = null;
		RarityColor2 = null;
		RarityColor3 = null;
		RarityColor4 = null;
		RarityColor5 = null;
	}
}
