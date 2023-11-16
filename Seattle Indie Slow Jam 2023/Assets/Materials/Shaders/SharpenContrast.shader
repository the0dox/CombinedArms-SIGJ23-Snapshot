Shader "Hidden/SharpenContrast"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeTex ("Texture", 2D) = "white" {}
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
            sampler2D _EdgeTex;
            float4 _EdgeTex_TexelSize;

            //Increases the difference between color and color 2 based on conVal
            //Returns a color which further away from color2
            float3 ChangeDiff(float3 color,float3 color2,float conVal) {
                return (color - color2) * conVal + color2;
            }

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
                float4 col = tex2D(_MainTex, i.uv);
                float4 eC = tex2D(_EdgeTex, i.uv);
                //if (eC.x > .5) {
                //    col = float4(1, 0, 0, 1);
                //}
                
                for(int n = 0; n < 8; n++) {
                    float4 mC_Off = tex2D(_MainTex, i.uv + offsets[n] * _MainTex_TexelSize.xy);
                    float4 eC_Off = tex2D(_EdgeTex, i.uv + offsets[n] * _EdgeTex_TexelSize.xy);
                    if (length(eC_Off - eC) > .6) {
                        //1.2
                        col.xyz = ChangeDiff(col.xyz, mC_Off.xyz, 1.5); //Incrementally increase the difference for each large diffence
                    } 
                }
                
                // just invert the colors
                //col.rgb = 1 - col.rgb;
                return col;
            }
            ENDCG
        }
    }
}
