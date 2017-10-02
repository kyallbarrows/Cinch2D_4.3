using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinch2D;

/// <summary>
/// Shows how to integrate Cinch2D with a non-orthographic 3D game.
/// </summary>
public class SwitchingCamera : Stage {
	
	private GameObject _gameplayContainer;
	private Camera _camera3D;
	private GameObject _teapot;
	private Light _light;
	
	public override void OnAwake()
	{
		var btn = Library.New<TextButton>("TextButtonInstance");
		AddChild(btn);
		btn.Text = "Cinch Button";
		
		//toss up a little info text to avoid confusing the user
		var info = SimpleTextField.NewFromString("Teapot is normal Unity 3D on a second camera.", "Cinch2D/FjallaOne-Regular", .25f, TextAnchor.UpperCenter);
		AddChild(info);
		info.Y =  -.75f;

		var silliness = SimpleTextField.NewFromString("CHOO CHOO IT\'S THE TEAPOT TRAIN.", "Cinch2D/FjallaOne-Regular", .5f, TextAnchor.UpperCenter);
		AddChild(silliness);
		silliness.Y =  1.5f;
		
		//if you need to hide all the Cinch stuff entirely, you can use Stage.Disable() and Stage.Enable();
	}
}