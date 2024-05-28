using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject connectionPanel;
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Start()
    {
        ShowConnectionPanel();

        GameManager.OnGameStateChangesd += GameStateChangedCallback;
        hostButton.onClick.AddListener(OnClickHostButton);
        clientButton.onClick.AddListener(OnClickClientButton);
    }

    private void GameStateChangedCallback(GameState state)
    {
        switch (state)
        {
            case GameState.Game:
                ShowGamePanel();
                break;
        }
    }

    private void ShowConnectionPanel()
    {
        connectionPanel.SetActive(true);
        waitingPanel.SetActive(false);
        gamePanel.SetActive(false);
    }
    private void ShowWaitingPanel()
    {
        waitingPanel.SetActive(true);
        connectionPanel.SetActive(false);
        gamePanel.SetActive(false);
    }
    private void ShowGamePanel()
    {
        gamePanel.SetActive(true);
        connectionPanel.SetActive(false);
        waitingPanel.SetActive(false);
    }
    private void OnClickHostButton()
    {
        ShowWaitingPanel();
        NetworkManager.Singleton.StartHost();
    }
    private void OnClickClientButton()
    {
        ShowWaitingPanel();
        NetworkManager.Singleton.StartClient();
    }
    private void OnDestroy()
    {
        GameManager.OnGameStateChangesd -= GameStateChangedCallback;
        hostButton.onClick.RemoveAllListeners();
        clientButton.onClick.RemoveAllListeners();
    }

}
