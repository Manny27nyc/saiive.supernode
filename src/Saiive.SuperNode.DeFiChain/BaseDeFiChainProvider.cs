﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction;
using System;
using System.Net.Http;

namespace Saiive.SuperNode.DeFiChain
{
    internal class BaseDeFiChainProvider : BaseProvider
    {
        protected readonly ILogger Logger;
        protected readonly string OceanUrl;
        protected readonly string DefiChainApiUrl;
        protected readonly string CoingeckoApiUrl;

        protected readonly HttpClient _client;

        public BaseDeFiChainProvider(ILogger logger, IConfiguration config) : base(logger, config)
        {
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromMinutes(5);

            Logger = logger;
            OceanUrl = config["OCEAN_URL"];
            DefiChainApiUrl = config["DEFI_CHAIN_API_URL"];
            CoingeckoApiUrl = config["COINGECKO_API_URL"];

            Logger.LogTrace($"Using ocean {OceanUrl}");
            Logger.LogTrace($"Using DefiChainApi {DefiChainApiUrl}");
            Logger.LogTrace($"Using CoingeckoApi {CoingeckoApiUrl}");
        }
    }
}