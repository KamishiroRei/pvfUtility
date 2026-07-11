using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using PvfCode.LoggerBase;
using PvfCode.Models.Options.Editor;
using PvfCode.Models.Pvf;
using PvfCode.Services.PvfParsingNew.CustomSectionFormat;
using PvfCode.Services.PvfParsingNew.TableFormatters;
using Swordfish.NET.Collections.Auxiliary;
using Utools;

namespace PvfCode.Services.PvfParsingNew;

public static class PraserInfoProviderConfiger
{
	public static string ConfigFilePath;

	private static Ilogger logger;

	public static Dictionary<string, Dictionary<string, List<CustomSectionFormatBase>>> Config { get; set; }

	internal static List<KeyValuePair<string, TableRowFormatterBase>> TableRowFormatters { get; set; }

	private static Ilogger GetLogger()
	{
		if (logger == null)
		{
			logger = AppSetting.Instance.GetIlogger();
		}
		return logger;
	}

	public static bool Init()
	{
		InitializeTableRowFormatters();
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		Config = new Dictionary<string, Dictionary<string, List<CustomSectionFormatBase>>>();
		if (File.Exists(ConfigFilePath))
		{
			try
			{
				File.Delete(ConfigFilePath);
			}
			catch (Exception)
			{
			}
		}
		try
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.IgnoreComments = true;
			XmlReader reader = XmlReader.Create(BytesHelper.StringToStream(lang.PraseInfo), xmlReaderSettings);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(reader);
			Dictionary<string, List<CustomSectionFormatBase>> value = new Dictionary<string, List<CustomSectionFormatBase>>
			{
				{
					"[sell item]",
					new List<CustomSectionFormatBase>
					{
						new ShopItemListSectionFormat()
					}
				},
				{
					"[one a day item]",
					new List<CustomSectionFormatBase>
					{
						new ShopItemListSectionFormat()
					}
				}
			};
			Config.Add("*.shp", value);
			Dictionary<string, List<CustomSectionFormatBase>> value2 = new Dictionary<string, List<CustomSectionFormatBase>> { 
			{
				"[dungeon party balance]",
				new List<CustomSectionFormatBase>
				{
					new DungeonPartyBalanceSectionFormat()
				}
			} };
			Config.Add("*.tbl", value2);
			Dictionary<string, List<CustomSectionFormatBase>> value3 = new Dictionary<string, List<CustomSectionFormatBase>>
			{
				{
					"[special passive object item]",
					new List<CustomSectionFormatBase>
					{
						new SpecialPassiveObjectItemSectionFormat()
					}
				},
				{
					"[monster difficulty bonus]",
					new List<CustomSectionFormatBase>
					{
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 5 }
						}
					}
				}
			};
			Config.Add("*.dgn", value3);
			Dictionary<string, List<CustomSectionFormatBase>> value4 = new Dictionary<string, List<CustomSectionFormatBase>>
			{
				{
					"[level info]",
					new List<CustomSectionFormatBase>
					{
						new LevelInfoSectionFormat()
					}
				},
				{
					"[special level up]",
					new List<CustomSectionFormatBase>
					{
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 4 }
						}
					}
				}
			};
			Config.Add("*.skl", value4);
			Dictionary<string, List<CustomSectionFormatBase>> value5 = new Dictionary<string, List<CustomSectionFormatBase>>
			{
				{
					"[int data]",
					new List<CustomSectionFormatBase>
					{
						new CountPrefixedGridSectionFormat
						{
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[use item]`"
								})
							}
						},
						new CountPrefixedGridSectionFormat
						{
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[disjoint item]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "1"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 2 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[use fortune coin]`"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 5 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[hunt enemy]`"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 2 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[clear map]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "0"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 1 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[clear map]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "-1"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 3 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[condition under clear]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "0"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 3 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[condition under clear]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "1"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 2 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[condition under clear]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "4"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 3 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[condition under clear]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "5"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 2 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[condition under clear]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "6"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 3 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[condition under clear]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "7"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 3 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[condition under clear]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "8"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 3 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[condition under clear]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "9"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 3 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[condition under clear]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "10"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 3 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[condition under clear]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "11"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 4 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[condition under clear]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "13"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 4 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[condition under clear]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "14"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 4 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[condition under clear]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "15"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 4 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[condition under clear]`"
								}),
								new KeyValuePair<string, ValidationSectionData>("[sub type]", new ValidationSectionData
								{
									Value = "16"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 2 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[seeking]`"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 4 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[hunt monster]`"
								})
							}
						},
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 3 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`[custom quest]`"
								})
							}
						}
					}
				},
				{
					"[reward selection int data]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_qst_reward__selection_int_data()
					}
				},
				{
					"[reward int data]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_qst_reward__selection_int_data()
					}
				},
				{
					"[enemy reward item]",
					new List<CustomSectionFormatBase>
					{
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 8 }
						}
					}
				},
				{
					"[dungeon info]",
					new List<CustomSectionFormatBase>
					{
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 2 }
						}
					}
				}
			};
			Config.Add("*.qst", value5);
			Dictionary<string, List<CustomSectionFormatBase>> value6 = new Dictionary<string, List<CustomSectionFormatBase>>
			{
				{
					"[int data]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_Stk_Blueprint
						{
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[stackable type]", new ValidationSectionData
								{
									Value = "`[recipe]`0",
									Index = 0
								})
							}
						},
						new CustomSectionFormat_Stk_Blueprint
						{
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[stackable type]", new ValidationSectionData
								{
									Value = "`[recipe]`1",
									Index = 0
								})
							}
						},
						new CustomSectionFormat_Stk_upgradable_legacy
						{
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[stackable type]", new ValidationSectionData
								{
									Value = "`[upgradable legacy]`1",
									Index = 0
								})
							}
						},
						new CustomSectionFormat_Stk_upgradable_legacy
						{
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[stackable type]", new ValidationSectionData
								{
									Value = "`[upgradable legacy]`0",
									Index = 0
								})
							}
						},
						new CustomSectionFormat_Stk_upgradable_legacy
						{
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[stackable type]", new ValidationSectionData
								{
									Value = "`[upgradable legacy]`2",
									Index = 0
								})
							}
						}
					}
				},
				{
					"[A condition item]",
					new List<CustomSectionFormatBase>
					{
						new VariableGroupSectionFormat
						{
							ParentSectionName = "[upgrade limit cube info]",
							GroupLengths = new List<int> { 2 }
						}
					}
				},
				{
					"[B condition item]",
					new List<CustomSectionFormatBase>
					{
						new VariableGroupSectionFormat
						{
							ParentSectionName = "[upgrade limit cube info]",
							GroupLengths = new List<int> { 2 }
						}
					}
				},
				{
					"[result item]",
					new List<CustomSectionFormatBase>
					{
						new VariableGroupSectionFormat
						{
							ParentSectionName = "[upgrade limit cube info]",
							GroupLengths = new List<int> { 3 }
						}
					}
				}
			};
			Config.Add("*.stk", value6);
			Dictionary<string, List<CustomSectionFormatBase>> value7 = new Dictionary<string, List<CustomSectionFormatBase>> { 
			{
				"[world drop]",
				new List<CustomSectionFormatBase>
				{
					new CustomSectionFormat_Worlddrop
					{
						MaxLen = 2
					}
				}
			} };
			Config.Add("etc/worlddrop.etc", value7);
			Dictionary<string, List<CustomSectionFormatBase>> value8 = new Dictionary<string, List<CustomSectionFormatBase>> { 
			{
				"[title collection info]",
				new List<CustomSectionFormatBase>
				{
					new CustomSectionFormatBookTitle()
				}
			} };
			Config.Add("etc/titlebook.etc", value8);
			Dictionary<string, List<CustomSectionFormatBase>> value9 = new Dictionary<string, List<CustomSectionFormatBase>>
			{
				{
					"[normal key]",
					new List<CustomSectionFormatBase>
					{
						new CountPrefixedGridSectionFormat()
					}
				},
				{
					"[skill key]",
					new List<CustomSectionFormatBase>
					{
						new CountPrefixedGridSectionFormat()
					}
				},
				{
					"[down key]",
					new List<CustomSectionFormatBase>
					{
						new CountPrefixedGridSectionFormat()
					}
				}
			};
			Config.Add("*.rep", value9);
			Dictionary<string, List<CustomSectionFormatBase>> value10 = new Dictionary<string, List<CustomSectionFormatBase>> { 
			{
				"[modified option selection]",
				new List<CustomSectionFormatBase>
				{
					new VariableGroupSectionFormat
					{
						GroupLengths = new List<int> { 12 }
					}
				}
			} };
			Config.Add("etc/randomoption/optiongroupselection.etc", value10);
			Dictionary<string, List<CustomSectionFormatBase>> value11 = new Dictionary<string, List<CustomSectionFormatBase>>
			{
				{
					"[dungeon]",
					new List<CustomSectionFormatBase>
					{
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 17 }
						}
					}
				},
				{
					"[quest]",
					new List<CustomSectionFormatBase>
					{
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 17 }
						}
					}
				},
				{
					"[dungeon minimap]",
					new List<CustomSectionFormatBase>
					{
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 6 }
						}
					}
				}
			};
			Config.Add("etc/chn_schedule.etc", value11);
			Dictionary<string, List<CustomSectionFormatBase>> value12 = new Dictionary<string, List<CustomSectionFormatBase>> { 
			{
				"[expertjob level limit]",
				new List<CustomSectionFormatBase>
				{
					new VariableGroupSectionFormat
					{
						GroupLengths = new List<int> { 2 }
					}
				}
			} };
			Config.Add("character/expertjob.etc", value12);
			Dictionary<string, List<CustomSectionFormatBase>> dictionary = new Dictionary<string, List<CustomSectionFormatBase>>
			{
				{
					"[hat avatar]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_compoundavatar()
					}
				},
				{
					"[hair avatar]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_compoundavatar()
					}
				},
				{
					"[face avatar]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_compoundavatar()
					}
				},
				{
					"[neck avatar]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_compoundavatar()
					}
				},
				{
					"[coat avatar]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_compoundavatar()
					}
				},
				{
					"[pants avatar]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_compoundavatar()
					}
				},
				{
					"[belt avatar]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_compoundavatar()
					}
				},
				{
					"[shoes avatar]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_compoundavatar()
					}
				}
			};
			Config.Add("etc/compoundavatar_atfighter.etc", new Dictionary<string, List<CustomSectionFormatBase>>(dictionary));
			Config.Add("etc/compoundavatar_atgunner.etc", new Dictionary<string, List<CustomSectionFormatBase>>(dictionary));
			Config.Add("etc/compoundavatar_atmage.etc", new Dictionary<string, List<CustomSectionFormatBase>>(dictionary));
			Config.Add("etc/compoundavatar_creator_mage.etc", new Dictionary<string, List<CustomSectionFormatBase>>(dictionary));
			Config.Add("etc/compoundavatar_demonic_swordman.etc", new Dictionary<string, List<CustomSectionFormatBase>>(dictionary));
			Config.Add("etc/compoundavatar_fighter.etc", new Dictionary<string, List<CustomSectionFormatBase>>(dictionary));
			Config.Add("etc/compoundavatar_gunner.etc", new Dictionary<string, List<CustomSectionFormatBase>>(dictionary));
			Config.Add("etc/compoundavatar_mage.etc", new Dictionary<string, List<CustomSectionFormatBase>>(dictionary));
			Config.Add("etc/compoundavatar_priest.etc", new Dictionary<string, List<CustomSectionFormatBase>>(dictionary));
			Config.Add("etc/compoundavatar_swordman.etc", new Dictionary<string, List<CustomSectionFormatBase>>(dictionary));
			Config.Add("etc/compoundavatar_thief.etc", new Dictionary<string, List<CustomSectionFormatBase>>(dictionary));
			Dictionary<string, List<CustomSectionFormatBase>> value13 = new Dictionary<string, List<CustomSectionFormatBase>>
			{
				{
					"[aurora graphic effects]",
					new List<CustomSectionFormatBase>
					{
						new CountPrefixedGridSectionFormat()
					}
				},
				{
					"[avatar select ability]",
					new List<CustomSectionFormatBase>
					{
						new AvatarSelectAbilitySectionFormat()
					}
				},
				{
					"[character item check]",
					new List<CustomSectionFormatBase>
					{
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 3 }
						}
					}
				},
				{
					"[int data]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_Stk_Blueprint
						{
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[stackable type]", new ValidationSectionData
								{
									Value = "`[recipe]`0",
									Index = 0
								})
							}
						},
						new CustomSectionFormat_Stk_Blueprint
						{
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[stackable type]", new ValidationSectionData
								{
									Value = "`[recipe]`1",
									Index = 0
								})
							}
						}
					}
				}
			};
			Config.Add("*.equ", value13);
			Dictionary<string, List<CustomSectionFormatBase>> value14 = new Dictionary<string, List<CustomSectionFormatBase>> { 
			{
				"[dungeon name]",
				new List<CustomSectionFormatBase>
				{
					new DungeonNameSectionFormat()
				}
			} };
			Config.Add("etc/itemdictionary/(r)dungeon_name.etc", value14);
			Dictionary<string, List<CustomSectionFormatBase>> value15 = new Dictionary<string, List<CustomSectionFormatBase>> { 
			{
				"[equipLottery list]",
				new List<CustomSectionFormatBase>
				{
					new EquipmentLotteryListSectionFormat()
				}
			} };
			Config.Add("etc/itemdictionary/(r)lotterylistmakeequip.etc", value15);
			Dictionary<string, List<CustomSectionFormatBase>> value16 = new Dictionary<string, List<CustomSectionFormatBase>> { 
			{
				"[random category]",
				new List<CustomSectionFormatBase>
				{
					new RandomCategorySectionFormat()
				}
			} };
			Config.Add("etc/randomoption/auctionrandomcategory.etc", value16);
			Dictionary<string, List<CustomSectionFormatBase>> value17 = new Dictionary<string, List<CustomSectionFormatBase>> { 
			{
				"[regeneration price]",
				new List<CustomSectionFormatBase>
				{
					new RegenerationPriceSectionFormat()
				}
			} };
			Config.Add("etc/randomoption/regenerationrandomoption.etc", value17);
			Dictionary<string, List<CustomSectionFormatBase>> value18 = new Dictionary<string, List<CustomSectionFormatBase>> { 
			{
				"[upgrade effect]",
				new List<CustomSectionFormatBase>
				{
					new UpgradeEffectSectionFormat()
				}
			} };
			Config.Add("etc/etcparameter.etc", value18);
			Dictionary<string, List<CustomSectionFormatBase>> value19 = new Dictionary<string, List<CustomSectionFormatBase>> { 
			{
				"[dungeon]",
				new List<CustomSectionFormatBase>
				{
					new CustomSectionFormat_wdm_dungeon()
				}
			} };
			Config.Add("*.wdm", value19);
			Dictionary<string, List<CustomSectionFormatBase>> value20 = new Dictionary<string, List<CustomSectionFormatBase>> { 
			{
				"[skill]",
				new List<CustomSectionFormatBase>
				{
					new VariableGroupSectionFormat
					{
						GroupLengths = new List<int> { 2 }
					}
				}
			} };
			Config.Add("*.chr", value20);
			Dictionary<string, List<CustomSectionFormatBase>> value21 = new Dictionary<string, List<CustomSectionFormatBase>>
			{
				{
					"[independent drop]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_independent_drop()
					}
				},
				{
					"[list]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_independent_drop_list()
					}
				}
			};
			Config.Add("etc/independent_drop.etc", value21);
			Dictionary<string, List<CustomSectionFormatBase>> value22 = new Dictionary<string, List<CustomSectionFormatBase>>
			{
				{
					"[item]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_etc_shp(2, new List<string>
						{
							"stackable",
							"equipment"
						}, 9, 7)
					}
				},
				{
					"[premium]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_etc_shp(2, new List<string>
						{
							"stackable",
							"equipment"
						}, 9, 7)
					}
				},
				{
					"[package]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_etc_shp(2, new List<string>
						{
							"stackable",
							"equipment"
						}, 10, 6)
					}
				}
			};
			Config.Add("etc/newcashshop.etc", value22);
			Dictionary<string, List<CustomSectionFormatBase>> value23 = new Dictionary<string, List<CustomSectionFormatBase>> { 
			{
				"[term]",
				new List<CustomSectionFormatBase>
				{
					new PremiumTermSectionFormat()
				}
			} };
			Config.Add("etc/premiumlist_new.etc", value23);
			Dictionary<string, List<CustomSectionFormatBase>> value24 = new Dictionary<string, List<CustomSectionFormatBase>>
			{
				{
					"[special passive object]",
					new List<CustomSectionFormatBase>
					{
						new SpecialPassiveObjectSectionFormat()
					}
				},
				{
					"[monster]",
					new List<CustomSectionFormatBase>
					{
						new CustomSectionFormat_map_monster()
					}
				}
			};
			Config.Add("*.map", value24);
			Dictionary<string, List<CustomSectionFormatBase>> value25 = new Dictionary<string, List<CustomSectionFormatBase>> { 
			{
				"[equipRecipe list]",
				new List<CustomSectionFormatBase>
				{
					new CustomSectionFormat_recipelistmakeequip()
				}
			} };
			Config.Add("etc/itemdictionary/(r)recipelistmakeequip.etc", value25);
			Dictionary<string, List<CustomSectionFormatBase>> value26 = new Dictionary<string, List<CustomSectionFormatBase>>
			{
				{
					"[string data]",
					new List<CustomSectionFormatBase>
					{
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 7 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`skill data up`"
								})
							}
						}
					}
				},
				{
					"[int data]",
					new List<CustomSectionFormatBase>
					{
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 3 },
							ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>
							{
								new KeyValuePair<string, ValidationSectionData>("[type]", new ValidationSectionData
								{
									Value = "`skill level`"
								})
							}
						}
					}
				}
			};
			Config.Add("*.apd", value26);
			Dictionary<string, List<CustomSectionFormatBase>> value27 = new Dictionary<string, List<CustomSectionFormatBase>>
			{
				{
					"[monstercard bind list]",
					new List<CustomSectionFormatBase>
					{
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 3 }
						}
					}
				},
				{
					"[enchanter extraction info]",
					new List<CustomSectionFormatBase>
					{
						new VariableGroupSectionFormat
						{
							GroupLengths = new List<int> { 2 }
						}
					}
				}
			};
			Config.Add("*.exj", value27);
			new Dictionary<string, List<CustomSectionFormatBase>>().Add("[reset item]", new List<CustomSectionFormatBase>
			{
				new VariableGroupSectionFormat
				{
					GroupLengths = new List<int> { 2 }
				}
			});
			Config.Add("etc/chn_server_limititemusageinfo.etc", value25);
			Dictionary<string, List<CustomSectionFormatBase>> value28 = new Dictionary<string, List<CustomSectionFormatBase>> { 
			{
				"[item]",
				new List<CustomSectionFormatBase>
				{
					new VariableGroupSectionFormat
					{
						GroupLengths = new List<int> { 2 }
					}
				}
			} };
			Config.Add("*.mob", value28);
			foreach (XmlElement item in xmlDocument.SelectNodes("/root/File"))
			{
				string attribute = item.GetAttribute("FileName");
				if (!Config.ContainsKey(attribute))
				{
					Dictionary<string, List<CustomSectionFormatBase>> dictionary2 = new Dictionary<string, List<CustomSectionFormatBase>>();
					Config.Add(attribute, dictionary2);
					AddVariableGroupFormats(dictionary2, item);
				}
				else
				{
					AddVariableGroupFormats(Config[attribute], item);
				}
			}
		}
		catch (Exception ex2)
		{
			ilogger.Error("PVF文档格式化配置文件读取失败：" + ex2.Message);
			ilogger.ShowMsg("PVF文档格式化配置文件读取失败：" + ex2.Message, isError: true);
			return false;
		}
		return true;
	}

	private static void InitializeTableRowFormatters()
	{
		TableRowFormatters = new List<KeyValuePair<string, TableRowFormatterBase>>();
		TableRowFormatters.Add(new KeyValuePair<string, TableRowFormatterBase>("*.tbl", new FiveColumnTableRowFormatter("dungeon")));
		TableRowFormatters.Add(new KeyValuePair<string, TableRowFormatterBase>("etc/itemgroupname.tbl", new TwoColumnTableRowFormatter(null)));
		TableRowFormatters.Add(new KeyValuePair<string, TableRowFormatterBase>("monster/bossmonsterbaseparameter.tbl", new FixedColumnTableRowFormatter(24, null)));
		TableRowFormatters.Add(new KeyValuePair<string, TableRowFormatterBase>("monster/newmonsters/advanceatlar/table/advancealtarmonsterbaseparameter.tbl", new FixedColumnTableRowFormatter(24, null)));
		TableRowFormatters.Add(new KeyValuePair<string, TableRowFormatterBase>("monster/chaosmonsterbaseparameter.tbl", new FixedColumnTableRowFormatter(11, null)));
		TableRowFormatters.Add(new KeyValuePair<string, TableRowFormatterBase>("monster/commonmonsterbaseparameter.tbl", new FixedColumnTableRowFormatter(24, null)));
		TableRowFormatters.Add(new KeyValuePair<string, TableRowFormatterBase>("monster/namedmonsterbaseparameter.tbl", new FixedColumnTableRowFormatter(24, null)));
		TableRowFormatters.Add(new KeyValuePair<string, TableRowFormatterBase>("monster/summonmonsterbaseparameter.tbl", new FixedColumnTableRowFormatter(24, null)));
		TableRowFormatters.Add(new KeyValuePair<string, TableRowFormatterBase>("*.tbl", new FixedColumnTableRowFormatter(5, "monster")));
		TableRowFormatters.Add(new KeyValuePair<string, TableRowFormatterBase>("monster/monsterbaseparameter.tbl", new FixedColumnTableRowFormatter(11, null)));
		TableRowFormatters.Add(new KeyValuePair<string, TableRowFormatterBase>("monster/warroommonsterbaseparameter.tbl", new FixedColumnTableRowFormatter(12, null)));
		TableRowFormatters.Add(new KeyValuePair<string, TableRowFormatterBase>("etc/iteminfo.dat", new FixedColumnTableRowFormatter(17, null)));
		TableRowFormatters.Add(new KeyValuePair<string, TableRowFormatterBase>("etc/motionhackcheck.etc", new FixedColumnTableRowFormatter(2, null)));
		TableRowFormatters.Add(new KeyValuePair<string, TableRowFormatterBase>("etc/itemdictionary/(r)itemdictionary.etc", new ItemDictionaryTableRowFormatter(null)));
		TableRowFormatters.Add(new KeyValuePair<string, TableRowFormatterBase>("equipment/oldequipmentstatinfolist.dat", new FixedColumnTableRowFormatter(34, null)));
	}

	internal static TableRowFormatterBase? GetTableRowFormatter(string fileName, string pathPrefix)
	{
		if (TableRowFormatters == null)
		{
			return null;
		}
		KeyValuePair<string, TableRowFormatterBase>? exactMatch = TableRowFormatters.Find(item => item.Key == fileName);
		if (exactMatch.HasValue && exactMatch.Value.Key != null)
		{
			return (TableRowFormatterBase)exactMatch.Value.Value.Clone();
		}
		foreach (KeyValuePair<string, TableRowFormatterBase> item in TableRowFormatters)
		{
			if (LikeOperator.LikeString(fileName, item.Key, CompareMethod.Binary) && (item.Value.PathPrefix == null || item.Value.PathPrefix == pathPrefix))
			{
				return (TableRowFormatterBase)item.Value.Clone();
			}
		}
		return null;
	}

	private static void AddVariableGroupFormats(Dictionary<string, List<CustomSectionFormatBase>> formats, XmlElement parentElement)
	{
		if (parentElement.ChildNodes == null)
		{
			return;
		}
		foreach (XmlElement childNode in parentElement.ChildNodes)
		{
			string attribute = childNode.GetAttribute("SectionName");
			if (!formats.TryGetValue(attribute, out List<CustomSectionFormatBase> value))
			{
				value = new List<CustomSectionFormatBase>();
				formats.Add(attribute, value);
			}
			string text = childNode.GetAttribute("ParentSectionName");
			if (string.IsNullOrEmpty(text))
			{
				text = null;
			}
			value.Add(new VariableGroupSectionFormat
			{
				SectionName = childNode.GetAttribute("SectionName"),
				GroupLengths = ReadGroupLengths(childNode),
				ParentSectionName = text
			});
		}
	}

	private static List<int> ReadGroupLengths(XmlElement element)
	{
		XmlNodeList xmlNodeList = element.SelectNodes("GroupLength");
		if (xmlNodeList == null || xmlNodeList.Count == 0)
		{
			if (!int.TryParse(element.GetAttribute("GroupLength"), out var result))
			{
				GetLogger()?.Error("脚本文件解析器配置错误：填写不正确的GroupLength");
				return new List<int>();
			}
			return new List<int> { result };
		}
		List<int> list = new List<int>();
		foreach (XmlElement item in xmlNodeList)
		{
			if (int.TryParse(item.GetAttribute("Value"), out var result2))
			{
				list.Add(result2);
			}
			else
			{
				GetLogger()?.Error("脚本文件解析器配置错误：填写不正确的GroupLength");
			}
		}
		return list;
	}

	public static Dictionary<string, List<CustomSectionFormatBase>> GetDic(string fileName)
	{
		if (Config == null)
		{
			return new Dictionary<string, List<CustomSectionFormatBase>>();
		}
		if (Config.TryGetValue(fileName, out Dictionary<string, List<CustomSectionFormatBase>> value))
		{
			return value;
		}
		string text = Path.GetDirectoryName(fileName).Replace("\\", "/");
		if (text == "etc/independentdrop")
		{
			if (Config.TryGetValue(text, out value))
			{
				return value;
			}
			return new Dictionary<string, List<CustomSectionFormatBase>>();
		}
		foreach (KeyValuePair<string, Dictionary<string, List<CustomSectionFormatBase>>> item in Config)
		{
			if (LikeOperator.LikeString(fileName, item.Key, CompareMethod.Binary))
			{
				return item.Value;
			}
		}
		return new Dictionary<string, List<CustomSectionFormatBase>>();
	}

	public static CustomSectionFormatBase GetConfig(Dictionary<string, List<CustomSectionFormatBase>> dic, PvfFile file, PvfGroup pvf, string sectionName, string? parentSectionName)
	{
		if (dic == null || sectionName.IsEmpty())
		{
			return new CustomSectionFormatDefault();
		}
		if (dic.TryGetValue(sectionName, out List<CustomSectionFormatBase> value))
		{
			if (value.Count == 1)
			{
				CustomSectionFormatBase customSectionFormatBase = value[0];
				if (customSectionFormatBase.CheckParentSectionName(parentSectionName) && customSectionFormatBase.ValidationSection(file, pvf))
				{
					return (CustomSectionFormatBase)customSectionFormatBase.Clone();
				}
			}
			foreach (CustomSectionFormatBase item in value.FindAll((CustomSectionFormatBase it) => !string.IsNullOrEmpty(it.ParentSectionName)))
			{
				if (item.CheckParentSectionName(parentSectionName) && item.ValidationSection(file, pvf))
				{
					return (CustomSectionFormatBase)item.Clone();
				}
			}
			foreach (CustomSectionFormatBase item2 in value.FindAll((CustomSectionFormatBase it) => string.IsNullOrEmpty(it.ParentSectionName)))
			{
				if (item2.CheckParentSectionName(parentSectionName) && item2.ValidationSection(file, pvf))
				{
					return (CustomSectionFormatBase)item2.Clone();
				}
			}
		}
		return new CustomSectionFormatDefault();
	}

	static PraserInfoProviderConfiger()
	{
		ConfigFilePath = Path.Combine(AppSetting.AppBasePath, "ScriptFileParserConfiger.xml");
	}
}
