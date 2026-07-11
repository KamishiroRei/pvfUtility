using System.Collections.Generic;

namespace pvfUtility.WebApi.Dto.Res;

public class ItemCodesToFileInfosRes
{
	public List<string> lstNames { get; set; }

	public List<int> ItemCodes { get; set; }
}
