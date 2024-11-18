using FishNet;
using FishNet.Connection;
using FishNet.Object;

public class UIManagerNetworkedDeprecated : NetworkBehaviour
{
    private UIManager _uiManager;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, true);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        SetPlayerName();
        PlayerNameDeprecated.OnNameAdd += OnNameAdd;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        PlayerNameDeprecated.OnNameAdd -= OnNameAdd;
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);

        SetPlayerName();
    }

    private void OnNameAdd(int _connectionID, string arg2)
    {
        //if (connection != Owner)
        //    return;

        SetPlayerName();
    }

    private void SetPlayerName()
    {
        string _playerName = string.Empty;

        if (Owner.IsValid)
            _playerName = InstanceFinder.GetInstance<PlayerNameDeprecated>().GetPlayerName(Owner.ClientId);

        if (string.IsNullOrEmpty(_playerName))
            _playerName = "Unset";

        if (_uiManager == null)
            _uiManager = ServiceLocator.Instance.GetService<UIManager>();

        Logging.Log($"Setting player name to: {_playerName}");

        //_uiManager.UpdatePlayerNames(NetworkManager.ClientManager.Connection.ClientId, _playerName);
    }
}