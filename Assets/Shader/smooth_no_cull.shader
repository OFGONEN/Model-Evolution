Shader "Custom/SmoothShader (No Culling)"
{
    Properties
    {
        [Header(Textures)]
       [NoScaleOffset] _MainTex ("Main Texture", 2D) = "white" {}
       [NoScaleOffset] [Normal] _NormalTex ("Normal Texture", 2D) = "bump" {}
        
        [Header(Settings)]
        [Gamma]_Color ("Color", Color) = (1,1,1,1)
        [Gamma] _EmissionColor ("Emission Color", Color) = (0,0,0,0)
        [Space(10)]
        _NdotLSmoothness ("Normal Dot Light Smoothness", Range(0,1)) = 0
        _AttenSmoothness ("Attenuation Smoothness", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull off


        CGPROGRAM
        #pragma surface surf Smooth fullforwardshadows
        #pragma target 3.0

        half _NdotLSmoothness;
        half _AttenSmoothness;
        sampler2D _MainTex, _NormalTex;
   
        half4 LightingSmooth(SurfaceOutput s, half3 lightDir, half atten)
        {
            half NdotL =  max(0, dot (s.Normal, lightDir)) * (1-_NdotLSmoothness) + _NdotLSmoothness  ;
            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * (atten * (1-_AttenSmoothness) + _AttenSmoothness));
            c.a = s.Alpha;
            return c;
        }

        struct Input
        {
            float2 uv_MainTex;
        };
        fixed4 _Color , _EmissionColor;

 
        void surf (Input IN, inout SurfaceOutput o)
        {

            fixed4 c = _Color * tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Normal = UnpackNormal( tex2D(_NormalTex, IN.uv_MainTex));
            o.Emission = _EmissionColor;
     
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
