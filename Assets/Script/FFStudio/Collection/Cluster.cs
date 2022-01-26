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
#if UNITY_EDITOR
			IClusterEntity cluster_entity;
			cluster_entities.TryGetValue( entity.GetID(), out cluster_entity );

			if( cluster_entity == null )
				cluster_entities.Add( entity.GetID(), entity );
			else
				FFLogger.Log( "Cluster Entity is already Subscribed", ( entity as MonoBehaviour ) );
#else
			cluster_entities.Add( entity.GetID(), entity );
#endif		
		}

		public void UnSubscribe( IClusterEntity entity )
		{
			cluster_entities.Remove( entity.GetID() );
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