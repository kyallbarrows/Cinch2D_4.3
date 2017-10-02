using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cinch2D
{
	/*! Used to define Sprite templates (symbols) */
	public class SymbolDef
	{
		public const string IMAGE_SPRITE_OBJECT = "sprite.image";
		
		public string ObjectType = IMAGE_SPRITE_OBJECT;
		/// <summary>
		/// A unique class name for this symbol, like "MainMenuButton"
		/// </summary>
		public string SymbolId;
		/// <summary>
		/// Resource path for texture
		/// </summary>
		public string TexturePath;
		/// <summary>
		/// Determines how large object will be.  100 pixels per meter with a 100x100 texture will produce a 1 meter object
		/// </summary>
		public float PixelsPerMeter = CinchOptions.DefaultPixelsPerMeter;
		/// <summary>
		/// Event dispatched when any mouse event occurs
		/// </summary>
		public RegistrationPoint RegistrationPoint;
		/// <summary>
		/// Left side of the texture rectangle, if any
		/// </summary>
		public float Left;
		/// <summary>
		/// The top sede of the texture rectangle, if any
		/// </summary>
		public float Top;
		/// <summary>
		/// The width of the texture rectangle, if any
		/// </summary>
		public float Width;
		/// <summary>
		/// The height of the texture rectangle, if any.
		/// </summary>
		public float Height;
			
		public SymbolDef()
		{
		}
	}


	/*! Mimics Flash's library, allowing you to predefine symbols, and create them later */
	public class Library : MonoBehaviour
	{
		private static Library _instance;
		public static Library Instance
		{
			get {
				if (_instance == null)
					_instance = new GameObject("Library").AddComponent<Library>();
				
				return _instance;
			}
		}
		
		protected Dictionary<string, SymbolDef> _definitions;
		protected Dictionary<string, Shader> _shaders;
		
		private Library()
		{
		}
		
		public void Awake ()
		{
			_definitions = new Dictionary<string, SymbolDef>();
			_shaders = new Dictionary<string, Shader>();
		}
		
		/// <summary>
		/// Creates a new instance of a symbol defined with AddLibraryDefinition.
		/// </summary>
		/// <param name='classId'>
		/// Id of the predefined symbol to create
		/// </param>
		/// <param name='name'>
		/// Name to assign to new instance
		/// </param>
		public static DisplayObjectContainer New(string classId, string name = "anonymous")
		{
			return Instance.__New(classId, name);
		}
		
		/// <summary>
		/// Creates a new instance of the supplied type T
		/// </summary>
		/// <param name='name'>
		/// Name to assign to the new instance.
		/// </param>
		/// <typeparam name='T'>
		/// The class type to create
		/// </typeparam>
		public static T New<T>(string name = "anonymous") where T:DisplayObjectContainer
		{
			Type t = typeof(T);
			var comp = new GameObject(name).AddComponent(typeof(T));
			((DisplayObjectContainer)comp).Name = name;
			return (T)Convert.ChangeType(comp, t);
		}
		
		public DisplayObjectContainer __New(string classId, string name)
		{
			return CreateNew(classId, name);
		}

		//keep this, add New<T> and GetIfAvailable<T> to recyclebin, provides 2 ways: get extension of sprite, or get from string classid of symbol
		//can 
		protected DisplayObjectContainer CreateNew(string classId, string name)
		{
			if (!_definitions.ContainsKey(classId))
				throw new DllNotFoundException("Could not find classId " + classId);
			
			var sd = _definitions[classId];
			
			switch(sd.ObjectType)
			{
			case SymbolDef.IMAGE_SPRITE_OBJECT:
				CinchSprite spr;
				if (sd.Width == 0)
				{
					spr = CinchSprite.NewFromImage(sd.TexturePath, sd.PixelsPerMeter, sd.RegistrationPoint);
					spr.Name = name;
					return spr;
				}
				
				spr = CinchSprite.NewFromSpriteSheet(sd.TexturePath, sd.Left, sd.Top, sd.Width, sd.Height, sd.PixelsPerMeter, sd.RegistrationPoint); 
				spr.Name = name;
				return spr;
			}
			
			return null;
		}

		/// <summary>
		/// Adds a definition for a symbol.
		/// </summary>
		/// <param name='sd'>
		/// The definition to add.
		/// </param>
		public static void AddDefinition(SymbolDef sd)
		{
			Instance.__AddDefinition(sd);
		}
		
		public void __AddDefinition(SymbolDef sd)
		{
			_definitions[sd.SymbolId] = sd;
		}
		
		/// <summary>
		/// Creates and adds a new Sprite definition.
		/// </summary>
		/// <returns>
		/// The newly created definition
		/// </returns>
		/// <param name='classId'>
		/// A unique id for the new symbol (like "MenuButton")
		/// </param>
		/// <param name='texturePath'>
		/// Texture path
		/// </param>
		/// <param name='registrationPoint'>
		/// Registration point.
		/// </param>
		public static SymbolDef AddImageSpriteDefinition(string classId, string texturePath, RegistrationPoint? registrationPoint = null)
		{
			return AddImageSpriteDefinition(classId, texturePath, null, registrationPoint);
		}
		
		/// <summary>
		/// Adds the image sprite definition.
		/// </summary>
		/// <returns>
		/// The image sprite definition.
		/// </returns>
		/// <param name='classId'>
		/// Class identifier.
		/// </param>
		/// <param name='texturePath'>
		/// Texture path.
		/// </param>
		/// <param name='spriteArea'>
		/// Texture rectangle.  Defautls to using entire texture.
		/// </param>
		/// <param name='registrationPoint'>
		/// Registration point of the definition.  Defaults to center.
		/// </param>
		public static SymbolDef AddImageSpriteDefinition(string classId, string texturePath, Rect? spriteArea, RegistrationPoint? registrationPoint = null)
		{
			var sd = new SymbolDef();
			sd.SymbolId = classId;
			sd.ObjectType = SymbolDef.IMAGE_SPRITE_OBJECT;
			sd.TexturePath = texturePath;
			var sa = spriteArea.GetValueOrDefault(new Rect());
			sd.Left = sa.x;
			sd.Top = sa.y;
			sd.Width = sa.width;
			sd.Height = sa.height;
			sd.RegistrationPoint = registrationPoint.GetValueOrDefault(RegistrationPoint.Center);
			Instance.__AddDefinition(sd);
			return sd;
		}
		
		public void OnApplicationQuit ()
		{
			_instance = null;
		}	
		
		public void Recycle(DisplayObjectContainer doc)
		{
		}
	}
}