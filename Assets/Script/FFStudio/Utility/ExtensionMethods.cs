/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace FFStudio
{
	public static class ExtensionMethods
	{
		//Static Variables
		private static List< Transform > baseModelBones = new List< Transform >( 96 );
		private static List< Transform > targetModelBones = new List< Transform >( 96 );

		public static Vector2 ReturnV2FromUnSignedAngle( this float angle )
		{
			switch( ( int )angle )
			{
				case   0: return Vector2.up;
				case  90: return Vector2.right;
				case 180: return Vector2.down;
				case 270: return Vector2.left;
				
				default: return Vector2.zero;
			}
		}

		public static bool FindSameColor( this List<Color> colors, Color color )
		{
			bool hasColor = false;

			for( int i = 0; i < colors.Count; i++ )
				hasColor |= colors[ i ].CompareColor( color );

			return hasColor;
		}

		public static bool FindSameColor( this List<Color> colors, Color color, out int index )
		{
			bool hasColor = false;
			index = -1;

			for( int i = 0; i < colors.Count; i++ )
			{
				hasColor |= colors[ i ].CompareColor( color );

				if( hasColor && index == -1 )
					index = i;
			}

			return hasColor;
		}

		public static bool CompareColor( this Color colorOne, Color colorTwo )
		{
			bool sameColor = true;

			sameColor &= Mathf.Abs( colorOne.r - colorTwo.r ) <= 0.01f;
			sameColor &= Mathf.Abs( colorOne.g - colorTwo.g ) <= 0.01f;
			sameColor &= Mathf.Abs( colorOne.b - colorTwo.b ) <= 0.01f;
			sameColor &= Mathf.Abs( colorOne.a - colorTwo.a ) <= 0.01f;

			return sameColor;
		}

		public static T ReturnLastItem<T>( this List<T> list )
		{
			var lastIndex = list.Count - 1;

			T item = list[ lastIndex ];
			list.RemoveAt( lastIndex );

			return item;
		}

		public static Vector3 ConvertV3( this Vector2 v2 )
		{
			return new Vector3( v2.x, v2.y, 0 );
		}

		public static Vector3 RandomPointBetween( this Vector3 first, Vector3 second )
		{
			return first + Random.Range( 0, 1f ) * ( second - first );
		}

		public static void LookAtOverTime( this Transform baseTransform, Vector3 targetPosition, float speed )
		{
			var directionVector = targetPosition - baseTransform.position;
			var step = speed * Time.deltaTime;

			Vector3 newDirection = Vector3.RotateTowards( baseTransform.forward, directionVector, step, 0.0f );

			baseTransform.rotation = Quaternion.LookRotation( newDirection );
		}

		public static void LookAtAxis( this Transform baseTransform, Vector3 targetPosition, Vector3 axis )
		{
			var newDirection = targetPosition - baseTransform.position;
			var eulerAngles = baseTransform.eulerAngles;
			var newRotationEuler = Quaternion.LookRotation( newDirection ).eulerAngles;

			newRotationEuler.x = eulerAngles.x + ( newRotationEuler.x - eulerAngles.x ) * axis.x;
			newRotationEuler.y = eulerAngles.y + ( newRotationEuler.y - eulerAngles.y ) * axis.y;
			newRotationEuler.z = eulerAngles.z + ( newRotationEuler.z - eulerAngles.z ) * axis.z;

			baseTransform.rotation = Quaternion.Euler( newRotationEuler );
		}

		public static void LookAtAxis( this Transform baseTransform, Vector3 targetPosition, Vector3 axis, float directionCofactor )
		{
			var newDirection = targetPosition - baseTransform.position;
			var eulerAngles = baseTransform.eulerAngles;
			var newRotationEuler = Quaternion.LookRotation( newDirection * directionCofactor ).eulerAngles;

			newRotationEuler.x = eulerAngles.x + ( newRotationEuler.x - eulerAngles.x ) * axis.x;
			newRotationEuler.y = eulerAngles.y + ( newRotationEuler.y - eulerAngles.y ) * axis.y;
			newRotationEuler.z = eulerAngles.z + ( newRotationEuler.z - eulerAngles.z ) * axis.z;

			// baseTransform.rotation = Quaternion.LookRotation( newDirection );
			baseTransform.rotation = Quaternion.Euler( newRotationEuler );
		}

		public static void LookAtOverTimeAxis( this Transform baseTransform, Vector3 targetPosition, Vector3 axis, float speed )
		{

			var _directionVector = targetPosition - baseTransform.position;
			var step = speed * Time.deltaTime;

			Vector3 newDirection = Vector3.RotateTowards( baseTransform.forward, _directionVector, step, 0.0f );

			var eulerAngles = baseTransform.eulerAngles;

			var newRotationEuler = Quaternion.LookRotation( newDirection ).eulerAngles;

			newRotationEuler.x = eulerAngles.x + ( newRotationEuler.x - eulerAngles.x ) * axis.x;
			newRotationEuler.y = eulerAngles.y + ( newRotationEuler.y - eulerAngles.y ) * axis.y;
			newRotationEuler.z = eulerAngles.z + ( newRotationEuler.z - eulerAngles.z ) * axis.z;

			// baseTransform.rotation = Quaternion.LookRotation( newDirection );
			baseTransform.rotation = Quaternion.Euler( newRotationEuler );
		}

		public static void LookAtDirectionOverTime( this Transform baseTransform, Vector3 direction, float speed )
		{
			Vector3 newDirection = Vector3.RotateTowards( baseTransform.forward, direction, speed * Time.deltaTime, 0.0f );

			baseTransform.rotation = Quaternion.LookRotation( newDirection );
		}

		public static void EmptyMethod()
		{
			/* Intentionally empty, by definition. */
		}

		public static Vector2 Clamp( this Vector2 value, Vector2 min, Vector2 max )
		{
			value.x = Mathf.Clamp( value.x, min.x, max.x );
			value.y = Mathf.Clamp( value.y, min.y, max.y );
			return value;
		}

		public static Vector3 Clamp( this Vector3 value, Vector3 min, Vector3 max )
		{
			value.x = Mathf.Clamp( value.x, min.x, max.x );
			value.y = Mathf.Clamp( value.y, min.y, max.y );
			value.z = Mathf.Clamp( value.z, min.z, max.z );
			return value;
		}

		public static Vector3 ClampXY( this Vector3 value, Vector2 min, Vector2 max )
		{
			value.x = Mathf.Clamp( value.x, min.x, max.x );
			value.y = Mathf.Clamp( value.y, min.y, max.y );
			return value;
		}

		public static Vector3 ClampXZ( this Vector3 value, Vector2 min, Vector2 max )
		{
			value.x = Mathf.Clamp( value.x, min.x, max.x );
			value.z = Mathf.Clamp( value.z, min.y, max.y );
			return value;
		}

		public static Vector3 ClampYZ( this Vector3 value, Vector2 min, Vector2 max )
		{
			value.y = Mathf.Clamp( value.y, min.x, max.x );
			value.z = Mathf.Clamp( value.z, min.y, max.y );
			return value;
		}

		public static Vector3 SetX( this Vector3 theVector, float newX )
		{
			theVector.x = newX;
			return theVector;
		}

		public static Vector3 SetY( this Vector3 theVector, float newY )
		{
			theVector.y = newY;
			return theVector;
		}

		public static Vector3 SetZ( this Vector3 theVector, float newZ )
		{
			theVector.z = newZ;
			return theVector;
		}

		public static float ComponentSum( this Vector3 theVector )
		{
			return theVector.x + theVector.y + theVector.z;
		}

		public static TransformData GetTransformData( this Transform transform ) // Global values
		{
			TransformData data;
			data.position = transform.position;
			data.rotation = transform.eulerAngles;
			data.scale    = transform.localScale;

			return data;
		}

		public static void SetTransformData( this Transform transform, TransformData data ) // Global values
		{
			transform.position    = data.position;
			transform.eulerAngles = data.rotation;
			transform.localScale  = data.scale;
		}

		// Takes root boes as parameters that are child of a humanoid model 
		public static void ReplaceHumanoidModel( this Transform baseBone, Transform targetBone )
		{
			baseModelBones.Clear();
			targetModelBones.Clear();

			baseBone.GetComponentsInChildren< Transform >( true, baseModelBones );
			targetBone.GetComponentsInChildren< Transform >( true, targetModelBones );

			targetBone.parent.position = baseBone.parent.position;

			for( var bone = 0; bone < baseModelBones.Count; bone++ )
			{
				targetModelBones[ bone ].position = baseModelBones[ bone ].position;
				targetModelBones[ bone ].rotation = baseModelBones[ bone ].rotation;
			}
		}

        public static void UpdateSkinnedMeshRenderer(this GameObject gameObject, SkinnedMeshRenderer currentRender, SkinnedMeshRenderer newRenderer)
        {
            currentRender.sharedMesh = newRenderer.sharedMesh;
            currentRender.sharedMaterials = newRenderer.sharedMaterials;
            currentRender.localBounds = newRenderer.localBounds;

            baseModelBones.Clear();
            targetModelBones.Clear();

            gameObject.GetComponentsInChildren<Transform>(true, baseModelBones);

            for (int boneOrder = 0; boneOrder < newRenderer.bones.Length; boneOrder++)
            {
                targetModelBones.Add(baseModelBones.Find(c => c.name == newRenderer.bones[boneOrder].name));
            }

            currentRender.bones = targetModelBones.ToArray();
        }

		public static void SetFieldValue( this object source, string fieldName, string value )
		{
				var fieldInfo = source.GetType().GetField( fieldName );

				if( fieldInfo == null )
					return;

				var fieldType = fieldInfo.FieldType;

                if( fieldType == typeof( int ) )
                {
				    fieldInfo.SetValue( source, int.Parse( value ) );
                }
                else if( fieldType == typeof( float ) )
                {
				    fieldInfo.SetValue( source, float.Parse( value, CultureInfo.InvariantCulture ) );
                }
                else if( fieldType == typeof( string ) )
                {
				    fieldInfo.SetValue( source, value );
                }
		}

		public static DG.Tweening.Sequence KillProper( this DG.Tweening.Sequence sequence )
		{
			if( sequence != null )
			{
				sequence.Kill();
				sequence = null;
			}

			return sequence;
		}

		public static DG.Tweening.Tween KillProper( this DG.Tweening.Tween tween )
		{
			if( tween != null )
			{
				tween.Kill();
				tween = null;
			}

			return tween;
		}

		public static Color SetAlpha( this Color color, float alpha )
		{
			Color newColor = color;
			newColor.a = alpha;

			return newColor;
		}

		public static void SetAlpha( this Image image, float alpha )
		{
			Color newColor = image.color;
			newColor.a = alpha;

			image.color = newColor;
		}

		public static float RoundTo( this float number, float step )
		{
			int quotient = Mathf.FloorToInt( number / step );
			var reminder = number % step;
			float rounded = quotient * step;

			if( reminder >= step / 2f )
				rounded += step;

			return rounded;
		}
	}
}

