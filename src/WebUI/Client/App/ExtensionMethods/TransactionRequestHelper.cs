﻿using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Provider.Dtos.Common.Transactions;

namespace Mx.Blazor.DApp.Client.App.ExtensionMethods
{
    public static class TransactionRequestHelper
    {
        public static TransactionRequestDto GetTransactionRequestDecoded(this TransactionRequest transactionRequest)
        {
            var transaction = transactionRequest.GetTransactionRequest();
            transaction.Data = transaction.Data is null ? string.Empty : DataCoder.DecodeData(transaction.Data);

            return transaction;
        }

        public static TransactionRequestDto[] GetTransactionsRequestDecoded(this TransactionRequest[] transactionsRequest)
        {
            var transactions = new List<TransactionRequestDto>();
            foreach (var transactionRequest in transactionsRequest)
            {
                var transaction = transactionRequest.GetTransactionRequest();
                transaction.Data = transaction.Data is null ? string.Empty : DataCoder.DecodeData(transaction.Data);
                transactions.Add(transaction);
            }

            return transactions.ToArray();
        }
    }
}
