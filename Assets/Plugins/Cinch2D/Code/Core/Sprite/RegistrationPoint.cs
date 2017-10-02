using UnityEngine;
using System.Collections;

namespace Cinch2D
{
	/// <summary>
	/// Contains constant values for symbol RegistrationPoints.  See http://askville.amazon.com/Flash-registration-points/AnswerViewer.do?requestId=6721785
	/// </summary>
	public struct RegistrationPoint {
		public static RegistrationPoint TopLeft { get{ return new RegistrationPoint(0f, 1f); } private set{}}
		public static RegistrationPoint Top { get{ return new RegistrationPoint(0.5f, 1f); } private set{}}
		public static RegistrationPoint TopRight { get{ return new RegistrationPoint(1f, 1f); } private set{}}
		public static RegistrationPoint Left { get{ return new RegistrationPoint(0f, 0.5f); } private set{}}
		public static RegistrationPoint Center { get{ return new RegistrationPoint(0.5f, 0.5f); } private set{}}
		public static RegistrationPoint Right { get{ return new RegistrationPoint(1f, 0.5f); } private set{}}
		public static RegistrationPoint BottomLeft { get{ return new RegistrationPoint(0f, 0f); } private set{}}
		public static RegistrationPoint Bottom { get{ return new RegistrationPoint(0.5f, 0f); } private set{}}
		public static RegistrationPoint BottomRight { get{ return new RegistrationPoint(1f, 0f); } private set{}}
		
		public float X;
		public float Y;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="RegistrationPoint"/> struct.
		/// </summary>
		/// <param name='x'>
		/// Value between 0 and 1.  0 would be left side, 1 would be right side.
		/// </param>
		/// <param name='y'>
		/// Value between 0 and 1.  0 would be bottom side, 1 would be top side, unless useTopLeftCoordinates is set to true
		/// </param>
		/// <param name='useTopLeftCoordinates'>
		/// Use top-down coordinates rather than bottom-up
		/// </param>
		public RegistrationPoint(float x, float y, bool useTopLeftCoordinates = false)
		{
			X = x;
			Y = y;
			if (useTopLeftCoordinates)
				Y = 1f - y;
		}
	}
}