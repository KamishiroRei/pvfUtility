using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

	[CompilerGenerated]
	private string EHSYbKoRS;

	private PvfFilePreviewOptions dBRJ2UuIs;

	private GMToolOptions OB2drtANm;

	private static AppSetting BpI1YHVB4;

	private AniDesignerConfig? fDfGO1HnE;

	private LangType? OJdOaEIOi;

	private LuanguageOptions tHJrh0jY4;

	private bool? WGTekXa67;

	private double sDpjtZBdq;

	[CompilerGenerated]
	private List<double>? SecxiJm0v;

	[CompilerGenerated]
	private bool? rLUmFjWyu;

	private GameOptions qFnSwiUVO;

	private ImagePacks2Options AoIBlg1ec;

	private PvfDocumentOptions GGB4mlnIw;

	private StoreOptions ddZCGW7eZ;

	[CompilerGenerated]
	private string KX5vweOlX;

	private bool IZcbgZO7g;

	private WindowSizeConfigOptions hn3V5f4QM;

	private MacroGroup iEcPM3FqD;

	[CompilerGenerated]
	private LoginAccountRes gVIFXSdt9;

	private BookMarkGroupDto inrXQ1AyK;

	private PvfOptions yEANefHAh;

	private TextEditConfig WXLiqPPXl;

	private TreeConfig zB8Mj20GJ;

	private PathConfigs av6aSufEK;

	private ClientApiOptions Cr6IoWBAp;

	private PublicSearchServiceOptions aaXURtJOi;

	[CompilerGenerated]
	private string vf5lRd8Fh;

	private ThemeType? pWuf2AIIg;

	private bool? KBGhlOGL0;

	private InsertListOrder k4gTDxt4T;

	private ICommand g2I0ZxSLo;

	[CompilerGenerated]
	private string ylVsWX1DC;

	public string Version
	{
		[CompilerGenerated]
		get
		{
			return EHSYbKoRS;
		}
		[CompilerGenerated]
		set
		{
			EHSYbKoRS = value;
		}
	}

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
			if (dBRJ2UuIs == null)
			{
				dBRJ2UuIs = new PvfFilePreviewOptions();
			}
			return dBRJ2UuIs;
		}
		set
		{
			dBRJ2UuIs = value;
		}
	}

	public GMToolOptions GMToolOptions
	{
		get
		{
			if (OB2drtANm == null)
			{
				OB2drtANm = new GMToolOptions();
			}
			return OB2drtANm;
		}
		set
		{
			OB2drtANm = value;
		}
	}

	public static AppSetting Instance
	{
		get
		{
			if (BpI1YHVB4 == null)
			{
				BpI1YHVB4 = UrM7ft5DD();
			}
			return BpI1YHVB4;
		}
		set
		{
			BpI1YHVB4 = value;
		}
	}

	public AniDesignerConfig AniDesigner
	{
		get
		{
			if (fDfGO1HnE == null)
			{
				fDfGO1HnE = new AniDesignerConfig();
			}
			return fDfGO1HnE;
		}
		set
		{
			fDfGO1HnE = value;
			DoNotify("AniDesigner");
		}
	}

	public LangType CurrentLang
	{
		get
		{
			if (!OJdOaEIOi.HasValue)
			{
				OJdOaEIOi = LangType.中文简体;
			}
			return OJdOaEIOi.Value;
		}
		set
		{
			OJdOaEIOi = value;
			DoNotify("CurrentLang");
		}
	}

	public LuanguageOptions LuanguageOptions
	{
		get
		{
			if (tHJrh0jY4 == null)
			{
				tHJrh0jY4 = new LuanguageOptions();
			}
			return tHJrh0jY4;
		}
		set
		{
			tHJrh0jY4 = value;
		}
	}

	public bool ChildWindowAttachmentMainWindow
	{
		get
		{
			if (!WGTekXa67.HasValue)
			{
				WGTekXa67 = true;
			}
			return WGTekXa67.Value;
		}
		set
		{
			WGTekXa67 = value;
			DoNotify("ChildWindowAttachmentMainWindow");
		}
	}

	public double FontSize
	{
		get
		{
			if (!(sDpjtZBdq < 3.0))
			{
				return sDpjtZBdq;
			}
			return 12.0;
		}
		set
		{
			sDpjtZBdq = value;
			DoNotify("FontSize");
		}
	}

	public List<double>? FontSizeList
	{
		[CompilerGenerated]
		get
		{
			return SecxiJm0v;
		}
		[CompilerGenerated]
		set
		{
			SecxiJm0v = value;
		}
	}

	public bool? FirstTime
	{
		[CompilerGenerated]
		get
		{
			return rLUmFjWyu;
		}
		[CompilerGenerated]
		set
		{
			rLUmFjWyu = value;
		}
	}

	public GameOptions GameOptions
	{
		get
		{
			if (qFnSwiUVO == null)
			{
				qFnSwiUVO = new GameOptions();
			}
			return qFnSwiUVO;
		}
		set
		{
			qFnSwiUVO = value;
		}
	}

	public ImagePacks2Options ImagePacks2Options
	{
		get
		{
			if (AoIBlg1ec == null)
			{
				AoIBlg1ec = new ImagePacks2Options();
			}
			return AoIBlg1ec;
		}
		set
		{
			AoIBlg1ec = value;
		}
	}

	public PvfDocumentOptions PvfDocumentOptions
	{
		get
		{
			if (GGB4mlnIw == null)
			{
				GGB4mlnIw = new PvfDocumentOptions();
			}
			return GGB4mlnIw;
		}
		set
		{
			GGB4mlnIw = value;
		}
	}

	public StoreOptions StoreOptions
	{
		get
		{
			if (ddZCGW7eZ == null)
			{
				ddZCGW7eZ = new StoreOptions();
			}
			return ddZCGW7eZ;
		}
		set
		{
			ddZCGW7eZ = value;
		}
	}

	public string MAC
	{
		[CompilerGenerated]
		get
		{
			return KX5vweOlX;
		}
		[CompilerGenerated]
		set
		{
			KX5vweOlX = value;
		}
	}

	public bool UploadScriptFileContentFormatting
	{
		get
		{
			return IZcbgZO7g;
		}
		set
		{
			IZcbgZO7g = value;
			DoNotify("UploadScriptFileContentFormatting");
		}
	}

	public WindowSizeConfigOptions WindowSizeConfigOptions
	{
		get
		{
			if (hn3V5f4QM == null)
			{
				hn3V5f4QM = new WindowSizeConfigOptions();
			}
			return hn3V5f4QM;
		}
		set
		{
			hn3V5f4QM = value;
		}
	}

	public MacroGroup MacroGroup
	{
		get
		{
			if (iEcPM3FqD == null)
			{
				iEcPM3FqD = new MacroGroup();
			}
			return iEcPM3FqD;
		}
		set
		{
			iEcPM3FqD = value;
			DoNotify("MacroGroup");
		}
	}

	[JsonIgnore]
	public LoginAccountRes LoginUser
	{
		[CompilerGenerated]
		get
		{
			return gVIFXSdt9;
		}
		[CompilerGenerated]
		set
		{
			gVIFXSdt9 = value;
		}
	}

	[JsonIgnore]
	public BookMarkGroupDto BookMarkGroup
	{
		get
		{
			if (inrXQ1AyK == null)
			{
				inrXQ1AyK = new BookMarkGroupDto();
			}
			return inrXQ1AyK;
		}
		set
		{
			inrXQ1AyK = value;
		}
	}

	public PvfOptions PvfConfig
	{
		get
		{
			if (yEANefHAh == null)
			{
				yEANefHAh = new PvfOptions();
			}
			return yEANefHAh;
		}
		set
		{
			yEANefHAh = value;
		}
	}

	public TextEditConfig EditConfig
	{
		get
		{
			if (WXLiqPPXl == null)
			{
				WXLiqPPXl = new TextEditConfig();
			}
			return WXLiqPPXl;
		}
		set
		{
			WXLiqPPXl = value;
		}
	}

	public TreeConfig TreeSetting
	{
		get
		{
			if (zB8Mj20GJ == null)
			{
				zB8Mj20GJ = new TreeConfig();
			}
			return zB8Mj20GJ;
		}
		set
		{
			zB8Mj20GJ = value;
		}
	}

	public PathConfigs PathConfig
	{
		get
		{
			if (av6aSufEK == null)
			{
				av6aSufEK = new PathConfigs();
			}
			return av6aSufEK;
		}
		set
		{
			av6aSufEK = value;
		}
	}

	public ClientApiOptions ClientApiOptions
	{
		get
		{
			if (Cr6IoWBAp == null)
			{
				Cr6IoWBAp = new ClientApiOptions();
			}
			return Cr6IoWBAp;
		}
		set
		{
			Cr6IoWBAp = value;
		}
	}

	public PublicSearchServiceOptions PublicSearchServiceOptions
	{
		get
		{
			if (aaXURtJOi == null)
			{
				aaXURtJOi = new PublicSearchServiceOptions();
			}
			return aaXURtJOi;
		}
		set
		{
			aaXURtJOi = value;
		}
	}

	[JsonIgnore]
	public string AppName
	{
		[CompilerGenerated]
		get
		{
			return vf5lRd8Fh;
		}
		[CompilerGenerated]
		set
		{
			vf5lRd8Fh = value;
		}
	}

	public ThemeType NowThemeType
	{
		get
		{
			if (!pWuf2AIIg.HasValue)
			{
				pWuf2AIIg = ThemeType.VS2019Dark;
			}
			return pWuf2AIIg.Value;
		}
		set
		{
			pWuf2AIIg = value;
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
			if (!KBGhlOGL0.HasValue)
			{
				KBGhlOGL0 = true;
			}
			return KBGhlOGL0.Value;
		}
		set
		{
			KBGhlOGL0 = value;
			DoNotify("MainWindowIsEnabled");
		}
	}

	public InsertListOrder InsertIndependent_drop_ListOrder
	{
		get
		{
			return k4gTDxt4T;
		}
		set
		{
			k4gTDxt4T = value;
			DoNotify("InsertIndependent_drop_ListOrder");
		}
	}

	[JsonIgnore]
	public ICommand SaveCommand
	{
		get
		{
			if (g2I0ZxSLo == null)
			{
				g2I0ZxSLo = new DelegateCommand(Fu9c3XcWQ);
			}
			return g2I0ZxSLo;
		}
	}

	public string LangVersion
	{
		[CompilerGenerated]
		get
		{
			return ylVsWX1DC;
		}
		[CompilerGenerated]
		set
		{
			ylVsWX1DC = value;
		}
	}

	[SpecialName]
	private static string ItMgEDGMJ()
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

	private static AppSetting UrM7ft5DD()
	{
		try
		{
			OfflineDataStore.EnsureDefaults();
			AppSetting setting;
			if (File.Exists(ItMgEDGMJ()))
			{
				string text = File.ReadAllText(ItMgEDGMJ());
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
			string failedPath = ItMgEDGMJ();
			if (File.Exists(failedPath))
			{
				string directoryName = Path.GetDirectoryName(failedPath);
				int num = 0;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 1);
				defaultInterpolatedStringHandler.AppendLiteral("AppConfig(");
				defaultInterpolatedStringHandler.AppendFormatted(num);
				defaultInterpolatedStringHandler.AppendLiteral(").json");
				string text2 = Path.Combine(directoryName, defaultInterpolatedStringHandler.ToStringAndClear());
				while (File.Exists(text2))
				{
					num++;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(15, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("AppConfig(");
					defaultInterpolatedStringHandler2.AppendFormatted(num);
					defaultInterpolatedStringHandler2.AppendLiteral(").json");
					text2 = Path.Combine(directoryName, defaultInterpolatedStringHandler2.ToStringAndClear());
				}
				try
				{
					File.Copy(failedPath, text2, overwrite: true);
				}
				catch (Exception)
				{
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(42, 2);
				defaultInterpolatedStringHandler3.AppendLiteral("配置文件加载时发生错误：");
				defaultInterpolatedStringHandler3.AppendFormatted(ex.Message);
				defaultInterpolatedStringHandler3.AppendLiteral("\r\n\r\n已为您将配置文件备份到：");
				defaultInterpolatedStringHandler3.AppendFormatted(text2);
				defaultInterpolatedStringHandler3.AppendLiteral("\r\n\r\n请发送给作者分析修复");
				MessageBox.Show(defaultInterpolatedStringHandler3.ToStringAndClear(), "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
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
			await File.WriteAllTextAsync(ItMgEDGMJ(), contents);
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

	private async void Fu9c3XcWQ()
	{
		await SaveSetting();
	}

	public AppSetting()
	{
		SecxiJm0v = new List<double>
		{
			3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0,
			13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 20.0, 22.0, 24.0,
			26.0, 28.0, 30.0, 32.0, 34.0, 36.0, 38.0, 40.0, 44.0, 48.0,
			52.0, 56.0, 60.0, 64.0, 68.0, 72.0, 76.0, 80.0, 88.0, 96.0,
			104.0, 112.0, 120.0, 128.0, 136.0, 144.0
		};
		vf5lRd8Fh = "pvfUtility";
	}

	static AppSetting()
	{
		ConfigPwd = "!#$%&*((qsdV2321SDFSD";
	}
}
