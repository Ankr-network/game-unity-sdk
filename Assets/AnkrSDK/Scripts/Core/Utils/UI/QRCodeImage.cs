using QRCoder;
using QRCoder.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.Core.Utils.UI
{
	public class QRCodeImage : MonoBehaviour
	{
		[SerializeField] private Image _qrImage;

		public void UpdateQRCode(string url)
		{
			var qrGenerator = new QRCodeGenerator();
			var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
			var qrCode = new UnityQRCode(qrCodeData);
			var qrCodeAsTexture2D = qrCode.GetGraphic(20);

			var qrCodeSprite = Sprite.Create(qrCodeAsTexture2D,
				new Rect(0, 0, qrCodeAsTexture2D.width, qrCodeAsTexture2D.height),
				new Vector2(0.5f, 0.5f), 100f);
			qrCodeSprite.name = "ConnectionURL_QR";
			_qrImage.sprite = qrCodeSprite;
		}

		public void SetImageActive(bool isActive)
		{
			gameObject.SetActive(isActive);
		}
	}
}