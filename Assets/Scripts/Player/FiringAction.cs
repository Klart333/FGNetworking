using Unity.Netcode;
using UnityEngine;

public class FiringAction : NetworkBehaviour
{
    [Header("Bullet")]
    [SerializeField] PlayerController playerController;
    [SerializeField] SingleBullet bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;

    [Header("Missile")]
    [SerializeField] private Missile missilePrefab;
    [SerializeField] private float missileCooldown = 5;

    private float missileTimer = 0;

    public override void OnNetworkSpawn()
    {
        playerController.onFireEvent += Fire;
        playerController.onMissileEvent += ShootMissile;
    }

    private void Update()
    {
        if (missileTimer > 0)
        {
            missileTimer -= Time.deltaTime;
        }
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
    }

    private void ShootLocalBullet()
    {
        ServerRpcParams clientParams = new ServerRpcParams();
        clientParams.Receive = new ServerRpcReceiveParams { SenderClientId = OwnerClientId };
        ShootBulletServerRpc(clientParams);
    }

    private void ShootMissile()
    {
        if (missileTimer > 0)
        {
            return;
        }

        missileTimer = missileCooldown;
        var param = new ServerRpcParams { Receive = new ServerRpcReceiveParams { SenderClientId = OwnerClientId } };
        SpawnMissileServerRpc(param);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SpawnMissileServerRpc(ServerRpcParams senderParam)
    {
        var missile = NetworkManager.SpawnManager.InstantiateAndSpawn(missilePrefab.GetComponent<NetworkObject>(), senderParam.Receive.SenderClientId, position: bulletSpawnPoint.position, rotation: bulletSpawnPoint.rotation);
        Physics2D.IgnoreCollision(missile.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());
    }
}
