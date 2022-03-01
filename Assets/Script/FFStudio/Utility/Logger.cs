/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;

public class Logger : MonoBehaviour
{
#region Fields
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    public void Log( string text )
    {
        FFLogger.Log( name + ": " + text + ".", this );
    }
    
    public void PopUpText( string text )
    {
        FFLogger.PopUpText( transform.position, name + ": " + text + ".", this );
    }
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}