/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class Model : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Setup" ) ] public SkinnedMeshRenderer renderer_top;
    [ BoxGroup( "Setup" ) ] public SkinnedMeshRenderer renderer_bottom;
    [ BoxGroup( "Setup" ) ] public SkinnedMeshRenderer renderer_shoe;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
        //todo dress up persistent clothes
    }
#endregion

#region API
    public void DressUp( DressData data )
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
		//todo add to persistent
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}