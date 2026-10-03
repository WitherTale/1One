using UnityEngine;
using System.Collections;

[System.Serializable]
public class MapObstacleSet
{
    public string mapName;
    public GameObject[] groundObstacles;
    public GameObject[] airObstacles;
}

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("1. Tham chiếu TilemapManager để đọc Map Index")]
    [SerializeField] private TilemapManager tilemapManager;

    [Header("2. Bộ bẫy tương ứng cho 7 Map")]
    [SerializeField] private MapObstacleSet[] mapObstacleSets;

    [Header("3. Cấu hình Vị trí Spawn")]
    [SerializeField] private float spawnX = 14f;
    [SerializeField] private float groundY = -2.5f;

    [Header("4. Độ cao cộng thêm cho Bẫy Bay (so với GroundY)")]
    [SerializeField] private float[] airYHeights = { 0.6f, 1.8f, 3.2f };

    [Header("5. Tần suất sinh bẫy (giây)")]
    [SerializeField] private float minDelay = 1.8f;
    [SerializeField] private float maxDelay = 3.0f;

    private Coroutine spawnCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartSpawning();
    }

    public void StartSpawning()
    {
        StopSpawning();
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        while (true)
        {
            
            bool isPlaying = (GameManager.Instance == null) || (GameManager.Instance.currentState == GameState.Playing);

            if (isPlaying)
            {
                SpawnRandomObstacle();
            }

           
            float speedMultiplier = 1f;
            if (GameManager.Instance != null && GameManager.Instance.baseSpeed > 0)
            {
                speedMultiplier = GameManager.Instance.currentSpeed / GameManager.Instance.baseSpeed;
            }

            float delay = Random.Range(minDelay, maxDelay) / Mathf.Max(speedMultiplier, 1.0f);
            yield return new WaitForSeconds(delay);
        }
    }

    void SpawnRandomObstacle()
    {
        if (mapObstacleSets == null || mapObstacleSets.Length == 0) return;

        int currentMapIndex = 0;
        if (tilemapManager != null)
        {
            currentMapIndex = tilemapManager.CurrentMapIndex;
        }
        currentMapIndex = Mathf.Clamp(currentMapIndex, 0, mapObstacleSets.Length - 1);

        MapObstacleSet currentSet = mapObstacleSets[currentMapIndex];
        if (currentSet == null) return;

       
        bool hasGround = currentSet.groundObstacles != null && currentSet.groundObstacles.Length > 0;
        bool hasAir = currentSet.airObstacles != null && currentSet.airObstacles.Length > 0;

        if (!hasGround && !hasAir) return;

        float randomValue = Random.value;

    
        if (randomValue < 0.45f && hasGround)
        {
            SpawnObstacle(currentSet.groundObstacles, groundY);
        }
        else if (hasAir)
        {
            SpawnAirObstacle(currentSet);
        }
        else if (hasGround) 
        {
            SpawnObstacle(currentSet.groundObstacles, groundY);
        }
    }

    void SpawnObstacle(GameObject[] prefabs, float yPos)
    {
        if (prefabs == null || prefabs.Length == 0) return;
        int index = Random.Range(0, prefabs.Length);
        if (prefabs[index] == null) return;

        Vector3 spawnPos = new Vector3(spawnX, yPos, 0);
        GameObject obs = Instantiate(prefabs[index], spawnPos, Quaternion.identity);
        obs.tag = "Obstacle";
    }

    void SpawnAirObstacle(MapObstacleSet set)
    {
        if (set.airObstacles == null || set.airObstacles.Length == 0) return;
        int index = Random.Range(0, set.airObstacles.Length);
        if (set.airObstacles[index] == null) return;

        int heightIndex = Random.Range(0, airYHeights.Length);
        float airY = groundY + airYHeights[heightIndex]; 
        Vector3 spawnPos = new Vector3(spawnX, airY, 0);

        GameObject obs = Instantiate(set.airObstacles[index], spawnPos, Quaternion.identity);
        obs.tag = "Obstacle";
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(spawnX, groundY, 0), 0.4f); 

        Gizmos.color = Color.cyan;
        if (airYHeights != null)
        {
            foreach (float h in airYHeights)
            {
                Gizmos.DrawWireSphere(new Vector3(spawnX, groundY + h, 0), 0.3f); 
            }
        }
    }
}