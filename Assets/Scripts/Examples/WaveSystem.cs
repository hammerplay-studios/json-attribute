using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Hammerplay.Utils.JsonAttribute;

public class WaveSystem : MonoBehaviour {

    [SerializeField]
    [Json("waves")]
    private Wave[] waves;

	[SerializeField]
	
	private float someNumber;
    
}



[System.Serializable]
public class Wave {

    public float interval;

	public SomeClass[] someClass;
	//public float[] enemies;

	public enum EnemyType { Human, Bike, Car }
}

[System.Serializable]
public class SomeClass {
	public int superNumber = 39;
}
