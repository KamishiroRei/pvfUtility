using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace UnitComboLib.Local;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
public class Strings
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("UnitComboLib.Local.Strings", typeof(Strings).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	public static string Enter_Font_Size_InRange_Message => ResourceManager.GetString("Enter_Font_Size_InRange_Message", resourceCulture);

	public static string Enter_Percent_Font_Size_InRange_Message => ResourceManager.GetString("Enter_Percent_Font_Size_InRange_Message", resourceCulture);

	public static string Enter_Percent_Size_InRange_Message => ResourceManager.GetString("Enter_Percent_Size_InRange_Message", resourceCulture);

	public static string Integer_Contain_ErrorMessage => ResourceManager.GetString("Integer_Contain_ErrorMessage", resourceCulture);

	public static string Integer_Conversion_ErrorMessage => ResourceManager.GetString("Integer_Conversion_ErrorMessage", resourceCulture);

	public static string Percent_String => ResourceManager.GetString("Percent_String", resourceCulture);

	public static string Percent_String_Short => ResourceManager.GetString("Percent_String_Short", resourceCulture);

	public static string Point_String => ResourceManager.GetString("Point_String", resourceCulture);

	public static string Point_String_Short => ResourceManager.GetString("Point_String_Short", resourceCulture);

	internal Strings()
	{
	}
}
