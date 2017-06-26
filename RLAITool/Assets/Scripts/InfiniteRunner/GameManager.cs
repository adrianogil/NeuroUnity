using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameOverEvent : UnityEvent <float> {}

public class GameManager : MonoBehaviour
{
    public float levelVelocity;
    public Transform cameraTransform;

    public GameOverEvent OnGameOver;

    private Transform player;

    private float lastPos;

    private bool isPlaying = false;

    void Awake()
    {
        player = (GameObject.FindGameObjectWithTag("Player") as GameObject).transform;
        // cameraTransform = Camera.main.transform;
    }


    void Start()
    {
        lastPos = player.position.x;

        StartGame();
    }

    public void StartGame()
    {
        isPlaying = true;
    }

    void Update()
    {
        if (!isPlaying) return;

        float currentPos = player.position.x - lastPos;
        ScoreManager.AddScore(Mathf.FloorToInt(currentPos));
        lastPos = player.position.x - (currentPos - Mathf.FloorToInt(currentPos));

        Vector3 currentPosition = cameraTransform.position;
        currentPosition.x += levelVelocity * Time.deltaTime;
        cameraTransform.position = currentPosition;

        Vector3 playerVPosition = Camera.main.WorldToViewportPoint(player.position);

        if (playerVPosition.x < 0f)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isPlaying = false;
        OnGameOver.Invoke((float)ScoreManager.Instance.score);
        Debug.Log("GilLog - GameManager::GameOver");
    }
}