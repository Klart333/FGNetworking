using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerScore : MonoBehaviour
{
    [SerializeField]
    private Text idText;

    [SerializeField]
    private Text scoreText;

    public int ClientId { get; private set; }

    public void Setup(int clientID)
    {
        idText.text = clientID.ToString();
        ClientId = clientID;
    }

    public void SetScore(int amount)
    {
        scoreText.text = amount.ToString();
    }
}
