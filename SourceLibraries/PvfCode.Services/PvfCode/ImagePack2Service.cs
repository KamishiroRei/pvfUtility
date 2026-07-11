using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Collections.Pooled;
using DevExpress.Mvvm;
using Newtonsoft.Json;
using PvfCode.Dot;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Models.Pvf.ScriptEnums;
using PvfCode.NPK.Utils;
using PvfCode.NPK.Utils.Models;
using PvfCode.Services;
using PvfCode.Services.PreviewPvfFileFolder.Stackable;
using Swordfish.NET.Collections;
using Utools;

namespace PvfCode;

[JsonObject(MemberSerialization.OptOut)]
public class ImagePack2Service : ViewModelBase
{
	public class FilesToIconBase64Reponse
	{
		public string Path { get; set; }

		public string Icon { get; set; }

		public FilesToIconBase64Reponse()
		{
		}
	}

	private static ImagePack2Service instance;

	[JsonIgnore]
	public bool IsWork { get; set; }

	[JsonIgnore]
	public static ImagePack2Service Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new ImagePack2Service();
			}
			return instance;
		}
	}

	public IDictionary<string, UtImgFile> NpkImgDIC { get; private set; }

	public Dictionary<int, string> NpkFilePathDic { get; set; }

	[JsonIgnore]
	public Dictionary<RarityType, Dictionary<int, EnchantCardInfoLevelIconInfo>> EnchantCardInfoLevelIcons { get; set; }

	[JsonIgnore]
	public Dictionary<MonsterCategoryType, ImageSource> MonsterTypeIcons { get; set; }

	[JsonIgnore]
	public Dictionary<string, Dictionary<int, ImageSource>> TreeFileIconList { get; private set; }

	[JsonIgnore]
	public List<ImageSource> QuestTypeIconList { get; set; }

	[JsonIgnore]
	public PooledDictionary<string, ImageSource> EmoIconList { get; set; }

	[JsonIgnore]
	public int Count
	{
		get
		{
			if (NpkImgDIC != null)
			{
				return NpkImgDIC.Count;
			}
			return 0;
		}
	}

	public bool IsLoadedTreeIcon()
	{
		if (TreeFileIconList != null)
		{
			return TreeFileIconList.Count > 0;
		}
		return false;
	}

	public void SetSource(IDictionary<string, UtImgFile> idic)
	{
		NpkImgDIC = new Dictionary<string, UtImgFile>(idic);
	}

	public ResultData<ImageSource> GetImage(string imgPath, int index)
	{
		if (imgPath == null)
		{
			return new ResultData<ImageSource>();
		}
		if (index < 0)
		{
			return new ResultData<ImageSource>
			{
				Msg = AppSetting.Instance.GetIlogger().GetStr("mess_ImageIndexError")
			};
		}
		if (Count == 0)
		{
			return new ResultData<ImageSource>
			{
				Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("EditorToolTipStyle_ItemCodeHover_NoImagePack2ModelDir")
			};
		}
		string normalizedPath = "sprite/" + imgPath.ToLower();
		if (NpkImgDIC.TryGetValue(normalizedPath, out UtImgFile value) && index < value.Count)
		{
			try
			{
				List<ImagePack> list = NpkCoder.ReadNpk(GetNpkFilePath(value));
				if (list != null && list.Any())
				{
					ImagePack imagePack = list.FirstOrDefault(item => item.Path == normalizedPath);
					if (imagePack != null && imagePack.ImgList != null && index < imagePack.ImgList.Count)
					{
						return new ResultData<ImageSource>
						{
							Data = imagePack.ImgList[index].ImageSource
						};
					}
				}
			}
			catch (Exception ex)
			{
				ex.Message.Contains("NPK已被加密 已忽略...");
			}
		}
		return new ResultData<ImageSource>();
	}

	public ResultData<Bitmap> GetImage2(string imgPath, int index)
	{
		if (Count == 0)
		{
			return new ResultData<Bitmap>
			{
				Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("EditorToolTipStyle_ItemCodeHover_NoImagePack2ModelDir")
			};
		}
		string normalizedPath = "sprite/" + imgPath.ToLower();
		if (NpkImgDIC.TryGetValue(normalizedPath, out UtImgFile value) && index < value.Count)
		{
			try
			{
				List<ImagePack> list = NpkCoder.ReadNpk(GetNpkFilePath(value));
				if (list != null && list.Any())
				{
					ImagePack imagePack = list.FirstOrDefault(item => item.Path == normalizedPath);
					if (imagePack != null && imagePack.ImgList != null && index < imagePack.ImgList.Count)
					{
						return new ResultData<Bitmap>
						{
							Data = imagePack.ImgList[index].Picture
						};
					}
				}
			}
			catch (Exception)
			{
			}
		}
		return new ResultData<Bitmap>();
	}

	public ResultData ImgGoToNpkFile(string imgPath)
	{
		ResultData resultData = new ResultData();
		if (IsWork)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_ImagePack2_LoadWork");
			return resultData;
		}
		if (Count == 0)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("EditorToolTipStyle_ItemCodeHover_NoImagePack2ModelDir");
			return resultData;
		}
		imgPath = CombineImgPath(imgPath).ToLower();
		if (NpkImgDIC.TryGetValue(imgPath, out UtImgFile value))
		{
			string npkFilePath = GetNpkFilePath(value);
			if (!File.Exists(npkFilePath))
			{
				resultData.Msg = string.Format(AppSetting.Instance.GetIlogger().GetStr("mess_NpkNotExists"), npkFilePath);
			}
			else
			{
				FileHelper.OpenFolderAndSelectFile(npkFilePath);
			}
		}
		else
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger().GetStr("mess_ImgNotExists"), imgPath);
		}
		return resultData;
	}

	private string ResolveNpkPath(string relativePath)
	{
		return Path.Combine(AppSetting.Instance.ImagePacks2Options.ImagePacks2Path, relativePath);
	}

	public string GetNpkFilePath(int id)
	{
		if (NpkFilePathDic == null)
		{
			return null;
		}
		return ResolveNpkPath(NpkFilePathDic[id]);
	}

	public string GetNpkFilePath(UtImgFile utImgFile)
	{
		if (NpkFilePathDic == null)
		{
			return null;
		}
		return ResolveNpkPath(NpkFilePathDic[utImgFile.FileNameId]);
	}

	public string CombineImgPath(string imgPath)
	{
		return "sprite/" + imgPath;
	}

	public ResultData<UtImgFile> First(string imgPath)
	{
		ResultData<UtImgFile> resultData = new ResultData<UtImgFile>();
		if (IsWork)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_ImagePack2_LoadWork");
			return resultData;
		}
		if (Count == 0)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_ImagePacks2NotCreated");
			return resultData;
		}
		imgPath = CombineImgPath(imgPath);
		if (NpkImgDIC.TryGetValue(imgPath, out UtImgFile value))
		{
			return new ResultData<UtImgFile>
			{
				Data = value
			};
		}
		resultData.Msg = string.Format(AppSetting.Instance.GetIlogger().GetStr("mess_ImgNotExists"), imgPath);
		return resultData;
	}

	public ResultData<UtImgFile> First(string imgPath, int index)
	{
		ResultData<UtImgFile> resultData = new ResultData<UtImgFile>();
		if (IsWork)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_ImagePack2_LoadWork");
			return resultData;
		}
		if (Count == 0)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_ImagePacks2NotCreated");
			return resultData;
		}
		imgPath = CombineImgPath(imgPath);
		if (NpkImgDIC.TryGetValue(imgPath, out UtImgFile value))
		{
			if (index < value.Count)
			{
				return new ResultData<UtImgFile>
				{
					Data = value
				};
			}
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger().GetStr("mess_ImgIndexNotExists"), imgPath, index);
		}
		else
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger().GetStr("mess_ImgNotExists"), imgPath);
		}
		return resultData;
	}

	public async Task<ResultData> LoadImagePack2Async(string dir)
	{
		if (DiskDetectionUtils.DetectDrive(AppSetting.Instance.ImagePacks2Options.ImagePacks2Path).HardwareType != HardwareType.Ssd)
		{
			AppSetting.Instance.GetIlogger().Warning(AppSetting.Instance.GetIlogger().GetStrNoReplace("Mess_NotSSD"));
		}
		ResultData re = new ResultData();
		NpkImgDIC = new Dictionary<string, UtImgFile>();
		RaisePropertyChanged("Count");
		Stopwatch sw = new Stopwatch();
		sw.Start();
		ResultData<Dictionary<string, UtImgFile>, string, Dictionary<int, string>> resultData = await NpkCoder.ParallelReadNpkList2(dir);
		sw.Stop();
		_ = sw.Elapsed;
		if (resultData.IsError)
		{
			return new ResultData
			{
				Msg = resultData.Msg
			};
		}
		if (resultData.Data2 != null && resultData.Data2.Length > 0)
		{
			AppSetting.Instance.GetIlogger()?.Error(resultData.Data2);
		}
		NpkImgDIC = resultData.Data;
		NpkFilePathDic = resultData.Data3;
		RaisePropertyChanged("Count");
		AppSetting.Instance.GetIlogger()?.Success(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ImagePacks2ModelDirLoadSuccess"), Count));
		return re;
	}

	public void LoadTreeIcons(PvfGroup pvf)
	{
		if (AppSetting.Instance.TreeSetting.TreeShowNpkIcon && Count != 0 && pvf != null && pvf.PvfIsOpen)
		{
			AppSetting.Instance.GetIlogger()?.Warning(AppSetting.Instance.GetIlogger().GetStrNoReplace("Mess_LoadingTheNPKIconLibrary"));
			LoadReferencedTreeIcons(pvf);
			AppSetting.Instance.GetIlogger()?.Success(AppSetting.Instance.GetIlogger().GetStrNoReplace("Mess_NPKIconLibraryIsLoaded"));
			SaveToDisk();
		}
	}

	private void LoadQuestTypeIcons()
	{
		List<ImageSource> list = new List<ImageSource>();
		const string questIconPath = "sprite/interface/quest/quest_tag.img";
		if (!NpkImgDIC.TryGetValue(questIconPath, out UtImgFile value))
		{
			return;
		}
		string npkFilePath = GetNpkFilePath(value);
		if (!File.Exists(npkFilePath))
		{
			return;
		}
		try
		{
			List<ImagePack> list2 = NpkCoder.ReadNpk(npkFilePath);
			if (list2 != null && list2.Count > 0)
			{
				ImagePack imagePack = list2.FirstOrDefault(item => item.Path == questIconPath);
				if (imagePack != null && imagePack.ImgList != null)
				{
					foreach (ImgFile img in imagePack.ImgList)
					{
						ImageSource imageSouce = img.GetImageSouce();
						((Freezable)imageSouce).Freeze();
						list.Add(imageSouce);
					}
				}
			}
			try
			{
				IsWork = true;
				QuestTypeIconList = list;
			}
			catch (Exception)
			{
			}
			finally
			{
				IsWork = false;
			}
		}
		catch (Exception ex2)
		{
			if (ex2.Message.Contains("because it is being used by another process"))
			{
				AppSetting.Instance.GetIlogger()?.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileInUse"), npkFilePath));
			}
			else
			{
				AppSetting.Instance.GetIlogger()?.ErrorUploadDialog(ex2, "LoadQuestTypeIcons");
			}
		}
	}

	private void LoadReferencedTreeIcons(PvfGroup pvf)
	{
		ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentHashSet<int>>> iconRequests = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentHashSet<int>>>();
		Parallel.ForEach(pvf.FileList.Values.ToArray(), file =>
		{
			if (file != null && file.GetIcon(pvf, out KeyValuePair<string, int>? icon))
			{
				string key = CombineImgPath(icon.Value.Key.ToLower());
				if (NpkImgDIC.TryGetValue(key, out UtImgFile value))
				{
					string npkFilePath = GetNpkFilePath(value);
					iconRequests.GetOrAdd(npkFilePath, new ConcurrentDictionary<string, ConcurrentHashSet<int>>()).GetOrAdd(key, new ConcurrentHashSet<int>()).Add(icon.Value.Value);
				}
			}
		});
		if (iconRequests.Count > 0)
		{
			Dictionary<string, Dictionary<int, ImageSource>> dictionary = NpkCoder.ReadNpkTreeIcon(iconRequests).ToDictionary<KeyValuePair<string, ConcurrentDictionary<int, ImageSource>>, string, Dictionary<int, ImageSource>>((KeyValuePair<string, ConcurrentDictionary<int, ImageSource>> it) => it.Key, (KeyValuePair<string, ConcurrentDictionary<int, ImageSource>> it) => it.Value.ToDictionary((KeyValuePair<int, ImageSource> ot) => ot.Key, (KeyValuePair<int, ImageSource> ot) => ot.Value));
			TreeFileIconList = dictionary;
		}
		LoadQuestTypeIcons();
		LoadEmoIconList(pvf);
		LoadEnchantCardLevelIcons();
		LoadMonsterTypeIcons();
	}

	private void LoadEmoIconList(PvfGroup pvf)
	{
		PooledDictionary<string, ImageSource> pooledDictionary = new PooledDictionary<string, ImageSource>();
		using (PooledDictionary<string, List<ImagePack>> pooledDictionary2 = new PooledDictionary<string, List<ImagePack>>())
		{
			foreach (string file in pvf.GetFiles("chatemoticon"))
			{
				if (!pvf.FileList.TryGetValue(file, out PvfFile value) || value == null || !value.GetEmoAniPath(pvf, out string aniFilePath) || !pvf.FileList.TryGetValue(aniFilePath, out PvfFile value2) || !BinaryAniCompiler.DecompileAniToModel(value2, out AniFile anifile) || anifile.Count <= 0)
				{
					continue;
				}
				FRAMEModel fRAMEModel = anifile.Items.FirstOrDefault();
				if (fRAMEModel.Image == null)
				{
					continue;
				}
				string imagePath = fRAMEModel.Image.ImgFullPath;
				if (string.IsNullOrEmpty(imagePath) || !NpkImgDIC.TryGetValue(imagePath, out UtImgFile value3))
				{
					continue;
				}
				string npkFilePath = GetNpkFilePath(value3);
				if (!pooledDictionary2.TryGetValue(npkFilePath, out var value4))
				{
					if (File.Exists(npkFilePath))
					{
						try
						{
							value4 = NpkCoder.ReadNpk(npkFilePath);
						}
						catch (Exception ex)
						{
							if (ex.Message.Contains("because it is being used by another process"))
							{
								AppSetting.Instance.GetIlogger()?.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileInUse"), npkFilePath));
							}
							else
							{
								AppSetting.Instance.GetIlogger()?.ErrorUploadDialog(ex, "LoadEmoIconList");
							}
							value4 = new List<ImagePack>();
						}
						pooledDictionary2.Add(npkFilePath, value4);
					}
					else
					{
						value4 = new List<ImagePack>();
						pooledDictionary2.Add(npkFilePath, value4);
					}
				}
				if (value4.Count != 0)
				{
					ConcurrentObservableCollection<ImgFile> concurrentObservableCollection = value4.FirstOrDefault(item => item.Path == imagePath)?.ImgList;
					if (concurrentObservableCollection != null && fRAMEModel.Image.Index < concurrentObservableCollection.Count)
					{
						ImageSource imageSouce = concurrentObservableCollection[fRAMEModel.Image.Index].GetImageSouce();
						((Freezable)imageSouce).Freeze();
						pooledDictionary.Add(file, imageSouce);
					}
				}
			}
		}
		try
		{
			IsWork = true;
			EmoIconList = pooledDictionary;
		}
		catch (Exception)
		{
		}
		finally
		{
			IsWork = false;
		}
	}

	private void LoadEnchantCardLevelIcons()
	{
		try
		{
			if (NpkImgDIC == null || !NpkImgDIC.TryGetValue("sprite/interface/monstercard/cut_source.img", out UtImgFile value))
			{
				return;
			}
			string npkFilePath = GetNpkFilePath(value);
			if (!File.Exists(npkFilePath))
			{
				return;
			}
			List<ImagePack> list = NpkCoder.ReadNpk(npkFilePath);
			if (list != null && list.Count != 0)
			{
				ImagePack imagePack = list.Where((ImagePack it) => it.Path == "sprite/interface/monstercard/cut_source.img").FirstOrDefault();
				if (imagePack != null && imagePack.Count >= 50)
				{
					List<ImgFile> list2 = imagePack.ImgList.ToList();
					EnchantCardInfoLevelIcons = new Dictionary<RarityType, Dictionary<int, EnchantCardInfoLevelIconInfo>>();
					EnchantCardInfoLevelIcons.Add(RarityType.史诗, CreateEnchantCardLevelIconMap(list2.GetRange(0, 10)));
					EnchantCardInfoLevelIcons.Add(RarityType.神器, CreateEnchantCardLevelIconMap(list2.GetRange(10, 10)));
					EnchantCardInfoLevelIcons.Add(RarityType.稀有, CreateEnchantCardLevelIconMap(list2.GetRange(20, 10)));
					EnchantCardInfoLevelIcons.Add(RarityType.高级, CreateEnchantCardLevelIconMap(list2.GetRange(30, 10)));
					EnchantCardInfoLevelIcons.Add(RarityType.普通, CreateEnchantCardLevelIconMap(list2.GetRange(40, 10)));
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void LoadMonsterTypeIcons()
	{
		try
		{
			if (NpkImgDIC == null || !NpkImgDIC.TryGetValue("sprite/common/etc/category.img", out UtImgFile value))
			{
				return;
			}
			string npkFilePath = GetNpkFilePath(value);
			if (!File.Exists(npkFilePath))
			{
				return;
			}
			List<ImagePack> list = NpkCoder.ReadNpk(npkFilePath);
			if (list == null || list.Count == 0)
			{
				return;
			}
			ImagePack imagePack = list.Where((ImagePack it) => it.Path == "sprite/common/etc/category.img").FirstOrDefault();
			if (imagePack == null || imagePack.Count < 1)
			{
				return;
			}
			MonsterTypeIcons = new Dictionary<MonsterCategoryType, ImageSource>();
			foreach (ImgFile img in imagePack.ImgList)
			{
				MonsterCategoryType index = (MonsterCategoryType)img.Index;
				if (!MonsterTypeIcons.ContainsKey(index))
				{
					ImageSource imageSouce = img.GetImageSouce();
					if (imageSouce != null)
					{
						((Freezable)imageSouce).Freeze();
					}
					MonsterTypeIcons.Add(index, imageSouce);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public KeyValuePair<EnchantCardInfoLevelIconInfo?, EnchantCardInfoLevelIconInfo?>? GetEnchantCardInfoLevelIcon(int level, RarityType rarityType)
	{
		if (level < 0)
		{
			return null;
		}
		if (level > 99)
		{
			level = 99;
		}
		if (EnchantCardInfoLevelIcons == null || !EnchantCardInfoLevelIcons.TryGetValue(rarityType, out Dictionary<int, EnchantCardInfoLevelIconInfo> value))
		{
			return null;
		}
		if (level > 9)
		{
			string text = level.ToString();
			int key = text[0].ToString().ToInt();
			int key2 = text[1].ToString().ToInt();
			return new KeyValuePair<EnchantCardInfoLevelIconInfo, EnchantCardInfoLevelIconInfo>(value[key], value[key2]);
		}
		return new KeyValuePair<EnchantCardInfoLevelIconInfo, EnchantCardInfoLevelIconInfo>(value[level], null);
	}

	private Dictionary<int, EnchantCardInfoLevelIconInfo> CreateEnchantCardLevelIconMap(List<ImgFile> images)
	{
		Dictionary<int, EnchantCardInfoLevelIconInfo> dictionary = new Dictionary<int, EnchantCardInfoLevelIconInfo>();
		for (int i = 0; i < images.Count; i++)
		{
			ImageSource imageSouce = images[i].GetImageSouce();
			((Freezable)imageSouce).Freeze();
			dictionary.Add(i, new EnchantCardInfoLevelIconInfo(imageSouce, images[i].Width, images[i].Height));
		}
		return dictionary;
	}

	public bool GetIcon(PvfGroup pvf, PvfFile file, out ImageSource? imageSource)
	{
		imageSource = null;
		if (IsWork)
		{
			return false;
		}
		if (file == null)
		{
			return false;
		}
		if (TreeGetIcon(pvf, file, out imageSource))
		{
			return true;
		}
		if (file.GetIcon(pvf, out KeyValuePair<string, int>? icon))
		{
			ResultData<ImageSource> image = GetImage(icon.Value.Key, icon.Value.Value);
			if (image.IsError)
			{
				return false;
			}
			imageSource = image.Data;
			return true;
		}
		return false;
	}

	public bool TreeGetIcon(PvfGroup pvf, PvfFile file, out ImageSource? imageSource)
	{
		if (IsWork)
		{
			imageSource = null;
			return false;
		}
		if (file.FileType == PvfFileType.qst)
		{
			return TryGetQuestTypeIcon(pvf, file, out imageSource);
		}
		if (file.FileType == PvfFileType.emo)
		{
			return TryGetEmoticonIcon(file, out imageSource);
		}
		if (TreeFileIconList == null || TreeFileIconList.Count == 0)
		{
			imageSource = null;
			return false;
		}
		if (file.GetIcon(pvf, out KeyValuePair<string, int>? icon) && TreeFileIconList != null && TreeFileIconList.TryGetValue(CombineImgPath(icon.Value.Key.ToLower()), out Dictionary<int, ImageSource> value) && value.TryGetValue(icon.Value.Value, out imageSource))
		{
			return true;
		}
		imageSource = null;
		return false;
	}

	private bool TryGetQuestTypeIcon(PvfGroup pvf, PvfFile file, out ImageSource? imageSource)
	{
		if (QuestTypeIconList == null || QuestTypeIconList.Count == 0)
		{
			imageSource = null;
			return false;
		}
		QuestType? questType = file.GetQuestType(pvf);
		if (!questType.HasValue)
		{
			imageSource = null;
			return false;
		}
		int value = (int)questType.Value;
		if (value < QuestTypeIconList.Count)
		{
			imageSource = QuestTypeIconList[value];
			return true;
		}
		imageSource = null;
		return false;
	}

	private bool TryGetEmoticonIcon(PvfFile file, out ImageSource? imageSource)
	{
		imageSource = null;
		if (EmoIconList == null || EmoIconList.Count == 0)
		{
			return false;
		}
		return EmoIconList.TryGetValue(file.FileName, out imageSource);
	}

	public void ClearTreeIcon()
	{
		TreeFileIconList?.Clear();
		TreeFileIconList = null;
		QuestTypeIconList?.Clear();
		QuestTypeIconList = null;
		EmoIconList?.Clear();
		EmoIconList?.Dispose();
		EmoIconList = null;
	}

	public void Clear()
	{
		NpkImgDIC?.Clear();
		ClearTreeIcon();
		RaisePropertyChanged("Count");
	}

	public async void SaveToDisk()
	{
		await Task.CompletedTask;
	}

	public static string ConvertImageSourceToBase64(ImageSource imageSource)
	{
		if (imageSource == null)
		{
			return null;
		}
		BitmapEncoder bitmapEncoder = new PngBitmapEncoder();
		bitmapEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource));
		using MemoryStream memoryStream = new MemoryStream();
		bitmapEncoder.Save(memoryStream);
		return Convert.ToBase64String(memoryStream.ToArray());
	}

	public HashSet<string> ImgGetNpkFilePath(string imgPath)
	{
		string searchPath = imgPath.ToLower();
		IEnumerable<KeyValuePair<string, UtImgFile>> enumerable = NpkImgDIC.Where(item => item.Key.Contains(searchPath));
		HashSet<string> hashSet = new HashSet<string>();
		foreach (KeyValuePair<string, UtImgFile> item in enumerable)
		{
			hashSet.Add(GetNpkFilePath(item.Value));
		}
		return hashSet;
	}

	public ResultData<Dictionary<string, string>> FilesToIconBase64(List<string> files, PvfGroup pvf)
	{
		ResultData<Dictionary<string, string>> resultData = new ResultData<Dictionary<string, string>>();
		if (!pvf.PvfIsOpen)
		{
			resultData.Msg = "请先载入PVF";
			return resultData;
		}
		if (IsWork)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_ImagePack2_LoadWork");
			return resultData;
		}
		if (Count == 0)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("EditorToolTipStyle_ItemCodeHover_NoImagePack2ModelDir");
			return resultData;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (string item in files.ToHashSet())
		{
			if (pvf.FileList.TryGetValue(item, out PvfFile value) && TreeGetIcon(pvf, value, out ImageSource imageSource) && imageSource != null)
			{
				try
				{
					dictionary.Add(item, EncodeImageSourceToBase64(imageSource));
				}
				catch (Exception)
				{
				}
			}
		}
		resultData.Data = dictionary;
		return resultData;
	}

	public ResultData<Dictionary<int, FilesToIconBase64Reponse>> FilesToIconBase64New(List<string> files, PvfGroup pvf)
	{
		ResultData<Dictionary<int, FilesToIconBase64Reponse>> resultData = new ResultData<Dictionary<int, FilesToIconBase64Reponse>>();
		if (!pvf.PvfIsOpen)
		{
			resultData.Msg = "请先载入PVF";
			return resultData;
		}
		if (IsWork)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_ImagePack2_LoadWork");
			return resultData;
		}
		if (Count == 0)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("EditorToolTipStyle_ItemCodeHover_NoImagePack2ModelDir");
			return resultData;
		}
		Dictionary<int, FilesToIconBase64Reponse> dictionary = new Dictionary<int, FilesToIconBase64Reponse>();
		foreach (string item in files.ToHashSet())
		{
			if (!pvf.FileList.TryGetValue(item, out PvfFile value))
			{
				continue;
			}
			int? itemCode = value.ItemCode;
			if (!itemCode.HasValue || !TreeGetIcon(pvf, value, out ImageSource imageSource) || imageSource == null)
			{
				continue;
			}
			try
			{
				if (!dictionary.ContainsKey(itemCode.Value))
				{
					dictionary.Add(itemCode.Value, new FilesToIconBase64Reponse
					{
						Icon = EncodeImageSourceToBase64(imageSource),
						Path = item
					});
				}
			}
			catch (Exception)
			{
			}
		}
		resultData.Data = dictionary;
		return resultData;
	}

	private string EncodeImageSourceToBase64(ImageSource imageSource)
	{
		BitmapSource bitmapSource = imageSource as BitmapSource;
		if (bitmapSource == null)
		{
			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext drawingContext = drawingVisual.RenderOpen())
			{
				drawingContext.DrawImage(imageSource, new Rect(new System.Windows.Size(imageSource.Width, imageSource.Height)));
			}
			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)imageSource.Width, (int)imageSource.Height, 96.0, 96.0, PixelFormats.Default);
			renderTargetBitmap.Render(drawingVisual);
			bitmapSource = renderTargetBitmap;
		}
		PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
		MemoryStream memoryStream = new MemoryStream();
		pngBitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
		pngBitmapEncoder.Save(memoryStream);
		return Convert.ToBase64String(memoryStream.ToArray());
	}

	public ImagePack2Service()
	{
	}
}
