using UnityEngine;
using System.Collections;

namespace Cinch2D
{
	/*! A clock that controls tweens */
	public class GameClock {
		
		/// <summary>
		/// An optional name for your clock, such as GameplayClock, or MenuClock.  Or Steve.  Steve is a good name.
		/// </summary>
		public string Name;
		
		/// <summary>
		/// Get/Set whether clock is paused.
		/// </summary>
	    public bool Paused = false;
		
		/// <summary>
		/// The elapsed time in seconds since clock started, not counting any time clock was paused.
		/// </summary>
	    public float CurrTime;
		
		private float _elapsed = 0f;
		
		/// <summary>
		/// Gets the time elapsed in seconds since the draw cycle.
		/// </summary>
		/// <value>
		/// The delta time in seconds.
		/// </value>
		public float DeltaTime { get { return Paused ? 0f : _elapsed; }}
		
		public GameClock(string name)
		{
			Name = name;
		}
		
		public void __Tick () {
	        if (!Paused)
			{
				_elapsed = Time.deltaTime;
	            CurrTime += _elapsed;
			}
		}
	}
}