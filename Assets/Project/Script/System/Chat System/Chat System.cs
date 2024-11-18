using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Transporting;
using TMPro;
using UnityEngine;

public class ChatSystem : MonoBehaviour
{
    private UIManager _uIManager;
    private RTSGameManager _rTSGameManager;

    public struct Message : IBroadcast
    {
        public string _playerName;
        public string _message;
    }

    private void OnEnable()
    {
        InstanceFinder.ServerManager.RegisterBroadcast<Message>(OnClientMessageRecieved);
        InstanceFinder.ClientManager.RegisterBroadcast<Message>(OnMessageRecieved);

        _uIManager = ServiceLocator.Instance.GetService<UIManager>();
        _uIManager.EnterButton.onClick.AddListener(SendMessage);
    }

    private void OnDisable()
    {
        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.UnregisterBroadcast<Message>(OnClientMessageRecieved);

        if (InstanceFinder.ClientManager != null)
            InstanceFinder.ClientManager.UnregisterBroadcast<Message>(OnMessageRecieved);

        if (_uIManager != null && _uIManager.EnterButton != null)
            _uIManager.EnterButton.onClick.RemoveListener(SendMessage);
    }

    public void SendMessage()
    {
        if (_rTSGameManager == null)
            _rTSGameManager = ServiceLocator.Instance.GetService<RTSGameManager>();

        Message message = new()
        {
            _playerName = "Player",
            _message = _uIManager.PlayerMessageField.text
        };

        _uIManager.PlayerMessageField.text = string.Empty;

        if (InstanceFinder.IsServerStarted)
            InstanceFinder.ServerManager.Broadcast(message);
        else if (InstanceFinder.IsClientStarted)
            InstanceFinder.ClientManager.Broadcast(message);
    }

    private void OnMessageRecieved(Message message, Channel channel)
    {
        GameObject msgElement = Instantiate(_uIManager.MsgElement, _uIManager.ChatHolder);
        msgElement.GetComponentInChildren<TextMeshProUGUI>().text = $"{message._playerName}: {message._message}";
    }

    private void OnClientMessageRecieved(NetworkConnection connection, Message message, Channel channel) => InstanceFinder.ServerManager.Broadcast(message);
}