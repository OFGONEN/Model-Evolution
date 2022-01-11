// Shapes © Freya Holmér - https://twitter.com/FreyaHolmer/
// Website & Documentation - https://acegikmo.com/shapes/
#include "UnityCG.cginc"
#include "../Shapes.cginc"
#pragma target 3.0

UNITY_INSTANCING_BUFFER_START(Props)
UNITY_DEFINE_INSTANCED_PROP(int, _ScaleMode)
UNITY_DEFINE_INSTANCED_PROP(int, _Alignment)
UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
UNITY_DEFINE_INSTANCED_PROP(float4, _ColorOuterStart)
UNITY_DEFINE_INSTANCED_PROP(float4, _ColorInnerEnd)
UNITY_DEFINE_INSTANCED_PROP(float4, _ColorOuterEnd)
UNITY_DEFINE_INSTANCED_PROP(float, _Radius)
UNITY_DEFINE_INSTANCED_PROP(int, _RadiusSpace)
UNITY_DEFINE_INSTANCED_PROP(float, _Thickness)
UNITY_DEFINE_INSTANCED_PROP(int, _ThicknessSpace)
UNITY_DEFINE_INSTANCED_PROP(float, _AngleStart)
UNITY_DEFINE_INSTANCED_PROP(float, _AngleEnd)
UNITY_DEFINE_INSTANCED_PROP(int, _RoundCaps)
UNITY_DEFINE_INSTANCED_PROP(int, _DashType)
UNITY_DEFINE_INSTANCED_PROP(half, _DashSize)
UNITY_DEFINE_INSTANCED_PROP(float, _DashOffset)
UNITY_DEFINE_INSTANCED_PROP(half, _DashSpacing)
UNITY_DEFINE_INSTANCED_PROP(int, _DashSpace)
UNITY_DEFINE_INSTANCED_PROP(int, _DashSnap)
UNITY_INSTANCING_BUFFER_END(Props)

#define ALIGNMENT_FLAT 0
#define ALIGNMENT_BILLBOARD 1

