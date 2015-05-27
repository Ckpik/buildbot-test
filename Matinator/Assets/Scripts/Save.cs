using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;


public class Save : ScriptableObject
{
	
    public List<Key> test;
	
	private string assetPathAndName;
	static string extension = ".asset";
    static string newExtension = ".matinat";
	
	public void Init(List<Key> _test)
	{
		test = _test;
	}
	
	public void CreateAsset<T> (Save data) where T : ScriptableObject
	{
		//T asset = ScriptableObject.CreateInstance<T> ();
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "";
		} 
		else if (Path.GetExtension (path) != "") 
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
		string file = EditorUtility.SaveFilePanel("Save in .asset", "assets", "", "asset");

		assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(file);
		Debug.Log(assetPathAndName);
 
		AssetDatabase.CreateAsset (data, assetPathAndName);
 
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = data;
	}
	
	public static string ConvertToInternalPath(string asset)
    {
       string left = asset.Substring(0, asset.Length - extension.Length);
       return left + newExtension;
    }
	
	public void LoadAsset()
	{

		Save data;
		data = AssetDatabase.LoadAssetAtPath(assetPathAndName, typeof(Save)) as Save;
		bool loaded = (data != null);
 
     	if(!loaded)
		{
			data = ScriptableObject.CreateInstance<Save>();	
		}
	
		if(!loaded)
		{
			AssetDatabase.CreateAsset(data, assetPathAndName);
		}
      	AssetDatabase.SaveAssets();
	}
}
