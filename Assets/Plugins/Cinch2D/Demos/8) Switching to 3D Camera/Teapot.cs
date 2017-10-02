using UnityEngine;
using System.Collections;
using Cinch2D;

public class Teapot : MonoBehaviour {
	
	public void Update()
	{
		gameObject.transform.rotation = Quaternion.AngleAxis((Time.time * 20f) % 360, Vector3.up);
	}
}
