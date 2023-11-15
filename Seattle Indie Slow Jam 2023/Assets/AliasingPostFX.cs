using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliasingPostFX : MonoBehaviour
{
    RenderTexture rt;
    RenderTexture upScale;
    [SerializeField]
    Shader EdgeDectectionPass;
    Material EdgeMat;
    [SerializeField]
    Shader SharpenContrastPass;
    Material SharpenMat;
    //Try out edge dection with sobel kernals on lower res rt
    //To increase the contrast between pixels by creating jagged lines around chracters
    // Start is called before the first frame update
    void Start()
    {
        rt = new RenderTexture(512, 512, 0);
        upScale = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 0);
        EdgeMat = new Material(EdgeDectectionPass);
        SharpenMat = new Material(SharpenContrastPass);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //Convert 
        Graphics.Blit(source,upScale, EdgeMat);
        //Graphics.Blit(rt, upScale);
        SharpenMat.SetTexture("_EdgeTex", upScale);
        Graphics.Blit(source,rt,SharpenMat);
        Graphics.Blit(rt, destination);
    }
}
