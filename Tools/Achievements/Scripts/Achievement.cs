using System;
using UnityEngine;

namespace GameKit.Tools
{
    public enum AchievementTypes
    {
        Simple, Progress
    }

    [Serializable]
    public class Achievement
    {
        [Header("Identification")]
        public string AchievementID;                // Identifier for this achievement
        public AchievementTypes AchievementType;    // Achievement progress based or
        public bool HiddenAchievement;              // if true, the achievement won't be displayed in a list
        public bool UnlockedStatus;                 // if true, the achievement has been unlocked. Otherwise, it's still up for grabs

        [Header("Description")]
        public string Title;                        // Achievement's name/title                                   
        public string Description;                  // Achievement's description                                  
        public int Points;                          // Amount of points unlocking this achievement gets you

        [Header("Image and Sounds")]
        public Sprite LockedImage;                  // Image to display while this achievement is locked
        public Sprite UnlockedImage;                // Image to display when the achievement is unlocked
        public AudioClip UnlockedSound;             // Sound to play when the achievement is unlocked

        [Header("Progress")]
        public int ProgressTarget;                  // Amount of progress needed to unlock this achievement.
        public int ProgressCurrent;                 // Current amount of progress made on this achievement
        protected AchievementDisplayItem _achievementDisplayItem;

        // Unlocks the achievement, asks for a save of the current achievements, and triggers an AchievementUnlockedEvent for this achievement.
        // This will usually then be caught by the AchievementDisplay class.
        public virtual void UnlockAchievement()
        {
            // if the achievement has already been unlocked, we do nothing and exit
            if (UnlockedStatus)
            {
                return;
            }

            UnlockedStatus = true;

            GameEvent.Trigger("Save");
            AchievementUnlockedEvent.Trigger(this);
        }

        // Locks the achievement.
        public virtual void LockAchievement()
        {
            UnlockedStatus = false;
        }

        // Adds the specified value to the current progress.
        public virtual void AddProgress(int newProgress)
        {
            ProgressCurrent += newProgress;
            EvaluateProgress();
        }

        // Sets the progress to the value passed in parameter.
        public virtual void SetProgress(int newProgress)
        {
            ProgressCurrent = newProgress;
            EvaluateProgress();
        }

        // Evaluates the current progress of the achievement, and unlocks it if needed.
        protected virtual void EvaluateProgress()
        {
            if (ProgressCurrent >= ProgressTarget)
            {
                ProgressCurrent = ProgressTarget;
                UnlockAchievement();
            }
        }

        // Copies this achievement (useful when loading from a scriptable object list)
        public virtual Achievement Copy()
        {
            Achievement clone = new Achievement();
            // we use Json utility to store a copy of our achievement, not a reference
            clone = JsonUtility.FromJson<Achievement>(JsonUtility.ToJson(this));
            return clone;
        }
    }
}
