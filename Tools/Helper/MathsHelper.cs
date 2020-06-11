using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Tools
{
    // Math helpers
	public static class MathsHelper
    {
		// Turns Vector3 into a Vector2
		public static Vector2 Vector3ToVector2 (Vector3 target) 
		{
			return new Vector2(target.x, target.y);
		}

		// Turns Vector2 into a Vector3
		public static Vector3 Vector2ToVector3 (Vector2 target) 
		{
			return new Vector3(target.x, target.y, 0);
		}

        // Turns Vector2 into a Vector3
        public static Vector3 Vector2ToVector3 (Vector2 target, float zValue) 
		{
			return new Vector3(target.x, target.y, zValue);
		}

		// Rounds all components of a Vector3.
		public static Vector3 RoundVector3 (Vector3 vector)
		{
			return new Vector3 ( Mathf.Round (vector.x), Mathf.Round (vector.y), Mathf.Round (vector.z));
        }

        // Rounds all components of a Vector3.
        public static Vector3 RoundVector3(Vector3 vector, int decimals)
        {
            float f = Mathf.Pow(10, decimals);

            return new Vector3(
                (Mathf.Round(vector.x * f) / f),
                (Mathf.Round(vector.y * f) / f),
                (Mathf.Round(vector.z * f) / f));
        }

        // Random Vector2 in range of two defined vectors.
        public static Vector2 RandomVector2(Vector2 minimum, Vector2 maximum)
        {
            return new Vector2(Random.Range(minimum.x, maximum.x), Random.Range(minimum.y, maximum.y));
        }

        // Random Vector3 in range of two defined vectors.
        public static Vector3 RandomVector3(Vector3 minimum, Vector3 maximum)
        {
            return new Vector3(Random.Range(minimum.x, maximum.x), Random.Range(minimum.y, maximum.y),Random.Range(minimum.z, maximum.z));
        }

        // Rotates a point around the given pivot.
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle) 
		{			
			angle = angle*(Mathf.PI/180f);
			var rotatedX = Mathf.Cos(angle) * (point.x - pivot.x) - Mathf.Sin(angle) * (point.y-pivot.y) + pivot.x;
			var rotatedY = Mathf.Sin(angle) * (point.x - pivot.x) + Mathf.Cos(angle) * (point.y - pivot.y) + pivot.y;
			return new Vector3(rotatedX,rotatedY,0);		
		}

		// Rotates a point around the given pivot.
		public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle) 
		{
			// we get point direction from the point to the pivot
		   	Vector3 direction = point - pivot; 
		   	// we rotate the direction
		   	direction = Quaternion.Euler(angle) * direction; 
		   	// we determine the rotated point's position
		   	point = direction + pivot; 
		   	return point; 
		}

		// Rotates a point around the given pivot.
		public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion quaternion) 
		{
			// we get point direction from the point to the pivot
		   	Vector3 direction = point - pivot; 
		   	// we rotate the direction
		   	direction = quaternion * direction; 
		   	// we determine the rotated point's position
		   	point = direction + pivot; 
		   	return point; 
		 }

		// Rotates a vector2 by the angle (in degrees) specified and returns it
		public static Vector2 RotateVector2(Vector2 vector, float angle)
        {
			if (angle == 0)
			{
				return vector;
			}
			float sinus = Mathf.Sin(angle * Mathf.Deg2Rad);
			float cosinus = Mathf.Cos(angle * Mathf.Deg2Rad);

			float oldX = vector.x;
			float oldY = vector.y;
			vector.x = (cosinus * oldX) - (sinus * oldY);
			vector.y = (sinus * oldX) + (cosinus * oldY);
			return vector;
		}

		// Returns the angle between two vectors, on a 360° scale
		public static float AngleBetween(Vector2 vectorA, Vector2 vectorB)
		{
			float angle = Vector2.Angle(vectorA, vectorB);
			Vector3 cross = Vector3.Cross(vectorA, vectorB);

			if (cross.z > 0)
			{
				angle = 360 - angle;
			}

			return angle;
		}

		// Returns the distance between a point and a line.
		public static float DistanceBetweenPointAndLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
		{
			return Vector3.Magnitude(ProjectPointOnLine(point, lineStart, lineEnd) - point);
		}

		// Projects a point on a line (perpendicularly) and returns the projected point.
		public static Vector3 ProjectPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
		{
			Vector3 rhs = point - lineStart;
			Vector3 vector2 = lineEnd - lineStart;
			float magnitude = vector2.magnitude;
			Vector3 lhs = vector2;
			if (magnitude > 1E-06f)
			{
				lhs = (Vector3)(lhs / magnitude);
			}
			float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
			return (lineStart + ((Vector3)(lhs * num2)));
		}

		// Returns the sum of all the parameters
		public static int Sum(params int[] thingsToAdd)
		{
			int result=0;
			for (int i = 0; i < thingsToAdd.Length; i++)
			{
				result += thingsToAdd[i];
			}
			return result;
		}

		// Returns the result of rolling a dice of the specified number of sides
		public static int RollADice(int numberOfSides)
		{
			return (Random.Range(1,numberOfSides+1));
		}

		// Returns a random success based on X% of chance.
		public static bool Chance(int percent)
		{
			return (Random.Range(0,100) <= percent);
		}

		// Moves from "from" to "to" by the specified amount and returns the corresponding value
		public static float Approach(float from, float to, float amount)
		{
			if (from < to)
			{
				from += amount;
				if (from > to)
				{
					return to;
				}
			}
			else
			{
				from -= amount;
				if (from < to)
				{
					return to;
				}
			}
			return from;
		} 


		// Remaps a value x in interval [A,B], to the proportional value in interval [C,D]
		// x The value to remap
		// A the minimum bound of interval [A,B] that contains the x value
		// B the maximum bound of interval [A,B] that contains the x value
		// C the minimum bound of target interval [C,D]
		// D the maximum bound of target interval [C,D]
		public static float Remap(float x, float A, float B, float C, float D)
		{
			float remappedValue = C + (x-A)/(B-A) * (D - C);
			return remappedValue;
		}

        // Clamps the angle in parameters between a minimum and maximum angle (all angles expressed in degrees)
        public static float ClampAngle(float angle, float minimumAngle, float maximumAngle)
        {
            if (angle < -360)
            {
                angle += 360;
            }
            if (angle > 360)
            {
                angle -= 360;
            }
            return Mathf.Clamp(angle, minimumAngle, maximumAngle);
        }

        public static float RoundToDecimal(float value, int numberOfDecimals)
        {
            return Mathf.Round(value * 10f * numberOfDecimals) / (10f * numberOfDecimals);
        }

        // Rounds the value passed in parameters to the closest value in the array
        public static float RoundToClosest(float value, float[] possibleValues)
		{
			if (possibleValues.Length == 0) 
			{
				return 0f;
			}

			float closestValue = possibleValues[0];

			foreach (float possibleValue in possibleValues)
			{
				if (Mathf.Abs(closestValue - value) > Mathf.Abs(possibleValue - value))
				{
					closestValue = possibleValue;
				}	
			}
			return closestValue;

        }

        // Returns a vector3 based on the angle
        public static Vector3 DirectionFromAngle(float angle, float additionalAngle)
        {
            angle += additionalAngle;

            Vector3 direction = Vector3.zero;
            direction.x = Mathf.Sin(angle * Mathf.Deg2Rad);
            direction.y = 0f;
            direction.z = Mathf.Cos(angle * Mathf.Deg2Rad);
            return direction;
        }

        // Returns a vector3 based on the angle
        public static Vector3 DirectionFromAngle2D(float angle, float additionalAngle)
        {
            angle += additionalAngle;

            Vector3 direction = Vector3.zero;
            direction.x = Mathf.Sin(angle * Mathf.Deg2Rad);
            direction.y = Mathf.Cos(angle * Mathf.Deg2Rad);
            direction.z = 0f;
            return direction;
        }
    }
}
