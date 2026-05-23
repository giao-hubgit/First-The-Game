Shader "Custom/SpriteSpacedOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Outline Settings)]
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _Thickness ("Outline Thickness", Range(0, 10)) = 2
        _Spacing ("Spacing Distance", Range(0, 10)) = 1
        
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaSplitEnabled ("Alpha Split Enabled", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            fixed4 _Color;
            fixed4 _OutlineColor;
            float _Thickness;
            float _Spacing;
            float4 _MainTex_TexelSize;
            fixed4 _RendererColor; 

            sampler2D _MainTex;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 mainColor = tex2D(_MainTex, IN.texcoord);
                
                // ⭐ LUẬT THÉP: Nếu pixel hiện tại là súng (có độ đục > 10%)
                // -> Lập tức hiển thị súng, CẤM vẽ viền đè lên đây!
                if (mainColor.a > 0.1) 
                {
                    return mainColor * IN.color;
                }

                // --- Chỉ xử lý viền ở những vùng TRONG SUỐT ---
                float2 pixelSize = _MainTex_TexelSize.xy;
                float isOutline = 0;
                float isSpacing = 0;

                // Quét 8 hướng xung quanh pixel hiện hành
                float2 directions[8] = {
                    float2(0,1), float2(0,-1), float2(1,0), float2(-1,0),
                    float2(1,1), float2(-1,-1), float2(1,-1), float2(-1,1)
                };

                for(int i = 0; i < 8; i++) 
                {
                    // 1. Kiểm tra vùng "Khoảng cách" (Ở gần có súng không?)
                    float alphaNear = tex2D(_MainTex, IN.texcoord + directions[i] * _Spacing * pixelSize).a;
                    if (alphaNear > 0.1) isSpacing = 1;

                    // 2. Kiểm tra vùng "Viền" (Ở xa có súng không?)
                    float alphaFar = tex2D(_MainTex, IN.texcoord + directions[i] * (_Spacing + _Thickness) * pixelSize).a;
                    if (alphaFar > 0.1) isOutline = 1;
                }

                // Nếu xung quanh KHÔNG GẦN súng (tạo khoảng trống) NHƯNG LẠI ĐỦ XA để làm viền
                if (isOutline > 0 && isSpacing == 0) 
                {
                    fixed4 outColor = _OutlineColor;
                    outColor.rgb *= outColor.a; // Tránh lỗi viền bị phát sáng/mờ nổ màu
                    return outColor * IN.color;
                }

                return fixed4(0,0,0,0); // Những vùng trống khác thì vô hình
            }
        ENDCG
        }
    }
}