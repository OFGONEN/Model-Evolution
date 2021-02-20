using UnityEngine;

namespace FFStudio
{
	public class CameraZoomCalculator : MonoBehaviour
	{
		public Vector2 minimumResolution = new Vector2( 750, 1334 ); // Reference: iPhone 6.
        public Vector2 maximumResolution = new Vector2( 828, 1792 ); // Reference: iPhone 11.

		public Vector3 Calculate( Vector3 positionForMinimumResolution, Vector3 positionForMaximumResolution )
		{
			var aspectRatio = ( float )Screen.height / Screen.width;

			var minAspectRatio = minimumResolution.y / minimumResolution.x;
			var maxAspectRatio = maximumResolution.y / maximumResolution.x;

			var lerpBy = ( aspectRatio - minAspectRatio ) / ( maxAspectRatio - minAspectRatio );

			return Vector3.Lerp( positionForMinimumResolution, positionForMaximumResolution, lerpBy );
		}
	}
}