﻿namespace Theatrical.Dto.TransactionDtos;

public class TransactionDtoFetch
{
    public int UserId { get; set; }
    public decimal CreditAmount { get; set; }
    public string Reason { get; set; }
    public DateTime DateCreated { get; set; }
    public string TransactionId { get; set; }
    public string AuthCode { get; set; }
    public string AccountNumber { get; set; }
    public string AccountType { get; set; }
}