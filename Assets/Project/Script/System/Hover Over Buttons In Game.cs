using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverOverButtonsInGame : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int _positionIndex;
    [SerializeField] private Image _buttonImage;

    //[SyncVar(OnChange = nameof(OnIsPlayedChanged))]
    internal readonly SyncVar<bool> _isPlayed = new(false);

    //[SyncVar(OnChange = nameof(OnPlayerIdChanged))]
    internal readonly SyncVar<int> _playerId = new(-1);

    private RTSGameManager _gameManager;

    internal Image ButtonImage => _buttonImage;

    internal int PositionIndex => _positionIndex;

    internal int PlayerId => _playerId.Value;

    private bool _playerIdChangedHandled = false;

    private void Awake()
    {
        _isPlayed.OnChange += OnIsPlayedChanged;
        _playerId.OnChange += OnPlayerIdChanged;
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        _gameManager = ServiceLocator.Instance.GetService<RTSGameManager>();

        // Set the transparency to 0%
        Color color = _buttonImage.color;
        color.a = 0f;
        _buttonImage.color = color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isPlayed.Value && _gameManager != null && _buttonImage != null)
        {
            // Check if it's the local player's turn
            if (_gameManager.GetCurrentPlayerId() != _gameManager.GetLocalPlayerId())
            {
                Color _buttoncolor = _buttonImage.color;
                _buttoncolor.a = 0f;
                _buttonImage.color = _buttoncolor;

                return;
            }

            // Set the button's sprite to the current player's sprite
            _buttonImage.sprite = _gameManager.GetCurrentPlayerSprite();

            // Set the transparency to 50%
            Color color = _buttonImage.color;
            color.a = 0.5f;
            _buttonImage.color = color;

            // Animate the button scale to 1.1x its original size
            gameObject.transform.DOScale(1.1f, 0.3f).SetEase(Ease.OutBack);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_gameManager == null || _buttonImage == null)
        {
            Debug.LogWarning("GameManager or ButtonImage is not set.");
            return;
        }

        if (!_isPlayed.Value)
        {
            // Check if it's the local player's turn
            if (_gameManager.GetCurrentPlayerId() != _gameManager.GetLocalPlayerId())
            {
                Color _buttoncolor = _buttonImage.color;
                _buttoncolor.a = 0f;
                _buttonImage.color = _buttoncolor;

                return;
            }

            // Set the transparency to 0%
            Color color = _buttonImage.color;
            color.a = 0f;
            _buttonImage.color = color;

            // Animate the button scale to 1.1x its original size
            gameObject.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerHandleButtonClick()
    {
        if (!_isPlayed.Value)
        {
            // Update the state on the server
            _isPlayed.Value = true;
            _playerId.Value = _gameManager.GetCurrentPlayerId();

            // Process the move, update the game board, and check for win conditions.
            _gameManager.ProcessMove();
        }
    }

    private void OnIsPlayedChanged(bool oldValue, bool newValue, bool asServer)
    {
        // Ensure this is only processed on the client side or if the host is acting as a client
        if (!IsServerInitialized || IsHostInitialized)
        {
            if (newValue)
            {
                // Set the transparency to 100%
                Color color = _buttonImage.color;
                color.a = 1f;
                _buttonImage.color = color;
            }
            else
            {
                // Set the transparency to 0%
                Color color = _buttonImage.color;
                color.a = 0f;
                _buttonImage.color = color;
            }
        }
    }

    private void OnPlayerIdChanged(int oldValue, int newValue, bool asServer)
    {
        // Check if this method has already been handled
        if (_playerIdChangedHandled)
            return;

        // Ensure this is only processed on the client side or if the host is acting as a client
        if (!IsServerInitialized || IsHostInitialized)
        {
            // Set the button's sprite to the correct player's sprite
            if (_gameManager != null)
                _buttonImage.sprite = _gameManager.GetCurrentPlayerSprite();
        }

        // Mark this method as handled
        _playerIdChangedHandled = true;
    }
}