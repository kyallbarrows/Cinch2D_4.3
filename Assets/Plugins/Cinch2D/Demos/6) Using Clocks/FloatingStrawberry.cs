using UnityEngine;
using System.Collections;
using Cinch2D;

public class FloatingStrawberry : CinchSprite {

	private float _velocityX;
	private float _velocityY;
	
	public override void OnAwake()
	{
		InitFromImage("Cinch2D/Strawberry", 1000f);
		_velocityX = Random.Range (-1f, 1f);
		_velocityY = Random.Range (-1f, 1f);
	}
	
	//this will only get called if Clock is running
	public override void OnEnterFrame(float time)
	{
		X += _velocityX * Clock.DeltaTime;
		Y += _velocityY * Clock.DeltaTime;
	}
}
