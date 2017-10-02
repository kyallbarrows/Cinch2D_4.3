using UnityEngine;
using System.Collections;

namespace Cinch2D
{
	/*! Options class */
	public static class CinchOptions {
		
		/// <summary>
		/// The default viewport max dimension, in meters.  If portrait mode, it will be the height.  If landscape, the width.
		/// </summary>
		public static float DefaultViewportMaxDimension = 10f;
		
		/// <summary>
		/// The default pixels per meter for all Sprites.  100 pixels per meter with a 100x100 pixel texture will produce a 1x1 meter object
		/// </summary>
		public static float DefaultPixelsPerMeter = 100f;
		
		/// <summary>
		/// The use top left sprite sheet coordinates.  Coordinates are lower-left and up by default.  Setting to true causes coordinates to be top-down, like most image editors.
		/// </summary>
		public static bool UseTopLeftSpriteSheetCoordinates = true;
		
		/// <summary>
		/// The default for all objects.  Text fields use a different shader.
		/// </summary>
		public static string DefaultShader = "Unlit/AlphaSelfIllum";
		
		private static float _cameraPos = 0f;
		/// <summary>
		/// The z-position of the main camera in space.
		/// </summary>
		/// <value>
		/// The camera position.
		/// </value>
		public static float CameraPos { 
			get { return _cameraPos; }
			set { 
				_cameraPos = value; 
				if (Stage.Instance != null)
					Stage.Instance.__CameraPosChanged();
			}
		}
		
		/// <summary>
		/// The layer all Cinch2D objects will go on.  If you have a second camera, set its culling mask to CinchOptions.CinchLayer.
		/// </summary>
		private static int _cinchLayer = 31;
		public static int CinchLayer { 
			get { return _cinchLayer; }
			set { 
				_cinchLayer = value; 
				if (Stage.Instance != null)
					Stage.Instance.__CameraPosChanged();
			}
		}
		
		private static float _cameraObjectGap = 10f;
		/// <summary>
		/// How far away the camera is from the top-most object
		/// </summary>
		/// <value>
		/// The camera object gap.
		/// </value>
		public static float CameraObjectGap {
			get { return _cameraObjectGap; }
			set { 
				_cameraObjectGap = value;
				if (Stage.Instance != null)
					Stage.Instance.__CameraPosChanged();
			}
		}
		
		/// <summary>
		/// How far apart to put each layer, in meters.
		/// In order to better see how things are layed out in Scene view (when paused), you can set to a smaller value, but it may cause depth sorting issues with large objects
		/// </summary>
		public static float LayerSpacing = 1f;
		
		/// <summary>
		/// Set to false to turn mouse disabled by default.  This can be a significat performance boost with large numbers of objects.
		/// </summary>
		public static bool MouseEnabledByDefault = true;
	}
}