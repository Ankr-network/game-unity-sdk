using System.Collections.Generic;
using System.Linq;

namespace MirageSDK.WalletConnectSharp.Core.Models
{
	public class SavedSession
	{
		public string ClientID { get; }
		public string BridgeURL { get; }
		public string Key { get; }
		public byte[] KeyRaw { get; }
		public string PeerID { get; }
		public int NetworkID { get; }
		public string[] Accounts { get; }
		public int ChainID { get; }
		public ClientMeta DappMeta { get; }

		public ClientMeta WalletMeta { get; }

		public SavedSession(string clientID, string bridgeURL, string key, byte[] keyRaw,
			string peerID, int networkID, string[] accounts, int chainID, ClientMeta dappMeta, ClientMeta walletMeta)
		{
			ClientID = clientID;
			BridgeURL = bridgeURL;
			Key = key;
			KeyRaw = keyRaw;
			PeerID = peerID;
			NetworkID = networkID;
			Accounts = accounts;
			ChainID = chainID;
			DappMeta = dappMeta;
			WalletMeta = walletMeta;
		}

		public bool Equals(SavedSession other)
		{
			return ClientID == other.ClientID
			       && BridgeURL == other.BridgeURL
			       && Key == other.Key
			       && PeerID == other.PeerID
			       && NetworkID == other.NetworkID
			       && ChainID == other.ChainID
			       && KeyRaw.SequenceEqual(other.KeyRaw)
			       && Accounts.SequenceEqual(other.Accounts)
			       && Equals(DappMeta, other.DappMeta)
			       && Equals(WalletMeta, other.WalletMeta);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SavedSession)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (ClientID != null ? ClientID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (BridgeURL != null ? BridgeURL.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Key != null ? Key.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (KeyRaw != null ? KeyRaw.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PeerID != null ? PeerID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ NetworkID;
				hashCode = (hashCode * 397) ^ (Accounts != null ? Accounts.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ ChainID;
				hashCode = (hashCode * 397) ^ (DappMeta != null ? DappMeta.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (WalletMeta != null ? WalletMeta.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(SavedSession session1, SavedSession session2)
		{
			var isFirstNull = ReferenceEquals(session1, null);
			var isSecondNull = ReferenceEquals(session2, null);

			switch (isFirstNull)
			{
				case true when isSecondNull:
					return true;
				case false when !isSecondNull:
					return session1.Equals(session2);
				default:
					return false;
			}
		}

		public static bool operator !=(SavedSession session, SavedSession other)
		{
			return !(session == other);
		}
	}
}