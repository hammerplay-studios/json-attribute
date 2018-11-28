using System;
using UnityEngine;
using System.Reflection;
using JsonFx;
using JsonFx.Json;
using System.Collections;
using System.Collections.Generic;

namespace Hammerplay.Utils.JsonAttribute {
	public class JsonAttributeParser : MonoBehaviour {

		[SerializeField]
		private MonoBehaviour gameManager;

		private void Awake() {
			MonoBehaviour[] sceneActive = FindObjectsOfType<MonoBehaviour>();
			jsonDictionary = new Dictionary<string, object>();
			foreach (MonoBehaviour mono in sceneActive) {
				FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				//FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
				for (int i = 0; i < objectFields.Length; i++) {
					JsonAttribute attribute = Attribute.GetCustomAttribute(objectFields[i], typeof(JsonAttribute)) as JsonAttribute;
					if (attribute != null) {
						/*Debug.Log(objectFields[i].Name + " - " + attribute.ParameterPath); // The name of the flagged variable.
						Debug.LogFormat(
							"DeclaringType: {0}, FieldType: {1}, MemberType {2}, ReflectedType {3}, Type {4}",
							objectFields[i].DeclaringType,
							objectFields[i].FieldType,
							objectFields[i].MemberType,
							objectFields[i].ReflectedType,
							objectFields[i].GetType()
							);*/


						if (!jsonDictionary.ContainsKey (attribute.ParameterPath)) {
							jsonDictionary.Add(attribute.ParameterPath, objectFields[i].GetValue(mono));
						} else {
							Debug.LogErrorFormat("{0} found already in JSON dictionary", attribute.ParameterPath);
						}

					}

				}
			}

			Debug.Log(new JsonWriter().Write(jsonDictionary));
		}

		private Dictionary<string, object> jsonDictionary = new Dictionary<string, object>();
	}
}
