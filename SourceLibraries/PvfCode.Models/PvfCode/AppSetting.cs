using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm;
using Newtonsoft.Json;
using PvfCode.Dot.Desktop;
using PvfCode.LoggerBase;
using PvfCode.Models.Enums;
using PvfCode.Models.Images;
using PvfCode.Models.Macro;
using PvfCode.Models.Options;
using PvfCode.Models.Options.AniDesigner;
using PvfCode.Models.Options.Editor;
using PvfCode.Models.Options.Enums;
using PvfCode.Models.Options.GMTool;
using ServiceLocator;
using Utools;

namespace PvfCode;

public class AppSetting : ModelBase
{
	public static string ConfigPwd;

	private PvfFilePreviewOptions pvfFilePreviewOptions;

	private GMToolOptions gmToolOptions;

	private static AppSetting instance;

	private AniDesignerConfig? aniDesigner;

	private LangType? currentLang;

	private LuanguageOptions languageOptions;

	private bool? childWindowAttachmentMainWindow;

	private double fontSize;

	private GameOptions gameOptions;

	private ImagePacks2Options imagePacks2Options;

	private PvfDocumentOptions pvfDocumentOptions;

	private StoreOptions storeOptions;

	private bool uploadScriptFileContentFormatting;

	private WindowSizeConfigOptions windowSizeConfigOptions;

	private MacroGroup macroGroup;

	private BookMarkGroupDto bookMarkGroup;

	private PvfOptions pvfConfig;

	private TextEditConfig editConfig;

	private TreeConfig treeSetting;

	private PathConfigs pathConfig;

	private ClientApiOptions clientApiOptions;

	private PublicSearchServiceOptions publicSearchServiceOptions;

	private ThemeType? nowThemeType;

	private bool? mainWindowIsEnabled;

	private InsertListOrder insertIndependentDropListOrder;

	private ICommand saveCommand;

	public string Version { get; set; }

	public static string AppBasePath
	{
		get
		{
			string text = Path.Combine(AppContext.BaseDirectory, "Options");
			FileHelper.CheckDir(text);
			return text;
		}
	}

	public static string LayoutSavePath => Path.Combine(AppBasePath, "pvfUtility.Layout");

	public PvfFilePreviewOptions PvfFilePreviewOptions
	{
		get
		{
			if (pvfFilePreviewOptions == null)
			{
				pvfFilePreviewOptions = new PvfFilePreviewOptions();
			}
			return pvfFilePreviewOptions;
		}
		set
		{
			pvfFilePreviewOptions = value;
		}
	}

	public GMToolOptions GMToolOptions
	{
		get
		{
			if (gmToolOptions == null)
			{
				gmToolOptions = new GMToolOptions();
			}
			return gmToolOptions;
		}
		set
		{
			gmToolOptions = value;
		}
	}

	public static AppSetting Instance
	{
		get
		{
			if (instance == null)
			{
				instance = LoadSetting();
			}
			return instance;
		}
		set
		{
			instance = value;
		}
	}

	public AniDesignerConfig AniDesigner
	{
		get
		{
			if (aniDesigner == null)
			{
				aniDesigner = new AniDesignerConfig();
			}
			return aniDesigner;
		}
		set
		{
			aniDesigner = value;
			DoNotify("AniDesigner");
		}
	}

	public LangType CurrentLang
	{
		get
		{
			if (!currentLang.HasValue)
			{
				currentLang = LangType.中文简体;
			}
			return currentLang.Value;
		}
		set
		{
			currentLang = value;
			DoNotify("CurrentLang");
		}
	}

	public LuanguageOptions LuanguageOptions
	{
		get
		{
			if (languageOptions == null)
			{
				languageOptions = new LuanguageOptions();
			}
			return languageOptions;
		}
		set
		{
			languageOptions = value;
		}
	}

	public bool ChildWindowAttachmentMainWindow
	{
		get
		{
			if (!childWindowAttachmentMainWindow.HasValue)
			{
				childWindowAttachmentMainWindow = true;
			}
			return childWindowAttachmentMainWindow.Value;
		}
		set
		{
			childWindowAttachmentMainWindow = value;
			DoNotify("ChildWindowAttachmentMainWindow");
		}
	}

	public double FontSize
	{
		get
		{
			if (!(fontSize < 3.0))
			{
				return fontSize;
			}
			return 12.0;
		}
		set
		{
			fontSize = value;
			DoNotify("FontSize");
		}
	}

