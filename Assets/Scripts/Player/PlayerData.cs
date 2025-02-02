using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] PlayerStats[] _PlayerLevelData;

    public PlayerStats Get(Character _type)
    {
        foreach (var player in _PlayerLevelData)
            if (player.type == _type)
                return player;
        return _PlayerLevelData[0];
    }

    public int TotalData => _PlayerLevelData.Length;
}
