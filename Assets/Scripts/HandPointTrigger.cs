using UnityEngine;

public class HandPointTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        BellNode bell = other.GetComponent<BellNode>();
        if (bell != null)
        {
            if (bell.isBellActive)
            {
                bell.DeactivateBell();
                Debug.Log("Đã tắt chuông: " + other.name);
            }
            else
            {
                bell.ActivateBell();
                Debug.Log("Đã bật chuông: " + other.name);
            }
        }
    }
}