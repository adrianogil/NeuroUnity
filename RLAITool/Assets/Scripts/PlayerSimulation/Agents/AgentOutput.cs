using UnityEngine;
using UnityEngine.Events;

using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum OutputType
{
    SimulatedInputKey,
    SendMessage,
    SendMessageFloat

}

[System.Serializable]
public class FloatEvent : UnityEvent <float> {}

[System.Serializable]
public class SimulatedOutputData
{
    public string name;

    public OutputType outputType;

    public UnityEvent messageAction;

    public FloatEvent messageFloatAction;

    public void ActivateOutput(float outputValue)
    {
        if (outputType == OutputType.SendMessage && outputValue >= 0.5)
        {
            messageAction.Invoke();
        }
        else if (outputType == OutputType.SendMessageFloat)
        {
            messageFloatAction.Invoke(outputValue);
        }
    }
}

public class AgentOutput : MonoBehaviour, IAgentOutput
{
    [HideInInspector]
    public List<SimulatedOutputData> agentOutputs = new List<SimulatedOutputData>();

    [HideInInspector] public UnityEvent customEvent;
    [HideInInspector] public FloatEvent customFloatEvent;

    public int GetOutputSize()
    {
        return agentOutputs.Count;
    }

    public void SetOutputValues(double[] output)
    {
        if (output != null)
        {
            for (int o = 0; o < output.Length; o++)
            {
                agentOutputs[o].ActivateOutput((float)output[o]);
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AgentOutput))]
public class AgentOutputEditor : Editor {

    public int currentActionUnfold = -1;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AgentOutput agentOutput = target as AgentOutput;

        if (agentOutput == null) return;

        int totalActions = agentOutput.agentOutputs.Count;

        EditorGUILayout.Space();

        if (totalActions == 1)
        {
            EditorGUILayout.LabelField("1 Action defined", EditorStyles.boldLabel);
        } else {
           EditorGUILayout.LabelField("" + totalActions + " Actions defined", EditorStyles.boldLabel);
        }

        bool unfold;

        for (int i = 0; i < totalActions; i++)
        {
            SimulatedOutputData o = agentOutput.agentOutputs[i];

            unfold = EditorGUILayout.Foldout(currentActionUnfold == i, "Action " + (i+1) + ": " + o.name);

            if (unfold)
            {
                currentActionUnfold = i;

                o.name = EditorGUILayout.TextField("Action name", o.name);
                o.outputType = (OutputType)EditorGUILayout.EnumPopup("Type", o.outputType);

                if (o.outputType == OutputType.SendMessage)
                {
                    agentOutput.customEvent = o.messageAction;
                    SerializedProperty sprop = serializedObject.FindProperty("customEvent");

                    EditorGUIUtility.LookLikeControls();
                    EditorGUILayout.PropertyField(sprop);

                    serializedObject.ApplyModifiedProperties();
                    o.messageAction = agentOutput.customEvent;
                }
                else if (o.outputType == OutputType.SendMessageFloat)
                {
                    agentOutput.customFloatEvent = o.messageFloatAction;
                    SerializedProperty sprop = serializedObject.FindProperty("customFloatEvent");

                    EditorGUIUtility.LookLikeControls();
                    EditorGUILayout.PropertyField(sprop);

                    serializedObject.ApplyModifiedProperties();
                    o.messageFloatAction = agentOutput.customFloatEvent;
                }

                EditorGUILayout.Space();
            }
            else if (currentActionUnfold == i)
            {
                currentActionUnfold = -1;
            }
        }

        if (GUILayout.Button("Add Output"))
        {
            agentOutput.agentOutputs.Add(new SimulatedOutputData()
            {
                outputType = OutputType.SendMessage
            });
        }
    }
}
#endif