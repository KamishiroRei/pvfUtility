using DevExpress.Mvvm;
using Newtonsoft.Json;

namespace PvfCode;

[JsonObject(MemberSerialization.OptOut)]
public class PvfReleaseClientOptions : ViewModelBase
{
	public bool SetDropFileBlank
	{
		get
		{
			return GetProperty(() => SetDropFileBlank);
		}
		set
		{
			SetProperty(() => SetDropFileBlank, value);
		}
	}

	public bool DeleteEquCreationRate
	{
		get
		{
			return GetProperty(() => DeleteEquCreationRate);
		}
		set
		{
			SetProperty(() => DeleteEquCreationRate, value);
		}
	}

	public bool AddGiftBomb
	{
		get
		{
			return GetProperty(() => AddGiftBomb);
		}
		set
		{
			SetProperty(() => AddGiftBomb, value);
		}
	}

	public bool ClearQuestMonsterRewardItem
	{
		get
		{
			return GetProperty(() => ClearQuestMonsterRewardItem);
		}
		set
		{
			SetProperty(() => ClearQuestMonsterRewardItem, value);
		}
	}

	public bool DeleteQuestClearRewardItem
	{
		get
		{
			return GetProperty(() => DeleteQuestClearRewardItem);
		}
		set
		{
			SetProperty(() => DeleteQuestClearRewardItem, value);
		}
	}

	public bool DeleteQuestEnemyRewardItem
	{
		get
		{
			return GetProperty(() => DeleteQuestEnemyRewardItem);
		}
		set
		{
			SetProperty(() => DeleteQuestEnemyRewardItem, value);
		}
	}

	public bool DeleteMapDungeon
	{
		get
		{
			return GetProperty(() => DeleteMapDungeon);
		}
		set
		{
			SetProperty(() => DeleteMapDungeon, value);
		}
	}

	public bool SetMapMonsterCode1
	{
		get
		{
			return GetProperty(() => SetMapMonsterCode1);
		}
		set
		{
			SetProperty(() => SetMapMonsterCode1, value);
		}
	}

	public bool SetMapAiCharacterCodeRandomApcId
	{
		get
		{
			return GetProperty(() => SetMapAiCharacterCodeRandomApcId);
		}
		set
		{
			SetProperty(() => SetMapAiCharacterCodeRandomApcId, value);
		}
	}

	public bool SetLotteryRate1000
	{
		get
		{
			return GetProperty(() => SetLotteryRate1000);
		}
		set
		{
			SetProperty(() => SetLotteryRate1000, value);
		}
	}

	public bool DeleteMobCommonChampionDropItem
	{
		get
		{
			return GetProperty(() => DeleteMobCommonChampionDropItem);
		}
		set
		{
			SetProperty(() => DeleteMobCommonChampionDropItem, value);
		}
	}

	public bool DeleteMobItem
	{
		get
		{
			return GetProperty(() => DeleteMobItem);
		}
		set
		{
			SetProperty(() => DeleteMobItem, value);
		}
	}

	public bool DeletePackageSections
	{
		get
		{
			return GetProperty(() => DeletePackageSections);
		}
		set
		{
			SetProperty(() => DeletePackageSections, value);
		}
	}

	public bool ReplaceMoboxRandomList
	{
		get
		{
			return GetProperty(() => ReplaceMoboxRandomList);
		}
		set
		{
			SetProperty(() => ReplaceMoboxRandomList, value);
		}
	}

	public bool DeletePetEggOutputIndex
	{
		get
		{
			return GetProperty(() => DeletePetEggOutputIndex);
		}
		set
		{
			SetProperty(() => DeletePetEggOutputIndex, value);
		}
	}

	public bool ClearEtcRefillItem
	{
		get
		{
			return GetProperty(() => ClearEtcRefillItem);
		}
		set
		{
			SetProperty(() => ClearEtcRefillItem, value);
		}
	}

	public bool DeleteDungeonMapSpecification
	{
		get
		{
			return GetProperty(() => DeleteDungeonMapSpecification);
		}
		set
		{
			SetProperty(() => DeleteDungeonMapSpecification, value);
		}
	}

	public bool ConfusePetCanItem
	{
		get
		{
			return GetProperty(() => ConfusePetCanItem);
		}
		set
		{
			SetProperty(() => ConfusePetCanItem, value);
		}
	}

	public bool SetEquEmancipateOutput
	{
		get
		{
			return GetProperty(() => SetEquEmancipateOutput);
		}
		set
		{
			SetProperty(() => SetEquEmancipateOutput, value);
		}
	}

	public bool SetPackageOutput
	{
		get
		{
			return GetProperty(() => SetPackageOutput);
		}
		set
		{
			SetProperty(() => SetPackageOutput, value);
		}
	}

	public void Select(bool value)
	{
		SetDropFileBlank = value;
		DeleteEquCreationRate = value;
		AddGiftBomb = value;
		ClearQuestMonsterRewardItem = value;
		DeleteQuestClearRewardItem = value;
		DeleteQuestEnemyRewardItem = value;
		DeleteMapDungeon = value;
		SetMapMonsterCode1 = value;
		SetMapAiCharacterCodeRandomApcId = value;
		SetLotteryRate1000 = value;
		DeleteMobCommonChampionDropItem = value;
		DeleteMobItem = value;
		DeletePackageSections = value;
		ReplaceMoboxRandomList = value;
		DeletePetEggOutputIndex = value;
		ClearEtcRefillItem = value;
		DeleteDungeonMapSpecification = value;
		ConfusePetCanItem = value;
		SetEquEmancipateOutput = value;
		SetPackageOutput = value;
	}

	public PvfReleaseClientOptions()
	{
	}
}
