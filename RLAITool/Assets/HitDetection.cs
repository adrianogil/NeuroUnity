using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitDetection : MonoBehaviour
{
    public Text timeCounter;
    private float timer = 0;

    public GameOverEvent OnGameOver;

    Vector3 lastPosition;

    int samePositionCounter = 0;

    void Start()
    {
        lastPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Time.timeScale = 0;
            Debug.Log("Wall hit");

            OnGameOver.Invoke(timer);
        }
    }

    private void Update()
    {
        float currentDiff = (transform.position - lastPosition).magnitude;

        timer += currentDiff;
        timeCounter.text = timer.ToString();

        if (currentDiff > 0.01f)
        {
            lastPosition = transform.position;
            samePositionCounter = 0;
        } else {
            samePositionCounter++;

            if (samePositionCounter > 20)
            {
                OnGameOver.Invoke(timer);
            }
        }
        
    }
}
