using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Definition;

namespace Runtime.Extensions
{
    public static class GameExtensions
    {
        #region Class Methods

        /// <summary>
        /// This function converts a vector2 direction to an index based on the slice count.
        /// This goes in a counter-clockwise direction.
        /// </summary>
        public static int ToIsometricDirectionIndex(this Vector2 direction, uint sliceCount)
        {
            Vector2 normalDirection = direction.normalized;
            float step = Constant.CIRCLE_DEGREES / sliceCount;
            float halfstep = step / 2.0f;
            float angle = Vector2.SignedAngle(Vector2.up, normalDirection);
            angle += halfstep;
            if (angle < 0.0f)
                angle += Constant.CIRCLE_DEGREES;
            float stepCount = angle / step;
            return Mathf.FloorToInt(stepCount);
        }

        /// <summary>
        /// This function converts an isometric direction index to a direction based on the slice count.
        /// </summary>
        public static Vector2 ToDirection(this int isometricDirectionIndex, uint sliceCount)
        {
            float step = Constant.CIRCLE_DEGREES / sliceCount;
            float radians = (isometricDirectionIndex * step + Constant.COORDINATE_AXES_OFFSET_DEGREES) * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        }

        public static Vector2 RotateByDeflectionAngle(this Vector2 direction, float deflectionAngle)
        {
            float angleInDegrees = Vector2.SignedAngle(Vector2.up, direction);
            float newAngleInDegrees = angleInDegrees + deflectionAngle;
            float newAngleInRadians = (newAngleInDegrees + Constant.COORDINATE_AXES_OFFSET_DEGREES) * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(newAngleInRadians), Mathf.Sin(newAngleInRadians));
        }

        public static Vector2 ToDirection(this float angle)
        {
            float newAngleInRadians = (angle + Constant.COORDINATE_AXES_OFFSET_DEGREES) * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(newAngleInRadians), Mathf.Sin(newAngleInRadians));
        }

        public static Vector3 ChangeDirection(this Vector3 origin, Vector3 direction)
        {
            float rotationInRadians = Vector3.Angle(direction, Vector3.forward) * Mathf.Deg2Rad;

            if (Vector3.Dot(Vector3.right, direction) >= 0)
            {
                float offsetX = origin.z * Mathf.Sin(rotationInRadians) + origin.x * Mathf.Cos((rotationInRadians));
                float offsetZ = origin.z * Mathf.Cos(rotationInRadians) - origin.x * Mathf.Sin((rotationInRadians));
                float offsetY = origin.y;
                return new Vector3(offsetX, offsetY, offsetZ);
            }
            else
            {
                float offsetX = -origin.z * Mathf.Sin(rotationInRadians) + origin.x * Mathf.Cos((rotationInRadians));
                float offsetZ = origin.z * Mathf.Cos(rotationInRadians) + origin.x * Mathf.Sin((rotationInRadians));
                float offsetY = origin.y;
                return new Vector3(offsetX, offsetY, offsetZ);
            }
        }

        public static Quaternion ToQuaternion(this Vector2 direction, float offsetDegree = Constant.COORDINATE_AXES_OFFSET_DEGREES)
            => Quaternion.Euler(Vector3.forward * (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - offsetDegree));

        public static Quaternion ToQuaternion(this Vector3 direction, float offsetDegree = Constant.COORDINATE_AXES_OFFSET_DEGREES)
            => Quaternion.Euler(Vector3.forward * (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - offsetDegree));

        public static string ToColorString(this string textInfo, Color color)
        {
            string colorString = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{textInfo}</color>";
            return colorString;
        }

        public static string ToTrackingFormat(this Enum enumType)
        {
            var inputString = enumType.ToString();
            return ToTrackingFormat(inputString);
        }

        public static string ToTrackingFormat(this string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return inputString;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(char.ToLower(inputString[0]));

            for (int i = 1; i < inputString.Length; i++)
            {
                char character = inputString[i];
                if (char.IsUpper(character) || (char.IsDigit(character) && i > 1 && !char.IsDigit(inputString[i - 1])))
                    stringBuilder.Append('_');
                stringBuilder.Append(char.ToLower(character));
            }

            return stringBuilder.ToString();
        }

        public static string ToIronSourcePlacementFormat(this Enum enumType)
        {
            var inputString = enumType.ToString();
            return ToIronSourcePlacementFormat(inputString);
        }

        public static string ToIronSourcePlacementFormat(this string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return inputString;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(char.ToUpper(inputString[0]));

            for (int i = 1; i < inputString.Length; i++)
            {
                char character = inputString[i];
                if (char.IsUpper(character) || (char.IsDigit(character) && i > 1 && !char.IsDigit(inputString[i - 1])))
                {
                    stringBuilder.Append('_');
                    stringBuilder.Append(character);
                }
                else stringBuilder.Append(char.ToLower(character));
            }

            return stringBuilder.ToString();
        }

        public static string ToLowercaseFormat(this Enum enumType)
        {
            var inputString = enumType.ToString();
            return ToLowercaseFormat(inputString);
        }

        public static string ToLowercaseFormat(this string inputString)
        {
            var outputString = inputString.ToLower();
            return outputString;
        }

        public static string ToUppercaseFormat(this Enum enumType)
        {
            var inputString = enumType.ToString();
            return ToUppercaseFormat(inputString);
        }

        public static string ToUppercaseFormat(this string inputString)
        {
            var outputString = inputString.ToUpper();
            return outputString;
        }

        public static void Replace<TKey, TValue>(this Dictionary<TKey, TValue> original, TKey key, TValue value)
        {
            if (original.ContainsKey(key))
                original.Remove(key);
            original.Add(key, value);
        }

        private static string DecToHex(int value)
        {
            return value.ToString("X2");
        }

        private static string FloatNormalizedToHext(float value)
        {
            return DecToHex(Mathf.RoundToInt(value * 255f));
        }

        public static string GetStringFromColor(Color c)
        {
            string red = FloatNormalizedToHext(c.r);
            string green = FloatNormalizedToHext(c.g);
            string blue = FloatNormalizedToHext(c.b);
            return "#" + red + green + blue;
        }

        public static int ChooseRandomItem(float[] itemRatios)
        {
            float randomValue = (float)new System.Random().NextDouble();

            float totalRatio = 0f;
            foreach (float ratio in itemRatios)
            {
                totalRatio += ratio;
            }

            float cumulativeRatio = 0f;
            for (int i = 0; i < itemRatios.Length; i++)
            {
                cumulativeRatio += itemRatios[i] / totalRatio;
                if (randomValue < cumulativeRatio)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion Class Methods
    }
}