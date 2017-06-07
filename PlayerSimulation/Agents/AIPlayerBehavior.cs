using UnityEngine;
using System.Collections;

using AForge.Neuro;

public class AIPlayerBehavior : MonoBehaviour, IEventObserver
{

	#region Singleton
	private static AIPlayerBehavior _instance = null;
	public static AIPlayerBehavior Instance { get { return _instance; } }
	#endregion

    private GameObject mPlayer;

    #region Public variables to take a glance at current results
    [Header("Neural Parameters")]
    public double[] NeuralWeights;

	public double[] NeuralInputs;

	public double[] NeuralOutputs;

    [Header("Neural response")]
    public bool CurrentJumpInput = false;
    public bool LastJumpInput = false;

    [Header("GA Parameters")]
    public int currentChromosomeId = 0;
    public int currentPopulationId = 0;
    public double lastFitness = 0;
    public double bestFitness = 0;
    #endregion

    private ActivationNetwork mActivationNetwork;
    private Population mGeneticPopulation;
    private Chromosome _currentChromosome = null;
    private System.Random mRandObj;

    IPerception mPerception = null;

    private double mRefoircementScore = 0;
    private bool alreadyDetectStumbling = false;

    private double sigmoidAlphaValue = 2.0;

    public static class PrefsKey
    {
        public const string CURRENT_CHROMOSOME = "CURRENT_CHROMOSOME";
    }

	public bool GetJumpInput()
	{
		return !LastJumpInput && CurrentJumpInput;
	}

	public bool GetLongJumpInput()
	{
		return CurrentJumpInput;
	}

    void Awake()
    {
		_instance = this;
    }

    void Start()
    {
        Invoke("ForceGameToStart", 3f);
        Invoke("SetupSimulation", 4f);
    }

    void ForceGameToStart()
    {
        Debug.Log("ForceGameToStart");

        HomeGUIManager.Instance.StartGame();
    }

	void SetupSimulation()
	{
        Debug.Log("SetupSimulation");

        mPlayer = GameManager.Instance.GetPlayer();

        GameManager.Instance.OnGameOver += OnOneSimulationOver;

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

		mPerception = new Raycast2DPerception(mPlayer.transform, 30, 10, tagRecognition);

        mRandObj = new System.Random();

        /** Setup Neural Network **/
		NeuralInputs = new double[mPerception.GetFeatureSize()+1];

        mActivationNetwork = new ActivationNetwork(
                (IActivationFunction) new BipolarSigmoidFunction( sigmoidAlphaValue ),
                NeuralInputs.Length, // Input size
                8, 1, 1 // Neural Architecture
                );

        /** Setup GA **/
        mGeneticPopulation = Population.Restore(mRandObj);

        if (mGeneticPopulation == null)
        {
            mGeneticPopulation = new Population(30, mActivationNetwork.GetWeights().Length, mRandObj);
        }
        else
        {
            Debug.Log("Load Population");
        }

        NeuralWeights = mActivationNetwork.GetWeights();

        _currentChromosome = mGeneticPopulation.CurrentChromosome;
        currentChromosomeId = mGeneticPopulation.CurrentChromosomeID;
        currentPopulationId = mGeneticPopulation.CurrentPopulation;
        bestFitness = mGeneticPopulation.BestFitness;

        mActivationNetwork.SetWeights(mGeneticPopulation.CurrentChromosome.Weights);

        NeuralWeights = mActivationNetwork.GetWeights();

        // Add itself as an event Observer
        EventTracker.Instance.AddObserver(this);

        if (!mGeneticPopulation.IsLastChromosome())
        {
            mGeneticPopulation.NextChromosome();
            mGeneticPopulation.Save();
            mGeneticPopulation.LastChromosome();
        }
        else
        {
            mGeneticPopulation.NewGeneration(mRandObj);
            mGeneticPopulation.Save();
            // Workaround to allow save the fitness value at the end of the simulation
            mGeneticPopulation.mCurrentPopulation--;
        }
	}

    private void ResetScore()
    {
        ScoreManager.Instance.totalScore = 0;
        ScoreManager.Instance.totalBonusScore = 0;
        ScoreManager.Instance.totalCoinsCollected = 0;
        ScoreManager.Instance.totalDistanceTravelled = 0;
        ScoreManager.Instance.totalBadTouchdowns = 0;
    }

	public void OnOneSimulationOver()
	{
//		lastFitness = 
//            0.65f*ScoreManager.Instance.totalScore + 
//            0.2f * ScoreManager.Instance.totalCoinsCollected +
//            0.15f * ScoreManager.Instance.totalBonusScore +
//            (-100f) * ScoreManager.Instance.totalBadTouchdowns +
//            mRefoircementScore;

		mPlayer = GameManager.Instance.GetPlayer();

		lastFitness = mPlayer.transform.position.x / 200f;
        //lastFitness += ScoreManager.Instance.totalCoinsCollected / 400f;
		//lastFitness += ScoreManager.Instance.totalScore / 8000000f;

		mRefoircementScore = 0;

        //lastFitness = lastFitness / ScoreManager.Instance.totalDistanceTravelled;

        _currentChromosome.Fitness = lastFitness;

        mGeneticPopulation.SaveFitnessValue(currentChromosomeId, _currentChromosome, lastFitness, mRandObj);

		DestroyLevel();
	}

	public void OnBadTouchdown()
	{
        mRefoircementScore -= 100f;

		OnOneSimulationOver();
	}

	void DestroyLevel()
	{
        Destroy(GameObject.Find("~LeanTween"));
        LeanTween.reset();
        Application.LoadLevel("LoadingScreen");
	}

	// Update is called once per frame
	void Update ()
	{
        if (mPerception == null)
        {
            return;
        }

		double[] perception = mPerception.GetFeatureVector();

		for (int i = 0; i < perception.Length; i++)
		{
			NeuralInputs[i] = perception[i];
		}

		NeuralInputs[perception.Length] = CurrentJumpInput? 1 : -1;

		NeuralOutputs = mActivationNetwork.Compute(NeuralInputs);

		LastJumpInput = CurrentJumpInput;
		CurrentJumpInput = false;

		if (NeuralOutputs[0] >= 0.5)
		{
			CurrentJumpInput = true;
		}
	}

    public void ReceiveEvent(string eventName, float eventValue)
    {
        if (!alreadyDetectStumbling && eventName == Constants.Events.StumblingMovement)
        {
            alreadyDetectStumbling = true;

            OnBadTouchdown();
        }
    }
}

