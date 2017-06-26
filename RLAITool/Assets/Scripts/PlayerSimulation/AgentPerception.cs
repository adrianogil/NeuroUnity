using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum PerceptionType
{
    Raycast2D,
    Raycast3D
}

public class AgentPerception : MonoBehaviour
{
    public PerceptionType perceptionMode;

    public IPerception perception;

    [HideInInspector] public int numberOfRays;
    [HideInInspector] public double raycastDistance;
    [HideInInspector] public List<CategoryObject> categoriesList = new List<CategoryObject>();

    public void GeneratePerception()
    {
        RLAIToolAgent agent = GetComponent<RLAIToolAgent>();

        CategoryObjectRecognition tagRecognition = new CategoryObjectRecognition(categoriesList.ToArray());

        perception = new Raycast2DPerception(agent.player, numberOfRays, raycastDistance, tagRecognition);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AgentPerception))]
public class AgentPerceptionEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    
        AgentPerception editorObj = target as AgentPerception;
    
        if (editorObj == null) return;

        if (editorObj.perceptionMode == PerceptionType.Raycast2D)
        {
            editorObj.numberOfRays = EditorGUILayout.IntField("Number of Rays", editorObj.numberOfRays);
            editorObj.raycastDistance = EditorGUILayout.DoubleField("Raycast distance", editorObj.raycastDistance);

            EditorGUILayout.LabelField("Categories to be recognized");

            for (int c = 0; c < editorObj.categoriesList.Count; c++)
            {
                CategoryObject cat = editorObj.categoriesList[c];
                cat.CategoryName = EditorGUILayout.TextField("Category Name", cat.CategoryName);

                if (cat.Tags == null) cat.Tags = new string[] {};
                for (int t = 0; t < cat.Tags.Length; t++)
                {
                    cat.Tags[t] = EditorGUILayout.TextField("Tag " + t, cat.Tags[t]);                    
                }
            }

            if (GUILayout.Button("Create category"))
            {
                editorObj.categoriesList.Add(new CategoryObject());
            }
        }
    }

}
#endif