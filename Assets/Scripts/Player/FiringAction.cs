using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FiringAction : NetworkBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] SingleBullet bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;


    public override void OnNetworkSpawn()
    {
        playerController.onFireEvent += Fire;
    }

    private void Fire(bool isShooting)
    {

        if (isShooting)
        {
            ShootLocalBullet();
        }
    }

    [ServerRpc]
    private void ShootBulletServerRpc(ServerRpcParams clientParams)
    {
        var bullet = NetworkManager.SpawnManager.InstantiateAndSpawn(bulletPrefab.GetComponent<NetworkObject>(), clientParams.Receive.SenderClientId, position: bulletSpawnPoint.position, rotation: bulletSpawnPoint.rotation);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());

        //ShootBulletClientRpc();
    }

    /*[ClientRpc]
    private void ShootBulletClientRpc()
    {
        if (IsOwner) return;
        GameObject bullet = Instantiate(clientSingleBulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());

    }*/

    private void ShootLocalBullet()
    {
        //GameObject bullet = Instantiate(clientSingleBulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        //Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());

        ServerRpcParams clientParams = new ServerRpcParams();
        clientParams.Receive = new ServerRpcReceiveParams { SenderClientId = OwnerClientId };
        ShootBulletServerRpc(clientParams);
    }
}
