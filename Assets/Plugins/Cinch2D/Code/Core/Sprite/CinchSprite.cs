using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using FarseerPhysics.Dynamics;

namespace Cinch2D
{
	/*! Basically a draggable DisplayObjectContainer, with some setup methods for initinalizing from a texture.  Also, easier to type, so, bonus. */
	public class CinchSprite : DisplayObjectContainer {
		
		/// <summary>
		/// The Farseer Physics Body.  If set, and the Sprite is added to a physics container, it will control the position and rotation of the Sprite.
		/// </summary>
		public Body PhysicsBody;
		
		protected bool _dragging = false;
		protected Vector2 _dragCenter;
		
		/// <summary>
		/// Starts dragging the object.
		/// </summary>
		/// <param name='lockCenter'>
		/// Centers object to mouse around registration point.
		/// </param>
		/// <exception cref='Exception'>
		/// Throws exception if not added to the stage yet.
		/// </exception>
		public virtual void StartDrag(bool lockCenter = false){
			if (Parent == null || Stage == null)
				throw new Exception("Cannot startDrag until added to stage");
			
			_dragging = true;
			_dragCenter = new Vector2(0, 0);
			if (!lockCenter)
			{
				_dragCenter = new Vector2(Parent.MouseX - X, Parent.MouseY - Y);
			}
		}
		
		/// <summary>
		/// Stops the drag.
		/// </summary>
		public virtual void StopDrag(){
			_dragging = false;
		}
		
		public override void __InternalOnEnterFrame(){
			base.__InternalOnEnterFrame();
			if(_dragging)
			{
				var offset = Parent.GlobalToLocal(new Vector2(Stage.Instance.MouseX, Stage.Instance.MouseY));
				SetPosition(offset.x - _dragCenter.x, offset.y - _dragCenter.y);
			}
			else if (PhysicsBody != null)
			{
				SetPosition(PhysicsBody.Position.x, PhysicsBody.Position.y, PhysicsBody.Rotation);
			}
		}
		
		protected Texture2D _texture;
		protected Rect _textureRect;
		protected float _pixelsPerMeter;
		protected RegistrationPoint _regPoint;	
		
		/// <summary>
		/// Creates a new Sprite with the supplied texture.
		/// </summary>
		/// <returns>
		/// The new Sprite.
		/// </returns>
		/// <param name='texturePath'>
		/// Texture to be displayed in the Sprite.
		/// </param>
		/// <param name='pixelsPerMeter'>
		/// Pixels per meter.  Specifying 100 with a 100x100 texture will produce a 1x1 meter Sprite.  You can pass in CinchOptions.DefaultPixelsPerMeter to keep everything the same size.
		/// </param>
		/// <param name='regPoint'>
		/// The registration point (center to pivot around).  Defaults to center.  You can use any of the constants defined on the RegistrationPoint struct, new() up one.
		/// </param>
		public static CinchSprite NewFromImage(string texturePath, float pixelsPerMeter = 0, RegistrationPoint? regPoint = null)
		{
			GameObject go = new GameObject(texturePath);
			var sprite = go.AddComponent<CinchSprite>();
			
			sprite.InternalInitFromImage(texturePath, pixelsPerMeter, CinchOptions.DefaultShader, regPoint.GetValueOrDefault(RegistrationPoint.Center));
			return sprite;
		}
		
		/// <summary>
		/// Creates a new Sprite from a portion of the supplied sprite sheet texture.  See http://www.codeandweb.com/what-is-a-sprite-sheet for more on sprite sheets.
		/// </summary>
		/// <returns>
		/// The new Sprite.
		/// </returns>
		/// <param name='texturePath'>
		/// Sprite sheet to build Sprite from.
		/// </param>
		/// <param name='left'>
		/// Left side of sprite sheet coordinates, in pixels.
		/// </param>
		/// <param name='top'>
		/// Top side of sprite sheet coordinates, in pixels.
		/// </param>
		/// <param name='width'>
		/// Width of sprite sheet coordinates, in pixels.
		/// </param>
		/// <param name='height'>
		/// Height of sprite sheet coordinates, in pixels.
		/// </param>
		/// <param name='pixelsPerMeter'>
		/// Pixels per meter.  Specifying 100 with a 100x100 sprite rectangle will produce a 1x1 meter Sprite.  You can pass in CinchOptions.DefaultPixelsPerMeter to keep everything the same size.
		/// </param>
		/// <param name='regPoint'>
		/// The registration point (center to pivot around).  Defaults to center.  You can use any of the constants defined on the RegistrationPoint struct, new() up one.
		/// </param>
		public static CinchSprite NewFromSpriteSheet(string texturePath, float left, float top, float width, float height, float pixelsPerMeter = 0, RegistrationPoint? regPoint = null)
		{
			GameObject go = new GameObject(texturePath);
			var sprite = go.AddComponent<CinchSprite>();
			
			sprite.InternalInitFromImage(texturePath, pixelsPerMeter, CinchOptions.DefaultShader, regPoint.GetValueOrDefault(RegistrationPoint.Center), new Rect(left, top, width, height));
			return sprite;
		}
		
