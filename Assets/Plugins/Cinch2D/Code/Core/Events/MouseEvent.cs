using UnityEngine;
using System.Collections;

namespace Cinch2D
{
	/*! Event dispatched when any mouse event occurs */
	public class MouseEvent : CinchEvent {
		
		public const string MOUSE_DOWN = "mouseevent.mousedown";
		public const string MOUSE_MOVE = "mouseevent.mousemove";
		public const string MOUSE_UP = "mouseevent.mouseup";
		public const string RELEASE_OUTSIDE = "mouseevent.mouseupoutside";
		public const string MOUSE_OVER = "mosueevent.mouseover";
		public const string MOUSE_OUT = "mosueevent.mouseout";
		
		public Vector2 LocalPos;	/*! Mouse position relative to .CurrentTarget */
		public Vector2 StagePos;	/*! Mouse position relative to Stage */
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MouseEvent"/> class.  You would almost never dispatch this, as it is used internally to handle mouse events.
		/// </summary>
		/// <param name='type'>
		/// Type.  One of the type constants defined on this class.
		/// </param>
		/// <param name='target'>
		/// Target.
		/// </param>
		/// <param name='localPos'>
		/// Local position.
		/// </param>
		/// <param name='stagePos'>
		/// Stage position.
		/// </param>
		public MouseEvent(string type, object target, Vector2 localPos, Vector2 stagePos) : base(type, target)
		{
			LocalPos = localPos;
			StagePos = stagePos;
		}
	}
}