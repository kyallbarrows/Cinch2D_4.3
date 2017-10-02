using UnityEngine;
using System.Collections;
using System;

namespace Cinch2D
{
	/*
	 *
	 * TERMS OF USE - EASING EQUATIONS
	 * 
	 * Open source under the BSD License. 
	 * 
	 * Copyright © 2001 Robert Penner
	 * All rights reserved.
	 * 
	 * Redistribution and use in source and binary forms, with or without modification, 
	 * are permitted provided that the following conditions are met:
	 * 
	 * Redistributions of source code must retain the above copyright notice, this list of 
	 * conditions and the following disclaimer.
	 * Redistributions in binary form must reproduce the above copyright notice, this list 
	 * of conditions and the following disclaimer in the documentation and/or other materials 
	 * provided with the distribution.
	 * 
	 * Neither the name of the author nor the names of contributors may be used to endorse 
	 * or promote products derived from this software without specific prior written permission.
	 * 
	 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
	 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
	 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
	 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
	 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE
	 * GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED 
	 * AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
	 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
	 * OF THE POSSIBILITY OF SUCH DAMAGE. 
	 *
	 */
	public static class Easing {
	    public static class None
	    {
		// ==================================================================================================================================
		// TWEENING EQUATIONS functions -----------------------------------------------------------------------------------------------------
		// (the original equations are Robert Penner's work as mentioned on the disclaimer)