		/// <summary>
		/// Creates a new empty Sprite.
		/// </summary>
		/// <returns>
		/// The new sprite.
		/// </returns>
		public static CinchSprite NewEmpty(string name)
		{
			GameObject go = new GameObject(name);
			var sprite = go.AddComponent<CinchSprite>();
			return sprite;
		}

		protected void InitFromImage(string texturePath, float pixelsPerMeter = 0, RegistrationPoint? regPoint = null, Rect? rect = null)
		{
			InternalInitFromImage(texturePath, pixelsPerMeter, CinchOptions.DefaultShader, regPoint.GetValueOrDefault(RegistrationPoint.Center), rect);
		}

		private void InternalInitFromImage(string texturePath, float pixelsPerMeter, string shaderType, RegistrationPoint regPoint, Rect? rect = null)
		{
			if (pixelsPerMeter == 0)
				pixelsPerMeter = CinchOptions.DefaultPixelsPerMeter;
			
			this._pixelsPerMeter = pixelsPerMeter;
			_regPoint = regPoint;
					
			_texture = TextureCache.GetCachedTexture(texturePath);
			_textureRect = rect.GetValueOrDefault(new Rect(0f, 0f, (float)_texture.width, (float)_texture.height));

			//flip the y-axis
			if (CinchOptions.UseTopLeftSpriteSheetCoordinates)
			{
				var h = _textureRect.height;
				_textureRect.yMin = _texture.height - (_textureRect.yMin + _textureRect.height);
				_textureRect.height = h;
			}
			
			var scaledRect = new Rect(_textureRect.x/_texture.width, _textureRect.y/_texture.height, _textureRect.width/_texture.width, _textureRect.height/_texture.height);
			
			_originalMesh = new Mesh();
			_originalMesh.name = "Scripted_Plane_New_Mesh";
	        var fullWidth = _textureRect.width / pixelsPerMeter;
	        var fullHeight = _textureRect.height / pixelsPerMeter;
			var left = fullWidth * (0f-regPoint.X);
			var right = fullWidth * (1f-regPoint.X);
			var top = fullHeight * (0f-regPoint.Y);
			var bottom = fullHeight * (1f-regPoint.Y);
			var front = -.1f;
			var flt = new Vector3(left,  top, front); 
			var frt = new Vector3(right, top, front);
			var frb = new Vector3(right, bottom, front); 
			var flb = new Vector3(left,  bottom, front);
					
			_originalMesh.vertices = new Vector3[] {
										flt, frt, frb, flb
			};
			
			_originalMesh.uv = new Vector2[] {	
				new Vector2 (scaledRect.xMin, scaledRect.yMin), 
				new Vector2 (scaledRect.xMax, scaledRect.yMin), 
				new Vector2 (scaledRect.xMax, scaledRect.yMax), 
				new Vector2 (scaledRect.xMin, scaledRect.yMax)
			};			
			
			_originalMesh.triangles = new int[] {
				3 , 2 , 1 , 0 , 3 , 1
			};
			_originalMesh.RecalculateNormals();
			
			_meshFilter = gameObject.AddComponent<MeshFilter>();
			_meshFilter.sharedMesh = _transformedMesh;
			
			var shader = ShaderCache.GetShader(shaderType);
			_meshRenderer = gameObject.AddComponent<MeshRenderer>();
			_meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			_meshRenderer.receiveShadows = false;
			_material = _meshRenderer.material;
			_material.shader = shader;
	        _material.mainTexture = _texture;
			_material.color = new Color( 1f, 1f, 1f, 1f );
			_material.mainTexture.wrapMode = TextureWrapMode.Clamp;
			
			if (Parent == null)
				_meshRenderer.enabled = false; //default it to false until it gets added
			
			__UpdateMesh();
		}
		
