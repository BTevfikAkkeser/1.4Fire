using UnityEngine;
using UnityEditor;

public class InstantaiteVehicle : EditorWindow
{

    public string[] BodyKitOptions = new string[] { "BodyKit 1", "BodyKit 2", "BodyKit 3",};
    public string[] TyreOptions = new string[] { "Blur Tyres", "Solid Tyre 1", "Solid Tyre 2",};


    public int index = 0;
    public int indexsecond = 0;

    [MenuItem("Generate Vehicle/New Drift Vehicle")]
    public static void GenerateDriftCar()
    {
        GetWindow<InstantaiteVehicle>("Generate Drift Car");
    }

    void OnGUI()
    {
        GUI.color = Color.white;
        GUILayout.Label("Generate a drift car by mixing and matching\n\n", EditorStyles.boldLabel);
        GUILayout.Space(125);
        Texture tex1 = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Mobile Drift Physics/Editor/Editor Icons/BodyKit1.png", typeof(Texture));
        Texture tex2 = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Mobile Drift Physics/Editor/Editor Icons/BodyKit2.png", typeof(Texture));
        Texture tex3 = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Mobile Drift Physics/Editor/Editor Icons/BodyKit3.png", typeof(Texture));

        EditorGUI.LabelField(new Rect(25, 40, 100, 15), "BodyKit 1:");
        EditorGUI.DrawPreviewTexture(new Rect(25, 60, 100, 100), tex1);

        EditorGUI.LabelField(new Rect(25 + 120, 40, 100, 15), "BodyKit 2:");
        EditorGUI.DrawPreviewTexture(new Rect(25 + 120, 60, 100, 100), tex2);

        EditorGUI.LabelField(new Rect(25 + 240, 40, 100, 15), "BodyKit 3:");
        EditorGUI.DrawPreviewTexture(new Rect(25 + 240, 60, 100, 100), tex3);


        index = EditorGUILayout.Popup(index, BodyKitOptions);

        GameObject prefab1 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Mobile Drift Physics/Prefabs/DriftVehicle1.prefab", typeof(GameObject));
        GameObject prefab2 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Mobile Drift Physics/Prefabs/DriftVehicle2.prefab", typeof(GameObject));
        GameObject prefab3 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Mobile Drift Physics/Prefabs/DriftVehicle3.prefab", typeof(GameObject));

        GUILayout.Space(140);
        Texture tex6 = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Mobile Drift Physics/Editor/Editor Icons/Tyre1.png", typeof(Texture));
        Texture tex7 = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Mobile Drift Physics/Editor/Editor Icons/Tyre2.png", typeof(Texture));
        Texture tex8 = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Mobile Drift Physics/Editor/Editor Icons/Tyre3.png", typeof(Texture));

        EditorGUI.LabelField(new Rect(25, 40 + 160, 100, 15), "Blur Tyres");
        EditorGUI.DrawPreviewTexture(new Rect(25, 60 + 160, 100, 100), tex6);

        EditorGUI.LabelField(new Rect(25 + 120, 40 + 160, 100, 15), "Solid Tyre 1:");
        EditorGUI.DrawPreviewTexture(new Rect(25 + 120, 60 + 160, 100, 100), tex7);

        EditorGUI.LabelField(new Rect(25 + 240, 40 + 160, 100, 15), "Solid Tyre 2:");
        EditorGUI.DrawPreviewTexture(new Rect(25 + 240, 60 + 160, 100, 100), tex8);


        indexsecond = EditorGUILayout.Popup(indexsecond, TyreOptions);

        GameObject tyreModel1 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Mobile Drift Physics/Models/BlurWheel.fbx", typeof(GameObject));
        GameObject tyreModel2 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Mobile Drift Physics/Models/ClassicWheel.fbx", typeof(GameObject));
        GameObject tyreModel3 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Mobile Drift Physics/Models/DriftWheel.fbx", typeof(GameObject));

        if (GUILayout.Button("Generate Drift Car"))
        {
            switch (index)
            {
                case 0:
                    switch (indexsecond)
                    {
                        case 0:
                            CarGen(prefab1, tyreModel1, "DriftWheel"); //Drift Wheel is what to search for to replace
                            break;
                        case 1:
                            CarGen(prefab1, tyreModel2, "DriftWheel");
                            break;
                        case 2:
                            CarGen(prefab1, tyreModel3, "DriftWheel");
                            break;
                    }
                    break;

                case 1:
                    switch (indexsecond)
                    {
                        case 0:
                            CarGen(prefab2, tyreModel1, "DriftWheel");
                            break;
                        case 1:
                            CarGen(prefab2, tyreModel2, "DriftWheel");
                            break;
                        case 2:
                            CarGen(prefab2, tyreModel3, "DriftWheel");
                            break;
                    }
                    break;

                case 2:
                    switch (indexsecond)
                    {
                        case 0:
                            CarGen(prefab3, tyreModel1, "DriftWheel");
                            break;
                        case 1:
                            CarGen(prefab3, tyreModel2, "DriftWheel");
                            break;
                        case 2:
                            CarGen(prefab3, tyreModel3, "DriftWheel");
                            break;
                    }
                    break;
            }


        }
        void CarGen(GameObject prefab, GameObject tyreModel, string search)
        {
            prefab.name = "Custom Drift Vehicle";
            GameObject clone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            Transform[] allChildren = clone.GetComponentsInChildren<Transform>();
            var i = 0;
            foreach (Transform eachChild in allChildren)
            {
                if (eachChild.name == search)
                {
                    var tyre = Instantiate(tyreModel, Vector3.zero, Quaternion.identity);
                    tyre.transform.parent = eachChild.parent;
                    tyre.transform.position = new Vector3(eachChild.transform.position.x, eachChild.transform.position.y, eachChild.transform.position.z);
                    tyre.transform.eulerAngles = new Vector3(eachChild.transform.rotation.x, eachChild.transform.rotation.y, eachChild.rotation.z);
                    tyre.transform.localScale = new Vector3(eachChild.transform.localScale.x, eachChild.transform.localScale.y, eachChild.transform.localScale.z);
                    clone.GetComponent<DriftController>().m_WheelMeshes[i] = tyre.gameObject;
                    i++;
                }
            }
            foreach (Transform eachChild in allChildren)
            {
                if (eachChild.name == search)
                {
                    DestroyImmediate(eachChild.gameObject);
                }
            }

        }
    }

}