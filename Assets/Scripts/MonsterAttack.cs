using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    public int damage = 15;
    public float attackCooldown = 1f;

    private float lastAttackTime = -999f;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (Time.time < lastAttackTime + attackCooldown)
        {
            return;
        }

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null)
        {
            return;
        }

        health.TakeDamage(damage);
        lastAttackTime = Time.time;
    }
}
