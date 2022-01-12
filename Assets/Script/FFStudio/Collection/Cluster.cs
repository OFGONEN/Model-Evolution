/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "cluster", menuName = "FF/Data/Cluster" )]
	public class Cluster : ScriptableObject
	{
		public int cluster_count;
		private Dictionary<int, IClusterEntity> cluster_entities;

		public void Init()
		{
			cluster_entities = new Dictionary<int, IClusterEntity>( cluster_count );
		}

		public void Subscribe( IClusterEntity entity )
		{
			cluster_entities.Add( entity.GetInstanceID(), entity );
		}

		public void UnSubscribe( IClusterEntity entity )
		{
			cluster_entities.Remove( entity.GetInstanceID() );
		}

		public void UpdateCluster()
		{
			foreach( var entity in cluster_entities.Values )
			{
				entity.OnUpdate_Cluster();
			}
		}
	}
}