struct VertexInput {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float2 uv0 : TEXCOORD0;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct VertexOutput {
    float4 pos : SV_POSITION;
    float2 uv0 : TEXCOORD0;
    half pxCoverage : TEXCOORD1;
    half innerRadiusFraction : TEXCOORD2;
    #ifdef INNER_RADIUS 
        half centerRadiusMeters : TEXCOORD3; // these are all used for dashes
        half thicknessMeters : TEXCOORD4;
        half pxPerMeter : TEXCOORD5;
        half uniformScale : TEXCOORD6;
    #endif
	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
};

// I hate C
inline void ApplyRadialMask( inout half mask, VertexOutput i, out half tRadial );
inline void ApplyAngularMask( inout half mask, VertexOutput i,out half tAngularFull, out half tAngular, out half2 coord, out half ang, out half angStart, out half angEnd, out bool useRoundCaps, out half sectorSize );
inline void ApplyEndCaps( inout half mask, VertexOutput i, half2 coord, half ang, half angStart, half angEnd, bool useRoundCaps );
inline void ApplyDashes( inout half mask, VertexOutput i, half t, half tRadial, half sectorSize );
inline half4 GetColor( half tRadial, half tAngular );

VertexOutput vert (VertexInput v) {
	UNITY_SETUP_INSTANCE_ID(v);
    VertexOutput o = (VertexOutput)0;
	UNITY_TRANSFER_INSTANCE_ID(v, o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	half uniformScale = GetUniformScale();
	float radius = UNITY_ACCESS_INSTANCED_PROP(Props, _Radius) * uniformScale;
	float radiusSpace = UNITY_ACCESS_INSTANCED_PROP(Props, _RadiusSpace);
	float3 wPos = LocalToWorldPos( float3(0,0,0) ); // per vertex makes it real wonky so shrug~
    float3 camRight = CameraToWorldVec( float3(1,0,0) );
    LineWidthData widthDataRadius = GetScreenSpaceWidthDataSimple( wPos, camRight, radius*2, radiusSpace );
    o.pxCoverage = widthDataRadius.thicknessPixelsTarget;

	// padding correction
	half paddingMeters = AA_PADDING_PX/widthDataRadius.pxPerMeter;
    half radiusInMeters = widthDataRadius.thicknessMeters / 2; // actually, center radius
	half vertexRadius;
	half outerRadiusFraction;
	
	#ifdef INNER_RADIUS
        o.pxPerMeter = widthDataRadius.pxPerMeter;
	    int scaleMode = UNITY_ACCESS_INSTANCED_PROP(Props, _ScaleMode);
        half scaleThickness = scaleMode == SCALE_MODE_UNIFORM ? uniformScale : 1;
	    float thickness = UNITY_ACCESS_INSTANCED_PROP(Props, _Thickness) * scaleThickness;
	    float thicknessSpace = UNITY_ACCESS_INSTANCED_PROP(Props, _ThicknessSpace);
	    LineWidthData widthDataThickness = GetScreenSpaceWidthDataSimple( wPos, camRight, thickness, thicknessSpace );
	    half thicknessRadius = widthDataThickness.thicknessMeters / 2;
	    o.thicknessMeters = widthDataThickness.thicknessMeters;
	    o.pxCoverage = widthDataThickness.thicknessPixelsTarget; // todo: this isn't properly handling coordinate scaling yet
	    half radiusOuter = radiusInMeters + thicknessRadius;
		vertexRadius = radiusOuter + paddingMeters;
		outerRadiusFraction = radiusOuter / vertexRadius;
	    o.innerRadiusFraction = (radiusOuter - thicknessRadius*2) / radiusOuter;
	    o.centerRadiusMeters = radiusInMeters;
	    o.uniformScale = uniformScale;
	#else
		vertexRadius = radiusInMeters + paddingMeters;
		outerRadiusFraction = radiusInMeters / vertexRadius;
	#endif
	
	v.vertex.xy = v.uv0 * vertexRadius;
	v.uv0 /= outerRadiusFraction; // padding correction

	if( UNITY_ACCESS_INSTANCED_PROP(Props, _Alignment) == ALIGNMENT_BILLBOARD ) {
		half3 frw = WorldToLocalVec( -DirectionToNearPlanePos( wPos ) );
		half3 camRightLocal = WorldToLocalVec( camRight );
		half3 up = normalize( cross( frw, camRightLocal ) );
		half3 right = cross( up, frw ); // already normalized
		v.vertex.xyz = v.vertex.x * right + v.vertex.y * up;
	}
	
	o.uv0 = v.uv0;
    o.pos = UnityObjectToClipPos( v.vertex / uniformScale );

    return o;
}

FRAG_OUTPUT_V4 frag( VertexOutput i ) : SV_Target {
	UNITY_SETUP_INSTANCE_ID(i); 

    half tRadial, tAngular; // interpolators for radial & angular gradient
    half tAngularFull; // angular gradient 0 to 1, used for dashes
	half ang, angStart, angEnd;
	bool useRoundCaps;
	half2 coord; // coordinates used for end caps, if applicable
	half sectorSize; // used to snap dash coords
	
	half mask = 1;
	ApplyRadialMask( /*inout*/ mask, i, /*out*/ tRadial );
	ApplyAngularMask( /*inout*/ mask, i,/*out*/ tAngularFull, /*out*/ tAngular, /*out*/ coord, /*out*/ ang, /*out*/ angStart, /*out*/ angEnd, /*out*/ useRoundCaps, /*out*/ sectorSize );
	ApplyEndCaps(/*inout*/ mask, i, coord, ang, angStart, angEnd, useRoundCaps);
	#ifdef INNER_RADIUS
	    ApplyDashes( /*inout*/ mask, i, tAngularFull, tRadial, sectorSize );
	#endif
	mask *= saturate(i.pxCoverage); // pixel fade
	
    half4 color = GetColor( tRadial, tAngular );    
	return ShapesOutput( color, mask );
}

inline half ArcLengthToAngle( half radius, half arcLength ){
    return arcLength / radius;
}

inline void ApplyDashes( inout half mask, VertexOutput i, half t, half tRadial, half sectorSize ){

    #ifdef INNER_RADIUS
    
    half dashInputSize = UNITY_ACCESS_INSTANCED_PROP(Props, _DashSize);
    if( dashInputSize <= VERY_SMOL )
        return;
    
    half dashInputSpacing = UNITY_ACCESS_INSTANCED_PROP(Props, _DashSpacing);
    half radiusMeters = i.centerRadiusMeters;
    int scaleMode = UNITY_ACCESS_INSTANCED_PROP(Props, _ScaleMode);
    int dashSpace = UNITY_ACCESS_INSTANCED_PROP(Props, _DashSpace);
    
    bool rescaleDashSpace = dashSpace != DASH_SPACE_FIXED_COUNT && scaleMode == SCALE_MODE_COORDINATE;
    half scaleDashes = rescaleDashSpace ? 1/i.uniformScale : 1;
    half dashSize = dashInputSize * scaleDashes;
	half scaleSpacing = rescaleDashSpace ? (dashInputSpacing + dashInputSize - dashSize) / dashInputSpacing  : 1;
    half offset = UNITY_ACCESS_INSTANCED_PROP(Props, _DashOffset); // todo: scale?
    int thicknessSpace = UNITY_ACCESS_INSTANCED_PROP(Props, _ThicknessSpace);
    int snap = UNITY_ACCESS_INSTANCED_PROP(Props, _DashSnap);
    
    // radius * TAU = arc len
    #ifdef SECTOR
        half angularSpan = sectorSize;
    #else
        half angularSpan = TAU;
    #endif 
    half distanceAroundRingTotal = radiusMeters * angularSpan;
    half distanceAroundRing = t * radiusMeters * TAU; // arc length in meters right now
    half dashSpacing = dashInputSpacing * scaleSpacing;
    LineDashData dashData = GetDashCoordinates( dashSize, dashSpacing, distanceAroundRing, distanceAroundRingTotal, i.thicknessMeters, thicknessSpace, i.pxPerMeter, offset, dashSpace, snap );
    
    int dashType = UNITY_ACCESS_INSTANCED_PROP(Props, _DashType);
    ApplyDashMask( /*inout*/ mask, dashData, tRadial*2-1, dashType );
    
    #endif
}

inline void ApplyRadialMask( inout half mask, VertexOutput i, out half tRadial ){
    half len = length( i.uv0 );
    mask = min( mask, StepAA( len, 1 ) ); // outer radius
	#ifdef INNER_RADIUS
		mask = min( mask, 1.0-StepAA( len, i.innerRadiusFraction ) ); // inner radius
		tRadial = saturate(InverseLerp( i.innerRadiusFraction, 1, len ) );
	#else
	    tRadial = saturate(len);
	#endif
}

inline void ApplyAngularMask( inout half mask, VertexOutput i, out half tAngularFull, out half tAngular, out half2 coord, out half ang, out half angStart, out half angEnd, out bool useRoundCaps, out half sectorSize ){

    #ifdef SECTOR
        half angStartInput = UNITY_ACCESS_INSTANCED_PROP(Props, _AngleStart);
		angStart = angStartInput;
	    angEnd = UNITY_ACCESS_INSTANCED_PROP(Props, _AngleEnd);
	    // Rotate so that the -pi/pi seam is opposite of the visible segment
		// 0 is the center of the segment post-rotate
		half angOffset = -(angEnd + angStart) * 0.5;
	    coord = Rotate( i.uv0, angOffset );
	    angStart += angOffset;
	    angEnd += angOffset;
	#else
	    // required for angular gradients on rings and discs
		angStart = 0;
        angEnd = TAU;
        coord = -i.uv0;
	#endif
	
	ang = atan2( coord.y, coord.x ); // -pi to pi
	sectorSize = abs(angEnd - angStart);
	tAngular = saturate(ang/sectorSize + 0.5); // angular interpolator for color
	
	
	
	#ifdef SECTOR
	
	    useRoundCaps = UNITY_ACCESS_INSTANCED_PROP(Props, _RoundCaps);
	        
	    float segmentMask;
		#if LOCAL_ANTI_ALIASING_QUALITY == 0
		    segmentMask = StepAA( abs(ang), sectorSize*0.5 );
		#else
		    // if arc
		    #ifdef INNER_RADIUS
		        if( useRoundCaps ){ // arcs with round caps hide the border anyway, so use cheap version
                    segmentMask = step( abs(ang), sectorSize*0.5 );
                } else {
		    #endif
		    
            float2 pdCoordSpace = float2( -coord.y, coord.x ) / dot( coord, coord );
            segmentMask = StepAAManualPD( coord, abs(ang), sectorSize*0.5, pdCoordSpace );
            
            // if arc
            #ifdef INNER_RADIUS
                } // this is a little cursed I know I'm sorry~
		    #endif
		    
		#endif
		
		// Adjust if close to 0 or TAU radians, fade in or out completely
		float THRESH_INVIS = 0.001;
		float THRESH_VIS = 0.002;
		float fadeInMask = saturate( InverseLerp( TAU - THRESH_VIS, TAU - THRESH_INVIS, sectorSize ) );
		float fadeOutMask = saturate( InverseLerp( THRESH_INVIS, THRESH_VIS, sectorSize ) );
		mask *= lerp( segmentMask * fadeOutMask, 1, fadeInMask );
	#else 
	    // SECTOR not defined
	    useRoundCaps = false;
	#endif
	
	// used for dashes
	#ifdef INNER_RADIUS
	    tAngularFull = (ang + sectorSize/2)/TAU;
	#else
	    tAngularFull = 0;
	#endif

}

inline void ApplyEndCaps( inout half mask, VertexOutput i, half2 coord, half ang, half angStart, half angEnd, bool useRoundCaps ){
    #if defined(INNER_RADIUS) && defined(SECTOR)
        if( useRoundCaps ){
            half halfThickness = (1-i.innerRadiusFraction)/2;
            half distToCenterOfRing = 1-halfThickness;
            half angToA = abs( ang - angStart );
            half angToB = abs( ang - angEnd );
            half capAng = (angToA < angToB) ? angStart : angEnd;
            half2 capCenter = AngToDir(capAng) * distToCenterOfRing;
            half endCapMask = StepAA( length(coord - capCenter), halfThickness );
            mask = max(mask, endCapMask);
        }   
    #endif
}

inline half4 GetColor( half tRadial, half tAngular ){
    half4 colInnerStart = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
    half4 colOuterStart = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorOuterStart);
    half4 colInnerEnd = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorInnerEnd);
    half4 colOuterEnd = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorOuterEnd);
	half4 colorStart = lerp( colInnerStart, colOuterStart, tRadial );
	half4 colorEnd = lerp( colInnerEnd, colOuterEnd, tRadial );
	return lerp( colorStart, colorEnd, tAngular );
} 