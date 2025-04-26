using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour {
	
	//variables visible in the inspector
	[Header("Spawn settings")]
	public bool leftSameDirection;
	public bool rightSameDirection;
	
	[Space(5)]
	public float startSpawnWait;
	[Space(5)]
	public int maxCarsPerWave;
	public int minCarsPerWave;
	[Space(5)]
	public float maxCarSpawnWait;
	public float minCarSpawnWait;
	[Space(5)]
	public float maxCarWaveWait;
	public float minCarWaveWait;
	
	[Header("Spawn positions")]
	public Vector3 carSpawnPos1;
	public Vector3 carSpawnPos2;
	public Vector3 carSpawnPos3;
	public Vector3 carSpawnPos4;
	
	public ObjectPool objectPool;
	
	//variables not visible in the inspector
	int carsPerWave;
	float carSpawnWait;
	float carWaveWait;
	Vector3 lastSpawnPos;

	//IEnumerator to randomize car waves
	IEnumerator Start(){
		//wait a moment at the beginning of the game
		yield return new WaitForSeconds(startSpawnWait);
        while(true){
			//while wave has not ended, keep spawning cars and wait a moment between spawning new ones
            for(int i = 0; i < carsPerWave; i++){
				SpawnNewCar();
				//time between spawning new cars is based on scrollspeed (car speed) to have the same amount of cars with differend speeds
				carSpawnWait = Random.Range(minCarSpawnWait, maxCarSpawnWait) * (1/FindObjectOfType<ScrollTexture>().scrollSpeed) * 10f;
                yield return new WaitForSeconds(carSpawnWait);
            }
			//after the wave, change the amount of cars and wave wait for next wave to randomize them
			carsPerWave = Random.Range(minCarsPerWave, maxCarsPerWave);
			carWaveWait = Random.Range(minCarWaveWait, maxCarWaveWait);
			
			//wait some time before starting next wave
            yield return new WaitForSeconds(carWaveWait);
        }
    }
	
	void SpawnNewCar(){
		//check if there are cars, choose a random one, choose a position and instantiate it
		if(objectPool.cars.Count == 0)
			return;
		
		Vector3 randomPos = RandomPosition();
	
		//make sure car is not spawned at the same position as last car
		while(lastSpawnPos == randomPos){
			randomPos = RandomPosition();
		}
			
		GameObject newCar = objectPool.PoolObject(objectPool.cars, false);
		newCar.transform.position = randomPos;
		lastSpawnPos = randomPos;
		newCar.GetComponent<MoveObject>().sameDirection = true;
	
		newCar.transform.localRotation = Quaternion.identity;
	
		if((randomPos.z < 0 && !leftSameDirection) || (randomPos.z > 0 && !rightSameDirection)){
			newCar.GetComponent<MoveObject>().sameDirection = false;
			newCar.transform.Rotate(Vector3.up * 180);
		}	
	}
	
	public Vector3 RandomPosition(){
		switch(Random.Range(0, 4)){
			case 0: return carSpawnPos1; 
			case 1: return carSpawnPos2; 
			case 2: return carSpawnPos3; 
			case 3: return carSpawnPos4; 
		}
		
		return Vector3.zero;
	}
}
