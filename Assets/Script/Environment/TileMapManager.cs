using UnityEngine;

public class TilemapManager : MonoBehaviour
{
    public int CurrentMapIndex => currentMapIndex;

    [Header("--- CẤU HÌNH MAP ---")]
    [Tooltip("KÉO FILE PREFAB MÀU XANH 🔷 TỪ THƯ MỤC ASSETS VÀO ĐÂY (KHÔNG KÉO TỪ SCENE)")]
    [SerializeField] private GameObject[] chunkPrefabs;
    [SerializeField] private float chunkWidth = 20f;
    [SerializeField] private int scoreToNextMap = 1500;
    [SerializeField] private ScoreManager scoreManager;

    private float speed = 0f;
    private GameObject currentChunk;
    private GameObject nextChunk;
    private int currentMapIndex = 0;

    private void Awake()
    {
        
    }
    public void InitMap()
    {
       
        if (chunkPrefabs == null || chunkPrefabs.Length == 0 || chunkPrefabs[0] == null)
        {
            Debug.LogError("[TilemapManager] LỖI: Chưa kéo File Prefab MÀU XANH 🔷 từ thư mục Assets vào mảng chunkPrefabs!");
            return;
        }

        if (chunkWidth <= 0) chunkWidth = 20f;

    
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        currentMapIndex = 0;

        float startX = -chunkWidth / 2f;
        currentChunk = Instantiate(chunkPrefabs[0], new Vector3(startX, 0f, 0f), Quaternion.identity, transform);
        nextChunk = Instantiate(chunkPrefabs[0], new Vector3(startX + chunkWidth, 0f, 0f), Quaternion.identity, transform);
        nextChunk.SetActive(true);
    }

    private void Update()
    {
        if (currentChunk == null || nextChunk == null) return;

        float step = speed * Time.deltaTime;

        foreach (Transform child in transform)
        {
            if (child != null)
            {
                child.position += Vector3.left * step;
            }
        }

        if (scoreManager != null)
        {
            int score = scoreManager.CurrentScore;
            int rawIndex = score / scoreToNextMap;

            if (chunkPrefabs.Length > 1)
            {
                if (rawIndex < chunkPrefabs.Length)
                {
                    currentMapIndex = rawIndex;
                }
                else
                {
                    int loopRange = chunkPrefabs.Length - 1;
                    currentMapIndex = 1 + ((rawIndex - chunkPrefabs.Length) % loopRange);
                }
            }
            else
            {
                currentMapIndex = 0;
            }
        }


        if (currentChunk.transform.position.x <= (-chunkWidth * 1.5f))
        {
            Destroy(currentChunk);
            currentChunk = nextChunk;

            float spawnX = currentChunk.transform.position.x + chunkWidth;
            nextChunk = Instantiate(
                chunkPrefabs[currentMapIndex],
                new Vector3(spawnX, 0f, 0f),
                Quaternion.identity,
                transform
            );
        }
        nextChunk.SetActive(true);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public int GetCurrentThemeIndex()
    {
        return currentMapIndex;
    }
}