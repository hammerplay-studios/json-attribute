using JsonFx.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Hammerplay.Utils.JsonAttribute
{

    [InitializeOnLoadAttribute]
    public class JsonAttributeWindow : EditorWindow
    {

        Dictionary<string, object> jsonDictionary = new Dictionary<string, object>();

        //public static JsonAttributeStorage jsonAttribStorage;

         static Source source;
         string onlinePath;
        [MenuItem("Window/JSON Attribute")]
         static void Init()
        {
            JsonAttributeWindow window = (JsonAttributeWindow)EditorWindow.GetWindow(typeof(JsonAttributeWindow));
            window.Show();
        }

        static JsonAttributeWindow()
        {
            EditorApplication.playModeStateChanged += LogPlayModeState;

        }

        static void LogPlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                EditorPrefs.SetInt("DataSource", (int)source);
         
            }

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
               source = (Source)EditorPrefs.GetInt("DataSource");
               EditorWindow.GetWindow(typeof(JsonAttributeWindow)).Repaint();
                if ((Source)EditorPrefs.GetInt("DataSource") == Source.Resources)
                {
                    ReadFromJson();
                }
            }
        }

        

       static Dictionary<string, object> temp = new Dictionary<string, object>();

        void OnGUI()
        {
           
                GUILayout.Label("JSON Attribute Settings", EditorStyles.boldLabel);
                source = (Source)EditorGUILayout.EnumPopup("Source: ", source);


                if (GUI.Button(new Rect(20, 70, 200, 20), "write"))
                {
                    WriteToJson();
                }

            if (source == Source.Online)
            {
                onlinePath = EditorGUILayout.TextField("URL", onlinePath);
            }


        }

        void WriteToJson()
        {
            Debug.Log("writing");
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
                            string key = attribute.ParameterPath;

                            if (!jsonDictionary.ContainsKey(key))
                            {
                                jsonDictionary.Add(key, objectFields[i].GetValue(mono));
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

        static void ReadFromJson()
        {
            MonoBehaviour[] sceneActive = FindObjectsOfType<MonoBehaviour>();

            Dictionary<string, object> resourceJSONDictionary = new JsonReader().Read(File.ReadAllText("Assets/Resources/attributes.json"), typeof(Dictionary<string, object>)) as Dictionary<string, object>;

            foreach (MonoBehaviour mono in sceneActive)
            {
                FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                for (int i = 0; i < objectFields.Length; i++)
                {
                    JsonAttribute attribute = Attribute.GetCustomAttribute(objectFields[i], typeof(JsonAttribute)) as JsonAttribute;
                    if (attribute != null)
                    {
                        //string key = attribute.ParameterPath + "-" + objectFields[i].FieldType;
                        string key = attribute.ParameterPath;

                        if (resourceJSONDictionary.ContainsKey(key))
                        {

                            temp.Clear();
                            temp.Add(key, resourceJSONDictionary[key]);
                            JsonUtility.FromJsonOverwrite(new JsonWriter().Write(temp), mono);

                        }
                    }

                }
            }
        }
    }

    public enum Source { Editor, Resources, Online}
}


        