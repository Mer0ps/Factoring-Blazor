using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Common.Interfaces;
public interface IFactoringContext
{
    public DbSet<ScAdmin> ScAdmins { get; }
    public DbSet<WhitelistedToken> WhitelistedTokens { get; }
    public DbSet<Account> Accounts { get; }
    public DbSet<Contract> Contracts { get; }
    public DbSet<Administrator> Administrators { get; }
    public DbSet<JobExecution> JobExecutions { get; }
    public DbSet<Invoice> Invoices { get; }
    public DbSet<CollectedFee> CollectedFees { get; }
    public DbSet<InvoiceHistory> InvoiceHistories { get; }
    public DbSet<LegalForm> LegalForms { get; }
    public DbSet<BusinessActivity> BusinessActivities { get; }
    public DbSet<Country> Countries { get; }

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