			/**
			 * Easing equation function for a simple linear tweening, with no easing.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseNone (float t, float b, float c, float d) {
				return c*t/d + b;
			}
		}
		
		public static class Quad
		{
			/**
			 * Easing equation function for a quadratic (t^2) easing in: accelerating from zero velocity.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseIn (float t, float b, float c, float d) {
				return c*(t/=d)*t + b;
			}
		
			/**
			 * Easing equation function for a quadratic (t^2) easing out: decelerating to zero velocity.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOut (float t, float b, float c, float d) {
				return -c *(t/=d)*(t-2) + b;
			}
		
			/**
			 * Easing equation function for a quadratic (t^2) easing in/out: acceleration until halfway, then deceleration.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseInOut (float t, float b, float c, float d) {
				if ((t/=d/2) < 1) return c/2*t*t + b;
				return -c/2 * ((--t)*(t-2) - 1) + b;
			}
		
			/**
			 * Easing equation function for a quadratic (t^2) easing out/in: deceleration until halfway, then acceleration.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOutIn (float t, float b, float c, float d) {
				if (t < d/2) return EaseOut (t*2, b, c/2, d);
				return EaseIn((t*2)-d, b+c/2, c/2, d);
			}
		}
		
		public static class Cubic
		{
			/**
			 * Easing equation function for a cubic (t^3) easing in: accelerating from zero velocity.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseIn (float t, float b, float c, float d) {
				return c*(t/=d)*t*t + b;
			}
		
			/**
			 * Easing equation function for a cubic (t^3) easing out: decelerating from zero velocity.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOut (float t, float b, float c, float d) {
				return c*((t=t/d-1)*t*t + 1) + b;
			}
		
			/**
			 * Easing equation function for a cubic (t^3) easing in/out: acceleration until halfway, then deceleration.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseInOut (float t, float b, float c, float d) {
				if ((t/=d/2) < 1) return c/2*t*t*t + b;
				return c/2*((t-=2)*t*t + 2) + b;
			}
		
			/**
			 * Easing equation function for a cubic (t^3) easing out/in: deceleration until halfway, then acceleration.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOutIn (float t, float b, float c, float d) {
				if (t < d/2) return EaseOut (t*2, b, c/2, d);
				return EaseIn((t*2)-d, b+c/2, c/2, d);
			}
		}
		
		public static class Quart
		{
			/**
			 * Easing equation function for a quartic (t^4) easing in: accelerating from zero velocity.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseIn (float t, float b, float c, float d) {
				return c*(t/=d)*t*t*t + b;
			}
		
			/**
			 * Easing equation function for a quartic (t^4) easing out: decelerating from zero velocity.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOut (float t, float b, float c, float d) {
				return -c * ((t=t/d-1)*t*t*t - 1) + b;
			}
		
			/**
			 * Easing equation function for a quartic (t^4) easing in/out: acceleration until halfway, then deceleration.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseInOut (float t, float b, float c, float d) {
				if ((t/=d/2) < 1) return c/2*t*t*t*t + b;
				return -c/2 * ((t-=2)*t*t*t - 2) + b;
			}
		
			/**
			 * Easing equation function for a quartic (t^4) easing out/in: deceleration until halfway, then acceleration.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOutIn (float t, float b, float c, float d) {
				if (t < d/2) return EaseOut (t*2, b, c/2, d);
				return EaseIn((t*2)-d, b+c/2, c/2, d);
			}
		}
		
		public static class Quint
		{
			/**
			 * Easing equation function for a quintic (t^5) easing in: accelerating from zero velocity.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseIn (float t, float b, float c, float d) {
				return c*(t/=d)*t*t*t*t + b;
			}
		
			/**
			 * Easing equation function for a quintic (t^5) easing out: decelerating from zero velocity.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOut (float t, float b, float c, float d) {
				return c*((t=t/d-1)*t*t*t*t + 1) + b;
			}
		
			/**
			 * Easing equation function for a quintic (t^5) easing in/out: acceleration until halfway, then deceleration.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseInOut (float t, float b, float c, float d) {
				if ((t/=d/2) < 1) return c/2*t*t*t*t*t + b;
				return c/2*((t-=2)*t*t*t*t + 2) + b;
			}
		
			/**
			 * Easing equation function for a quintic (t^5) easing out/in: deceleration until halfway, then acceleration.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOutIn (float t, float b, float c, float d) {
				if (t < d/2) return EaseOut (t*2, b, c/2, d);
				return EaseIn((t*2)-d, b+c/2, c/2, d);
			}
		}
		
		public static class Sine
		{
			/**
			 * Easing equation function for a sinusoidal (sin(t)) easing in: accelerating from zero velocity.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseIn (float t, float b, float c, float d) {
				return -c * (float)Math.Cos(t/d * (Math.PI/2)) + c + b;
			}
		
			/**
			 * Easing equation function for a sinusoidal (sin(t)) easing out: decelerating from zero velocity.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOut (float t, float b, float c, float d) {
				return c * (float)Math.Sin(t/d * (Math.PI/2)) + b;
			}
		
			/**
			 * Easing equation function for a sinusoidal (sin(t)) easing in/out: acceleration until halfway, then deceleration.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseInOut (float t, float b, float c, float d) {
				return -c/2 * ((float)Math.Cos(Math.PI*t/d) - 1) + b;
			}
		
			/**
			 * Easing equation function for a sinusoidal (sin(t)) easing out/in: deceleration until halfway, then acceleration.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOutIn (float t, float b, float c, float d) {
				if (t < d/2) return EaseOut (t*2, b, c/2, d);
				return EaseIn((t*2)-d, b+c/2, c/2, d);
			}
		}
		
		public static class Expo
		{	
			/**
			 * Easing equation function for an exponential (2^t) easing in: accelerating from zero velocity.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseIn (float t, float b, float c, float d) {
				return (t==0) ? b : c * (float)Math.Pow(2, 10 * (t/d - 1)) + b - c * 0.001f;
			}
		
			/**
			 * Easing equation function for an exponential (2^t) easing out: decelerating from zero velocity.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOut (float t, float b, float c, float d) {
				return (t==d) ? b+c : c * 1.001f * (float)(-Math.Pow(2, -10 * t/d) + 1) + b;
			}
		
			/**
			 * Easing equation function for an exponential (2^t) easing in/out: acceleration until halfway, then deceleration.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseInOut (float t, float b, float c, float d) {
				if (t==0) return b;
				if (t==d) return b+c;
				if ((t/=d/2) < 1) return c/2 * (float)Math.Pow(2, 10 * (t - 1)) + b - c * 0.0005f;
				return c/2 * 1.0005f * (float)(-Math.Pow(2, -10 * --t) + 2) + b;
			}
		
			/**
			 * Easing equation function for an exponential (2^t) easing out/in: deceleration until halfway, then acceleration.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOutIn (float t, float b, float c, float d) {
				if (t < d/2) return EaseOut (t*2, b, c/2, d);
				return EaseIn((t*2)-d, b+c/2, c/2, d);
			}
		}
		
		public static class Circle
		{
			/**
			 * Easing equation function for a circular (sqrt(1-t^2)) easing in: accelerating from zero velocity.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseIn (float t, float b, float c, float d) {
				return -c * (float)(Math.Sqrt(1 - (t/=d)*t) - 1) + b;
			}
		
			/**
			 * Easing equation function for a circular (sqrt(1-t^2)) easing out: decelerating from zero velocity.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOut (float t, float b, float c, float d) {
				return c * (float)Math.Sqrt(1 - (t=t/d-1)*t) + b;
			}
		
			/**
			 * Easing equation function for a circular (sqrt(1-t^2)) easing in/out: acceleration until halfway, then deceleration.
	 		 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseInOut (float t, float b, float c, float d) {
				if ((t/=d/2) < 1) return -c/2 * (float)(Math.Sqrt(1 - t*t) - 1) + b;
				return c/2 * (float)(Math.Sqrt(1 - (t-=2)*t) + 1) + b;
			}
		
			/**
			 * Easing equation function for a circular (sqrt(1-t^2)) easing out/in: deceleration until halfway, then acceleration.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOutIn (float t, float b, float c, float d) {
				if (t < d/2) return EaseOut (t*2, b, c/2, d);
				return EaseIn((t*2)-d, b+c/2, c/2, d);
			}
		}
		
		public static class Elastic
		{
			/**
			 * Easing equation function for an elastic (exponentially decaying sine wave) easing in: accelerating from zero velocity.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @param a		Amplitude.
			 * @param p		Period.
			 * @return		The correct value.
			 */
			public static float EaseIn (float t, float b, float c, float d)
			{
			    return EaseIn(t, b, c, d, -9999, -9999);
			}

