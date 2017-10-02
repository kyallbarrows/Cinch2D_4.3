using UnityEngine;
using System.Collections;
using Cinch2D;

public class Joe : CinchSprite {
	public Vector2 FakeVelocity;
	
	public override void OnAwake()
	{
		InitFromImage("JoeDucreux", 100f);
	}
	
	public override void OnEnterFrame (float time) {
		SetPosition(X + FakeVelocity.x * Clock.DeltaTime, Y + FakeVelocity.y * Clock.DeltaTime);	
	}
}
