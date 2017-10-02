using System;
using UnityEngine;
using System.Collections;
using Cinch2D;

public class TextStage : DemoStageBase {
	
	private DisplayObjectContainer _container;
	
	public override void OnAwake()
	{
		base.OnAwake();
		
		var outerContainer = Library.New<DisplayObjectContainer>("OuterContainer");
		AddChild (outerContainer);
		
		var button = Library.New<TextButtonThing>("TextButton");
		button.SetColor(Color.green);
		outerContainer.AddChild(button);
		
		button.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, onButtonPress);
	}
	
	private void onButtonPress(MouseEvent e)
	{
		new Tween((DisplayObjectContainer)e.CurrentTarget, "ScaleX", 1.5f, .3f, Easing.Circle.EaseOut, .2f).ContinueTo(1f, .5f, Easing.Bounce.EaseOut);
		new Tween((DisplayObjectContainer)e.CurrentTarget, "ScaleY", 1.5f, .5f, Easing.Cubic.EaseOut).ContinueTo(1f, .5f, Easing.Bounce.EaseOut);
		new Tween((DisplayObjectContainer)e.CurrentTarget, "Alpha", 0f, .5f, Easing.None.EaseNone).ContinueTo(1f, .5f, Easing.None.EaseNone);
	}
}
