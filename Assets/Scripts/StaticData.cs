using UnityEngine;
using System;

[Serializable]
public enum TriggerEvents
{
    GameOver
}

[Serializable]
public enum Scenes
{
    MainMenu = 0,
    Level1 = 1,
    Level2 = 2,
    Level3 = 3
}

public enum BodyPart
{
    Head,
    Torso,
    Hand_L,
    Hand_R,
    Leg_R,
    Leg_L
}
public enum Character
{
    Kid,
    Daddy,
    Grandpa
}

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
    [Space(5)]
    public float jumpForce;
    [Range(1, 5)] public int maxJumpCount;
    [Space(5)]
    [Range(0, 100)] public float acceleration;
    [Range(0, 100)] public float deceleration;
    [Space(5)]
    public PlayerSprites[] sprites;
}

public static class GameEvents
{
    public static class Game
    {
        public static Action<bool> OnGamePause;
        public static Action OnLevelOver;
    }

    public static class Camera
    {

    }

    public static class Input
    {
        public static Action<Vector2> OnPlayerMove;
        public static Action<Vector2> OnPlayerLook;
        public static Action<bool> OnPlayerJump;
        public static Action<bool> OnPlayerSprint;
        public static Action<bool> OnObjectSelect;
        public static Action<bool> OnObjectReverse;
        public static Action OnUICancel;

        public static Action<bool> OnSetInputState;
    }
}
