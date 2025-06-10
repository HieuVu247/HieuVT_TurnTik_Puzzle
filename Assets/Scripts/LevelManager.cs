using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private GameModeManager gameModeManager;
    private bool hasWon = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        gameModeManager = FindObjectOfType<GameModeManager>();
    }

    public bool AreAllBellsActive()
    {
        BellNode[] allBells = FindObjectsOfType<BellNode>();
        if (allBells.Length == 0) return true;
        foreach (var bell in allBells)
        {
            if (!bell.isBellActive) return false;
        }
        return true;
    }

    public void TriggerWinSequence()
    {
        if (hasWon) return;
        hasWon = true;
        Debug.Log("🎉🎉🎉 BẠN ĐÃ THẮNG! 🎉🎉🎉");

        var clockHand = FindObjectOfType<ClockHand>();
        if (clockHand != null) clockHand.enabled = false;

        if (gameModeManager != null)
        {
            gameModeManager.TriggerWin();
        }
    }
}