		protected Rect? _scale9Grid;
		private Vector3[] _nineSliceVertices;
		private float _nineSliceBottomInset;		//distance from top to top of nine-slice rectangle
		private float _nineSliceTopInset;	//distance from bottom to bottom of nine-slice rectangle
		private float _nineSliceLeftInset;		//distance from left to left side of nine-slice rectangle
		private float _nineSliceRightInset;		//distance from right to right side of nine-slice rectangle
		private float _nineSliceHorizontalLimit;//the farthest we can slide control points left or right
		private float _nineSliceVerticalLimit;	//the farthest we can slide control points up or down
		private static int[] _nineSliceLeftVertexIndices = {1, 5, 9, 13};
		private static int[] _nineSliceRightVertexIndices = {2, 6, 10, 14};
		private static int[] _nineSliceTopVertexIndices = {8, 9, 10, 11};
		private static int[] _nineSliceBottomVertexIndices = {4, 5, 6, 7};
		private float _nativeWidth;
		private float _nativeHeight;
		
		/// <summary>
		/// Sets the scale9 grid.  Currently, this operation cannot be undone (you can't just set it to null)
		/// </summary>
		/// <value>
		/// The scale9 grid.  
		/// NOTE: in keeping with Unity's positive-y-is-up coordinate system, yMin is the bottom value, yMax is the top
		/// WARNING: don't go all crazy with the Scale9Grids.  They add a lot of overhead, and will slow down your app if used excessively.  
		/// </value>
		public virtual void SetScale9Grid(Rect rect)
		{
			_scale9Grid = rect;
			if (_scale9Grid.HasValue)
			{
				_nativeWidth = _texture.width/_pixelsPerMeter;
				_nativeHeight = _texture.height/_pixelsPerMeter;
				
				//rebuild the mesh
				_nineSliceTopInset = _scale9Grid.GetValueOrDefault().y;
				_nineSliceLeftInset = _scale9Grid.GetValueOrDefault().x;
				_nineSliceBottomInset = 1 - _scale9Grid.GetValueOrDefault().yMax;
				_nineSliceRightInset = 1 - _scale9Grid.GetValueOrDefault().xMax;
				_nineSliceHorizontalLimit = Math.Abs(_nineSliceLeftInset) / ((Math.Abs(_nineSliceLeftInset) + Math.Abs(_nineSliceRightInset)));
				_nineSliceVerticalLimit = Math.Abs(_nineSliceTopInset) / ((Math.Abs(_nineSliceTopInset) + Math.Abs(_nineSliceBottomInset)));
				
				RebuildMeshAsNineSlice();
				//update everybody
			}
		}
		
		//public virtual Rect Scale9GridInPixels{...}
		
		private void RebuildMeshAsNineSlice()
		{
			if (!_scale9Grid.HasValue)
				return;
			
			_originalMesh = new Mesh();
			var rect = _scale9Grid.GetValueOrDefault();
			var w = _texture.width / _pixelsPerMeter;
			var h = _texture.height / _pixelsPerMeter;
			var horizStops = new float[] {0, rect.xMin, rect.xMax, 1};
			var vertStops = new float[] {0, rect.yMin, rect.yMax, 1};
			Debug.Log ("YMin/Max: " + rect.yMin + "   " + rect.yMax);
			var rowLength = horizStops.Length;
			var columnHeight = vertStops.Length;
			_nineSliceVertices = new Vector3[rowLength * columnHeight];
			var uv = new Vector2[rowLength * columnHeight];
			var triangles = new int[6 * (rowLength-1) * (columnHeight-1)];
			var currTriangle = 0;
			for (var x = 0; x < rowLength; x++){
				for (var y = 0; y < columnHeight; y++){
					var vertIndex = x + y*rowLength;
					_nineSliceVertices[vertIndex] = new Vector3((horizStops[x] - _regPoint.X) * w, (vertStops[y] - _regPoint.Y) * h, 0f);
					uv[vertIndex] = new Vector2(horizStops[x], vertStops[y]);
					
					Debug.Log ("Adding vertex: " + _nineSliceVertices[vertIndex] +  "  vertstop: " + vertStops[y]);
					if (x < rowLength-1 && y < rowLength - 1)
					{
						triangles[currTriangle] = vertIndex + rowLength;		//one below
						triangles[currTriangle+1] = vertIndex + rowLength + 1;	//one below, right
						triangles[currTriangle+2] = vertIndex + 1;				//one right
						triangles[currTriangle+3] = vertIndex;					//current
						triangles[currTriangle+4] = vertIndex + rowLength;		//one below
						triangles[currTriangle+5] = vertIndex + 1;				//one right
						currTriangle += 6;
					}			
				}
			}
			
			_originalMesh.vertices = _nineSliceVertices;
			_originalMesh.uv = uv;
			_originalMesh.triangles = triangles;
			_originalMesh.RecalculateNormals();
			
			_texture.mipMapBias = -100f;
			
			__UpdateMesh();
		}
		
