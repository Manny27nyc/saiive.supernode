﻿using Newtonsoft.Json;
using saiive.defi.api.Converter;

namespace saiive.defi.api.Model
{
    public class BlockModel
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("chain")]
        public string Chain { get; set; }

        [JsonProperty("network")]
        public string Network { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("merkleRoot")]
        public string MerkleRoot { get; set; }

        [JsonProperty("time")]
        public string Time { get; set; }

        [JsonProperty("nonce")]
        public ulong Nonce { get; set; }

        [JsonProperty("bits")]
        public ulong Bits { get; set; }

        [JsonProperty("previousBlockHash")]
        public string PreviousBlockHash { get; set; }

        [JsonProperty("nextBlockHash")]
        public string NextBlockHash { get; set; }

        [JsonProperty("reward")]
        [JsonConverter(typeof(CoinValueConverter))]
        public double Reward { get; set; }

        [JsonProperty("transactionCount")]
        public int TransactionCount { get; set; }

        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }
    }
}