	public List<double>? FontSizeList { get; set; }

	public bool? FirstTime { get; set; }

	public GameOptions GameOptions
	{
		get
		{
			if (gameOptions == null)
			{
				gameOptions = new GameOptions();
			}
			return gameOptions;
		}
		set
		{
			gameOptions = value;
		}
	}

	public ImagePacks2Options ImagePacks2Options
	{
		get
		{
			if (imagePacks2Options == null)
			{
				imagePacks2Options = new ImagePacks2Options();
			}
			return imagePacks2Options;
		}
		set
		{
			imagePacks2Options = value;
		}
	}

	public PvfDocumentOptions PvfDocumentOptions
	{
		get
		{
			if (pvfDocumentOptions == null)
			{
				pvfDocumentOptions = new PvfDocumentOptions();
			}
			return pvfDocumentOptions;
		}
		set
		{
			pvfDocumentOptions = value;
		}
	}

	public StoreOptions StoreOptions
	{
		get
		{
			if (storeOptions == null)
			{
				storeOptions = new StoreOptions();
			}
			return storeOptions;
		}
		set
		{
			storeOptions = value;
		}
	}

	public string MAC { get; set; }

	public bool UploadScriptFileContentFormatting
	{
		get
		{
			return uploadScriptFileContentFormatting;
		}
		set
		{
			uploadScriptFileContentFormatting = value;
			DoNotify("UploadScriptFileContentFormatting");
		}
	}

	public WindowSizeConfigOptions WindowSizeConfigOptions
	{
		get
		{
			if (windowSizeConfigOptions == null)
			{
				windowSizeConfigOptions = new WindowSizeConfigOptions();
			}
			return windowSizeConfigOptions;
		}
		set
		{
			windowSizeConfigOptions = value;
		}
	}

	public MacroGroup MacroGroup
	{
		get
		{
			if (macroGroup == null)
			{
				macroGroup = new MacroGroup();
			}
			return macroGroup;
		}
		set
		{
			macroGroup = value;
			DoNotify("MacroGroup");
		}
	}

	[JsonIgnore]
	public LoginAccountRes LoginUser { get; set; }

	[JsonIgnore]
	public BookMarkGroupDto BookMarkGroup
	{
		get
		{
			if (bookMarkGroup == null)
			{
				bookMarkGroup = new BookMarkGroupDto();
			}
			return bookMarkGroup;
		}
		set
		{
			bookMarkGroup = value;
		}
	}

	public PvfOptions PvfConfig
	{
		get
		{
			if (pvfConfig == null)
			{
				pvfConfig = new PvfOptions();
			}
			return pvfConfig;
		}
		set
		{
			pvfConfig = value;
		}
	}

	public TextEditConfig EditConfig
	{
		get
		{
			if (editConfig == null)
			{
				editConfig = new TextEditConfig();
			}
			return editConfig;
		}
		set
		{
			editConfig = value;
		}
	}

	public TreeConfig TreeSetting
	{
		get
		{
			if (treeSetting == null)
			{
				treeSetting = new TreeConfig();
			}
			return treeSetting;
		}
		set
		{
			treeSetting = value;
		}
	}

	public PathConfigs PathConfig
	{
		get
		{
			if (pathConfig == null)
			{
				pathConfig = new PathConfigs();
			}
			return pathConfig;
		}
		set
		{
			pathConfig = value;
		}
	}

	public ClientApiOptions ClientApiOptions
	{
		get
		{
			if (clientApiOptions == null)
			{
				clientApiOptions = new ClientApiOptions();
			}
			return clientApiOptions;
		}
		set
		{
			clientApiOptions = value;
		}
	}

	public PublicSearchServiceOptions PublicSearchServiceOptions
	{
		get
		{
			if (publicSearchServiceOptions == null)
			{
				publicSearchServiceOptions = new PublicSearchServiceOptions();
			}
			return publicSearchServiceOptions;
		}
		set
		{
			publicSearchServiceOptions = value;
		}
	}

	[JsonIgnore]
	public string AppName { get; set; }

	public ThemeType NowThemeType
	{
		get
		{
			if (!nowThemeType.HasValue)
			{
				nowThemeType = ThemeType.VS2019Dark;
			}
			return nowThemeType.Value;
		}
		set
		{
			nowThemeType = value;
			DoNotify("NowThemeType");
			TreeSetting.ChangedNowTreeColorConfig();
			EditConfig.ChangedPvfEdiorHighlightingColorOptions();
			PvfFilePreviewOptions.ChangedCurrentColorConfing();
		}
	}

