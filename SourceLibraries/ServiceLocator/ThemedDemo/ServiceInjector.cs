using HL.Manager;
using ServiceLocator;

namespace ThemedDemo;

public static class ServiceInjector
{
	public static ServiceContainer InjectServices()
	{
		ServiceContainer.Instance.AddService(ThemedHighlightingManager.Instance);
		return ServiceContainer.Instance;
	}
}
