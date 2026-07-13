#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using PvfCode.ViewModels.DropRateManagement;

namespace PvfCode.Views.Tools;

public sealed class DropRateManagementWindow : ThemedWindow
{
	private const string HellPath = "etc/itemdropinfo_monster_hell.etc";
	private const string ClearRewardPath = "etc/itemdropinfo_clearreward.etc";

	private static readonly string[] RarityNames = { "普通", "高级", "稀有", "神器", "史诗" };
	private static readonly Brush[] RarityBrushes =
	{
		CreateBrush("#F1F1F1"),
		CreateBrush("#68D5ED"),
		CreateBrush("#B36BFF"),
		CreateBrush("#FF4DF2"),
		CreateBrush("#FFB100")
	};

	private readonly PvfGroup pvf;
	private readonly List<RateColumnEditor> hellEditors = new();
	private RateColumnEditor? clearRewardEditor;
	private string hellText = string.Empty;
	private string clearRewardText = string.Empty;

	public DropRateManagementWindow()
	{
		pvf = AppCore.ViewModelBase.PVF;
		Title = "深渊/翻牌爆率管理";
		Width = 920;
		Height = 460;
		MinWidth = 860;
		MinHeight = 440;
		WindowStartupLocation = WindowStartupLocation.CenterOwner;
		AutomationProperties.SetAutomationId(this, "DropRateManagement.Window");
		Content = BuildContent();
		LoadRates();
	}

	private UIElement BuildContent()
	{
		DockPanel root = new() { Margin = new Thickness(20) };

		TextBlock description = new()
		{
			Text = "数值单位为百分比。普通爆率自动按 100% 减去其他稀有度之和计算；保存时会换算为百万分母的累加值。",
			TextWrapping = TextWrapping.Wrap,
			Margin = new Thickness(0, 0, 0, 14)
		};
		DockPanel.SetDock(description, Dock.Top);
		root.Children.Add(description);

		Button saveButton = new()
		{
			Content = "保存",
			Width = 110,
			Height = 34,
			HorizontalAlignment = HorizontalAlignment.Right,
			Margin = new Thickness(0, 14, 0, 0),
			Background = CreateBrush("#007ACC"),
			BorderBrush = CreateBrush("#007ACC"),
			Foreground = Brushes.White
		};
		saveButton.Click += (_, _) => SaveRates();
		AutomationProperties.SetAutomationId(saveButton, "DropRateManagement.Save");
		DockPanel.SetDock(saveButton, Dock.Bottom);
		root.Children.Add(saveButton);

		Grid groups = new();
		groups.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
		groups.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		GroupBox hellGroup = BuildHellGroup();
		hellGroup.Margin = new Thickness(0, 0, 14, 0);
		groups.Children.Add(hellGroup);
		GroupBox clearRewardGroup = BuildClearRewardGroup();
		Grid.SetColumn(clearRewardGroup, 1);
		groups.Children.Add(clearRewardGroup);
		root.Children.Add(groups);
		return root;
	}

	private GroupBox BuildHellGroup()
	{
		Grid grid = CreateRateGrid(new[] { "非常困难", "困难" });
		hellEditors.Add(CreateColumnEditor(grid, 1, "Hell.VeryHard"));
		hellEditors.Add(CreateColumnEditor(grid, 2, "Hell.Hard"));
		return new GroupBox
		{
			Header = "深渊爆率",
			Padding = new Thickness(12),
			Content = grid
		};
	}

	private GroupBox BuildClearRewardGroup()
	{
		Grid grid = CreateRateGrid(new[] { "翻牌" });
		clearRewardEditor = CreateColumnEditor(grid, 1, "ClearReward");
		return new GroupBox
		{
			Header = "翻牌爆率",
			Padding = new Thickness(12),
			Content = grid
		};
	}

