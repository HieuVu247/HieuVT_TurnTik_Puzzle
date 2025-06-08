using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GameModeManager gameModeManager;

    void Start()
    {
        gameModeManager = FindObjectOfType<GameModeManager>();
        // Chúng ta sẽ bỏ qua việc kiểm tra null ở đây vì bạn sẽ thêm GameModeManager sau.
    }

    /// <summary>
    /// Hàm này sẽ được gọi từ ClockHand sau mỗi lần di chuyển xong.
    /// </summary>
    public void CheckWinState(BaseNode nodeA, BaseNode nodeB, ClockHand clockHand)
    {
        // 1. Kiểm tra điều kiện Chuông
        if (!AreAllBellsActive())
        {
            return; 
        }

        // 2. Nếu tất cả chuông đã bật, kiểm tra xem kim có ở đúng đích không
        BaseNode goalNode = null;
        if (nodeA is GoalNode) goalNode = nodeA;
        else if (nodeB is GoalNode) goalNode = nodeB;

        if (goalNode == null) return;
        
        // --- LOGIC KIỂM TRA ĐIỀU KIỆN THẮNG ĐÃ SỬA LẠI ---

        // Nếu ClockHand không có kim giây đang hoạt động, chỉ cần đến đích là thắng
        if (clockHand.secondHand == null || !clockHand.secondHand.gameObject.activeSelf)
        {
            TriggerWinSequence();
            return;
        }
        
        // Nếu có kim giây, HỎI clockHand xem nó có đang ở trên GoalNode không
        if (clockHand.IsSecondHandOnNode(goalNode))
        {
            TriggerWinSequence();
        }
    }
    private bool AreAllBellsActive()
    {
        // Tìm tất cả các chuông trong màn chơi
        BellNode[] allBells = FindObjectsOfType<BellNode>();

        // Nếu không có chuông nào, coi như điều kiện này được thỏa mãn
        if (allBells.Length == 0) return true;

        // Duyệt qua từng chuông
        foreach (var bell in allBells)
        {
            // Nếu tìm thấy dù chỉ một chuông chưa được kích hoạt...
            if (!bell.isBellActive)
            {
                return false; // ...thì trả về false ngay lập tức.
            }
        }

        // Nếu vòng lặp kết thúc mà không có chuông nào bị tắt, trả về true
        return true;
    }

    private void TriggerWinSequence()
    {
        Debug.Log("🎉🎉🎉 BẠN ĐÃ THẮNG! 🎉🎉🎉");

        // Tạm thời vô hiệu hóa kim để người chơi không di chuyển được nữa
        var clockHand = FindObjectOfType<ClockHand>();
        if (clockHand != null) clockHand.enabled = false;

        // Gọi hàm TriggerWin từ GameModeManager để hiện UI chiến thắng
        if (gameModeManager != null)
        {
            gameModeManager.TriggerWin();
        }
    }
}