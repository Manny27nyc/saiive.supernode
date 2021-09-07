﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
{
    public class TransactionDetailModel
    {
        [JsonProperty("inputs")]
        public List<TransactionModel> Inputs { get; set; }
        
        [JsonProperty("outputs")]
        public List<TransactionModel> Outputs { get; set; }

    }
}
