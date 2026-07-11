using System.Text;
using PvfCode.Dot;
using Swordfish.NET.Collections;

namespace PvfCode.ViewModels.independent_drop.DropList;

public class DropListList : DropListBase
{
	public DropListList(ConcurrentObservableCollection<ListItem> items)
	{
		base.Items = items;
	}

	public override ResultData<string> ToText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ListItem item in base.Items)
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder2);
			handler.AppendFormatted(item.ItemCode);
			handler.AppendLiteral("\t");
			handler.AppendFormatted(item.DropWeight);
			stringBuilder2.AppendLine(ref handler);
		}
		return new ResultData<string>
		{
			Data = stringBuilder.ToString()
		};
	}
}
