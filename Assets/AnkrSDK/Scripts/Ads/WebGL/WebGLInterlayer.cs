using System.Runtime.InteropServices;

namespace AnkrSDK.Ads
{
	public static class WebGLInterlayer
	{
		[DllImport("__Internal")]
		public static extern string GetUniqueID(string storageKey = null);
	}
}