		    public static float EaseIn (float t, float b, float c, float d, float a, float p) {
				if (t==0) return b;  if ((t/=d)==1) return b+c;  if (p == -9999) p=d*.3f;
			    float s;
				if (a == -9999 || a < (float)Math.Abs(c)) { a=c; s=p/4; }
				else s = p/(2*(float)Math.PI) * (float)Math.Asin (c/a);
				return -(a*(float)Math.Pow(2,10*(t-=1)) * (float)Math.Sin( (t*d-s)*(2*Math.PI)/p )) + b;
			}
		
			/**
			 * Easing equation function for an elastic (exponentially decaying sine wave) easing out: decelerating from zero velocity.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @param a		Amplitude.
			 * @param p		Period.
			 * @return		The correct value.
			 */
			public static float EaseOut (float t, float b, float c, float d)
			{
			    return EaseOut(t, b, c, d, -9999, -9999);
			}

			public static float EaseOut (float t, float b, float c, float d, float a, float p) {
				if (t==0) return b;  if ((t/=d)==1) return b+c;  if (p == -9999) p=d*.3f;
			    float s;
				if (a == -9999 || a < Math.Abs(c)) { a=c; s=p/4; }
				else s = p/(2*(float)Math.PI) * (float)Math.Asin (c/a);
				return (a*(float)Math.Pow(2,-10*t) * (float)Math.Sin( (t*d-s)*(2*Math.PI)/p ) + c + b);
			}
		
			/**
			 * Easing equation function for an elastic (exponentially decaying sine wave) easing in/out: acceleration until halfway, then deceleration.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @param a		Amplitude.
			 * @param p		Period.
			 * @return		The correct value.
			 */
	        public static float EaseInOut (float t, float b, float c, float d, float a, float p) {
	            if (t==0) return b;  if ((t/=d/2)==2) return b+c;  if (p == -9999) p=d*(.3f*1.5f);
			    float s;
	            if (a == -9999 || a < Math.Abs(c)) { a=c; s=p/4; }
	            else s = p/(2*(float)Math.PI) * (float)Math.Asin (c/a);
	            if (t < 1) return -.5f*(a*(float)Math.Pow(2,10*(t-=1)) * (float)Math.Sin( (t*d-s)*(2*(float)Math.PI)/p )) + b;
	            return a*(float)Math.Pow(2f,-10f*(t-=1)) * (float)Math.Sin( (t*d-s)*(2*(float)Math.PI)/p )*.5f + c + b;
	        }
		
