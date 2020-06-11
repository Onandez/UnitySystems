using UnityEngine;
using UnityEngine.UI;

namespace GameKit.Tools
{
    // Used to display an achievement. Add it to a prefab containing all the required elements listed below.
    [AddComponentMenu("Tools/Achievements/AchievementDisplayItem")]
    public class AchievementDisplayItem : MonoBehaviour
    {
        public Image BackgroundLocked;
        public Image BackgroundUnlocked;
        public Image Icon;
        public Text Title;
        public Text Description;
        public ProgressBar ProgressBarDisplay;
    } 
}
