using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
    public static class ExtensionMethods
    {
        public static Vector2 ReturnV2FromUnSignedAngle(this float angle)
        {
            var _angle = (int)angle;
            switch (_angle)
            {
                case 0: return Vector2.up;
                case 90: return Vector2.right;
                case 180: return Vector2.down;
                case 270: return Vector2.left;
                default: return Vector2.zero;
            }
        }

        public static bool FindSameColor(this List<Color> colors, Color color)
        {
            bool _hasColor = false;

            for (int i = 0; i < colors.Count; i++)
            {
                _hasColor |= colors[i].CompareColor(color);
            }

            return _hasColor;
        }

        public static bool FindSameColor(this List<Color> colors, Color color, out int index)
        {
            bool _hasColor = false;
            index = -1;

            for (int i = 0; i < colors.Count; i++)
            {
                _hasColor |= colors[i].CompareColor(color);

                if (_hasColor && index == -1) index = i;
            }

            return _hasColor;
        }

        public static bool CompareColor(this Color colorOne, Color colorTwo)
        {
            bool _sameColor = true;


            _sameColor &= Mathf.Abs(colorOne.r - colorTwo.r) <= 0.01f;
            _sameColor &= Mathf.Abs(colorOne.g - colorTwo.g) <= 0.01f;
            _sameColor &= Mathf.Abs(colorOne.b - colorTwo.b) <= 0.01f;
            _sameColor &= Mathf.Abs(colorOne.a - colorTwo.a) <= 0.01f;

            return _sameColor;
        }
        public static T ReturnLastItem<T>(this List<T> list)
        {
            var _lastIndex = list.Count - 1;

            T _item = list[_lastIndex];
            list.RemoveAt(_lastIndex);

            return _item;
        }
        public static Vector3 ConvertV3(this Vector2 v2)
        {
            return new Vector3(v2.x, v2.y, 0);
        }
        public static Vector3 RandomPointBetween(this Vector3 first, Vector3 second)
        {
            return first + Random.Range(0, 1f) * (second - first);
        }
		public static void Push< T >( this RunTimeStack< T > set, T item )
		{
			set.stack.Push( item );
		}
		public static T Pop< T >( this RunTimeStack< T > set )
		{
			return set.stack.Pop();
		}
        public static void EmptyMethod()
        {

        }
    }
}

