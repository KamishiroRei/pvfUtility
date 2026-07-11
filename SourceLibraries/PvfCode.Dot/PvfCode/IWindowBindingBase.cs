using System;

namespace PvfCode;

public interface IWindowBindingBase
{
	Action CloseAction { get; set; }
}
