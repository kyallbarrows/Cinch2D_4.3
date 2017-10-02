using UnityEngine;
using System.Collections;

namespace Cinch2D
{
	/// <summary>
	/// Defines constants for EventPhases.  See http://www.adobe.com/devnet/actionscript/articles/event_handling_as3.html#articlecontentAdobe_numberedheader_0
	/// </summary>
	public class EventPhase {
		/// <summary>
		/// Constant for Capturing phase.  See http://www.adobe.com/devnet/actionscript/articles/event_handling_as3.html#articlecontentAdobe_numberedheader_0
		/// </summary>
		public const uint CAPTURE_PHASE = 1;
		/// <summary>
		/// Constant for At-target phase.  See http://www.adobe.com/devnet/actionscript/articles/event_handling_as3.html#articlecontentAdobe_numberedheader_0
		/// </summary>
		public const uint AT_TARGET = 2;
		/// <summary>
		/// Constant for Bubbling phase.  See http://www.adobe.com/devnet/actionscript/articles/event_handling_as3.html#articlecontentAdobe_numberedheader_0
		/// </summary>
		public const uint BUBBLING_PHASE = 3;
	}
}