using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UnitComboLib;

[TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
[Localizability(LocalizationCategory.ComboBox)]
[TemplatePart(Name = "PART_EditableTextBox", Type = typeof(TextBox))]
[StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ComboBoxItem))]
public class UnitCombobox : ComboBox
{
	static UnitCombobox()
	{
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(UnitCombobox), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)typeof(UnitCombobox)));
	}
}
