using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public int score;

    public Text scoreText;

    public static ScoreManager Instance
    {
        get;
        private set;
    }

    void Awake()
    {
        Instance = this;
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    public static void AddScore(int points)
    {
        Instance.score += points;

        Instance.UpdateScoreText();
    }
}

