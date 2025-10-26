using UnityEngine;

public class Monster : MonoBehaviour
{
    public int lvl = 1;

    // health bar
    public HealthBar healthBar;
    public int maxHealth;
    public int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = lvl * 2;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        Debug.Log(gameObject.name + " has been destroyed");
        Destroy(gameObject);
    }
}
