using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cinch2D
{
	/*! Base class for anything that dispatches events */
	public class EventDispatcher : MonoBehaviour {
		
		/// <summary>
		/// A unique ID for this instance.
		/// </summary>
	    public Guid Id;	
		
		/// <summary>
		/// Instance Name, similar to Flash's Sprite.Name.  Defaults to empty string.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; set; }
	    public override int GetHashCode()
	    {
	        return Id.ToString().GetHashCode();
	    }
		
		protected class EventListener<T> where T:CinchEvent
		{
			public EventDispatcher Listener;
			public Action<T> Handler;
			public bool UseCapture;
			public int Priority;
		}
		
		private Dictionary<string, object> _listeners;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="EventDispatcher"/> class.
		/// </summary>
		public EventDispatcher()
		{
			Id = Guid.NewGuid();
			_listeners = null;
		}
			
		private string GetEventTypeId(string className, string type)
		{
			return className + "." + type;
		}
		
		/// <summary>
		/// Adds an event listener.
		/// </summary>
		/// <param name='type'>
		/// The string type of the event, such as MouseEvent.MOUSE_DOWN.
		/// </param>
		/// <param name='handler'>
		/// A function with no return value, which takes an event of type T as its only parameter.
		/// </param>
		/// <param name='useCapture'>
		/// Whether to listen on capture phase or bubbling phase.
		/// </param>
		/// <param name='priority'>
		/// Priority.
		/// </param>
		/// <typeparam name='T'>
		/// Any subclass of CinchEvent, such as MouseEvent.
		/// </typeparam>
		/// <example>
		/// private void HandleButtonPress(MouseEvent e)
		/// {
		/// 	Sprite button = (Sprite)e.CurrentTarget;
		/// 	Debug.Log("You pressed " + button.Name);
		/// }
		/// buttonObj.AddEventListner<MouseEvent>(MouseEvent.MOUSE_DOWN, handleButtonPress);
		/// </example>
		public void AddEventListener<T>(string type, Action<T> handler, bool useCapture = false, int priority = 0) where T:CinchEvent
		{
			if (_listeners == null)
				_listeners = new Dictionary<string, object>();
			
			var typeId = GetEventTypeId(typeof(T).FullName, type);
			
			if (!_listeners.ContainsKey(typeId))
				_listeners.Add(typeId, new List<EventListener<T>>());
			
			var thisSet = _listeners[typeId] as List<EventListener<T>>;
			thisSet.Add(new EventListener<T>(){Listener = this, Handler = handler, Priority = priority, UseCapture = useCapture});
			thisSet.Sort((x, y) => x.Priority.CompareTo(y.Priority));
		}
		
		/// <summary>
		/// Removes an event listener.
		/// </summary>
		/// <param name='type'>
		/// The type of event to remove listener for, such as MouseEvent.MOUSE_DOWN.
		/// </param>
		/// <param name='handler'>
		/// The handler function to stop calling.
		/// </param>
		/// <param name='useCapture'>
		/// Specifies whether to remove listener for capture or bubbling.
		/// </param>
		/// <typeparam name='T'>
		/// The CinchEvent subclass to remove listener for.
		/// </typeparam>
		/// <example>
		/// buttonObj.AddEventListner<MouseEvent>(MouseEvent.MOUSE_DOWN, handleButtonPress);	//let's listen for mouse clicks!
		/// buttonObj.RemoveEventListner<MouseEvent>(MouseEvent.MOUSE_DOWN, handleButtonPress);	//LOLJK let's not.
		/// </example>
		public void RemoveEventListener<T>(string type, Action<T> handler, bool useCapture = false) where T:CinchEvent
		{
			var typeId = GetEventTypeId(typeof(T).FullName, type);
			if (_listeners == null || !_listeners.ContainsKey(typeId))
				return;
			
			var thisSet = _listeners[typeId] as List<EventListener<T>>;
			thisSet.RemoveAll((lnr) => { return lnr.UseCapture == useCapture && lnr.Handler == handler; });
		}
		
		/// <summary>
		/// Determines whether this instance has an event listener of the specified type.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance has event listener the specified type; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='type'>
		/// The type of event to remove listener for, such as MouseEvent.MOUSE_DOWN.
		/// </param>
		/// <typeparam name='T'>
		/// The CinchEvent subclass to remove listener for.
		/// </typeparam>
		/// <example>
		/// buttonObj.AddEventListner<MouseEvent>(MouseEvent.MOUSE_DOWN, handleButtonPress);	
		/// Debug.Log(buttonObj.HasEventListener<MouseEvent>(MouseEvent.MOUSE_DOWN));				//output: "true"
		/// </example>
		public bool HasEventListener<T>(string type) where T:CinchEvent
		{
			var typeId = GetEventTypeId(typeof(T).FullName, type);
			if (_listeners != null && _listeners.ContainsKey(typeId))
			{
				var thisSet = _listeners[typeId] as List<EventListener<T>>;
				return (thisSet.Any ());
			}
			
			return false;
		}
		
		/// <summary>
		/// Determines whether this instance OR any of its children has an event listener of the specified type.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance has event listener the specified type; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='type'>
		/// The type of event to remove listener for, such as MouseEvent.MOUSE_DOWN.
		/// </param>
		/// <typeparam name='T'>
		/// The CinchEvent subclass to remove listener for.
		/// </typeparam>
		/// <example>
		/// someChildObject.AddEventListner<MouseEvent>(MouseEvent.MOUSE_DOWN, handleButtonPress);	
		/// Debug.Log(parentOfSomeChildObject.WillTrigger<MouseEvent>(MouseEvent.MOUSE_DOWN));				//output: "true"
		/// </example>
		public virtual bool WillTrigger<T>(string type) where T:CinchEvent
		{
			return HasEventListener<T>(type);
			
			//displayObjectContainer overrides and deals with children
		}
		
		/// <summary>
		/// Dispatchs the event.
		/// </summary>
		/// <param name='e'>
		/// A CinchEvent or subclass to dispatch.
		/// </param>
		/// <typeparam name='T'>
		/// A CinchEvent or subclass to dispatch.
		/// </typeparam>
		/// <example>
		/// someObjThatJustGotDoneWithSomething.DispatchEvent<CinchEvent>(new CinchEvent(CinchEvent.COMPLETE, someObjThatJustGotDoneWithSomething));
		/// </example>
		public void DispatchEvent<T>(T e) where T:CinchEvent
		{
			//walk the parent() chain and grab all the dispatchers
			var chain = new List<EventDispatcher>();
			var currDispatcher = this;
			chain.Add(currDispatcher);
			while (e.Bubbles && currDispatcher is DisplayObjectContainer && ((DisplayObjectContainer)currDispatcher).Parent != null)
			{
				currDispatcher = ((DisplayObjectContainer)currDispatcher).Parent;
				chain.Add(currDispatcher);
			}
			
			//walk chain from top to bottom, and call processEventDispatch with the event in capture mode
			e.EventPhase = EventPhase.CAPTURE_PHASE;
			for (var i=chain.Count - 1; i>0 && !e.GetStoppedPropagation(); i--)
			{
				e.CurrentTarget = chain[i];
				chain[i].__ProcessEventDispatch<T>(e);
			}
			
			//call dispatchEvent locally with AtTarget mode
			e.EventPhase = EventPhase.AT_TARGET;
			e.CurrentTarget = this;
			if (!e.GetStoppedPropagation())
				__ProcessEventDispatch<T>(e);
			
			//walk back up, and call DispatchEvent with Bubble mode
			e.EventPhase = EventPhase.BUBBLING_PHASE;
			
			for (var i=1; i < chain.Count && !e.GetStoppedPropagation(); i++)
			{
				e.CurrentTarget = chain[i];
				chain[i].__ProcessEventDispatch<T>(e);
			}
		}
		
		public void __ProcessEventDispatch<T>(T e) where T:CinchEvent
		{
			var typeId = GetEventTypeId(e.GetType().FullName, e.Type);
			if (_listeners == null || !_listeners.ContainsKey(typeId))
				return;
			
			var thisSet = (List<EventListener<T>>)_listeners[typeId];
			foreach (var lnr in thisSet)
			{
				if (e.EventPhase == EventPhase.AT_TARGET || (lnr.UseCapture == (e.EventPhase == EventPhase.CAPTURE_PHASE)))
					lnr.Handler(e);
			}
		}
		
	    public override bool Equals(object o)
	    {
	        if (!(o is EventDispatcher))
	        {
	            return false;
	        }

	        return ((EventDispatcher)o).Id == Id;
	    }
	}
}