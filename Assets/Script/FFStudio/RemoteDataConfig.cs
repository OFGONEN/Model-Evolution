/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public class RemoteDataConfig : MonoBehaviour
	{
#region Fields
        public MonoBehaviour targetComponent;
        public RemoteData[] remoteDatas;
		public ElephantConfigEvent elephantConfigEvent;
#endregion

#region Properties
#endregion

#region Unity API
        private void Awake()
        {
			if( !GameSettings.Instance.useRemoveConfig_Components )
				return;

            for( var i = 0; i < remoteDatas.Length; i++ )
            {
				elephantConfigEvent.Raise( targetComponent, remoteDatas[ i ].targetFieldName, remoteDatas[ i ].configKeyName );
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

[ System.Serializable ]
public struct RemoteData
{
	public string configKeyName;
	public string targetFieldName;
}