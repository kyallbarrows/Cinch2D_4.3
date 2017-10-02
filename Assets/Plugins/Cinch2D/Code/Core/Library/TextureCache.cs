using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cinch2D
{
	/// <summary>
	/// Loads and caches textures for faster loading in the future.  
	/// Care must be taken to balance load speed with resource consumption.  
	/// Textures are uncompressed, so caching too many textures can eat tons of memory quickly.
	/// </summary>
	public class TextureCache {

	    private static Dictionary<String, Texture2D> _textureCache;
		
		/// <summary>
		/// Gets a cached texture, or loads it and caches it.
		/// </summary>
		/// <returns>
		/// The cached texture.
		/// </returns>
		/// <param name='texturePath'>
		/// Resource path of the texture.
		/// </param>
	    public static Texture2D GetCachedTexture(String texturePath)
	    {
	        if (_textureCache == null)
	            _textureCache = new Dictionary<string, Texture2D>();

	        if (!_textureCache.ContainsKey(texturePath))
	        {
	            CacheTexture(texturePath);
	        }

	        return _textureCache[texturePath];
	    }
		
		/// <summary>
		/// Caches a texture.
		/// </summary>
		/// <param name='texturePath'>
		/// Resource path of the texture to cache.
		/// </param>
	    public static void CacheTexture(string texturePath)
	    {
	        if (_textureCache == null)
	            _textureCache = new Dictionary<string, Texture2D>();

	        if (_textureCache.ContainsKey(texturePath))
	            return;

	        var tex = Resources.Load(texturePath) as Texture2D;
	        if (tex == null)
	        {
	            throw (new Exception("CacheTexture: could not find texture: " + texturePath));
	        }
	        _textureCache.Add(texturePath, tex);
	    }
	}
}