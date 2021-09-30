﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.DeFiChain.Ocean;
using Saiive.SuperNode.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Providers
{
    internal class TransactionProvider : BaseDeFiChainProvider, ITransactionProvider
    {
        private readonly AddressProvider _addressProvider;
        private readonly BlockProvider _blockProvider;
        private readonly AddressTransactionDetailProvider _txProvider;

        public TransactionProvider(ILogger<TransactionProvider> logger, IConfiguration config, AddressProvider addressProvider, BlockProvider blockProvider, AddressTransactionDetailProvider txDetailProvider) : base(logger, config)
        {
            _addressProvider = addressProvider;
            _blockProvider = blockProvider;
            _txProvider = txDetailProvider;
        }

        private TransactionModel ConvertOceanModel(OceanDataEntity<OceanTransactionDetailData> data)
        {


            var tx = new TransactionModel
            {
                Id = data.Data.Id
            };
            return tx;


        }
        private async Task<TransactionDetailModel> GetLegacyTransactionDetails(string coin, string network, string txId)
        {
            try
            {
                var response = await _client.GetAsync($"{String.Format(LegacyBitcoreUrl, network)}/api/{coin}/{network}/tx/{txId}/coins");

                var data = await response.Content.ReadAsStringAsync();
                var tx = JsonConvert.DeserializeObject<TransactionDetailModel>(data);
                return tx;
            }
            catch
            {
                return null;
            }
        }


        public async Task<TransactionModel> GetTransactionById(string network, string txId)
        {
            var detailModel = await _addressProvider.GetTransactionDetails(network, txId);


            var response = await _client.GetAsync($"{OceanUrl}/v0/{network}/transactions/{txId}");

            try
            {

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<OceanDataEntity<OceanTransactionDetailData>>(data);
                var ret = ConvertOceanModel(obj);
                ret.Details = detailModel;


                return ret;
            }
            catch(Exception)
            {
                var responseLegacy = await _client.GetAsync($"{String.Format(LegacyBitcoreUrl, network)}/api/DFI/{network}/tx/{txId}");

                responseLegacy.EnsureSuccessStatusCode();

                var data = await responseLegacy.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<TransactionModel>(data);
                obj.Details = await GetTransactionDetails("DFI", network, txId);

                return obj;
            }
        }

        public async Task<IList<TransactionModel>> GetTransactionsByBlock(string network, string block)
        {

            var txs = await _blockProvider.GetTransactionForBlock(network, block);


            return txs;
        }

        public async Task<IList<BlockTransactionModel>> GetTransactionsByBlockHeight(string network, int height, bool includeDetails)
        {
            var blockInstance = await _blockProvider.GetBlockByHeightOrHash(network, height.ToString());

            var txs = await _blockProvider.GetTransactionForBlock(network, blockInstance.Hash);

            var ret = new List<BlockTransactionModel>();

            foreach(var tx in txs)
            {
                ret.Add(await _txProvider.GetBlockTransaction(network, tx.Id));
            }

            return ret;

        }

        public async Task<string> SendRawTransaction(string network, TransactionRequest request)
        {
            var body = new OceanRawTx
            {
                Hex = request.RawTx
            };
            var httpContent =
                 new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{OceanUrl}/v0/{network}/rawtx/send", httpContent);

            var data = await response.Content.ReadAsStringAsync();

            try
            {
                response.EnsureSuccessStatusCode();
                var obj = JsonConvert.DeserializeObject<OceanDataEntity<string>>(data);

                Logger.LogInformation("{coin}+{network}: Committed tx to blockchain, {txId} {txHex}", "DFI", network, obj?.Data, request.RawTx);

                return obj.Data;
            }
            catch
            {
                throw new ArgumentException(data);
            }

        }

        private async Task<TransactionDetailModel> GetTransactionDetails(string coin, string network, string txId)
        {
                return await _addressProvider.GetTransactionDetails(network, txId);
        }
    }
}
