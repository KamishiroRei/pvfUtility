using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

public class PostalArrHelper
{
	private List<User_ItemsArray> AvaDatas;

	private List<Creature_ItemsArray> CreDatas;

	private List<LetterArray> LetterDatas;

	public List<PostalSend> PostalDatas;

	private List<PostalSend> SendPostalDatas;

	internal SqlSugarClient DB;

	internal string SendName { get; set; }

	internal string SendText { get; set; }

	private async Task<int> GetLetterIdMax()
	{
		int num = await DB.Queryable<Letter>().MaxAsync((Letter it) => it.letter_id) + 1;
		Random random = new Random(Guid.NewGuid().GetHashCode());
		int num2 = random.Next(2, 100);
		int num3 = random.Next(99, 9999);
		return num + num2 * 1000 + num3;
	}

	private async Task<int> GetCreMaxId()
	{
		int num = await DB.Queryable<Creature_Items>().MaxAsync((Creature_Items it) => it.ui_id) + 1;
		Random random = new Random(Guid.NewGuid().GetHashCode());
		int num2 = random.Next(2, 100);
		int num3 = random.Next(99, 9999);
		return num + num2 * 1000 + num3;
	}

	private async Task<int> GetAvaMaxId()
	{
		int num = await DB.Queryable<User_Items>().MaxAsync((User_Items it) => it.ui_id) + 1;
		Random random = new Random(Guid.NewGuid().GetHashCode());
		int num2 = random.Next(2, 100);
		int num3 = random.Next(99, 9999);
		return num + num2 * 1000 + num3;
	}

	public async Task<int> Send()
	{
		return 0;
	}
}
