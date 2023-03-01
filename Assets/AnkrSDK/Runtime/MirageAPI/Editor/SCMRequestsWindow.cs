using System;
using System.Collections.Generic;
using System.IO;
using AnkrSDK.MirageAPI.Data;
using AnkrSDK.MirageAPI.MirageID.Implementation;
using AnkrSDK.MirageAPI.SmartContractManager.Data.AllContracts;
using AnkrSDK.MirageAPI.SmartContractManager.Data.GetContract;
using AnkrSDK.MirageAPI.SmartContractManager.Implementation;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace AnkrSDK.MirageAPI.Editor
{
	public class SCMRequestsWindow : EditorWindow
	{
		private static string _applicationToken;
		private static string _contractId = "";
		private static List<SCMContractIDPair> _allContracts = new List<SCMContractIDPair>();

		private static bool _contractRequestInProgress;
		private static bool _allContractsRequestInProgress;
		private static bool _applicationTokenRequestInProgress;
		
		[MenuItem("AnkrSDK/SCMEditor")]
		public static void ShowWindow()
		{
			var window = GetWindow<SCMRequestsWindow>();

			window.titleContent = new GUIContent("Smart Contract Manager Requests");
		}

		private void OnGUI()
		{
			if (string.IsNullOrWhiteSpace(_applicationToken))
			{
				if (!_applicationTokenRequestInProgress)
				{
					GetApplicationToken().Forget();
				}

				GUILayout.Label("Waiting to receive application token...", EditorStyles.boldLabel);
				return;
			}

			EditorGUILayout.LabelField("Enter contract id to receive: ");
			_contractId = EditorGUILayout.TextField("ContractId", _contractId);

			if (_contractRequestInProgress || _allContractsRequestInProgress)
			{
				GUI.enabled = false;
			}

			if (string.IsNullOrWhiteSpace(_contractId))
			{
				GUILayout.Label("Input a correct contract id", EditorStyles.boldLabel);
			}
			else
			{
				if (GUILayout.Button("Request contract info"))
				{
					RequestContractInfo(_contractId).Forget();
				}
			}

			if (GUILayout.Button("Clear"))
			{
				_allContracts.Clear();
				_applicationToken = string.Empty;
			}

			if (GUILayout.Button("Display All available contracts"))
			{
				RequestContractsList().Forget();
			}

			if (_allContractsRequestInProgress)
			{
				GUILayout.Label("Loading all contracts info...", EditorStyles.boldLabel);
			}

			DrawContractsInfo();

			GUI.enabled = true;
		}

		private void DrawContractsInfo()
		{
			foreach (var contractIDPair in _allContracts)
			{
				var r = EditorGUILayout.BeginHorizontal("Button");
				if (GUI.Button(r, GUIContent.none))
				{
					RequestContractInfo(contractIDPair.ID).Forget();
					Debug.Log($"Requesting Info for: {contractIDPair.Name}");
				}

				GUILayout.Label($"{contractIDPair.Name} | ID:{contractIDPair.ID}");
				EditorGUILayout.EndHorizontal();
			}
		}

		private async UniTaskVoid RequestContractsList()
		{
			_allContractsRequestInProgress = true;
			try
			{
				_allContracts.Clear();
				var scmRequests = new SmartContractRequests();
				scmRequests.SetToken(_applicationToken);
				_allContracts = await scmRequests.GetAllContracts();
			}
			catch (UnityWebRequestException ex)
			{
				if (ex.ResponseCode == 401)
				{
					_applicationToken = string.Empty;
				}

				Debug.LogError(ex.Message);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}

			_allContractsRequestInProgress = false;
		}

		private async UniTaskVoid GetApplicationToken()
		{
			_applicationTokenRequestInProgress = true;

			try
			{
				var mirageAPISettingsSO = LoadMirageAPISettingsSO();

				Selection.activeObject = mirageAPISettingsSO;
				if (string.IsNullOrEmpty(mirageAPISettingsSO.ClientSecret))
				{
					GUILayout.Label($"Please fill in ClientSecret");
				}

				if (string.IsNullOrEmpty(mirageAPISettingsSO.ClientID))
				{
					GUILayout.Label($"Please fill in ClientID");
				}

				var applicationRequests = new MirageIdApplicationRequests();
				_applicationToken = await applicationRequests.Initialize(mirageAPISettingsSO);
				Debug.Log($"Application token: {_applicationToken}");
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}

			_applicationTokenRequestInProgress = false;
		}

		private static MirageAPISettingsSO LoadMirageAPISettingsSO()
		{
			const string resourcesPath = MirageConstants.ResourcesPath;

			if (!Directory.Exists(resourcesPath))
			{
				Directory.CreateDirectory(resourcesPath);
			}

			var mirageAPISettingsSO = Resources.Load<MirageAPISettingsSO>(MirageConstants.MirageAPISettingsName);
			if (mirageAPISettingsSO != null)
			{
				return mirageAPISettingsSO;
			}

			Debug.LogWarning(MirageConstants.MirageAPISettingsName + " can't be found, creating a new one...");
			mirageAPISettingsSO = CreateInstance<MirageAPISettingsSO>();
			AssetDatabase.CreateAsset(mirageAPISettingsSO, MirageConstants.DefaultSettingsAssetPath);

			return mirageAPISettingsSO;
		}

		private static async UniTaskVoid RequestContractInfo(string contractId)
		{
			_contractRequestInProgress = true;

			try
			{
				Debug.Log($"Requesting contract info for {contractId}");
				var scmRequests = new SmartContractRequests();
				scmRequests.SetToken(_applicationToken);
				var contractInfo = await scmRequests.GetContractInfo(contractId);
				Debug.Log(
					$"Received contract info: {contractInfo.ContractName} with {contractInfo.Deployments.Count} deployments");
				CreateContractSO(contractInfo);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}

			_contractRequestInProgress = false;
		}

		private static void CreateContractSO(GetContractResponseDTO contractInfo)
		{
			Debug.Log("Generating contracts info");
			var assetPath = $"{MirageConstants.ResourcesPath}/{contractInfo.ContractName}.asset";

			var contractInfoSO = AssetDatabase.LoadAssetAtPath<SCMContractInfoSO>(assetPath);
			var isAssetCreationRequired = false;
			if (!contractInfoSO)
			{
				isAssetCreationRequired = true;
				contractInfoSO = CreateInstance<SCMContractInfoSO>();
			}

			contractInfoSO.Setup(contractInfo.ContractName, contractInfo.ContractABI,
				contractInfo.Contract.ContractAddress);

			if (isAssetCreationRequired)
			{
				AssetDatabase.CreateAsset(contractInfoSO, assetPath);
			}

			EditorUtility.SetDirty(contractInfoSO);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Selection.activeObject = contractInfoSO;
			Debug.Log($"Created Asset: {assetPath}");
		}
	}
}