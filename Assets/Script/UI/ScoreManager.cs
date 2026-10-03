using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private int lastMilestone = 0;
    private float score = 0f;
    private int highScore = 0;
    private bool isScoring = false;

    public int CurrentScore => (int)score;

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        UpdateScoreUI();
        UpdateHighScoreUI();

       
        isScoring = false;
    }

    void Update()
    {
      
        if (!isScoring) return;
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameState.Playing) return;

        score += Time.deltaTime * 10f;

        UpdateScoreUI();

       
        int currentCheck = (int)score / 100;
        if (currentCheck > lastMilestone)
        {
            lastMilestone = currentCheck;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMilestone();
            }
        }

        if ((int)score > highScore)
        {
            highScore = (int)score;
            UpdateHighScoreUI();
        }
    }

    public void StartScoring()
    {
        score = 0f;
        lastMilestone = 0;
        isScoring = true;
    }

    public void StopScoring()
    {
        isScoring = false;

        if ((int)score >= highScore)
        {
            highScore = (int)score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            UpdateHighScoreUI();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = ((int)score).ToString("D8");
        }
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "HI " + highScore.ToString("D8");
        }
    }
}