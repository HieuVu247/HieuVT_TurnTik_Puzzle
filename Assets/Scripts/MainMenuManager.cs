using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Data & Prefabs")]
    public GameLevels gameLevelsConfig;
    public GameObject levelButtonPrefab;
    public GameObject lockIconPrefab;

    [Header("Canvases")]
    public GameObject mainMenuCanvas;
    public GameObject helpCanvas;
    public GameObject modeSelectCanvas;
    public GameObject levelSelectCanvas;

    [Header("UI Containers")]
    public Transform levelButtonContainer;

    void Start()
    {
        // Khi bắt đầu, luôn hiển thị Menu chính và tắt các màn hình khác
        mainMenuCanvas.SetActive(true);
        helpCanvas.SetActive(false);
        modeSelectCanvas.SetActive(false);
        levelSelectCanvas.SetActive(false);
    }
    
    // --- CÁC HÀM QUẢN LÝ VIỆC HIỂN THỊ CANVAS ---

    private void ShowMainCanvas(GameObject canvasToShow)
    {
        mainMenuCanvas.SetActive(false);
        modeSelectCanvas.SetActive(false);
        levelSelectCanvas.SetActive(false);
        canvasToShow.SetActive(true);
    }

    // --- CÁC HÀM GỌI TỪ NÚT BẤM ---

    public void OnClickStudy()
    {
        int highestLevelUnlocked = PlayerPrefs.GetInt("HighestLevelUnlocked", 1);
        if (highestLevelUnlocked <= 1)
        {
            OnClickHelp(); // Nếu là người chơi mới, bật màn hình Help
        }
        else
        {
            ShowMainCanvas(modeSelectCanvas);
        }
    }

    public void OnClickHelp()
    {
        helpCanvas.SetActive(true); // Chỉ đơn giản là bật màn hình Help
    }

    public void CloseHelp()
    {
        helpCanvas.SetActive(false); // Chỉ đơn giản là tắt màn hình Help
    }

    public void SelectMode(int mode) // 1 = Timer, 2 = No Timer
    {
        PlayerPrefs.SetInt("GameMode", mode);
        PlayerPrefs.Save();
        
        ShowMainCanvas(levelSelectCanvas);
        // Tạo lại các nút mỗi khi vào màn hình này
        PopulateLevelButtons(); 
    }

    public void GoBackToMainMenu()
    {
        ShowMainCanvas(mainMenuCanvas);
    }
    
    // --- LOGIC TẠO VÀ KHÓA LEVEL ---

    private void PopulateLevelButtons()
    {
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }

        int highestLevelUnlocked = PlayerPrefs.GetInt("HighestLevelUnlocked", 1);

        for (int i = 0; i < gameLevelsConfig.allLevels.Count; i++)
        {
            GameObject buttonGO = Instantiate(levelButtonPrefab, levelButtonContainer);
            buttonGO.GetComponentInChildren<Text>().text = (i + 1).ToString();
            
            Button button = buttonGO.GetComponent<Button>();
            int levelIndex = i;

            if ((i + 1) > highestLevelUnlocked)
            {
                button.interactable = false;
                if(lockIconPrefab != null) Instantiate(lockIconPrefab, button.transform);
            }
            else
            {
                button.interactable = true;
                button.onClick.AddListener(() => PlayLevel(levelIndex));
            }
        }
    }

    private void PlayLevel(int levelIndex)
    {
        PlayerPrefs.SetInt("SelectedLevelIndex", levelIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameplayScene");
    }
}