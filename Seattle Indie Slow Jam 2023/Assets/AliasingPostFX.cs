using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliasingPostFX : MonoBehaviour
{
    RenderTexture rt;
    [SerializeField]
    Shader EdgeDectectionPass;
    Material EdgeMat;
    //Try out edge dection with sobel kernals on lower res rt
    //To increase the contrast between pixels by creating jagged lines around chracters
    // Start is called before the first frame update
    void Start()
    {
        rt = new RenderTexture(256, 256, 0);
        EdgeMat = new Material(EdgeDectectionPass);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source,destination, EdgeMat);
        //Grpahics.Blit(source,rt);
        //Graphics.Blit(rt, destination);
    }
}
