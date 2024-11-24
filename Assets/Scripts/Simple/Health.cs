using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int totalHealth;
    [SerializeField] int currentHealth;

    [SerializeField] Material damageEffect;
    [SerializeField] GameObject deathEffect;

    private void Start()
    {
        currentHealth = totalHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(DamageEffect());
        CheckDeath();
    }
    IEnumerator DamageEffect()
    {
        List<Material> materials = new List<Material>();
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.GetMaterials(materials);
        materials.Add(damageEffect);
        meshRenderer.SetMaterials(materials);
        yield return new WaitForSeconds(0.1f);
        Material m = materials[0];
        materials.Clear();
        materials.Add(m);
        meshRenderer.SetMaterials(materials);

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

        StopAllCoroutines();
        Destroy(gameObject);
    }
}
