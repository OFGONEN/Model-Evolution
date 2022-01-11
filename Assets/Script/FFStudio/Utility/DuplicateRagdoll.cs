/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace FFStudio
{
	public class DuplicateRagdoll : MonoBehaviour
	{
		#region Fields
		public GameObject baseRagdoll;
		public GameObject targetRagdoll;

		private Dictionary<string, Transform> targetObjects = new Dictionary<string, Transform>( 128 );

		public Rigidbody[] baseRigidbodies;
		public Collider[] baseColliders;
		public CharacterJoint[] baseJoints;
		#endregion

		#region Properties
		#endregion

		#region Unity API
		[Button()]
		public void Dublicate()
		{
			targetObjects.Clear();
			var target_ChildObjects = targetRagdoll.GetComponentsInChildren<Transform>();

			for( var i = 0; i < target_ChildObjects.Length; i++ )
			{
				targetObjects.Add( target_ChildObjects[ i ].name, target_ChildObjects[ i ] );
			}

			baseRigidbodies = baseRagdoll.GetComponentsInChildren<Rigidbody>();
			baseColliders = baseRagdoll.GetComponentsInChildren<Collider>();
			baseJoints = baseRagdoll.GetComponentsInChildren<CharacterJoint>();

			for( var i = 0; i < baseRigidbodies.Length; i++ )
			{
				var baseRigidbody = baseRigidbodies[ i ];
				var baseCollider = baseColliders[ i ];

				Transform targetObject;
				targetObjects.TryGetValue( baseRigidbody.transform.name, out targetObject );

				var rb = targetObject.gameObject.AddComponent<Rigidbody>();
				rb.mass = baseRigidbody.mass;
				rb.drag = baseRigidbody.drag;
				rb.angularDrag = baseRigidbody.angularDrag;

				var collider = targetObject.gameObject.AddComponent( baseCollider.GetType() );

				if( collider is BoxCollider )
				{
					var baseBoxCollider = baseCollider as BoxCollider;
					var boxCollider = collider as BoxCollider;
					boxCollider.center = baseBoxCollider.center;
					boxCollider.size = baseBoxCollider.size;
				}
				else if( collider is SphereCollider )
				{
					var baseSphereCollider = baseCollider as SphereCollider;
					var sphereCollider = collider as SphereCollider;

					sphereCollider.center = baseSphereCollider.center;
					sphereCollider.radius = baseSphereCollider.radius;
				}
				else if( collider is CapsuleCollider )
				{
					var baseCapsuleCollider = baseCollider as CapsuleCollider;
					var capsuleCollider = collider as CapsuleCollider;

					capsuleCollider.center = baseCapsuleCollider.center;
					capsuleCollider.radius = baseCapsuleCollider.radius;
					capsuleCollider.height = baseCapsuleCollider.height;
					capsuleCollider.direction = baseCapsuleCollider.direction;
				}
			}

			for( var i = 0; i < baseJoints.Length; i++ )
			{
				var baseJoint = baseJoints[ i ];

				Transform targetObject;
				targetObjects.TryGetValue( baseJoint.transform.name, out targetObject );

				CharacterJoint joint = targetObject.gameObject.AddComponent<CharacterJoint>();

				Transform connectedBody;
				targetObjects.TryGetValue( baseJoint.connectedBody.name, out connectedBody );

				joint.connectedBody = connectedBody.GetComponent<Rigidbody>();
				joint.anchor = baseJoint.anchor;
				joint.axis = baseJoint.axis;
				joint.autoConfigureConnectedAnchor = baseJoint.autoConfigureConnectedAnchor;
				joint.connectedAnchor = baseJoint.connectedAnchor;
				joint.swingAxis = baseJoint.swingAxis;
				joint.twistLimitSpring = baseJoint.twistLimitSpring;
				joint.lowTwistLimit = baseJoint.lowTwistLimit;
				joint.highTwistLimit = baseJoint.highTwistLimit;
				joint.swingLimitSpring = baseJoint.swingLimitSpring;
				joint.swing1Limit = baseJoint.swing1Limit;
				joint.swing2Limit = baseJoint.swing2Limit;
				joint.enableProjection = baseJoint.enableProjection;
				joint.projectionDistance = baseJoint.projectionDistance;
				joint.projectionAngle = baseJoint.projectionAngle;
				joint.breakForce = baseJoint.breakForce;
				joint.breakTorque = baseJoint.breakTorque;
				joint.enableCollision = baseJoint.enableCollision;
				joint.enablePreprocessing = baseJoint.enablePreprocessing;
				joint.massScale = baseJoint.massScale;
				joint.connectedMassScale = baseJoint.connectedMassScale;
			}
		}
		#endregion

		#region API
		#endregion

		#region Implementation
		#endregion

		#region Editor Only
#if UNITY_EDITOR
#endif
		#endregion
	}
}