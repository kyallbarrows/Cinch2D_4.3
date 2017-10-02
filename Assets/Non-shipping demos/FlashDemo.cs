using UnityEngine;
using System.Collections;
using Cinch2D;

public class FlashDemo : Stage {
	
	public override void OnAwake () 
	{
		CreateBackground();

		//make the start button and add it to the stage
		CinchSprite startButton = CinchSprite.NewFromImage("StartButton");
		this.AddChild(startButton);
		startButton.Width = 3f;
		startButton.ScaleY = startButton.ScaleX;
		
		//give it a jaunty little tilt
		startButton.Rotation = 10f;
		
		//do an animation effect and start the game when startButton is pressed
		startButton.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, (mouseEvent) => {
			new Tween(startButton, "ScaleX", 
				1.5f, .5f, Easing.Bounce.EaseOut, 0f);
		});
		
		//add an instructions text field
		SimpleTextField instructions = SimpleTextField.NewFromString("Press Start to begin!", "FjallaOne-Regular", .4f, TextAnchor.MiddleCenter);
		this.AddChildAt(instructions, 1);
		
		startButton.X = ViewportWidth/2;
		startButton.Y = ViewportHeight/2;
		instructions.X = startButton.X;
		instructions.Y = startButton.Y - 1.4f;
	}
	
	private void CreateBackground()
	{
		CinchSprite background = CinchSprite.NewFromImage("Background", 100f, RegistrationPoint.BottomLeft);
		background.Width = ViewportWidth;
		background.Height = ViewportHeight;
		AddChild(background);
	}
}
