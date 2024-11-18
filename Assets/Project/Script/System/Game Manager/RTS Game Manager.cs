using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RTSGameManager : NetworkBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int _playerCount = 2;

    [SerializeField] private int _movesPerTurn = 1;

    [Header("Game Board")]
    [SerializeField] private GameObject _gameBoard = null;

    [SerializeField] private Sprite _xSprite = null;
    [SerializeField] private Sprite _oSprite = null;

    private readonly SyncVar<GameState> _gameState = new(GameState.None);
    private readonly SyncVar<int> _currentPlayer = new(0);
    private readonly SyncVar<int> _connectedPlayers = new(0);

    private int _movesLeft;
    private Dictionary<int, HoverOverButtonsInGame> _hoverOverButtons;

    private UIManager _uIManager;

    private void Awake()
    {
        _hoverOverButtons = new();
        ServiceLocator.Instance.RegisterService(this, false);

        _currentPlayer.OnChange += OnCurrentPlayerChanged;
        _gameState.OnChange += OnGameStateChanged;
        _connectedPlayers.OnChange += OnConnectedPlayersChanged;
    }

    private void OnDestroy()
    {
        _currentPlayer.OnChange -= OnCurrentPlayerChanged;
        _gameState.OnChange -= OnGameStateChanged;
        _connectedPlayers.OnChange -= OnConnectedPlayersChanged;
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        _uIManager = ServiceLocator.Instance.GetService<UIManager>();
        CheckAndStartGame();
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

    private void OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            _connectedPlayers.Value = NetworkManager.ServerManager.Clients.Count;
            CheckAndStartGame();
        }
    }

    #region Game System

    private void SetupBoard()
    {
        foreach (Transform child in _gameBoard.transform)
        {
            if (child.TryGetComponent<Button>(out var button))
                button.onClick.AddListener(() => OnCellClicked(child));

            if (child.TryGetComponent<HoverOverButtonsInGame>(out var hoverOverButton))
                _hoverOverButtons[hoverOverButton.PositionIndex] = hoverOverButton;
        }
    }

    private void OnCellClicked(Transform cell)
    {
        if (_gameState.Value != GameState.PlayerTurn)
            return;

        if (GetCurrentPlayerId() != GetLocalPlayerId())
        {
            Logging.Log("It's not your turn!");
            return;
        }

        if (cell.TryGetComponent<HoverOverButtonsInGame>(out var hoverOverButton))
        {
            if (!hoverOverButton._isPlayed.Value)
            {
                hoverOverButton.ServerHandleButtonClick();
            }
            else
            {
                Logging.Log("Cell already played or invalid");
            }
        }
        else
        {
            Logging.Log("Cell does not have HoverOverButtons component");
        }
    }

    internal void ProcessMove()
    {
        _movesLeft--;

        if (CheckWinConditions())
            return;

        if (_movesLeft <= 0)
        {
            _currentPlayer.Value = (_currentPlayer.Value + 1) % _playerCount;
            _movesLeft = _movesPerTurn;
        }
    }

    private bool CheckWinConditions()
    {
        int[][] winningCombinations = new int[][]
        {
            new int[] { 1, 2, 3 }, // Top row
            new int[] { 4, 5, 6 }, // Middle row
            new int[] { 7, 8, 9 }, // Bottom row
            new int[] { 1, 4, 7 }, // Left column
            new int[] { 2, 5, 8 }, // Middle column
            new int[] { 3, 6, 9 }, // Right column
            new int[] { 1, 5, 9 }, // Diagonal from top-left to bottom-right
            new int[] { 3, 5, 7 }  // Diagonal from top-right to bottom-left
        };

        foreach (var combination in winningCombinations)
        {
            if (IsWinningCombination(combination))
            {
                _gameState.Value = GameState.GameOver;
                return true;
            }
        }

        if (IsBoardFull())
        {
            _gameState.Value = GameState.Draw;
            return true;
        }

        return false;
    }

    #endregion Game System

    #region Helper Methods

    #region Player Connection

    private void CheckAndStartGame()
    {
        if (_connectedPlayers.Value >= _playerCount)
        {
            _gameState.Value = GameState.PlayerTurn;
            SetupBoard();

            _uIManager.ShowChatButton(true);
            _uIManager.WaitingScreen(false);
        }
        else
        {
            _uIManager.WaitingScreen(true);
            Logging.Log("Waiting for more players to join...");
        }
    }

    #endregion Player Connection

    #region Win Conditions

    private bool IsWinningCombination(int[] combination)
    {
        if (!_hoverOverButtons.TryGetValue(combination[0], out var firstCell) || !firstCell._isPlayed.Value)
            return false;

        foreach (var index in combination)
        {
            if (!_hoverOverButtons.TryGetValue(index, out var cell) || cell._isPlayed.Value != firstCell._isPlayed.Value || cell.PlayerId != firstCell.PlayerId)
                return false;
        }

        return true;
    }

    private bool IsBoardFull()
    {
        foreach (var cell in _hoverOverButtons.Values)
        {
            if (!cell._isPlayed.Value)
                return false;
        }

        return true;
    }

    #endregion Win Conditions

    #region Hooks

    private void OnConnectedPlayersChanged(int oldCount, int newCount, bool asServer)
    {
        if (newCount >= _playerCount)
            CheckAndStartGame();
    }

    private void OnGameStateChanged(GameState oldState, GameState newState, bool asServer)
    {
        if (newState == GameState.PlayerTurn)
            UpdateTurnText();

        if (newState == GameState.GameOver)
        {
            _uIManager.ShowGameOver($"Player {_currentPlayer.Value + 1} Wins!", true);
            Logging.Log($"Player {_currentPlayer.Value + 1} Wins!");
        }

        if (newState == GameState.Draw)
        {
            _uIManager.ShowGameOver("It's a Draw!", false);
            Logging.Log("It's a Draw!");
        }
    }

    private void OnCurrentPlayerChanged(int oldPlayer, int newPlayer, bool asServer)
    {
        if (_gameState.Value == GameState.PlayerTurn)
            UpdateTurnText();
    }

    #endregion Hooks

    private void UpdateTurnText() => _uIManager.SetTurnIndicator(_currentPlayer.Value);

    internal Sprite GetCurrentPlayerSprite() => _currentPlayer.Value == 0 ? _xSprite : _oSprite;

    internal int GetCurrentPlayerId() => _currentPlayer.Value;

    internal int GetLocalPlayerId()
    {
        if (NetworkManager == null || NetworkManager.ClientManager == null)
        {
            Logging.LogWarning("NetworkManager or ClientManager is not initialized.");
            return -1;
        }

        var localConnection = NetworkManager.ClientManager.Connection;

        if (localConnection == null)
        {
            Logging.LogWarning("Local connection is not established.");
            return -1;
        }

        // Check if the connection is the host
        if (localConnection.IsHost)
        {
            return 0; // Assign a valid ID for the host
        }

        return localConnection.ClientId;
    }

    public bool IsClientManagerInitialized() => NetworkManager != null && NetworkManager.ClientManager != null && NetworkManager.ClientManager.Connection != null;

    #endregion Helper Methods
}