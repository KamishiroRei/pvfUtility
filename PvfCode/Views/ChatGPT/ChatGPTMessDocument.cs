using System;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using PvfCode.AiAssistant;
using PvfCode.ViewModels.DocumentFolder;

namespace PvfCode.Views.ChatGPT;

public class ChatGPTMessDocument : UserControl, IComponentConnector
{
	private sealed class InverseBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is not true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}

	private sealed class ZeroCountToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is int count && count == 0 ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}

	private readonly IValueConverter _inverseBooleanConverter = new InverseBooleanConverter();

	private bool GG6vqnyuOV;

	private PasswordBox _apiKeyBox;

	private TextBox _promptBox;

	private Button _sendButton;

	private ScrollViewer _messageScrollViewer;

	private ItemsControl _messageItems;

	private INotifyCollectionChanged _boundMessages;

	private bool _syncingApiKey;

	public ChatGPTMessDocument()
	{
		InitializeComponent();
		Padding = new Thickness(0);
		AutomationProperties.SetAutomationId(this, "AiAssistantConversationView");
		SetResourceReference(BackgroundProperty, "WindowBackColor");
		SetResourceReference(ForegroundProperty, "WindowForeground");
		Content = CreateLayout();
		DataContextChanged += OnDataContextChanged;
		Loaded += OnLoaded;
		Unloaded += OnUnloaded;
	}

	public void FocusPrompt()
	{
		Dispatcher.BeginInvoke((Action)(() =>
		{
			_promptBox?.Focus();
			Keyboard.Focus(_promptBox);
		}), DispatcherPriority.Input);
	}

	private UIElement CreateLayout()
	{
		Grid root = new Grid
		{
			Margin = new Thickness(10, 8, 10, 10),
			MinWidth = 0
		};
		root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
		root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		UIElement header = CreateHeader();
		Grid.SetRow(header, 0);
		root.Children.Add(header);

		UIElement settings = CreateConnectionSettings();
		Grid.SetRow(settings, 1);
		root.Children.Add(settings);

		UIElement messages = CreateMessagesPanel();
		Grid.SetRow(messages, 2);
		root.Children.Add(messages);

		UIElement composer = CreateComposer();
		Grid.SetRow(composer, 3);
		root.Children.Add(composer);
		return root;
	}

	private UIElement CreateHeader()
	{
		Grid header = new Grid
		{
			Margin = new Thickness(0, 0, 0, 6)
		};
		header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		StackPanel heading = new StackPanel
		{
			MinWidth = 0
		};
		TextBlock title = new TextBlock
		{
			Text = "AI 助手",
			FontSize = 14,
			FontWeight = FontWeights.SemiBold
		};
		title.SetResourceReference(TextBlock.ForegroundProperty, "WindowForeground");
		heading.Children.Add(title);

		TextBlock status = new TextBlock
		{
			Margin = new Thickness(0, 2, 0, 0),
			FontSize = 11,
			TextTrimming = TextTrimming.CharacterEllipsis
		};
		status.SetResourceReference(TextBlock.ForegroundProperty, "EditorNonPrintableCharacterBrush");
		status.SetBinding(TextBlock.TextProperty, new Binding("StatusText"));
		heading.Children.Add(status);
		header.Children.Add(heading);

		Button clearButton = CreateCommandButton("清空", "ClearWindowContent", "OnClearCommand");
		clearButton.MinWidth = 64;
		clearButton.Margin = new Thickness(8, 0, 0, 0);
		clearButton.SetBinding(IsEnabledProperty, new Binding("IsSending")
		{
			Converter = _inverseBooleanConverter
		});
		Grid.SetColumn(clearButton, 1);
		header.Children.Add(clearButton);
		return header;
	}

	private UIElement CreateConnectionSettings()
	{
		Expander expander = new Expander
		{
			Header = "连接设置",
			IsExpanded = false,
			Margin = new Thickness(0, 0, 0, 6),
			HorizontalContentAlignment = HorizontalAlignment.Stretch
		};
		AutomationProperties.SetName(expander, "连接设置");

		Border separator = new Border
		{
			Padding = new Thickness(0, 8, 0, 10),
			BorderThickness = new Thickness(0, 0, 0, 1)
		};
		separator.SetResourceReference(Border.BorderBrushProperty, "EditorFindKeyWordTextBoxBorderBrush");

		StackPanel settings = new StackPanel();
		TextBox endpointBox = CreateBoundTextBox("ApiEndpoint");
		settings.Children.Add(CreateField("API Endpoint", endpointBox));

		TextBox modelBox = CreateBoundTextBox("Model");
		settings.Children.Add(CreateField("模型", modelBox, new Thickness(0, 8, 0, 0)));

		_apiKeyBox = new PasswordBox
		{
			MinHeight = 30
		};
		ConfigureInput(_apiKeyBox);
		_apiKeyBox.PasswordChanged += OnApiKeyChanged;
		settings.Children.Add(CreateField("API Key", _apiKeyBox, new Thickness(0, 8, 0, 0)));

		TextBox tokenBox = CreateBoundTextBox("MaxOutputTokens");
		settings.Children.Add(CreateField("最大输出 Token", tokenBox, new Thickness(0, 8, 0, 0)));

		CheckBox allowPvfRead = new CheckBox
		{
			Content = "允许 AI 读取当前 PVF",
			Margin = new Thickness(0, 10, 0, 0),
			VerticalAlignment = VerticalAlignment.Center
		};
		AutomationProperties.SetName(allowPvfRead, "允许 AI 读取当前 PVF");
		allowPvfRead.SetResourceReference(ForegroundProperty, "WindowForeground");
		allowPvfRead.SetBinding(CheckBox.IsCheckedProperty, new Binding("AllowPvfRead")
		{
			Mode = BindingMode.TwoWay,
			UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
		});
		settings.Children.Add(allowPvfRead);

		Button saveButton = CreateCommandButton("保存设置", "Save_16x", "OnSaveSettingsCommand");
		saveButton.HorizontalAlignment = HorizontalAlignment.Right;
		saveButton.Margin = new Thickness(0, 10, 0, 0);
		settings.Children.Add(saveButton);

		separator.Child = settings;
		expander.Content = new ScrollViewer
		{
			Content = separator,
			MaxHeight = 360,
			HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto
		};
		return expander;
	}

	private UIElement CreateMessagesPanel()
	{
		Grid messagesPanel = new Grid
		{
			Margin = new Thickness(0, 2, 0, 8),
			MinHeight = 80
		};

		_messageItems = new ItemsControl
		{
			HorizontalContentAlignment = HorizontalAlignment.Stretch,
			ItemTemplate = CreateMessageTemplate()
		};
		_messageItems.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("Messages"));

		_messageScrollViewer = new ScrollViewer
		{
			Content = _messageItems,
			HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
			Padding = new Thickness(2, 0, 4, 0)
		};
		messagesPanel.Children.Add(_messageScrollViewer);

		TextBlock emptyState = new TextBlock
		{
			Text = "还没有对话",
			FontSize = 12,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			IsHitTestVisible = false
		};
		emptyState.SetResourceReference(TextBlock.ForegroundProperty, "EditorNonPrintableCharacterBrush");
		emptyState.SetBinding(VisibilityProperty, new Binding("Messages.Count")
		{
			Converter = new ZeroCountToVisibilityConverter()
		});
		messagesPanel.Children.Add(emptyState);
		return messagesPanel;
	}

	private UIElement CreateComposer()
	{
		Border separator = new Border
		{
			Padding = new Thickness(0, 10, 0, 0),
			BorderThickness = new Thickness(0, 1, 0, 0)
		};
		separator.SetResourceReference(Border.BorderBrushProperty, "EditorFindKeyWordTextBoxBorderBrush");

		Grid composer = new Grid();
		composer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		composer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		_promptBox = new TextBox
		{
			MinHeight = 68,
			MaxHeight = 144,
			AcceptsReturn = true,
			TextWrapping = TextWrapping.Wrap,
			VerticalContentAlignment = VerticalAlignment.Top,
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
			Padding = new Thickness(8, 7, 8, 7)
		};
		ConfigureInput(_promptBox);
		_promptBox.VerticalContentAlignment = VerticalAlignment.Top;
		AutomationProperties.SetName(_promptBox, "消息输入");
		AutomationProperties.SetAutomationId(_promptBox, "AiAssistantPrompt");
		_promptBox.SetBinding(TextBox.TextProperty, new Binding("Keyword")
		{
			Mode = BindingMode.TwoWay,
			UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
		});
		_promptBox.PreviewKeyDown += OnPromptKeyDown;
		composer.Children.Add(_promptBox);

		StackPanel actions = new StackPanel
		{
			Orientation = Orientation.Horizontal,
			HorizontalAlignment = HorizontalAlignment.Right,
			Margin = new Thickness(0, 8, 0, 0)
		};
		Button stopButton = CreateCommandButton("停止", "StopIcon", "OnCancelCommand");
		stopButton.MinWidth = 72;
		stopButton.SetBinding(IsEnabledProperty, new Binding("IsSending"));
		actions.Children.Add(stopButton);

		_sendButton = CreateCommandButton("发送", "chatGPTSend", "OnSendCommand", isPrimary: true);
		_sendButton.MinWidth = 72;
		_sendButton.Margin = new Thickness(8, 0, 0, 0);
		AutomationProperties.SetAutomationId(_sendButton, "AiAssistantSend");
		_sendButton.SetBinding(IsEnabledProperty, new Binding("IsSending")
		{
			Converter = _inverseBooleanConverter
		});
		actions.Children.Add(_sendButton);
		Grid.SetRow(actions, 1);
		composer.Children.Add(actions);

		separator.Child = composer;
		return separator;
	}

	private TextBox CreateBoundTextBox(string path)
	{
		TextBox textBox = new TextBox
		{
			MinHeight = 30,
			TextWrapping = TextWrapping.NoWrap
		};
		ConfigureInput(textBox);
		textBox.SetBinding(TextBox.TextProperty, new Binding(path)
		{
			Mode = BindingMode.TwoWay,
			UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
			ValidatesOnExceptions = true
		});
		return textBox;
	}

	private static FrameworkElement CreateField(string label, Control editor, Thickness? margin = null)
	{
		AutomationProperties.SetName(editor, label);
		StackPanel field = new StackPanel
		{
			Margin = margin ?? default
		};
		TextBlock caption = new TextBlock
		{
			Text = label,
			FontSize = 11,
			Margin = new Thickness(0, 0, 0, 4)
		};
		caption.SetResourceReference(TextBlock.ForegroundProperty, "EditorNonPrintableCharacterBrush");
		field.Children.Add(caption);
		field.Children.Add(editor);
		return field;
	}

	private static void ConfigureInput(Control input)
	{
		input.MinHeight = Math.Max(input.MinHeight, 30);
		input.Padding = input.Padding == default ? new Thickness(8, 5, 8, 5) : input.Padding;
		input.BorderThickness = new Thickness(1);
		input.VerticalContentAlignment = VerticalAlignment.Center;
		input.SetResourceReference(Control.BackgroundProperty, "EditorFindKeyWordTextBoxBackBrush");
		input.SetResourceReference(Control.ForegroundProperty, "WindowForeground");
		input.SetResourceReference(Control.BorderBrushProperty, "EditorFindKeyWordTextBoxBorderBrush");
	}

	private Button CreateCommandButton(string text, string iconResourceKey, string commandPath, bool isPrimary = false)
	{
		Button button = new Button
		{
			MinHeight = 30,
			MinWidth = 64,
			Padding = new Thickness(8, 5, 8, 5),
			VerticalAlignment = VerticalAlignment.Center,
			VerticalContentAlignment = VerticalAlignment.Center
		};
		AutomationProperties.SetName(button, text);
		button.SetBinding(Button.CommandProperty, new Binding(commandPath));
		if (isPrimary)
		{
			button.SetResourceReference(Control.BackgroundProperty, "ControlAccentBrushKey");
			button.Foreground = Brushes.White;
		}

		StackPanel content = new StackPanel
		{
			Orientation = Orientation.Horizontal,
			VerticalAlignment = VerticalAlignment.Center
		};
		Image icon = new Image
		{
			Width = 16,
			Height = 16,
			Stretch = Stretch.Uniform,
			Margin = new Thickness(0, 0, 5, 0)
		};
		icon.SetResourceReference(Image.SourceProperty, iconResourceKey);
		content.Children.Add(icon);
		content.Children.Add(new TextBlock
		{
			Text = text,
			VerticalAlignment = VerticalAlignment.Center
		});
		button.Content = content;
		return button;
	}

	private static DataTemplate CreateMessageTemplate()
	{
		FrameworkElementFactory bubble = new FrameworkElementFactory(typeof(Border), "Bubble");
		bubble.SetValue(FrameworkElement.MaxWidthProperty, 620.0);
		bubble.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
		bubble.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 8));
		bubble.SetValue(Border.PaddingProperty, new Thickness(10, 8, 10, 9));
		bubble.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
		bubble.SetValue(Border.BorderThicknessProperty, new Thickness(1));
		bubble.SetResourceReference(Border.BackgroundProperty, "NormalBackgroundBrushKey");
		bubble.SetResourceReference(Border.BorderBrushProperty, "EditorFindKeyWordTextBoxBorderBrush");

		FrameworkElementFactory body = new FrameworkElementFactory(typeof(StackPanel));
		FrameworkElementFactory meta = new FrameworkElementFactory(typeof(DockPanel));
		meta.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 4));

		FrameworkElementFactory timestamp = new FrameworkElementFactory(typeof(TextBlock), "Timestamp");
		timestamp.SetValue(DockPanel.DockProperty, Dock.Right);
		timestamp.SetValue(TextBlock.FontSizeProperty, 10.0);
		timestamp.SetValue(UIElement.OpacityProperty, 0.75);
		timestamp.SetBinding(TextBlock.TextProperty, new Binding("TimestampLabel"));
		timestamp.SetResourceReference(TextBlock.ForegroundProperty, "EditorNonPrintableCharacterBrush");
		meta.AppendChild(timestamp);

		FrameworkElementFactory role = new FrameworkElementFactory(typeof(TextBlock), "Role");
		role.SetValue(TextBlock.FontSizeProperty, 11.0);
		role.SetValue(TextBlock.FontWeightProperty, FontWeights.SemiBold);
		role.SetBinding(TextBlock.TextProperty, new Binding("RoleLabel"));
		role.SetResourceReference(TextBlock.ForegroundProperty, "ControlAccentBrushKey");
		meta.AppendChild(role);
		body.AppendChild(meta);

		FrameworkElementFactory content = new FrameworkElementFactory(typeof(TextBox), "MessageContent");
		content.SetValue(TextBox.IsReadOnlyProperty, true);
		content.SetValue(TextBox.AcceptsReturnProperty, true);
		content.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
		content.SetValue(TextBox.BorderThicknessProperty, new Thickness(0));
		content.SetValue(TextBox.PaddingProperty, new Thickness(0));
		content.SetValue(TextBox.BackgroundProperty, Brushes.Transparent);
		content.SetValue(TextBox.FontSizeProperty, 12.0);
		content.SetValue(TextBox.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
		content.SetValue(TextBox.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
		content.SetBinding(TextBox.TextProperty, new Binding("Content"));
		content.SetResourceReference(TextBox.ForegroundProperty, "WindowForeground");
		body.AppendChild(content);
		bubble.AppendChild(body);

		DataTemplate template = new DataTemplate(typeof(AiChatMessage))
		{
			VisualTree = bubble
		};

		DataTrigger userTrigger = new DataTrigger
		{
			Binding = new Binding("IsUser"),
			Value = true
		};
		userTrigger.Setters.Add(CreateTemplateSetter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Right, "Bubble"));
		userTrigger.Setters.Add(CreateTemplateSetter(Border.BackgroundProperty, new DynamicResourceExtension("ControlAccentBrushKey"), "Bubble"));
		userTrigger.Setters.Add(CreateTemplateSetter(Border.BorderBrushProperty, new DynamicResourceExtension("ControlAccentBrushKey"), "Bubble"));
		userTrigger.Setters.Add(CreateTemplateSetter(TextBlock.ForegroundProperty, Brushes.White, "Role"));
		userTrigger.Setters.Add(CreateTemplateSetter(TextBlock.ForegroundProperty, Brushes.White, "Timestamp"));
		userTrigger.Setters.Add(CreateTemplateSetter(TextBox.ForegroundProperty, Brushes.White, "MessageContent"));
		template.Triggers.Add(userTrigger);

		DataTrigger errorTrigger = new DataTrigger
		{
			Binding = new Binding("IsError"),
			Value = true
		};
		errorTrigger.Setters.Add(CreateTemplateSetter(Border.BorderBrushProperty, new DynamicResourceExtension("EditorFindKeyWordTextBoxErrorBorderBrush"), "Bubble"));
		errorTrigger.Setters.Add(CreateTemplateSetter(TextBlock.ForegroundProperty, new DynamicResourceExtension("EditorFindKeyWordTextBoxErrorBorderBrush"), "Role"));
		template.Triggers.Add(errorTrigger);
		return template;
	}

	private static Setter CreateTemplateSetter(DependencyProperty property, object value, string targetName)
	{
		return new Setter(property, value)
		{
			TargetName = targetName
		};
	}

	private void OnApiKeyChanged(object sender, RoutedEventArgs e)
	{
		if (!_syncingApiKey && DataContext is ChatGPTDocumentVm viewModel)
		{
			viewModel.ApiKey = _apiKeyBox.Password;
		}
	}

	private void OnPromptKeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key != Key.Enter || (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
		{
			return;
		}

		_promptBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
		ICommand command = _sendButton.Command;
		if (command?.CanExecute(null) == true)
		{
			command.Execute(null);
		}
		e.Handled = true;
	}

	private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		SynchronizeApiKey();
		BindMessages(e.NewValue as ChatGPTDocumentVm);
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		SynchronizeApiKey();
		BindMessages(DataContext as ChatGPTDocumentVm);
	}

	private void OnUnloaded(object sender, RoutedEventArgs e)
	{
		UnbindMessages();
	}

	private void SynchronizeApiKey()
	{
		if (_apiKeyBox == null || DataContext is not ChatGPTDocumentVm viewModel)
		{
			return;
		}

		_syncingApiKey = true;
		try
		{
			_apiKeyBox.Password = viewModel.ApiKey ?? string.Empty;
		}
		finally
		{
			_syncingApiKey = false;
		}
	}

	private void BindMessages(ChatGPTDocumentVm viewModel)
	{
		UnbindMessages();
		if (viewModel == null)
		{
			return;
		}

		_boundMessages = viewModel.Messages;
		_boundMessages.CollectionChanged += OnMessagesChanged;
		ScrollMessagesToEnd();
	}

	private void UnbindMessages()
	{
		if (_boundMessages != null)
		{
			_boundMessages.CollectionChanged -= OnMessagesChanged;
			_boundMessages = null;
		}
	}

	private void OnMessagesChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		ScrollMessagesToEnd();
	}

	private void ScrollMessagesToEnd()
	{
		if (_messageScrollViewer == null)
		{
			return;
		}

		Dispatcher.BeginInvoke((Action)(() =>
		{
			_messageItems.UpdateLayout();
			if (_messageItems.Items.Count == 0)
			{
				_messageScrollViewer.ScrollToHome();
			}
			else
			{
				_messageScrollViewer.ScrollToEnd();
			}
		}), DispatcherPriority.Background);
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!GG6vqnyuOV)
		{
			GG6vqnyuOV = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/chatgpt/chatgptmessdocument.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		GG6vqnyuOV = true;
	}
}
