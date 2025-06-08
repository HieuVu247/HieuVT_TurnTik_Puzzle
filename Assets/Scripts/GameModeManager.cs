using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Thư viện để dùng TextMeshPro

public class GameModeManager : MonoBehaviour
{
    [Header("UI Canvases")]
    public GameObject gameplayCanvas;
    public GameObject winCanvas;
    public GameObject loseCanvas;

    [Header("Timer Settings (Chế độ Timer)")]
    public GameObject timerUI; // Đối tượng UI chứa Text của timer
    public TextMeshProUGUI timerText;
    public float timeLimit = 120f; // Ví dụ: 2 phút
    
    [Header("Win Canvas UI")]
    public TextMeshProUGUI scoreText; // Text để hiển thị điểm (thời gian còn lại)

    private float currentTime;
    private bool isTimerActive = false;
    private int currentLevelIndex;

    void Start()
    {
        // Ẩn các canvas không cần thiết
        if (winCanvas != null) winCanvas.SetActive(false);
        if (loseCanvas != null) loseCanvas.SetActive(false);
        if (gameplayCanvas != null) gameplayCanvas.SetActive(true);

        // Đảm bảo thời gian chạy bình thường khi bắt đầu hoặc chơi lại
        Time.timeScale = 1f;

        // Lấy thông tin level và chế độ chơi
        currentLevelIndex = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        int gameMode = PlayerPrefs.GetInt("GameMode", 2); // Mặc định là không timer

        if (gameMode == 1) // Chế độ Timer
        {
            isTimerActive = true;
            currentTime = timeLimit;
            if (timerUI != null) timerUI.SetActive(true);
        }
        else // Chế độ không Timer
        {
            isTimerActive = false;
            if (timerUI != null) timerUI.SetActive(false);
        }
    }

    void Update()
    {
        if (!isTimerActive) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            TriggerLose();
        }
        
        // Cập nhật hiển thị thời gian
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // --- XỬ LÝ THẮNG/THUA ---

    public void TriggerWin()
    {
        if ((winCanvas != null && winCanvas.activeSelf) || (loseCanvas != null && loseCanvas.activeSelf)) return;
        
        Time.timeScale = 0f; // Dừng game
        if (gameplayCanvas != null) gameplayCanvas.SetActive(false);
        if (winCanvas != null) winCanvas.SetActive(true);

        // Xử lý điểm số và mở khóa level tiếp theo
        if (isTimerActive)
        {
            if (scoreText != null) scoreText.text = "Thời gian còn lại: " + Mathf.FloorToInt(currentTime) + "s";
        }
        else
        {
            if (scoreText != null) scoreText.text = "Bạn đã hoàn thành!";
        }

        int highestLevelUnlocked = PlayerPrefs.GetInt("HighestLevelUnlocked", 1);
        int nextLevel = currentLevelIndex + 2; // +1 vì index, +1 nữa là level tiếp theo
        if (nextLevel > highestLevelUnlocked)
        {
            PlayerPrefs.SetInt("HighestLevelUnlocked", nextLevel);
            PlayerPrefs.Save();
        }
    }
    
    public void TriggerLose()
    {
        if ((winCanvas != null && winCanvas.activeSelf) || (loseCanvas != null && loseCanvas.activeSelf)) return;

        isTimerActive = false;
        Time.timeScale = 0f;
        if (gameplayCanvas != null) gameplayCanvas.SetActive(false);
        if (loseCanvas != null) loseCanvas.SetActive(true);
    }

    // --- CÁC HÀM CHO NÚT BẤM ---
    public void RetryLevel()
    {
        // Khôi phục thời gian trước khi tải lại scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToNextLevel()
    {
        // Khôi phục thời gian trước khi tải scene mới
        Time.timeScale = 1f;
        int nextLevelIndex = currentLevelIndex + 1;
        // ... (phần còn lại của hàm giữ nguyên)
    }

    public void GoToMainMenu()
    {
        // Khôi phục thời gian trước khi về menu
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}