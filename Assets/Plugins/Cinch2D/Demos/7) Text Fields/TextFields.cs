using UnityEngine;
using System.Collections;
using Cinch2D;

/// <summary>
/// Shows how to create text fields.  Non-interactive.
/// </summary>
public class TextFields : DemoBase {
	
	public override void OnAwake()
	{
		CreateBackground();
		
		var upperLeft = SimpleTextField.NewFromString("Upper-Left", "Cinch2D/FjallaOne-Regular", .5f, TextAnchor.UpperLeft);
		upperLeft.TextColor = Color.green;
		AddChild(upperLeft);
		
		var lowerLeft = SimpleTextField.NewFromString("Lower-Left", "Cinch2D/FjallaOne-Regular", .5f, TextAnchor.LowerLeft);
		lowerLeft.TextColor = Color.red;
		AddChild(lowerLeft);
		
		var upperRight = SimpleTextField.NewFromString("Upper-Right", "Cinch2D/FjallaOne-Regular", .5f, TextAnchor.UpperRight);
		upperRight.TextColor = Color.magenta;
		AddChild(upperRight);
		
		var lowerRight = SimpleTextField.NewFromString("Lower-Right", "Cinch2D/FjallaOne-Regular", .5f, TextAnchor.LowerRight);
		lowerRight.TextColor = Color.cyan;
		AddChild(lowerRight);
		
		//this will overlap all the others, not a bug.
		var centeredText = SimpleTextField.NewFromString("Centered", "Cinch2D/FjallaOne-Regular", .5f, TextAnchor.MiddleCenter);
		AddChild(centeredText);
	}
}
