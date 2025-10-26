using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : MonoBehaviour
{
    // collisions
    private bool isColliding = false;
    private GameObject monsterObject;

    // click detection
    private PlayerActions playerActions;

    private void Awake()
    {
        playerActions = new PlayerActions();
        playerActions.Gameplay.Click.performed += OnClickPerformed;
    }

    private void OnEnable()
    {
        playerActions.Enable();
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (isColliding)
        {
            Debug.Log("mouse click detected");
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(1);
            }
        }

    }

    // collisions
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("monster"))
        {
            isColliding = true;
            monsterObject = collision.gameObject;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
    }
}
