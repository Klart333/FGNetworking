using System;
using Unity.Netcode;
using UnityEngine;

public class SingleBullet : NetworkBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] int bulletSpeed = 200;

    [SerializeField] float lifeSpan = 2;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.up * bulletSpeed;
        Invoke("KillBullet", lifeSpan);

    }

    void KillBullet()
    {
        KillBulletServerRpc();    
    }

    [ServerRpc(RequireOwnership = false)]
    private void KillBulletServerRpc()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOwner)
        {
            return;
        }

        if (other.gameObject.TryGetComponent(out StandardMine mine) && mine.HitMine())
        {
            IncreasePlayerScore();

            KillBullet();
        }

    }

    private void IncreasePlayerScore()
    {
        var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None); // yes yes very fast

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].OwnerClientId == this.OwnerClientId)
            {
                players[i].IncreaseScore();
            }
        }
    }
}
