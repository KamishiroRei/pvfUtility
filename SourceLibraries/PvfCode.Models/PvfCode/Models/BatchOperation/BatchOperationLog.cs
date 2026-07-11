using System.Collections.Generic;

namespace PvfCode.Models.BatchOperation;

public class BatchOperationLog
{
	public IEnumerable<string> SuccessFileList { get; set; }

	public IEnumerable<string> ErrorFileList { get; set; }

	public BatchOperationLog()
	{
	}
}
