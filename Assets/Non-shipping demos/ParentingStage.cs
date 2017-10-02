using UnityEngine;
using System.Collections;
using Cinch2D;

public class ParentingStage : Stage {
	
	private DisplayObjectContainer _everythingContainer;
	private DisplayObjectContainer _table;
	
	private void ValidateNumberMouseObjects(int expected)
	{
		if (MouseInput.Instance.__GetNumberOfMouseEnabledObjects() != expected)
			Debug.Log ("WRONG NUMBER OF MOUSE OBJECTS: EXPECTED: " + expected + "  ACTUAL: " + MouseInput.Instance.__GetNumberOfMouseEnabledObjects());
	}
	
	public override void OnAwake()
	{
		base.OnAwake();
		
		CinchOptions.UseTopLeftSpriteSheetCoordinates = true;
		CinchOptions.DefaultPixelsPerMeter = 100f;
		
		this.Name = "ParentingStage";
		AddSymbolDefinitions();
		
		AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, onStagePress);
		
		_everythingContainer = Library.New<DisplayObjectContainer>();
		_everythingContainer.Name = "Everything container";
		_everythingContainer.MouseEnabled = false;
		AddChild(_everythingContainer);
		
		_table = Library.New ("Table");
		_table.MouseEnabled = true;
		ValidateNumberMouseObjects(0);
		_everythingContainer.AddChild(_table);
		ValidateNumberMouseObjects(1);
		_table.Name = "Table";
		_table.Width = ViewportWidth * .8f;
		_table.ScaleY = _table.ScaleX;
		
		var checkerBoard = Library.New ("CheckerBoard");
		_table.AddChild(checkerBoard);
		checkerBoard.MouseEnabled = true;
		ValidateNumberMouseObjects(2);
		checkerBoard.Rotation = 30;
		checkerBoard.Name = "Checkerboard";
		checkerBoard.Y = checkerBoard.Height * .75f;
		
		var w = checkerBoard.Width;
		var h = checkerBoard.Height;

		var totalCheckers = 0;
		var isRed = false;
		for (var __x = -3.5f; __x <= 3f; __x += 1f)
		{
			isRed = !isRed;
			for (var __y = -3.5f; __y <= 3.5f; __y += 1f)
			{
				isRed = !isRed;
				var checker = Library.New (isRed ? "RedChecker" : "BlackChecker");
				checker.Width = checker.Height = w/8;
				checker.Name = "Checker" + totalCheckers++;
				checkerBoard.AddChild(checker);
				checker.MouseEnabled = true;
				checker.X = w * __x/8f;
				checker.Y = h * __y/8f;

				checker.AddEventListener<MouseEvent>(MouseEvent.MOUSE_OVER, onAnythingOver);
				checker.AddEventListener<MouseEvent>(MouseEvent.MOUSE_OUT, onAnythingOut);
			}
		}
		
		_everythingContainer.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, onAnythingPress);
		_everythingContainer.AddEventListener<MouseEvent>(MouseEvent.RELEASE_OUTSIDE, onAnythingUpOutside);
		AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, onStagePress);
	}
	
	private void AddSymbolDefinitions()
	{
		Library.AddImageSpriteDefinition("Table", "Table", RegistrationPoint.Center);
		Library.AddImageSpriteDefinition("CheckerBoard", "CheckerBoard", RegistrationPoint.Center);
		Library.AddImageSpriteDefinition("RedChecker", "Checker", RegistrationPoint.Center);
		Library.AddImageSpriteDefinition("BlackChecker", "BlackChecker", RegistrationPoint.Center);
	}

	private void onStagePress(MouseEvent ev)
	{
		var me = (MouseEvent)ev;
		if (me.Target is ParentingStage && (ParentingStage)me.Target == this)
		{
			new Tween(_everythingContainer, "ScaleX", .8f, .5f, Easing.None.EaseNone).ContinueTo(1f, .5f, Easing.None.EaseNone);
			new Tween(_everythingContainer, "ScaleY", .8f, .5f, Easing.None.EaseNone).ContinueTo(1f, .5f, Easing.None.EaseNone);
		}
	}

	private void StagePressComplete(DisplayObjectContainer d)
	{
	}
	
	private void onAnythingPress(MouseEvent ev)
	{
		var me = (MouseEvent)ev;
		var dispObj = (DisplayObjectContainer)(me.Target);
		var origScale = dispObj.ScaleX;
		new Tween(dispObj, "ScaleX", origScale*2, .5f, Easing.Bounce.EaseOut).ContinueTo(origScale, .25f, Easing.None.EaseNone);
		new Tween(dispObj, "ScaleY", origScale*2, .5f, Easing.Bounce.EaseOut).ContinueTo(origScale, .25f, Easing.None.EaseNone);
	}

	private void onAnythingOver(MouseEvent ev)
	{
		var me = (MouseEvent)ev;
		var dispObj = (DisplayObjectContainer)(me.Target);
		new Tween(dispObj, "Alpha", .6f, .25f, Easing.None.EaseNone);
	}
	private void onAnythingOut(MouseEvent ev)
	{
		var me = (MouseEvent)ev;
		var dispObj = (DisplayObjectContainer)(me.Target);
		new Tween(dispObj, "Alpha", 1, .25f, Easing.None.EaseNone);
	}

	
	private void onAnythingUpOutside(MouseEvent ev)
	{
		var me = (MouseEvent)ev;
		var dispObj = (DisplayObjectContainer)(me.Target);
		new Tween(dispObj, "Alpha", 0, .5f, Easing.None.EaseNone);
		new Tween(dispObj, "Alpha", 1, .5f, Easing.None.EaseNone, .7f);
	}
	
}
