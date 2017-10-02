using UnityEngine;
using System.Collections;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using System.Collections.Generic;

namespace Cinch2D
{
	/// <summary>
	/// Internal class, nothing to see here.
	/// </summary>
	public class PhysicsDebugDraw : MonoBehaviour {
		
		private static Color _awakeColor;
		private static Color _asleepColor;
		private static Color _staticColor;
		
		private World _world;
		private PhysicsContainer _physicsContainer;
		private Shader _shader;
		private Material _defaultMaterial;
		private Material _circleMaterial;
		public void Init(World w, PhysicsContainer p)
		{
			_world = w;
			_physicsContainer = p;
			_shader = ShaderCache.GetShader(CinchOptions.DefaultShader);
			_defaultMaterial = new Material (_shader);
			_circleMaterial = new Material (_shader);
			_defaultMaterial.mainTexture = TextureCache.GetCachedTexture("DebugDrawOutline");
			_circleMaterial.mainTexture = TextureCache.GetCachedTexture("DebugDrawCircleOutline");
			
			_awakeColor = Color.red;
			_asleepColor = Color.green;
			_staticColor = Color.gray;
		}
		
		public void OnPostRender()
		{
			if (_world == null || _physicsContainer == null)
				return;
			
			Matrix4x4 tform = _physicsContainer.__GetGlobalTransform();
			
			foreach(Body body in _world.BodyList)
			{
				if (!body.DisableDebugDraw)
				{
					foreach(Fixture fixture in body.FixtureList)
					{
						var mesh = CreateFixtureMesh(fixture);
						var mat = fixture.ShapeType == ShapeType.Circle ? _circleMaterial : _defaultMaterial;
						mat.color = _awakeColor;
						if (body.IsStatic)
							mat.color = _staticColor;
						else if (!body.Awake)
							mat.color = _asleepColor;
						
						for (int pass = 0; pass < mat.passCount; pass++)
						{
							mat.SetPass(pass);
							Graphics.DrawMeshNow(mesh, 
								tform * Matrix4x4.TRS(
									new Vector3(body.Position.x, body.Position.y, 10f), 
									Quaternion.AngleAxis(body.Rotation * Mathf.Rad2Deg, 
									Vector3.forward), new Vector3(1, 1, 1)));
					   }
					}
				}
			}
		}
		
		private Mesh CreateFixtureMesh(Fixture fixture)
		{
			var mesh = new Mesh();
			var vertices = new List<Vector3>();
			var uv = new List<Vector2>();
			var triangles = new List<int>();
			
			switch(fixture.ShapeType)
			{
			case ShapeType.Polygon:
				var polygon = fixture.Shape as PolygonShape;
				//Add center vertex
				vertices.Add (new Vector3(0, 0, 0));
				uv.Add (new Vector2(0, 0));
				
				for(var i=0; i<polygon.Vertices.Count-1; i++)
				{
					//add a triangle to mesh
					AddMeshTriangle(vertices, uv, triangles, polygon.Vertices[i], polygon.Vertices[i+1]);
				}
				//then add last
				AddMeshTriangle(vertices, uv, triangles, polygon.Vertices[polygon.Vertices.Count-1], polygon.Vertices[0]);
				
				break;
				
			case ShapeType.Circle:
				var circle = fixture.Shape as CircleShape;
				var negative = circle.Radius * -1f;
				var positive = circle.Radius;
						
				vertices.Add(new Vector3(negative,  negative, 0f));
				vertices.Add(new Vector3(positive, negative, 0f));
				vertices.Add(new Vector3(positive, positive, 0f));
				vertices.Add(new Vector3(negative,  positive, 0f));
				
				uv.Add(new Vector2 (0, 0));
				uv.Add(new Vector2 (1, 0));
				uv.Add(new Vector2 (1, 1));
				uv.Add(new Vector2 (0, 1));
				triangles.Add(3);
				triangles.Add(2);
				triangles.Add(1);
				triangles.Add(0);
				triangles.Add(3);
				triangles.Add(1);
				
				break;
			}
			
			mesh.vertices = vertices.ToArray();
			mesh.uv = uv.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.RecalculateNormals();
			return mesh;
		}
		
		private void AddMeshTriangle(List<Vector3> verts, List<Vector2> uv, List<int> triangles, Vector2 v1, Vector2 v2)
		{
			var vertCount = verts.Count;
			verts.Add (new Vector3(v1.x, v1.y, 0));
			verts.Add (new Vector3(v2.x, v2.y, 0));
			uv.Add (new Vector2(0, 1));
			uv.Add (new Vector2(1, 1));
			
			triangles.Add (0);
			triangles.Add (vertCount+1);
			triangles.Add (vertCount);
		}
		
		private void CreateCircleMesh()
		{
			
		}
	}
}