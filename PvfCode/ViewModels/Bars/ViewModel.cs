using System;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace PvfCode.ViewModels.Bars;

public abstract class ViewModel : ModelBase, IDisposable
{
	public string BindableName => CreateBindableName(DisplayName);

	public virtual string DisplayName { get; protected set; }

	public virtual ImageSource Glyph { get; set; }

	private string CreateBindableName(string displayName)
	{
		return "_" + Regex.Replace(displayName, "\\W", "");
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