			/**
			 * Easing equation function for an elastic (exponentially decaying sine wave) easing out/in: deceleration until halfway, then acceleration.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @param a		Amplitude.
			 * @param p		Period.
			 * @return		The correct value.
			 */
	        public static float EaseOutIn (float t, float b, float c, float d, float a, float p) {
	            if (t < d/2) return EaseOut (t*2, b, c/2, d, a, p);
	            return EaseIn((t*2)-d, b+c/2, c/2, d, a, p);
	        }
		
		}
		
		public static class Back
		{
			/**
			 * Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: accelerating from zero velocity.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @param s		Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
			 * @return		The correct value.
			 */
	        public static float EaseIn (float t, float b, float c, float d) {
	            float overshoot = 1.70158f;
	            return c*(t/=d)*t*((overshoot+1)*t - overshoot) + b;
	        }
		
			/**
			 * Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: decelerating from zero velocity.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @param s		Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
			 * @return		The correct value.
			 */
	        public static float EaseOut (float t, float b, float c, float d) {
	            float overshoot = 1.70158f;
	            return c*((t=t/d-1)*t*((overshoot+1)*t + overshoot) + 1) + b;
	        }
		
			/**
			 * Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: acceleration until halfway, then deceleration.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @param s		Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
			 * @return		The correct value.
			 */
	        public static float EaseInOut (float t, float b, float c, float d) {
	            float overshoot = 1.70158f;
	            if ((t/=d/2) < 1) return c/2*(t*t*(((overshoot*=(1.525f))+1)*t - overshoot)) + b;
	            return c/2*((t-=2)*t*(((overshoot*=(1.525f))+1)*t + overshoot) + 2) + b;
	        }
		
			/**
			 * Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out/in: deceleration until halfway, then acceleration.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @param s		Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
			 * @return		The correct value.
			 */
	        public static float EaseOutIn (float t, float b, float c, float d) {
	            if (t < d/2) return EaseOut (t*2, b, c/2, d);
	            return EaseIn((t*2)-d, b+c/2, c/2, d);
	        }
		}
		
		public static class Bounce
		{
			/**
			 * Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: accelerating from zero velocity.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseIn (float t, float b, float c, float d) {
				return c - EaseOut (d-t, 0, c, d) + b;
			}
		
			/**
			 * Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: decelerating from zero velocity.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOut (float t, float b, float c, float d) {
				if ((t/=d) < (1/2.75)) {
					return c*(7.5625f*t*t) + b;
				} else if (t < (2/2.75)) {
					return c*(7.5625f*(t-=(1.5f/2.75f))*t + .75f) + b;
				} else if (t < (2.5/2.75)) {
					return c*(7.5625f*(t-=(2.25f/2.75f))*t + .9375f) + b;
				} else {
					return c*(7.5625f*(t-=(2.625f/2.75f))*t + .984375f) + b;
				}
			}
		
			/**
			 * Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in/out: acceleration until halfway, then deceleration.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseInOut (float t, float b, float c, float d) {
				if (t < d/2) return EaseIn (t*2, 0, c, d) * .5f + b;
				else return EaseOut (t*2-d, 0, c, d) * .5f + c*.5f + b;
			}
		
			/**
			 * Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out/in: deceleration until halfway, then acceleration.
			 *
			 * @param t		Current time (in frames or seconds).
			 * @param b		Starting value.
			 * @param c		Change needed in value.
			 * @param d		Expected easing duration (in frames or seconds).
			 * @return		The correct value.
			 */
			public static float EaseOutIn (float t, float b, float c, float d) {
				if (t < d/2) return EaseOut (t*2, b, c/2, d);
				return EaseIn((t*2)-d, b+c/2, c/2, d);
			}
		}	
	}
}