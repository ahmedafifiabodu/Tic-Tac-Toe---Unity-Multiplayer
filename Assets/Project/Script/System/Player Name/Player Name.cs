using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerName : NetworkBehaviour
{
    private readonly SyncDictionary<int, string> _playerName = new();

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, true);
        Logging.Log("Awake called");
    }

    private void OnEnable()
    {
        _playerName.OnChange += OnPlayerNameChanged;
        Logging.Log("OnEnable called");
    }

    private void OnDestroy()
    {
        _playerName.OnChange -= OnPlayerNameChanged;
        Logging.Log("OnDisable called");
    }

    internal string GetPlayerName(int playerId)
    {
        Logging.Log($"GetPlayerName: {playerId}");
        return _playerName.TryGetValue(playerId, out var playerName) ? playerName : string.Empty;
    }

    internal void SetPlayerName(int playerId, string playerName)
    {
        Logging.Log($"SetPlayerName: {playerId}, {playerName}");
        _playerName[playerId] = playerName;
    }

    private void OnPlayerNameChanged(SyncDictionaryOperation op, int key, string value, bool asServer)
    {
        Logging.Log($"OnPlayerNameChanged: {op}, {key}, {value}, {asServer}");

        if (op == SyncDictionaryOperation.Add || op == SyncDictionaryOperation.Set)
        {
            RpcUpdatePlayerName(key, value);

            UpdatePlayerName(key, value);
        }
        else if (op == SyncDictionaryOperation.Remove)
        {
            _playerName.Remove(key);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RpcUpdatePlayerName(int playerId, string playerName)
    {
        Logging.Log($"RpcUpdatePlayerName called with: {playerId}, {playerName}");

        var uiManager = ServiceLocator.Instance.GetService<UIManager>();
        if (uiManager == null)
        {
            Logging.Log("UIManager service not found!");
            return;
        }

        if (playerId == -1 || playerId == 0)
        {
            uiManager.Player1NameText.text = playerName;
        }
        else if (playerId == 2)
        {
            uiManager.Player2NameText.text = playerName;
        }
    }

    private void UpdatePlayerName(int playerId, string playerName)
    {
        Logging.Log($"RpcUpdatePlayerName called with: {playerId}, {playerName}");

        var uiManager = ServiceLocator.Instance.GetService<UIManager>();
        if (uiManager == null)
        {
            Logging.Log("UIManager service not found!");
            return;
        }

        if (playerId == -1 || playerId == 0)
        {
            uiManager.Player1NameText.text = playerName;
        }
        else if (playerId == 2)
        {
            uiManager.Player2NameText.text = playerName;
        }
    }
}