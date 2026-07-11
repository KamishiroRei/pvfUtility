using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMTool.Dot;
using GMTool.Dot.taiwan_cain_2nd;
using GMTool.SqlModel.Enums;
using GMTool.SqlModel.d_taiwan;
using GMTool.SqlModel.taiwan_cain_2nd;
using PvfCode.Dot;
using SqlSugar;

namespace GMTool.Services;

public class DnfSqlService : Repository<accounts>
{
	public DnfSqlService(ISqlSugarClient context)
		: base(context)
	{
	}

	public async Task<ResultData<string>> SendPostal(IList<CharacInfoDto> characList, IList<PostalSendRes> postalItems, string sendTitle = null, string? sendText = null)
	{
		ResultData<string> result = new ResultData<string>();
		bool isSendLetter = !string.IsNullOrEmpty(sendTitle) || !string.IsNullOrEmpty(sendText);
		ResultData<PostalIdsData> resultData = await GetSendPostalId(getAvatarId: true, getCreateId: true, isSendLetter);
		if (resultData.IsError)
		{
			result.Msg = resultData.Msg;
			return result;
		}
		PostalIdsData idsData = resultData.Data;
		StringBuilder sendLog = new StringBuilder();
		List<PostalSendRes> avatarItems = null;
		List<PostalSendRes> petItems = null;
		List<Task> list = new List<Task>();
		list.Add(Task.Run(async delegate
		{
			avatarItems = await CreateAvatarPostalModels(characList, postalItems, sendTitle, idsData.AvatarId, sendLog);
		}));
		list.Add(Task.Run(async delegate
		{
			petItems = await CreatePetPostalModels(characList, postalItems, sendTitle, idsData.CreateId, sendLog);
		}));
		List<PostalSendRes> defaultItems = new List<PostalSendRes>();
		List<PostalSendRes> list2 = postalItems.Where((PostalSendRes it) => it.PostalType == PostalType.普通邮件 || it.PostalType == PostalType.金币).ToList();
		if (list2 != null && list2.Count > 0)
		{
			foreach (CharacInfoDto charac in characList)
			{
				foreach (PostalSendRes item in list2)
				{
					PostalSendRes postalSendRes = item.Clone();
					postalSendRes.receive_charac_no = charac.charac_no;
					sendLog.AppendLine(postalSendRes.GetSendLog(charac.charac_name));
					defaultItems.Add(postalSendRes);
				}
			}
		}
		await Task.WhenAll(list);
		if (avatarItems != null)
		{
			defaultItems.AddRange(avatarItems);
		}
		if (petItems != null)
		{
			defaultItems.AddRange(petItems);
		}
		List<LetterArray> letters = new List<LetterArray>();
		if (isSendLetter)
		{
			int num = idsData.LetterId;
			foreach (PostalSendRes item2 in defaultItems)
			{
				num = (item2.letter_id = num + 1);
				item2.send_charac_name = sendTitle;
				letters.Add(new LetterArray
				{
					letter_id = num,
					charac_no = item2.receive_charac_no,
					letter_text = sendTitle,
					send_charac_name = sendText
				});
			}
		}
		else
		{
			defaultItems.ForEach(delegate(PostalSendRes it)
			{
				it.send_charac_name = sendTitle;
			});
		}
		List<Task> list3 = new List<Task>();
		if (letters.Count > 0)
		{
			list3.Add(Task.Run(async delegate
			{
				await idsData.LetterDb.Insertable(letters).ExecuteCommandAsync();
			}));
		}
		list3.Add(Task.Run(async delegate
		{
			int value = await base.Db.Insertable(defaultItems).ExecuteCommandAsync();
			StringBuilder stringBuilder = sendLog;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder);
			handler.AppendLiteral("本次共发送邮件：");
			handler.AppendFormatted(value);
			handler.AppendLiteral("封");
			stringBuilder.AppendLine(ref handler);
		}));
		await Task.WhenAll(list3);
		result.Data = sendLog.ToString();
		return result;
	}

	public async Task<List<PostalSendRes>> CreateAvatarPostalModels(IList<CharacInfoDto> characList, IList<PostalSendRes> postalItems, string sendTitle, int currentUiId, StringBuilder sendLog)
	{
		List<PostalSendRes> targetPostal = new List<PostalSendRes>();
		IEnumerable<PostalSendRes> enumerable = postalItems.Where((PostalSendRes it) => it.PostalType == PostalType.时装邮件);
		if (enumerable == null || !enumerable.Any())
		{
			return targetPostal;
		}
		List<User_ItemsArray> list = new List<User_ItemsArray>();
		DateTime expire_date = DateTime.Now.AddDays(9999.0);
		foreach (CharacInfoDto charac in characList)
		{
			foreach (PostalSendRes item in enumerable)
			{
				currentUiId++;
				PostalSendRes postalSendRes = item.Clone();
				postalSendRes.receive_charac_no = charac.charac_no;
				postalSendRes.send_charac_name = sendTitle;
				postalSendRes.add_info = currentUiId;
				targetPostal.Add(postalSendRes);
				sendLog.AppendLine(postalSendRes.GetSendLog(charac.charac_name));
				list.Add(new User_ItemsArray
				{
					ui_id = currentUiId,
					charac_no = charac.charac_no,
					it_id = item.item_id,
					expire_date = expire_date,
					stat = 2,
					obtain_from = 1,
					hidden_option = (short)item.hidden_option
				});
			}
		}
		using (SqlSugarClient db = NewDb())
		{
			await db.Insertable(list).ExecuteCommandAsync();
		}
		return targetPostal;
	}

	public async Task<List<PostalSendRes>> CreatePetPostalModels(IList<CharacInfoDto> characList, IList<PostalSendRes> postalItems, string sendTitle, int creId, StringBuilder sendLog)
	{
		List<PostalSendRes> targetPostal = new List<PostalSendRes>();
		IEnumerable<PostalSendRes> enumerable = postalItems.Where((PostalSendRes it) => it.PostalType == PostalType.宠物 || it.PostalType == PostalType.宠物蛋);
		if (enumerable == null || !enumerable.Any())
		{
			return targetPostal;
		}
		List<Creature_ItemsArray> list = new List<Creature_ItemsArray>();
		DateTime dateTime = DateTime.Now.AddDays(9999.0);
		foreach (CharacInfoDto charac in characList)
		{
			foreach (PostalSendRes item in enumerable)
			{
				creId++;
				PostalSendRes postalSendRes = item.Clone();
				postalSendRes.receive_charac_no = charac.charac_no;
				postalSendRes.send_charac_name = sendTitle;
				if (postalSendRes.PostalType == PostalType.宠物)
				{
					if (postalSendRes.IsEqu.HasValue && postalSendRes.IsEqu.Value)
					{
						if (postalSendRes.Best)
						{
							postalSendRes.SetBest();
						}
						else
						{
							postalSendRes.add_info = creId;
						}
					}
					else
					{
						postalSendRes.add_info = creId;
					}
				}
				else
				{
					postalSendRes.add_info = creId;
				}
				sendLog.AppendLine(postalSendRes.GetSendLog(charac.charac_name));
				postalSendRes.add_info = creId;
				targetPostal.Add(postalSendRes);
				list.Add(new Creature_ItemsArray
				{
					ui_id = creId,
					charac_no = charac.charac_no,
					creature_type = ((postalSendRes.PostalType == PostalType.宠物) ? Creature_ItemsStyle.宠物 : Creature_ItemsStyle.宠物蛋),
					it_id = item.item_id,
					stat = true,
					expire_date = dateTime,
					delete_date = dateTime,
					stomach = 100
				});
			}
		}
		using (SqlSugarClient db = NewDb())
		{
			await db.Insertable(list).ExecuteCommandAsync();
		}
		return targetPostal;
	}

	public async Task<ResultData<PostalIdsData>> GetSendPostalId(bool getAvatarId, bool getCreateId, bool getLetterId)
	{
		ResultData<PostalIdsData> re = new ResultData<PostalIdsData>
		{
			Data = new PostalIdsData()
		};
		try
		{
			List<Task> list = new List<Task>();
			if (getAvatarId)
			{
				list.Add(Task.Run(async delegate
				{
					using SqlSugarClient db = NewDb();
					PostalIdsData data = re.Data;
					data.AvatarId = await db.Queryable<User_Items>().MaxAsync((User_Items it) => it.ui_id);
					re.Data.AvatarId += 1000;
				}));
			}
			if (getCreateId)
			{
				list.Add(Task.Run(async delegate
				{
					using SqlSugarClient db = NewDb();
					PostalIdsData data = re.Data;
					data.CreateId = await db.Queryable<Creature_Items>().MaxAsync((Creature_Items it) => it.ui_id);
					re.Data.CreateId += 1000;
				}));
			}
			if (getLetterId)
			{
				list.Add(Task.Run(async delegate
				{
					using SqlSugarClient db = NewDb();
					PostalIdsData data = re.Data;
					data.LetterId = await db.Queryable<Letter>().MaxAsync((Letter it) => it.letter_id);
					re.Data.LetterId += 1000;
				}));
				list.Add(Task.Run(async delegate
				{
					re.Data.LetterDb = NewDb();
					await re.Data.LetterDb.Ado.ExecuteCommandAsync("set names latin1;");
				}));
			}
			await Task.WhenAll(list);
		}
		catch (Exception ex)
		{
			re.Msg = ex.Message;
		}
		return re;
	}
}
