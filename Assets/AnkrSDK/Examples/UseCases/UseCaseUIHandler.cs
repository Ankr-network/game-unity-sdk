using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnkrSDK.UseCases
{
	public class UseCaseUIHandler : MonoBehaviour
	{
		[SerializeField]
		private List<UseCaseUI> _useCaseUIs;

		private void Awake()
		{
			SubscribeToEvents();
		}

		private void OnDestroy()
		{
			UnsubscribeToEvents();
		}
		
		private void SubscribeToEvents()
		{
			foreach (var useCaseUI in _useCaseUIs)
			{
				useCaseUI.OnButtonClickedEvent += SetButtonsActive;
			}
		}
		
		private void UnsubscribeToEvents()
		{
			foreach (var useCaseUI in _useCaseUIs)
			{
				useCaseUI.OnButtonClickedEvent -= SetButtonsActive;
			}
		}

		private void SetButtonsActive(bool isActive)
		{
			foreach (var useCaseUI in _useCaseUIs)
			{
				useCaseUI.SelectButton.gameObject.SetActive(isActive);
			}
		}
	}
}