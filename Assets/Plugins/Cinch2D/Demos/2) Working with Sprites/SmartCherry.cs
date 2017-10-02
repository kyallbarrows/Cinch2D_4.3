using UnityEngine;
using System.Collections;
using Cinch2D;

public class SmartCherry : CinchSprite {
	
	//called whenever Awake is called.  
	//You could actually use Awake(), but you MUST call base.Awake() on the first line, and since we were always forgetting to during the development process, we added this.
	public override void OnAwake()
	{
		InitFromImage("Cinch2D/Cherries", 256);
	}
	
	//called whenver Update would be.  Unlike Update, however, this will not be called when the Clock is paused.
	public override void OnEnterFrame(float time)
	{
		Rotation = time;
	}
}
