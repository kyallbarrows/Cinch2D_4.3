using System;
using UnityEngine;
using System.Collections;
using Cinch2D;

public class DemoStageBase : Stage {
	protected void Assert(bool isTrue, string message)
	{
		if (!isTrue)
			throw new Exception("Assertion failed: " + message);
	}
	
	public override void OnAwake()
	{
		DefinePlayingCards();
	}
	
	protected string[] _cardSuits = {"diamond", "heart", "anchor", "bell"};
	protected void DefinePlayingCards()
	{
		CinchOptions.DefaultPixelsPerMeter = 100f;
		CinchOptions.UseTopLeftSpriteSheetCoordinates = true;
		for (var number = 0; number < 5; number++)
		{
			for (var suit = 0; suit < 4; suit++){
				var classId = _cardSuits[suit] + (number+1).ToString();
				Library.AddImageSpriteDefinition(classId, "Cinch2D/PlayingCards", new Rect(number * 100, suit * 128, 100, 128));
			}
		}
	}
}
