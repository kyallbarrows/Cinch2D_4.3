using UnityEngine;
using System.Collections;
using Cinch2D;

/// <summary>
/// Shows all the ways to create an object.  No user interaction required.
/// </summary>
public class CreateObjects : DemoBase {

	public override void OnAwake()
	{
		CreateBackground();
		
		//4 ways to create Sprites:

		//1: Create empty Sprites with Sprite.NewEmpty() or Library.New<Sprite>()
		var container = CinchSprite.NewEmpty("Container");
		this.AddChild(container);
		container.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, DestroyWhateverWasClicked);
		
		//2: Create from Texture via Sprite.NewFromImage()
		var cherries = CinchSprite.NewFromImage("Cinch2D/Cherries", 256);
		container.AddChild(cherries);
		cherries.X = ViewportWidth/-4;
		
		//3: Library.New<Any CinchSprite Subclass>().  
		//Watermelon is defined in a separate file, and actually contains another way to instantiate a sprite
		var watermelon = Library.New<Watermelon>("Watermelon");
		container.AddChild(watermelon);
		
		//4: Library.New("SymbolId")
		//first, add a definition.  You can define all your symbols in one place using this method.
		var strawberryDef = new SymbolDef{
			SymbolId = "Strawberry",
			TexturePath = "Cinch2D/Strawberry",
			PixelsPerMeter = 256,
			RegistrationPoint = RegistrationPoint.Center
		};
		Library.AddDefinition(strawberryDef);
		
		//then instantiate the definition.  
		var strawberry = Library.New("Strawberry");
		container.AddChild(strawberry);
		strawberry.X = ViewportWidth/4;
	}
	
	private void DestroyWhateverWasClicked(MouseEvent me)
	{
		var target = (CinchSprite)me.Target;
		//Whatever you do, DON'T call Destroy(gameObject) or Destroy(some sprite instance).  Instead, use the .Destroy() method
		target.Destroy();
	}
}
