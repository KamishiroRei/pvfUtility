using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PvfCode.Dot;

namespace SevenZip;

public class SevenZipHelper
{
	public static ResultData<byte[]> Get7zipFileBytes(string zipFilePath, string filePath)
	{
		ResultData<byte[]> resultData = new ResultData<byte[]>();
		try
		{
			using SevenZipExtractor sevenZipExtractor = new SevenZipExtractor(zipFilePath);
			List<ArchiveFileInfo> source = sevenZipExtractor.ArchiveFileData.ToList();
			IEnumerable<ArchiveFileInfo> enumerable = source.Where((ArchiveFileInfo it) => it.FileName == filePath);
			if (enumerable == null)
			{
				resultData.Msg = "压缩包内未能找到：" + filePath;
				return resultData;
			}
			ArchiveFileInfo archiveFileInfo = enumerable.First();
			using MemoryStream memoryStream = new MemoryStream();
			sevenZipExtractor.ExtractFile(archiveFileInfo.Index, memoryStream);
			resultData.Data = memoryStream.ToArray();
		}
		catch (Exception ex)
		{
			resultData.Msg = ex.Message;
		}
		return resultData;
	}
}
