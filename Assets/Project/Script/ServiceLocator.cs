using System;
using System.Collections.Generic;

public class ServiceLocator
{
    private readonly IDictionary<object, (object service, bool dontDestroyOnLoad)> services = new Dictionary<object, (object service, bool dontDestroyOnLoad)>();

    private static ServiceLocator _instance;

    public static ServiceLocator Instance
    {
        get
        {
            _instance ??= new ServiceLocator();
            return _instance;
        }
    }

    // Get a service of the specified type.
    public T GetService<T>()
    {
        if (services.TryGetValue(typeof(T), out var serviceTuple))
            return (T)serviceTuple.service;
        else
            throw new ApplicationException("The requested service is not registered");
    }

    // Try to get a service of the specified type.
    public bool TryGetService<T>(out T service)
    {
        service = default;
        if (services.TryGetValue(typeof(T), out var serviceTuple))
        {
            service = (T)serviceTuple.service;
            return true;
        }
        else
            return false;
    }

    // Register a service of the specified type.
    public void RegisterService<T>(T service, bool dontDestroyOnLoad)
    {
        Type serviceType = typeof(T);

        if (services.ContainsKey(serviceType))
        {
            Logging.LogWarning($"Service of type {serviceType.Name} is already registered.");

            if (service is UnityEngine.Component existingComponent)
                UnityEngine.Object.Destroy(existingComponent.gameObject);
            else if (service is UnityEngine.GameObject existingGameObject)
                UnityEngine.Object.Destroy(existingGameObject);

            return;
        }

        if (service is UnityEngine.Component newComponent)
        {
            if (dontDestroyOnLoad && newComponent.transform.parent != null)
            {
                newComponent.transform.parent = null;
                UnityEngine.Object.DontDestroyOnLoad(newComponent.gameObject);
            }
        }
        else if (service is UnityEngine.GameObject newGameObject)
        {
            if (dontDestroyOnLoad && newGameObject.transform.parent != null)
            {
                newGameObject.transform.parent = null;
                UnityEngine.Object.DontDestroyOnLoad(newGameObject);
            }
        }

        services[typeof(T)] = (service, dontDestroyOnLoad);
    }

    // Unregister all services that are not marked as DontDestroyOnLoad.
    private void UnregisterNonPersistentServices()
    {
        foreach (var service in new List<object>(services.Keys))
        {
            if (!services[service].dontDestroyOnLoad)
                services.Remove(service);
        }
    }

    // Unregister a service of the specified type.
    public void UnregisterService<T>()
    {
        Type serviceType = typeof(T);

        if (services.ContainsKey(serviceType))
            services.Remove(serviceType);
        else
            Logging.LogWarning($"Attempted to unregister service of type {serviceType.Name}, but it was not registered.");
    }

    // Get all services that are marked as DontDestroyOnLoad.
    public List<object> GetDontDestroyOnLoadServices()
    {
        List<object> dontDestroyOnLoadServices = new();

        foreach (var (service, dontDestroyOnLoad) in services.Values)
        {
            if (dontDestroyOnLoad)
                dontDestroyOnLoadServices.Add(service);
        }

        return dontDestroyOnLoadServices;
    }

    // Private constructor to prevent instantiation.
    private ServiceLocator() => UnityEngine.SceneManagement.SceneManager.sceneUnloaded += scene => UnregisterNonPersistentServices();
}