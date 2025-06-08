using UnityEngine;

public class DisappearingNode : BaseNode
{
    [Header("Trạng thái & Giao diện")]
    public bool isVisible { get; private set; } = true;
    public NodeStyle style;

    private SpriteRenderer nodeSprite;
    private bool isConnected = false;

    private void Awake()
    {
        nodeSprite = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    public override void SetConnected(bool connected)
    {
        isConnected = connected;
        UpdateSprite();
    }

    public void ToggleVisibility()
    {
        isVisible = !isVisible;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (nodeSprite == null || style == null) return;

        if (isConnected)
        {
            nodeSprite.sprite = style.connectedSprite;
        }
        else
        {
            nodeSprite.sprite = isVisible ? style.activeSprite : style.inactiveSprite;
        }
    }

    // Viết đè lên hàm Gizmo để nó trông khác đi khi "ẩn"
    protected override void OnDrawGizmos()
    {
        // Khi isVisible là true, dùng màu gốc.
        // Khi isVisible là false, tạo một màu mới có độ trong suốt (alpha) là 30%.
        Color originalColor = gizmoColor; // Lưu lại màu gốc
        gizmoColor = isVisible ? originalColor : new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);

        // Gọi hàm vẽ vòng tròn 2D từ lớp cha
        base.OnDrawGizmos();
        
        // Khôi phục lại màu gốc để không ảnh hưởng đến các Gizmo khác
        gizmoColor = originalColor;
    }
}