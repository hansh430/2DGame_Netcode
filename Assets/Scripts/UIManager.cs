using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels/Buttons")]
    [SerializeField] private GameObject connectionPanel;
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button winNextButton;
    [SerializeField] private Button loseNextButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        ShowConnectionPanel();

        GameManager.OnGameStateChangesd += GameStateChangedCallback;
        hostButton.onClick.AddListener(OnClickHostButton);
        clientButton.onClick.AddListener(OnClickClientButton);
        winNextButton.onClick.AddListener(OnClickWinNextButton);
        loseNextButton.onClick.AddListener(OnClickLoseNextButton);
    }


    private void GameStateChangedCallback(GameState state)
    {
        switch (state)
        {
            case GameState.Game:
                ShowGamePanel();
                break;

            case GameState.Win:
                ShowWinPanel();
                break;

            case GameState.Lose:
                ShowLosePanel();
                break;
        }
    }

    private void ShowConnectionPanel()
    {
        connectionPanel.SetActive(true);
        waitingPanel.SetActive(false);
        gamePanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        joinPanel.SetActive(false);
    }
    private void ShowWaitingPanel()
    {
        waitingPanel.SetActive(true);
        connectionPanel.SetActive(false);
        joinPanel.SetActive(false);
        gamePanel.SetActive(false);
    }
    private void ShowGamePanel()
    {
        gamePanel.SetActive(true);
        connectionPanel.SetActive(false);
        waitingPanel.SetActive(false);
        joinPanel.SetActive(false);
    }
    private void ShowWinPanel()
    {
        winPanel.SetActive(true);
    }
    private void ShowLosePanel()
    {
        losePanel.SetActive(true);
    }
    private void OnClickHostButton()
    {
        // NetworkManager.Singleton.StartHost();
        RelayManager.Instance.StartCoroutine(RelayManager.Instance.ConfigureTransportAndStartNgoAsHost());
        ShowWaitingPanel();
    }
    private void OnClickClientButton()
    {
        //with lan
        /*  string ipAddress = IPManager.Instance.InputIP;
          UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
          utp.SetConnectionData(ipAddress, 7777);
          NetworkManager.Singleton.StartClient();*/

        // with relay
        RelayManager.Instance.StartCoroutine(RelayManager.Instance.ConfigureTransportAndStartNgoAsConnectingPlayer());
        ShowWaitingPanel();
    }
    private void OnClickWinNextButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        NetworkManager.Singleton.Shutdown();
    }
    private void OnClickLoseNextButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        NetworkManager.Singleton.Shutdown();
    }
    private void OnDestroy()
    {
        GameManager.OnGameStateChangesd -= GameStateChangedCallback;
        hostButton.onClick.RemoveAllListeners();
        clientButton.onClick.RemoveAllListeners();
        winNextButton.onClick.RemoveAllListeners();
        loseNextButton.onClick.RemoveAllListeners();
    }

}
