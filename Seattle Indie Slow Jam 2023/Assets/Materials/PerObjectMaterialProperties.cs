using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
	static MaterialPropertyBlock block;
	static int levelColorId = Shader.PropertyToID("_LevelColor");
	static int ammoLevelId = Shader.PropertyToID("_AmmoLevel");
	static int mainTexId = Shader.PropertyToID("_MainTex");


	[SerializeField, Range(-1f, 1f)]
	float ammoLevel = 0.5f;

	[SerializeField]
	Color levelColor = Color.white;

	[SerializeField]
	Texture2D baseTex;
	void Awake()
	{
		OnValidate();
	}
	//Called when values changed
	void OnValidate()
	{
		if (block == null)
		{
			block = new MaterialPropertyBlock();
		}
		block.SetColor(levelColorId, levelColor);
		block.SetFloat(ammoLevelId, ammoLevel);
		if(baseTex != null) block.SetTexture(mainTexId, baseTex);
		GetComponent<Renderer>().SetPropertyBlock(block);
	}
}
