using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cinch2D
{
	/// <summary>
	/// Internal class, no usable public methods.
	/// </summary>
	public class MouseInput : MonoBehaviour {
		
		private bool _mouseDown;
		private Vector2 _lastMousePos;
		private DisplayObjectContainer _mouseDownObject;
		private DisplayObjectContainer _mouseOverObject;
		
		protected List<DisplayObjectContainer> _mouseEnabledObjects;
		protected Dictionary<Guid, DisplayObjectContainer> _mouseEnabledObjectIds;
		public void __AddMouseEnabledObject(DisplayObjectContainer obj)
		{
			if (_mouseEnabledObjectIds.ContainsKey(obj.Id))
				return; 
			
			_mouseEnabledObjects.Add(obj);
			_mouseEnabledObjectIds.Add (obj.Id, obj);
			__SortMouseEnabledObjects();
		}
		public void __RemoveMouseEnabledObject(DisplayObjectContainer obj)
		{
			if (!_mouseEnabledObjectIds.ContainsKey(obj.Id))
				return; 
			
			_mouseEnabledObjects.Remove(obj);
			_mouseEnabledObjectIds.Remove(obj.Id);
		}	
		public void __SortMouseEnabledObjects()
		{
			//sort in descending, so that lowest Z value is at front of list
			_mouseEnabledObjects.Sort ((o1, o2) => o1.Z.CompareTo(o2.Z));
		}
		public int __GetNumberOfMouseEnabledObjects()
		{
			return _mouseEnabledObjects.Count;
		}
		
		private static MouseInput _instance;
		public static MouseInput Instance
		{
			get {
				return _instance;
			}
		}		
		
		public void Disable()
		{
			_mouseDownObject = null;
		}
		
		public void Enable()
		{
			_mouseDown = Input.GetMouseButton(0);
			_lastMousePos = Stage.Instance.TheCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
			_mouseOverObject = GetObjectUnderMouse();
		}
		
		public void Awake()
		{
			if (_instance != null)
				throw new Exception("You can only have one instance of MouseInput");
			
			_instance = this;
			
			_mouseEnabledObjects = new List<DisplayObjectContainer>();		
			_mouseEnabledObjectIds = new Dictionary<Guid, DisplayObjectContainer>();
			_lastMousePos = new Vector2(-999999, -999999);
			_mouseOverObject = Stage.Instance;
		}
		
		public void Update () {
			//just started
			if (_mouseOverObject == null)
				_mouseOverObject = GetObjectUnderMouse();
			
			var mouseDown = Input.GetMouseButton(0);
			var mousePos = Stage.Instance.TheCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
			var stagePos = new Vector2(mousePos.x, mousePos.y) - Stage.Instance.__GetOffset();
			var localPos = stagePos;
			if (_lastMousePos.x == -999999 && _lastMousePos.y == -999999)
				_lastMousePos = mousePos;
			
			var mouseMoved = (Vector2)_lastMousePos != (Vector2)mousePos;
			var mouseButtonChanged = _mouseDown != mouseDown;
			
			DisplayObjectContainer newMouseOverObject = (mouseMoved || mouseButtonChanged) ? 
																(GetObjectUnderMouse() ?? Stage.Instance) : 
																_mouseOverObject;
			
			if (mouseMoved)
			{
				localPos = newMouseOverObject.GlobalToLocal(stagePos);
				
				if (newMouseOverObject != _mouseOverObject)
				{
					if (_mouseOverObject != null)
					{
						var me = new MouseEvent(MouseEvent.MOUSE_OUT, _mouseOverObject, new Vector2(_mouseOverObject.MouseX, _mouseOverObject.MouseY), stagePos);
						me.Bubbles = false;
						_mouseOverObject.DispatchEvent(me); 
					}
					
					if (newMouseOverObject != null)
					{
						var me = new MouseEvent(MouseEvent.MOUSE_OVER, newMouseOverObject, localPos, stagePos);
						me.Bubbles = false;
						newMouseOverObject.DispatchEvent(me); 
					}			
				}
				
				//don't dispatch a move if we just entered or left
				if (newMouseOverObject == _mouseOverObject)
				{
					var me = new MouseEvent(MouseEvent.MOUSE_MOVE, newMouseOverObject, localPos, stagePos);
					me.Bubbles = false;
					newMouseOverObject.DispatchEvent(me); 
				}
			}
			
			if (mouseDown != _mouseDown)
			{
				if (mouseDown)
				{
					newMouseOverObject.DispatchEvent(new MouseEvent(MouseEvent.MOUSE_DOWN, newMouseOverObject, localPos, stagePos)); 
					_mouseDownObject = newMouseOverObject;
				}
				else
				{
					newMouseOverObject.DispatchEvent(new MouseEvent(MouseEvent.MOUSE_UP, newMouseOverObject, localPos, stagePos)); 
					
					if (newMouseOverObject != _mouseDownObject && _mouseDownObject != null)
						_mouseDownObject.DispatchEvent(new MouseEvent(MouseEvent.RELEASE_OUTSIDE, _mouseDownObject, localPos, stagePos)); 
					
					_mouseDownObject = null;
				}
				
			}
			
			_mouseDown = mouseDown;
			_lastMousePos = mousePos;
			_mouseOverObject = newMouseOverObject;
		}
		
		
		private DisplayObjectContainer GetObjectUnderMouse()
		{
			//if the mouse isn't
			var x = Stage.Instance.MouseX;
			var y = Stage.Instance.MouseY;
			var p = new Vector2(x, y);
			
			for (var i=0; i<_mouseEnabledObjects.Count; i++)
			{
				if (_mouseEnabledObjects[i].__Clickable && 
					_mouseEnabledObjects[i].__Level1HitTest(p) &&
					_mouseEnabledObjects[i].PointIsInMouseArea(p))
					return _mouseEnabledObjects[i];
			}
			
			return null;
		}
	}
}