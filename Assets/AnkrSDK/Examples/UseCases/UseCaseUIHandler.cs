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
			SetButtonLinks();
		}

		private void OnDestroy()
		{
			UnsubscribeButtonLinks();
		}

		private void SetButtonsActive(bool isActive)
		{
			foreach (var useCaseUI in _useCaseUIs)
			{
				useCaseUI.SelectButton.gameObject.SetActive(isActive);
			}
		}

		private void SetButtonLinks()
		{
			foreach (var useCaseUI in _useCaseUIs)
			{
				useCaseUI.SelectButton.onClick.AddListener(() => OnSelectButtonClicked(useCaseUI));
				useCaseUI.BackButton.onClick.AddListener(() => OnBackButtonClicked(useCaseUI));
			}
		}
		
		private void UnsubscribeButtonLinks()
		{
			foreach (var useCaseUI in _useCaseUIs)
			{
				useCaseUI.SelectButton.onClick.RemoveAllListeners();
				useCaseUI.BackButton.onClick.RemoveAllListeners();
			}
		}

		private void OnBackButtonClicked(UseCaseUI useCaseUI)
		{
			SetButtonsActive(true);
			useCaseUI.UseCase.SetActive(false);
		}

		private void OnSelectButtonClicked(UseCaseUI useCaseUI)
		{
			SetButtonsActive(false);
			useCaseUI.UseCase.SetActive(true);
		}
	}
}