	private static Grid CreateRateGrid(IReadOnlyList<string> columnNames)
	{
		Grid grid = new();
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
		foreach (string _ in columnNames)
		{
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		}
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		for (int index = 0; index < RarityNames.Length; index++)
		{
			grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(44) });
			TextBlock label = new()
			{
				Text = RarityNames[index],
				Foreground = RarityBrushes[index],
				FontWeight = FontWeights.SemiBold,
				VerticalAlignment = VerticalAlignment.Center
			};
			Grid.SetRow(label, index + 1);
			grid.Children.Add(label);
		}
		for (int index = 0; index < columnNames.Count; index++)
		{
			TextBlock header = new()
			{
				Text = columnNames[index],
				FontWeight = FontWeights.SemiBold,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(0, 0, 0, 8)
			};
			Grid.SetColumn(header, index + 1);
			grid.Children.Add(header);
		}
		return grid;
	}

	private static RateColumnEditor CreateColumnEditor(Grid grid, int column, string automationPrefix)
	{
		TextBox normalTextBox = CreateRateTextBox(readOnly: true, RarityBrushes[0], $"{automationPrefix}.Normal");
		AddRateCell(grid, normalTextBox, 1, column);

		List<TextBox> editableTextBoxes = new(4);
		for (int rarity = 1; rarity < RarityNames.Length; rarity++)
		{
			TextBox textBox = CreateRateTextBox(readOnly: false, RarityBrushes[rarity], $"{automationPrefix}.Rarity{rarity}");
			editableTextBoxes.Add(textBox);
			AddRateCell(grid, textBox, rarity + 1, column);
		}

		RateColumnEditor editor = new(normalTextBox, editableTextBoxes);
		foreach (TextBox textBox in editableTextBoxes)
		{
			textBox.TextChanged += (_, _) => editor.RefreshNormalRate();
		}
		return editor;
	}

	private static TextBox CreateRateTextBox(bool readOnly, Brush foreground, string automationId)
	{
		TextBox textBox = new()
		{
			Width = 118,
			Height = 28,
			IsReadOnly = readOnly,
			Foreground = foreground,
			VerticalContentAlignment = VerticalAlignment.Center,
			HorizontalContentAlignment = HorizontalAlignment.Right,
			Padding = new Thickness(6, 0, 6, 0)
		};
		AutomationProperties.SetAutomationId(textBox, $"DropRateManagement.{automationId}");
		return textBox;
	}

	private static void AddRateCell(Grid grid, TextBox textBox, int row, int column)
	{
		StackPanel panel = new()
		{
			Orientation = Orientation.Horizontal,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center
		};
		panel.Children.Add(textBox);
		panel.Children.Add(new TextBlock
		{
			Text = "%",
			Margin = new Thickness(6, 0, 0, 0),
			VerticalAlignment = VerticalAlignment.Center
		});
		Grid.SetRow(panel, row);
		Grid.SetColumn(panel, column);
		grid.Children.Add(panel);
	}

	private void LoadRates()
	{
		if (!pvf.FileAny(HellPath))
		{
			throw new InvalidOperationException($"深渊爆率文件不存在：{HellPath}");
		}
		if (!pvf.FileAny(ClearRewardPath))
		{
			throw new InvalidOperationException($"翻牌爆率文件不存在：{ClearRewardPath}");
		}

		hellText = pvf.GetFileText(HellPath);
		clearRewardText = pvf.GetFileText(ClearRewardPath);
		if (!DropRateDocument.TryParseHell(hellText, out DropRateDocument? hellDocument, out string hellError))
		{
			throw new InvalidOperationException($"无法读取 {HellPath}：{hellError}");
		}
		if (!DropRateDocument.TryParseClearReward(clearRewardText, out DropRateDocument? clearDocument, out string clearError))
		{
			throw new InvalidOperationException($"无法读取 {ClearRewardPath}：{clearError}");
		}

		for (int index = 0; index < hellEditors.Count; index++)
		{
			hellEditors[index].Load(hellDocument!.Columns[index]);
		}
		clearRewardEditor!.Load(clearDocument!.Columns[0]);
	}

	private void SaveRates()
	{
		List<int[]> hellColumns = new(2);
		for (int index = 0; index < hellEditors.Count; index++)
		{
			if (!hellEditors[index].TryBuildCumulative(out int[] values, out string error))
			{
				AppCore.ShowMsg($"深渊{(index == 0 ? "非常困难" : "困难")}：{error}", isError: true, caption: Title);
				return;
			}
			hellColumns.Add(values);
		}
		if (!clearRewardEditor!.TryBuildCumulative(out int[] clearValues, out string clearError))
		{
			AppCore.ShowMsg($"翻牌：{clearError}", isError: true, caption: Title);
			return;
		}

		string newHellText = DropRateDocument.ReplaceHellSection(hellText, hellColumns);
		string newClearText = DropRateDocument.ReplaceClearRewardSection(clearRewardText, clearValues);
		if (!pvf.SaveFileText(HellPath, newHellText))
		{
			AppCore.ShowMsg($"保存失败：{HellPath}", isError: true, caption: Title);
			return;
		}
		if (!pvf.SaveFileText(ClearRewardPath, newClearText))
		{
			pvf.SaveFileText(HellPath, hellText);
			AppCore.ShowMsg($"保存失败：{ClearRewardPath}。已尝试恢复深渊爆率文件。", isError: true, caption: Title);
			return;
		}

		hellText = newHellText;
		clearRewardText = newClearText;
		AppCore.ShowMsg("深渊/翻牌爆率保存成功。", caption: Title);
	}

	private static Brush CreateBrush(string color)
	{
		SolidColorBrush brush = new((Color)ColorConverter.ConvertFromString(color));
		brush.Freeze();
		return brush;
	}

	private sealed class RateColumnEditor
	{
		private readonly TextBox normalTextBox;
		private readonly IReadOnlyList<TextBox> editableTextBoxes;
		private DropRateColumn? sourceColumn;

		public RateColumnEditor(TextBox normalTextBox, IReadOnlyList<TextBox> editableTextBoxes)
		{
			this.normalTextBox = normalTextBox;
			this.editableTextBoxes = editableTextBoxes;
		}

		public void Load(DropRateColumn column)
		{
			sourceColumn = column;
			normalTextBox.Text = DropRateColumn.FormatPercentage(column.Rates[0]);
			for (int index = 0; index < editableTextBoxes.Count; index++)
			{
				editableTextBoxes[index].Text = DropRateColumn.FormatPercentage(column.Rates[index + 1]);
			}
			RefreshNormalRate();
		}

		public bool TryBuildCumulative(out int[] values, out string error)
		{
			if (sourceColumn == null)
			{
				values = Array.Empty<int>();
				error = "爆率数据尚未加载。";
				return false;
			}
			return sourceColumn.TryBuildCumulative(
				editableTextBoxes.Select(textBox => textBox.Text).ToArray(),
				out values,
				out error);
		}

		public void RefreshNormalRate()
		{
			if (TryBuildCumulative(out int[] values, out _))
			{
				normalTextBox.Text = DropRateColumn.FormatPercentage(values[0]);
			}
			else
			{
				normalTextBox.Text = "—";
			}
		}
	}
}
