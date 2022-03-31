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
		private GameObject _useCase;

		public Button SelectButton => _selectButton;

		public Button BackButton => _backButton;

		public GameObject UseCase => _useCase;
	}
}