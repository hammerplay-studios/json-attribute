using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JsonFx;
using JsonFx.Json;
using System;
using System.IO;

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

            if (GUI.Button(new Rect(20, 70, 200, 20), "write"))
            {

                MonoBehaviour[] sceneActive = FindObjectsOfType<MonoBehaviour>();
              
                foreach (MonoBehaviour mono in sceneActive)
                {
                    FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    //FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                    for (int i = 0; i < objectFields.Length; i++)
                    {
                        JsonAttribute attribute = Attribute.GetCustomAttribute(objectFields[i], typeof(JsonAttribute)) as JsonAttribute;
                        if (attribute != null)
                        {
                        
                            //jsonDictionary.Clear();

                            if (!jsonDictionary.ContainsKey(attribute.ParameterPath))
                            {
                                jsonDictionary.Add(attribute.ParameterPath,objectFields[i].GetValue(mono));
                                Debug.Log("add");
                            }
                            else
                            {
                                Debug.LogErrorFormat("{0} found already in JSON dictionary", attribute.ParameterPath);
                            }

                        }

                    }
                }

                File.WriteAllText("Assets/Resources/attributes.json", new JsonWriter().Write(jsonDictionary));
               


            }


            if (GUI.Button(new Rect(20, 100, 200, 20), "read"))
            {
                /*MonoBehaviour[] sceneActive = FindObjectsOfType<MonoBehaviour>();
                JsonUtility.FromJsonOverwrite(File.ReadAllText("Assets/Resources/attributes.json"), sceneActive[0]);
                Debug.Log(waveSys[0]);*/

                MonoBehaviour[] sceneActive = FindObjectsOfType<MonoBehaviour>();

               //Dictionary<string,object> resourceJSONDictionary = (Dictionary<string,object>) new JsonReader().Read(File.ReadAllText("Assets/Resources/attributes.json"));
                Dictionary<string, Wave> resourceJSONDictionary = new JsonReader().Read(File.ReadAllText("Assets/Resources/attributes.json"),typeof(Dictionary<string, Wave>)) as Dictionary<string, Wave>;

                foreach (MonoBehaviour mono in sceneActive)
                    {
                        FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                       
                        for (int i = 0; i < objectFields.Length; i++)
                        {
                            JsonAttribute attribute = Attribute.GetCustomAttribute(objectFields[i], typeof(JsonAttribute)) as JsonAttribute;
                            if (attribute != null)

                            {
                           
                            if (resourceJSONDictionary.ContainsKey(attribute.ParameterPath))
                                {
                                   objectFields[i].SetValue(mono, resourceJSONDictionary[attribute.ParameterPath]);

                                }
                            else
                                {
                                    Debug.LogErrorFormat("{0} not found in JSON dictionary", attribute.ParameterPath);
                                }

                            }


                    }

                }
            }
        }


    }
}
