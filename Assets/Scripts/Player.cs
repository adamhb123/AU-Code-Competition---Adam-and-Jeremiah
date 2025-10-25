using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    // Input
    InputAction moveAction;

    // physics
    public Rigidbody2D myRigidBody;
    public float speed = 12;

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
    }

    // Update is called once per frame
    void Update()
    {
        moveValue = moveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        myRigidBody.linearVelocity = moveValue * speed;
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
