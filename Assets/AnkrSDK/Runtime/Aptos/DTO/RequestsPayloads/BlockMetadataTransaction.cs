using AnkrSDK.Aptos.Converters;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class BlockMetadataTransaction : TypedTransaction
	{
		[JsonProperty(PropertyName = "version")]
		public string Version;
		[JsonProperty(PropertyName = "hash")]
		public string Hash;
		[JsonProperty(PropertyName = "state_change_hash")]
		public string StateChangeHash;
		[JsonProperty(PropertyName = "event_root_hash")]
		public string EventRootHash;
		[JsonProperty(PropertyName = "state_checkpoint_hash")]
		public string StateCheckpointHash;
		[JsonProperty(PropertyName = "gas_used")]
		public string GasUsed;
		[JsonProperty(PropertyName = "success")]
		public bool Success;
		[JsonProperty(PropertyName = "vm_status")]
		public string VmStatus;
		[JsonProperty(PropertyName = "accumulator_root_hash")]
		public string AccumulatorRootHash;
		[JsonProperty(PropertyName = "changes")]
		public WriteSetChange[] Changes;
		[JsonProperty(PropertyName = "id")]
		public string ID;
		[JsonProperty(PropertyName = "epoch")]
		public string Epoch;
		[JsonProperty(PropertyName = "round")]
		public string Round;
		[JsonProperty(PropertyName = "events")]
		public Event[] GasUnitPrice;
		[JsonProperty(PropertyName = "previous_block_votes_bitvec")]
		public int[] PreviousBlockVotesBitvec;
		[JsonProperty(PropertyName = "proposer")]
		public string Proposer;
		[JsonProperty(PropertyName = "failed_proposer_indices")]
		public int[] FailedProposerIndices;
		[JsonProperty(PropertyName = "timestamp")]
		public string Timestamp;
	}
}