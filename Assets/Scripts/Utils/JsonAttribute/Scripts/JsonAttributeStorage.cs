using UnityEngine;

public class JsonAttributeStorage : ScriptableObject {

	public Source source;

	public string onlinePath;


	public enum Source { Editor, Resources, Online }

}
