using UnityEngine;
using UnityEditor;

public class RLAIConfiguration
{
    [MenuItem("RLAI/Add RLAI Agent")]
    public static void AddAITool()
    {
        // GameObject rlaiToolObject = GameObject.FindWithTag("RLAITool");

        // if (rlaiToolObject == null){
        GameObject rlaiAgent = new GameObject("RLAIAgent");
        rlaiAgent.tag = "RLAITool";
        rlaiAgent.AddComponent<RLAIToolAgent>();
        // }
        
    }
}