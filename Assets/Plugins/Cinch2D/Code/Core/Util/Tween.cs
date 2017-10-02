using System;
using UnityEngine;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace Cinch2D
{
	/*! Tweening class.  Does not currently support .COMPLETE events, use EndCallback instead */
	public class Tween
	{
	    public Guid Id;
		public DisplayObjectContainer TargetObj;
		public PropertyInfo Property;
		public Func<float, float> PropSetter;
	    public float StartVal;
	    public float EndVal;
	    public float StartTime;
	    public float Duration;
	    public EaseValue EasingType;
		public bool Started;
		public bool Complete;
		public float Delay;
		public Tween ChainedTween;
		public TweenCallback StartCallback;
		public TweenCallback EndCallback;
		
		public delegate float EaseValue(float t, float b, float c, float d);
		public delegate void CinchEventHandler(CinchEvent e);
		public delegate void TweenCallback(DisplayObjectContainer TargetObj);
		
		public override bool Equals(object o)
		{
			if (o.GetType() != typeof (Tween))
				return false;
				
			return ((Tween)o).Id == Id;
		}
		
	    public override int GetHashCode()
	    {
	        return Id.ToString().GetHashCode();
	    }	
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Tween"/> class.
		/// </summary>
		/// <param name='targetObj'>
		/// The objet to perform the Tween on.
		/// </param>
		/// <param name='propertyName'>
		/// Property name, eg. "X", "Y", "ScaleX", "Alpha", etc.
		/// </param>
		/// <param name='endVal'>
		/// The value to tween to.  The tween will run from the property's value at start time to the end value.
		/// </param>
		/// <param name='duration'>
		/// Duration in seconds of the tween.
		/// </param>
		/// <param name='easingType'>
		/// Easing type.  Any function matching the see cref="EaseValue"/> delegate.  
		/// </param>
		/// <param name='delay'>
		/// Delay, in seconds.
		/// </param>
		/// <param name='startCallback'>
		/// Called when tween starts (after any delay).  
		/// </param>
		/// <param name='endCallback'>
		/// Called when tween ends
		/// </param>
		/// <exception cref='Exception'>
		/// Thrown when a Tween is created before Stage
		/// </exception>
		/// <exception cref='ArgumentException'>
		/// Thrown when targetObj param is null or targetObj does not have property referenced by propertyName arg.
		/// </exception>
		public Tween(DisplayObjectContainer targetObj, string propertyName, float endVal, float duration, EaseValue easingType, float delay = 0f, TweenCallback startCallback = null, TweenCallback endCallback = null)
		{
			if (TweenRunner.Instance == null)
				throw new Exception("TweenRunner must be added to Stage before creating Tweens");
			
			if (targetObj == null)
				throw new ArgumentException("targetObj is null");
			
			Id = Guid.NewGuid();
			TargetObj = targetObj;
			if (!String.IsNullOrEmpty(propertyName))
			{
				Property = targetObj.GetType().GetProperty(propertyName);
				if (Property == null)
					throw new ArgumentException("targetObj does not have property named " + propertyName);
			}
			//StartVal gets set when tween actually starts running, since the value may change during a delay
			EndVal = endVal;
			StartTime = TweenRunner.Instance.__GetCurrentTime();
			Duration = duration;
			EasingType = easingType;
			Delay = delay;
			StartCallback = startCallback;
			EndCallback = endCallback;
			
			if (!String.IsNullOrEmpty(propertyName))
				TweenRunner.Instance.__AddTween(this);
		}

		public void __SetStartVal()
		{
			StartVal = (float)Property.GetValue(TargetObj, null);
		}
		
		/// <summary>
		/// Continues to second value after tween when complete
		/// </summary>
		/// <returns>
		/// The tween
		/// </returns>
		/// <param name='endVal'>
		/// The new value to tween to.
		/// </param>
		/// <param name='duration'>
		/// The duration of the tween in seconds.
		/// </param>
		/// <param name='easingType'>
		/// Easing type.
		/// </param>
		/// <param name='delay'>
		/// Delay in seconds.
		/// </param>
		public Tween ContinueTo(float endVal, float duration, EaseValue easingType, float delay = 0f)
		{
			var tw = new Tween(TargetObj, "", endVal, duration, easingType, delay);
			tw.Property = Property;
			this.ChainedTween = tw;
			return tw;
		}
		
		
		public void Stop()
		{
			Complete = true;
		}	
		
		#region Some often used callbacks
		public static void MakeObjectVisible(DisplayObjectContainer targetObj)
		{
			targetObj.Visible = true;
		}
		
		public static void MakeObjectInvisible(DisplayObjectContainer targetObj)
		{
			targetObj.Visible = false;
		}
		
		#endregion
	}
}