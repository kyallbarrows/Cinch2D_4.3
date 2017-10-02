using UnityEngine;
using System.Collections;
using Cinch2D;

/// <summary>
/// Demonstrates how Sprites work.  Click the strawberry or melon to attach them to the cherries.
/// </summary>
public class WorkingWithSprites : DemoBase {
	
	private SmartCherry _smartCherry;
	
	public override void OnAwake(){
		CreateBackground();
		
		//Cinch2D uses meters rather than pixels.  This makes it much easier to code for devices with varying screen sizes.
		//Sprites come from Textures, which are measured in pixels.
		//When making a new Sprite, use PixelsPerMeter to determine how big it will be in meters.
		
		//Strawberry texture is 512x512 pixels, so at 256 pixels per meter it will be 2x2 meters.
		var strawberry = CinchSprite.NewFromImage("Cinch2D/Strawberry", 256);
		AddChild(strawberry);
		//Sprites default to 0,0 which is the center of the screen.  This will set it halfway between center and the left edge of the screen
		strawberry.X = ViewportWidth/-4;
		
		//Registration points are the center point of the Sprite.  The Sprite will move, rotate, and scale around this point.
		//Let's center the watermelon around its bottom-left corner:
				var watermelon = CinchSprite.NewFromImage("Cinch2D/Watermelon", 256, RegistrationPoint.BottomLeft);
		AddChild(watermelon);
		//set its center halfway between center and the right edge.  
		watermelon.X = ViewportWidth/4;
		
		//you can extend the Sprite class just like in Flash.  
		//SmartCherry is a Sprite subclass that rotates continuously.
		//Instantiate Sprite subclasses via Library.New<Subclass Type>(new name);
		_smartCherry = Library.New<SmartCherry>("SmartCherryInstance");
		AddChild(_smartCherry);
		
		//Sprites can be added to each other.  This handler will add whatever was clicked to SmartCherry's display tree
		watermelon.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, AddToSmartCherry);
		strawberry.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, AddToSmartCherry);
	}
	
	private void AddToSmartCherry(MouseEvent e)
	{
		CinchSprite target = (CinchSprite)e.Target;
		_smartCherry.AddChild(target);
		
		//and let's make them half-transparent
		target.Alpha = .5f;
	}
}
