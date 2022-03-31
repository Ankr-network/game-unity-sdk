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

		private void SetButtonsActive(bool isActive)
		{
			foreach (var useCaseUI in _useCaseUIs)
			{
				useCaseUI.GetSelectButton().gameObject.SetActive(isActive);
			}
		}

		private void SetButtonLinks()
		{
			foreach (var useCaseUI in _useCaseUIs)
			{
				useCaseUI.GetSelectButton().onClick.AddListener(() => OnSelectButtonClicked(useCaseUI));
				useCaseUI.GetBackButton().onClick.AddListener(() => OnBackButtonClicked(useCaseUI));
			}
		}

		private void OnBackButtonClicked(UseCaseUI useCaseUI)
		{
			SetButtonsActive(true);
			useCaseUI.GetUseCase().SetActive(false);
		}

		private void OnSelectButtonClicked(UseCaseUI useCaseUI)
		{
			SetButtonsActive(false);
			useCaseUI.GetUseCase().SetActive(true);
		}
	}
}