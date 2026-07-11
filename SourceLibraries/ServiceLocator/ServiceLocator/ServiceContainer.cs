using System;
using System.Collections.Generic;

namespace ServiceLocator;

public class ServiceContainer
{
	public static readonly ServiceContainer Instance = new ServiceContainer();

	private readonly Dictionary<Type, object> _serviceMap;

	private readonly object _serviceMapLock;

	private ServiceContainer()
	{
		_serviceMap = new Dictionary<Type, object>();
		_serviceMapLock = new object();
	}

	public void AddService<TServiceContract>(TServiceContract implementation) where TServiceContract : class
	{
		lock (_serviceMapLock)
		{
			_serviceMap[typeof(TServiceContract)] = implementation;
		}
	}

	public TServiceContract GetService<TServiceContract>() where TServiceContract : class
	{
		object value;
		lock (_serviceMapLock)
		{
			_serviceMap.TryGetValue(typeof(TServiceContract), out value);
		}
		return value as TServiceContract;
	}
}
