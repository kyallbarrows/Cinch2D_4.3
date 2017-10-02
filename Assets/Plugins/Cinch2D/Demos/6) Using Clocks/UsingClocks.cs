using UnityEngine;
using System.Collections;
using Cinch2D;

/// <summary>
/// Demonstrates Clocks, which let you pause or play different parts of the scene tree.  
/// Press the pause button to stop the berries.
/// </summary>
public class UsingClocks : DemoBase {
	
	private CinchSprite _gameplayContainer;
	
	private CinchSprite _play;
	private CinchSprite _pause;
	
	
	public override void OnAwake()
	{
		CreateBackground();
		
		//Create a container for "GamePlay" (a bunch of floating berries)
		_gameplayContainer = Library.New<CinchSprite>("StrawberriesContainer");
		AddChild(_gameplayContainer);
		
		//pausing this Clock will pause all children
		_gameplayContainer.Clock = new GameClock("GameplayClock");
		
		//Create 15 FloatingStrawberry's
		for (var i=0; i<15; i++)
		{
			_gameplayContainer.AddChild(Library.New<FloatingStrawberry>("Berry" + Random.Range (1, 1000)));			
		}
		
		//Create a button to pause gameplay
		_pause = CinchSprite.NewFromImage("Cinch2D/Pause");
		AddChild(_pause);
		//SetPosition is faster than setting X and Y individually, since X and Y each cause display chain updates
		_pause.SetPosition(ViewportWidth/2 - _pause.Width, ViewportHeight/2 - _pause.Height);
		_pause.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, Pause);		

		//Create a button to start gameplay
		_play = CinchSprite.NewFromImage("Cinch2D/Play");
		AddChild(_play);
		_play.SetPosition(ViewportWidth/2 - _play.Width, ViewportHeight/2 - _play.Height);
		_play.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, Play);
		_play.Visible = false;
		
	}
	
	public override void OnEnterFrame(float time)
	{
		base.OnEnterFrame(time);
		
		for(var i=_gameplayContainer.NumChildren-1; i>=0; i--)
		{
			//reset to center if we've drifted too far
			var child = _gameplayContainer.GetChildAt(i);
			if (Mathf.Abs (child.X) > ViewportWidth/2 || Mathf.Abs (child.Y) > ViewportHeight/2)
				child.Destroy();
		}
		
		if (_gameplayContainer.NumChildren < 700)
			_gameplayContainer.AddChild (Library.New<FloatingStrawberry>("Berry" + Random.Range (1, 1000)));
	}

	private void Pause(MouseEvent e)
	{
		//pausing the _gameplayContainer's clock will pause all OnEnterFrame calls of it and its children, so the berries will stop
		_gameplayContainer.Clock.Paused = true;
		//But this would be super-bad: Stage.Clock.Paused = true; 
		//It would stop EVERYTHING, including menus, and there would be no way to restart
		
		_play.Visible = true;
		_pause.Visible = false;
		//because _play and _pause still use the Stage's clock, they can still animate, even though gameplay is paused
		new Tween(_play, "ScaleX", 1.5f, .3f, Easing.Cubic.EaseOut).ContinueTo(1f, .2f, Easing.Cubic.EaseIn);
		new Tween(_play, "ScaleY", 1.5f, .3f, Easing.Cubic.EaseOut).ContinueTo(1f, .2f, Easing.Cubic.EaseIn);
	}
	
	private void Play(MouseEvent e)
	{
		_gameplayContainer.Clock.Paused = false;
		
		_pause.Visible = true;
		_play.Visible = false;
		new Tween(_pause, "ScaleX", 1.5f, .3f, Easing.Cubic.EaseOut).ContinueTo(1f, .2f, Easing.Cubic.EaseIn);
		new Tween(_pause, "ScaleY", 1.5f, .3f, Easing.Cubic.EaseOut).ContinueTo(1f, .2f, Easing.Cubic.EaseIn);
	}	
}
