using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Tools
{
    public class AchievementRules : MonoBehaviour,EventListener<GameEvent>
    {
        // Loads the achievement list and the saved file
        protected virtual void Awake()
        {
            // Load the list of achievements, stored in a ScriptableObject in our Resources folder.
            AchievementManager.LoadAchievementList();
            // Load our saved file, to update that list with the saved values.
            AchievementManager.LoadSavedAchievements();
        }

		// Start listening for GameEvents
		protected virtual void OnEnable()
        {
            this.EventStartListening<GameEvent>();
        }

        // Stop listening for GameEvents
        protected virtual void OnDisable()
        {
            this.EventStopListening<GameEvent>();
        }

		/// Catch an MMGameEvent, we do stuff based on its name
		public virtual void OnEvent(GameEvent gameEvent)
        {
            switch (gameEvent.EventName)
            {
                case "Save":
                    AchievementManager.SaveAchievements();
                    break;
                    /*
                    // These are just examples of how you could catch a GameStart MMGameEvent and trigger the potential unlock of a corresponding achievement 
                    case "GameStart":
                        MMAchievementManager.UnlockAchievement("theFirestarter");
                        break;
                    case "LifeLost":
                        MMAchievementManager.UnlockAchievement("theEndOfEverything");
                        break;
                    case "Pause":
                        MMAchievementManager.UnlockAchievement("timeStop");
                        break;
                    case "Jump":
                        MMAchievementManager.UnlockAchievement ("aSmallStepForMan");
                        MMAchievementManager.AddProgress ("toInfinityAndBeyond", 1);
                        break;*/
            }
        }
    }
}
