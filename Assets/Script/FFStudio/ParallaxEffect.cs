/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class ParallaxEffect : MonoBehaviour
	{
#region Fields
		public SharedReferenceNotifier targetReference;
		public Vector3 parallaxRatio;
		public float parallaxSpeed;

		/* Private Fields */
		private Transform targetTransform;
		private Vector3 startPosition;
		private Vector3 target_StartPosition;
#endregion

#region Unity API
		private void Start()
		{
			targetTransform = ( targetReference.SharedValue as Rigidbody ).transform;

			startPosition = transform.position;
			target_StartPosition = targetTransform.position;
		}

private void Update()
		{
			var diff = targetTransform.position - target_StartPosition;
			diff.Scale( parallaxRatio );

			transform.position = Vector3.MoveTowards( transform.position, startPosition + diff, Time.deltaTime * parallaxSpeed );
		}
#endregion

#region API
#endregion

#region Implementation
#endregion
	}
}