using UnityEngine;

public class NormalNode : BaseNode
{
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

    private void UpdateSprite()
    {
        if (nodeSprite == null || style == null) return;
        nodeSprite.sprite = isConnected ? style.connectedSprite : style.activeSprite;
    }
}