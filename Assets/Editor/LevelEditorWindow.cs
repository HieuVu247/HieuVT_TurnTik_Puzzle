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
        ClearScene();
        LevelGenerator.GenerateLevel(currentLevelData, clockHandPrefab, normalNodePrefab, bellNodePrefab, disappearingNodePrefab, goalNodePrefab, null);
        Debug.Log("Đã tải thành công level: " + currentLevelData.name);
    }

    private void SaveSceneToNewData()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save New Level Data", "NewLevelData", "asset", "Vui lòng chọn nơi lưu file");
        if (string.IsNullOrEmpty(path)) return;
        
        LevelData newLevelData = ScriptableObject.CreateInstance<LevelData>();
        PopulateDataFromScene(newLevelData);
        
        AssetDatabase.CreateAsset(newLevelData, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newLevelData;
        
        Debug.Log("Đã lưu level mới tại: " + path);
        ClearScene();
        Debug.Log("Đã dọn dẹp Scene, sẵn sàng để Play Test.");
    }
    
    private void SaveSceneToExistingData(LevelData dataToOverwrite)
    {
        PopulateDataFromScene(dataToOverwrite);
        
        EditorUtility.SetDirty(dataToOverwrite);
        AssetDatabase.SaveAssets();
        Debug.Log("Đã ghi đè thành công level: " + dataToOverwrite.name);

        ClearScene();
        Debug.Log("Đã dọn dẹp Scene, sẵn sàng để Play Test.");
    }

    private void PopulateDataFromScene(LevelData data)
    {
        data.nodes.Clear();

        ClockHand clockHand = FindObjectOfType<ClockHand>();
        if (clockHand != null)
        {
            data.clockHandStartPosition = clockHand.transform.position;
            data.clockHandStartRotationZ = clockHand.transform.eulerAngles.z;

            if (clockHand.secondHand != null)
            {
                data.hasPlayerSecondHand = clockHand.secondHand.gameObject.activeSelf;
                Transform secondHandHandler = clockHand.secondHand.parent;
                data.playerSecondHandRotationZ = secondHandHandler.localEulerAngles.z;
            }
            else
            {
                data.hasPlayerSecondHand = false;
            }
        }

        BaseNode[] nodesInScene = FindObjectsOfType<BaseNode>();
        foreach (var node in nodesInScene)
        {
            NodePlacementInfo info = new NodePlacementInfo();
            info.position = node.transform.position;

            if (node is GoalNode goalNode)
            {
                info.nodeType = NodeType.Goal;
                info.goalMainHandRotation = goalNode.transform.eulerAngles.z;

                GoalNode gnScript = node.GetComponent<GoalNode>();
                if (gnScript != null && gnScript.SecondTargetHandler != null)
                {
                    info.hasTargetSecondHand = gnScript.SecondTargetHandler.activeSelf;
                    info.targetSecondHandRotation = gnScript.SecondTargetHandler.transform.localEulerAngles.z;
                }
            }
            else if (node is BellNode) { info.nodeType = NodeType.Bell; }
            else if (node is DisappearingNode) { info.nodeType = NodeType.Disappearing; }
            else { info.nodeType = NodeType.Normal; }

            data.nodes.Add(info);
        }
    }
    
    private void ClearScene()
    {
        BaseNode[] nodes = FindObjectsOfType<BaseNode>();
        foreach(var node in nodes) if(node != null) DestroyImmediate(node.gameObject);
        
        ClockHand clockHand = FindObjectOfType<ClockHand>();
        if (clockHand != null) DestroyImmediate(clockHand.gameObject);
    }
}