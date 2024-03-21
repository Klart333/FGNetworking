using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : NetworkBehaviour
{
    [SerializeField]
    private Text nameText;

    public NetworkVariable<FixedString64Bytes> NameNetworkVariable = new NetworkVariable<FixedString64Bytes>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);

    private async void Start()
    {
        if (IsOwner)
        {
            SetNameServerRpc(PlayerPrefs.GetString("userName"));

            PlayerName[] otherPlayers = FindObjectsByType<PlayerName>(FindObjectsSortMode.None).Where(x => x != this).ToArray();

            for (int i = 0; i < otherPlayers.Length; i++)
            {
                string name = otherPlayers[i].NameNetworkVariable.Value.ToString();
                if (string.IsNullOrEmpty(name))
                {
                    await Task.Delay(1000);

                    if (string.IsNullOrEmpty(name))
                    {
                        otherPlayers[i].SetName("Connection Error or smth");
                        continue;
                    }
                }

                otherPlayers[i].SetName(name);
            }
        }
    }

    [ServerRpc]
    private void SetNameServerRpc(string playerName)
    {
        NameNetworkVariable.Value = playerName;

        DisplayNameClientRpc(playerName);
    }

    [ClientRpc]
    private void DisplayNameClientRpc(string playerName)
    {
        SetName(playerName); 
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }
}
