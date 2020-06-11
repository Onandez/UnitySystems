using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameKit.Tools
{
    // A serializable class used to store an achievement into a save file
    [Serializable]
    public class SerializedAchievement
    {
        public string AchievementID;
        public bool UnlockedStatus;
        public int ProgressCurrent;

        // Initializes a new instance 
        public SerializedAchievement(string achievementID, bool unlockedStatus, int progressCurrent)
        {
            AchievementID = achievementID;
            UnlockedStatus = unlockedStatus;
            ProgressCurrent = progressCurrent;
        }
    }

    [Serializable]
    // Serializable achievement manager.
    public class SerializedAchievementManager
    {
        public SerializedAchievement[] Achievements;
    }
}
