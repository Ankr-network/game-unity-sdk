using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEditor.Compilation;
using UnityEngine;

namespace Tests
{
	public class CompilationTest
	{
		private static readonly Dictionary<string, ScriptCompilationSettings> ScriptCompilationSettings = new Dictionary<string, ScriptCompilationSettings>
		{
			["AndroidQa"] = new ScriptCompilationSettings
			{
				group = BuildTargetGroup.Android, options = ScriptCompilationOptions.None, target = BuildTarget.Android,
			},
			["iOSProd"] = new ScriptCompilationSettings
			{
				group = BuildTargetGroup.iOS, options = ScriptCompilationOptions.None, target = BuildTarget.iOS,
			}
		};

		private static string[] _keysForIterate = ScriptCompilationSettings.Keys.ToArray();

		[Test]
		public void CompileOnPlatform([ValueSource(nameof(_keysForIterate))] string keyOfDictionary)
		{
			var scriptCompilationSettings = ScriptCompilationSettings[keyOfDictionary];
			var filePath = $"{Directory.GetCurrentDirectory()}/Library/ScriptAssemblies/PlayerBuildInterface/" + scriptCompilationSettings.target;

			if (!Directory.Exists(filePath))
			{
				Directory.CreateDirectory(filePath);
			}

			CompilationPipeline.assemblyCompilationFinished += CompilationFinished;
			PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, filePath);
			CompilationPipeline.assemblyCompilationFinished -= CompilationFinished;
		}

		private static void CompilationFinished(string assemblyName, CompilerMessage[] compilerMessages)
		{
			foreach (var m in compilerMessages)
			{
				switch (m.type)
				{
					case CompilerMessageType.Warning:
						Debug.LogWarning(m.message);

						break;
					case CompilerMessageType.Error:
						Debug.LogError(m.message);

						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}