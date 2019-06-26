using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameOverEvent : UnityEvent <float> {}

public class GameManager : MonoBehaviour
{
    public float minLevelVelocity;
    public Transform cameraTransform;

    public GameOverEvent OnGameOver;

    private Transform player;

    private float lastPos;
    private float lastPlayerPos;
    private int framesInSamePos = 0;

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
        currentPosition.x += Mathf.Max(minLevelVelocity *Time.deltaTime, player.position.x - currentPosition.x);
        cameraTransform.position = currentPosition;

        Vector3 playerVPosition = Camera.main.WorldToViewportPoint(player.position);

        if (Mathf.Abs(player.position.x - lastPlayerPos) < 0.1f)
        {
            framesInSamePos++;
        } else {
            framesInSamePos = 0;
        }

        lastPlayerPos = player.position.x;

        if (playerVPosition.x < 0f || framesInSamePos > 10)
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