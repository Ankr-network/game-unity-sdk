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

		public Button GetSelectButton()
		{
			return _selectButton;
		}
		
		public Button GetBackButton()
		{
			return _backButton;
		}
		
		public GameObject GetUseCase()
		{
			return _useCase;
		}
	}
}