using System;
using System.Collections.Generic;
using PvfCode.Services.PvfParsingNew;

namespace PvfCode.Services.PvfFileServices;

public class ServicePvfFile
{
	private readonly PvfFile file;

	private readonly PvfGroup pvf;

	public ServicePvfFile(PvfGroup pvf, PvfFile file)
	{
		this.pvf = pvf;
		this.file = file;
		if (file.Data == null || file.DataLen < 7)
		{
			return;
		}
		List<ScriptItem> list = new List<ScriptItem>();
		for (int i = 2; i < file.DataLen - 4; i += 5)
		{
			byte b = file.Data[i];
			if (b >= 2 && b <= 10)
			{
				ScriptItem item = new ScriptItem
				{
					Type = (ScriptType)b,
					Data = BitConverter.ToInt32(file.Data, i + 1)
				};
				list.Add(item);
			}
		}
		foreach (ScriptItem item2 in list)
		{
			_ = item2;
		}
	}
}
