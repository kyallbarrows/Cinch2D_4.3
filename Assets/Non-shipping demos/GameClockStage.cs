using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinch2D;

public class GameClockStage : Stage {

	private List<CinchSprite> _joes;
	private CinchSprite _joesContainer;
	private CinchSprite _play;
	private CinchSprite _pause;
	
	// Use this for initialization
	public override void OnAwake () {
		
		CinchOptions.DefaultPixelsPerMeter = 25f;
		
		Stage.Instance.TheCamera.backgroundColor = new Color(.2f, .2f, .2f, 1f);
		_joesContainer = Library.New<CinchSprite>("Joes Container");
		AddChild(_joesContainer);
		_joesContainer.X = Stage.Instance.ViewportWidth/2;
		_joesContainer.Y = Stage.Instance.ViewportHeight/2;
		_joesContainer.Clock = new GameClock("JoesContainerClock");
		
		_play = CinchSprite.NewFromImage("Play", 100f);
		_pause = CinchSprite.NewFromImage("Pause", 100f);
		AddChild(_play);
		AddChild(_pause);
		_play.Visible = false;
		_play.X = _pause.X = ViewportWidth - .5f;
		_play.Y = _pause.Y = ViewportHeight - .5f;
		
		_play.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, onPlayPress);
		_pause.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, onPausePress);
		
		_joes = new List<CinchSprite>();
		for (var i=0; i < 15; i++)
		{
			var joe = CreateJoe();
			_joes.Add (joe);
			joe.ScaleX = -1;
		}
	}
	
	public override void OnEnterFrame(float time)
	{
		foreach (var joe in _joes)
		{
			if (Mathf.Abs (joe.X) > 10f || Mathf.Abs (joe.Y) > 10f)
				joe.SetPosition(0, 0);
		}
	}
	
	private void onPausePress(MouseEvent e)
	{
		_pause.Visible = false;
		_play.Visible = true;
		_play.ScaleX = _play.ScaleY = .7f;
		new Tween(_play, "ScaleX", 1f, .5f, Easing.None.EaseNone);
		new Tween(_play, "ScaleY", 1f, .5f, Easing.None.EaseNone);
		_joesContainer.Clock.Paused = true;
	}
	
	private void onPlayPress(MouseEvent e)
	{
		_play.Visible = false;
		_pause.Visible = true;
		_pause.ScaleX = _pause.ScaleY = .7f;
		new Tween(_pause, "ScaleX", 1f, .5f, Easing.None.EaseNone);
		new Tween(_pause, "ScaleY", 1f, .5f, Easing.None.EaseNone);
		_joesContainer.Clock.Paused = false;
	}
	
	private Joe CreateJoe()
	{
		var joe = Library.New<Joe>("Joe");
		_joesContainer.AddChild(joe);
		var velX = Random.Range(-2f, 2f);
		var velY = Random.Range(-2f, 2f);
		joe.Rotation = -90 + Mathf.Atan2(velY, velX) * Mathf.Rad2Deg;
		joe.FakeVelocity = new Vector2(velX, velY);
		return joe;
	}
}
