using UnityEngine;
using System.Collections;
using Cinch2D;

public class CardsDemo : Stage {
	
	private const float CARD_WIDTH = 100f;
	private const float CARD_HEIGHT = 128f;
	
	public override void OnAwake()
	{
		CreateBackground();
		CinchOptions.DefaultPixelsPerMeter = 100f;
		
		//let's make some playing cards!
		for(var row=0; row<4; row++)
		{
			for(var column=0; column<5; column++)
			{
				var card = CinchSprite.NewFromSpriteSheet("Cinch2D/PlayingCards", 
					column*CARD_WIDTH, row*CARD_HEIGHT, 
					CARD_WIDTH, CARD_HEIGHT);
				AddChild (card);
				
				card.X = ViewportWidth * Random.Range(-.4f, .4f);
				card.Y = ViewportHeight * Random.Range(-.4f, .4f);
				card.Rotation = Random.Range(-45f, 45f);
				
				card.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, OnCardPress);
				card.AddEventListener<MouseEvent>(MouseEvent.MOUSE_UP, OnCardRelease);
			}
		}
	}
	
	private void OnCardPress(MouseEvent e)
	{
		var card = (CinchSprite)e.Target;
		
		new Tween(card, "ScaleX", 1.5f, .35f, Easing.Cubic.EaseOut);
		new Tween(card, "ScaleY", 1.5f, .35f, Easing.Cubic.EaseOut);
		
		AddChild(card);	//this will float it back to the top
		card.StartDrag();
	}

	private void OnCardRelease(MouseEvent e)
	{
		var card = (CinchSprite)e.Target;
		card.StopDrag();
		
		new Tween(card, "ScaleX", 1f, .35f, Easing.Cubic.EaseOut);
		new Tween(card, "ScaleY", 1f, .35f, Easing.Cubic.EaseOut);		
	}

	private void CreateBackground()
	{
		CinchSprite background = CinchSprite.NewFromImage("Cinch2D/Background", 100f);
		background.Width = ViewportWidth;
		background.Height = ViewportHeight;
		AddChild(background);
	}
			
}

