using UnityEngine;
using Photon.Pun;
using TMPro;

public class CollectedCoins : MonoBehaviourPunCallbacks
{
    public int player1Score = 0;
    public int player2Score = 0;

    public TextMeshProUGUI textScore1;
    public TextMeshProUGUI textScore2;

    private void Update()
    {
        if (PlayerPrefs.HasKey("Player1Score"))
        {
            textScore1.text = PlayerPrefs.GetInt("Player1Score").ToString();
        }

        if (PlayerPrefs.HasKey("Player2Score"))
        {
            textScore2.text = PlayerPrefs.GetInt("Player2Score").ToString();
        }
    }

    [PunRPC]
    public void AddScore(int playerID)
    {

        if (playerID == 1)
        {
            player1Score++;
            PlayerPrefs.SetInt("Player1Score", player1Score);
            PlayerPrefs.GetInt("Player1Score");
        }
        else if (playerID == 2)
        {
            player2Score++;
            PlayerPrefs.SetInt("Player2Score", player2Score);
            PlayerPrefs.GetInt("Player2Score");
        }
    }
}
