using UnityEngine;
using UnityEditor; // Thư viện cần thiết cho lập trình Editor
using System.Collections.Generic;

public class LevelEditorWindow : EditorWindow
{
    // Biến để lưu trữ LevelData đang được chỉnh sửa
    private LevelData currentLevelData;

    // Biến để lưu trữ các Prefab
    private GameObject clockHandPrefab;
    private GameObject normalNodePrefab;
    private GameObject bellNodePrefab;
    private GameObject disappearingNodePrefab;
    private GameObject goalNodePrefab;

    // Hàm để tạo menu item mở cửa sổ này
    [MenuItem("Window/Puzzle Game/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    // Hàm để vẽ giao diện cho cửa sổ Editor
    private void OnGUI()
    {
        GUILayout.Label("Level Editor Tool", EditorStyles.boldLabel);
        
        // Ô để kéo thả LevelData asset
        currentLevelData = (LevelData)EditorGUILayout.ObjectField("Current Level Data", currentLevelData, typeof(LevelData), false);

        // Nút để tải dữ liệu từ LevelData ra Scene
        if (GUILayout.Button("Load Data to Scene"))
        {
            if (currentLevelData != null)
            {
                LoadLevelIntoScene();
            }
            else
            {
                EditorUtility.DisplayDialog("Lỗi", "Vui lòng chọn một Level Data để tải.", "OK");
            }
        }
        
        EditorGUILayout.Space();
        
        // Các ô để kéo thả Prefab
        GUILayout.Label("Prefabs", EditorStyles.boldLabel);
        clockHandPrefab = (GameObject)EditorGUILayout.ObjectField("ClockHand Prefab", clockHandPrefab, typeof(GameObject), false);
        normalNodePrefab = (GameObject)EditorGUILayout.ObjectField("NormalNode Prefab", normalNodePrefab, typeof(GameObject), false);
        bellNodePrefab = (GameObject)EditorGUILayout.ObjectField("BellNode Prefab", bellNodePrefab, typeof(GameObject), false);
        disappearingNodePrefab = (GameObject)EditorGUILayout.ObjectField("DisappearingNode Prefab", disappearingNodePrefab, typeof(GameObject), false);
        goalNodePrefab = (GameObject)EditorGUILayout.ObjectField("GoalNode Prefab", goalNodePrefab, typeof(GameObject), false);
        
        EditorGUILayout.Space();

        // Nút để lưu Scene hiện tại thành một LevelData MỚI
        if (GUILayout.Button("Save Scene to NEW Level Data"))
        {
            SaveSceneToNewData();
        }
        
        // Nút để ghi đè dữ liệu vào LevelData đã chọn
        if (GUILayout.Button("Overwrite Selected Level Data"))
        {
            if (currentLevelData != null)
            {
                SaveSceneToExistingData(currentLevelData);
            }
            else
            {
                EditorUtility.DisplayDialog("Lỗi", "Vui lòng chọn một Level Data để ghi đè.", "OK");
            }
        }
    }

    private void LoadLevelIntoScene()
    {
        // Xóa các đối tượng level cũ trước khi tải
        ClearScene();
        
        // Dựng level từ data (tương tự LevelGenerator)
        LevelGenerator.GenerateLevel(currentLevelData, clockHandPrefab, normalNodePrefab, bellNodePrefab, disappearingNodePrefab, goalNodePrefab, null);
        Debug.Log("Đã tải thành công level: " + currentLevelData.name);
    }

    private void SaveSceneToNewData()
    {
        // Mở cửa sổ để người dùng chọn nơi lưu file
        string path = EditorUtility.SaveFilePanelInProject("Save New Level Data", "NewLevelData", "asset", "Vui lòng chọn nơi lưu file");
        if (string.IsNullOrEmpty(path)) return;
        
        LevelData newLevelData = ScriptableObject.CreateInstance<LevelData>();
        PopulateDataFromScene(newLevelData);
        
        AssetDatabase.CreateAsset(newLevelData, path);
        AssetDatabase.SaveAssets();
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newLevelData;
        
        ClearScene(); 
        Debug.Log("Đã lưu level mới tại: " + path);
    }
    
    private void SaveSceneToExistingData(LevelData dataToOverwrite)
    {
        PopulateDataFromScene(dataToOverwrite);
        
        // Đánh dấu asset là đã bị thay đổi để Unity biết cần lưu lại
        EditorUtility.SetDirty(dataToOverwrite);
        AssetDatabase.SaveAssets();
        ClearScene(); 
        Debug.Log("Đã ghi đè thành công level: " + dataToOverwrite.name);
    }

    private void PopulateDataFromScene(LevelData data)
    {
        data.nodes.Clear();
        BaseNode[] nodesInScene = FindObjectsOfType<BaseNode>();
        foreach (var node in nodesInScene)
        {
            NodePlacementInfo info = new NodePlacementInfo();
            info.position = node.transform.position;
            info.nodeType = GetNodeTypeFromObject(node, info); 
            data.nodes.Add(info);
        }
        
        ClockHand clockHand = FindObjectOfType<ClockHand>();
        if (clockHand != null)
        {
            data.clockHandStartPosition = clockHand.transform.position;
            data.clockHandStartRotationZ = clockHand.transform.eulerAngles.z;
            data.hasSecondHand = (clockHand.secondHand != null && clockHand.secondHand.gameObject.activeSelf);
        }
    }
    
    private void ClearScene()
    {
        BaseNode[] nodes = FindObjectsOfType<BaseNode>();
        foreach(var node in nodes) DestroyImmediate(node.gameObject);
        
        ClockHand clockHand = FindObjectOfType<ClockHand>();
        if (clockHand != null) DestroyImmediate(clockHand.gameObject);
    }
    
    private NodeType GetNodeTypeFromObject(BaseNode node, NodePlacementInfo infoToPopulate)
    {
        if (node is GoalNode goalNode) // Sửa lại một chút ở đây
        {
            // Lưu lại góc xoay của GoalNode vào data
            infoToPopulate.goalRotation = goalNode.transform.eulerAngles.z;
            return NodeType.Goal;
        }
        if (node is BellNode) return NodeType.Bell;
        if (node is DisappearingNode) return NodeType.Disappearing;
        return NodeType.Normal;
    }
}