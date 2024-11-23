using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int totalHealth;
    [SerializeField] int currentHealth;

    [SerializeField]GameObject deathEffect;

    private void Start()
    {
        currentHealth = totalHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        CheckDeath();
    }

    public void Heal(int healing)
    {
        currentHealth += healing;
        if (currentHealth > totalHealth) currentHealth = totalHealth;
    }

    void CheckDeath()
    {
        if (currentHealth <= 0)
            Death();
    }

    void Death()
    {
        GameObject o = Instantiate(deathEffect, transform.position, Quaternion.identity);
        FindFirstObjectByType<SimpleController>().score++;

        Destroy(gameObject);
    }
}
