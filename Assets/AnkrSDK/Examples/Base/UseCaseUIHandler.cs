using System.Collections.Generic;
using UnityEngine;

namespace AnkrSDK.Base
{
	public class UseCaseUIHandler : MonoBehaviour, IUseCaseUIController
	{
		[SerializeField] private List<UseCaseUI> _useCaseUIs;

		private void Awake()
		{
			foreach (var useCaseUi in _useCaseUIs)
			{
				useCaseUi.Setup(this);
			}
		}

		public void SetUseCaseButtonsActive(bool active)
		{
			_useCaseUIs.ForEach(item => item.SelectButton.gameObject.SetActive(active));
		}
	}
}