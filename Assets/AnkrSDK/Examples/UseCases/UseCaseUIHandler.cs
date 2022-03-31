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
				useCaseUI._selectButton.gameObject.SetActive(isActive);
			}
		}

		private void SetButtonLinks()
		{
			foreach (var useCaseUI in _useCaseUIs)
			{
				useCaseUI._selectButton.onClick.AddListener(() => OnSelectButtonClicked(useCaseUI));
				useCaseUI._backButton.onClick.AddListener(() => OnBackButtonClicked(useCaseUI));
			}
		}

		private void OnBackButtonClicked(UseCaseUI useCaseUI)
		{
			SetButtonsActive(true);
			useCaseUI._useCase.SetActive(false);
		}

		private void OnSelectButtonClicked(UseCaseUI useCaseUI)
		{
			SetButtonsActive(false);
			useCaseUI._useCase.SetActive(true);
		}
	}
}