using System;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UseCases
{
	public class UseCaseUI : MonoBehaviour
	{
		[SerializeField]
		private Button _selectButton;

		[SerializeField]
		private Button _backButton;

		[SerializeField]
		private UseCase _useCase;

		public Button SelectButton => _selectButton;

		public Button BackButton => _backButton;

		public UseCase UseCase => _useCase;
		
		public event Action<bool> OnButtonClickedEvent;

		private void Awake()
		{
			SubscribeButtonLinks();
		}

		private void OnDestroy()
		{
			UnsubscribeButtonLinks();
		}

		private void SubscribeButtonLinks()
		{
			SelectButton.onClick.AddListener(OnSelectButtonClicked);
			BackButton.onClick.AddListener(OnBackButtonClicked);
		}

		private void UnsubscribeButtonLinks()
		{
			SelectButton.onClick.RemoveListener(OnSelectButtonClicked);
			BackButton.onClick.RemoveListener(OnBackButtonClicked);
		}

		private void OnBackButtonClicked()
		{
			OnButtonClickedEvent?.Invoke(true);
			UseCase.DeActivateUseCase();
		}

		private void OnSelectButtonClicked()
		{
			OnButtonClickedEvent?.Invoke(false);
			UseCase.ActivateUseCase();
		}
	}
}