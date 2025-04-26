using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvironmentSpawner : MonoBehaviour {
	
	//variables visible in the inspector
	[Header("Spawn settings")]
	public int minObjectsPerWave;
	public int maxObjectsPerWave;
	[Space(5)]
	public float minSpawnWait;
	public float maxSpawnWait;
	[Space(5)]
	public float minWaveWait;
	public float maxWaveWait;
	
	[Header("Spawn positions")]
	public Vector3 spawnPos1;
    public Vector3 spawnPos2;
	[Space(5)]
	public int randomSpawnRange;

	public ObjectPool objectPool;
	
	[Space(10)]
	public GameObject bridge;
	public string boatName;
	
	//variables not visible in the inspector
    Vector3 randomPos;
	Vector3 spawnPos;
	
	RoadTransitions roadTransitionsScript;
	
	int objectsPerWave;
	float spawnWait;
	float waveWait;
	
    IEnumerator Start(){
		roadTransitionsScript = GameObject.Find("Road manager").GetComponent<RoadTransitions>();
		
		//loop
        while(true){
            for(int i = 0; i < objectsPerWave; i++){
				
				if(!roadTransitionsScript.roadTransition){
					//while wave has not ended, keep spawning new objects
					int random = Random.Range(0, 50);
					//if random number is 0, spawn a bridge, else spawn object
					if(random == 0){
						if(!GameObject.Find("road lake(Clone)")){
							GameObject newBridge = objectPool.PoolObject(objectPool.roadSideObjects, true);	
							newBridge.transform.position = new Vector3(-60, 0, 0);
						}
					}
					else{
						Spawn();	
					}
				}
				//time between spawning new objects is based on scrollspeed (car speed) to have the same amount of objects with differend speeds
				spawnWait = Random.Range(minSpawnWait, maxSpawnWait) * (1/FindObjectOfType<ScrollTexture>().scrollSpeed) * 10f;
                yield return new WaitForSeconds(spawnWait);
            }
			//after the wave, change amount of objects and wave wait for next wave to randomize them
			objectsPerWave = Random.Range(minObjectsPerWave, maxObjectsPerWave);
			waveWait = Random.Range(minWaveWait, maxWaveWait);
			
			//wait some time before starting next wave
            yield return new WaitForSeconds(waveWait);
        }
    }
	
	void Spawn(){
		//choose random position
		int randomPosNumber = Random.Range(0, 2);  
		if(randomPosNumber == 0){
			randomPos = spawnPos1;	
		}
		else{
			randomPos = spawnPos2;	
		}
	
		//get the object and set its position
		GameObject newObject = objectPool.PoolObject(objectPool.roadSideObjects, false);
	
		if(newObject == null)
			return;
	
		newObject.transform.position = randomPos;
	
		//randomize the position
		int spawnRandomness = Random.Range(-randomSpawnRange, randomSpawnRange);
	
		if(newObject.name != boatName + "(Clone)"){
			spawnPos = new Vector3(randomPos.x, randomPos.y, randomPos.z + spawnRandomness);
			newObject.transform.localEulerAngles = new Vector3(newObject.transform.localEulerAngles.x, 0, newObject.transform.localEulerAngles.z);
		}
		else{
			spawnPos = new Vector3(randomPos.x - 40, randomPos.y, randomPos.z + spawnRandomness);
			newObject.transform.Rotate(Vector3.up * Random.Range(-30, 30));
		}
	
		newObject.transform.position = spawnPos;
	
		//turn houses on the left 180° to rotate them towards the road
		if(randomPosNumber == 1 && newObject.name != "tree(Clone)" && newObject.name != "cactus(Clone)" && newObject.name != "cactus 1(Clone)" && newObject.name != boatName + "(Clone)"){
			newObject.transform.Rotate(Vector3.up * 180);	
		}
	}
}
