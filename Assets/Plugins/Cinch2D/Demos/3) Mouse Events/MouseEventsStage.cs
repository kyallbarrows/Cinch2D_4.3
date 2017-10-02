using UnityEngine;
using System.Collections;
using Cinch2D;

/// <summary>
/// Shows how to use mouse events.  Click the melon or text on the melon to make the bounce.
/// </summary>
public class MouseEventsStage : DemoBase {
	public override void OnAwake()
	{
		base.OnAwake();
		CreateBackground();
		
		//create a watermelon to listen to.  
		var watermelon = CinchSprite.NewFromImage("Cinch2D/Watermelon", 256);
		AddChild(watermelon);
		watermelon.Name = "Watermelon";
		
		//let's add a label to the melon
		var textField = SimpleTextField.NewFromString("Watermelon!", "Cinch2D/FjallaOne-Regular", .5f);
		watermelon.AddChild(textField);
		textField.Name = "TextField";
		
		//now add a listener.  Cinch2D implements MOUSE_DOWN, MOUSE_UP, RELEASE_OUTSIDE, MOUSE_OVER, MOUSE_OUT, and MOUSE_MOVE
		watermelon.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, onWatermelonPress);
	}
	
	public void onWatermelonPress(MouseEvent e)
	{
		//there's a big difference between e.Target and e.CurrentTarget.
		var target = (DisplayObjectContainer)e.Target;		
		var currentTarget = (DisplayObjectContainer)e.CurrentTarget;
		
		//e.Target is what was actually clicked (it may be the textfield or the watermelon slice
		//note: Rotation is in radians, hence the tiny value, use RotationDegrees if you like numbers that add up to 360.
		new Tween(target, "Rotation", .2f, .6f, Easing.Bounce.EaseIn).ContinueTo(0f, .2f, Easing.None.EaseNone);
		new Tween(target, "ScaleX", 1.7f, .6f, Easing.Cubic.EaseIn).ContinueTo(1f, .2f, Easing.None.EaseNone);
		new Tween(target, "ScaleY", 1.7f, .6f, Easing.Cubic.EaseIn).ContinueTo(1f, .2f, Easing.None.EaseNone);
		
		//e.CurrentTarget represents the object that is listening to the event (in this case, it will always be the watermelon object, since that's where we called AddEventListener)
		new Tween(currentTarget, "Y", .25f, .6f, Easing.Cubic.EaseOut).ContinueTo(0, .2f, Easing.Cubic.EaseIn);
	}
}
