using UnityEngine;

public class GoalNode : BaseNode
{
    [Header("Các thành phần con")]
    public GameObject secondHandVisual; // Kéo GO kim giây mục tiêu vào đây

    /// <summary>
    /// Hàm này được gọi bởi LevelGenerator để cài đặt cho GoalNode
    /// </summary>
    public void Configure(float targetRotation, bool showSecondHand)
    {
        // Xoay toàn bộ đối tượng GoalNode (tức là TargetHandler)
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);

        // Bật hoặc tắt hình ảnh kim giây
        if (secondHandVisual != null)
        {
            secondHandVisual.SetActive(showSecondHand);
        }
    }
    
    // Ghi đè lại hàm Gizmo để vẽ màu khác biệt cho Nút Đích
    protected override void OnDrawGizmos()
    {
        gizmoColor = Color.green; // Màu xanh lá cây cho may mắn!
        base.OnDrawGizmos();
    }
}