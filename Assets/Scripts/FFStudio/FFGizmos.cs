using UnityEngine;

public static class FFGizmos
{
	public static void DrawCamera( Transform transform, Color color, Camera mainCamera )
	{
		if( transform == null || mainCamera == null )
			return;

		Gizmos.color = color;
		Gizmos.matrix = Matrix4x4.TRS( transform.position, transform.rotation, new Vector3( mainCamera.aspect, 1.0f, 1.0f ) );
		Gizmos.DrawFrustum( Vector3.zero, mainCamera.fieldOfView, 1.0f, mainCamera.nearClipPlane, 1.0f );
		Gizmos.DrawWireCube( Vector3.zero, new Vector3( mainCamera.nearClipPlane * 2,
														mainCamera.nearClipPlane * 2,
														mainCamera.nearClipPlane * 2 ) );
	}
}