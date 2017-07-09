using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitDetection : MonoBehaviour
{
    public Text timeCounter;
    private float timer = 0;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Time.timeScale = 0;
            Debug.Log("Wall hit");
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        timeCounter.text = timer.ToString();
    }
}
