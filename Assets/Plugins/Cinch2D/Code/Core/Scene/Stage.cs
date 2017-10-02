using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using FarseerPhysics.Dynamics;


namespace Cinch2D
{
	/*! Base class for all Cinch projects.  See examples projects. */
	public class Stage : DisplayObjectContainer {
		
		private static Stage _instance;
		/// <summary>
		/// Gets the single instance of Stage.  Only one instance is allowed at any time.
		/// </summary>
		public static Stage Instance
		{
			get {
				return _instance;
			}
		}	
		
		/// <summary>
		/// Core method.  Calling this could produce very bad results.
		/// </summary>
		public void __CameraPosChanged()
		{
			if (_camera != null)
			{
				_camera.transform.position = new Vector3(_camera.transform.position.x, _camera.transform.position.y, CinchOptions.CameraPos);
				_camera.cullingMask = ~(CinchOptions.CinchLayer);
				_camera.farClipPlane = CinchOptions.CameraPos + 16000;		
			}		
		}
		
		protected Camera _camera;
		/// <summary>
		/// Gets the main Cinch camera.  Useful for setting backgrounds, skyboxes, etc.  WARNING: Changing .orthographic to false will produce very bad results.
		/// </summary>
		/// <value>
		/// The main Cinch camera.
		/// </value>
		public Camera TheCamera{ get{ return _camera; }}
		
		protected GameObject _holder;
		
		protected override void SetAsTransformChild(GameObject go)
		{
			go.transform.parent = _holder.transform;
		}
		
		
		//the position of the stage relative to the camera.  FLash lower left corner is 0, 0.  Unity lower left is 0 - viewport(width/height)/2
		protected Vector2 _stageOffset;
		
		/// <summary>
		/// Core method.  Calling this could produce very bad results.
		/// </summary>
		public Vector2 __GetOffset(){ return _stageOffset; }

		/// <summary>
		/// Gets or sets the width of the viewport.  Uses meters, not pixels.  Setting ViewportWidth will change ViewportHeight.
		/// </summary>
		/// <value>
		/// The width of the viewport.
		/// </value>
		public float ViewportWidth { 
			get { return 2f * _camera.orthographicSize * _camera.aspect; }
			set { 
				_camera.orthographicSize = value / (2f * _camera.aspect); 
				var worldPos = new Vector3(0f, 0f, 0f);
				_holder.transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
				_stageOffset = new Vector2(worldPos.x, worldPos.y);
			}
		}
		/// <summary>
		/// Gets or sets the height of the viewport.  Uses meters, not pixels.  Setting ViewportHeight will change ViewportWidth.
		/// </summary>
		/// <value>
		/// The height of the viewport.
		/// </value>
		public float ViewportHeight { 
			get { return 2f * _camera.orthographicSize; }
			set { 
				_camera.orthographicSize = value / 2f; 
				var worldPos = new Vector3(0f, 0f, 0f);
				_holder.transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
				_stageOffset = new Vector2(worldPos.x, worldPos.y);
			}
		}
		
		// Use this for initialization
		public override void Awake () {
			if (_instance != null)
				throw new Exception("You can only create one Stage");
			_instance = this;
			
			Name = "Stage";
			
			_holder = new GameObject("StageContainer");
			_clock = new GameClock("DefaultClock");
			_holder.AddComponent<Library>();
			_holder.AddComponent<MouseInput>();
			
			_camera = GetComponent<Camera>();
			_camera.orthographic = true;
			_camera.transform.position = new Vector3(0, 0, CinchOptions.CameraPos);
			_camera.tag = "MainCamera";
			_holder.transform.parent = _camera.transform;
			RenderSettings.ambientLight = new Color(1f, 1f, 1f);
			
			var tweenRunner = gameObject.AddComponent<TweenRunner>();
			tweenRunner.__Init (_clock);
		}
		
