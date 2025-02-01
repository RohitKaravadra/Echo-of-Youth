using UnityEngine;
using System;

[Serializable]
public enum GameState
{
    None,
    Start,
    Play,
    Pause,
    Over
}

[Serializable]
public enum Scenes
{
    MainMenu = 0,
    Level = 1
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
    public Sprite sprite;
}

[Serializable]
public struct PlayerStats
{
    public string name;
    public float speed;
    [Space(5)]
    public float jumpForce;
    [Range(1, 5)] public int maxJumpCount;
    [Space(5)]
    [Range(0, 100)] public float acceleration;
    [Range(0, 100)] public float deceleration;
    [Space(5)]
    public PlayerSprites[] sprite;
}

public static class GameEvents
{
    public static class Game
    {
        public static Action<GameState> OnGameStateChanged;
    }

    public static class UI
    {
        public static Action OnAirControlChanged;
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
    }
}
