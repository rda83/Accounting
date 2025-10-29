using Accounting.Api.Data;
using Accounting.Api.DTOs.TestDataService;
using Accounting.Api.Models;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Api.Services
{
    public class TestDataService
    {
        private readonly AppDbContext _context;

        public TestDataService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateTestClients(CreateTestClientsRequestDto request)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                await CreateCurrenciesIfNotExist();

                var currencies = await _context.Currencies.ToListAsync();

                var businessClientFaker = new Faker<Client>("ru")
                    .RuleFor(c => c.Name, f => f.Company.CompanyName())
                    .RuleFor(c => c.Email, f => f.Internet.Email(provider: "corporation.com"));
                var businessClients = businessClientFaker.Generate(request.Count);
                await _context.Clients.AddRangeAsync(businessClients);
                await _context.SaveChangesAsync();

                var accounts = new List<Account>();

                foreach (var item in businessClients)
                {
                    var accountFaker = new Faker<Account>("ru")
                        .RuleFor(a => a.Number, f => GenerateRealisticAccountNumber(f))
                        .RuleFor(a => a.Balance, f => f.Finance.Amount(100, 100000, 2))
                        .RuleFor(a => a.CurrencyId, f => f.PickRandom(currencies).Id)
                        .RuleFor(a => a.ClientId, (f, a) => item.Id);

                    var account = accountFaker.Generate(1);
                    accounts.AddRange(account);
                }

                await _context.Accounts.AddRangeAsync(accounts);
                await _context.SaveChangesAsync();

                transaction.Commit();
            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }

        private string GenerateRealisticAccountNumber(Faker f)
        {
            return $"40702{f.Random.Int(1000, 9999)}{f.Random.Int(1000, 9999)}{f.Random.Int(1000, 9999)}{f.Random.Int(1000, 9999)}";
        }
        private async Task CreateCurrenciesIfNotExist()
        {
            if (!await _context.Currencies.AnyAsync())
            {
                var currencies = new[]
                {
                new Currency { Code = "USD", Name = "US Dollar" },
                new Currency { Code = "EUR", Name = "Euro" },
                new Currency { Code = "RUB", Name = "Russian Ruble" },
                new Currency { Code = "GBP", Name = "British Pound" },
                new Currency { Code = "JPY", Name = "Japanese Yen" }
            };

                await _context.Currencies.AddRangeAsync(currencies);
                await _context.SaveChangesAsync();
            }
        }
    }
}