		public void Start()
		{
			__CameraPosChanged();
			
			if (ViewportHeight > ViewportWidth)
				ViewportHeight = CinchOptions.DefaultViewportMaxDimension;
			else
				ViewportWidth = CinchOptions.DefaultViewportMaxDimension;
			
			//no, this is not a typo.  We can't get screen dim's until Start() in web player mode, so call Awake() here, when ViewportWidth/Height will return correct values
			base.Awake ();		
		}
		
		public void Update () {
			__CallOnEnterFrames();
		}
		
		public void LateUpdate()
		{
			__CallOnExitFrames();
			
			if (DisplayObjectContainer.__ChildrenUpdated)
			{
				DisplayObjectContainer.__ChildrenUpdated = false;
				var totalDepth = __OrderChildren();
				_holder.transform.position = new Vector3(0, 0, totalDepth + CinchOptions.CameraObjectGap);
				
				MouseInput.Instance.__SortMouseEnabledObjects();
			}
		}
		
		/// <summary>
		/// Enables the Stage, turning the viewport camera back on, along with everything else.
		/// </summary>
		public void Enable()
		{
			_camera.enabled = true;
			MouseInput.Instance.Enable();
			//If you are using Unity3.5, you may need to switch this to _holder.SetActiveRecursively(true);
			_holder.SetActive(true);
			Clock.Paused = false;
		}
		
		/// <summary>
		/// Disables the Stage, hiding all objects, stopping the clock, and disabling the main camera.
		/// </summary>
		public void Disable()
		{
			_camera.enabled = false;
			//If you are using Unity3.5, you may need to switch this to _holder.SetActiveRecursively(false);
			_holder.SetActive(false);
			MouseInput.Instance.Disable();
			Clock.Paused = true;
		}
		
		/// <summary>
		/// Stage cannot be rotated or scaled.  Will always be 0.
		/// </summary>
		/// <value>
		/// Always 0.
		/// </value>
		public override float Rotation{
			get { return 0; }
			set { }
		}
		
		/// <summary>
		/// Essentially a pass-through, since local is global for Stage.
		/// </summary>
		/// <returns>
		/// The input value.
		/// </returns>
		/// <param name='local'>
		/// Local value to be converted to global
		/// </param>
		public override Vector2 LocalToGlobal(Vector2 local)
		{
			return local;
		}
		
		/// <summary>
		/// Essentially a pass-through, since local is global for Stage.
		/// </summary>
		/// <returns>
		/// The input value.
		/// </returns>
		/// <param name='__global'>
		/// Global value to be converted to local
		/// </param>
		public override Vector2 GlobalToLocal (Vector2 __global)
		{
			return __global;
		}
		
		/// <summary>
		/// Stage cannot be moved, so this is always 0
		/// </summary>
		/// <value>
		/// Always 0.
		/// </value>
		public override float X {
			get {return 0;}
			set {}
		}
		
		/// <summary>
		/// Stage cannot be moved, so this is always 0
		/// </summary>
		/// <value>
		/// Always 0.
		/// </value>
		public override float Y {
			get {return 0;}
			set {}
		}
		
		/// <summary>
		/// Stage cannot be moved, so this does nothing
		/// </summary>
		public override void SetPosition(float x, float y){}

		/// <summary>
		/// Gets the x position of the mouse relative to the Stage.
		/// </summary>
		/// <value>
		/// The mouse x.
		/// </value>
		public override float MouseX { get{ 
				var mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
				return GlobalToLocal(new Vector2(mousePos.x, mousePos.y)).x - _holder.transform.position.x;
		} }
		/// <summary>
		/// Gets the y position of the mouse relative to the Stage.
		/// </summary>
		/// <value>
		/// The mouse y.
		/// </value>
		public override float MouseY { get{ 
				var mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
				return GlobalToLocal(new Vector2(mousePos.x, mousePos.y)).y - _holder.transform.position.y;
		} }		
	}
}