using System.IO;

namespace ICSharpCode.AvalonEdit.Highlighting;

internal static class Resources
{
	private static readonly string Prefix = typeof(Resources).FullName + ".";

	public static Stream OpenStream(string name)
	{
		return typeof(Resources).Assembly.GetManifestResourceStream(Prefix + name) ?? throw new FileNotFoundException("The resource file '" + name + "' was not found.");
	}

	internal static void RegisterBuiltInHighlightings(HighlightingManager.DefaultHighlightingManager hlm)
	{
		hlm.RegisterHighlighting("XmlDoc", null, "XmlDoc.xshd");
		hlm.RegisterHighlighting("C#", new string[1] { ".cs" }, "CSharp-Mode.xshd");
		hlm.RegisterHighlighting("JavaScript", new string[1] { ".js" }, "JavaScript-Mode.xshd");
		hlm.RegisterHighlighting("HTML", new string[2] { ".htm", ".html" }, "HTML-Mode.xshd");
		hlm.RegisterHighlighting("ASP/XHTML", new string[6] { ".asp", ".aspx", ".asax", ".asmx", ".ascx", ".master" }, "ASPX.xshd");
		hlm.RegisterHighlighting("Boo", new string[1] { ".boo" }, "Boo.xshd");
		hlm.RegisterHighlighting("Coco", new string[1] { ".atg" }, "Coco-Mode.xshd");
		hlm.RegisterHighlighting("CSS", new string[1] { ".css" }, "CSS-Mode.xshd");
		hlm.RegisterHighlighting("C++", new string[5] { ".c", ".h", ".cc", ".cpp", ".hpp" }, "CPP-Mode.xshd");
		hlm.RegisterHighlighting("Java", new string[1] { ".java" }, "Java-Mode.xshd");
		hlm.RegisterHighlighting("Patch", new string[2] { ".patch", ".diff" }, "Patch-Mode.xshd");
		hlm.RegisterHighlighting("PowerShell", new string[3] { ".ps1", ".psm1", ".psd1" }, "PowerShell.xshd");
		hlm.RegisterHighlighting("PHP", new string[1] { ".php" }, "PHP-Mode.xshd");
		hlm.RegisterHighlighting("Python", new string[2] { ".py", ".pyw" }, "Python-Mode.xshd");
		hlm.RegisterHighlighting("TeX", new string[1] { ".tex" }, "Tex-Mode.xshd");
		hlm.RegisterHighlighting("TSQL", new string[1] { ".sql" }, "TSQL-Mode.xshd");
		hlm.RegisterHighlighting("VB", new string[1] { ".vb" }, "VB-Mode.xshd");
		hlm.RegisterHighlighting("XML", ".xml;.xsl;.xslt;.xsd;.manifest;.config;.addin;.xshd;.wxs;.wxi;.wxl;.proj;.csproj;.vbproj;.ilproj;.booproj;.build;.xfrm;.targets;.xaml;.xpt;.xft;.map;.wsdl;.disco;.ps1xml;.nuspec".Split(';'), "XML-Mode.xshd");
		hlm.RegisterHighlighting("MarkDown", new string[1] { ".md" }, "MarkDown-Mode.xshd");
		hlm.RegisterHighlighting("MarkDownWithFontSize", new string[1] { ".md" }, "MarkDownWithFontSize-Mode.xshd");
		hlm.RegisterHighlighting("Json", new string[1] { ".json" }, "Json.xshd");
	}
}
