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
    [HideInInspector] public int numberOfLatitudeRays;
    [HideInInspector] public int numberOfLongitudeRays;

    [HideInInspector] public float initialLatitude;
    [HideInInspector] public float finalLatitude;

    [HideInInspector] public float initialLongitude;
    [HideInInspector] public float finalLongitude;

    [HideInInspector] public double raycastDistance;
    [HideInInspector] public List<CategoryObject> categoriesList = new List<CategoryObject>();

    public void GeneratePerception()
    {
        RLAIToolAgent agent = GetComponent<RLAIToolAgent>();

        CategoryObjectRecognition tagRecognition = new CategoryObjectRecognition(categoriesList.ToArray());

        if (perceptionMode == PerceptionType.Raycast2D)
        {
            perception = new Raycast2DPerception(agent.player, numberOfRays, raycastDistance, tagRecognition);
        }
        else if (perceptionMode == PerceptionType.Raycast3D)
        {
            perception = new Raycast3DPerception(agent.player, numberOfLatitudeRays, numberOfLongitudeRays, 
                                                 initialLatitude, finalLatitude, initialLongitude, finalLongitude,
                                                 raycastDistance, tagRecognition);
        }
        
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AgentPerception))]
public class AgentPerceptionEditor : Editor {

    public int currentCategoryUnfold = -1;
    public PerceptionType lastPerceptionType;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        bool changed = false;

        AgentPerception editorObj = target as AgentPerception;

        if (editorObj == null) return;

        if (editorObj.perceptionMode != lastPerceptionType)
        {
            changed = true;
        }
        lastPerceptionType = editorObj.perceptionMode;

        EditorGUILayout.Space();

