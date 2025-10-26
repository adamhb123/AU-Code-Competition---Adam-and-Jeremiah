using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    // Player reference
    public GameObject player;

    // Monster prefab
    public GameObject monsterPrefab;

    // Potion prefab
    public GameObject potionPrefab;

    // Trail visibility
    public bool showTrail = false;

    // Tile settings
    private float tileSize;

    // Trail overlay system
    private List<GameObject> trailOverlays = new List<GameObject>();
    private bool previousShowTrail;
    private List<Vector2Int> pathToExit = new List<Vector2Int>();

    // Tile types: 0 = empty (black), 1 = ground (walkable), 2 = wall, 3 = player start, 4 = monster spawn, 6 = exit (red)
    private int[,] map = new int[,]
    {
        {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
        {2,3,1,1,1,2,1,1,1,1,1,2,1,1,1,2,1,1,1,1,1,1,1,2,1,1,1,1,1,2},
        {2,2,2,2,1,2,1,2,2,2,1,2,1,2,1,2,1,2,2,2,2,2,1,2,1,2,2,2,1,2},
        {2,1,1,1,1,1,1,2,4,2,1,1,1,2,1,1,1,2,1,1,1,1,1,2,1,1,1,2,1,2},
        {2,1,2,2,2,2,2,2,1,2,2,2,2,2,1,2,2,2,1,2,1,2,2,2,2,2,1,2,1,2},
        {2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,2,1,2,1,2},
        {2,2,2,2,1,2,2,2,2,2,2,2,1,2,2,2,2,2,2,2,1,2,2,2,1,2,1,2,1,2},
        {2,1,1,2,1,2,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,2,4,2,1,1,1,2,1,2},
        {2,1,1,2,1,2,1,2,2,2,2,2,2,2,1,2,2,2,2,2,2,2,1,2,2,2,2,2,1,2},
        {2,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2},
        {2,1,2,2,2,2,2,2,2,2,1,2,2,2,2,2,2,2,2,2,1,2,2,2,2,2,2,2,2,2},
        {2,1,2,1,1,1,1,1,1,1,1,2,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,4,1,2},
        {2,1,2,1,2,2,2,2,2,2,2,2,1,2,1,2,1,2,2,2,2,2,2,2,2,2,2,2,1,2},
        {2,1,1,1,2,1,1,1,1,1,1,1,1,2,1,1,1,2,1,1,1,1,1,2,1,1,1,1,1,2},
        {2,2,2,2,2,1,2,2,2,2,2,2,2,2,2,2,2,2,1,2,1,2,1,2,1,2,2,2,2,2},
        {2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,2,1,1,1,1,1,1,1,2},
        {2,1,2,2,2,2,2,2,2,2,2,2,1,2,2,2,2,2,2,2,1,2,2,2,2,2,1,2,1,2},
        {2,1,2,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,2,1,2,1,2},
        {2,1,2,1,2,2,2,2,2,2,2,2,2,2,1,2,2,2,2,2,2,2,2,2,1,2,1,2,1,2},
        {2,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1,2},
        {2,2,2,2,2,1,2,2,2,2,2,2,2,2,2,2,2,2,1,2,2,2,2,2,2,2,2,2,2,2},
        {2,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,2},
        {2,1,2,2,2,2,2,2,2,2,2,2,1,2,1,2,2,2,2,2,2,2,2,2,2,2,2,2,1,2},
        {2,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1,2,1,1,1,1,1,1,1,1,1,2,1,2},
        {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1,2,1,2,2,2,2,2,2,2,1,2,6,2},
        {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2}
    };

    void Start()
    {
        // Calculate tile size based on player size (1.5x player size)
        if (player != null)
        {
            SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
            if (playerSprite != null)
            {
                // Get player bounds and use the larger dimension
                float playerSize = Mathf.Max(playerSprite.bounds.size.x, playerSprite.bounds.size.y);
                tileSize = playerSize * 1.5f;
            }
            else
            {
                // Fallback: check for collider
                Collider2D playerCollider = player.GetComponent<Collider2D>();
                if (playerCollider != null)
                {
                    float playerSize = Mathf.Max(playerCollider.bounds.size.x, playerCollider.bounds.size.y);
                    tileSize = playerSize * 1.5f;
                }
                else
                {
                    tileSize = 1.5f; // Default fallback
                }
            }
        }
        else
        {
            tileSize = 1f; // Default if no player assigned
        }

        GenerateMap();
        previousShowTrail = showTrail;

        // Initialize trail visibility
        UpdateTrailVisibility();
    }

    void Update()
    {
        // Check if showTrail has changed during gameplay
        if (showTrail != previousShowTrail)
        {
            UpdateTrailVisibility();
            previousShowTrail = showTrail;
        }
    }

    void UpdateTrailVisibility()
    {
        foreach (GameObject overlay in trailOverlays)
        {
            if (overlay != null)
            {
                overlay.SetActive(showTrail);
            }
        }
    }

    // Activate trail for a specific duration
    private Coroutine trailTimerCoroutine;

    public void ActivateTrailForDuration(int seconds)
    {
        // Stop any existing trail timer
        if (trailTimerCoroutine != null)
        {
            StopCoroutine(trailTimerCoroutine);
        }

        // Start new trail timer
        trailTimerCoroutine = StartCoroutine(TrailTimer(seconds));
    }

    private System.Collections.IEnumerator TrailTimer(int seconds)
    {
        // Turn on the trail
        showTrail = true;
        Debug.Log($"Trail activated for {seconds} seconds");

        // Wait for the specified duration
        yield return new WaitForSeconds(seconds);

        // Turn off the trail
        showTrail = false;
        Debug.Log("Trail deactivated");

        trailTimerCoroutine = null;
    }

    void GenerateMap()
    {
        int height = map.GetLength(0);
        int width = map.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int tileType = map[y, x];

                // Calculate position (centered map)
                Vector3 position = new Vector3(x * tileSize, -y * tileSize, 0);

                // Create tile GameObject
                GameObject tile = new GameObject($"Tile_{x}_{y}");
                tile.transform.position = position;
                tile.transform.localScale = new Vector3(tileSize, tileSize, 1);
                tile.transform.parent = this.transform;

                // Add SpriteRenderer
                SpriteRenderer spriteRenderer = tile.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = CreateSquareSprite();
                spriteRenderer.sortingOrder = -10; // Render tiles behind everything

                // Set color and collider based on tile type
                switch (tileType)
                {
                    case 0: // Empty (black)
                        spriteRenderer.color = Color.black;
                        break;
                    case 1: // Ground (walkable) - gray
                        spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f);
                        break;
                    case 2: // Wall (with collider) - brown
                        spriteRenderer.color = new Color(0.4f, 0.2f, 0.1f);
                        BoxCollider2D collider = tile.AddComponent<BoxCollider2D>();
                        collider.size = new Vector2(1, 1);
                        tile.tag = "Wall";
                        break;
                    case 3: // Player start - walkable ground
                        spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f);
                        // Position player here
                        if (player != null)
                        {
                            player.transform.position = position;

                            // Set sorting order for player sprite
                            SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
                            if (playerSprite != null)
                            {
                                playerSprite.sortingOrder = 10; // Render player above everything else
                            }
                        }
                        break;
                    case 4: // Monster spawn - walkable ground with monster
                        spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f);
                        // Spawn monster here
                        if (monsterPrefab != null)
                        {
                            GameObject monster = Instantiate(monsterPrefab, position, Quaternion.identity);
                            monster.transform.parent = this.transform;

                            // Set sorting order for monster sprite
                            SpriteRenderer monsterSprite = monster.GetComponent<SpriteRenderer>();
                            if (monsterSprite != null)
                            {
                                monsterSprite.sortingOrder = 5; // Render above tiles
                            }

                            // Set the player reference on the monster if it has the Monster script
                            Monster monsterScript = monster.GetComponent<Monster>();
                            if (monsterScript != null && player != null)
                            {
                                monsterScript.player = player;
                            }
                        }
                        break;
                    case 6: // Exit - red tile (goal)
                        spriteRenderer.color = Color.red;
                        break;
                }
            }
        }

        // Calculate the optimal path from start to exit
        CalculatePath();

        // Create overlay tiles for the trail
        CreateTrailOverlays();

        // Scatter potions throughout the map
        ScatterPotions();
    }

    // Create a simple square sprite programmatically
    Sprite CreateSquareSprite()
    {
        int pixelsPerUnit = 100;
        int size = pixelsPerUnit;

        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        // Fill with white (color will be tinted by SpriteRenderer)
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }

    // Use BFS to find the shortest path from player start to exit
    void CalculatePath()
    {
        pathToExit.Clear();

        int height = map.GetLength(0);
        int width = map.GetLength(1);

        // Find start (tile type 3) and exit (tile type 6) positions
        Vector2Int start = Vector2Int.zero;
        Vector2Int exit = Vector2Int.zero;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[y, x] == 3)
                {
                    start = new Vector2Int(x, y);
                }
                else if (map[y, x] == 6)
                {
                    exit = new Vector2Int(x, y);
                }
            }
        }

        // BFS pathfinding
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, -1), // up
            new Vector2Int(0, 1),  // down
            new Vector2Int(-1, 0), // left
            new Vector2Int(1, 0)   // right
        };

        bool foundPath = false;

        while (queue.Count > 0 && !foundPath)
        {
            Vector2Int current = queue.Dequeue();

            if (current == exit)
            {
                foundPath = true;
                break;
            }

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                // Check bounds
                if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height)
                    continue;

                // Check if already visited
                if (visited.Contains(next))
                    continue;

                int tileType = map[next.y, next.x];

                // Check if walkable (not wall or empty)
                if (tileType == 2 || tileType == 0)
                    continue;

                visited.Add(next);
                queue.Enqueue(next);
                cameFrom[next] = current;
            }
        }

        // Reconstruct path
        if (foundPath)
        {
            Vector2Int current = exit;
            while (current != start)
            {
                pathToExit.Add(current);
                current = cameFrom[current];
            }
            pathToExit.Add(start);
            pathToExit.Reverse();
        }
    }

    void CreateTrailOverlays()
    {
        // Clear any existing overlays
        foreach (GameObject overlay in trailOverlays)
        {
            if (overlay != null)
            {
                Destroy(overlay);
            }
        }
        trailOverlays.Clear();

        Debug.Log($"Creating trail overlays for {pathToExit.Count} positions");

        // Create overlay for each position in the path
        foreach (Vector2Int pos in pathToExit)
        {
            Vector3 position = new Vector3(pos.x * tileSize, -pos.y * tileSize, 0); // Same z as tiles

            GameObject overlay = new GameObject($"TrailOverlay_{pos.x}_{pos.y}");
            overlay.transform.position = position;
            overlay.transform.localScale = new Vector3(tileSize * 0.7f, tileSize * 0.7f, 1); // 70% of tile size
            overlay.transform.parent = this.transform;

            SpriteRenderer spriteRenderer = overlay.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateSquareSprite();
            spriteRenderer.color = new Color(1f, 0.84f, 0f, 0.8f); // Gold with slight transparency
            spriteRenderer.sortingOrder = 10; // Higher sorting order to render above map tiles

            // Initially set active based on showTrail
            overlay.SetActive(showTrail);
            trailOverlays.Add(overlay);
        }

        Debug.Log($"Trail overlays created. showTrail = {showTrail}");
    }

    void ScatterPotions()
    {
        if (potionPrefab == null)
        {
            Debug.LogWarning("Potion prefab not assigned!");
            return;
        }

        int height = map.GetLength(0);
        int width = map.GetLength(1);

        // Collect all walkable tile positions (excluding player start and exit)
        List<Vector2Int> walkableTiles = new List<Vector2Int>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int tileType = map[y, x];
                // Include ground tiles and monster spawn tiles, but exclude player start and exit
                if (tileType == 1 || tileType == 4)
                {
                    walkableTiles.Add(new Vector2Int(x, y));
                }
            }
        }

        // Scatter potions on random walkable tiles (approximately 10-15% of walkable tiles)
        int potionCount = Mathf.Max(5, walkableTiles.Count / 10);

        // Shuffle the walkable tiles list
        for (int i = 0; i < walkableTiles.Count; i++)
        {
            Vector2Int temp = walkableTiles[i];
            int randomIndex = Random.Range(i, walkableTiles.Count);
            walkableTiles[i] = walkableTiles[randomIndex];
            walkableTiles[randomIndex] = temp;
        }

        // Place potions
        for (int i = 0; i < Mathf.Min(potionCount, walkableTiles.Count); i++)
        {
            Vector2Int pos = walkableTiles[i];
            Vector3 position = new Vector3(pos.x * tileSize, -pos.y * tileSize, 0);

            GameObject potion = Instantiate(potionPrefab, position, Quaternion.identity);
            potion.transform.parent = this.transform;

            // Set sorting order for potion sprite
            SpriteRenderer potionSprite = potion.GetComponent<SpriteRenderer>();
            if (potionSprite != null)
            {
                potionSprite.sortingOrder = 3; // Render above tiles but below monsters and player
            }
        }

        Debug.Log($"Scattered {Mathf.Min(potionCount, walkableTiles.Count)} potions across the map");
    }
}
