using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Purchaser))]
public class PurchaserInspector : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        
        if (GUILayout.Button("Edit In-app settings", GUILayout.MinHeight(30)))
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Resources/Data/Products/ProductsData.xml");
        }

        EditorGUILayout.Space();
        GUILayout.TextField("Product data path:\nAssets/Resources/Data/Products/ProductsData.xml", EditorStyles.boldLabel);
    }
}
