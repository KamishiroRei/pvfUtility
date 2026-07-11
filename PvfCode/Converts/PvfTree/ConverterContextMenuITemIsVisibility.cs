using System;
using System.Globalization;
using System.Windows.Data;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Converts.PvfTree;

public class ConverterContextMenuITemIsVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return false;
		}
		TreeViewType treeViewType = (TreeViewType)value;
		bool flag = false;
		string text = parameter.ToString();
		if (text != null)
		{
			switch (text.Length)
			{
			case 2:
				switch (text[0])
				{
				case '删':
					if (text == "删除" && (uint)treeViewType <= 1u)
					{
						flag = true;
					}
					break;
				case '提':
					if (text == "提取" && (uint)treeViewType <= 1u)
					{
						flag = true;
					}
					break;
				case '导':
					if (text == "导入" && treeViewType == TreeViewType.FileList)
					{
						flag = true;
					}
					break;
				case '粘':
					if (text == "粘贴")
					{
						flag = treeViewType == TreeViewType.FileList;
					}
					break;
				case '属':
					if (text == "属性" && (uint)treeViewType <= 1u)
					{
						flag = true;
					}
					break;
				}
				break;
			case 7:
				switch (text[0])
				{
				case '转':
					if (text == "转到文件..." && treeViewType == TreeViewType.FileList)
					{
						flag = true;
					}
					break;
				case '添':
					if (text == "添加到查找视图")
					{
						flag = treeViewType == TreeViewType.FileList;
					}
					break;
				}
				break;
			case 6:
				switch (text[2])
				{
				case 'm':
					if (text == "提取main" && ((uint)treeViewType <= 1u || (uint)(treeViewType - 8) <= 1u))
					{
						flag = true;
					}
					break;
				case '为':
					if (text == "提取为lst" && ((uint)treeViewType <= 1u || treeViewType == TreeViewType.ExtractFiles))
					{
						flag = true;
					}
					break;
				case '所':
					if (text == "展开所有节点")
					{
						flag = treeViewType != TreeViewType.FileList;
					}
					break;
				case '空':
					if (text == "新建空白文件")
					{
						flag = treeViewType == TreeViewType.FileList;
					}
					break;
				case 't':
					if (text == "lst管理器")
					{
						flag = treeViewType == TreeViewType.FileList;
					}
					break;
				}
				break;
			case 3:
				switch (text[0])
				{
				case '重':
					if (text == "重命名" && treeViewType == TreeViewType.FileList)
					{
						flag = true;
					}
					break;
				case '批':
					if (text == "批处理" && (uint)treeViewType <= 1u)
					{
						flag = true;
					}
					break;
				}
				break;
			case 4:
				switch (text[0])
				{
				case '移':
					if (text == "移除选中")
					{
						switch (treeViewType)
						{
						case TreeViewType.SearchResult:
						case TreeViewType.ExtractFiles:
						case TreeViewType.ImportFiles:
						case TreeViewType.BatchOperation:
							flag = true;
							break;
						}
					}
					break;
				case '编':
					if (text == "编辑注释" && treeViewType == TreeViewType.FileList)
					{
						flag = true;
					}
					break;
				case '获':
					if (text == "获取代码")
					{
						switch (treeViewType)
						{
						case TreeViewType.FileList:
						case TreeViewType.SearchResult:
						case TreeViewType.ExtractFiles:
						case TreeViewType.BatchOperation:
							flag = true;
							break;
						}
					}
					break;
				case '复':
					if (text == "复制剪切" && (uint)treeViewType <= 1u)
					{
						flag = true;
					}
					break;
				case '发':
					if (text == "发送邮件")
					{
						switch (treeViewType)
						{
						case TreeViewType.FileList:
						case TreeViewType.SearchResult:
						case TreeViewType.ExtractFiles:
						case TreeViewType.SelectFiles:
						case TreeViewType.BatchOperation:
						case TreeViewType.BatchOperationLog:
							flag = true;
							break;
						}
					}
					break;
				}
				break;
			case 9:
				switch (text[0])
				{
				case '转':
					if (text == "转到文件资源管理器")
					{
						flag = treeViewType == TreeViewType.SearchResult;
					}
					break;
				case '使':
					if (text == "使用差异比较器打开")
					{
						switch (treeViewType)
						{
						case TreeViewType.FileList:
						case TreeViewType.SearchResult:
						case TreeViewType.ExtractFiles:
						case TreeViewType.ImportFiles:
						case TreeViewType.BatchOperation:
							flag = true;
							break;
						}
					}
					break;
				}
				break;
			case 5:
				if (text == "添加到书签" && (uint)treeViewType <= 1u)
				{
					flag = true;
				}
				break;
			}
		}
		return flag;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConverterContextMenuITemIsVisibility()
	{
	}
}
