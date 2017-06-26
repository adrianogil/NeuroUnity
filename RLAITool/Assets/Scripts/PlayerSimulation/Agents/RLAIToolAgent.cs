using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum RLAIType
{
    NeuralNetwork,
    GeneticAlgorithms,
    Neuroevolution
}

public enum RLAILearningMode
{
    Supervised,
    Reinforcement
}

public enum RLAIUpdateMode
{
    EveryFrame,
    Custom
}

public class RLAIToolAgent : MonoBehaviour
{
    public Transform player;

    public RLAIUpdateMode updateMode = RLAIUpdateMode.EveryFrame;

    public RLAIType AIMethod;

    public RLAILearningMode AILearningMode;



    [HideInInspector] public UnityEvent RestartGameplay;
    [HideInInspector] public int[] neuralArchitecture = new int[] {8, 1};
    [HideInInspector] public bool IsLearningMode = false;

    private ILearningAgent m_LearningAgent;

    public void Awake()
    {
        AgentPerception agentPerception = GetComponent<AgentPerception>();
        agentPerception.GeneratePerception();

        AgentOutput agentOutput = GetComponent<AgentOutput>();

        IPerception perception = agentPerception.perception;

        if (AIMethod == RLAIType.Neuroevolution)
        {
            m_LearningAgent = new NeuroevolutionAgent(neuralArchitecture);
        }

        if (m_LearningAgent != null)
        {
            m_LearningAgent.SetupPerception(perception);
            m_LearningAgent.SetupOutput(agentOutput);

            m_LearningAgent.InitializeLearningMethod();
        }
    }

    public void Update()
    {
        if (!IsLearningMode || updateMode != RLAIUpdateMode.EveryFrame) return;

        if (m_LearningAgent != null)
        {
            m_LearningAgent.UpdateAgentReasoning();
        }
    }

    public void OnSimulationOver(float reinforcementScore)
    {
        IReinforcementLearningAgent agent = m_LearningAgent as IReinforcementLearningAgent;

        if (agent != null)
        {
            agent.OnSimulationOver(reinforcementScore);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RLAIToolAgent))]
public class RLAIToolEditor : Editor {


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RLAIToolAgent tool = target as RLAIToolAgent;

        if (tool == null) return;

        AgentPerception perception = tool.gameObject.GetComponent<AgentPerception>();
        AgentOutput agentOutput = tool.gameObject.GetComponent<AgentOutput>();

        if (perception == null)
        {
            if (GUILayout.Button("Configure Agent Perception"))
            {
                tool.gameObject.AddComponent<AgentPerception>();
            }
        }

        if (agentOutput == null)
        {
            if (GUILayout.Button("Configure Agent Output"))
            {
                tool.gameObject.AddComponent<AgentOutput>();
            }
        }


        EditorGUILayout.Space();

        if (tool.AIMethod == RLAIType.Neuroevolution)
        {
            EditorGUILayout.LabelField("Configure Neuroevolution", EditorStyles.boldLabel);
            int layers = EditorGUILayout.IntField("Neural Layers", tool.neuralArchitecture.Length);

            if (layers != tool.neuralArchitecture.Length)
            {
                int[] newArch = new int[layers];
                for (int a = 0; a < layers && a < tool.neuralArchitecture.Length; a++)
                {
                    newArch[a] = tool.neuralArchitecture[a];
                }
                newArch[newArch.Length-1] = agentOutput.GetOutputSize();
                tool.neuralArchitecture = newArch;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Neural Architecture: ");
            for (int i = 0; i < tool.neuralArchitecture.Length-1; i++)
            {
                tool.neuralArchitecture[i] = EditorGUILayout.IntField("", tool.neuralArchitecture[i], GUILayout.Width(30));
            }
            EditorGUILayout.LabelField("" + tool.neuralArchitecture[tool.neuralArchitecture.Length-1]);
            EditorGUILayout.EndHorizontal();
        }


        EditorGUILayout.Space();

        if (perception != null && agentOutput != null && GUILayout.Button("Start Agent Training"))
        {
            tool.IsLearningMode = true;
            EditorApplication.isPlaying = true;
        }
    }

}
#endif
