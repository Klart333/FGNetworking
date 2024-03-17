using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MineSpawner : NetworkBehaviour
{
    [SerializeField]
    private StandardMine mine;

    public void SpawnMine()
    {
        if (!IsOwner)
        {
            return;
        }

        NetworkManager.SpawnManager.InstantiateAndSpawn(mine.GetComponent<NetworkObject>(), 0, position: transform.position, rotation: transform.rotation);
    }
}
