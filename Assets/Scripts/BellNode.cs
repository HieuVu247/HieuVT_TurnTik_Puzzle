using DG.Tweening;
using UnityEngine;

public class BellNode : BaseNode
{
    [Header("Trạng thái & Giao diện")]
    public bool isBellActive { get; private set; } = false;

    // Kéo thả NodeStyle dành cho chuông vào đây
    public NodeStyle style;

    private SpriteRenderer nodeSprite;

    private void Awake()
    {
        nodeSprite = GetComponent<SpriteRenderer>();
        if (nodeSprite == null) Debug.LogError("BellNode thiếu SpriteRenderer!", this.gameObject);

        UpdateSprite();
    }

    public void ActivateBell()
    {
        isBellActive = true;
        UpdateSprite();
        // Chơi âm thanh và hiệu ứng
        AudioManager.instance?.PlaySFX(AudioManager.instance.soundLibrary.bellRing);
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.4f, 10, 1);
    }

    public void DeactivateBell()
    {
        isBellActive = false;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (nodeSprite == null || style == null) return;
        nodeSprite.sprite = isBellActive ? style.activeSprite : style.inactiveSprite;
    }

    // Cập nhật Gizmo để vẫn hoạt động
    protected override void OnDrawGizmos()
    {
        gizmoColor = isBellActive ? Color.yellow : new Color(0.5f, 0.5f, 0.1f);
        base.OnDrawGizmos();
    }
}