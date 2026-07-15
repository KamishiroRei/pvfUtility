using System.Collections.Generic;

namespace Utools;

public class SimilarityResultInfo<TSource>
{
	public double SimilarityValue { get; set; }

	public IEnumerable<TSource> SimilarityTargetList { get; set; }

	public SimilarityResultInfo()
	{
	}
}
