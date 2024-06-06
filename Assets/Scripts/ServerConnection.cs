using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServerConnection : NetworkBehaviour
{
    [SerializeField] private GameObject serverErrorPanel;
    [SerializeField] private Button restartButton;
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
        restartButton.onClick.AddListener(RestartGame);
    }

    private void RestartGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {

        Debug.Log("Client disconnected from server...");
        serverErrorPanel.SetActive(true);
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            NetworkManager.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        }
    }
}
