using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

// Shapes © Freya Holmér - https://twitter.com/FreyaHolmer/
// Website & Documentation - https://acegikmo.com/shapes/
namespace Shapes {

	public class SceneDiscEditor : SceneEditGizmos {

		static bool isEditing;

		ArcHandle arcHandleEnd = ShapesHandles.InitAngularHandle();
		ArcHandle arcHandleStart = ShapesHandles.InitAngularHandle();
		ArcHandle arcHandleRadius = ShapesHandles.InitRadialHandle();
		ArcHandle arcHandleThicknessOuter = ShapesHandles.InitRadialHandle();
		ArcHandle arcHandleThicknessInner = ShapesHandles.InitRadialHandle();

		public SceneDiscEditor( Editor parentEditor ) => this.parentEditor = parentEditor;

		protected override bool IsEditing {
			get => isEditing;
			set => isEditing = value;
		}

		Color GetAvgDiscColor( Disc disc ) {
			switch( disc.ColorMode ) {
				case Disc.DiscColorMode.Single:   return disc.Color;
				case Disc.DiscColorMode.Radial:   return ( disc.ColorOuter + disc.ColorInner ) / 2f;
				case Disc.DiscColorMode.Angular:  return ( disc.ColorStart + disc.ColorEnd ) / 2f;
				case Disc.DiscColorMode.Bilinear: return ( disc.ColorInnerStart + disc.ColorInnerEnd + disc.ColorOuterStart + disc.ColorOuterEnd ) / 2f;
				default:                          throw new ArgumentOutOfRangeException();
			}
		}

		Color GetHandleColor( Disc disc ) => ShapesHandles.GetHandleColor( GetAvgDiscColor( disc ) );

		public bool DoSceneHandles( Disc disc ) {
			if( IsEditing == false )
				return false;
			if( disc.RadiusSpace != ThicknessSpace.Meters )
				return false;

			bool holdingShift = ( Event.current.modifiers & EventModifiers.Shift ) != 0;
			bool editInnerOuterRadius = holdingShift;

			// set up matrix
			Vector3 rootDir = disc.transform.right;
			Vector3 discNormal = disc.transform.forward;
			Quaternion rot = Quaternion.LookRotation( rootDir, discNormal );
			Matrix4x4 mtx = Matrix4x4.TRS( disc.transform.position, rot, Vector3.one ); // todo: scale?

			using( new Handles.DrawingScope( GetHandleColor( disc ), mtx ) ) {
				// thickness handles
				if( disc.HasThickness && disc.ThicknessSpace == ThicknessSpace.Meters ) {
					using( var chchk = new EditorGUI.ChangeCheckScope() ) {
						arcHandleThicknessOuter.radius = disc.Radius + disc.Thickness * 0.5f;
						arcHandleThicknessOuter.DrawHandle();
						if( chchk.changed ) {
							Undo.RecordObject( disc, "edit disc" );
							if( editInnerOuterRadius ) {
								float prevInnerRadius = disc.Radius - disc.Thickness * 0.5f;
								float newOuterRadius = arcHandleThicknessOuter.radius;
								disc.Radius = ( prevInnerRadius + newOuterRadius ) / 2;
								disc.Thickness = newOuterRadius - prevInnerRadius;
							} else {
								disc.Thickness = ( arcHandleThicknessOuter.radius - disc.Radius ) * 2;
							}
						}
					}

					// inner radius
					if( editInnerOuterRadius ) {
						using( var chchk = new EditorGUI.ChangeCheckScope() ) {
							arcHandleThicknessInner.radius = disc.Radius - disc.Thickness * 0.5f;
							arcHandleThicknessInner.DrawHandle();
							if( chchk.changed ) {
								Undo.RecordObject( disc, "edit disc" );
								float prevOuterRadius = disc.Radius + disc.Thickness * 0.5f;
								float newInnerRadius = arcHandleThicknessInner.radius;
								disc.Radius = ( newInnerRadius + prevOuterRadius ) / 2;
								disc.Thickness = prevOuterRadius - newInnerRadius;
							}
						}
					}
				}

				// radius handle
				using( var chchk = new EditorGUI.ChangeCheckScope() ) {
					arcHandleRadius.radius = disc.Radius;
					arcHandleRadius.DrawHandle();
					if( chchk.changed ) {
						Undo.RecordObject( disc, "edit disc radius" );
						disc.Radius = arcHandleRadius.radius;
					}
				}


				// angle handles
				if( disc.HasSector && editInnerOuterRadius == false ) {
					arcHandleEnd.angle = disc.AngRadiansEnd * Mathf.Rad2Deg;
					arcHandleStart.angle = disc.AngRadiansStart * Mathf.Rad2Deg;

					foreach( ArcHandle arcHandle in new[] { arcHandleStart, arcHandleEnd } ) {
						float radius = disc.Radius;
						if( disc.ThicknessSpace == ThicknessSpace.Meters && disc.HasThickness )
							radius += disc.Thickness / 2f;
						arcHandle.radius = radius;
						arcHandle.wireframeColor = Color.clear;
						arcHandle.radiusHandleSizeFunction = pos => 0f; // no radius handle

						using( var chchk = new EditorGUI.ChangeCheckScope() ) {
							arcHandle.DrawHandle();
							if( chchk.changed ) {
								Undo.RecordObject( disc, "edit disc angle" );
								//disc.Radius = arcHandle.radius;
								if( arcHandle == arcHandleEnd )
									disc.AngRadiansEnd = arcHandle.angle * Mathf.Deg2Rad;
								else
									disc.AngRadiansStart = arcHandle.angle * Mathf.Deg2Rad;
							}
						}
					}
				}
			}

			return false;
		}


	}

}