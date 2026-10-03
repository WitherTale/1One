using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState currentState = GameState.Menu;
    public bool isGameOver = false;

    public float baseSpeed = 6f;
    public float maxSpeed = 14f;
    public float speedIncreaseRate = 0.2f;
    public float currentSpeed;

    public GameObject menuPanel;
    public GameObject hudPanel;
    public GameObject gameOverPanel;

    public string[] mapNames = new string[] { "OVERWORLD", "NETHER", "DEEP DARK", "THE END" };
    public TextMeshProUGUI gameOverTitleText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI bestScoreText;

    public ScoreManager scoreManager;
    public TilemapManager tilemapManager;
    public SpawnManager spawnManager;
    public GameObject player;

    private Vector3 playerInitialPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (player != null)
        {
            playerInitialPosition = player.transform.position;
        }

        ShowMenu();
    }

    private void Update()
    {
        if (currentState == GameState.Playing)
        {
            UpdateGameSpeed();
        }
    }

    private void UpdateGameSpeed()
    {
        float score = (scoreManager != null) ? scoreManager.CurrentScore : 0f;
        currentSpeed = Mathf.Min(baseSpeed + (score / 100f) * speedIncreaseRate, maxSpeed);

        if (tilemapManager != null)
        {
            tilemapManager.SetSpeed(currentSpeed);
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBGMForMap(tilemapManager.CurrentMapIndex);
            }
        }
    }

    public void ShowMenu()
    {
        Time.timeScale = 1f;
        currentState = GameState.Menu;
        isGameOver = false;
        currentSpeed = 0f;

        if (menuPanel) menuPanel.SetActive(true);
        if (hudPanel) hudPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (player != null) player.SetActive(false);

        if (tilemapManager != null)
        {
            tilemapManager.gameObject.SetActive(true);
            tilemapManager.InitMap();
            tilemapManager.SetSpeed(0f);
        }

        if (spawnManager != null) spawnManager.StopSpawning();
        if (scoreManager != null) scoreManager.StopScoring();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
        }

        ClearAllObstacles();
        ResetPlayer();
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        currentState = GameState.Playing;
        isGameOver = false;
        currentSpeed = baseSpeed;

        if (menuPanel) menuPanel.SetActive(false);
        if (hudPanel) hudPanel.SetActive(true);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        ResetPlayer();
        ClearAllObstacles();

        if (player != null) player.SetActive(true);
        if (tilemapManager != null)
        {
            tilemapManager.gameObject.SetActive(true);
            tilemapManager.InitMap();
            tilemapManager.SetSpeed(baseSpeed);
        }

        if (scoreManager != null) scoreManager.StartScoring();
        if (spawnManager != null) spawnManager.StartSpawning();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClick();
            AudioManager.Instance.PlayBGMForMap(0);
        }
    }
    //
    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        currentState = GameState.GameOver;

    
        currentSpeed = 0f;
        if (tilemapManager != null) tilemapManager.SetSpeed(0f);
        if (spawnManager != null) spawnManager.StopSpawning();
        if (scoreManager != null) scoreManager.StopScoring();


        if (scoreManager != null)
        {
            int finalScore = scoreManager.CurrentScore;
            int bestScore = PlayerPrefs.GetInt("HighScore", 0);

            if (finalScore > bestScore)
            {
                PlayerPrefs.SetInt("HighScore", finalScore);
                PlayerPrefs.Save();
            }

            if (finalScoreText) finalScoreText.text = "SCORE: " + finalScore.ToString("D8");
            if (bestScoreText) bestScoreText.text = "BEST:  " + bestScore.ToString("D8");
        }

        UpdateGameOverLogo();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
        }

        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
  
        yield return new WaitForSeconds(0.5f);

     
        if (hudPanel) hudPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(true);

        ResetPlayer();
    }

    private void UpdateGameOverLogo()
    {
        if (gameOverTitleText != null && tilemapManager != null && mapNames != null && mapNames.Length > 0)
        {
            int currentMapIndex = tilemapManager.CurrentMapIndex;
            if (currentMapIndex >= 0 && currentMapIndex < mapNames.Length)
            {
                
                gameOverTitleText.text = "GAME OVER\n<size=70%>" + mapNames[currentMapIndex] + "</size>";
            }
            else
            {
                gameOverTitleText.text = "GAME OVER";
            }
        }
    }


    public void RestartGame()
    {
        Time.timeScale = 1f;
        StartGame(); 
    }

    public void ReturnMenu()
    {
        Time.timeScale = 1f;
        ShowMenu(); 
    }
   
    public void Quit()
    { 
        Application.Quit(); 
    }

    private void ResetPlayer()
    {
        if (player != null)
        {
            player.transform.position = playerInitialPosition;
            player.transform.rotation = Quaternion.identity;

            
            PlayerControl pc = player.GetComponent<PlayerControl>();
            if (pc != null)
            {
                pc.ResetPlayerState();
            }
        }
    }

    public void ClearAllObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obs in obstacles)
        {
            if (obs != null) Destroy(obs);
        }

        GameObject[] falls = GameObject.FindGameObjectsWithTag("Fall");
        foreach (GameObject fall in falls)
        {
            if (fall != null) Destroy(fall);
        }
    }
}