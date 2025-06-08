using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameLevels", menuName = "Puzzle Game/Game Levels Configuration")]
public class GameLevels : ScriptableObject
{
    public List<LevelData> allLevels;
}