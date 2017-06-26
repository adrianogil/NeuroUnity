using UnityEngine;
using UnityEngine.Events;

using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum OutputType
{
    SimulatedInputKey,
    SendMessage
}

[System.Serializable]
public class SimulatedOutputData
{
    public string name;

    public OutputType outputType;

    public UnityEvent messageAction;

    public void ActivateOutput()
    {
        if (outputType == OutputType.SendMessage)
        {
            messageAction.Invoke();
        }
    }
}

public class AgentOutput : MonoBehaviour, IAgentOutput
{
    [HideInInspector]
    public List<SimulatedOutputData> agentOutputs = new List<SimulatedOutputData>();

    [HideInInspector]
    public UnityEvent customEvent;

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
                if (output[o] >= 0.5) {
                    agentOutputs[o].ActivateOutput();
                }
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