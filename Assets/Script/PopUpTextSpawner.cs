/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;
using UnityEditor;

public class PopUpTextSpawner : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Setup" ) ] public Pool_UIPopUpText pool_UIPopUpText;
    [ BoxGroup( "Setup" ) ] public PopUpTextData[] popUpTextDatas;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    [ Button() ]
	public void Spawn( int index )
	{
		var data = popUpTextDatas[ index ];

		Transform parent = data.parent ? transform : null;

		var entity = pool_UIPopUpText.GetEntity();
		entity.Spawn( transform.position + data.offset, data.text, data.size, data.color );
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		for( var i = 0; i < popUpTextDatas.Length; i++ )
		{
			var data = popUpTextDatas[ i ];
			Handles.Label( transform.position + data.offset, "PopUp:" + data.text );
			Handles.DrawWireCube( transform.position + data.offset, Vector3.one / 4f );
		}
	}
#endif
#endregion
}