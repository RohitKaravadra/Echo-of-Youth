using UnityEngine;
using System;
using JetBrains.Annotations;

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

[Serializable]
public struct PlayerStats
{
    public float speed;
    [Space(5)]
    public float jumpForce;
    [Range(1, 5)] public int maxJumpCount;
    [Space(5)]
    [Range(0, 100)] public float acceleration;
    [Range(0, 100)] public float deceleration;
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
        public static Action OnUICancel;
    }
}
