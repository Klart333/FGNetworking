using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Missile : NetworkBehaviour
{
    [SerializeField]
    private float speed = 10;

    [SerializeField]
    private float rotateSpeed = 10;

    [SerializeField]
    private float lifetime = 5;

    [SerializeField]
    private float stunTime = 1f;

    private Rigidbody2D rb;

    private List<PlayerController> players = new List<PlayerController>();

    private void Start()
    {
        Invoke(nameof(BlowMissile), lifetime);

        players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None).Where(x => x.OwnerClientId != OwnerClientId).ToList();
    }

    void BlowMissile()
    {
        if (!IsOwner)
        {
            return;
        }

        KillMissileServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void KillMissileServerRpc()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (players.Count == 0)
        {
            Debug.LogError("Please play against more players");
            return;
        }

        int index = 0;
        float best = -1;
        if (players.Count > 1)
        {
            for (int i = 0; i < players.Count; i++)
            {
                Vector3 dir = (players[i].transform.position - transform.position).normalized;
                float dot = Vector3.Dot(dir, transform.forward);

                if (dot > best)
                {
                    best = dot;
                    index = i; // I don't know why I made it work with more than 2 players, but hey it does!
                }
            }
        }
        
        Vector3 targetDirection = (players[index].transform.position - transform.position).normalized;
        Vector2 currentDirection = Vector2.Lerp(transform.up, targetDirection, Time.deltaTime * rotateSpeed);
        transform.up = currentDirection;
        transform.position += transform.up * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOwner)
        {
            return;
        }

        if (other.gameObject.TryGetComponent(out PlayerController player) && player.OwnerClientId != OwnerClientId)
        {
            player.StunServerRpc(stunTime);
            BlowMissile();
        }

    }

}