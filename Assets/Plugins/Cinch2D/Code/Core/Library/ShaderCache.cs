using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Cinch2D
{
	/// <summary>
	/// Shader cache.  Loads shaders by name and then saves them so they load faster in the future.
	/// </summary>
	/// <exception cref='DllNotFoundException'>
	/// Thrown when shader is not found.
	/// </exception>
	public static class ShaderCache {
		
		public static Dictionary<string, Shader> _shaders;
		
		/// <summary>
		/// Gets the shader.
		/// </summary>
		/// <returns>
		/// The shader.
		/// </returns>
		/// <param name='shaderType'>
		/// The resource name of the shader to get.
		/// </param>
		/// <exception cref='DllNotFoundException'>
		/// Thrown when shader isn't found.
		/// </exception>
		public static Shader GetShader(string shaderType)
		{
			if (_shaders == null)
				_shaders = new Dictionary<string, Shader>();
			
			if (!_shaders.ContainsKey(shaderType))
			{
				var shader = Shader.Find (shaderType);
				if (shader == null)
					throw new DllNotFoundException("Could not find shader: " + shaderType);
				
				_shaders.Add (shaderType, shader);
			}
			
			return _shaders[shaderType];
		}
	}
}