	public bool MainWindowIsEnabled
	{
		get
		{
			if (!mainWindowIsEnabled.HasValue)
			{
				mainWindowIsEnabled = true;
			}
			return mainWindowIsEnabled.Value;
		}
		set
		{
			mainWindowIsEnabled = value;
			DoNotify("MainWindowIsEnabled");
		}
	}

	public InsertListOrder InsertIndependent_drop_ListOrder
	{
		get
		{
			return insertIndependentDropListOrder;
		}
		set
		{
			insertIndependentDropListOrder = value;
			DoNotify("InsertIndependent_drop_ListOrder");
		}
	}

	[JsonIgnore]
	public ICommand SaveCommand
	{
		get
		{
			if (saveCommand == null)
			{
				saveCommand = new DelegateCommand(SaveCommandExecuted);
			}
			return saveCommand;
		}
	}

	public string LangVersion { get; set; }

	private static string GetConfigPath()
	{
		string text = Path.Combine(AppBasePath, "AppConfig.json");
		FileHelper.CheckDir(Path.GetDirectoryName(text));
		return text;
	}

	public void InitMac()
	{
		try
		{
			string text = SystemInfo.GetDiskInfo().FirstOrDefault()?.SerialNumber;
			if (text != null)
			{
				if (MAC != text)
				{
					LoginUser = null;
				}
				MAC = text;
			}
		}
		catch (Exception)
		{
		}
	}

	private static AppSetting LoadSetting()
	{
		try
		{
			OfflineDataStore.EnsureDefaults();
			AppSetting setting;
			if (File.Exists(GetConfigPath()))
			{
				string text = File.ReadAllText(GetConfigPath());
				if (string.IsNullOrEmpty(text))
				{
					setting = new AppSetting();
				}
				else
				{
					setting = text.JsonToObject<AppSetting>() ?? new AppSetting();
				}
			}
			else
			{
				setting = new AppSetting();
			}
			OfflineDataStore.Load(setting);
			setting.LoginUser = null;
			return setting;
		}
		catch (Exception ex)
		{
			string failedPath = GetConfigPath();
			if (File.Exists(failedPath))
			{
				string directoryName = Path.GetDirectoryName(failedPath);
				int num = 0;
				string text2 = Path.Combine(directoryName, $"AppConfig({num}).json");
				while (File.Exists(text2))
				{
					num++;
					text2 = Path.Combine(directoryName, $"AppConfig({num}).json");
				}
				try
				{
					File.Copy(failedPath, text2, overwrite: true);
				}
				catch (Exception)
				{
				}
				MessageBox.Show($"配置文件加载时发生错误：{ex.Message}\r\n\r\n已为您将配置文件备份到：{text2}\r\n\r\n请发送给作者分析修复", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			return new AppSetting();
		}
	}

	public async Task SaveSetting()
	{
		try
		{
			_ = EditConfig.SizeUnitLabel.SelectedItem.DefaultValues;
			OfflineDataStore.Save(this);
			string contents = JsonConvert.SerializeObject(this, Formatting.Indented);
			await File.WriteAllTextAsync(GetConfigPath(), contents);
		}
		catch (Exception)
		{
		}
	}

	public Ilogger GetIlogger()
	{
		return GetService<Ilogger>();
	}

	public IRes GetRes()
	{
		return GetService<IRes>();
	}

	public TServiceContract GetService<TServiceContract>() where TServiceContract : class
	{
		return ServiceLocator.ServiceContainer.Instance.GetService<TServiceContract>();
	}

	public SolidColorBrush ToColor(string htmlColor)
	{
		return new SolidColorBrush((Color)ColorConverter.ConvertFromString(htmlColor));
	}

	private async void SaveCommandExecuted()
	{
		await SaveSetting();
	}

	public AppSetting()
	{
		FontSizeList = new List<double>
		{
			3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0,
			13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 20.0, 22.0, 24.0,
			26.0, 28.0, 30.0, 32.0, 34.0, 36.0, 38.0, 40.0, 44.0, 48.0,
			52.0, 56.0, 60.0, 64.0, 68.0, 72.0, 76.0, 80.0, 88.0, 96.0,
			104.0, 112.0, 120.0, 128.0, 136.0, 144.0
		};
		AppName = "pvfUtility";
	}

	static AppSetting()
	{
		ConfigPwd = "!#$%&*((qsdV2321SDFSD";
	}
}
