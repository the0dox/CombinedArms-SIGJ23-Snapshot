Shader "Hidden/EdgeDetection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
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
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 frag (v2f i) : SV_Target
            {
                const float2 offsets[8] = {
                    float2(-1, -1),
                    float2(0, -1),
                    float2(1, -1),
                    float2(-1, 0),
                    float2(1, 0),
                    float2(-1, 1),
                    float2(0, 1),
                    float2(1, 1)
                };
            const float Gy[8] = {-1.0,0.0,1.0,
                -2.0,2.0,
                -1.0,0.0,1.0 };
            const float Gx[8] = { 1.0,2.0,1.0,
                 0.0,0.0,
                -1.0,-2.0,-1.0 };
                float4 gxSum = float4(0,0,0,0);
                float4 gySum = float4(0, 0, 0, 0);
                for (int n = 0; n < 8; n++) {
                    //Convert To GrayScale to make indentifying edges easier
                    float4 inColor = tex2D(_MainTex, i.uv + offsets[n] * _MainTex_TexelSize.xy);
                    inColor = float4(pow(inColor.x, 2), pow(inColor.y, 2), pow(inColor.z, 2),1);
                    float gray = dot(inColor.xyz, float3(0.2126, 0.7152, 0.0722));
                    float4 grayColor = float4(sqrt(gray), sqrt(gray), sqrt(gray), 1);
                    gxSum += Gx[n] * grayColor;//tex2D(_MainTex,i.uv + offsets[n] * _MainTex_TexelSize.xy);
                    gySum += Gy[n] * grayColor;//tex2D(_MainTex, i.uv +  offsets[n] * _MainTex_TexelSize.xy);
                }
                float4 G = abs(gxSum) + abs(gySum);          
                //if (length(G) < 1) G = float4(0, 0, 0, 0);
                G.w = 1.0;
                fixed4 col = G;
                // just invert the colors
                //col.rgb = 1 - col.rgb;
                return col;
            }
            ENDCG
        }
    }
}
