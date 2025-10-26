using UnityEngine;

public class TrailPotion : MonoBehaviour
{
    public GameObject mapGenerator;
    public int seconds = 10;

    private MapGenerator mapGenScript;

    void Start()
    {
        // Get the MapGenerator component
        if (mapGenerator != null)
        {
            mapGenScript = mapGenerator.GetComponent<MapGenerator>();
        }

        // Add a 2D trigger collider if one doesn't exist
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
        }
        collider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player picked up the potion
        if (other.CompareTag("Player") && mapGenScript != null)
        {
            // Activate trail on the MapGenerator (which won't be destroyed)
            mapGenScript.ActivateTrailForDuration(seconds);

            // Destroy the potion
            Destroy(gameObject);
        }
    }
}
