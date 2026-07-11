using System.Collections.Generic;
using Newtonsoft.Json;

namespace ViewModels.Diff;

public class DiffEditorCompareResult
{
	public class CharChanx
	{
		[JsonProperty("modifiedEndColumn")]
		public int ModifiedEndColumn { get; set; }

		[JsonProperty("modifiedEndLineNumber")]
		public int ModifiedEndLineNumber { get; set; }

		[JsonProperty("modifiedStartColumn")]
		public int ModifiedStartColumn { get; set; }

		[JsonProperty("modifiedStartLineNumber")]
		public int ModifiedStartLineNumber { get; set; }

		[JsonProperty("originalEndColumn")]
		public int OriginalEndColumn { get; set; }

		[JsonProperty("originalEndLineNumber")]
		public int OriginalEndLineNumber { get; set; }

		[JsonProperty("originalStartColumn")]
		public int OriginalStartColumn { get; set; }

		[JsonProperty("originalStartLineNumber")]
		public int OriginalStartLineNumber { get; set; }
	}

	[JsonProperty("originalStartLineNumber")]
	public int OriginalStartLineNumber { get; set; }

	[JsonProperty("originalEndLineNumber")]
	public int OriginalEndLineNumber { get; set; }

	[JsonProperty("modifiedStartLineNumber")]
	public int ModifiedStartLineNumber { get; set; }

	[JsonProperty("modifiedEndLineNumber")]
	public int ModifiedEndLineNumber { get; set; }

	[JsonProperty("charChanges")]
	public IList<CharChanx> CharChanges { get; set; }
}
