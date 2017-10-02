using UnityEngine;
using System.Collections;
using Cinch2D;

//This class extends the Stage base class (ok, actually DemoBase, which extends Stage), and is attached to the main camera
//Demonstrates a few things about working with the stage, displays a giant strawberry, and that's about it.
public class UsingTheStage : Stage {

	//don't use Start(), Awake(), Update(), or LateUpdate().  
	//Use OnAwake(), OnEnterFrame(), OnExitFrame() instead
	public override void OnAwake()
	{
		//ViewportWidth and ViewportHeight will give you the dimensions of the viewport
		//Let's figure out if we're running in landscape or portrait:
		bool isLandscape = ViewportWidth > ViewportHeight;
		
		//you can also set ViewportWidth or ViewportHeight.  Since aspect ratio is fixed, setting one will change the other.
		//Let's set the larger dimension to 20 meters:
		if (isLandscape)
			ViewportWidth = 20f;
		else
			ViewportHeight = 20f;
		
		//the origin (0,0 point) is in the middle of the screen.  Anything we add to the stage will show up there by default.
		//Let's add a strawberry to the screen:
		var strawberry = CinchSprite.NewFromImage("Cinch2D/Strawberry");
		AddChild(strawberry);
		
		//Stage also has Width and Height properties.  These are different from ViewportWidth/Height.  
		//They are the width/height of the contents of the Stage, rather than the Stage itself.
		//Let's see how big the strawberry is:
		Debug.Log ("Stage contents are " + Width + "x" + Height + " meters");
	}
}
