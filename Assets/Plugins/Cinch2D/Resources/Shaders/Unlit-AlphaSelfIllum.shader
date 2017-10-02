//Originally by Bill Vinton (Sync1B)

Shader "Unlit/AlphaSelfIllum" {
	Properties {
		_Color ("Color Tint", Color) = (1,1,1,1)
		_MainTex ("SelfIllum Color (RGB) Alpha (A)", 2D) = "white"
	} 
	 
	SubShader {
	    Tags {Queue = Transparent}
	    ZWrite Off
	    Blend SrcAlpha OneMinusSrcAlpha
	 
	    Pass { 
	        SetTexture[_MainTex] {
	            ConstantColor [_Color]
	            Combine texture * constant
	        } 
	    } 
	} 
}