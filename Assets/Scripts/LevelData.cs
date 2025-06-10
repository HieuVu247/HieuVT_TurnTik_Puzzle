using UnityEngine;
using System.Collections.Generic;

public enum NodeType { Normal, Bell, Disappearing, Goal }

[System.Serializable]
public class NodePlacementInfo
{
    public NodeType nodeType;
    public Vector2 position;

    // Các thông tin riêng cho GoalNode
    public float goalMainHandRotation;    // Góc xoay của kim giờ mục tiêu
    public bool hasTargetSecondHand;      // Mục tiêu có kim giây không?
    public float targetSecondHandRotation; // Góc xoay của kim giây mục tiêu
}

[CreateAssetMenu(fileName = "LevelData_00", menuName = "Puzzle Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Thông tin Level")]
    public int levelIndex;

    [Header("Cấu hình ClockHand của Người chơi")]
    public Vector2 clockHandStartPosition;
    public float clockHandStartRotationZ;
    public bool hasPlayerSecondHand; // Người chơi có kim giây không?
    public float playerSecondHandRotationZ; // Góc xoay của kim giây người chơi

    [Header("Danh sách các Node")]
    public List<NodePlacementInfo> nodes = new List<NodePlacementInfo>();
}