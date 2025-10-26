using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    // Input
    InputAction moveAction;

    // physics
    public Rigidbody2D myRigidBody;
    public int movementSpeed = 12;

    // sprite flipping
    private SpriteRenderer spriteRenderer;

    // health bar
    public HealthBar healthBar;
    public int maxHealth = 8;
    public int currentHealth;
    public int timeForDamage = 1;
    public int monsterDamage = 1;


    // collisions
    private Vector2 moveValue;
    private float collisionDuration = 0f;
    private bool isColliding = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        moveValue = moveAction.ReadValue<Vector2>();

        // Flip sprite based on horizontal movement
        if (moveValue.x > 0)
        {
            // Moving right - flip sprite
            spriteRenderer.flipX = true;
        }
        else if (moveValue.x < 0)
        {
            // Moving left - use default orientation
            spriteRenderer.flipX = false;
        }
    }

    void FixedUpdate()
    {
        myRigidBody.linearVelocity = moveValue * movementSpeed;
    }

    // health bar functions
    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }


    // collision functions
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("monster"))
        {
            isColliding = true;
            collisionDuration = 0f;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isColliding && collision.gameObject.CompareTag("monster"))
        {
            collisionDuration += Time.deltaTime;
            if (collisionDuration > timeForDamage)
            {
                TakeDamage(monsterDamage);
                collisionDuration = 0f;
                Debug.Log("reached time for damage");
            }
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
        Debug.Log("collision ended");
    }
}
