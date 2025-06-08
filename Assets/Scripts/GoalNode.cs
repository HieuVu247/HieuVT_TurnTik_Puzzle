using UnityEngine;

public class GoalNode : BaseNode
{
    // Ghi đè lại hàm Gizmo để vẽ màu khác biệt cho Nút Đích
    protected override void OnDrawGizmos()
    {
        gizmoColor = Color.green; // Màu xanh lá cây cho may mắn!
        base.OnDrawGizmos();
    }
}