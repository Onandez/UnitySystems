using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Tools
{
    // An event type used to broadcast the fact that an achievement has been unlocked
    public struct AchievementUnlockedEvent
    {
        // The achievement that has been unlocked
		public Achievement Achievement;

        // Constructor
        public AchievementUnlockedEvent(Achievement newAchievement)
        {
            Achievement = newAchievement;
        }

        static AchievementUnlockedEvent e;
        public static void Trigger(Achievement newAchievement)
        {
            e.Achievement = newAchievement;
            EventManager.TriggerEvent(e);
        }
    }
}
