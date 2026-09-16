using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using PvfCode.Models.Options;

namespace PvfCode.Views;

/// <summary>
/// 设置窗口“文件预览 → 自动打开预览”页：逐后缀决定打开该后缀文件时是否自动弹出右侧可视化预览页。
///
/// 该页用编译期 C# 构建控件树，而不是写进 views/windowpublicsetting.xaml：
/// 主程序在 Hybrid 资源模式下只有 app.xaml、themes/styles/iconsdark.xaml、views/viewscripteditor.xaml
/// 三个 XAML 参与编译，其余可读 XAML 被 Directory.Build.targets 分类为 None，只是原始 BAML 的镜像，
/// 新增模板写在里面不会进入运行时资源字典。仓库内既有同类做法见
/// DocumentItemContentTemplateSelector / EditorTooltipDataTemplateSelector。
/// </summary>
public sealed class PreviewAutoOpenSettingsView : UserControl
{
	private static readonly DataTemplate ExtensionItemTemplate = CreateExtensionItemTemplate();

	/// <summary>供设置窗口树节点注册的页面模板（SettingMenuItem.Data）。</summary>
	public static readonly DataTemplate SettingsTemplate = new DataTemplate
	{
		VisualTree = new FrameworkElementFactory(typeof(PreviewAutoOpenSettingsView))
	};

	public PreviewAutoOpenSettingsView()
	{
		PvfFilePreviewOptions options = AppSetting.Instance.PvfFilePreviewOptions;
		ItemsControl extensionList = new ItemsControl
		{
			Margin = new Thickness(0, 4, 0, 0),
			ItemTemplate = ExtensionItemTemplate,
			ItemsSource = options.AutoOpenExtensionSettings
		};
		GroupBox group = new GroupBox
		{
			Margin = new Thickness(0, 10, 0, 0),
			Header = "按后缀自动打开预览",
			Content = extensionList
		};
		StackPanel panel = new StackPanel();
		panel.Children.Add(CreateHint("在编辑器中打开或切换到下列后缀的文件时，是否自动在右侧打开对应的可视化预览页。取消勾选后该后缀不再自动打开，也不会把已打开的预览页切换到该文件；编辑器工具栏的“在侧边打开预览”按钮仍可手动打开。", null));
		panel.Children.Add(group);
		panel.Children.Add(CreateHint("全部取消勾选即完全关闭自动打开预览。此处只控制自动行为；悬停预览提示与预览配色仍由“文件预览”页管理，勾选后需点“保存”写入配置。", new Thickness(0, 10, 0, 0)));
		Content = new ScrollViewer
		{
			Margin = new Thickness(0, 10, 0, 0),
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
			Content = panel
		};
	}

	private static TextBlock CreateHint(string text, Thickness? margin)
	{
		return new TextBlock
		{
			Text = text,
			TextWrapping = TextWrapping.Wrap,
			Foreground = new SolidColorBrush(Color.FromRgb(128, 128, 128)),
			Margin = margin ?? new Thickness(0)
		};
	}

	private static DataTemplate CreateExtensionItemTemplate()
	{
		FrameworkElementFactory checkBox = new FrameworkElementFactory(typeof(CheckBox));
		checkBox.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 4, 0, 0));
		checkBox.SetBinding(ContentControl.ContentProperty, new Binding(nameof(PvfPreviewExtensionToggle.DisplayText)));
		checkBox.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(PvfPreviewExtensionToggle.IsEnabled))
		{
			Mode = BindingMode.TwoWay,
			UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
		});
		return new DataTemplate
		{
			VisualTree = checkBox
		};
	}
}
