using System;
using UnityEngine;

public enum BodyPart { Head, Torso, Hand_L, Hand_R, Leg_R, Leg_L }
public enum Character { Kid, Daddy, Grandpa }

[Serializable]
public struct PlayerSprites
{
    public BodyPart part;
    public Sprite sprite;
}

[Serializable]
public struct PlayerSpriteRenderers
{
    public BodyPart part;
    public SpriteRenderer renderer;
}

[Serializable]
public struct PlayerStats
{
    public Character type;
    [Range(0, 1)] public float scale;
    public float speed;
    [Range(0, 1)] public float airControl;
    [Space(5)]
    public float jumpForce;
    [Range(1, 5)] public int maxJumpCount;
    [Space(5)]
    [Range(0, 100)] public float acceleration;
    [Range(0, 100)] public float deceleration;
    [Space(5)]
    public PlayerSprites[] sprites;
}


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
