using UnityEngine;
using System.Collections;
using Cinch2D;

public class Watermelon : CinchSprite {

	public override void OnAwake()
	{
		InitFromImage("Cinch2D/Watermelon", 256f);
	}
}
