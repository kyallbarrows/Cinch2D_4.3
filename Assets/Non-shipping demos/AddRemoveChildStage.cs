using System;
using UnityEngine;
using System.Collections;
using Cinch2D;

public class AddRemoveChildStage : DemoStageBase {
	
	private DisplayObjectContainer _container;
	
	public override void OnAwake()
	{
		base.OnAwake();
		
		var outerContainer = Library.New<DisplayObjectContainer>("OuterContainer");
		AddChild (outerContainer);
		outerContainer.X = outerContainer.Y = 1f;
		_container = Library.New<DisplayObjectContainer>("Container");
		outerContainer.AddChild(_container);
		_container.Rotation = 30f;
		
		var offsetX = ViewportWidth/50f;
		var offsetY = ViewportHeight/50f;
		for (var suit = 0; suit < 4; suit++)
		{
			for (var number = 0; number < 5; number++)
			{
				var card = Library.New (_cardSuits[suit] + (number+1).ToString());
				Assert (!_container.Contains(card), "_container should not contain " + card.Name);
				_container.AddChild(card);
				Assert (_container.Contains(card), "_container should contain " + card.Name);
				card.X = offsetX * (suit * 5 + number);
				card.Y = offsetY * (suit * 5 + number);
				card.MouseEnabled = true;
				card.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, onCardPress);
				card.AddEventListener<MouseEvent>(MouseEvent.MOUSE_UP, onCardRelease);
			}
		}
	}
	
	private void onCardPress(MouseEvent e)
	{
		var me = (MouseEvent)e;
		var target = (CinchSprite)me.Target;
		_container.AddChild(target);
		target.StartDrag();
	}
	
	private void onCardRelease(MouseEvent e)
	{
		var me = (MouseEvent)e;
		var target = (CinchSprite)me.Target;
		target.StopDrag();
	}
}
