using Accounting.Api.Data;
using Accounting.Api.DTOs.TransferService;
using Accounting.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Api.Services
{
    public class TransferService : ITransferService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TransferService> _logger;

        public TransferService(AppDbContext context, ILogger<TransferService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TransferResult> TransferAsync(TransferRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var accountsToLock = new[] { request.FromAccountNumber, request.ToAccountNumber }
                    .OrderBy(x => x).ToArray();

                var lockedAccounts = new List<Account>();
                foreach (var accountNumber in accountsToLock)
                {
                    var account = await LockAndGetAccount(accountNumber);
                    lockedAccounts.Add(account);
                }

                var fromAccount = lockedAccounts.First(a => a.Number == request.FromAccountNumber);
                var toAccount = lockedAccounts.First(a => a.Number == request.ToAccountNumber);

                if (fromAccount.Balance < request.Amount)
                {
                    return TransferResult.Failed("Недостаточно средств");
                }

                await ExecuteTransfer(fromAccount, toAccount, request.Amount, request.Description);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return TransferResult.Success();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return TransferResult.Failed($"Ошибка: {ex.Message}");
            }
        }

        private async Task<Account> LockAndGetAccount(string accountNumber)
        {
            return await _context.Accounts
                .FromSqlRaw(@"SELECT * FROM ""Accounts"" WHERE ""Number"" = {0} FOR UPDATE", accountNumber)
                .Include(a => a.Currency)
                .AsNoTracking()
                .FirstOrDefaultAsync()
                ?? throw new ArgumentException($"Счет {accountNumber} не найден");
        }

        private async Task ExecuteTransfer(Account fromAccount, Account toAccount, decimal amount, string description)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "UPDATE \"Accounts\" SET \"Balance\" = \"Balance\" - {0} WHERE \"Number\" = {1}",
                    amount, fromAccount.Number);

            await _context.Database.ExecuteSqlRawAsync(
                "UPDATE \"Accounts\" SET \"Balance\" = \"Balance\" + {0} WHERE \"Number\" = {1}",
                    amount, toAccount.Number);

            var transaction = new Transaction
            {
                CreatedAt = DateTime.UtcNow,
                Amount = amount,
                Description = description,
                FromAccountId = fromAccount.Id,
                ToAccountId = toAccount.Id,                
            };

            _context.Transactions.Add(transaction);
        }
    }
}
