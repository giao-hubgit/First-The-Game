Shader "Unlit/PixelOutline"
{ 
    Properties 
    { 
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Radius("Radius", Range(0,10)) = 5

        [Toggle] _EnableOutline ("Enable Outline", Float) = 0
    } 
    
    SubShader 
    { 
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        
        Cull Off     
        Lighting Off   
        ZWrite Off      
        Blend SrcAlpha OneMinusSrcAlpha 
        LOD 100    
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define MAX_RADIUS 5

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _Color;
            float _Radius;
            float _EnableOutline;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);

                if (_EnableOutline == 0)
                {
                    return c;
                }

                float na = 0;
                int r = ceil(_Radius);
                r = min(r, MAX_RADIUS);

                for (int nx = -MAX_RADIUS; nx <= MAX_RADIUS; nx++)
                {
                    for (int ny = -MAX_RADIUS; ny <= MAX_RADIUS; ny++)
                    {
                        if (abs(nx) > r || abs(ny) > r) continue;

                        if (nx*nx + ny*ny <= r*r)
                        {
                            fixed4 nc = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x*nx, _MainTex_TexelSize.y*ny));
                            na += ceil(nc.a);
                        }
                    }
                }

                na = clamp(na, 0, 1);
                na -= ceil(c.a);

                return lerp(c, _Color, na);
            }
            ENDCG
        }
    }
}