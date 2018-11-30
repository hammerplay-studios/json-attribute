using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hammerplay.Utils.JsonAttribute;

public class WaveSystem : MonoBehaviour {

    [SerializeField]
    [Json("Waves")]
    private Wave[] waves;

	[SerializeField]
	
	private float someNumber;

	// Use this for initialization
	void Start () {

       
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
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
