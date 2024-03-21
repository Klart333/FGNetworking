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

    public void Setup(string name, int clientID)
    {
        idText.text = name;
        ClientId = clientID;
    }

    public void SetScore(int amount)
    {
        scoreText.text = amount.ToString();
    }
}
