using System;
using System.Reflection;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Bars;

namespace PvfCode.Compatibility;

internal static class DevExpressNet10Compatibility
{
	private const BindingFlags InstanceField = BindingFlags.Instance | BindingFlags.NonPublic;
	private const BindingFlags StaticField = BindingFlags.Static | BindingFlags.NonPublic;

	internal static void Apply()
	{
		if (Environment.Version.Major < 10)
		{
			return;
		}

		FieldInfo popupRootField = typeof(Popup).GetField("_popupRoot", InstanceField);
		FieldInfo devExpressGetterField = typeof(BarPopupBase).GetField("getPopupRoot", StaticField);
		FieldInfo devExpressValueGetterField = typeof(BarPopupBase).GetField("getPopupRootValue", StaticField);
		if (popupRootField == null || devExpressGetterField == null || devExpressValueGetterField == null)
		{
			return;
		}

		MethodInfo legacyValueGetter = popupRootField.FieldType.GetMethod(
			"get_Value",
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (legacyValueGetter != null)
		{
			return;
		}

		Func<Popup, object> popupRootGetter = popup => popupRootField.GetValue(popup);
		Func<object, object> directValueGetter = value => value;
		devExpressGetterField.SetValue(null, popupRootGetter);
		devExpressValueGetterField.SetValue(null, directValueGetter);
	}
}
