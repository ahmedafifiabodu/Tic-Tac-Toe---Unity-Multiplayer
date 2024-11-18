using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System;
using UnityEngine;

public class PlayerNameDeprecated : NetworkBehaviour
{
    public static event Action<int, string> OnNameAdd;

    private readonly SyncDictionary<int, string> _playerName = new();

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, true);
        _playerName.OnChange += OnChangePlayerName;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
    }

    private void OnRemoteConnectionState(NetworkConnection connection, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState != RemoteConnectionState.Started)
            _playerName.Remove(connection.ClientId);
    }

    private void OnChangePlayerName(SyncDictionaryOperation op, int key, string value, bool asServer)
    {
        if (op == SyncDictionaryOperation.Add || op == SyncDictionaryOperation.Set)
        {
            Debug.Log($"[OnChangePlayerName] Player name updated for connection ID {key}: {value}");
            OnNameAdd?.Invoke(key, value);
        }
    }

    public string GetPlayerName(int _connectionID)
    {
        if (ServiceLocator.Instance.GetService<PlayerNameDeprecated>()._playerName.TryGetValue(_connectionID, out string name))
            return name;
        else
            return string.Empty;
    }

    [Client]
    public static void SetPlayerName(int _connectionID, string name)
    {
        var playerNameService = ServiceLocator.Instance.GetService<PlayerNameDeprecated>();
        playerNameService.SetPlayerNameServerRpc(_connectionID, name);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(int _connectionID, string name)
    {
        Debug.Log($"[Server] Setting player name for connection ID {_connectionID} to: {name}");
        _playerName[_connectionID] = name;
    }
}