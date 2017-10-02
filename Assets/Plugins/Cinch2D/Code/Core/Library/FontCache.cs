using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cinch2D
{
	public class ResourceCache {

	    private static Dictionary<String, object> _cache;
		
	    public static T GetCachedItem <T>(String resourcePath)
	    {
	        if (_cache == null)
	            _cache = new Dictionary<string, object>();

			var strType = typeof(T).ToString();
			var cacheKey = GenerateCacheKey(strType, resourcePath);
			
	        if (!_cache.ContainsKey(cacheKey))
	        {
	            CacheResource(resourcePath);
	        }

	        return (T)_cache[cacheKey];
	    }
		
		private static string GenerateCacheKey(string type, string resourcePath)
		{
			return type + "_" + resourcePath;
		}

	    public static void CacheResource(string resourcePath)
	    {
	        if (_cache == null)
	            _cache = new Dictionary<string, object>();
			
	        var resource = Resources.Load(resourcePath);
	        if (resource == null)
	            throw (new Exception("ResourceCache: could not cache resource " + resourcePath));
			
			var strType = resource.GetType().ToString();
			var cacheKey = GenerateCacheKey(strType, resourcePath);
			
	        if (_cache.ContainsKey(cacheKey))
	            return;

	        _cache.Add(cacheKey, resource);
	    }
	}
}