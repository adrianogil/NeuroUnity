using UnityEngine;
using AForge.Neuro;

public class NeuroevolutionAgent : IReinforcementLearningAgent
{
    private IPerception m_Perception;
    private IAgentOutput m_AgentOutput;
    private int[] m_NeuralArchitecture;


    public double[] NeuralWeights;

    public double[] NeuralInputs;

    public double[] NeuralOutputs;

    public int currentChromosomeId = 0;
    public int currentPopulationId = 0;
    public double lastFitness = 0;
    public double bestFitness = 0;

    private ActivationNetwork m_ActivationNetwork;
    private Population m_GeneticPopulation;
    private Chromosome m_CurrentChromosome = null;
    private System.Random m_RandObj;

    private double mRefoircementScore = 0;
    private bool alreadyDetectStumbling = false;

    private double sigmoidAlphaValue = 2.0;

    private string mName;

    public NeuroevolutionAgent(int[] neuralArch)
    {
        m_NeuralArchitecture = neuralArch;
    }

    public void SetAgentName(string name)
    {
        mName = name;
        Population.SetAgentName(name);
    }

    public void SetupPerception(IPerception perception)
    {
        m_Perception = perception;
    }

    public void SetupOutput(IAgentOutput agentOutput)
    {
        m_AgentOutput = agentOutput;
    }

    public void InitializeLearningMethod()
    {
        m_RandObj = new System.Random();

        /** Setup Neural Network **/
        NeuralInputs = new double[m_Perception.GetFeatureSize()];

        m_ActivationNetwork = new ActivationNetwork(
                (IActivationFunction) new BipolarSigmoidFunction( sigmoidAlphaValue ),
                NeuralInputs.Length, // Input size
                m_NeuralArchitecture // Neural Architecture
                );

        /** Setup GA **/
        m_GeneticPopulation = Population.Restore(m_RandObj);

        if (m_GeneticPopulation == null)
        {
            m_GeneticPopulation = new Population(30, m_ActivationNetwork.GetWeights().Length, m_RandObj);
        }
        else
        {
            // Debug.Log("Load Population");
        }

        NeuralWeights = m_ActivationNetwork.GetWeights();

        m_CurrentChromosome = m_GeneticPopulation.CurrentChromosome;
        currentChromosomeId = m_GeneticPopulation.CurrentChromosomeID;
        currentPopulationId = m_GeneticPopulation.CurrentPopulation;
        bestFitness = m_GeneticPopulation.BestFitness;

        m_ActivationNetwork.SetWeights(m_GeneticPopulation.CurrentChromosome.Weights);

        NeuralWeights = m_ActivationNetwork.GetWeights();

        if (!m_GeneticPopulation.IsLastChromosome())
        {
            m_GeneticPopulation.NextChromosome();
            m_GeneticPopulation.Save();
            m_GeneticPopulation.LastChromosome();
        }
        else
        {
            m_GeneticPopulation.NewGeneration(m_RandObj);
            m_GeneticPopulation.Save();
            // Workaround to allow save the fitness value at the end of the simulation
            m_GeneticPopulation.mCurrentPopulation--;
        }
    }

    public void UpdateAgentReasoning ()
    {
        if (m_Perception == null || m_AgentOutput == null)
        {
            return;
        }

        double[] perception = m_Perception.GetFeatureVector();

        for (int i = 0; i < perception.Length; i++)
        {
            NeuralInputs[i] = perception[i];
        }

        NeuralOutputs = m_ActivationNetwork.Compute(NeuralInputs);

        m_AgentOutput.SetOutputValues(NeuralOutputs);
    }

    public void OnSimulationOver(float reinforcementScore)
    {
        m_CurrentChromosome.Fitness = lastFitness;

        m_GeneticPopulation.SaveFitnessValue(currentChromosomeId, m_CurrentChromosome, reinforcementScore, m_RandObj);
    }
}