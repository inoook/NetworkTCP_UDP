using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public enum PrefType
{
	INT, FLOAT, STRING
}
public class Pref
{
	public string key;
	public PrefType type;
	public object v;
	
	public Pref(string key_, PrefType type_, object v_){
		key = key_;
		type = type_;
		v = v_;
	}
}

public static class PlayerPrefsExtra  {

	public static string fileName = "/savedData.txt";
	
	private static List<Pref> keys = new List<Pref>();
	
	private static List<MonoBehaviour> managedComponents;
	
	public static void AddManagedComponent(MonoBehaviour monoBehaviour)
	{
		if(managedComponents == null){
			managedComponents = new List<MonoBehaviour>();
		}
		managedComponents.Add(monoBehaviour);
	}
	
	public static void SaveSettingManagedComponent()
	{
		for(int i = 0; i < managedComponents.Count; i++){
			MonoBehaviour mono = managedComponents[i];
			mono.SendMessage("SaveSetting");
		}
	}
	
	// original method
	public static void SetInt(string key, int v)
	{
		PlayerPrefs.SetInt(key, v);
	}
	public static int GetInt(string key)
	{
		int v = PlayerPrefs.GetInt(key);
		keys.Add(new Pref(key, PrefType.INT, v));
		return v;
	}
	
	public static void SetFloat(string key, float v)
	{
		PlayerPrefs.SetFloat(key, v);
	}
	public static float GetFloat(string key)
	{
		float v = PlayerPrefs.GetFloat(key);
		keys.Add(new Pref(key, PrefType.FLOAT, v));
		return v;
	}
	
	public static void SetString(string key, string str)
	{
		PlayerPrefs.SetString(key, str);
	}
	public static string GetString(string key)
	{
		string v = PlayerPrefs.GetString(key);
		keys.Add(new Pref(key, PrefType.STRING, v));
		return v;
	}
	
