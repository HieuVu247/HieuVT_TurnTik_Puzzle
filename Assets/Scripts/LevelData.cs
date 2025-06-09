using UnityEngine;
using System.Collections.Generic;

public enum NodeType { Normal, Bell, Disappearing, Goal }

[System.Serializable]
public class NodePlacementInfo
{
    public NodeType nodeType;
    public Vector2 position;
    public float goalRotation;
}

[CreateAssetMenu(fileName = "LevelData_00", menuName = "Puzzle Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Thông tin Level")]
    public int levelIndex;

    [Header("Cấu hình ClockHand")]
    public Vector2 clockHandStartPosition;
    public float clockHandStartRotationZ;
    public bool hasSecondHand;

    [Header("Danh sách các Node")]
    public List<NodePlacementInfo> nodes = new List<NodePlacementInfo>();
}   