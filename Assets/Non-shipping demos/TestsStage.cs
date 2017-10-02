using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinch2D;

public class AddRemoveTestObject : CinchSprite
{
	public int AddedCount;
	public int RemovedCount;
	public int AddedToStageCount;
	public int RemovedFromStageCount;
	
	public override void AddedToStage()
	{
		AddedToStageCount++;
	}
	
	public override void RemovedFromStage()
	{
		RemovedFromStageCount++;
	}
	
	protected override void __Added()
	{
		base.__Added();
		AddedCount++;
	}
	
	protected override void __Removed()
	{
		base.__Removed();
		RemovedCount++;
	}
}


public class TestsStage : DemoStageBase {
	
	private DisplayObjectContainer _country;
	private DisplayObjectContainer _state;
	private DisplayObjectContainer _city;
	private DisplayObjectContainer _house;
	
	private List<String> _eventRecord;
	
	public override void OnAwake () {
		base.OnAwake ();
		
		TestEventListeners();
		TestAddGetChildMethods();
	}
		
	private void TestAddGetChildMethods()
	{
		var parent = Library.New<CinchSprite>();
		Assert (parent.Stage == null, "parent stage should be null");
		AddChild(parent);
		Assert (parent.Stage == this, "parent stage should be this");
		Assert (parent.Parent == this, "parent parent should be this");
		Assert ((parent.NumChildren == 0), "Parent.NumChildren should return 0, returned " + parent.NumChildren);
		
		for (var i=1; i<=10; i++)
		{
			var thisChild = Library.New<CinchSprite>("Child" + i);
			parent.AddChild(thisChild);
		}
		Assert ((parent.NumChildren == 10), "Parent.NumChildren should return 10, returned " + parent.NumChildren);
		
		for (var i=1; i<=10; i++)
		{
			parent.AddChildAt(parent.GetChildByName("Child" + i), 0);
		}
		
		for (var i=0; i<10; i++)
		{
			Assert((parent.GetChildAt(i).Name == ("Child" + (10-i))), "wrong child: " + parent.GetChildAt(i).Name);
		}
		
		var c8 = parent.GetChildByName("Child8");
		Assert(c8.Name == "Child8", "Child8 not found");
		Assert (c8.Stage == this, "Child8 stage should be this");
		Assert (c8.Parent == parent, "Child8 parent should be parent");
		
		//test removing ranges
		parent.RemoveChildren(1, 8);
		Assert (parent.NumChildren == 2, "Should have 2 children, has " + parent.NumChildren);
		Assert (c8.Parent == null, "Child8 parent should be null");
		Assert (c8.Stage == null, "Child8 stage should be null");
		//now we're left with Child10 at 0, Child1 at 1
		parent.SwapChildrenAt(0, 1);
		Assert ((parent.GetChildAt(0).Name == "Child1"), "wrong child, expected Child1: " + parent.GetChildAt(0).Name);
		Assert ((parent.GetChildAt(1).Name == "Child10"), "wrong child, expected Child10: " + parent.GetChildAt(1).Name);
		
		var c1 = parent.GetChildByName("Child1");
		var c10 = parent.GetChildByName("Child10");
		Assert (c1 != null, "C1 was null");
		Assert (parent.GetChildIndex(c1) == 0, "Child1 should be at index 0");
		Assert (c10 != null, "C10 was null");
		Assert (parent.GetChildIndex(c10) == 1, "Child10 should be at index 1");
		Assert (parent.GetChildIndex(c8) == -1, "Child8 should be at index -1");
		
		parent.SwapChildren(c1, c10);
		Assert ((parent.GetChildAt(1).Name == "Child1"), "wrong child, expected Child1: " + parent.GetChildAt(1).Name);
		Assert ((parent.GetChildAt(0).Name == "Child10"), "wrong child, expected Child10: " + parent.GetChildAt(0).Name);
		Assert (parent.NumChildren == 2, "Should have 2 children");
		
		var caughtChildNotChild = false;
		try
		{
			parent.SetChildIndex(c8, 1);		
		}
		catch (ArgumentException)
		{
			caughtChildNotChild = true;
		}
		Assert (caughtChildNotChild, "Did not throw exception when setting index on object that is not child");

		var caughtIndexOutOfRange = false;
		try
		{
			parent.SetChildIndex(c1, 2);		
		}
		catch (ArgumentException)
		{
			caughtIndexOutOfRange = true;
		}
		Assert (caughtIndexOutOfRange, "Did not throw exception when setting index out of range");
		
		Assert (parent.NumChildren == 2, "Should have 2 children");
		Assert (c8.Parent == null, "Child8 parent should be null");
		Assert (c8.Stage == null, "Child8 stage should be null");
		parent.AddChild(c8);
		Assert (c8.Parent == parent, "Child8 parent should be parent");
		Assert (c8.Stage == this, "Child8 stage should be this");
		Assert (parent.NumChildren == 3, "Should have 3 children");
		parent.SetChildIndex(c8, 1);
		Assert (c8.Parent == parent, "Child8 parent should be parent");
		Assert (c8.Stage == this, "Child8 stage should be this");
		
		Assert ((parent.GetChildAt(0).Name == "Child10"), "wrong child, expected Child10: " + parent.GetChildAt(0).Name);
		Assert ((parent.GetChildAt(1).Name == "Child8"), "wrong child, expected Child8: " + parent.GetChildAt(1).Name);
		Assert ((parent.GetChildAt(2).Name == "Child1"), "wrong child, expected Child1: " + parent.GetChildAt(2).Name);
		
		parent.RemoveChild(c8);
		Assert (c8.Parent == null, "Child8 parent should be null");
		Assert (c8.Stage == null, "Child8 stage should be null");
		
		Assert ((parent.GetChildAt(0).Name == "Child10"), "wrong child, expected Child10: " + parent.GetChildAt(0).Name);
		Assert ((parent.GetChildAt(1).Name == "Child1"), "wrong child, expected Child1: " + parent.GetChildAt(1).Name);
		
		
		//CHECKING THAT __added and __removed are called
		var addRem = Library.New<AddRemoveTestObject>("AddRem");
		Assert (addRem.AddedCount == 0, "AddedCount should be 0");
		Assert (addRem.RemovedCount == 0, "RemovedCount should be 0");
		
		try{
			parent.RemoveChild(addRem);
		}
		catch(Exception){}
		Assert (addRem.AddedCount == 0, "AddedCount should be 0");
		Assert (addRem.RemovedCount == 0, "RemovedCount should be 0");
		
		parent.AddChild(addRem);
		Assert (addRem.AddedCount == 1, "AddedCount should be 1");
		Assert (addRem.RemovedCount == 0, "RemovedCount should be 0");
		
		parent.RemoveChild(addRem);
		Assert (addRem.AddedCount == 1, "AddedCount should be 1");
		Assert (addRem.RemovedCount == 1, "RemovedCount should be 1");
		
		//remove others, counts don't change
		parent.RemoveChildren(0, 100);
		Assert (parent.NumChildren == 0, "Parent should have 0 children, has " + parent.NumChildren);
		Assert (addRem.AddedCount == 1, "AddedCount should be 1");
		Assert (addRem.RemovedCount == 1, "RemovedCount should be 1");
		
		//remove range, removedcount increments
		parent.AddChild(addRem);
		parent.RemoveChildren(0);
		Assert (parent.NumChildren == 0, "Parent should have 0 children");
		Assert (addRem.AddedCount == 2, "AddedCount should be 2");
		Assert (addRem.RemovedCount == 2, "RemovedCount should be 2");
		
		//add it twice, counts should not change
		parent.AddChild(addRem);
		Assert (parent.NumChildren == 1, "Parent should have 1 children");
		Assert (addRem.AddedCount == 3, "AddedCount should be 3, was " + addRem.AddedCount);
		Assert (addRem.RemovedCount == 2, "RemovedCount should be 2, was " + addRem.RemovedCount);
		parent.AddChild(addRem);
		Assert (parent.NumChildren == 1, "Parent should have 1 children");
		Assert (addRem.AddedCount == 3, "AddedCount should be 3, was " + addRem.AddedCount);
		Assert (addRem.RemovedCount == 2, "RemovedCount should be 2, was " + addRem.RemovedCount);
		
		//change index, counts should not change
		parent.AddChild(c1);
		parent.AddChild(c10);
		parent.SetChildIndex(addRem, 1);
		Assert (addRem.AddedCount == 3, "AddedCount should be 3, was " + addRem.AddedCount);
		Assert (addRem.RemovedCount == 2, "RemovedCount should be 2, was " + addRem.RemovedCount);
		
		//removeat should update count
		parent.RemoveChildAt(1);
		Assert (addRem.AddedCount == 3, "AddedCount should be 3, was " + addRem.AddedCount);
		Assert (addRem.RemovedCount == 3, "RemovedCount should be 3, was " + addRem.RemovedCount);
		
		var stageAddRem = Library.New<AddRemoveTestObject>("stageAddRem");
		Assert (stageAddRem.AddedToStageCount == 0, "AddedToStageCount should be 0, was " + stageAddRem.AddedToStageCount);
		Assert (stageAddRem.RemovedFromStageCount == 0, "RemovedFromStageCount should be 0, was " + stageAddRem.RemovedFromStageCount);

		//Adding to object not on stage should not change counts
		RemoveChild(parent);
		parent.AddChild(stageAddRem);
		Assert (stageAddRem.AddedToStageCount == 0, "AddedToStageCount should be 0, was " + stageAddRem.AddedToStageCount);
		Assert (stageAddRem.RemovedFromStageCount == 0, "RemovedFromStageCount should be 0, was " + stageAddRem.RemovedFromStageCount);

		//and adding parent should not touch counts
		var anotherParent = Library.New<DisplayObjectContainer>("anotherParent");
		anotherParent.AddChild (parent);
		Assert (stageAddRem.AddedToStageCount == 0, "AddedToStageCount should be 0, was " + stageAddRem.AddedToStageCount);
		Assert (stageAddRem.RemovedFromStageCount == 0, "RemovedFromStageCount should be 0, was " + stageAddRem.RemovedFromStageCount);
		
		AddChild(anotherParent);
		Assert (stageAddRem.AddedToStageCount == 1, "AddedToStageCount should be 1, was " + stageAddRem.AddedToStageCount);
		Assert (stageAddRem.RemovedFromStageCount == 0, "RemovedFromStageCount should be 0, was " + stageAddRem.RemovedFromStageCount);

		RemoveChild(anotherParent);
		Assert (stageAddRem.AddedToStageCount == 1, "AddedToStageCount should be 1, was " + stageAddRem.AddedToStageCount);
		Assert (stageAddRem.RemovedFromStageCount == 1, "RemovedFromStageCount should be 1, was " + stageAddRem.RemovedFromStageCount);

		AddChild(anotherParent);
		anotherParent.RemoveChildAt(0);
		Assert (stageAddRem.AddedToStageCount == 2, "AddedToStageCount should be 2, was " + stageAddRem.AddedToStageCount);
		Assert (stageAddRem.RemovedFromStageCount == 2, "RemovedFromStageCount should be 2, was " + stageAddRem.RemovedFromStageCount);
		
		Debug.Log("WIN: TestAddGetChildMethods");
		
	}
	
