using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour {
	
	//visible in the inspector
	public bool car;
	public bool switchLine;
	public int slowDownDistance;
	public float vehicleLength = 6;
	
	[HideInInspector]
	public bool sameDirection = true;
	
	[HideInInspector]
	public string poolID;
	[HideInInspector]
	public ObjectPool objectPool;
	
	[HideInInspector]
	public bool nearMiss;
	[HideInInspector]
	public bool collision;
	[HideInInspector]
	public float randomSpeed;
	
	//not visible in the inspector
	float nextPos;
	
	GameObject lightLeft;
	GameObject lightRight;
	
	CarSpawner carSpawner;
	ScrollTexture scrollTextureScript;
	
	float movespeed;
	
	Coroutine switchLineCoroutine;
	
	public void Start(){
		scrollTextureScript = FindObjectOfType<ScrollTexture>();
		//define random speed for car objects
		randomSpeed = Random.Range(2.5f, 3f);
		nextPos = 0;
		movespeed = 0;
		
		//find car lights and turn them off
		if(car && switchLine){
			lightLeft = transform.Find("light left").gameObject;
			lightRight = transform.Find("light right").gameObject;
	
			lightLeft.SetActive(false);
			lightRight.SetActive(false);
	
			carSpawner = GameObject.FindObjectOfType<CarSpawner>();
		}
	
		//if this car can change track, make the change of actually change track smaller
		if(Random.Range(0, 3) == 0 && car && switchLine)
			//start changing track (switching line)
			StartCoroutine(OtherLine());	
	}

	void Update(){	
		//if object is a car, move it with car speed (randomspeed) and if another car is to close, slow down
		if(car){
			if(Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), -transform.right, slowDownDistance))
				randomSpeed += Time.deltaTime;	
		
			if(sameDirection){
				transform.Translate(Vector3.right * Time.deltaTime * scrollTextureScript.scrollSpeed * randomSpeed);	
			}
			else{
				transform.Translate(Vector3.left * Time.deltaTime * scrollTextureScript.scrollSpeed * randomSpeed * 3.5f);	
			}
		}
		//else, move it as fast as all other objects and textures
		else{
			transform.Translate(Vector3.right * Time.deltaTime * scrollTextureScript.scrollSpeed * 6f, Space.World);
		}
	
		//reset all objects after they aren't visible
		if(transform.position.x > 15)
			objectPool.addToStorage(gameObject, poolID);
	
		if(!car && GameObject.Find("road river(Clone)") && transform.position.x < -50)
			objectPool.addToStorage(gameObject, poolID);
		
		if(movespeed != 0){
			transform.Translate(Vector3.forward * Time.deltaTime * movespeed);	
	
			if((transform.position.z >= nextPos && movespeed > 0) || (transform.position.z <= nextPos && movespeed < 0)){
				StopCoroutine(switchLineCoroutine);
				
				lightRight.SetActive(false);
				lightLeft.SetActive(false);
				
				movespeed = 0;
			}
		}
	}
	
	IEnumerator OtherLine(){
		//after instantiating this car, wait for seconds based on car speed (scrollspeed of the textures)
		yield return new WaitForSeconds(Random.Range(0.2f, 0.5f) * (1/scrollTextureScript.scrollSpeed) * 10);
		
		//will this car move left or right?
		int leftRight = Random.Range(0, 2);
		//get current z position
		float posZ = transform.position.z;
	
		if(canSwitchLine(leftRight)){
			//if possible, check if car wants to go left and move it using isMovingLeft. Also turn on left light
			if(leftRight == 0 && transform.position.z > -5.8f){
				nextPos = posZ - 4;
				switchLineCoroutine = StartCoroutine(SwitchLine(false));
			}
			else if(leftRight == 1 && transform.position.z < 5.8f){
				//if possible, check if car wants to go right and move it using isMovingRight. Also turn on right light
				nextPos = posZ + 4;
				switchLineCoroutine = StartCoroutine(SwitchLine(true));
			}
		}
	}
	
	//check if it is save to switch line
	public bool canSwitchLine(int leftRight){
		//get ray start positions
		float[] xStartPositions = {vehicleLength, (vehicleLength/2), 0, -(vehicleLength/2), -vehicleLength};
		
		//check if the road is empty with raycasts
		for(int i = 0; i < 5; i++){
			Vector3 startPosition = new Vector3(transform.position.x + xStartPositions[i], transform.position.y + 0.5f, transform.position.z);
			if((leftRight == 0 && (Physics.Raycast(startPosition, -transform.forward, 6))) || (leftRight == 1 && (Physics.Raycast(startPosition, transform.forward, 6))))
				return false;
		}
		
		//check if we're not going to drive on the wrong side of the road
		if(carSpawner.leftSameDirection != carSpawner.rightSameDirection && 
		((leftRight == 0 && transform.position.z > 0 && transform.position.z < 4) || 
		(leftRight == 1 && transform.position.z < 0 && transform.position.z > -4)))
			return false;
			
		return true;
	}
	
	//switch lines
	IEnumerator SwitchLine(bool right){
		//set movespeed and light based on the move direction
		movespeed = -2;
		GameObject light = lightLeft;
		
		if(right){
			movespeed = 2;
			light = lightRight;
		}
		
		//use the lights to indicate line switching
		while(true){
			yield return new WaitForSeconds(0.25f);
			light.SetActive(!light.activeSelf);
		}
	}
}
