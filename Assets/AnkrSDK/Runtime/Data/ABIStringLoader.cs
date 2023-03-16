using System.IO;
using UnityEngine;

namespace AnkrSDK.Data
{
	public class ABIStringLoader
	{
		private readonly string _pathToAbiFolder;

		public ABIStringLoader(string relativePath)
		{
			_pathToAbiFolder = Path.Combine(Application.dataPath, relativePath);
		}

		public string LoadAbi(string abiName)
		{
			var filePath = Path.Combine(_pathToAbiFolder, $"{abiName}_ab.txt");
			return File.ReadAllText(filePath);
		}
	}
}