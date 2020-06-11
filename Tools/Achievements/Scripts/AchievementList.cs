using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

namespace GameKit.Tools
{
    public class AchievementList : ScriptableObject
    {
		public string AchievementsListID = "AchievementsList";  // ID of this achievement list. This is used to save/load data.
        public List<Achievement> Achievements;                  // List of achievements 

        // Asks for a reset of all the achievements in this list (they'll all be locked again, their progress lost).
        public virtual void ResetAchievements()
        {
            Debug.LogFormat("Reset Achievements");
            AchievementManager.ResetAchievements(AchievementsListID);
        }
    }
}
