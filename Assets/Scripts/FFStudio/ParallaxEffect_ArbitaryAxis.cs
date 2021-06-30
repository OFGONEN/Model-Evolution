/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class ParallaxEffect_ArbitaryAxis : MonoBehaviour
	{
#region Fields
		public SharedReferenceProperty targetReference;
		public Vector3 parallaxRatio_X_Axis;
		public Vector3 parallaxRatio_Y_Axis;
		public Vector3 parallaxRatio_Z_Axis;

		/* Private Fields */
		private Transform targetTransform;
		private Vector3 startPosition;
		private Vector3 target_StartPosition;
#endregion

#region Unity API
		private void Start()
		{
			targetTransform = ( targetReference.sharedValue as Rigidbody ).transform;

			startPosition = transform.position;
			target_StartPosition = targetTransform.position;
		}

		private void Update()
		{
			var diff = targetTransform.position - target_StartPosition;

			var final = startPosition;

			final.x += Vector3.Scale( diff, parallaxRatio_X_Axis ).ComponentSum();
			final.y += Vector3.Scale( diff, parallaxRatio_Y_Axis ).ComponentSum();
			final.z += Vector3.Scale( diff, parallaxRatio_Z_Axis ).ComponentSum();

			transform.position = final;
		}
#endregion

#region API
#endregion

#region Implementation
#endregion
	}
}