		protected override void ScaleXHook()
		{
			if (!_scale9Grid.HasValue)
				return;
			
			var newLeftX = _nativeWidth * (Math.Min (_nineSliceLeftInset / _scaleX, _nineSliceHorizontalLimit) - _regPoint.X);
			var newRightX = _nativeWidth * (Math.Max (1 - _nineSliceRightInset / _scaleX, _nineSliceHorizontalLimit) - _regPoint.X);
			for(var i=0; i<4; i++)
			{
				_nineSliceVertices[_nineSliceLeftVertexIndices[i]] = new Vector3(newLeftX, _nineSliceVertices[_nineSliceLeftVertexIndices[i]].y, 0);
				_nineSliceVertices[_nineSliceRightVertexIndices[i]] = new Vector3(newRightX, _nineSliceVertices[_nineSliceRightVertexIndices[i]].y, 0);
			}
			_originalMesh.vertices = _nineSliceVertices;
			_originalMesh.RecalculateNormals();
		}
		
		protected override void ScaleYHook()
		{
			if (!_scale9Grid.HasValue)
				return;
			
			var newTopY = _nativeHeight * (Math.Min (_nineSliceTopInset / _scaleY, _nineSliceVerticalLimit) - _regPoint.Y);
			var newBottomY = _nativeHeight * (Math.Max (1 - _nineSliceBottomInset / _scaleY, _nineSliceVerticalLimit) - _regPoint.Y);
			for(var i=0; i<4; i++)
			{
				_nineSliceVertices[_nineSliceBottomVertexIndices[i]] = new Vector3(_nineSliceVertices[_nineSliceBottomVertexIndices[i]].x, newTopY, 0);
				_nineSliceVertices[_nineSliceTopVertexIndices[i]] = new Vector3(_nineSliceVertices[_nineSliceTopVertexIndices[i]].x, newBottomY, 0);
			}
			_originalMesh.vertices = _nineSliceVertices;
			_originalMesh.RecalculateNormals();
		}
		
		/// <summary>
		/// Creates a circular mesh with the supplied radius and number of sections.  Useful in conjunction with DisplayObjectContainer.SetMouseArea()
		/// </summary>
		/// <returns>
		/// The round mesh.
		/// </returns>
		/// <param name='radius'>
		/// The radius of the new mesh.
		/// </param>
		/// <param name='numSlices'>
		/// Number slices.  5 will produce a pentagon, 8 a hexagon, etc.  10 is a high enough approximation for most sprites, high numbers will slow performance.
		/// </param>
		public static Mesh CreateRoundHitMesh(float radius, int numSlices)
		{
			var hitMesh = new Mesh();
			var vertices = new List<Vector3>();
			var triangles = new List<int>();
			var uv = new List<Vector2>();
			
			var garbageUV = new Vector2(0, 0);
			vertices.Add(new Vector3(0, 0, 0));
			vertices.Add(new Vector3(radius * Mathf.Cos (0), radius * Mathf.Sin (0), 0));
			uv.Add(garbageUV);
			uv.Add(garbageUV);
			for (var step = 1; step < numSlices; step++)
			{
				var theta = step * 360/numSlices * Mathf.Deg2Rad;
				var newVert = new Vector3(radius * Mathf.Cos (theta), radius * Mathf.Sin (theta), 0);
				vertices.Add(newVert);
				triangles.Add(0);
				triangles.Add(step-1);
				triangles.Add(step);
				uv.Add(garbageUV);
			}

			//add the last triangle
			triangles.Add(0);
			triangles.Add(numSlices-1);
			triangles.Add(1);
			
			hitMesh.vertices = vertices.ToArray();
			hitMesh.triangles = triangles.ToArray();
			hitMesh.uv = uv.ToArray();
			hitMesh.RecalculateNormals();
			
			return hitMesh;
		}
		
		public override float X { 
			get{return _x;} 
			set{
				_x = value; 
				if (PhysicsBody != null)
					PhysicsBody.Position = new Vector2(_x, _y);
				
				__UpdateMesh(); 
				NotifyParentMeshInvalid();
			}}
		public override float Y { 
			get{return _y;} 
			set{
				_y = value; 
				if (PhysicsBody != null)
					PhysicsBody.Position = new Vector2(_x, _y);
				
				__UpdateMesh(); 
				NotifyParentMeshInvalid();
			}}
		public override void SetPosition (float x, float y, float rotation)
		{
			if (PhysicsBody != null)
			{
				PhysicsBody.Position = new Vector2(x, y);
				PhysicsBody.Rotation = rotation;
			}
			
			base.SetPosition (x, y, rotation);
		}
		
		public override float Rotation{
			get { return _rotation; }
			set { 
				_rotation = value; 
				if (PhysicsBody != null)
					PhysicsBody.Rotation = value;
				
				__UpdateMesh();
				NotifyParentMeshInvalid();
			}
		}	
	}
}