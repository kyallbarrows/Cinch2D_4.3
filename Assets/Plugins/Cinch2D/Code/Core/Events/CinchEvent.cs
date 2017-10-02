using UnityEngine;
using System.Collections;
using System;

namespace Cinch2D
{		
	/*! Base class for all events */
	public class CinchEvent {
		
		public const string COMPLETE = "CinchEvent.COMPLETE";
		
		/// <summary>
		/// The event type, eg MOUSE_DOWN or COMPLETE
		/// </summary>
		public string Type;
		
		/// <summary>
		/// The object that dispatched the event.
		/// </summary>
		public object Target;
		
		/// <summary>
		/// The object that is currently processing the event.
		/// </summary>
		public object CurrentTarget;
		
		/// <summary>
		/// The event phase: Capturing, at-target, or bubbling.  See http://www.adobe.com/devnet/actionscript/articles/event_handling_as3.html#articlecontentAdobe_numberedheader_0
		/// </summary>
		public uint EventPhase;
		
		/// <summary>
		/// Whether the event can be canceled to stop further propagation.
		/// </summary>
		public bool Cancelable = true;
		
		/// <summary>
		/// Whether the event bubbles.  See http://www.adobe.com/devnet/actionscript/articles/event_handling_as3.html#articlecontentAdobe_numberedheader_0
		/// </summary>
		public bool Bubbles = true;
		
		private bool _stopPropagation;
		
		/**
	       * Stops an event from propagating any further (up or down the event chain)
	       */	
		public void StopPropagation(){ if (Cancelable) _stopPropagation = true; }

		/**
	       * Checks if propagation has been stopped
	       * @return Whether propagation has been stopped
	       */	
		public bool GetStoppedPropagation() { return _stopPropagation; }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="CinchEvent"/> class.
		/// </summary>
		/// <param name='type'>
		/// Type.
		/// </param>
		/// <param name='target'>
		/// Target.  The object that the event happened on, oftentimes just pass <c>this</c>.
		/// </param>
		public CinchEvent(string type, object target)
		{
			Type = type;
			Target = target;
		}
	}
}