using UnityEngine;
using System.Collections;
using Cinch2D;

public class MouseEventsTestStage : DemoStageBase {
	public override void OnAwake()
	{
		base.OnAwake();
		
		var cardContainer = Library.New<DisplayObjectContainer>("CardContainer");
		AddEventListener<MouseEvent>(MouseEvent.MOUSE_MOVE, onMouseMove);
		cardContainer.Y = ViewportHeight * .4f;
		cardContainer.X = ViewportWidth * -.4f;
		AddChild(cardContainer);
		
		var offsetX = ViewportWidth/30f;
		var offsetY = -ViewportHeight/30f;
		DisplayObjectContainer lastParent = cardContainer;
		for (var suit = 0; suit < 4; suit++)
		{
			for (var number = 0; number < 5; number++)
			{
				var card = Library.New (_cardSuits[suit] + (number+1).ToString(), "Card" + _cardSuits[suit] + (number + 1).ToString());
				lastParent.AddChild(card);
				lastParent = card;
				card.X = offsetX;
				card.Y = offsetY;
				card.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, onCardPress);
				card.AddEventListener<MouseEvent>(MouseEvent.MOUSE_MOVE, onCardMouseMove);
				card.AddEventListener<MouseEvent>(MouseEvent.RELEASE_OUTSIDE, onCardReleaseOutside);
			}
		}
		
		this.Name = "Mouse Events Stage";
	}
	
	public void onCardPress(MouseEvent e)
	{
		e.StopPropagation();
		var dispObj = (DisplayObjectContainer)e.Target;
		
		dispObj.MouseChildren = !dispObj.MouseChildren;

		if (dispObj.NumChildren > 0)
		{
			var child = dispObj.GetChildAt(0);
			var newScale = dispObj.MouseChildren ? 1f : .65f;
			new Tween(child, "ScaleX", newScale, .2f, Easing.None.EaseNone);
			new Tween(child, "ScaleY", newScale, .2f, Easing.None.EaseNone);
		}
	}
	
	public void onCardReleaseOutside(MouseEvent e)
	{
		e.StopPropagation();
		
		var dispObj = (DisplayObjectContainer)e.Target;
		do
		{
			Debug.Log ("Fixign card " + dispObj.Name);
			dispObj = dispObj.GetChildAt(0);
			dispObj.MouseChildren = true;
			new Tween(dispObj, "ScaleX", 1f, .1f, Easing.None.EaseNone);
			new Tween(dispObj, "ScaleY", 1f, .1f, Easing.None.EaseNone);
		}
		while(dispObj.NumChildren > 0);
/*		for (var dispObj = (DisplayObjectContainer)e.Target; dispObj.NumChildren > 0; dispObj = dispObj.GetChildAt(0))
		{
			Debug.Log ("Fixign card " + dispObj.Name);
			dispObj.MouseChildren = true;
			new Tween(dispObj, "ScaleX", 1f, .1f, Easing.None.EaseNone);
			new Tween(dispObj, "ScaleY", 1f, .1f, Easing.None.EaseNone);
		}			 */
	}
	
	
	private void onMouseMove(MouseEvent e)
	{
		if (e.LocalPos != e.StagePos)
			Debug.Log ("MouseEvent: LocalPos and StagePos dont match, but should");
		
		if (e.LocalPos != new Vector2(MouseX, MouseY))
			Debug.Log ("MouseEvent: LocalPos and MouseX/Y dont match, but should");
	}

	private void onCardMouseMove(MouseEvent e)
	{
		var dispObj = (DisplayObjectContainer)e.CurrentTarget;
		if (e.LocalPos != new Vector2(dispObj.MouseX, dispObj.MouseY))
			Debug.Log ("MouseEvent: LocalPos and MouseX/Y dont match, but should");
	}
}
