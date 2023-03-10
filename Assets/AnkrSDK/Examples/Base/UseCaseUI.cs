using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.Base
{
	public class UseCaseUI : MonoBehaviour
	{
		[SerializeField] private Button _selectButton;
		[SerializeField] private Button _backButton;
		[SerializeField] private UseCaseBodyUI _useCase;

		public Button SelectButton => _selectButton;

		public Button BackButton => _backButton;
		public UseCaseBodyUI UseCase => _useCase;

		private IUseCaseUIController _uiController;

		public void Setup(IUseCaseUIController uiController)
		{
			_uiController = uiController;
		}

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
			UseCase.SetUseCaseBodyActive(false);
			_uiController.SetUseCaseButtonsActive(true);
		}

		private void OnSelectButtonClicked()
		{
			UseCase.SetUseCaseBodyActive(true);
			_uiController.SetUseCaseButtonsActive(false);
		}
	}
}