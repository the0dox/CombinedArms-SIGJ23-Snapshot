using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliasingPostFX : MonoBehaviour
{
    public int rtRes = 512;
    public float conVal = 1.5f;
    RenderTexture rt;
    RenderTexture upScale;
    [SerializeField]
    Shader EdgeDectectionPass;
    Material EdgeMat;
    [SerializeField]
    Shader SharpenContrastPass;
    Material SharpenMat;
    public static List<AliasingPostFX> activeFXs = new List<AliasingPostFX>();

    void Awake()
    {
        activeFXs.Add(this);
    }

    void OnDestroy()
    {
        activeFXs.Remove(this);
    }

    //Try out edge dection with sobel kernals on lower res rt
    //To increase the contrast between pixels by creating jagged lines around chracters
    // Start is called before the first frame update
    void Start()
    {
        rt = new RenderTexture(rtRes, rtRes, 0);
        upScale = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 0);
        EdgeMat = new Material(EdgeDectectionPass);
        SharpenMat = new Material(SharpenContrastPass);
        SharpenMat.SetFloat("_ConVal", conVal);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //Convert 
        Graphics.Blit(source,upScale, EdgeMat);
        //Graphics.Blit(rt, upScale);
        SharpenMat.SetTexture("_EdgeTex", upScale);
        Graphics.Blit(source,rt,SharpenMat);
        Graphics.Blit(rt, destination);
        //Graphics.Blit(source, destination, EdgeMat);
    }
}
