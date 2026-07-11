using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace PvfCode.ViewModels.Bars;

public abstract class ViewModel : ModelBase, IDisposable
{
	[CompilerGenerated]
	private string xB0xMtRjx3;

	[CompilerGenerated]
	private ImageSource xUaxVBdLNr;

	public string BindableName => Tcax8OPyYt(DisplayName);

	public virtual string DisplayName
	{
		[CompilerGenerated]
		get
		{
			return xB0xMtRjx3;
		}
		[CompilerGenerated]
		protected set
		{
			xB0xMtRjx3 = value;
		}
	}

	public virtual ImageSource Glyph
	{
		[CompilerGenerated]
		get
		{
			return xUaxVBdLNr;
		}
		[CompilerGenerated]
		set
		{
			xUaxVBdLNr = value;
		}
	}

	private string Tcax8OPyYt(string P_0)
	{
		return "_" + Regex.Replace(P_0, "\\W", "");
	}

	public void Dispose()
	{
		OnDispose();
	}

	protected virtual void OnDispose()
	{
	}

	protected ViewModel()
	{
	}
}
