using UnityEngine;
using System.Collections;
using Cinch2D;

/// <summary>
/// Demonstrates using sprite sheets to create Sprites.
/// A sprite sheet saves tons of file-size by putting all your textures into one file.
/// Click a card to float it to the top.
/// </summary>
public class SpriteSheets : DemoBase {
	
	private CinchSprite _cardsContainer;
	
	public override void OnAwake()
	{
		CreateBackground();
		
		_cardsContainer = Library.New<CinchSprite>("CardsContainer");
		AddChild(_cardsContainer);
		_cardsContainer.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, MoveCardToTop);
		
		float cardWidth = 100f;
		float cardHeight = 128f;
		
		CinchOptions.DefaultPixelsPerMeter = 100f;
		for (var number = 0; number < 5; number++)
		{
			for (var suit = 0; suit < 4; suit++){
				//even though the coordinate system is bottom up, sprite sheets default to top-down coordinates
				//this can be reversed by setting CinchOptions.UseTopLeftSpriteSheetCoordinates = false
				var left = number * cardWidth;	//move from left to right 100 px at a time
				var top = suit * cardHeight;	//move top to bottom 128 px at a time
				var pixPerMeter = 100f;		
				
				var newCard = CinchSprite.NewFromSpriteSheet("Cinch2D/PlayingCards", left, top, cardWidth, cardHeight, pixPerMeter);
				_cardsContainer.AddChild(newCard);
				
				//give the cards random positions and rotations
				newCard.X = ViewportWidth * Random.Range(-.4f, .4f);
				newCard.Y = ViewportHeight * Random.Range(-.4f, .4f);
				newCard.RotationDegrees = Random.Range (-45f, 45f);
			}
		}
	}
	
	private void MoveCardToTop(MouseEvent e)
	{
		var targetCard = (CinchSprite)e.Target;
		
		//adding something that has already been added will just float it to the top
		_cardsContainer.AddChild(targetCard);
	}
}
