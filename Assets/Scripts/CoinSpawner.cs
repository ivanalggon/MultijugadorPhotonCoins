using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class CoinSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject coinPrefab;
    [SerializeField]
    private int coinCount = 10;
    [SerializeField]
    private float spawnInterval = 10f;
    private List<GameObject> _coins = new List<GameObject>();

    private float minX, maxX, spawnY;

    public void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            AdjustSpawnRange();
            StartCoroutine(SpawnLoop());
        }
    }

    private void AdjustSpawnRange()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            float camHalfWidth = cam.orthographicSize * cam.aspect;
            minX = cam.transform.position.x - camHalfWidth + 1f;
            maxX = cam.transform.position.x + camHalfWidth - 1f;
            spawnY = cam.transform.position.y - 1.5f; // Altura monedas
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                CleanCoins();
                SpawnCoins();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnCoins()
    {
        for (int i = 0; i < coinCount; i++)
        {
            Vector2 spawnPosition;
            bool positionIsValid;

            do
            {
                spawnPosition = new Vector2(Random.Range(minX, maxX), spawnY);
                positionIsValid = !Physics2D.OverlapCircle(spawnPosition, 0.5f);
            }
            while (!positionIsValid);

            GameObject newCoin = PhotonNetwork.InstantiateRoomObject(
                coinPrefab.name,
                spawnPosition,
                Quaternion.identity
            );
            _coins.Add(newCoin);
        }
    }

    private void CleanCoins()
    {
        foreach (var coin in _coins.Where(coin => coin != null && coin.GetComponent<PhotonView>().IsMine))
        {
            PhotonNetwork.Destroy(coin);
        }
        _coins.Clear();
    }
}
