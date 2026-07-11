using System.Collections.Generic;

namespace pvfUtility.WebApi.Dto;

public class ItemCodesToFileInfosDto
{
	public Dictionary<int, ItemCodeToFileInfoDto> Infos { get; set; }
}