        if (editorObj.perceptionMode == PerceptionType.Raycast2D)
        {
            EditRaycast2D(editorObj, changed);
        } else if (editorObj.perceptionMode == PerceptionType.Raycast3D)
        {
            EditRaycast3D(editorObj, changed);
        }
    }

    public void EditRaycast2D(AgentPerception editorObj, bool changed)
    {   
        // EditorGUILayout.BeginHorizontal();
        editorObj.numberOfRays = EditorGUILayout.IntField("Number of Rays", editorObj.numberOfRays);
        editorObj.raycastDistance = EditorGUILayout.DoubleField("Raycast distance", editorObj.raycastDistance);
        // EditorGUILayout.EndHorizontal();

        int totalCategories = editorObj.categoriesList.Count;

        if (totalCategories == 1)
        {
            EditorGUILayout.LabelField("1 Category to be recognized", EditorStyles.boldLabel);
        } else {
            EditorGUILayout.LabelField("" + totalCategories + " Categories to be recognized", EditorStyles.boldLabel);
        }

        EditorGUI.indentLevel++;

        bool unfold;

        List<int> categoriesToBeRemoved = new List<int>();

        for (int c = 0; c < editorObj.categoriesList.Count; c++)
        {
            CategoryObject cat = editorObj.categoriesList[c];

            EditorGUILayout.BeginHorizontal();
            unfold = EditorGUILayout.Foldout(currentCategoryUnfold == c, "Category " + (c+1) + ": " + cat.CategoryName);
            if (unfold && GUILayout.Button("Remove category"))
            {
                categoriesToBeRemoved.Add(c);
            }
            EditorGUILayout.EndHorizontal();

            if (unfold) {
                currentCategoryUnfold = c;
                cat.CategoryName = EditorGUILayout.TextField("Category Name", cat.CategoryName);

                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

                List<int> tagsToBeRemoved = new List<int>();

                if (cat.Tags == null) cat.Tags = new string[] {""};
                for (int t = 0; t < cat.Tags.Length; t++)
                {
                    EditorGUILayout.BeginHorizontal();
                    cat.Tags[t] = EditorGUILayout.TextField("Tag " + t, cat.Tags[t]);
                    if (GUILayout.Button("Remove"))
                    {
                        changed = true;
                        tagsToBeRemoved.Add(t);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (tagsToBeRemoved.Count > 0)
                {
                    string[] newTagArray = new string[cat.Tags.Length - tagsToBeRemoved.Count];
                    int index = 0;
                    for (int i = 0; i < newTagArray.Length; i++)
                    {
                        for (int a = 0; a < tagsToBeRemoved.Count; a++)
                        {
                            if (index == tagsToBeRemoved[a])
                            {
                                index++;
                            }
                        }
                        if (index >= 0 && index < cat.Tags.Length)
                            newTagArray[i] = cat.Tags[index];
                        index++;
                    }
                    cat.Tags = newTagArray;
                }

                // GUIStyle buttonStyle = new GUIStyle();
                // buttonStyle.alignment = TextAnchor.MiddleRight;
                if (GUILayout.Button("Add Tag", GUILayout.Width(100)))
                {
                    changed = true;
                    string[] newTagArray = new string[cat.Tags.Length + 1];
                    for (int i = 0; i < cat.Tags.Length; i++)
                    {
                        newTagArray[i] = cat.Tags[i];
                    }
                    cat.Tags = newTagArray;
                }

                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            } else if (currentCategoryUnfold == c)
            {
                currentCategoryUnfold = -1;
            }
        }

        for (int i = 0; i < categoriesToBeRemoved.Count; i++)
        {
            changed = true;
            editorObj.categoriesList.RemoveAt(categoriesToBeRemoved[i]);
        }

        EditorGUI.indentLevel--;

        if (GUILayout.Button("Create category", GUILayout.Width(100)))
        {
            changed = true;
            editorObj.categoriesList.Add(new CategoryObject());
        }

        if (editorObj.perception == null || changed)
        {
            editorObj.GeneratePerception();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Perception Feature Size:  " + editorObj.perception.GetFeatureSize(), EditorStyles.boldLabel);
    }

    public void EditRaycast3D(AgentPerception editorObj, bool changed)
    {   

        int lastIntValue;
        float lastFloatValue;

        // EditorGUILayout.BeginHorizontal();
        lastIntValue = EditorGUILayout.IntField("Number of Rays Latitude", editorObj.numberOfLatitudeRays);
        if (editorObj.numberOfLatitudeRays != lastIntValue) {editorObj.numberOfLatitudeRays = lastIntValue; changed = true; }
        
        lastIntValue = EditorGUILayout.IntField("Number of Rays Longitude", editorObj.numberOfLongitudeRays);
        if (editorObj.numberOfLongitudeRays != lastIntValue) {editorObj.numberOfLongitudeRays = lastIntValue; changed = true; }

        lastFloatValue = EditorGUILayout.FloatField("Initial Latitude", editorObj.initialLatitude);
        if (editorObj.initialLatitude != lastFloatValue) {editorObj.initialLatitude = lastFloatValue; changed = true; }

        lastFloatValue = EditorGUILayout.FloatField("Final Latitude", editorObj.finalLatitude);
        if (editorObj.finalLatitude != editorObj.finalLatitude) {editorObj.finalLatitude = lastFloatValue; changed = true; }

        lastFloatValue = EditorGUILayout.FloatField("Initial Longitude", editorObj.initialLongitude);
        if (editorObj.finalLongitude != lastFloatValue) {editorObj.initialLongitude = lastFloatValue; changed = true; }

        lastFloatValue = EditorGUILayout.FloatField("Final Longitude", editorObj.finalLongitude);
        if (editorObj.finalLongitude != lastFloatValue) {editorObj.finalLongitude = lastFloatValue; changed = true; }

        double lastDoubleValue = EditorGUILayout.DoubleField("Raycast distance", editorObj.raycastDistance);
        if (editorObj.raycastDistance != lastDoubleValue) {editorObj.raycastDistance = lastDoubleValue; changed = true; }

        // EditorGUILayout.EndHorizontal();

        int totalCategories = editorObj.categoriesList.Count;

        if (totalCategories == 1)
        {
            EditorGUILayout.LabelField("1 Category to be recognized", EditorStyles.boldLabel);
        } else {
            EditorGUILayout.LabelField("" + totalCategories + " Categories to be recognized", EditorStyles.boldLabel);
        }

        EditorGUI.indentLevel++;

        bool unfold;

        List<int> categoriesToBeRemoved = new List<int>();

        for (int c = 0; c < editorObj.categoriesList.Count; c++)
        {
            CategoryObject cat = editorObj.categoriesList[c];

            EditorGUILayout.BeginHorizontal();
            unfold = EditorGUILayout.Foldout(currentCategoryUnfold == c, "Category " + (c+1) + ": " + cat.CategoryName);
            if (unfold && GUILayout.Button("Remove category"))
            {
                categoriesToBeRemoved.Add(c);
            }
            EditorGUILayout.EndHorizontal();

            if (unfold) {
                currentCategoryUnfold = c;
                cat.CategoryName = EditorGUILayout.TextField("Category Name", cat.CategoryName);

                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

                List<int> tagsToBeRemoved = new List<int>();

                if (cat.Tags == null) cat.Tags = new string[] {""};
                for (int t = 0; t < cat.Tags.Length; t++)
                {
                    EditorGUILayout.BeginHorizontal();
                    cat.Tags[t] = EditorGUILayout.TextField("Tag " + t, cat.Tags[t]);
                    if (GUILayout.Button("Remove"))
                    {
                        changed = true;
                        tagsToBeRemoved.Add(t);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (tagsToBeRemoved.Count > 0)
                {
                    string[] newTagArray = new string[cat.Tags.Length - tagsToBeRemoved.Count];
                    int index = 0;
                    for (int i = 0; i < newTagArray.Length; i++)
                    {
                        for (int a = 0; a < tagsToBeRemoved.Count; a++)
                        {
                            if (index == tagsToBeRemoved[a])
                            {
                                index++;
                            }
                        }
                        if (index >= 0 && index < cat.Tags.Length)
                            newTagArray[i] = cat.Tags[index];
                        index++;
                    }
                    cat.Tags = newTagArray;
                }

                // GUIStyle buttonStyle = new GUIStyle();
                // buttonStyle.alignment = TextAnchor.MiddleRight;
                if (GUILayout.Button("Add Tag", GUILayout.Width(100)))
                {
                    changed = true;
                    string[] newTagArray = new string[cat.Tags.Length + 1];
                    for (int i = 0; i < cat.Tags.Length; i++)
                    {
                        newTagArray[i] = cat.Tags[i];
                    }
                    cat.Tags = newTagArray;
                }

                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            } else if (currentCategoryUnfold == c)
            {
                currentCategoryUnfold = -1;
            }
        }

        for (int i = 0; i < categoriesToBeRemoved.Count; i++)
        {
            changed = true;
            editorObj.categoriesList.RemoveAt(categoriesToBeRemoved[i]);
        }

        EditorGUI.indentLevel--;

        if (GUILayout.Button("Create category", GUILayout.Width(100)))
        {
            changed = true;
            editorObj.categoriesList.Add(new CategoryObject());
        }

        if (editorObj.perception == null || changed)
        {
            editorObj.GeneratePerception();
        }

        editorObj.perception.DebugDraw();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Perception Feature Size:  " + editorObj.perception.GetFeatureSize(), EditorStyles.boldLabel);
    }

}
#endif