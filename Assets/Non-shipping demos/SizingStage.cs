using UnityEngine;
using System;
using System.Collections;
using Cinch2D;

public class SizingStage : Stage {
	
	private CinchSprite _sizeChart;
	private DisplayObjectContainer _earthContainer;
	private CinchSprite _mars;
	
	public override void OnAwake()
	{
		base.OnAwake();
		
		VerifySizingForEmptyObject();
		VerifySizingForEmptyParent();
		
		if (ViewportWidth > ViewportHeight)
			ViewportWidth = 10f;
		else
			ViewportHeight = 10f;
		
		_sizeChart = CinchSprite.NewFromImage("SizeChart", 100f, RegistrationPoint.BottomLeft);
		_sizeChart.Name = "SizeChart";
		_sizeChart.X = ViewportWidth/-2;
		_sizeChart.Y = ViewportHeight/-2;
		AddChild(_sizeChart);
		
		//give it a weird pixels-per-meter to make sure it's not an even width in meters
		_earthContainer = Library.New<DisplayObjectContainer>("EarthContainer");
		_earthContainer.SetPosition(3, 7);
		AddChild(_earthContainer);
		var innerContainer = Library.New<DisplayObjectContainer>("InnerContainer");
		_earthContainer.AddChild(innerContainer);
		var yetAnotherContainer = Library.New<DisplayObjectContainer>("YetAnotherContainer");
		innerContainer.AddChild(yetAnotherContainer);
		var earth = CinchSprite.NewFromImage("Earth", 158f, RegistrationPoint.Center);
		earth.MouseEnabled = true;
		earth.Name = "Earth";
		earth.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, onEarthPress);
		yetAnotherContainer.AddChild(earth);
		
		//give it a weird pixels-per-meter to make sure it's not an even width in meters
		_mars = CinchSprite.NewFromImage("Mars", 212f, RegistrationPoint.BottomLeft);
		_mars.MouseEnabled = true;
		_mars.Name = "Mars";
		_mars.SetPosition(.5f, .5f);
		_mars.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, onMarsPress);
		AddChild(_mars);
	}
	
	private void onEarthPress(MouseEvent e)
	{
		_earthContainer.Width = 2;
		_earthContainer.ScaleY = _earthContainer.ScaleX;
		_earthContainer.SetPosition(4, 4);
	}
	
	private void onMarsPress(MouseEvent e)
	{
		_mars.Height = 1;
		_mars.ScaleX = _mars.ScaleY;
		//mars is bottom-left registered, so put it at 5, 5 rather than 5.5, 5.5
		_mars.SetPosition(5, 5);
	}
	
	private void VerifySizingForEmptyObject(){
		var emptyContainer = Library.New<DisplayObjectContainer>("EmptyContainer");
		AddChild(emptyContainer);
		
		//should be able to set width/height, scaleX/Y to whatever without any issues
		emptyContainer.Width = 20;
		emptyContainer.Height = 30;
		emptyContainer.ScaleX = 2;
		emptyContainer.ScaleY = 10;
		
		var caughtBadScaleXException = false;
		var caughtBadScaleYException = false;
		
		try	{	emptyContainer.ScaleX = 0;	} catch(Exception){ caughtBadScaleXException = true; }
		try	{	emptyContainer.ScaleY = 0;	} catch(Exception){ caughtBadScaleYException = true; }
		
		if (!caughtBadScaleXException || !caughtBadScaleYException)
			throw new Exception("Test failed for throwing on zero-scales");
		
		RemoveChild(emptyContainer);
	}
	
	private void VerifySizingForEmptyParent(){
		var emptyParent = Library.New<DisplayObjectContainer>("EmptyParent");
		AddChild(emptyParent);
		emptyParent.AddChild(CinchSprite.NewFromImage("Earth", 158f, RegistrationPoint.Center));
		
		//should be able to set width/height, scaleX/Y to whatever without any issues
		emptyParent.Width = 20;
		emptyParent.Height = 30;
		emptyParent.ScaleX = 2;
		emptyParent.ScaleY = 10;
		
		var caughtSmallWidthException = false;
		var caughtSmallHeightException = false;
		var caughtLargeWidthException = true;
		var caughtLargeHeightException = true;
		
		try	{	emptyParent.Width = 0;	} catch(Exception){ caughtSmallWidthException = true; }
		try	{	emptyParent.Height = 0;	} catch(Exception){ caughtSmallHeightException = true; }
		try	{	emptyParent.Width = 10000f;	} catch(Exception){ caughtLargeWidthException = true; }
		try	{	emptyParent.Height = 10000f;	} catch(Exception){ caughtLargeHeightException = true; }
		
		if (!caughtSmallWidthException || !caughtSmallHeightException)
			throw new Exception("Test failed for throwing on small sizes");
		
		if (!caughtLargeWidthException || !caughtLargeHeightException)
			throw new Exception("Test failed for throwing on large sizes");
		
		RemoveChild(emptyParent);
	}
}
