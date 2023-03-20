using System.IO;
using UnityEngine;

namespace AnkrSDK.Data
{
	public static class ABIStringLoader
	{
		private const string RelativeABIsFolderPath = "AnkrSDK/Examples/ABIs";
		private static string AbsoluteABIsFolderPath = Path.Combine(Application.dataPath, RelativeABIsFolderPath);

		public static string LoadAbi(string abiName)
		{
			var filePath = Path.Combine(AbsoluteABIsFolderPath, $"{abiName}_abi.txt");
			if (File.Exists(filePath))
			{
				return File.ReadAllText(filePath);
			}
			
			Debug.LogError($"File {filePath} not found");

			return "";
		}
	}
}