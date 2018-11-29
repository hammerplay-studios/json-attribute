using JsonFx.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Hammerplay.Utils.JsonAttribute {
	public class JsonAttributeWindow : EditorWindow {

		Dictionary<string, object> jsonDictionary = new Dictionary<string, object>();

		[MenuItem("Window/JSON Attribute")]
		static void Init() {
			JsonAttributeWindow window = (JsonAttributeWindow)EditorWindow.GetWindow(typeof(JsonAttributeWindow));
			window.Show();

		}

		private static JsonAttributeStorage jsonAttribStorage;

		void OnGUI() {
			GUILayout.Label("JSON Attribute Settings", EditorStyles.boldLabel);

			if (jsonAttribStorage == null) {
				Debug.Log("Load from resources");
				jsonAttribStorage = Resources.Load<JsonAttributeStorage>("JsonAttribStorage");
			}

			if (jsonAttribStorage == null) {

				if (GUILayout.Button("Setup")) {
					jsonAttribStorage = ScriptableObject.CreateInstance<JsonAttributeStorage>();

					AssetDatabase.CreateAsset(jsonAttribStorage, "Assets/Resources/JsonAttribStorage.asset");
					AssetDatabase.SaveAssets();

					EditorUtility.FocusProjectWindow();
					//AssetDatabase.CreateAsset ()
				}

				return;
			}


			jsonAttribStorage.source = (JsonAttributeStorage.Source)EditorGUILayout.EnumPopup("Source: ", jsonAttribStorage.source);

			if (jsonAttribStorage.source == JsonAttributeStorage.Source.Online) {
				jsonAttribStorage.onlinePath = EditorGUILayout.TextField("URL", jsonAttribStorage.onlinePath);
			}

			if (GUI.Button(new Rect(20, 70, 200, 20), "write")) {

				MonoBehaviour[] sceneActive = FindObjectsOfType<MonoBehaviour>();

				foreach (MonoBehaviour mono in sceneActive) {
					FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
					//FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
					for (int i = 0; i < objectFields.Length; i++) {
						JsonAttribute attribute = Attribute.GetCustomAttribute(objectFields[i], typeof(JsonAttribute)) as JsonAttribute;
						if (attribute != null) {

							Debug.Log(objectFields[i].Name + " - " + attribute.ParameterPath); // The name of the flagged variable.
							Debug.LogFormat(
								"DeclaringType: {0}, FieldType: {1}, MemberType {2}, ReflectedType {3}, Type {4}",
								objectFields[i].DeclaringType,
								objectFields[i].FieldType,
								objectFields[i].MemberType,
								objectFields[i].ReflectedType,
								objectFields[i].GetType()
								);
							//string key = attribute.ParameterPath + "-" + objectFields[i].FieldType;
							string key = attribute.ParameterPath;

							if (!jsonDictionary.ContainsKey(key)) {
								jsonDictionary.Add(key, objectFields[i].GetValue(mono));
								Debug.Log("add");
							}
							else {
								Debug.LogErrorFormat("{0} found already in JSON dictionary", attribute.ParameterPath);
							}

						}

					}
				}

				File.WriteAllText("Assets/Resources/attributes.json", new JsonWriter().Write(jsonDictionary));



			}


			if (GUI.Button(new Rect(20, 100, 200, 20), "read")) {
				/*MonoBehaviour[] sceneActive = FindObjectsOfType<MonoBehaviour>();
                JsonUtility.FromJsonOverwrite(File.ReadAllText("Assets/Resources/attributes.json"), sceneActive[0]);
                Debug.Log(waveSys[0]);*/

				MonoBehaviour[] sceneActive = FindObjectsOfType<MonoBehaviour>();

				//Dictionary<string,object> resourceJSONDictionary = (Dictionary<string,object>) new JsonReader().Read(File.ReadAllText("Assets/Resources/attributes.json"));
				Dictionary<string, object> resourceJSONDictionary = new JsonReader().Read(File.ReadAllText("Assets/Resources/attributes.json"), typeof(Dictionary<string, object>)) as Dictionary <string, object>;

				foreach (MonoBehaviour mono in sceneActive) {
					FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

					for (int i = 0; i < objectFields.Length; i++) {
						JsonAttribute attribute = Attribute.GetCustomAttribute(objectFields[i], typeof(JsonAttribute)) as JsonAttribute;
						if (attribute != null) {
							//string key = attribute.ParameterPath + "-" + objectFields[i].FieldType;
							string key = attribute.ParameterPath;
							Type t = Type.GetType(objectFields[i].FieldType.ToString());

							//Debug.Log(objectFields[i].FieldType);
							if (resourceJSONDictionary.ContainsKey(key)) {
								//Debug.Log(resourceJSONDictionary[attribute.ParameterPath]);
								//var waves = (Wave)resourceJSONDictionary[attribute.ParameterPath];
								Debug.Log(resourceJSONDictionary[attribute.ParameterPath]);
								Debug.Log(resourceJSONDictionary[attribute.ParameterPath] as Wave);
								/*foreach (var item in resourceJSONDictionary[attribute.ParameterPath] as Dictionary <string, object>) {
									Debug.Log(item);
								}*/
								//Debug.Log(Activator.CreateInstance(objectFields[i].FieldType));
								//Debug.Log("Derp: " + Convert.ChangeType(resourceJSONDictionary[attribute.ParameterPath], objectFields[i].FieldType));

								/*objectFields[i].SetValue(mono, 
									Convert.ChangeType (resourceJSONDictionary[attribute.ParameterPath], objectFields[i].FieldType));*/

							}
							else {
								Debug.LogErrorFormat("{0} not found in JSON dictionary", attribute.ParameterPath);
							}

						}


					}

				}
			}

			
		}

		private object GetInstanceDerp(string strFullyQualifiedName) {
			Type t = Type.GetType(strFullyQualifiedName);
			return Activator.CreateInstance(t);
		}


	}
}
