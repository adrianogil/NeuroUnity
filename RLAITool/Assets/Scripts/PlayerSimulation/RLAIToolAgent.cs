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

public class RLAIToolAgent : MonoBehaviour
{
    public RLAIType AIMethod;

    public RLAILearningMode AILearningMode;

    public Transform player;

    [HideInInspector] public UnityEvent RestartGameplay;
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
    }

}
#endif
