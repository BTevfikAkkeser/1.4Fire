using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(RoadTransitions))]
public class roadManagerEditor : Editor{
	
	RoadTransitions roadTransitionsScript;
	ReorderableList transitionsList;
	
	Texture plus;
	Texture arrow;
	Texture start;
	Texture noSelection;
	Texture drop;
	Texture prefab;
	
	GameObject tempObject;
	
	List<string> environmentObjectNames = new List<string>();
	
	void OnEnable(){
	roadTransitionsScript = (target as RoadTransitions).gameObject.GetComponent<RoadTransitions>();
	
	plus = Resources.Load("plus") as Texture;
	arrow = Resources.Load("arrow") as Texture;
	start = Resources.Load("start") as Texture;
	noSelection = Resources.Load("no selection") as Texture;
	drop = Resources.Load("drop") as Texture;
	prefab = Resources.Load("prefab") as Texture;
	
	environmentObjectNames.Clear();
	if(GameObject.FindObjectOfType<ObjectPool>()){
		foreach(poolObject environmentObject in GameObject.FindObjectOfType<ObjectPool>().roadSideObjects){
			environmentObjectNames.Add(environmentObject.prefab.name);
		}
	}
	else{
		environmentObjectNames.Add("No object pool script found");
	}
	
	transitionsList = new ReorderableList(serializedObject, serializedObject.FindProperty("roadTransitions"), true, true, false, true);
	
	transitionsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
    var element = transitionsList.serializedProperty.GetArrayElementAtIndex(index);
	
	int labelWidth = 60;
	
	if(isActive){
	GUI.color = new Color(0.9f, 0.9f, 0.9f, 1);
	EditorGUI.DrawRect(new Rect(16, rect.y + EditorGUIUtility.singleLineHeight + 5, rect.width + 22, EditorGUIUtility.singleLineHeight * 6f + labelWidth + 5), new Color(0.8f, 0.85f, 0.9f, 1));	
	}
	
	GUI.color = new Color(0.8f, 0.8f, 0.8f, 1);
	EditorGUI.DrawRect(new Rect(16, rect.y + 10 + EditorGUIUtility.singleLineHeight * 7f + labelWidth, rect.width + 22, 1), new Color(0.8f, 0.85f, 0.9f, 1));	
	
	GUI.color = Color.white;
	
    EditorGUI.PropertyField(
        new Rect(rect.x + labelWidth * 2, rect.y + 2, 60, EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("minDelay"), GUIContent.none);
		
	EditorGUI.LabelField(new Rect(rect.x + labelWidth * 2 + 70, rect.y + 2, 10, EditorGUIUtility.singleLineHeight), "-", EditorStyles.boldLabel);
		
	EditorGUI.PropertyField(
		new Rect(rect.x + labelWidth * 2 + 90, rect.y + 2, rect.width - ((labelWidth * 2) - 2 + (labelWidth * 2 - 10)), EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("maxDelay"), GUIContent.none);
		
	EditorGUI.PropertyField(
        new Rect(rect.x + labelWidth * 2, rect.y + EditorGUIUtility.singleLineHeight + 7, rect.width - labelWidth * 2, EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("transitionRoadObject"), GUIContent.none);
		
	EditorGUI.PropertyField(
        new Rect(rect.x + labelWidth * 2, rect.y + EditorGUIUtility.singleLineHeight * 2 + 12, rect.width - labelWidth * 2, EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("newRoadObject"), GUIContent.none);

    EditorGUI.LabelField(new Rect(rect.x, rect.y + 2, labelWidth * 2 - 10, EditorGUIUtility.singleLineHeight), "Delay", EditorStyles.boldLabel);
	EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 7, labelWidth * 2 - 10, EditorGUIUtility.singleLineHeight), "Transition object");
	EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2 + 12, labelWidth * 2 - 10, EditorGUIUtility.singleLineHeight), "New road object");
	
	if(index != 0){
		if(roadTransitionsScript.roadTransitions[index - 1].newRoadObject){
			if(AssetPreview.GetAssetPreview(roadTransitionsScript.roadTransitions[index - 1].newRoadObject)){
				EditorGUI.DrawPreviewTexture(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 4 + 17, labelWidth, labelWidth), 
				AssetPreview.GetAssetPreview(roadTransitionsScript.roadTransitions[index - 1].newRoadObject));
			}
			else{
				EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 4 + 17, labelWidth, labelWidth), new GUIContent(prefab));
			}
		}
		else{
			EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 4 + 17, labelWidth, labelWidth), new GUIContent(noSelection));
		}
	}
	else{
		EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 4 + 17, labelWidth, labelWidth), new GUIContent(start));
	}
	
	EditorGUI.LabelField(new Rect(rect.width * 0.27f + labelWidth * 0.5f, rect.y + EditorGUIUtility.singleLineHeight * 4 + 17, labelWidth * 0.3f, labelWidth), new GUIContent(arrow));
	
	if(roadTransitionsScript.roadTransitions[index].transitionRoadObject){
		if(AssetPreview.GetAssetPreview(roadTransitionsScript.roadTransitions[index].transitionRoadObject)){
			EditorGUI.DrawPreviewTexture(new Rect(rect.width * 0.5f, rect.y + EditorGUIUtility.singleLineHeight * 4 + 17, labelWidth, labelWidth), 
			AssetPreview.GetAssetPreview(roadTransitionsScript.roadTransitions[index].transitionRoadObject));
		}
		else{
			EditorGUI.LabelField(new Rect(rect.width * 0.5f, rect.y + EditorGUIUtility.singleLineHeight * 4 + 17, labelWidth, labelWidth), new GUIContent(prefab));
		}
	}
	else{
		EditorGUI.LabelField(new Rect(rect.width * 0.5f, rect.y + EditorGUIUtility.singleLineHeight * 4 + 17, labelWidth, labelWidth), new GUIContent(noSelection));
	}
	
	EditorGUI.LabelField(new Rect(rect.width * 0.77f, rect.y + EditorGUIUtility.singleLineHeight * 4 + 17, labelWidth * 0.3f, labelWidth), new GUIContent(arrow));
	
	if(roadTransitionsScript.roadTransitions[index].newRoadObject){
		if(AssetPreview.GetAssetPreview(roadTransitionsScript.roadTransitions[index].newRoadObject)){
			EditorGUI.DrawPreviewTexture(new Rect(rect.width - (labelWidth * 0.5f), rect.y + EditorGUIUtility.singleLineHeight * 4 + 17, labelWidth, labelWidth), 
			AssetPreview.GetAssetPreview(roadTransitionsScript.roadTransitions[index].newRoadObject));
		}
		else{
			EditorGUI.LabelField(new Rect(rect.width - (labelWidth * 0.5f), rect.y + EditorGUIUtility.singleLineHeight * 4 + 17, labelWidth, labelWidth), new GUIContent(prefab));
		}
	}
	else{
		EditorGUI.LabelField(new Rect(rect.width - (labelWidth * 0.5f), rect.y + EditorGUIUtility.singleLineHeight * 4 + 17, labelWidth, labelWidth), new GUIContent(noSelection));
	}
	
	roadTransitionsScript.roadTransitions[index].environmentMask = EditorGUI.MaskField(
		new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 9 + 10, rect.width, EditorGUIUtility.singleLineHeight), 
		"Environment:", roadTransitionsScript.roadTransitions[index].environmentMask, environmentObjectNames.ToArray());
	
	};
	
	transitionsList.elementHeightCallback = (index) => { 
		return EditorGUIUtility.singleLineHeight * 7.5f + 70;
	};
	
	transitionsList.onRemoveCallback = (ReorderableList l) => {  
    if (EditorUtility.DisplayDialog("Remove road transition", 
        "Are you sure you want to remove this transition?", "Yes", "No")) {
        ReorderableList.defaultBehaviours.DoRemoveButton(l);
    }
	};
	
	transitionsList.drawHeaderCallback = (Rect rect) => {  
    EditorGUI.LabelField(rect, "Road transitions (" + roadTransitionsScript.roadTransitions.Count + ")");
	};
	}
	
	public override void OnInspectorGUI(){	
		if(!GameObject.FindObjectOfType<EnvironmentSpawner>())
			EditorGUILayout.HelpBox("No object pool script found. Please add one to use the environment filter.", MessageType.Error);
	
		GUILayout.Space(5);
		DrawDefaultInspector();
		roadTransitionsScript.startEnvironmentMask = EditorGUILayout.MaskField("Start environment:", roadTransitionsScript.startEnvironmentMask, environmentObjectNames.ToArray());
		GUILayout.Space(5);
		
		serializedObject.Update();
		transitionsList.DoLayoutList();
		serializedObject.ApplyModifiedProperties();
		
		GUILayout.Space(-10);
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(drop, GUILayout.Width(60), GUILayout.Height(40));
		if(tempObject != null){
			GUI.color = Color.green;
		}
		else{
			GUI.color = Color.white;
		}
		EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
		GUI.color = Color.black;
		EditorGUILayout.BeginHorizontal("Box");
		tempObject = EditorGUILayout.ObjectField(tempObject, typeof(GameObject), false, GUILayout.Height(20)) as GameObject;
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndHorizontal();
	
		GUI.color = new Color(0.7f, 1f, 0.7f, 1);
		if(GUILayout.Button(plus, GUILayout.Height(34), GUILayout.Width(34))){
			if(!roadTransitionsScript.cloneLast){
				roadTransitionsScript.roadTransitions.Add(new RoadTransition{ 
					minDelay = 0, 
					maxDelay = 0, 
					transitionRoadObject = null, 
					newRoadObject = null, 
					environmentMask = -1
				});
			}
			else{
				roadTransitionsScript.roadTransitions.Add(new RoadTransition{ 
					minDelay = roadTransitionsScript.roadTransitions[roadTransitionsScript.roadTransitions.Count - 1].minDelay, 
					maxDelay = roadTransitionsScript.roadTransitions[roadTransitionsScript.roadTransitions.Count - 1].maxDelay, 
					transitionRoadObject = roadTransitionsScript.roadTransitions[roadTransitionsScript.roadTransitions.Count - 1].transitionRoadObject, 
					newRoadObject = roadTransitionsScript.roadTransitions[roadTransitionsScript.roadTransitions.Count - 1].newRoadObject, 
					environmentMask = roadTransitionsScript.roadTransitions[roadTransitionsScript.roadTransitions.Count - 1].environmentMask
				});
			}
		}
		
		GUILayout.Space(40);
		EditorGUILayout.EndHorizontal();
		
		GUI.color = Color.white;
		roadTransitionsScript.cloneLast = EditorGUILayout.ToggleLeft("Clone last transition", roadTransitionsScript.cloneLast, GUILayout.Width(140));
		
		if(tempObject != null){
			roadTransitionsScript.roadTransitions.Add(new RoadTransition{ 
				minDelay = 0, 
				maxDelay = 0, 
				transitionRoadObject = tempObject, 
				newRoadObject = null, 
				environmentMask = -1
			});
			
			tempObject = null;
		}
		
		GUILayout.Space(10);
		
		Undo.RecordObject(roadTransitionsScript, "change in road transitions");
	}
}

