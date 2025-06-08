using UnityEngine;

[CreateAssetMenu(fileName = "NewNodeStyle", menuName = "Puzzle Game/Node Style")]
public class NodeStyle : ScriptableObject
{
    // Sprite cho trạng thái được kết nối với kim
    public Sprite connectedSprite;
    // Sprite cho trạng thái hoạt động (ví dụ: DisappearingNode đang hiện)
    public Sprite activeSprite;
    // Sprite cho trạng thái không hoạt động (ví dụ: DisappearingNode đang ẩn)
    public Sprite inactiveSprite;
}