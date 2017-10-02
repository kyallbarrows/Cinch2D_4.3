using UnityEngine;
using System.Collections;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision;


namespace Cinch2D
{
	/// <summary>
	/// A sprite that also contains a Farseer Physics world.
	/// Use .World to access the physics World instance.
	/// </summary>
	public class PhysicsContainer : CinchSprite {
		
		private World _world;
		/// <summary>
		/// Gets the Farseer Physics World
		/// </summary>
		/// <value>
		/// The World.
		/// </value>
		public World World { get { return _world; }}
		
		/// <summary>
		/// Creates and returns a new empty PhysicsContainer
		/// </summary>
		/// <returns>
		/// The new physics container.
		/// </returns>
		/// <param name='gravity'>
		/// Gravity.  Try Vector2.up * -10f for a decent approximation of normal gravity.
		/// </param>
		/// <param name='name'>
		/// Name.
		/// </param>
		public static PhysicsContainer NewEmpty(Vector2 gravity, string name = "Anonymous Physics Container")
		{
			GameObject go = new GameObject(name);
			var physicsContainer = go.AddComponent<PhysicsContainer>();
			physicsContainer.Init (gravity);
			return physicsContainer;
		}
		
		/// <summary>
		/// Initialize the world with the given gravity.
		/// </summary>
		/// <param name='gravity'>
		/// Gravity.
		/// </param>
		public void Init(Vector2 gravity)
		{
			_world = new World(gravity);
		}
		
		public override void __InternalOnEnterFrame()
		{
			base.__InternalOnEnterFrame();
			var timeStep = Mathf.Min (1f/30f, Clock.DeltaTime);
			_world.Step(timeStep);
		}
			
		#region DebugDraw stuff
		private bool _debugDrawEnabled;
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PhysicsContainer"/> has debug draw enabled.
		/// Debug Draw shows graphical representations of the underlying physics bodies. 
		/// WARNING: It's not fast. 
		/// ANOTHER WARNING: It looks completely awful, and we feel bad, but we're planning on improving in forthcoming releases.  Send us email if you have suggestions.  See: http://oi43.tinypic.com/30k4uxc.jpg
		/// Color meanings:
		/// Red: Awake object
		/// Green: Sleeping object
		/// Gray: Static object
		/// </summary>
		/// <value>
		/// <c>true</c> if debug draw enabled; otherwise, <c>false</c>.
		/// </value>
		public bool DebugDrawEnabled{
			get { return _debugDrawEnabled; }
			set {
				if (value == _debugDrawEnabled)
					return;
				
				_debugDrawEnabled = value;
				if (_debugDrawEnabled)
				{
					_debugDraw = Stage.Instance.TheCamera.gameObject.AddComponent<PhysicsDebugDraw>() as PhysicsDebugDraw;
					_debugDraw.name = "PhysicsDebugDraw" + this.Id.ToString();
					_debugDraw.Init (_world, this);
				}
				else
				{
					Destroy(_debugDraw);
					_debugDraw = null;
				}
			}
		}
		private PhysicsDebugDraw _debugDraw;
			
		private Mesh CreateFixtureMesh(Fixture fixture)
		{
			var mesh = new Mesh();
	        var fullWidth = 5;
	        var fullHeight = 5;
			var left = fullWidth * (-.5f);
			var right = fullWidth * (.5f);
			var top = fullHeight * (-.5f);
			var bottom = fullHeight * (.5f);
			var front = -.1f;
			var flt = new Vector3(left,  top, front); 
			var frt = new Vector3(right, top, front);
			var frb = new Vector3(right, bottom, front); 
			var flb = new Vector3(left,  bottom, front);
					
			mesh.vertices = new Vector3[] {
										flt, frt, frb, flb
			};
			
			mesh.uv = new Vector2[] {	
				new Vector2 (0, 0), new Vector2 (1, 0), new Vector2 (1, 1), new Vector2 (0, 1)
			};			
			
			mesh.triangles = new int[] {
				3 , 2 , 1 , 0 , 3 , 1
			};
			mesh.RecalculateNormals();
			return mesh;
		}
		#endregion
	}
}