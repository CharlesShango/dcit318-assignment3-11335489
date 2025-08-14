using System;
using System.Collections.Generic;

// Core models using records
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// Payment Behavior Interfaces
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// Concrete processor implementations
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing bank transfer: ${transaction.Amount} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing mobile money: ${transaction.Amount} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing crypto transaction: ${transaction.Amount} for {transaction.Category}");
    }
}

// Account hierarchy
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"Transaction applied. New balance: ${Balance}");
    }
}

public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance)
    {
    }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            base.ApplyTransaction(transaction);
        }
    }
}

// Main application
public class FinanceApp
{
    private List<Transaction> _transactions = new List<Transaction>();

    public void Run()
    {
        // i. Instantiate a SavingsAccount
        var savingsAccount = new SavingsAccount("SAV-12345", 1000m);
        Console.WriteLine($"Created savings account with balance: ${savingsAccount.Balance}");

        // ii. Create three Transaction records
        var transaction1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
        var transaction2 = new Transaction(2, DateTime.Now, 75m, "Utilities");
        var transaction3 = new Transaction(3, DateTime.Now, 200m, "Entertainment");

        // iii. Process each transaction with different processors
        var mobileProcessor = new MobileMoneyProcessor();
        var bankProcessor = new BankTransferProcessor();
        var cryptoProcessor = new CryptoWalletProcessor();

        mobileProcessor.Process(transaction1);
        bankProcessor.Process(transaction2);
        cryptoProcessor.Process(transaction3);

        // iv. Apply transactions to account
        Console.WriteLine("\nApplying transactions to account:");
        savingsAccount.ApplyTransaction(transaction1);
        savingsAccount.ApplyTransaction(transaction2);
        savingsAccount.ApplyTransaction(transaction3);

        // v. Add transactions to list
        _transactions.Add(transaction1);
        _transactions.Add(transaction2);
        _transactions.Add(transaction3);

        Console.WriteLine($"\nTotal transactions recorded: {_transactions.Count}");
    }

    public static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}