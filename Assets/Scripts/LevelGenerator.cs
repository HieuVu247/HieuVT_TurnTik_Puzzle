using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Dữ liệu Level để dựng")]
    public LevelData levelToGenerate;

    [Header("Các Prefab cần thiết")]
    public GameObject clockHandPrefab;
    public GameObject normalNodePrefab;
    public GameObject bellNodePrefab;
    public GameObject disappearingNodePrefab;
    public GameObject goalNodePrefab;
    
    [Header("Game Levels Reference")]
    public GameLevels gameLevels;
    void Start()
    {
        // Đọc chỉ số level đã được lưu từ Menu
        int levelIndex = PlayerPrefs.GetInt("SelectedLevelIndex", 0);

        // Đảm bảo chỉ số hợp lệ
        if (levelIndex >= 0 && levelIndex < gameLevels.allLevels.Count)
        {
            LevelData dataToLoad = gameLevels.allLevels[levelIndex];
            GenerateLevel(dataToLoad, clockHandPrefab, normalNodePrefab, bellNodePrefab, disappearingNodePrefab, goalNodePrefab, this.transform);
        }
        else
        {
            Debug.LogError("Chỉ số level không hợp lệ!");
        }
    }

    // Hàm được sửa thành static để có thể gọi từ Editor Script
    public static void GenerateLevel(LevelData data, GameObject clockHandPrefab, GameObject normalNodePrefab, GameObject bellNodePrefab, GameObject disappearingNodePrefab, GameObject goalNodePrefab, Transform parent)
    {
        if (data == null) return;
        
        GameObject clockHandInstance = Instantiate(clockHandPrefab, data.clockHandStartPosition, Quaternion.Euler(0, 0, data.clockHandStartRotationZ));
        ClockHand clockHand = clockHandInstance.GetComponent<ClockHand>();
        if (clockHand != null && clockHand.secondHand != null)
        {
            clockHand.secondHand.gameObject.SetActive(data.hasPlayerSecondHand);
            clockHand.secondHand.parent.localRotation = Quaternion.Euler(0, 0, data.playerSecondHandRotationZ);
        }

        foreach (var nodeInfo in data.nodes)
        {
            GameObject prefabToSpawn = GetPrefabForNodeType(nodeInfo.nodeType, normalNodePrefab, bellNodePrefab, disappearingNodePrefab, goalNodePrefab);
            if (prefabToSpawn != null)
            {
                GameObject nodeInstance = Instantiate(prefabToSpawn, nodeInfo.position, Quaternion.identity, parent);
                
                if (nodeInfo.nodeType == NodeType.Goal)
                {
                    GoalNode goalNodeScript = nodeInstance.GetComponent<GoalNode>();
                    if (goalNodeScript != null)
                    {
                        goalNodeScript.Configure(nodeInfo);
                    }
                }
            }
        }
    }

    private static GameObject GetPrefabForNodeType(NodeType type, GameObject normal, GameObject bell, GameObject disappearing, GameObject goal)
    {
        switch (type)
        {
            case NodeType.Normal:       return normal;
            case NodeType.Bell:         return bell;
            case NodeType.Disappearing: return disappearing;
            case NodeType.Goal:         return goal;
            default:                    return null;
        }
    }
}