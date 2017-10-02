using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Cinch2D
{
	/// <summary>
	/// A value-menu version of the AS3 TextField class.  Won't work with dynamic fonts, and only does single lines of text.  Better version coming soon, hopefully.
	/// </summary>
	public class SimpleTextField : DisplayObjectContainer {
		
		protected string _rawText;
		protected float _width = 1f;
		protected float _height = 1f;
		
		//http://en.wikipedia.org/wiki/Typeface#Font_metrics
		protected float _fontAscenderHeight;
		protected float _fontCapHeight;
		protected float _fontBaseline;
		protected float _fontDescenderHeight;
		protected float _fontMedian;
		protected float _fontScale;
		protected Font _fontFace;
		
		/// <summary>
		/// Gets or sets the font face.  Must not be dynamic font.
		/// </summary>
		/// <value>
		/// The font face.
		/// </value>
		public Font FontFace { 
			get { return _fontFace; }
			set { 
				_fontFace = value;
				ScanFont();
				RebuildText();
				__UpdateMesh();
			}
		}
		
		protected float _fontHeight = .25f;
		/// <summary>
		/// The height of a capital letter in meters.
		/// </summary>
		/// <value>
		/// The height of the font.
		/// </value>
		public float FontHeight { 
			get { return _fontHeight; }
			set {
				_fontHeight = value;
				RecalculateScale();
				RebuildText();
				__UpdateMesh();
			}
		}
		
		/// <summary>
		/// Gets or sets the text.  Any character not in the font will be removed
		/// </summary>
		/// <value>
		/// The text.
		/// </value>
		public string Text { get{ return _rawText; } set { _rawText = value; RebuildText(); __UpdateMesh ();}}
		
		/// <summary>
		/// Creates a new SimpleTextField instance.
		/// </summary>
		/// <returns>
		/// The textfield.
		/// </returns>
		/// <param name='text'>
		/// Text.
		/// </param>
		/// <param name='fontName'>
		/// Font name.  Must be a non-dynamic font.
		/// </param>
		/// <param name='fontHeight'>
		/// Font height.
		/// </param>
		/// <param name='textAnchor'>
		/// Text anchor <see cref="TextAnchor"/>
		/// </param>
		/// <param name='textAlignment'>
		/// Text alignment <see cref="TextAlignment"/>
		/// </param>
		/// <exception cref='ArgumentException'>
		/// Is thrown when font is not found
		/// </exception>
		public static SimpleTextField NewFromString(string text, string fontName, float fontHeight, TextAnchor textAnchor = TextAnchor.MiddleCenter, string characterSet = null)
		{
			if (String.IsNullOrEmpty(characterSet))
				characterSet = CharacterSet.Latin1;

			var go = new GameObject("SimpleTextField");
			
			var fontFace = ResourceCache.GetCachedItem<Font>(fontName);
			if (fontFace == null)
				throw new ArgumentException("Font could not be found: " + fontName);
			
			var dynText = go.AddComponent<SimpleTextField>();
			dynText.Init(text, fontFace, fontHeight, textAnchor, characterSet);
			return dynText;
		}
		
		private static Shader _textShader;
		
		/// <summary>
		/// Initializes the textfield.  Generally, you should just use SImpleTextField.NewFromString().
		/// </summary>
		/// <param name='text'>
		/// Text.
		/// </param>
		/// <param name='fontFace'>
		/// Font face.
		/// </param>
		/// <param name='fontHeight'>
		/// Font height.
		/// </param>
		/// <param name='textAnchor'>
		/// Text anchor.
		/// </param>
		public void Init(string text, Font fontFace, float fontHeight, TextAnchor textAnchor, string characterSet)
		{
			_color = new Color(1f, 1f, 1f, 1f);
			_rawText = text;
			_fontFace = fontFace;
			_fontHeight = fontHeight;
			_textAnchor = textAnchor;
			
			_meshFilter = gameObject.AddComponent<MeshFilter>();
			_meshFilter.sharedMesh = _transformedMesh;
			
			if (_textShader == null)
			{
				_textShader = Shader.Find ("Transparent/Cutout/Diffuse");
			}

			
			_meshRenderer = gameObject.AddComponent<MeshRenderer>();
			_meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			_meshRenderer.receiveShadows = false;
			
			_material = new Material(_textShader);
			_material.mainTexture = fontFace.material.mainTexture;
			_meshRenderer.material = _material;
			
			_fontFace.RequestCharactersInTexture(characterSet);
			ScanFont();
			RebuildText();
		}

		protected void ScanFont()
		{
			CharacterInfo characterInfo;
			
			//get the baseline and cap height
			var capsWithNoDescender = "0123456789ABCDEFGHIJKLMNOPRSTUVWXYZ";
			_fontBaseline = 10000f;
			_fontCapHeight = -10000f;
			var totalMedian = 0f;
			
			for(var i=0; i<capsWithNoDescender.Length; i++)
			{
				if (_fontFace.GetCharacterInfo(capsWithNoDescender[i], out characterInfo))
				{
					//for whatever reason, font rects have corner at bottom, and negative heights, so yMin and yMax are reversed
					_fontBaseline = Mathf.Min (_fontBaseline, -1 * characterInfo.minY);
					_fontCapHeight = Mathf.Max (_fontCapHeight, -1 * characterInfo.maxY);
					totalMedian += -1 * characterInfo.minY + (characterInfo.maxY - characterInfo.minY) / -2;
				}
			}
			

			_fontMedian = totalMedian / capsWithNoDescender.Length;
			RecalculateScale();
		}
		
		protected void RecalculateScale()
		{
			_fontScale = _fontHeight / Mathf.Abs (_fontBaseline - _fontCapHeight);
		}
		
		protected TextAnchor _textAnchor;
		/// <summary>
		/// Gets or sets the text anchor.  <see cref="TextAnchor"/>
		/// </summary>
		/// <value>
		/// The anchor.
		/// </value>
		public TextAnchor Anchor { get { return _textAnchor; } set { _textAnchor = value; RebuildText(); __UpdateMesh();}}
		
		protected void RebuildText()
		{
			if (Text == "" || _width <= 0 || _height <= 0)
				return;
			
			CharacterInfo characterInfo;
			
			var totalWidth = 0f;
			
			//calculate the width
			for (var i = 0; i < _rawText.Length; i++)
			{
				if (_fontFace.GetCharacterInfo(_rawText[i], out characterInfo))
					totalWidth += characterInfo.advance;
			}
			
			totalWidth *= _fontScale;
			
			var left = 0f - totalWidth/2;
			if (_textAnchor == TextAnchor.LowerLeft || _textAnchor == TextAnchor.MiddleLeft || _textAnchor == TextAnchor.UpperLeft)
				left = 0f;
			else if (_textAnchor == TextAnchor.LowerRight || _textAnchor == TextAnchor.MiddleRight || _textAnchor == TextAnchor.UpperRight)
				left = 0 - totalWidth;
			
			var horizCenterLine = _fontMedian * _fontScale;
			if (_textAnchor == TextAnchor.LowerLeft || _textAnchor == TextAnchor.LowerCenter || _textAnchor == TextAnchor.LowerRight)
				horizCenterLine = _fontCapHeight * _fontScale;
			else if (_textAnchor == TextAnchor.UpperLeft || _textAnchor == TextAnchor.UpperCenter || _textAnchor == TextAnchor.UpperRight)
				horizCenterLine = _fontBaseline * _fontScale;
			
			var vertices = new List<Vector3>();
			var uv = new List<Vector2>();
			var triangles = new List<int>();
			var totalCharactersRendered = 0;
			for (var i = 0; i < _rawText.Length; i++)
			{
				//skip over characters we don't have
				if (_fontFace.GetCharacterInfo(_rawText[i], out characterInfo))
				{
					vertices.Add (new Vector3(left + _fontScale * characterInfo.minX, 	_fontScale * characterInfo.minY + horizCenterLine, 0f)); //left top   //should be horizCenterLine - yValue, but y is flipped, so +
					vertices.Add (new Vector3(left + _fontScale * characterInfo.maxX, 	_fontScale * characterInfo.minY + horizCenterLine, 0f)); //right top
					vertices.Add (new Vector3(left + _fontScale * characterInfo.maxX,	_fontScale * characterInfo.maxY + horizCenterLine,	0f)); //right bottom
					vertices.Add (new Vector3(left + _fontScale * characterInfo.minX, 	_fontScale * characterInfo.maxY + horizCenterLine, 	0f)); //left bottom
					
					uv.Add (new Vector2(characterInfo.uvTopLeft.x, 	characterInfo.uvBottomLeft.y)); //upper left
					uv.Add (new Vector2(characterInfo.uvTopRight.x, 	characterInfo.uvBottomLeft.y)); //upper right
					uv.Add (new Vector2(characterInfo.uvTopRight.x, 	characterInfo.uvTopLeft.y)); //lower right
					uv.Add (new Vector2(characterInfo.uvTopLeft.x, 	characterInfo.uvTopLeft.y)); //lower left
					
					triangles.Add (totalCharactersRendered*4 + 3);
					triangles.Add (totalCharactersRendered*4 + 2);
					triangles.Add (totalCharactersRendered*4 + 1);
					triangles.Add (totalCharactersRendered*4 + 0);
					triangles.Add (totalCharactersRendered*4 + 3);
					triangles.Add (totalCharactersRendered*4 + 1);
					
					left += characterInfo.advance * _fontScale;
					totalCharactersRendered++;
				}
			}
			
			_originalMesh = new Mesh();
			_originalMesh.name = "SimpleTextField Mesh";
			_originalMesh.vertices = vertices.ToArray();
			_originalMesh.uv = uv.ToArray();
			_originalMesh.triangles = triangles.ToArray();
			_originalMesh.RecalculateNormals();	
			
			UpdateColorTint();
		}
		
		private Color _color;
		
		/// <summary>
		/// Gets or sets the color of the text.
		/// </summary>
		/// <value>
		/// The color of the text.
		/// </value>
		public Color TextColor
		{
			get{
				return _color;
			}
			set{
				_color = value;
				UpdateColorTint ();
			}
		}
		
		/// <summary>
		/// Gets or sets the transparency, between 0 and 1.
		/// </summary>
		/// <value>
		/// The alpha.
		/// </value>
		public override float Alpha {
			get { return _color.a; }
			set { 
				_color.a = value; 
				_alphaUpdated = true; 
				__NotifyChildrenAlphaUpdated();
			} 
		}
		
		public override float __GetCombinedAlpha()
		{
			if (Parent == null)
				return _color.a;
			
			return _color.a * Parent.__GetCombinedAlpha();
		}

		protected override void UpdateColorTint()
		{
			var combinedAlpha = __GetCombinedAlpha();
			_meshRenderer.material.color = new Color(_color.r, _color.g, _color.b, combinedAlpha);
		}
		
		
		/// <summary>
		/// Gets or sets the width of the object AND all children, in meters.  Affects ScaleX when set.  Unlike a Flash text field, width does not expand the box, but simply stretches the object.
		/// </summary>
		/// <value>
		/// The width in meters.  Cannot be set to 0, or to obnoxiously large values (currently 5000 meters).
		/// </value>
		/// <exception cref='Exception'>
		/// Thrown when set to a bad value, see Value above.
		/// </exception>
		public override float Width { 
			get { return base.Width; }
			set { base.Width = value; }
		}
		
		/// <summary>
		/// Gets or sets the height of the object AND all children, in meters.  Affects ScaleY when set.  Unlike a Flash text field, width does not expand the box, but simply stretches the object.
		/// </summary>
		/// <value>
		/// The height in meters.  Cannot be set to 0, or to obnoxiously large values (currently 5000 meters).
		/// </value>
		/// <exception cref='Exception'>
		/// Thrown when set to a bad value, see Value above.
		/// </exception>
		public override float Height { 
			get {	return base.Height; }
			set {	base.Height = value; }
		}
		
	}
}