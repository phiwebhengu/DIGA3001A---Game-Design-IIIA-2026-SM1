using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public HealthUI healthUI;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);

        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy)
        {
            TakeDamage(enemy.damage);
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
        
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.green;
    }


}