[CustomEditor(typeof(ObjectPool))]
public class objectPoolEditor : Editor{
	
	//variables not visible in the inspector
	ObjectPool poolScript;
	
	AnimBool roadSide;
	AnimBool cars;
	
	void OnEnable(){
		//get target script and volume slider
		poolScript = (ObjectPool)target;
		
		roadSide = new AnimBool(false);
		roadSide.valueChanged.AddListener(Repaint);
		cars = new AnimBool(false);
		cars.valueChanged.AddListener(Repaint);
	}
	
	public override void OnInspectorGUI(){	
	GUILayout.Space(10);
	DisplayFoldout(roadSide, "Roadside objects");
	DisplayFoldout(cars, "Cars");
	
	poolScript.bridge = EditorGUILayout.ObjectField(poolScript.bridge, typeof(GameObject), false) as GameObject;
		
	//apply modifications
	serializedObject.ApplyModifiedProperties(); 
	//undo funtionality
	Undo.RecordObject(poolScript, "change in object pool");
	}
	
	void DisplayFoldout(AnimBool animBool, string title){
		GUI.color = new Color(0.5f, 0.8f, 1f, 0.4f);
		GUILayout.BeginVertical("Box");
		GUI.color = Color.white;
		
		if(animBool.target){
			if(GUILayout.Button(title, EditorStyles.boldLabel)){
				animBool.target = false;
			}
		}
		else{
			if(GUILayout.Button(title, EditorStyles.label)){
				animBool.target = true;
			}
		}
		
		if(EditorGUILayout.BeginFadeGroup(animBool.faded)){
			GUILayout.Space(5);
			content(title);
		}
		EditorGUILayout.EndFadeGroup();
		GUILayout.EndVertical();
	}
	
	void content(string title){
		List<poolObject> list = null;
		if(title == "Cars"){
			list = poolScript.cars;
		}
		else{
			list = poolScript.roadSideObjects;
		}
		
		for(int i = 0; i < list.Count; i++){
			GUILayout.BeginHorizontal();
			list[i].prefab = EditorGUILayout.ObjectField(list[i].prefab, typeof(GameObject), false) as GameObject;
			
			GUI.color = new Color(0.9f, 0.5f, 0.5f, 1f);
			if(GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(15)))
				list.RemoveAt(i);
			
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
		}
		
		GUILayout.Space(5);
		
		if(GUILayout.Button("New"))
			list.Add(new poolObject{});
		
		if(GUILayout.Button("Clear all") && EditorUtility.DisplayDialog("Clear " + title + "?", "Are you sure you want to clear all " + title + "?", "Yes", "No"))
			list.Clear();
	}
}
