﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Saiive.SuperNode.Model.Requests
{
    public class AddressesBodyRequest
    {
        [JsonProperty("addresses")]
        public IList<string> Addresses { get; set; }
    }
}
