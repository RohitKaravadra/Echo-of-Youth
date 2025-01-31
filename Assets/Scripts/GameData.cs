using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    [SerializeField] PlayerStats[] _PlayerLevelData;
    [Space(10)]
    [SerializeField] float _MaxReverseHistory;
    [SerializeField] float _MinRecordDistance;
}
