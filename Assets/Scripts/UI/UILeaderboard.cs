using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class UILeaderboard : NetworkBehaviour
{
    [SerializeField]
    private UIPlayerScore playerScore;

    private List<UIPlayerScore> scores = new List<UIPlayerScore>();

    private List<PlayerController> playerControllers = new List<PlayerController>();

    private void Awake()
    {
        NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager_OnClientConnectedCallback(0);
    }

    private void OnDisable()
    {
        NetworkManager.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
    }

    private async void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        await Task.Yield();
        var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        for (int i = 0; i < players.Length; i++)
        {
            if (!playerControllers.Contains(players[i]))
            {
                var score = Instantiate(playerScore, transform);
                score.Setup((int)players[i].OwnerClientId);

                scores.Add(score);
                playerControllers.Add(players[i]);
                players[i].Score.OnValueChanged += (oldValue, newValue) => 
                {
                    score.SetScore(newValue);

                    SortScores();
                };
            }
        }
    }

    private void SortScores()
    {
        playerControllers.Sort((x, y) => y.Score.Value.CompareTo(x.Score.Value));

        for (int i = 0; i < playerControllers.Count; i++)
        {
            scores.Where((x) => x.ClientId == (int)playerControllers[i].OwnerClientId).FirstOrDefault().transform.SetSiblingIndex(i); // lol
        }
    }
}
