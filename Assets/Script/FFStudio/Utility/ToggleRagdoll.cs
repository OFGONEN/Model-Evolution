/* Created by and for usage of FF Studios (2021). */

using System.Linq;
using UnityEngine;
using NaughtyAttributes;

namespace FFStudio
{
    public class ToggleRagdoll : MonoBehaviour
    {
#region Fields (Inspector Interface)
        [ SerializeField ] private bool includeRigidbodyOnThisGameObject;
        [ SerializeField ] private bool deactivateOnStart = true;
        [ SerializeField ] private Rigidbody[] excludeTheseRigidbodies;
#endregion
        
#region Fields (Private, Auto-acquired)
        [ SerializeField ] private Rigidbody[] ragdollRigidbodies;
#endregion

#region Properties
#endregion

#region Unity API
        private void Awake()
        {
            if( includeRigidbodyOnThisGameObject )
                ragdollRigidbodies = GetComponentsInChildren< Rigidbody >();
            else
            {
                var ragdollRigidbodies_Temporary = GetComponentsInChildren< Rigidbody >();
                ragdollRigidbodies = ragdollRigidbodies_Temporary.Skip( 1 ).Take( ragdollRigidbodies_Temporary.Length - 1 ).ToArray();
            }

            ragdollRigidbodies = ragdollRigidbodies.Except( excludeTheseRigidbodies ).ToArray();

            if( deactivateOnStart )
                Deactivate();
        }
#endregion

#region API
        [ Button() ]
        public void Activate()
        {
            foreach( var rb in ragdollRigidbodies )
                rb.isKinematic = false;
        }
        
        [ Button()]
        public void Deactivate()
        {
            foreach( var rb in ragdollRigidbodies )
                rb.isKinematic = true;
        }
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
    }
}