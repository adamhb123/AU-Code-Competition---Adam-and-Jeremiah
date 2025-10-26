using UnityEngine;

public class Monster : MonoBehaviour
{
    public GameObject player;

    public int level;

    // tracking and movement
    public int trackDistance = 10;
    public int movementSpeed = 2;
    public Rigidbody2D myRigidBody;

    // health bar
    public HealthBar healthBar;
    private int maxHealth;
    private int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = level * 2;
        currentHealth = maxHealth;
        //healthBar.SetHealth(currentHealth);

        // Get Rigidbody2D component if not assigned
        if (myRigidBody == null)
        {
            myRigidBody = GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate movement direction in Update
    }

    void FixedUpdate()
    {
        if (player != null && myRigidBody != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= trackDistance)
            {
                // Calculate direction to player
                Vector2 direction = (player.transform.position - transform.position).normalized;

                // Move using Rigidbody2D velocity
                myRigidBody.linearVelocity = direction * movementSpeed;
            }
            else
            {
                // Stop moving if player is out of range
                myRigidBody.linearVelocity = Vector2.zero;
            }
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }
}
