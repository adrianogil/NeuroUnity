 using UnityEngine;

public enum PerceptionType
{
    Raycast2D,
    Raycast3D
}

public class AgentPerception : MonoBehaviour
{
    public PerceptionType perceptionMode;
}