using System;
using Utools;

namespace PvfCode.Dot.Desktop;

public class StoreListDto : StoreListRes
{
	public int Id { get; set; }

	public int UserId { get; set; }

	public string NickName { get; set; }

	public int DownLoadNumber { get; set; }

	public string Avatar { get; set; }

	public DateTime Create { get; set; }

	public DateTime UpdateTime { get; set; }

	public T ToData<T>()
	{
		return base.Data.ToString().JsonToObject<T>();
	}
}