	public static bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(key);
	}
	
	public static void DeleteKey(string key)
	{
		keys.Clear();
		PlayerPrefs.DeleteKey(key);
	}
	
	public static string[] GetAllKeys()
	{
		Pref[] prefs = keys.ToArray();
		string[] keyStrs = new string[prefs.Length];
		for(int i = 0; i < prefs.Length; i++){
			keyStrs[i] = prefs[i].key;
		}
		return keyStrs;
	}
	
	public static void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}

	public static int GetKeyCount()
	{
		return keys.Count;
	}
	
	public static string GetAllKeyAndValues()
	{
		Pref[] prefs = keys.ToArray();
		//ArrayList valuesList = new ArrayList();
		
		string str = "";
		for(int i = 0; i < prefs.Length; i++){
			Pref p = prefs[i];
			string key = p.key;
			PrefType type = p.type;
			object v = p.v;
			/*
			if(type == PrefType.INT){
				valuesList.Add(PlayerPrefs.GetInt(key));
			}else if(type == PrefType.FLOAT){
				valuesList.Add(PlayerPrefs.GetFloat(key));
			}else if(type == PrefType.STRING){
				
			}else{
				Debug.LogWarning("noKeyType: "+key);
			}
			*/
			str += (int)type+","+key +","+v.ToString() + "\n";
		}
		return str;
	}

	public static void DeleteFile()
	{
		string path = Application.dataPath+fileName;
		System.IO.FileInfo fi = new FileInfo (path);
		if (fi.Exists) {
			fi.Delete();
		}
	}
	public static void WriteToFile()
	{
		PlayerPrefs.Save();
		
		string str = GetAllKeyAndValues();
		Debug.Log("WriteToFile");
		Debug.Log(str);
		byte[] bytes = Encoding.UTF8.GetBytes(str);
		string path = Application.dataPath+fileName;
		File.WriteAllBytes(path, bytes);
	}

	// load from File to PlayerPrefs
	public static void LoadFromFile ()
	{
		string path = Application.dataPath+fileName;
		Debug.LogWarning ("LoadFromFile: " + path);
		try {
			using (System.IO.StreamReader sr = new StreamReader (path, Encoding.UTF8)) {

				string str = sr.ReadToEnd ();
				Debug.LogWarning (str);
				string[] splitStr = str.Split ('\n');
		
//				keys = new List<Pref> ();
				for (int i = 0; i < splitStr.Length; i++) {
					string[] prefLine = splitStr [i].Split (',');
					if (prefLine.Length > 2) {
						int index = int.Parse (prefLine [0]);
						PrefType type = (PrefType)(Enum.GetValues (typeof(PrefType)).GetValue (index));
						string key = prefLine [1];
						object v = prefLine [2];
						Pref p = new Pref (key, type, v);
				
//						keys.Add (p);
				
						if (type == PrefType.INT) {
							PlayerPrefs.SetInt (key, System.Convert.ToInt32 (v));
						} else if (type == PrefType.FLOAT) {
							PlayerPrefs.SetFloat (key, System.Convert.ToSingle (v));
						} else if (type == PrefType.STRING) {
							PlayerPrefs.SetString (key, (string)v);
						} else {
							Debug.LogWarning ("noKeyType: " + key);
						}
					}
				}

			}
		} catch {
			Debug.LogWarning("No File");
		}
	}

	public static IEnumerator LoadFromFile__(string path)
	{
		Debug.LogWarning("LoadFromFile: "+ path);
		WWW wwwFile = new WWW(path);
		yield return wwwFile;
		
		if(wwwFile.error != null){
			Debug.LogWarning("www Error: "+ path);
			yield return null;
		}else{
			//Debug.Log(wwwFile.text);
		}
		string str = wwwFile.text;
		string[] splitStr = str.Split('\n');
		
		keys = new List<Pref>();
		for(int i = 0; i < splitStr.Length; i++){
			string[] prefLine = splitStr[i].Split(',');
			if(prefLine.Length > 2){
				int index = int.Parse(prefLine[0]);
				PrefType type = (PrefType)(Enum.GetValues(typeof(PrefType)).GetValue(index));
				string key = prefLine[1];
				object v = prefLine[2];
				Pref p = new Pref(key, type,v);
				
				keys.Add(p);
				
				if(type == PrefType.INT){
					PlayerPrefs.SetInt(key, System.Convert.ToInt32(v));
				}else if(type == PrefType.FLOAT){
					PlayerPrefs.SetFloat(key, System.Convert.ToSingle(v));
				}else if(type == PrefType.STRING){
					PlayerPrefs.SetString(key, (string)v);
				}else{
					Debug.LogWarning("noKeyType: "+key);
				}
			}
		}
	}
	// helper
	
	
	// bool
	public static void SetBool(string key, bool b)
	{
		PlayerPrefsExtra.SetInt(key, b ? 1 : 0);
	}
	public static bool GetBool(string key)
	{
		int v = PlayerPrefsExtra.GetInt(key);
		return (v == 1) ? true : false;
	}
	public static bool HasKeyBool(string key)
	{
		return PlayerPrefsExtra.HasKey(key);
	}
	
	// Vector3
	public static void SetVector3(string name, Vector3 vec)
	{
		PlayerPrefsExtra.SetFloat(name+"_x", vec.x);
		PlayerPrefsExtra.SetFloat(name+"_y", vec.y);
		PlayerPrefsExtra.SetFloat(name+"_z", vec.z);
	}
	
	public static Vector3 GetVector3(string name)
	{
		Vector3 vec = new Vector3();
		vec.x = PlayerPrefsExtra.GetFloat(name+"_x");
		vec.y = PlayerPrefsExtra.GetFloat(name+"_y");
		vec.z = PlayerPrefsExtra.GetFloat(name+"_z");
		
		return vec;
	}
	
	public static bool HasKeyVector3(string name)
	{
		return PlayerPrefsExtra.HasKey(name+"_x");
	}
	
	public static void DeleteKeyVector3(string name)
	{
		PlayerPrefsExtra.DeleteKey(name+"_x");
		PlayerPrefsExtra.DeleteKey(name+"_y");
		PlayerPrefsExtra.DeleteKey(name+"_z");
	}
	
	// Vector2
	public static void SetVector2(string name, Vector2 vec)
	{
		PlayerPrefsExtra.SetFloat(name+"_x", vec.x);
		PlayerPrefsExtra.SetFloat(name+"_y", vec.y);
	}
	
	public static Vector3 GetVector2(string name)
	{
		Vector2 vec = new Vector2();
		vec.x = PlayerPrefsExtra.GetFloat(name+"_x");
		vec.y = PlayerPrefsExtra.GetFloat(name+"_y");
		
		return vec;
	}
	public static bool HasKeyVector2(string name)
	{
		return PlayerPrefsExtra.HasKey(name+"_x");
	}
	public static void DeleteKeyVector2(string name)
	{
		PlayerPrefsExtra.DeleteKey(name+"_x");
		PlayerPrefsExtra.DeleteKey(name+"_y");
	}
	
	// Rect
	public static void SetRect(string name, Rect rect)
	{
		PlayerPrefsExtra.SetFloat(name+"_x", rect.x);
		PlayerPrefsExtra.SetFloat(name+"_y", rect.y);
		PlayerPrefsExtra.SetFloat(name+"_width", rect.width);
		PlayerPrefsExtra.SetFloat(name+"_height", rect.height);
	}
	public static Rect GetRect(string name)
	{
		Rect rect = new Rect();
		rect.x = PlayerPrefsExtra.GetFloat(name+"_x");
		rect.y = PlayerPrefsExtra.GetFloat(name+"_y");
		rect.width = PlayerPrefsExtra.GetFloat(name+"_width");
		rect.height = PlayerPrefsExtra.GetFloat(name+"_height");
		
		return rect;
	}
	public static bool HasKeyRect(string name)
	{
		return PlayerPrefsExtra.HasKey(name+"_x");
	}
	
	// Color
	public static void SetColor(string name, Color color)
	{
		PlayerPrefsExtra.SetFloat(name+"_r", color.r);
		PlayerPrefsExtra.SetFloat(name+"_g", color.g);
		PlayerPrefsExtra.SetFloat(name+"_b", color.b);
		PlayerPrefsExtra.SetFloat(name+"_a", color.a);
	}
	public static Color GetColor(string name)
	{
		Color color = new Color();
		color.r = PlayerPrefsExtra.GetFloat(name+"_r");
		color.g = PlayerPrefsExtra.GetFloat(name+"_g");
		color.b = PlayerPrefsExtra.GetFloat(name+"_b");
		color.a = PlayerPrefsExtra.GetFloat(name+"_a");
		
		return color;
	}
	public static bool HasKeyColor(string name)
	{
		return PlayerPrefsExtra.HasKey(name+"_r");
	}
	
	// SelectionEnum
	public static void SetSelectionEnum(string key, object v, System.Type enumType)
	{
		int id = (int)v;
		string t_key = enumType.ToString() + key;
		PlayerPrefsExtra.SetInt(t_key, id);
	}
	public static object GetSelectionEnum(string key, System.Type enumType)
	{
		string t_key = enumType.ToString() + key;
		int modeId = PlayerPrefsExtra.GetInt(t_key);
		return ( Enum.GetValues( enumType ) ).GetValue(modeId);
	}
	
	public static bool HasKeySelectionEnum(string key, System.Type enumType)
	{
		string t_key = enumType.ToString() + key;
		return PlayerPrefsExtra.HasKey(t_key);
	}
	public static void DeleteSelectionEnum(string key, System.Type enumType)
	{
		string t_key = enumType.ToString() + key;
		PlayerPrefsExtra.DeleteKey(t_key);
		
	}
}
