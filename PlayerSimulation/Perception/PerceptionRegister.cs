using UnityEngine;
using System.Collections;

public class PerceptionRegister : MonoBehaviour {

	int currentFrameRegister = 0;

	const string PerceptionFramePrefsKey = "PERCEPTION_FRAME_NUMBER";
	const string BaseFileName = "frame_perception_";
	const string BaseFileExtension = ".txt";

	IPerception mPerception = null;
	PlayerController playerController = null;
	bool lastJumping = false;

	void Start () 
	{
		currentFrameRegister = PlayerPrefs.GetInt(PerceptionFramePrefsKey, 0);	
		currentFrameRegister = 0;

		Invoke("SetupPerception", 4.0f);
		InvokeRepeating("RegisterFramePerception", 5.0f, 1.0f / 20f);	
	}
	
	void SetupPerception()
	{
		/** Instantiate perception **/
        CategoryObjectRecognition tagRecognition = new CategoryObjectRecognition(new CategoryObject[]
        {
            new CategoryObject()
            {
                CategoryName = "obstacle",
                Tags = new string[] { 
                    Tags.Obstacle_Island,
                    Tags.Obstacle_Ramp,
                    Tags.Obstacle_Platform,
                    Tags.Obstacle_Box_Wood,
                    Tags.Obstacle_Box_Iron,
                    Tags.Obstacle_Bomb,
                    Tags.Obstacle_TimeArc,
                    Tags.Coin_Basic
                },
                ObjectNames = null
            },

            new CategoryObject()
            {
                CategoryName = "collectable",
                Tags = new string[] { 
                    Tags.Coin_Basic
                },
                ObjectNames = null
            }
        });

        GameObject player = GameManager.Instance.GetPlayer();
        playerController = player.GetComponent<PlayerController>();
		mPerception = new Raycast2DPerception(player.transform, 30, 10, tagRecognition);
	}

	void RegisterFramePerception () 
	{
		if (mPerception == null)
		{
			return;
		}

		string filename = BaseFileName + currentFrameRegister.ToString() + BaseFileExtension;

		double[] perception = mPerception.GetFeatureVector();

		string[] lines  = new string[4+perception.Length];

		lines[0] = "2";
		lines[1] = playerController.isRolling? "1": "-1";
		lines[2] = perception.Length.ToString();

		for (int p = 0; p < perception.Length; p++)
		{
			lines[p+3] = perception[p].ToString();
		}

		lines[perception.Length+3] = lastJumping? "1" : "-1";

		System.IO.File.WriteAllLines(Application.persistentDataPath + "/" + filename, lines);

		lastJumping = playerController.isRolling;

		currentFrameRegister++;
		PlayerPrefs.SetInt(PerceptionFramePrefsKey, currentFrameRegister);
	}
}
