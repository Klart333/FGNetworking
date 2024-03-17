using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StandardMine : NetworkBehaviour
{
    [SerializeField] GameObject minePrefab;

    public bool HitMine()
    {
        NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();

        if (!networkObject.IsSpawned)
        {
            return false;
        }

        DoTheStuffServerRpc();

        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DoTheStuffServerRpc()
    {
        int xPosition = Random.Range(-4, 4);
        int yPosition = Random.Range(-2, 2);

        GameObject newMine = Instantiate(minePrefab, new Vector3(xPosition, yPosition, 0), Quaternion.identity);
        NetworkObject no = newMine.GetComponent<NetworkObject>();
        no.Spawn();

        NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
        networkObject.Despawn();
    }

}
