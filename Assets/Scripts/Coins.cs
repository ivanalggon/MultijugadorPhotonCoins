using UnityEngine;
using Photon.Pun;

public class Coin : MonoBehaviourPunCallbacks
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            int playerID = (other.CompareTag("Player1")) ? 1 : 2;

            Debug.Log($"[Coin] Moneda recogida por {other.tag} (ID: {playerID})");

            CollectedCoins collectedCoins = FindObjectOfType<CollectedCoins>();
            if (collectedCoins != null)
            {
                collectedCoins.AddScore(playerID);
            }
            Debug.Log($"[Coin] Moneda recogida por {other.tag} (ID: {playerID})");
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
