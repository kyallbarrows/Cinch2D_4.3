using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Cinch2D
{
	/*! Base class for anything that can be added to the Stage */
	public class DisplayObjectContainer : EventDispatcher {
		
		private const float MAX_DIMENSION = 5000f;
		protected List<DisplayObjectContainer> _children;
		protected bool _mouseChildren = true;
		
		/// <summary>
		/// Get/set whether children will respound to mouse input.
		/// </summary>
		/// <value>
		/// <c>true</c> if mouse children; otherwise, <c>false</c>.
		/// </value>
		public bool MouseChildren { 
			get { return _mouseChildren; }
			set
			{
				_mouseChildren = value;
				for(var i=0; i<_children.Count; i++)
					_children[i].UpdateChildrenOfParentClickable(value && _childrenOfParentAreClickable);			
			}
		}
		
		protected bool _childrenOfParentAreClickable = true;
		protected void UpdateChildrenOfParentClickable(bool childrenOfParentAreClickable)
		{
			_childrenOfParentAreClickable = childrenOfParentAreClickable;
			for(var i=0; i<_children.Count; i++)
				_children[i].UpdateChildrenOfParentClickable(_childrenOfParentAreClickable && MouseChildren);
		}
		
		public bool __Clickable { get { 
				return Stage != null && Visible && _childrenOfParentAreClickable && MouseEnabled; 
			}}
		protected bool _mouseEnabled = false;
		
		/// <summary>
		/// Get/set whether this object responds to mouse.  NOTE: does not affect child objects, use MouseChildren instead.
		/// </summary>
		/// <value>
		/// <c>true</c> if mouse enabled; otherwise, <c>false</c>.
		/// </value>
		public bool MouseEnabled { 
			get{ return _mouseEnabled; } 
			set{
				if (value != _mouseEnabled && Stage != null)
				{
					if (value)
						MouseInput.Instance.__AddMouseEnabledObject(this);
					else
						MouseInput.Instance.__RemoveMouseEnabledObject(this);
				}
				
				_mouseEnabled = value;
			}
		}
			
		protected bool _alphaUpdated;	
		public void __NotifyChildrenAlphaUpdated(){ 
			foreach(var c in _children)
				c.__NotifyChildrenAlphaUpdated();
			
			_alphaUpdated = true; 
		}
		
		public virtual float __GetCombinedAlpha()
		{
			if (Parent == null)
				return _alpha;
			
			return _alpha * Parent.__GetCombinedAlpha();
		}
		
		private float _alpha = 1f;
		/// <summary>
		/// Gets or sets the alpha (transparency).
		/// </summary>
		/// <value>
		/// The alpha, between 0 (invisible) and 1 (fully visible).  Note: much faster to use <c>obj.Visible = false;</c> than <c>obj.Alpha = 0;</c>
		/// </value>
		public virtual float Alpha {
			get { return _alpha; }
			set { 
				_alpha = value; 
				_alphaUpdated = true; 
				__NotifyChildrenAlphaUpdated();
			} 
		}
		
		/// <summary>
		/// Called from Awake.  Exists because it's hard to remember to call base.Awake() every time you override it, and bad things happen when you do.
		/// </summary>
		public virtual void OnAwake(){}
		
		public virtual void __InternalOnEnterFrame(){}
		/// <summary>
		/// Called on every Update call, unless Clock is paused.
		/// </summary>
		/// <param name='time'>
		/// Current Clock time, in seconds
		/// </param>
		public virtual void OnEnterFrame(float time){}
		
		public virtual void __InternalOnExitFrame(){
			if (_alphaUpdated)
			{
				_alphaUpdated = false;
				UpdateColorTint();
			}
			
			if (_clock != null)
				_clock.__Tick ();
		}
		/// <summary>
		/// Called on every LateUpdate, unless Clock is pause.
		/// </summary>
		/// <param name='time'>
		/// Current Clock time, in seconds.
		/// </param>
		public virtual void OnExitFrame(float time){}
		
		public void __CallOnEnterFrames(){
			if (_clock != null && _clock.Paused)
				return;
			
			__InternalOnEnterFrame();
			OnEnterFrame(Clock.CurrTime);
			
			foreach(var c in _children)
			{
				c.__CallOnEnterFrames();
			}
		}
		
		public void __CallOnExitFrames(){
			if (_clock != null && _clock.Paused)
				return;
			
			__InternalOnExitFrame();
			OnExitFrame(Clock.CurrTime);
			
			foreach(var c in _children)
			{
				c.__CallOnExitFrames();
			}
		}
		
		
		
		protected virtual void UpdateColorTint()
		{
			var combinedAlpha = __GetCombinedAlpha();
			_meshRenderer.material.color = new Color(1f, 1f, 1f, combinedAlpha);
		}
		
		protected MeshFilter _meshFilter;
		protected MeshRenderer _meshRenderer;
		protected Material _material;
		
		protected bool _parentVisible = true;
		protected bool _visible = true;
		
		public bool __ParentVisible { 
			get {return _parentVisible;} 
			set{
				if (_parentVisible == value)
					return;
				
				_parentVisible = value; UpdateVisibilityChain();
			} 
		}
		/// <summary>
		/// Gets or sets a visibility.  Sprite continues to run while invisible.
		/// </summary>
		/// <value>
		/// <c>true</c> if visible; otherwise, <c>false</c>.
		/// </value>
		public bool Visible { 
			get {return _visible;} 
			set{
				if (_visible == value)
					return;
				
				_visible = value; UpdateVisibilityChain();
			}
		}
		
		private void UpdateVisibility()
		{
			var isVisible = __ParentVisible && Visible && (Stage != null || __IsStage());
			
			if (_meshRenderer != null)
			{
				//if we're turning it back on, need to update mesh because it wasn't getting updated while invisible
				if (!_meshRenderer.enabled && isVisible)
					__UpdateMesh();
				
				_meshRenderer.enabled = isVisible;
			}
		}
		
		private void UpdateVisibilityChain()
		{
			UpdateVisibility();
			
			var isVisible = __ParentVisible && Visible && (Stage != null || __IsStage());
			foreach(var c in _children)
			{
				c.__ParentVisible = isVisible;
			}
		}
		
		
		protected GameClock _clock;
		/// <summary>
		/// Gets or sets the GameClock.  Children with no attached clock use their parent's clock, so all objects initally use the Stage's.  
		/// Setting a clock will affect all children, and would be useful for cases where you want to pause gameplay but keep menus and dialogs running.
		/// </summary>
		/// <value>
		/// The clock.
		/// </value>
		public GameClock Clock { 
			get { 
				if (_clock != null) return _clock;
				if (Parent != null) return Parent.Clock;
				return null;
			}
			set {
				_clock = value;
			}
		}	
			
		/// <summary>
		/// Mouse position x relative to this object.  
		/// </summary>
		/// <value>
		/// The mouse x.
		/// </value>
		/// <exception cref='Exception'>
		/// Thrown if object is not on Stage or a descendant of Stage
		/// </exception>
		public virtual float MouseX { get{ 
				if (Stage == null)
					throw new Exception("Must be added to stage to use MouseX");
			
				return GlobalToLocal(new Vector2(Stage.MouseX, Stage.MouseY)).x;
		} }
		
		/// <summary>
		/// Mouse position y relative to this object.  
		/// </summary>
		/// <value>
		/// The mouse y.
		/// </value>
		/// <exception cref='Exception'>
		/// Thrown if object is not on Stage or a descendant of Stage
		/// </exception>
		public virtual float MouseY { get{ 
				if (Stage == null)
					throw new Exception("Must be added to stage to use MouseY");
				
				return GlobalToLocal(new Vector2(Stage.MouseX, Stage.MouseY)).y;
		} }
		
		/// <summary>
		/// Converts a point from local (moved, scaled, and rotated) coordinates to global Stage coordinates.
		/// </summary>
		/// <returns>
		/// The global value.
		/// </returns>
		/// <param name='local'>
		/// The local value to convert.
		/// </param>
		public virtual Vector2 LocalToGlobal(Vector2 local){
			var fakeZ = 0f;
			var newPos = __GetGlobalTransform().MultiplyPoint3x4(new Vector3(local.x, local.y, fakeZ));
			return new Vector2(newPos.x,newPos.y);
		}
		
		/// <summary>
		/// Converts a point from global (Stage) coordinates to local (moved, scaled, and rotated) coordinates.
		/// </summary>
		/// <returns>
		/// The local value.
		/// </returns>
		/// <param name='local'>
		/// The global value to convert.
		/// </param>
		public virtual Vector2 GlobalToLocal(Vector2 __global){
			var fakeZ = 0f;
			var newPos = __GetGlobalTransform().inverse.MultiplyPoint3x4(new Vector3(__global.x, __global.y, fakeZ));
			return new Vector2(newPos.x,newPos.y);
		}
		
		/// <summary>
		/// Readonly, gets the parent.  Parent is set when child is added using .AddChild() or other methods.
		/// </summary>
		/// <value>
		/// The parent.  Returns null if object has not been added to Stage or another DisplayObjectContainer.
		/// </value>
		public DisplayObjectContainer Parent { get; private set; }
		
		protected void __SetParent(DisplayObjectContainer parent) { 
			bool changed = Parent != parent;
			Parent = parent;
			if (changed)
			{
				if (parent == null)
					__Removed();
				else
					__Added();
			}
		}

		protected virtual void __Removed()
		{
			UpdateVisibilityChain();

			if (MouseEnabled)
				MouseInput.Instance.__RemoveMouseEnabledObject(this);
				
			RemovedFromStage();
			foreach(var c in _children)
				c.__Removed();
		}
		
		protected virtual void __Added()
		{
			UpdateVisibilityChain();
			
			if (Stage != null)
			{
				if (MouseEnabled)
					MouseInput.Instance.__AddMouseEnabledObject(this);
				
				gameObject.transform.localRotation = Quaternion.identity;
				
				AddedToStage();
				foreach(var c in _children)
					c.__Added ();
			}
		}
		
		/// <summary>
		/// Called when 1) object is added to Stage 2)object is added to an object that is in Stage's descendants 3) the object that this object is a child of is added to Stage.
		/// </summary>
		public virtual void AddedToStage()
		{
		}

		/// <summary>
		/// Called when 1) object is removed to Stage 2) object is removed from an object that is in Stage's descendants 3) object that contains this object is removed from Stage.
		/// </summary>
		public virtual void RemovedFromStage()
		{
		}
		
		/// <summary>
		/// Readonly, gets the stage.
		/// </summary>
		/// <value>
		/// The stage, if object is in Stage's display hierarchy.  Otherwise returns null.
		/// </value>
		public Stage Stage { get{
				if (Parent == null) return null;
				if (Parent is Stage) return (Stage)Parent;
				return Parent.Stage;
			} }
		public bool __IsStage() { return this is Stage; }
		
		protected Mesh _originalMesh;
		protected Mesh _transformedMesh;
		protected Mesh _combinedMesh;
		protected bool _subMeshesValid = false;
		public void __InvalidateSubMeshes() { _subMeshesValid = false; }
		protected void NotifyParentMeshInvalid(){
			if (Parent != null)
				Parent.__InvalidateSubMeshes();		
		}
		
		protected Mesh _originalHitAreaMesh;
		protected Mesh _transformedHitAreaMesh;
		
		public Matrix4x4 __LocalTransformMatrix{get; protected set;}
		public Matrix4x4 __GlobalTransformMatrix{get; protected set;}
		
		protected float _rotation;
		/// <summary>
		/// Gets or sets the rotation in radians.  Positive values are counter-clockwise, negative values clockwise.
		/// If you prefer to use degrees instead of radians, use RotationDegrees
		/// </summary>
		/// <value>
		/// The rotation, in radians.
		/// </value>
		public virtual float Rotation{
			get { return _rotation; }
			set { 
				_rotation = value; 
				__UpdateMesh();
				NotifyParentMeshInvalid();
			}
		}	
		/// <summary>
		/// Gets or sets the rotation in degrees.  Positive values are counter-clockwise, negative values clockwise.
		/// </summary>
		/// <value>
		/// The rotation, in degrees.
		/// </value>
		public virtual float RotationDegrees{
			get { return Rotation * Mathf.Rad2Deg; }
			set { Rotation = value * Mathf.Deg2Rad; }
		}
		
		
		
		public Mesh __GetCombinedMeshes()
		{
			if (_subMeshesValid)
				return _combinedMesh;
			
			if (_children.Count == 0)
				return _originalMesh;
			
			var combine = new CombineInstance[1 + _children.Count];
	        combine[0].mesh = _originalMesh;
	   	    combine[0].transform = Matrix4x4.identity;
			
			for (var i=0; i < _children.Count; i++)
			{
		        combine[i+1].mesh = _children[i].__GetCombinedMeshes();
	    	    combine[i+1].transform = _children[i].__LocalTransformMatrix;
			}
		
	        _combinedMesh.CombineMeshes(combine);
			return _combinedMesh;
		}

		
		public Matrix4x4 __GetGlobalTransform()
		{
			if (this is Stage || Parent == null)
			{
				return Matrix4x4.identity;
			}
			
			var rv = Parent.__GetGlobalTransform() * __LocalTransformMatrix;
			return rv;
		}
		
		public virtual void __UpdateMesh()
		{
			if (!Visible || !__ParentVisible)
				return;
			
			__LocalTransformMatrix = Matrix4x4.TRS(new Vector3(_x, _y, 0), Quaternion.AngleAxis(_rotation * Mathf.Rad2Deg, Vector3.forward), new Vector3(_scaleX, _scaleY, 1f));
			__GlobalTransformMatrix = __GetGlobalTransform();
			
			if (_meshFilter != null && _originalMesh != null){
				var meshCombine = new CombineInstance[1];
		        meshCombine[0].mesh = _originalMesh;
		        meshCombine[0].transform = __GlobalTransformMatrix;
		        _transformedMesh.CombineMeshes(meshCombine);
				_meshFilter.sharedMesh = _transformedMesh;
				
				TransformColliderMesh();
			}
			
			foreach(var c in _children)
				c.__UpdateMesh();
		}
		
		private void TransformColliderMesh()
		{
			if (_originalHitAreaMesh != null)
			{
				var hitAreaMeshCombine = new CombineInstance[1];
		        hitAreaMeshCombine[0].mesh = _originalHitAreaMesh;
		        hitAreaMeshCombine[0].transform = __GlobalTransformMatrix;
		        _transformedHitAreaMesh.CombineMeshes(hitAreaMeshCombine);
			}
		}
		
		/// <summary>
		/// Sets the mouse area (aka hit shape) of the object.  By default, the object uses its display mesh as the hit shape, and this will be null.
		/// </summary>
		/// <param name='mesh'>
		/// The new mesh to use as the mouse area, or null to clear a previously set mesh and return to using the display mesh as the mouse area.
		/// </param>
		public void SetMouseArea(Mesh mesh)
		{
			_originalHitAreaMesh = mesh;
			_transformedHitAreaMesh = new Mesh();
			TransformColliderMesh();
		}
		
		/// <summary>
		/// Gets or sets the width of the object AND all children, in meters.  Affects ScaleX when set.  Does nothing if object is empty.
		/// </summary>
		/// <value>
		/// The width in meters.  Cannot be set to 0, or to obnoxiously large values (currently 5000 meters).
		/// </value>
		/// <exception cref='Exception'>
		/// Thrown when set to a bad value, see Value above.
		/// </exception>
		public virtual float Width { 
			get { 
				return _scaleX * __GetCombinedMeshes().bounds.size.x; 
			}
			set { 
				if (Width == 0)
				{
					return;
				}
				if (value == 0)
				{
					throw new Exception("Width cannot be set to 0");
				}
				if (value > MAX_DIMENSION)
				{
					throw new Exception ("Width cannot be set greart than " + MAX_DIMENSION);
				}
				if (_originalMesh.vertexCount == 0 && _children.Count == 0)
				{
					return;
				}
				
				var naturalMeshWidth = __GetCombinedMeshes().bounds.size.x;
				if (naturalMeshWidth == 0)
					ScaleX = 1;
				else
					ScaleX = value / naturalMeshWidth; 
			}	
		}
		
		/// <summary>
		/// Gets or sets the height of the object AND all children, in meters.  Affects ScaleY when set.  Does nothing if object is empty.
		/// </summary>
		/// <value>
		/// The height in meters.  Cannot be set to 0, or to obnoxiously large values (currently 5000 meters).
		/// </value>
		/// <exception cref='Exception'>
		/// Thrown when set to a bad value, see Value above.
		/// </exception>
		public virtual float Height { 
			get {
				return _scaleY * __GetCombinedMeshes().bounds.size.y; 
			}
			set {
				if (Height == 0)
				{
					return;
				}
				if (value == 0)
				{
					throw new Exception("Height cannot be set to 0");
				}
				if (value > MAX_DIMENSION)
				{
					throw new Exception ("Height cannot be set greart than " + MAX_DIMENSION);
				}
				if (_originalMesh.vertexCount == 0 && _children.Count == 0)
				{
					return;
				}
				
				var naturalMeshHeight = __GetCombinedMeshes().bounds.size.y;
				if (naturalMeshHeight == 0)
					ScaleY = 1;
				else
					ScaleY = value / naturalMeshHeight; 
			}	
		}

		
		protected float _scaleX = 1f;
		/// <summary>
		/// Gets or sets the x scale.  Affects Width.
		/// </summary>
		/// <value>
		/// The new x scale (1 is 100% normal size, .5 is half, etc.)
		/// </value>
		/// <exception cref='Exception'>
		/// Thrown if new scale is 0, or if new scale would cause object to be larger than max size (5000 meters).
		/// </exception>
		public float ScaleX { 
			get { return _scaleX; } 
			set { 
				if (value == 0)
					throw new Exception("ScaleX must be non-zero");
				
				//make sure not to high value
				if (value * __GetCombinedMeshes().bounds.size.x > MAX_DIMENSION)
					throw new Exception("ScaleX would make size greater than max (5000)");
				
				_scaleX = value;
				ScaleXHook();
				__UpdateMesh();
				NotifyParentMeshInvalid();
			}
		}	
		
		protected float _scaleY = 1f;
		/// <summary>
		/// Gets or sets the y scale.  Affects Height.
		/// </summary>
		/// <value>
		/// The new y scale (1 is 100% normal size, .5 is half, etc.)
		/// </value>
		/// <exception cref='Exception'>
		/// Thrown if new scale is 0, or if new scale would cause object to be larger than max size (5000 meters).
		/// </exception>
		public float ScaleY { 
			get { return _scaleY; } 
			set { 
				if (value == 0)
					throw new Exception("ScaleY must be non-zero");
				
				//make sure not to high value
				if (value * __GetCombinedMeshes().bounds.size.y > MAX_DIMENSION)
					throw new Exception("ScaleY would make size greater than max (5000)");
				
				_scaleY = value;
				ScaleYHook();
				__UpdateMesh(); 
				NotifyParentMeshInvalid();
			}
		}
		
		protected virtual void ScaleXHook()
		{
			//override in derived classes, let's us do fancy things after setting ScaleX
		}
		
		protected virtual void ScaleYHook()
		{
			//override in derived classes, let's us do fancy things after setting ScaleX
		}
			
		/// <summary>
		/// Sets the position.  Setting X and Y individually causes 2 updates to the display chain, this is faster since it only causes 1.
		/// </summary>
		/// <param name='x'>
		/// X.
		/// </param>
		/// <param name='y'>
		/// Y.
		/// </param>
		public virtual void SetPosition(float x, float y)
		{
			_x = x;
			_y = y;
			__UpdateMesh();
		}
		
		/// <summary>
		/// Sets the position and rotation.  Setting X, Y and Rotation individually causes 3 updates to the display chain. This is faster since it only causes 1.
		/// </summary>
		/// <param name='x'>
		/// New X.
		/// </param>
		/// <param name='y'>
		/// New Y.
		/// </param>
		/// <param name='rotation'>
		/// New Rotation, in radians.
		/// </param>
		public virtual void SetPosition(float x, float y, float rotation)
		{
			_x = x;
			_y = y;
			_rotation = rotation;
			__UpdateMesh();
		}
		
		/// <summary>
		/// Sets the position and rotation.  Setting X, Y and Rotation individually causes 3 updates to the display chain. This is faster since it only causes 1.
		/// </summary>
		/// <param name='x'>
		/// New X.
		/// </param>
		/// <param name='y'>
		/// New Y.
		/// </param>
		/// <param name='rotation'>
		/// New Rotation, in degrees.
		/// </param>
		public virtual void SetPositionDegrees(float x, float y, float rotationDegrees)
		{
			SetPosition(x, y, rotationDegrees * Mathf.Deg2Rad);
		}
		
		protected float _x;
		/// <summary>
		/// Gets or sets the x position of the object, in meters, relative to its parent.
		/// </summary>
		/// <value>
		/// The x.
		/// </value>
		public virtual float X { 
			get{return _x;} 
			set{
				_x = value; 
				__UpdateMesh(); 
				NotifyParentMeshInvalid();
			}}
		
		protected float _y;
		/// <summary>
		/// Gets or sets the y position of the object, in meters, relative to its parent.
		/// </summary>
		/// <value>
		/// The y.
		/// </value>
		public virtual float Y { 
			get{return _y;} 
			set{
				_y = value; 
				__UpdateMesh(); 
				NotifyParentMeshInvalid();
			}}
		
		protected float _z;
		/// <summary>
		/// Gets the z, aka depth.  This is set automatically by adding and swapping children.
		/// </summary>
		/// <value>
		/// The z (depth).
		/// </value>
		public virtual float Z { get { 
				return gameObject.transform.position.z; }}
		
		public DisplayObjectContainer()
		{
			_children = new List<DisplayObjectContainer>();
			_mouseEnabled = CinchOptions.MouseEnabledByDefault;
			MouseChildren = true;		
		}
		
		/// <summary>
		/// Don't override this, instead, override and use OnAwake().
		/// </summary>
		public virtual void Awake()
		{
			gameObject.layer = CinchOptions.CinchLayer;
			_originalMesh = new Mesh();
			_transformedMesh = new Mesh();
			_combinedMesh = new Mesh();
			
			OnAwake();
		}
		
		/// <summary>
		/// Destroys this instance and cleans up any associated mess.  Use this instead of Destroy(sprite).
		/// </summary>
		public void Destroy()
		{
			if (Parent != null)
				Parent.RemoveChild(this);
			
			while (NumChildren > 0)
				RemoveChildAt (0);
			
			if (MouseEnabled)
				MouseInput.Instance.__RemoveMouseEnabledObject(this);	
			
			Destroy (gameObject);
		}
		
		/// <summary>
		/// Called when user calls Destroy([any displayObjectContainer]).  WARNING: Be sure to call Base.OnDestroy if you override this, otherwise bad behavior will happen
		/// </summary>
		public virtual void OnDestroy()
		{
	//		gameObject.transform.parent = null;
			
			
			
	//		Destroy(gameObject);
		}	
		
		protected void AddChildInternal(DisplayObjectContainer child)
		{
			if (child == null)
				throw new ArgumentNullException("child", "child cannot be null");
			
			if (child.Parent != null)
				child.Parent.RemoveChild(child);
			
			child.__SetParent(this);
			SetAsTransformChild(child.gameObject);
			__ChildrenUpdated = true;
			child.__UpdateMesh();		
		}
		
		protected virtual void SetAsTransformChild(GameObject go)
		{
			go.transform.parent = this.gameObject.transform;
		}
		
		/// <summary>
		/// Removes a child form its current parent, if any, and adds the child to this object's display hierarchy.  Can be used to floating an existing object to the top of this object's children.
		/// </summary>
		/// <returns>
		/// The child to add.
		/// </returns>
		/// <param name='child'>
		/// Child.
		/// </param>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
		/// </exception>
		public DisplayObjectContainer AddChild(DisplayObjectContainer child) { 
			if (child == null)
				throw new ArgumentNullException("child", "child cannot be null");
			
			if (child.Parent == this)
			{
				SetChildIndex(child, _children.Count-1);
				return child;
			}
			
			AddChildInternal(child);
			_children.Add(child);
			return child;
		}
		
		/// <summary>
		/// Removes the child from its current parent and adds it at the given index.
		/// </summary>
		/// <returns>
		/// The <see cref="DisplayObjectContainer"/> that was added
		/// </returns>
		/// <param name='child'>
		/// Child to add
		/// </param>
		/// <param name='index'>
		/// Index to add child at
		/// </param>
		/// <exception cref='ArgumentNullException'>
		/// Thrown when child is null
		/// </exception>
		/// <exception cref='ArgumentException'>
		/// Thrown when index is less than 0 or too high.
		/// </exception>
		public DisplayObjectContainer AddChildAt(DisplayObjectContainer child, int index ) { 
			if (child == null)
				throw new ArgumentNullException("child", "child cannot be null");
			
			if (index < 0 || index > _children.Count - 1)
				throw new ArgumentException("index out of range");
			
			if (child.Parent == this)
			{
				SetChildIndex(child, index);
				return child;
			}
			
			AddChildInternal(child);
			_children.Insert(index, child);
			
			return child;
		}
		
		/// <summary>
		/// Checks if container contains the specified child.
		/// </summary>
		/// <param name='child'>
		/// True if child is child of parent, otherwise false
		/// </param>
		public bool Contains( DisplayObjectContainer child ) { 
			return _children.Contains(child); 
		}
		
		/// <summary>
		/// Gets child by name
		/// </summary>
		/// <returns>
		/// The child, if it exists, otherwise null
		/// </returns>
		/// <param name='name'>
		/// Name of child to find
		/// </param>
		public DisplayObjectContainer GetChildByName( string name ) { 
			var objs = (from c in _children where c.Name == name select c).ToList();
			
			if (objs.Count == 0)
				return null;
			
			return objs.FirstOrDefault();
		}
		
		/// <summary>
		/// Gets the child at the given index
		/// </summary>
		/// <returns>
		/// The <see cref="DisplayObjectContainer"/>.
		/// </returns>
		/// <param name='index'>
		/// Index.
		/// </param>
		/// <exception cref='ArgumentException'>
		/// Thrown when index is less than 0 or more than NumChildren-1
		/// </exception>
		public DisplayObjectContainer GetChildAt(int index) { 
			if (index < 0 || index >= _children.Count)
				throw new ArgumentException("index out of range");
			
			var obj = _children[index];
			return obj;
		}
		
		/// <summary>
		/// Removes the child.
		/// </summary>
		/// <returns>
		/// The child.
		/// </returns>
		/// <param name='child'>
		/// Child.
		/// </param>
		/// <exception cref='ArgumentException'>
		/// Is thrown when an argument passed to a method is invalid.
		/// </exception>
		public DisplayObjectContainer RemoveChild(DisplayObjectContainer child){ 
			var childIndex = _children.IndexOf(child);
			if (childIndex == -1)
				throw new ArgumentException("Object is not a child of this parent");

			RemoveChildAt(childIndex);
			return child;
		}
		
		/// <summary>
		/// Removes the child at given index.
		/// </summary>
		/// <returns>
		/// The <see cref="DisplayObjectContainer"/> at that index.
		/// </returns>
		/// <param name='index'>
		/// The index to remove at
		/// </param>
		/// <exception cref='ArgumentException'>
		/// Thrown if index is less than 0 or more than NumChildren-1
		/// </exception>
		public DisplayObjectContainer RemoveChildAt(int index){ 
			if (index < 0 || index >= _children.Count)
				throw new ArgumentException("index out of range");
			
			var child = _children[index];
			_children.RemoveAt(index);

			child.__SetParent(null);
			
			//might be destroyed already if app is closing
			if (child.gameObject != null && child.gameObject.transform != null)
				child.gameObject.transform.parent = null;

			return child;
		}
		
		/// <summary>
		/// Removes the all children from the given range of indices.
		/// </summary>
		/// <param name='beginIndex'>
		/// Begin index, inclusive.
		/// </param>
		/// <param name='endIndex'>
		/// End index, exclusive (see example)
		/// </param>
		/// <exception cref='Exception'>
		/// Thrown if beginIndex or endIndex are out of range (too high or less than 0)
		/// </exception>
		/// <exception cref='ArgumentException'>
		/// Is thrown when an argument passed to a method is invalid.
		/// </exception>
		/// <example>
		/// <code>
		/// //removes the first 2 children from an object
		/// someObject.RemoveChildren(0, 2);
		/// </code>
		/// </example>
		public void RemoveChildren(int beginIndex = 0, int endIndex = int.MaxValue){ 
			if (_children.Count == 0)
				throw new Exception("no children to remove");
			if (beginIndex < 0 || beginIndex >= _children.Count)
				throw new ArgumentException("beginIndex out of range");
			if (endIndex < beginIndex)
				throw new ArgumentException("endIndex must be greater than or equal to beginIndex");
			
			endIndex = Mathf.Min(endIndex, _children.Count-1);
			//need it to include endIndex, so add 1
			var count = (endIndex+1) - beginIndex;
			for (var i=0; i<count; i++)
				RemoveChildAt(beginIndex);
		}
		
		/// <summary>
		/// Gets the number children in this object's display hierarchy.
		/// </summary>
		/// <value>
		/// The number children.
		/// </value>
		public int NumChildren { get{ return _children.Count;} }
		public int NumDescendants { get {
				var totalDesc = NumChildren;
				foreach (var c in _children)
					totalDesc += c.NumDescendants;
				
				return totalDesc;
		}}
		
		/// <summary>
		/// Gets the index of the child.
		/// </summary>
		/// <returns>
		/// The child index, or -1 if the child is not actually a child of this object.
		/// </returns>
		/// <param name='child'>
		/// Child.
		/// </param>
		public int GetChildIndex(DisplayObjectContainer child)
		{
			for (var i = 0; i<_children.Count; i++)
			{
				if (_children[i].Id == child.Id)
					return i;
			}
			
			return -1;
		}
		
		/// <summary>
		/// Sets the index of the child.
		/// </summary>
		/// <param name='child'>
		/// The child to move.  Must be a child of this object
		/// </param>
		/// <param name='index'>
		/// The index to set the child to.  Must be between 0 and NumChildren-1.
		/// </param>
		/// <exception cref='ArgumentException'>
		/// Thrown when child is not actually a child of this object, or index is outside acceptable range (see above)
		/// </exception>
		public void SetChildIndex(DisplayObjectContainer child, int index){
			if (!Contains (child))
				throw new ArgumentException("Parent must contain supplied child to set index");

			if (index >= _children.Count)
				throw new ArgumentException("index out of range");
			
			_children.Remove(child);
			_children.Insert(index, child);
			__ChildrenUpdated = true;		
		}
		
		/// <summary>
		/// Swaps depths of the children.
		/// </summary>
		/// <param name='child1'>
		/// Child1
		/// </param>
		/// <param name='child2'>
		/// Child2
		/// </param>
		/// <exception cref='ArgumentException'>
		/// Thrown when either child1 or child2 are not actually children of this object.
		/// </exception>
		public void SwapChildren(DisplayObjectContainer child1, DisplayObjectContainer child2){ 
			var index1 = GetChildIndex(child1);
			var index2 = GetChildIndex(child2);
			
			if (index1 < 0)
				throw new ArgumentException("Argument 'child1' is not child of this parent.");
			if (index2 < 0)
				throw new ArgumentException("Argument 'child2' is not child of this parent.");
			
			_children[index1] = child2;
			_children[index2] = child1;
			__ChildrenUpdated = true;
		}
		
		/// <summary>
		/// Swaps depths of the children at the given indices.
		/// </summary>
		/// <param name='index1'>
		/// First index.
		/// </param>
		/// <param name='index2'>
		/// Some nachos.  LOLJK, the second index.
		/// </param>
		/// <exception cref='ArgumentException'>
		/// Thrown when either index is out of range (less than 0, or greater than NumChildren-1)
		/// </exception>
		public void SwapChildrenAt(int index1, int index2){ 
			if (index1 < 0 || index1 >= _children.Count)
				throw new ArgumentException("Argument 'index1' is out of range.");
			if (index2 < 0 || index2 >= _children.Count)
				throw new ArgumentException("Argument 'index2' is out of range.");
			
			if (index1 == index2)
				return;
			
			var child2 = _children[index2];
			_children[index2] = _children[index1];
			_children[index1] = child2;
			__ChildrenUpdated = true;
		}
		
		//if this is set true, need to re-order everything
		public static bool __ChildrenUpdated;
		
		public float __OrderChildren()
		{
			var subtreeThickness = CinchOptions.LayerSpacing;
			
			for (var i=0; i<_children.Count; i++)
			{
				_children[i].gameObject.transform.localPosition = new Vector3(0, 0, -1 * subtreeThickness);
				subtreeThickness += _children[i].__OrderChildren();
			}
			
			return subtreeThickness;
		}
		
		public bool __Level1HitTest(Vector2 p)
		{
			var bounds = _transformedMesh.bounds;
			return (p.x >= bounds.min.x && p.x <= bounds.max.x && p.y >= bounds.min.y && p.y <= bounds.max.y);
		}
		
		/// <summary>
		/// Checks whether the given point is within the bounds of the mouse area mesh.
		/// </summary>
		/// <returns>
		/// true or false.
		/// </returns>
		/// <param name='p'>
		/// The point to check
		/// </param>
		public bool PointIsInMouseArea(Vector2 p)
		{
			var mesh = _transformedHitAreaMesh ?? _transformedMesh;
			for (var i=0; i<mesh.triangles.Length; i+=3)
			{
				if (PointIsInTriangle(p, mesh.vertices[mesh.triangles[i]], mesh.vertices[mesh.triangles[i+1]], mesh.vertices[mesh.triangles[i+2]]))
					return true;
			}
			
			return false;
		}
		
		private bool PointIsInTriangle(Vector2 p, Vector2 t0, Vector2 t1, Vector2 t2)
		{
			// Compute vectors        
			var v0 = t2 - t0;
			var v1 = t1 - t0;
			var v2 = p - t0;
			
			// Compute dot products
			var dot00 = Vector2.Dot(v0, v0);
			var dot01 = Vector2.Dot(v0, v1);
			var dot02 = Vector2.Dot(v0, v2);
			var dot11 = Vector2.Dot(v1, v1);
			var dot12 = Vector2.Dot(v1, v2);
			
			// Compute barycentric coordinates
			var invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
			var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
			var v = (dot00 * dot12 - dot01 * dot02) * invDenom;
			
			// Check if point is in triangle
			return (u >= 0) && (v >= 0) && (u + v < 1);
		}
		
		/// <summary>
		/// Checks whether this object or any of its children will trigger the given event.  May return an inaccurate <c>true</c> if the event is set to not bubble.
		/// </summary>
		/// <returns>
		/// true or false
		/// </returns>
		/// <param name='type'>
		/// The event class to check.  See example for more details
		/// </param>
		/// <typeparam name='T'>
		/// The specific event type to check.  See example for more details
		/// </typeparam>
		/// <example>
		/// <code>
		/// bool willTriggerMouseDown = someObject.WillTrigger<MouseEvent>(MouseEvent.MOUSE_DOWN);
		/// </code>
		/// </example>
		public override bool WillTrigger<T>(string type)
		{
			if (HasEventListener<T>(type))
				return true;
			
			if (_children.Count > 0)
			{
				foreach(var c in _children)
					if (c.WillTrigger<T>(type))
						return true;
			}
			
			return false;
		}	
	}
}