	private void TestEventListeners()
	{
		_country = Library.New<CinchSprite>("Country");
		_state = Library.New<CinchSprite>("Stage");
		_city = Library.New<CinchSprite>("City");
		_house = Library.New<CinchSprite>("House");
		_country.Name = "Country";
		_state.Name = "State";
		_city.Name = "City";
		_house.Name = "House";
		_country.MouseEnabled = _state.MouseEnabled = _city.MouseEnabled = _house.MouseEnabled;
		_country.AddChild(_state);
		_state.AddChild(_city);
		_city.AddChild(_house);
		
		AssertEverybodyWillTrigger(MouseEvent.MOUSE_DOWN, false);
		AssertOnlyCityHasEventListener(MouseEvent.MOUSE_DOWN, false);
		
		_city.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, GenericEventHandler);
		_city.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, GenericEventHandler);
		_city.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, GenericEventHandler);
		AssertEverybodyWillTrigger(MouseEvent.MOUSE_DOWN, true);
		AssertOnlyCityHasEventListener(MouseEvent.MOUSE_DOWN, true);
		
		_city.RemoveEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, GenericEventHandler);
		AssertEverybodyWillTrigger(MouseEvent.MOUSE_DOWN, false);
		AssertOnlyCityHasEventListener(MouseEvent.MOUSE_DOWN, false);
		
		_eventRecord = new List<String>();
		AssertEventRecordIs("");
		_country.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, StackRecordingEventHandler, true);
		_state.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, StackRecordingEventHandler, false);
		_city.AddEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, StackRecordingEventHandler, true);
		_city.DispatchEvent(new MouseEvent(MouseEvent.MOUSE_DOWN, _city, new Vector2(0, 0), new Vector2(0, 0)));
		AssertEventRecordIs("CAP:Country,AT:City,BUB:State");

		//removed the wrong handler, so should still do all the dispatching
		_country.RemoveEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, GenericEventHandler);
		//removed the right handler with wrong capture setting, so should still do all the dispatching
		_country.RemoveEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, StackRecordingEventHandler, false);
		_city.DispatchEvent(new MouseEvent(MouseEvent.MOUSE_DOWN, _city, new Vector2(0, 0), new Vector2(0, 0)));
		AssertEventRecordIs("CAP:Country,AT:City,BUB:State");
		_state.DispatchEvent(new MouseEvent(MouseEvent.MOUSE_DOWN, _city, new Vector2(0, 0), new Vector2(0, 0)));
		AssertEventRecordIs("CAP:Country,AT:State");
		
		//removed the right handler, so should do no the dispatching
		_country.RemoveEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, StackRecordingEventHandler, true);
		_city.DispatchEvent(new MouseEvent(MouseEvent.MOUSE_DOWN, _city, new Vector2(0, 0), new Vector2(0, 0)));
		AssertEventRecordIs("AT:City,BUB:State");
		_state.DispatchEvent(new MouseEvent(MouseEvent.MOUSE_DOWN, _city, new Vector2(0, 0), new Vector2(0, 0)));
		AssertEventRecordIs("AT:State");
		
		//removed the right handler with wrong capture setting, so should still do all the dispatching
		_state.RemoveEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, StackRecordingEventHandler, true);
		_city.DispatchEvent(new MouseEvent(MouseEvent.MOUSE_DOWN, _city, new Vector2(0, 0), new Vector2(0, 0)));
		AssertEventRecordIs("AT:City,BUB:State");
		_state.DispatchEvent(new MouseEvent(MouseEvent.MOUSE_DOWN, _city, new Vector2(0, 0), new Vector2(0, 0)));
		AssertEventRecordIs("AT:State");
		
		_state.RemoveEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN, StackRecordingEventHandler);
		_city.DispatchEvent(new MouseEvent(MouseEvent.MOUSE_DOWN, _city, new Vector2(0, 0), new Vector2(0, 0)));
		AssertEventRecordIs("AT:City");
		_state.DispatchEvent(new MouseEvent(MouseEvent.MOUSE_DOWN, _city, new Vector2(0, 0), new Vector2(0, 0)));
		AssertEventRecordIs("");
		
		Debug.Log("WIN: TestEventListeners");
	}
			
	private void GenericEventHandler(MouseEvent e){}
	
	private void AssertEverybodyWillTrigger(string eventName, bool expected)
	{
		//ok, not everybody.  Just everybody other than _house
		Assert(expected == _country.WillTrigger<MouseEvent>(eventName), "Country should " + (expected ? "" : "not") + " WillTrigger " + eventName);
		Assert(expected == _state.WillTrigger<MouseEvent>(eventName), "Stage should " + (expected ? "" : "not") + " WillTrigger " + eventName);
		Assert(expected == _city.WillTrigger<MouseEvent>(eventName), "City should " + (expected ? "" : "not") + " WillTrigger " + eventName);
		Assert(!_house.WillTrigger<MouseEvent>(eventName), "House should not WillTrigger " + eventName);
	}
	
	private void AssertOnlyCityHasEventListener(string eventName, bool expected)
	{
		Assert(!_country.HasEventListener<MouseEvent>(eventName), "Country should not have event listener for " + eventName);
		Assert(!_state.HasEventListener<MouseEvent>(eventName), "Stage should not have event listener for " + eventName);
		Assert(expected == _city.HasEventListener<MouseEvent>(eventName), "City should " + (expected ? "" : "not") + " have event listener for " + eventName);
		Assert(!_house.HasEventListener<MouseEvent>(eventName), "House should have event listener for " + eventName);
	}
	
	private void StackRecordingEventHandler(MouseEvent e){
		var currName = ((CinchSprite)e.CurrentTarget).Name;
		var phase = "AT:";
		if (e.EventPhase == EventPhase.CAPTURE_PHASE)
			phase = "CAP:";
		else if (e.EventPhase == EventPhase.BUBBLING_PHASE)
			phase = "BUB:";
		
		_eventRecord.Add (phase + currName);
	}	
	
	private void AssertEventRecordIs(string expectedRecord)
	{
		var actual = String.Join(",", _eventRecord.ToArray());
		Assert (expectedRecord == actual, "Event records did not match: expected: " + expectedRecord + "  actual: " + actual);
		_eventRecord = new List<string>();
	}
}
