using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonEffects : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Cài đặt hiệu ứng")]
    public float scaleFactor = 1.1f;      // Hệ số phóng to khi di chuột vào
    public float pressScaleFactor = 0.9f; // Hệ số thu nhỏ khi nhấn
    public float duration = 0.2f;         // Thời gian thực hiện hiệu ứng

    // Biến để lưu trữ scale ban đầu của đối tượng
    private Vector3 originalScale;

    private void Awake()
    {
        // Ngay khi đối tượng được tạo ra, ghi nhớ ngay scale gốc của nó
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Thay vì dùng số 1, giờ chúng ta dùng originalScale làm mốc
        transform.DOScale(originalScale * pressScaleFactor, duration / 2).SetEase(Ease.OutQuad).SetUpdate(true);
        
        // Chơi âm thanh click
        if (AudioManager.instance != null && AudioManager.instance.soundLibrary != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.soundLibrary.uiClick);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Quay về scale gốc
        transform.DOScale(originalScale, duration / 2).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Phóng to dựa trên scale gốc
        transform.DOScale(originalScale * scaleFactor, duration).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Quay về scale gốc
        transform.DOScale(originalScale, duration).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    // Đảm bảo kill tween khi đối tượng bị hủy để tránh lỗi
    private void OnDestroy()
    {
        transform.DOKill();
    }
}