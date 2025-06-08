using UnityEngine;

public abstract class BaseNode : MonoBehaviour
{
    [Header("Cài đặt Gizmo")]
    // Đã cập nhật bán kính mặc định thành 6 đơn vị
    [SerializeField] protected float gizmoRadius = 6f;
    [SerializeField] protected Color gizmoColor = Color.white;

    // OnDrawGizmos đã được cập nhật để vẽ vòng tròn 2D
    protected virtual void OnDrawGizmos()
    {
        // Vẽ một vòng tròn 2D rỗng tại vị trí của Node
        DrawWireCircle(transform.position, gizmoRadius, 32);
    }

    /// <summary>
    /// Hàm trợ giúp vẽ một vòng tròn 2D bằng các đường thẳng.
    /// </summary>
    protected void DrawWireCircle(Vector3 center, float radius, int segments)
    {
        Gizmos.color = gizmoColor; // Luôn đảm bảo màu được set trước khi vẽ
        float angle = 0f;
        Vector3 lastPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);

        for (int i = 1; i <= segments; i++)
        {
            angle = i * (360f / segments) * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }
    
    public virtual void SetConnected(bool isConnected)
    {
        // Lớp cơ sở không làm gì cả, để cho các lớp con tự định nghĩa
    }
}