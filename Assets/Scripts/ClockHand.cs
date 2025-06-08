using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClockHand : MonoBehaviour
{
    [Header("Các thành phần của kim")]
    public Transform pointA;
    public Transform pointB;

    [Header("Thành phần phụ")]
    public Transform secondHand; 
    
    [Header("Cài đặt xoay")]
    public float rotationSpeed = 180f;

    private bool isRotating = false;
    private BaseNode nodeAtA;
    private BaseNode nodeAtB;
    private float armLength;
    private BaseNode prevNodeA;
    private BaseNode prevNodeB;
    // ----- CÁC HÀM KHỞI TẠO -----

    void Start()
    {
        armLength = Vector3.Distance(pointA.position, pointB.position);
        UpdateAttachedNodes();
    }

    void Update()
    {
        if (!isRotating && Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        BaseNode clickedNode = FindClosestNode(mouseWorldPos, 0.5f);

        if (clickedNode == null) return;
        
        Transform pivot = null;
        Transform rotating = null;

        if (clickedNode == nodeAtA)
        {
            pivot = pointA;
            rotating = pointB;
        }
        else if (clickedNode == nodeAtB)
        {
            pivot = pointB;
            rotating = pointA;
        }

        if (pivot != null)
        {
            BaseNode destination = FindNextClockwiseNode(pivot, rotating);
            if (destination != null)
            {
                StartCoroutine(RotateArmCoroutine(pivot, rotating, destination));
            }
            else
            {
                Debug.Log("Không tìm thấy điểm đến hợp lệ.");
            }
        }
    }
    
    // --- COROUTINE VỚI LOGIC SNAP CUỐI CÙNG, CHÍNH XÁC NHẤT ---
    private IEnumerator RotateArmCoroutine(Transform pivot, Transform rotating, BaseNode destinationNode)
    {
        // BƯỚC 1: KHỞI TẠO VÀ TÍNH TOÁN (Giữ nguyên)
        isRotating = true;
        Debug.Log("Đã tìm thấy điểm đến: " + destinationNode.name + ". Bắt đầu xoay.");

        Vector3 startDirection = rotating.position - pivot.position;
        Vector3 endDirection = destinationNode.transform.position - pivot.position;
        
        float signedAngle = Vector3.SignedAngle(startDirection, endDirection, Vector3.forward);
        float totalAngleToRotate;
        if (signedAngle < 0) totalAngleToRotate = -signedAngle;
        else if (signedAngle > 0) totalAngleToRotate = 360 - signedAngle;
        else totalAngleToRotate = 360;
        
        float angleRotated = 0f;

        // BƯỚC 2: GIAI ĐOẠN ANIMATION XOAY (Giữ nguyên)
        while (angleRotated < totalAngleToRotate)
        {
            float step = rotationSpeed * Time.deltaTime;
            if (angleRotated + step > totalAngleToRotate) step = totalAngleToRotate - angleRotated;
            
            transform.RotateAround(pivot.position, Vector3.back, step);
            angleRotated += step;
            yield return null; 
        }

        // --- BƯỚC 3: GIAI ĐOẠN SNAP ĐÃ SỬA LỖI TRIỆT ĐỂ ---

        // Tính toán vector "up" cuối cùng một cách thông minh
        Vector3 finalUpVector;

        // Kiểm tra xem đầu kim nào đã thực hiện việc xoay
        if (rotating == pointA)
        {
            // Nếu PointA xoay, thì vector "up" (hướng của PointA) phải hướng về đích
            finalUpVector = destinationNode.transform.position - pivot.position;
        }
        else // (rotating == pointB)
        {
            // Nếu PointB xoay, thì vector "up" (hướng của PointA) phải hướng về TÂM XOAY
            // để đảm bảo PointB hướng về đích.
            finalUpVector = pivot.position - destinationNode.transform.position;
        }
        
        // Tạo góc xoay cuối cùng dựa trên vector đã tính toán
        Quaternion finalRotation = Quaternion.LookRotation(Vector3.forward, finalUpVector.normalized);
        
        // Tính toán vị trí cuối cùng
        Vector3 finalPosition = (pivot.position + destinationNode.transform.position) / 2;

        // Áp dụng vị trí và góc xoay hoàn hảo
        transform.position = finalPosition;
        transform.rotation = finalRotation;
        
        // BƯỚC 4: HOÀN TẤT (Giữ nguyên)
        isRotating = false;
        UpdateAttachedNodes();
        HandleSpecialNodesOnMoveComplete();
        EnsureSecondHandParent(); // Vẫn giữ hàm này để đảm bảo kim giây không bao giờ sai
        FindObjectOfType<LevelManager>()?.CheckWinState(nodeAtA, nodeAtB, this);
        Debug.Log("Xoay hoàn tất!");
    }
    // ----- CÁC HÀM HỖ TRỢ -----
    
    private void UpdateAttachedNodes()
    {
        // Lưu lại các node cũ
        prevNodeA = nodeAtA;
        prevNodeB = nodeAtB;

        // Tìm các node mới
        nodeAtA = FindClosestNode(pointA.position, 0.1f);
        nodeAtB = FindClosestNode(pointB.position, 0.1f);

        // Cập nhật trạng thái "kết nối"
        prevNodeA?.SetConnected(false); // Tắt node cũ A
        prevNodeB?.SetConnected(false); // Tắt node cũ B
        nodeAtA?.SetConnected(true);    // Bật node mới A
        nodeAtB?.SetConnected(true);    // Bật node mới B
    }
    
    private void HandleSpecialNodesOnMoveComplete()
    {
        DisappearingNode[] allDisappearingNodes = FindObjectsOfType<DisappearingNode>();
        foreach (var dNode in allDisappearingNodes)
        {
            if (dNode != nodeAtA && dNode != nodeAtB)
            {
                dNode.ToggleVisibility();
            }
        }
    }

    private BaseNode FindClosestNode(Vector3 position, float maxDistance)
    {
        BaseNode[] allNodes = FindObjectsOfType<BaseNode>();
        BaseNode closest = null;
        float minDistance = maxDistance;

        foreach (var node in allNodes)
        {
            float dist = Vector3.Distance(position, node.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = node;
            }
        }
        return closest;
    }
    
    private BaseNode FindNextClockwiseNode(Transform pivotPoint, Transform rotatingPoint)
    {
        List<BaseNode> potentialNodes = new List<BaseNode>();
        BaseNode[] allNodes = FindObjectsOfType<BaseNode>();
        BaseNode pivotNode = FindClosestNode(pivotPoint.position, 0.1f);
        BaseNode rotatingNode = FindClosestNode(rotatingPoint.position, 0.1f);

        // Lọc các node tiềm năng
        foreach (var node in allNodes)
        {
            if (node == pivotNode) continue;
            if (node is BellNode) continue;
            if (node is DisappearingNode dNode && !dNode.isVisible) continue;
            
            float distanceToNode = Vector3.Distance(pivotPoint.position, node.transform.position);
            if (Mathf.Abs(distanceToNode - armLength) < 0.3f)
            {
                potentialNodes.Add(node);
            }
        }
        
        if (potentialNodes.Count == 0) return null;

        // Tìm node mới tốt nhất
        BaseNode bestNewTarget = null;
        float smallestAngle = 361f; 

        Vector3 pivotPos = pivotPoint.position;
        Vector3 rotatingPos = rotatingPoint.position;
        Vector3 forwardVector = (rotatingPos - pivotPos).normalized;

        foreach (var potentialNode in potentialNodes)
        {
            if (potentialNode == rotatingNode) continue;

            // ===== LOGIC MỚI: KIỂM TRA RÀNG BUỘC CỦA KIM GIÂY =====
            if (secondHand != null && secondHand.gameObject.activeSelf)
            {
                Quaternion rotationToApply = Quaternion.FromToRotation(rotatingPoint.position - pivotPoint.position, potentialNode.transform.position - pivotPoint.position);
                Vector3 futureSecondHandPos = rotationToApply * (secondHand.position - pivotPoint.position) + pivotPoint.position;

                BaseNode nodeNearSecondHand = FindClosestNode(futureSecondHandPos, 0.4f);
                if (nodeNearSecondHand != null && nodeNearSecondHand != potentialNode && nodeNearSecondHand != pivotNode)
                {
                    continue; // Bỏ qua nước đi này nếu kim giây sẽ va vào một node khác
                }
            }
            // =======================================================

            Vector3 directionToNode = (potentialNode.transform.position - pivotPos).normalized;
            float angle = Vector3.SignedAngle(forwardVector, directionToNode, Vector3.forward);

            if (angle > 0) angle = 360 - angle;
            else angle = -angle;

            if (angle < smallestAngle && angle > 0.01f)
            {
                smallestAngle = angle;
                bestNewTarget = potentialNode;
            }
        }
        
        if (bestNewTarget != null) return bestNewTarget;
        
        if (potentialNodes.Count == 1 && potentialNodes[0] == rotatingNode) return rotatingNode;
        
        return null;
    }
    
    private void EnsureSecondHandParent()
    {
        if (secondHand == null) return;

        // Chúng ta quy ước kim giây luôn gắn vào đầu B
        // Nếu cha hiện tại của nó không phải là pointB, đổi lại cho đúng
        if (secondHand.parent != pointB)
        {
            secondHand.SetParent(pointB, true); // true: giữ nguyên world position
        }
    }

    /// <summary>
    /// Hàm công khai để các script khác có thể hỏi xem kim giây có đang ở trên một node cụ thể không.
    /// </summary>
    public bool IsSecondHandOnNode(BaseNode targetNode)
    {
        // Nếu không có kim giây hoặc nó đang bị tắt, coi như không có.
        if (secondHand == null || !secondHand.gameObject.activeSelf)
        {
            return false;
        }

        // Dựa trên quy tắc của chúng ta (hàm EnsureSecondHandParent), 
        // kim giây luôn được gắn vào đầu kim tương ứng với 'nodeAtB'.
        // Vì vậy, chỉ cần kiểm tra xem node mục tiêu có phải là nodeAtB hay không.
        return nodeAtB == targetNode;
    }
    
    // ----- GIZMOS ĐỂ TRỰC QUAN HÓA -----
    
    // private void OnDrawGizmosSelected()
    // {
    //     // Lấy node mà kim đang xoay quanh để vẽ gizmo
    //     BaseNode gizmoPivot = FindClosestNode(pointA.position) ?? FindClosestNode(pointB.position);
    //
    //     if (gizmoPivot != null && Application.isEditor)
    //     {
    //         float currentArmLength = Vector3.Distance(pointA.position, pointB.position);
    //         DrawWireCircle(gizmoPivot.transform.position, currentArmLength, 32);
    //     }
    // }

    // private void DrawWireCircle(Vector3 center, float radius, int segments)
    // {
    //     Gizmos.color = Color.cyan;
    //     float angle = 0f;
    //     Vector3 lastPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
    //
    //     for (int i = 1; i <= segments; i++)
    //     {
    //         angle = i * (360f / segments) * Mathf.Deg2Rad;
    //         Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
    //         Gizmos.DrawLine(lastPoint, nextPoint);
    //         lastPoint = nextPoint;
    //     }
    // }//
}