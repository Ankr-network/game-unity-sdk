namespace AnkrSDK.Ads.WebGL
{
	public static class WebGLInterlayer
	{
	#if UNITY_WEBGL
		[System.Runtime.InteropServices.DllImport("__Internal")]
		public static extern string GetUniqueID(string storageKey = null);
	#endif
	}
}