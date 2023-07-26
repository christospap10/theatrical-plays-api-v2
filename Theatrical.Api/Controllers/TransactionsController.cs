﻿using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Models;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Dto.TransactionDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionRepository _repo;

    public TransactionsController(ITransactionRepository repository)
    {
        _repo = repository;
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> PostTransaction([FromBody]TransactionDto transactionDto)
    {
        try
        {
            var transaction = new Transaction
            {
                UserId = transactionDto.UserId,
                CreditAmount = transactionDto.CreditAmount,
                Reason = transactionDto.Reason
            };

            await _repo.PostTransaction(transaction);
            
            var response = new ApiResponse("Transaction Successful!");
            
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var exceptionResponse = new ApiResponse(ErrorCode.ServerError, e.InnerException.Message);
            return new ObjectResult(exceptionResponse) { StatusCode = 500 };
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetTransaction([FromRoute] int id)
    {
        try
        {
            var transaction = await _repo.GetTransaction(id);
            var response = new ApiResponse<Transaction>(transaction);
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var exceptionResponse = new ApiResponse(ErrorCode.ServerError, e.InnerException.Message);
            return new ObjectResult(exceptionResponse) { StatusCode = 500 };
        }
    }

    [HttpGet("user/{id}")]
    public async Task<ActionResult<ApiResponse>> GetUserTransactions([FromRoute] int id)
    {
        try
        {
            var transactions = await _repo.GetTransactions(id);
            return new OkObjectResult(new ApiResponse<List<Transaction>>(transactions));
        }
        catch (Exception e)
        {
            var exceptionResponse = new ApiResponse(ErrorCode.ServerError, e.InnerException.Message);
            return new ObjectResult(exceptionResponse) { StatusCode = 500 };
        }
    }
}