/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class Model : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Shared" ) ] public DressDataList list_dressData;

    [ BoxGroup( "Setup" ) ] public SkinnedMeshRenderer renderer_top;
    [ BoxGroup( "Setup" ) ] public SkinnedMeshRenderer renderer_bottom;
    [ BoxGroup( "Setup" ) ] public SkinnedMeshRenderer renderer_shoe;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
        for( var i = 0; i < list_dressData.itemList.Count; i++ )
			DressUp( list_dressData.itemList[ i ] );
	}
#endregion

#region API
#endregion

#region Implementation
    private void DressUp( DressData data )
    {
		SkinnedMeshRenderer targetRenderer = null;

		if( data.dress_type == DressType.Top )
        {
			targetRenderer = renderer_top;
		}
        else if( data.dress_type == DressType.Bottom )
        {
			targetRenderer = renderer_bottom;
        }
        else if( data.dress_type == DressType.Shoe )
        {
			targetRenderer = renderer_shoe;
        }

		gameObject.UpdateSkinnedMeshRenderer( targetRenderer, data );
		list_dressData.AddList( data );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}