using DG.Tweening;
using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Start Canvas")]
    [SerializeField] private CanvasGroup _startCanvasGroup;

    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private int _gameSceneNumber;

    [Header("Name Canvas")]
    [SerializeField] private CanvasGroup _nameCanvasGroup;

    [SerializeField] private TextMeshProUGUI _player1NameText;
    [SerializeField] private TextMeshProUGUI _player2NameText;
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private Button _nameButton;

    [Header("Turn")]
    [SerializeField] private Image _player1Turn;

    [SerializeField] private Image _player2Turn;

    [Header("Waiting Screen")]
    [SerializeField] private GameObject _WaitingScreen;

    [SerializeField] private Image _blackScreenImageForWaitingScreen;
    [SerializeField] private CanvasGroup _waitingCanvasGroup;
    [SerializeField] private Image _waitingImage;
    [SerializeField] private TextMeshProUGUI _waitingText;

    [Header("Result Screen")]
    [SerializeField] private GameObject _resultScreen;

    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private Image _blackScreenImageForResultScreen;
    [SerializeField] private CanvasGroup _resultCanvasGroup;
    [SerializeField] private TextMeshProUGUI _resultText;

    [Header("Result Image")]
    [SerializeField] private Image _resultImage;

    [SerializeField] private Sprite _winSprite;
    [SerializeField] private Sprite _drawSprite;

    [Header("Chat Box")]
    [SerializeField] private Button _chatButton;

    [SerializeField] private Image _blackScreenImageForChatBox;
    [SerializeField] private CanvasGroup _chatBox;

    [SerializeField] private Transform _chatHolder;
    [SerializeField] private GameObject _msgElement;
    [SerializeField] private TMP_InputField _playerMessageField;
    [SerializeField] private Button _enterButton;

    private RectTransform _resultRectTransform;

    #region Properties

    internal TextMeshProUGUI Player1NameText { get => _player1NameText; }
    internal TextMeshProUGUI Player2NameText { get => _player2NameText; }
    internal Transform ChatHolder { get => _chatHolder; }
    internal GameObject MsgElement { get => _msgElement; }
    internal TMP_InputField PlayerMessageField { get => _playerMessageField; }
    internal Button EnterButton { get => _enterButton; }

    #endregion Properties

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, true);

        _resultRectTransform = _resultCanvasGroup.GetComponent<RectTransform>();

        _WaitingScreen.SetActive(false);
        _resultScreen.SetActive(false);
        _chatBox.gameObject.SetActive(false);
        _chatButton.gameObject.SetActive(false);

        _startCanvasGroup.gameObject.GetComponent<Canvas>().enabled = true;
        _nameCanvasGroup.gameObject.GetComponent<Canvas>().enabled = false;

        _player1Turn.gameObject.SetActive(false);
        _player2Turn.gameObject.SetActive(false);

        _blackScreenImageForWaitingScreen.color = new Color(0, 0, 0, 0);
        _blackScreenImageForResultScreen.color = new Color(0, 0, 0, 0);
        _blackScreenImageForChatBox.color = new Color(0, 0, 0, 0);

        _waitingCanvasGroup.alpha = 0;
        _resultCanvasGroup.alpha = 0;
        _chatBox.alpha = 0;

        _player1NameText.text = string.Empty;
        _player2NameText.text = string.Empty;

        _resultText.text = string.Empty;
        _waitingText.text = string.Empty;

        _backgroundImage.transform.DORotate(new Vector3(0, 0, 360), 10f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    private void OnEnable()
    {
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
        _nameButton.onClick.AddListener(EnterTheGame);
    }

    private void OnDisable()
    {
        hostButton.onClick.RemoveListener(StartHost);
        clientButton.onClick.RemoveListener(StartClient);
        _nameButton.onClick.RemoveListener(EnterTheGame);
    }

    #region Start Screen

    private void StartHost()
    {
        InstanceFinder.ServerManager.StartConnection();

        FadeInStartCanvasAndNameCanvas(true);
    }

    private void StartClient()
    {
        FadeInStartCanvasAndNameCanvas(true);
    }

    private void FadeInStartCanvasAndNameCanvas(bool _fadeIn)
    {
        if (_fadeIn)
        {
            _startCanvasGroup.DOFade(0, _fadeDuration).OnComplete(() =>
            {
                _startCanvasGroup.GetComponent<Canvas>().enabled = false;

                _nameCanvasGroup.GetComponent<Canvas>().enabled = true;
                _nameCanvasGroup.alpha = 0;
                _nameCanvasGroup.DOFade(1, _fadeDuration);
            });
        }
        else
        {
            _nameCanvasGroup.DOFade(0, _fadeDuration).OnComplete(() =>
            {
                _nameCanvasGroup.GetComponent<Canvas>().enabled = false;
            });
        }
    }

    private void EnterTheGame()
    {
        InstanceFinder.ClientManager.StartConnection();

        // Make sure to handle if the user enter Empty Name
        LoadGameScene();
    }

    private void LoadGameScene()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(_gameSceneNumber);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe from the event to avoid multiple calls
        SceneManager.sceneLoaded -= OnSceneLoaded;

        FadeInStartCanvasAndNameCanvas(false);
    }

    #endregion Start Screen

    #region Turn Indicator

    internal void SetTurnIndicator(int _playerId)
    {
        if (_playerId == 0)
        {
            _player1Turn.gameObject.SetActive(true);
            _player2Turn.gameObject.SetActive(false);

            _player1Turn.transform.DORotate(new Vector3(0, 0, -360), 2f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }
        else
        {
            _player1Turn.gameObject.SetActive(false);
            _player2Turn.gameObject.SetActive(true);

            _player2Turn.transform.DORotate(new Vector3(0, 0, -360), 2f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }
    }

    #endregion Turn Indicator

    #region Waiting Screen

    internal void WaitingScreen(bool _isWaiting)
    {
        if (_isWaiting)
            ShowWaitingScreen();
        else
            HideWaitingScreen();
    }

    private void ShowWaitingScreen()
    {
        _WaitingScreen.SetActive(true);

        _blackScreenImageForWaitingScreen.DOFade(1f, 0.5f).OnComplete(() =>
        {
            _waitingCanvasGroup.alpha = 1;
            _waitingText.text = "Waiting for other player...";
            _waitingCanvasGroup.DOFade(1, _fadeDuration);

            // Start rotating the waiting image
            _waitingImage.transform.DORotate(new Vector3(0, 0, -360), 2f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        });
    }

    private void HideWaitingScreen()
    {
        _waitingCanvasGroup.DOFade(0, _fadeDuration);
        _waitingText.DOFade(0, _fadeDuration);

        _blackScreenImageForWaitingScreen.DOFade(0, _fadeDuration).OnComplete(() =>
        {
            _waitingText.text = string.Empty;
            _waitingImage.transform.DOKill();

            _WaitingScreen.SetActive(false);
        });
    }

    #endregion Waiting Screen

    #region Result Screen

    internal void ShowGameOver(string _resultText, bool _isWin)
    {
        this._resultText.text = _resultText;

        if (_isWin)
            _resultImage.sprite = _winSprite;
        else
            _resultImage.sprite = _drawSprite;

        FadeIn();
    }

    private void FadeIn()
    {
        _resultScreen.SetActive(true);

        _blackScreenImageForResultScreen.DOFade(1f, _fadeDuration).OnComplete(() =>
        {
            _resultCanvasGroup.alpha = 1;
            _resultRectTransform.transform.localPosition = new Vector3(0, -1000, 0);
            _resultRectTransform.DOAnchorPos(Vector2.zero, _fadeDuration, false).SetEase(Ease.OutElastic);
            _resultCanvasGroup.DOFade(1, _fadeDuration);

            // Color grading animation for _resultText
            _resultText.DOColor(Color.gray, _fadeDuration).SetLoops(-1, LoopType.Yoyo).From(Color.green);
        });
    }

    private void FadeOut()
    {
        _blackScreenImageForResultScreen.DOFade(0, _fadeDuration).OnComplete(() =>
        {
            _resultCanvasGroup.alpha = 0;
            _resultRectTransform.transform.localPosition = new Vector3(0, 0, 0);
            _resultRectTransform.DOAnchorPos(new Vector2(0, -1000), _fadeDuration, false).SetEase(Ease.InElastic);
            _resultCanvasGroup.DOFade(0, _fadeDuration);

            _resultScreen.SetActive(false);
        });
    }

    #endregion Result Screen

    #region Chat Box

    internal void ChatBox(bool _isChatBoxActive)
    {
        if (_isChatBoxActive)
            ShowChatBox();
        else
            HideChatBox();
    }

    internal void ShowChatButton(bool _isGameStarted)
    {
        if (_isGameStarted)
        {
            _chatButton.gameObject.SetActive(true);
            _chatButton.transform.DOScale(1.1f, 0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            _chatButton.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                _chatButton.gameObject.SetActive(false);
            });
        }
    }

    private void ShowChatBox()
    {
        _chatBox.gameObject.SetActive(true);

        _blackScreenImageForChatBox.DOFade(1f, 0.5f).OnComplete(() =>
        {
            _chatBox.DOFade(1, _fadeDuration);
        });
    }

    private void HideChatBox()
    {
        _chatBox.DOFade(0, _fadeDuration);

        _blackScreenImageForChatBox.DOFade(0, _fadeDuration).OnComplete(() =>
        {
            _chatBox.gameObject.SetActive(false);
        });
    }

    #endregion Chat Box

    #region Player Name

    private void OnNameButtonClicked()
    {
        var playerNameService = ServiceLocator.Instance.GetService<PlayerName>();
        if (playerNameService != null)
        {
            Logging.Log("PlayerName Service is found!");

            int playerId = InstanceFinder.ClientManager.Connection.ClientId; // Assuming playerId is 1 for this example
            string playerName = _nameInputField.text;

            Logging.Log($"PlayerId: {playerId}, PlayerName: {playerName}");
            playerNameService.SetPlayerName(playerId, playerName);
        }
        else
        {
            Logging.LogError("PlayerName Service is not found!");
        }
    }

    #endregion Player Name
}