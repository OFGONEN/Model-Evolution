/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class ClusterManager : MonoBehaviour
{
#region Fields
    public Cluster[] clusters;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
        for( var i = 0; i < clusters.Length; i++ )
        {
			clusters[ i ].Init();
		}
    }

    private void Update()
    {
        for( var i = 0; i < clusters.Length; i++ )
        {
			clusters[ i ].UpdateCluster();
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

public interface IClusterEntity
{
	void Subscribe_Cluster();
	void UnSubscribe_Cluster();
	void OnUpdate_Cluster();
	int GetInstanceID();
}
