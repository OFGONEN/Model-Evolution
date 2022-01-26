/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public class Fire_UnityEvent_Multiple : MonoBehaviour
	{
#region Fields
        public EventPair[] eventPairs;
#endregion

#region Properties
#endregion

#region Unity API
        private void OnEnable()
        {
            for( var i = 0; i < eventPairs.Length; i++ )
            {
				eventPairs[ i ].eventListener.OnEnable();
			}
        }

        private void OnDisable()
        {
            for( var i = 0; i < eventPairs.Length; i++ )
            {
				eventPairs[ i ].eventListener.OnDisable();
			}           
        }

        private void Awake()
        {
            for( var i = 0; i < eventPairs.Length; i++ )
            {
				eventPairs[ i ].Pair();
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