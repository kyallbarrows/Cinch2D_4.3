using UnityEngine;
using System.Collections;
using Cinch2D;

public class NineSliceScaling : Stage {

	public override void OnAwake () {
		var panel = CinchSprite.NewFromImage("Cinch2D/DialogBackground", 256f);
		//leave a 20% border around the edge of the panel, that won't scale
		panel.SetScale9Grid(new Rect(.2f, .2f, .6f, .6f));
		AddChild(panel);
		
		panel.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, (e) => {
			panel.ScaleX = .1f;
			panel.ScaleY = 2f;
			new Tween(panel, "ScaleX", 1f, 2f, Easing.Bounce.EaseOut);
			new Tween(panel, "ScaleY", 1f, 2f, Easing.Bounce.EaseOut);
		});
		
		var instructions = SimpleTextField.NewFromString("Click panel", "Cinch2D/FjallaOne-Regular", .5f);
		AddChild(instructions);
		instructions.Y = -1.5f;
	}
}
