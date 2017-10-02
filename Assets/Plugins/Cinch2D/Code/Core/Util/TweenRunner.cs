using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

namespace Cinch2D
{
	/// <summary>
	/// Internal class.
	/// </summary>
	public class TweenRunner : MonoBehaviour
	{
		public static TweenRunner Instance {get; private set;}
		private GameClock _clock;
		private bool _running;
		private List<Tween> _tweens;
		
	    public TweenRunner()
	    {
			_tweens = new List<Tween>();
			Instance = this;
	    }
		
		public void __Init(GameClock clock)
		{
			_clock = clock;
			_running = true;
		}

	    public void LateUpdate()
	    {
			if (!_running || _clock == null)
				return;
			
			for(var i = _tweens.Count - 1; i >= 0; i--)
			{
				if (_tweens[i].Complete || RunTween(_tweens[i]))
					_tweens.RemoveAt(i);
			}
	    }
		
		public float __GetCurrentTime()
		{
			return _clock.CurrTime;
		}
		
		public void __AddTween(Tween tween)
		{
			tween.StartTime = _clock.CurrTime;
			_tweens.Add (tween);
		}

	    private bool RunTween(Tween tween)
	    {
	        var t = tween.TargetObj.Clock.CurrTime - tween.StartTime;
			
			if (t < tween.Delay)
				return false;
			
			if (!tween.Started)
			{
				tween.Started = true;
				tween.__SetStartVal();
				if (tween.StartCallback != null)
					tween.StartCallback(tween.TargetObj);
			}
			
	        if (t >= tween.Duration + tween.Delay)
	        {
				tween.Complete = true;
				tween.Property.SetValue(tween.TargetObj, tween.EndVal, null);
				
				if (tween.EndCallback != null)
					tween.EndCallback(tween.TargetObj);
							
				if (tween.ChainedTween != null)
				{
					__AddTween(tween.ChainedTween);
				}
				
	            return true;
	        }

	        var val = tween.EasingType(t - tween.Delay, tween.StartVal, tween.EndVal - tween.StartVal, tween.Duration);
			tween.Property.SetValue(tween.TargetObj, val, null);
	        return false;
	    }
	}
}