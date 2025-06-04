using UnityEngine;
using System.Collections;

public class PivotController : MonoBehaviour
{
    [Header("Thiết lập xoay")]
    [Tooltip("Tốc độ xoay của thanh, tính bằng độ mỗi giây.")]
    [SerializeField] private float rotationSpeed = 90f;

    [Header("Liên kết đối tượng")]
    [Tooltip("Kéo GameObject cha (Arm) chứa các pivot vào đây.")]
    [SerializeField] private Transform armToRotate;

    [Tooltip("Kéo Pivot còn lại trên cùng thanh Arm vào đây.")]
    [SerializeField] private PivotController partnerPivot;

    private Rigidbody2D armRigidbody;
    private Coroutine currentRotationCoroutine;
    
    // Biến để kiểm soát trạng thái (sẽ dùng nhiều hơn ở bước sau)
    private bool isRotating = false;
    private bool isActive = true;

    
    /// <summary>
    /// Được gọi khi người dùng nhấn chuột lên Collider của GameObject này.
    /// </summary>
    void Awake()
    {
        armRigidbody = armToRotate.GetComponent<Rigidbody2D>();
    }
    private void OnMouseDown()
    {
        if (isActive && !isRotating)
        {
            Debug.Log("Pivot " + gameObject.name + " được nhấn. Bắt đầu xoay!");
            
            partnerPivot.GetComponent<Collider2D>().isTrigger = true;

            // Chuyển đổi vị trí của pivot này (là tâm xoay mong muốn) sang không gian cục bộ của Arm
            Vector2 centerInLocalSpace = armToRotate.InverseTransformPoint(transform.position);
            // Gán vị trí đó làm tâm khối lượng mới cho Rigidbody
            armRigidbody.centerOfMass = centerInLocalSpace;

            currentRotationCoroutine = StartCoroutine(RotateArm());
        }
    }

    /// <summary>
    /// Coroutine xử lý việc xoay mượt mà theo thời gian.
    /// </summary>
    private IEnumerator RotateArm()
    {
        isRotating = true;

        // Trong Giai đoạn 1, chúng ta sẽ cho nó xoay vô hạn.
        // Sau này, chúng ta sẽ thêm điều kiện dừng (khi va chạm).
        while (true) 
        {
            // Tính toán góc cần xoay trong frame này
            float angleToRotate = rotationSpeed * Time.deltaTime;
            // Ra lệnh cho Rigidbody xoay đi một góc. Dấu trừ để xoay theo chiều kim đồng hồ.
            armRigidbody.MoveRotation(armRigidbody.rotation - angleToRotate);

            yield return null;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là một Static Pivot không
        if (other.CompareTag("StaticPivot"))
        {
            Debug.Log(gameObject.name + " đã va chạm với " + other.name);

            // Dừng vòng quay
            StopCoroutine(currentRotationCoroutine);
            isRotating = false;

            // Vô hiệu hóa trigger của chính mình để tránh va chạm không mong muốn
            GetComponent<Collider2D>().isTrigger = false;

            // Tính toán vector từ tâm xoay (pivot này) đến tâm của Arm
            Vector3 armCenterOffset = armRigidbody.transform.position - transform.position;
            // Đặt vị trí mới cho Arm bằng cách lấy vị trí của pivot tĩnh cộng với offset đã tính
            Vector3 newArmPosition = other.transform.position + armCenterOffset;
            armRigidbody.MovePosition(newArmPosition);//
        }
    }
}