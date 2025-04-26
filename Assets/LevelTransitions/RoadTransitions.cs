using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//road transition variables
public class RoadTransition{
	public int minDelay;
	public int maxDelay;
	public GameObject transitionRoadObject;
	public GameObject newRoadObject;
	public int environmentMask;
}

public class RoadTransitions : MonoBehaviour {
	
	[HideInInspector]
	public List<RoadTransition> roadTransitions;
	
	//visible in the inspector
	public GameObject startRoadObject;
	public GameObject water;
	public string[] roadPartsWithWater;
	
	//not visible in the inspector
	[HideInInspector]
	public bool roadTransition;
	
	[HideInInspector]
	public int transitionIndex;
	
	[HideInInspector]
	public int startEnvironmentMask;
	
	[HideInInspector]
	public bool cloneLast;
	
	bool transitionStart;
	
	GameObject currentRoadObject;
	GameObject newRoadObject;
	
	IEnumerator Start(){
		//set the transition index to -1 so the environment spawner knows there hasn't been a road transition yet
		transitionIndex = -1;
		
		//if there is a road object in the scene, start transitions
		if(startRoadObject){
			//assign starting road object
			currentRoadObject = startRoadObject;
			
			//loop through the transitions
			for(int i = 0; i < roadTransitions.Count; i++){
				
				//wait for a new transition
				yield return new WaitForSeconds(Random.Range(roadTransitions[i].minDelay, roadTransitions[i].maxDelay));
				
				//get transition index
				transitionIndex = i;
				
				//start transition if the objects have been assigned
				if(roadTransitions[i].transitionRoadObject && roadTransitions[i].newRoadObject){
					startTransition();
				}
				else{
					Debug.LogWarning("element " + transitionIndex + " of the road transitions is not complete");
				}
			}
		}
	}
	
	void Update(){
		//check if the water should be visible
		checkWater();
		
		//move road objects during transition
		if(roadTransition && newRoadObject.transform.position.x < 6.65f){
			float moveSpeed = FindObjectOfType<ScrollTexture>().scrollSpeed * 7;
			currentRoadObject.transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
			newRoadObject.transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
		}
		//end the transition if the original road object is not visible anymore
		else if(roadTransition && transitionStart){
			endTransition();
		}
		//if the transition ended already, remove the last road object and position the new object correctly
		else if(roadTransition){
			newRoadObject.transform.position = new Vector3(6.65f, newRoadObject.transform.position.y, newRoadObject.transform.position.z);
			roadTransition = false;
			Destroy(currentRoadObject);
			currentRoadObject = newRoadObject;
		}
	}
	
	//start a transition by adding the transition object
	void startTransition(){
		newRoadObject = Instantiate(roadTransitions[transitionIndex].transitionRoadObject, transform.position, transform.rotation);
		roadTransition = true;
		transitionStart = true;
	}
	
	void endTransition(){
		//destroy first road object
		Destroy(currentRoadObject);
		//position the transition object
		newRoadObject.transform.position = new Vector3(6.65f, newRoadObject.transform.position.y, newRoadObject.transform.position.z);
		//assign current road object
		currentRoadObject = newRoadObject;
		//add the end part
		newRoadObject = Instantiate(roadTransitions[transitionIndex].newRoadObject, transform.position, transform.rotation);
		transitionStart = false;
	}
	
	void checkWater(){
		//water is not visible
		bool visible = false;
		
		//if any of the roadparts with water is in the scene, show water
		foreach(string name in roadPartsWithWater){
			if(GameObject.Find(name + "(Clone)")){
				visible = true;
			}
		}
		
		//make water visible/invisible based on the current road parts
		water.SetActive(visible);
	}
}
