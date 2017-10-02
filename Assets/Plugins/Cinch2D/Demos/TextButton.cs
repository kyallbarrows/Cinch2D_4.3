using UnityEngine;
using System.Collections;
using Cinch2D;

public class TextButton : CinchSprite {
	
	private CinchSprite _foreground;
	private CinchSprite _background;
	private SimpleTextField _text;
	
	public override void OnAwake()
	{
		_background = CinchSprite.NewFromSpriteSheet("Cinch2D/ButtonParts", 0, 257, 512, 256, 256);
		AddChild(_background);
		
		_foreground = CinchSprite.NewFromSpriteSheet("Cinch2D/ButtonParts", 0, 0, 512, 256, 256);
		AddChild(_foreground);
		
		_text = SimpleTextField.NewFromString("", "Cinch2D/FjallaOne-Regular", .2f);
		AddChild(_text);
		
		this.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, OnMouseDown);
	}
	
	public string Text{
		get { return _text.Text; }
		set { _text.Text = value; }
	}
	
	private void OnMouseDown(MouseEvent e)
	{
		new Tween(_background, "ScaleX", 1.4f, .2f, Easing.Quart.EaseOut).ContinueTo(1f, .2f, Easing.Cubic.EaseIn, .2f);
		new Tween(_background, "ScaleY", 1.4f, .2f, Easing.Quart.EaseOut).ContinueTo(1f, .2f, Easing.Cubic.EaseIn, .2f);
		new Tween(_foreground, "ScaleX", 1.4f, .4f, Easing.Circle.EaseOut).ContinueTo(1f, .2f, Easing.Cubic.EaseIn);
		new Tween(_foreground, "ScaleY", 1.4f, .4f, Easing.Circle.EaseOut).ContinueTo(1f, .2f, Easing.Cubic.EaseIn);
		new Tween(_text, "ScaleX", 1.2f, .4f, Easing.Circle.EaseOut).ContinueTo(1f, .2f, Easing.Cubic.EaseIn);
		new Tween(_text, "ScaleY", 1.2f, .4f, Easing.Circle.EaseOut).ContinueTo(1f, .2f, Easing.Cubic.EaseIn);
	}
}
