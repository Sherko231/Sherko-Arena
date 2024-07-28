using System.Collections.Generic;

public static class OnlinePlayersRegistry
{
    private static readonly Dictionary<ulong, Player> _registry = new();
    private static readonly Dictionary<int, Player> _instanceIdRegistry = new();

    public static string ShowAll()
    {
        string reg = ",";
        foreach (Player player1 in _instanceIdRegistry.Values) reg += player1.gameObject.GetInstanceID() + ", ";
        return reg;
    }

    public static void Register(ulong clientId, Player player)
    {
        _registry.Add(clientId, player);
    }

    public static void RegisterInstanceId(int instanceId, Player player)
    {
        _instanceIdRegistry.Add(instanceId, player);
    }

    public static void Deregister(ulong clientId)
    {
        _registry.Remove(clientId);
    }

    public static void DeregisterInstanceId(int instanceId)
    {
        _instanceIdRegistry.Remove(instanceId);
    }

    public static bool Contains(ulong clientId)
    {
        return _registry.ContainsKey(clientId);
    }

    public static bool ContainsInstanceId(int instanceId)
    {
        return _instanceIdRegistry.ContainsKey(instanceId);
    }

    public static Player Get(ulong clientId)
    {
        return _registry[clientId];
    }

    public static Dictionary<ulong, Player> GetAll()
    {
        return _registry;
    }

    public static Player GetByInstanceId(int instanceId)
    {
        return _instanceIdRegistry[instanceId];
    }

    public static int Size()
    {
        return _registry.Count;
    }

    public static int InstanceIdSize()
    {
        return _instanceIdRegistry.Count;
    }

}
