using UnityEngine;
using System;

public interface InteractState
{
    bool LockPlayerControl { get; }
    bool ShowInteractPrompt { get; }
    bool ShowPopupWindow { get; }
}

namespace InteractStates
{
    public struct None : InteractState
    {
        public bool LockPlayerControl => false;
        public bool ShowInteractPrompt => false;
        public bool ShowPopupWindow => false;
    }

    public struct InFrontOfNPC : InteractState
    {
        public bool LockPlayerControl => false;
        public bool ShowInteractPrompt => true;
        public bool ShowPopupWindow => false;
    }

    public struct TalkingWithNPC : InteractState
    {
        public bool LockPlayerControl => true;
        public bool ShowInteractPrompt => false;
        public bool ShowPopupWindow => true;
    }

    public struct InFrontOfMinigame : InteractState
    {
        public bool LockPlayerControl => false;
        public bool ShowInteractPrompt => true;
        public bool ShowPopupWindow => false;

        public MinigameTag minigameTag;
    }

    public struct PlayingMinigame : InteractState
    {
        public bool LockPlayerControl => true;
        public bool ShowInteractPrompt => false;
        public bool ShowPopupWindow => false;

        public MinigameTag minigameTag;
    }

    public struct InFrontOfEnemy : InteractState
    {
        public bool LockPlayerControl => false;
        public bool ShowInteractPrompt => false;
        public bool ShowPopupWindow => false;

        public Enemy enemy;

        public InFrontOfEnemy(Enemy enemy)
        {
            this.enemy = enemy;
        }
    }

    public struct ViewingQuest : InteractState
    {
        public bool LockPlayerControl => true;
        public bool ShowInteractPrompt => false;
        public bool ShowPopupWindow => true;
    }

    public struct ViewingInventory : InteractState
    {
        public bool LockPlayerControl => true;
        public bool ShowInteractPrompt => false;
        public bool ShowPopupWindow => true;
    }

    public struct Popup : InteractState
    {
        public bool LockPlayerControl => true;
        public bool ShowInteractPrompt => false;
        public bool ShowPopupWindow => true;

        public string title;
        public string content;
        public string acceptButton;
        public Action onAccept;
    }

    public struct EscMenu : InteractState
    {
        public bool LockPlayerControl => true;
        public bool ShowInteractPrompt => false;
        public bool ShowPopupWindow => false;
    }
}
