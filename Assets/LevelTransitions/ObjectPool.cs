using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//pool object settings
[System.Serializable]
public class poolObject{
	public GameObject prefab;
	
	[HideInInspector]
	public List<GameObject> storage = new List<GameObject>();
}

public class ObjectPool : MonoBehaviour {
	
	//variables visible in the inspector
	public List<poolObject> roadSideObjects;
	public List<poolObject> cars;
	public GameObject bridge;
	
	//not visible in the inspector
	List<GameObject> bridgeStorage = new List<GameObject>();
	
	//return a new pool object
	public GameObject PoolObject(List<poolObject> objects, bool returnBridge){
		//check if we need to return a bridge
		if(returnBridge)
			return getObject(objects, 0, true);
		
		//get a random object index
		int randomIndex = 0;
		if(objects == cars){
			randomIndex = Random.Range(0, objects.Count);
		}
		else{
			randomIndex = newRandomIndex();
		}
		
		//don't return an object if the index is -1
		if(randomIndex == -1)
			return null;
		
		//return a new random object
		return getObject(objects, randomIndex, false);
	}
	
	//get the object to return
	public GameObject getObject(List<poolObject> objects, int randomIndex, bool returnBridge){
		//check the list type
		int listType = 0;
			
		if(objects == cars)
			listType = 1;
		
		//create the id and prefab
		List<GameObject> storageObjects = null;
		string id = "";
		GameObject prefab = null;
		
		//change the prefab, id and storage list when this should return a bridge
		if(returnBridge){
			storageObjects = bridgeStorage;
			id = "bridge";
			prefab = bridge;
		}
		else{
			storageObjects = objects[randomIndex].storage;
			id = listType + "" + randomIndex;
			prefab = objects[randomIndex].prefab;
		}
		
		//get a new object from the storage and return it
		if(storageObjects.Count > 0){
			GameObject storageObject = storageObjects[0];
			storageObject.SetActive(true);
			storageObjects.RemoveAt(0);
			storageObject.GetComponent<MoveObject>().Start();
			return storageObject;
		}
		//create a new object
		else{
			GameObject storageObject = Instantiate(prefab);
			storageObject.transform.parent = transform;
			MoveObject moveObject = storageObject.GetComponent<MoveObject>();
			moveObject.poolID = id;
			moveObject.objectPool = this;
			return storageObject;
		}
	}
	
	//return object back to the storage
	public void addToStorage(GameObject sceneObject, string objectID){
		//disable the object and zero out the position
		sceneObject.SetActive(false);
		sceneObject.transform.position = Vector3.zero;
		
		//check if this is a bridge and add it to the bridge storage
		if(objectID == "bridge"){
			bridgeStorage.Add(sceneObject);
			return;
		}
		
		//otherwise, use the index to find the list and add it to that list
		int objectIndex = int.Parse(objectID.Substring(1, objectID.Length - 1));
		
		if(int.Parse(objectID.Substring(0, 1)) == 1){
			cars[objectIndex].storage.Add(sceneObject);
		}
		else{
			roadSideObjects[objectIndex].storage.Add(sceneObject);
		}
	}
	
	//get a new random index for the environment objects
	public int newRandomIndex(){
		List<int> availableObjects = new List<int>();
		RoadTransitions roadTransitionsScript = GameObject.FindObjectOfType<RoadTransitions>();
		int currentEnvironment = roadTransitionsScript.transitionIndex;
			
		for(int i = 0; i < roadSideObjects.Count; i++){
			if(currentEnvironment >= 0){
				if((roadTransitionsScript.roadTransitions[currentEnvironment].environmentMask & (1 << i)) != 0)
					availableObjects.Add(i);
				}
			else{
				if((roadTransitionsScript.startEnvironmentMask & (1 << i)) != 0)
					availableObjects.Add(i);
			}
		}
		
		//if there are objects to spawn, return a random one
		if(availableObjects.Count > 0){
			return availableObjects[Random.Range(0, availableObjects.Count)];
		}
		//else, return -1 to let it know that there are no objects available
		else{
			return -1;
		}
	}
}
