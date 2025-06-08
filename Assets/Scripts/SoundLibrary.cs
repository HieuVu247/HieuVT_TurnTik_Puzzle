using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Puzzle Game/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [Header("Giao diện người dùng")]
    public AudioClip uiClick;
    public AudioClip uiWin;
    public AudioClip uiLose;

    [Header("Gameplay")]
    public AudioClip handRotateStart;
    public AudioClip handSnap;
    public AudioClip bellRing;

    [Header("Nhạc nền")]
    public AudioClip backgroundMusic;
}