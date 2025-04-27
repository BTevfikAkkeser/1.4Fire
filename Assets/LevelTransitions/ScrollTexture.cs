using UnityEngine;
using System.Collections;

public class ScrollTexture : MonoBehaviour {
	
	//not visible in the inspector
	[HideInInspector]
	public float scrollSpeed;
	
	//visible in the inspector
	public Material[] materials;
	public Material[] water;
	
	RoadTransitions roadTransitionsScript;
	
	//scrollspeed is 2 and start accelerating
	void Start(){
		roadTransitionsScript = GetComponent<RoadTransitions>();
	}

	void Update(){
		
		//assign the scrollspeed if it's 0
		//if(scrollSpeed < carHighSpeed)
		//	scrollSpeed += Time.deltaTime * 2;
		
		//for each material in the materials array, scroll it using offset
		foreach(Material material in materials){
			if(!roadTransitionsScript.roadTransition || material.name == "water scrolling"){
				material.mainTextureOffset += new Vector2(Time.deltaTime * scrollSpeed, 0);
			}
		}
		
		//also scroll water
		foreach(Material material in water){
			material.mainTextureOffset = new Vector2(material.mainTextureOffset.x, Time.time * 0.2f);
		}
	}
}
