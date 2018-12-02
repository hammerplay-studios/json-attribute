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
                if (source == Source.Resources)
                {
                    Debug.Log("resources");
                    ReadFromJson();
                }
            }

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
               source = (Source)EditorPrefs.GetInt("DataSource");
            }
        }

        

       static Dictionary<string, object> temp = new Dictionary<string, object>();
        void OnGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                GUILayout.Label("JSON Attribute Settings", EditorStyles.boldLabel);
                source = (Source)EditorGUILayout.EnumPopup("Source: ", source);


                if (GUI.Button(new Rect(20, 70, 200, 20), "write"))
                {
                    WriteToJson();
                }
            }

            


                /*if (jsonAttribStorage.source == JsonAttributeStorage.Source.Online)
                {
                    jsonAttribStorage.onlinePath = EditorGUILayout.TextField("URL", jsonAttribStorage.onlinePath);
                }*/

                /* if (GUI.Button(new Rect(20, 100, 200, 20), "read"))
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

                                    if (!(resourceJSONDictionary[key] is Dictionary<string, object>) && !(resourceJSONDictionary[key] is Array))
                                    {
                                        Debug.Log(resourceJSONDictionary[key]);
                                        objectFields[i].SetValue(mono, resourceJSONDictionary[key]);
                                    }
                                    else if (resourceJSONDictionary[key] is Dictionary<string, object>)
                                    {
                                        objectFields[i].SetValue(mono, JsonUtility.FromJson(new JsonWriter().Write(resourceJSONDictionary[key]), objectFields[i].FieldType));
                                    }
                                    else
                                    {
                                        string jsonString = "{\"Items\": " + new JsonWriter().Write(resourceJSONDictionary[attribute.ParameterPath]) + "}";
                                        Debug.Log(jsonString);
                                        object objectReference;                                                                     

                                        if (objectFields[i].FieldType.IsArray)
                                        {
                                            MethodInfo method = typeof(JsonHelper).GetMethod("FromJson");
                                            Debug.Log(objectFields[i].FieldType.GetElementType());
                                            MethodInfo generic = method.MakeGenericMethod(objectFields[i].FieldType.GetElementType());
                                            objectReference = (generic.Invoke(method, new object[] { jsonString }));

                                        }
                                        else
                                        {
                                            MethodInfo method = typeof(JsonHelper).GetMethod("FromJson");
                                            MethodInfo generic = method.MakeGenericMethod(objectFields[i].FieldType.GetGenericArguments()[0]);
                                            objectReference = (generic.Invoke(method, new object[] { jsonString }));


                                            MethodInfo arrayToListMethod = typeof(JsonAttributeWindow).GetMethod("ArrayToList");
                                            MethodInfo arrayToListGeneric = arrayToListMethod.MakeGenericMethod(objectFields[i].FieldType.GetGenericArguments()[0]);
                                            objectReference = (arrayToListGeneric.Invoke(arrayToListMethod, new object[] { objectReference }));

                                        }                                                                   

                                           objectFields[i].SetValue(mono, Convert.ChangeType(objectReference, objectFields[i].FieldType));
                                    }


                                }
                                else
                                {
                                    Debug.LogErrorFormat("{0} not found in JSON dictionary", attribute.ParameterPath);
                                }


                            }*/


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

                            /*Debug.Log(objectFields[i].Name + " - " + attribute.ParameterPath); // The name of the flagged variable.
                            Debug.LogFormat(
                                "DeclaringType: {0}, FieldType: {1}, MemberType {2}, ReflectedType {3}, Type {4}",
                                objectFields[i].DeclaringType,
                                objectFields[i].FieldType,
                                objectFields[i].MemberType,
                                objectFields[i].ReflectedType,
                                objectFields[i].GetType()
                                );*/
                            //string key = attribute.ParameterPath + "-" + objectFields[i].FieldType;
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


        
        
        

       /* private object GetInstanceDerp(string strFullyQualifiedName)
        {
            Type t = Type.GetType(strFullyQualifiedName);
            return Activator.CreateInstance(t);
        }

        private T GetObject<T>(Dictionary<string, object> dict)
        {
            Type type = typeof(T);
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                Debug.LogFormat("key: {0}, value: {1}", kv.Key, kv.Value);

                Debug.Log(type.GetField(kv.Key));
                type.GetField(kv.Key).SetValue(obj, kv.Value);
            }
            return (T)obj;
        }

        public static List<T> ArrayToList<T>(T[] array)
        {
            List<T> list = new List<T>();
            for (int j = 0; j < array.Length; j++)
            {
                list.Add(array[j]);
            }

            return list;

        }
    }

    public static class JsonHelper
    {
        
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }*/
    