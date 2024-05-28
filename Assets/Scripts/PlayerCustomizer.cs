using Unity.Netcode;
using UnityEngine;

public class PlayerCustomizer : NetworkBehaviour
{
    [Header("Elements")]
    [SerializeField] private SpriteRenderer[] renderers;

    public override void OnNetworkSpawn()
    {
        if (!IsServer && IsOwner)
            ColorizeServerRpc(Color.red);
    }
    [ServerRpc]
    private void ColorizeServerRpc(Color color)
    {
        ColorizeClientRpc(color);
    }

    [ClientRpc]
    private void ColorizeClientRpc(Color color)
    {
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.color = color;
        }

    }
}
