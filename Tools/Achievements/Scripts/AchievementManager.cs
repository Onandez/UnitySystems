using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GameKit.Tools
{
    // Handle the state of the achievements
    /// unlocking/locking them, and saving them to data files
    [ExecuteAlways]
    public static class AchievementManager
    {
        public static List<Achievement> AchievementsList { get { return _achievements; } }

        private static List<Achievement> _achievements;
        private static Achievement _achievement = null;

        private const string _defaultFileName = "Achievements";
        private const string _saveFolderName = "PlayerAchievements/";
        private const string _saveFileExtension = ".achievements";

        private static string _saveFileName;
        private static string _listID;

        // Call this method to initialize the manager
        public static void LoadAchievementList()
        {
            _achievements = new List<Achievement>();

            // Achievement List scriptable object must be in a Resources folder inside your project, like so : Resources/Achievements/PUT_SCRIPTABLE_OBJECT_HERE
            AchievementList achievementList = (AchievementList)Resources.Load("Achievements/AchievementList");

            if (achievementList == null)
            {
                return;
            }

            // Store the ID for save purposes
            _listID = achievementList.AchievementsListID;

            foreach (Achievement achievement in achievementList.Achievements)
            {
                _achievements.Add(achievement.Copy());
            }
        }

        // Unlocks the specified achievement, if found
        public static void UnlockAchievement(string achievementID)
        {
            _achievement = AchievementManagerContains(achievementID);
            if (_achievement != null)
            {
                _achievement.UnlockAchievement();
            }
        }

        // Locks the specified achievement, if found
        public static void LockAchievement(string achievementID)
        {
            _achievement = AchievementManagerContains(achievementID);
            if (_achievement != null)
            {
                _achievement.LockAchievement();
            }
        }

        // Adds progress to the specified achievement, if found
        public static void AddProgress(string achievementID, int newProgress)
        {
            _achievement = AchievementManagerContains(achievementID);
            if (_achievement != null)
            {
                _achievement.AddProgress(newProgress);
            }
        }

        // Sets the progress of the specified achievement, if found to the specified progress.
        public static void SetProgress(string achievementID, int newProgress)
        {
            _achievement = AchievementManagerContains(achievementID);
            if (_achievement != null)
            {
                _achievement.SetProgress(newProgress);
            }
        }

        // Determines if the achievement manager contains an achievement of the specified ID. Returns it if found, otherwise returns null
        private static Achievement AchievementManagerContains(string searchedID)
        {
            if (_achievements.Count == 0)
            {
                return null;
            }
            foreach (Achievement achievement in _achievements)
            {
                if (achievement.AchievementID == searchedID)
                {
                    return achievement;
                }
            }
            return null;
        }

        // SAVE ------------------------------------------------------------------------------------------------------------------------------------

        // Removes saved data and resets all achievements from a list
        public static void ResetAchievements(string listID)
        {
            if (_achievements != null)
            {
                foreach (Achievement achievement in _achievements)
                {
                    achievement.ProgressCurrent = 0;
                    achievement.UnlockedStatus = false;
                }
            }

            DeterminePath(listID);
            SaveLoadManager.DeleteSave(_saveFileName + _saveFileExtension, _saveFolderName);
            Debug.LogFormat("Achievements Reset");
        }

        public static void ResetAllAchievements()
        {
            LoadAchievementList();
            ResetAchievements(_listID);
        }

        // Loads the saved achievements file and updates the array with its content.
        public static void LoadSavedAchievements()
        {
            DeterminePath();
            SerializedAchievementManager serializedAchievementManager = (SerializedAchievementManager)SaveLoadManager.Load(typeof(SerializedAchievementManager), _saveFileName + _saveFileExtension, _saveFolderName);
            ExtractSerializedMMAchievementManager(serializedAchievementManager);
        }

        // Saves the achievements current status to a file on disk
        public static void SaveAchievements()
        {
            DeterminePath();
            SerializedAchievementManager serializedAchievementManager = new SerializedAchievementManager();
            FillSerializedAchievementManager(serializedAchievementManager);
            SaveLoadManager.Save(serializedAchievementManager, _saveFileName + _saveFileExtension, _saveFolderName);
        }

        // Determines the path the achievements save file should be saved to.
        private static void DeterminePath(string specifiedFileName = "")
        {
            string tempFileName = (!string.IsNullOrEmpty(_listID)) ? _listID : _defaultFileName;
            if (!string.IsNullOrEmpty(specifiedFileName))
            {
                tempFileName = specifiedFileName;
            }

            _saveFileName = tempFileName;
        }

        // Serializes the contents of the achievements array to a serialized, ready to save object
        public static void FillSerializedAchievementManager(SerializedAchievementManager serializedAchievements)
        {
            serializedAchievements.Achievements = new SerializedAchievement[_achievements.Count];

            for (int i = 0; i < _achievements.Count(); i++)
            {
                SerializedAchievement newAchievement = new SerializedAchievement(_achievements[i].AchievementID, _achievements[i].UnlockedStatus, _achievements[i].ProgressCurrent);
                serializedAchievements.Achievements[i] = newAchievement;
            }
        }

        // Extracts the serialized achievements into our achievements array if the achievements ID match.
        public static void ExtractSerializedMMAchievementManager(SerializedAchievementManager serializedAchievements)
        {
            if (serializedAchievements == null)
            {
                return;
            }

            for (int i = 0; i < _achievements.Count(); i++)
            {
                for (int j = 0; j < serializedAchievements.Achievements.Length; j++)
                {
                    if (_achievements[i].AchievementID == serializedAchievements.Achievements[j].AchievementID)
                    {
                        _achievements[i].UnlockedStatus = serializedAchievements.Achievements[j].UnlockedStatus;
                        _achievements[i].ProgressCurrent = serializedAchievements.Achievements[j].ProgressCurrent;
                    }
                }
            }
        }
    }
}
