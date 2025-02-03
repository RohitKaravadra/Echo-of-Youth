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
