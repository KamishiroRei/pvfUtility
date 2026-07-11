using System.Collections.Generic;

namespace pvfUtility.WebApi.Dto.Res;

public class GetFileContentRes
{
	public List<string> FileList { get; set; }

	public bool UseCompatibleDecompiler { get; set; }

	public string? EncodingType { get; set; }
}
