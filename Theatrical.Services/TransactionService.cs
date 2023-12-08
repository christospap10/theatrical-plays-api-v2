﻿using Theatrical.Data.Models;
using Theatrical.Dto.TransactionDtos;
using Theatrical.Dto.TransactionDtos.PurchaseDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface ITransactionService
{
    TransactionDtoFetch TransactionToDto(Transaction transaction);
    TransactionResponseDto TransactionToResponseDto(Transaction transcation);
    List<TransactionDtoFetch> TransactionListToDto(List<Transaction> transactions);
    Task<Transaction> PostTransaction(User user);
}

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;

    public TransactionService(ITransactionRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Transaction> PostTransaction(User user)
    {
        var transaction = new Transaction
        {
            UserId = user.Id,
            CreditAmount = 5,
            Reason = "Credit Purchase",
        };
        
        var newTransaction = await _repository.PostTransaction(transaction);
        return newTransaction;
    }
    
    public TransactionDtoFetch TransactionToDto(Transaction transaction)
    {

        var transactionDtoFetch = new TransactionDtoFetch
        {
            CreditAmount = transaction.CreditAmount,
            Reason = transaction.Reason,
            UserId = transaction.UserId,
            DateCreated = transaction.DateCreated,
        };

        return transactionDtoFetch;
    }

    public TransactionResponseDto TransactionToResponseDto(Transaction transaction)
    {
        var transactionResponseDto = new TransactionResponseDto
        {
            CreditAmount = transaction.CreditAmount,
            Reason = transaction.Reason,
            UserId = transaction.UserId,
            DateCreated = transaction.DateCreated,
            DatabaseTransactionId = transaction.Id
        };

        return transactionResponseDto;
    }

    public List<TransactionDtoFetch> TransactionListToDto(List<Transaction> transactions)
    {
        List<TransactionDtoFetch> transactionDtoFetches = 
            transactions.Select(transaction => new TransactionDtoFetch 
                { CreditAmount = transaction.CreditAmount, 
                    Reason = transaction.Reason, 
                    UserId = transaction.UserId, 
                    DateCreated = transaction.DateCreated,
                }).ToList();
        
        return transactionDtoFetches;
    }
}

