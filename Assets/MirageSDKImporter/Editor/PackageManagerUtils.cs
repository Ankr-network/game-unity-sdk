using System;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace MirageSDKImporter.Editor
{
	public static class PackageManagerUtils
	{
		private static ListRequest _listRequest;
		private static PackageManagerRequestStatus _status;
		private static Action<bool, PackageCollection> _resultCallback;

		private static void Reset()
		{
			_status = PackageManagerRequestStatus.Idle;
			_resultCallback = null;
			_listRequest = null;
		}

		public static void RequestPackagesList(Action<bool, PackageCollection> resultCallback)
		{
			if (_status == PackageManagerRequestStatus.InProgress)
			{
				throw new InvalidOperationException("PackageManagerUtils only allows one request at a time");
			}

			_resultCallback = resultCallback;
			_status = PackageManagerRequestStatus.InProgress;
			_listRequest = Client.List();
			EditorApplication.update += EditorApplicationOnUpdate;
		}

		private static void EditorApplicationOnUpdate()
		{
			if (!_listRequest.IsCompleted)
			{
				return;
			}

			EditorApplication.update -= EditorApplicationOnUpdate;

			if (_listRequest.Status == StatusCode.Success)
			{
				_resultCallback?.Invoke(true, _listRequest.Result);
			}
			else
			{
				_resultCallback?.Invoke(false, _listRequest.Result);
				Debug.Log("PackageManagerUtils: Could not check for packages: " + _listRequest.Error.message);
			}

			Reset();
		}
	}
}