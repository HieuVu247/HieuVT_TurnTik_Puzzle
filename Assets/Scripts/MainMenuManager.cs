using TMPro;
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
            OnClickHelp();
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

    public void SelectMode(int mode)
    {
        PlayerPrefs.SetInt("GameMode", mode);
        PlayerPrefs.Save();
        
        ShowMainCanvas(levelSelectCanvas);
        PopulateLevelButtons(); 
    }

    public void GoBackToMainMenu()
    {
        ShowMainCanvas(mainMenuCanvas);
    }
    
    // --- LOGIC TẠO VÀ KHÓA LEVEL ---

    private void PopulateLevelButtons()
    {
        // 1. Xóa các nút cũ đi để tránh tạo trùng lặp
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. Lấy tiến trình của người chơi
        int highestLevelUnlocked = PlayerPrefs.GetInt("HighestLevelUnlocked", 1);

        // 3. Vòng lặp sẽ chạy qua TẤT CẢ các level có trong file config
        for (int i = 0; i < gameLevelsConfig.allLevels.Count; i++)
        {
            // Tạo nút từ Prefab
            GameObject buttonGO = Instantiate(levelButtonPrefab, levelButtonContainer);
            // Lấy component Button từ đối tượng vừa tạo
            Button button = buttonGO.GetComponent<Button>();
            // Ghi số level lên nút
            buttonGO.GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();
            
            // Phải tạo một biến tạm 'levelIndex' ở đây để listener không bị lỗi
            int levelIndex = i;

            // 4. Kiểm tra xem level này đã được mở khóa hay chưa
            if ((i + 1) > highestLevelUnlocked)
            {
                // Nếu BỊ KHÓA:
                button.interactable = false; // Tắt khả năng tương tác của nút
                if(lockIconPrefab != null) Instantiate(lockIconPrefab, button.transform); // Thêm icon ổ khóa
            }
            else
            {
                // Nếu ĐÃ MỞ:
                button.interactable = true; // Bật khả năng tương tác
                // Gán sự kiện OnClick bằng code
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