using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Tools
{
    // Time helpers
    public class TimeHelper : MonoBehaviour
    {
        // Turns a float (expressed in seconds) into a string displaying hours, minutes, seconds and hundredths optionnally
        public static string FloatToTimeString(float t, bool displayHours = false, bool displayMinutes = true, bool displaySeconds = true, bool displayMilliseconds = false)
        {
            float hours = Mathf.Floor(t / 3600);
            float minutes = Mathf.Floor(t / 60);
            float seconds = (t % 60);
            float milliseconds = Mathf.Floor((t * 1000) % 1000);

            if (displayHours && displayMinutes && displaySeconds && displayMilliseconds)
            {
                return string.Format("{0:00}:{1:00}:{2:00}.{3:000}", hours, minutes, seconds, milliseconds);
            }
            if (!displayHours && displayMinutes && displaySeconds && displayMilliseconds)
            {
                return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
            }
            if (!displayHours && !displayMinutes && displaySeconds && displayMilliseconds)
            {
                return string.Format("{0:00}.{2:00}", seconds, milliseconds);
            }
            if (!displayHours && !displayMinutes && displaySeconds && !displayMilliseconds)
            {
                return string.Format("{0:00}", seconds);
            }
            if (displayHours && displayMinutes && displaySeconds && !displayMilliseconds)
            {
                return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            }
            if (!displayHours && displayMinutes && displaySeconds && !displayMilliseconds)
            {
                return string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            return null;

        }

        // Takes a hh:mm:ss:SSS string and turns it into a float value expressed in seconds
        public static float TimeStringToFloat(string timeInStringNotation)
        {
            if (timeInStringNotation.Length != 12)
            {
                throw new Exception("The time in the TimeStringToFloat method must be specified using a hh:mm:ss:SSS syntax");
            }

            string[] timeStringArray = timeInStringNotation.Split(new string[] { ":" }, StringSplitOptions.None);

            float startTime = 0f;
            float result;
            if (float.TryParse(timeStringArray[0], out result))
            {
                startTime += result * 3600f;
            }
            if (float.TryParse(timeStringArray[1], out result))
            {
                startTime += result * 60f;
            }
            if (float.TryParse(timeStringArray[2], out result))
            {
                startTime += result;
            }
            if (float.TryParse(timeStringArray[3], out result))
            {
                startTime += result / 1000f;
            }

            return startTime;
        }
    }
}

