using System.Collections.Generic;

namespace pvfUtility.WebApi.Dto;

public class DeleteFilesResultDto
{
	public IEnumerable<string> SuccessFiles { get; set; }

	public IEnumerable<string> ErrorFiles { get; set; }
}
