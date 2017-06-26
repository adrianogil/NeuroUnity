 using UnityEngine;

public enum PerceptionType
{
    Raycast2D,
    Raycast3D
}

public class AgentPerception : MonoBehaviour
{
    public PerceptionType perceptionMode;

    public void GeneratePerception()
    {
        // Raycast2DPerception(Transform playerTransform, int numberOfRays, double raycastDistance, IObjectRecognition objRecognition);
    }
}