using System;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	[Serializable]
	public class IndexResponse
	{
		[JsonProperty(PropertyName = "chain_id")]
		public uint ChainID;
		[JsonProperty(PropertyName = "epoch")]
		public uint Epoch;
		[JsonProperty(PropertyName = "ledger_version")]
		public uint LedgerVersion;
		[JsonProperty(PropertyName = "oldest_ledger_version")]
		public uint OldestLedgerVersion;
		[JsonProperty(PropertyName = "ledger_timestamp")]
		public ulong LedgerTimestamp;
		[JsonProperty(PropertyName = "node_role")]
		public string NodeRole;
		[JsonProperty(PropertyName = "oldest_block_height")]
		public uint OldestBlockHeight;
		[JsonProperty(PropertyName = "block_height")]
		public uint BlockHeight;
		[JsonProperty(PropertyName = "git_hash")]
		public string GitHash;
	}
}