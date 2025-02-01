using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    [SerializeField] PlayerStats[] _PlayerLevelData;

    public PlayerStats GetPlayerData(int level)
    {

    }
}
