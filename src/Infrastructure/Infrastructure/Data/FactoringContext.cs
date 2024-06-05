using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;
public class FactoringContext : DbContext, IFactoringContext
{

    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public FactoringContext(DbContextOptions<FactoringContext> options, AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    public DbSet<ScAdmin> ScAdmins => Set<ScAdmin>();
    public DbSet<WhitelistedToken> WhitelistedTokens => Set<WhitelistedToken>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<Administrator> Administrators => Set<Administrator>();
    public DbSet<JobExecution> JobExecutions => Set<JobExecution>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceHistory> InvoiceHistories => Set<InvoiceHistory>();
    public DbSet<CollectedFee> CollectedFees => Set<CollectedFee>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<LegalForm> LegalForms => Set<LegalForm>();
    public DbSet<BusinessActivity> BusinessActivities => Set<BusinessActivity>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.EnableDetailedErrors();
#endif

        optionsBuilder
            .AddInterceptors(_auditableEntitySaveChangesInterceptor);

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityColumns();

        modelBuilder.Entity<Country>()
            .HasKey(x => x.Code);

        modelBuilder.Entity<Country>()
            .HasMany(e => e.LegalForms)
            .WithOne(e => e.Country)
            .HasForeignKey(e => e.CountryCode)
            .HasPrincipalKey(x => x.Code);

        modelBuilder.Entity<BusinessActivity>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<BusinessActivity>()
            .HasMany(e => e.Accounts)
            .WithOne(e => e.BusinessActivity)
            .HasForeignKey(e => e.BusinessActivityId)
            .HasPrincipalKey(x => x.Id);

        modelBuilder.Entity<LegalForm>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<LegalForm>()
            .HasMany(e => e.Accounts)
            .WithOne(e => e.LegalForm)
            .HasForeignKey(e => e.LegalFormId)
            .HasPrincipalKey(x => x.Id);

        modelBuilder.Entity<Invoice>()
            .HasKey(x => new { x.Id, x.ContractId });

        modelBuilder.Entity<Invoice>()
            .HasMany(e => e.InvoiceHistories)
            .WithOne(e => e.Invoice)
            .HasForeignKey(e => new { e.InvoiceId, e.ContractId })
            .HasPrincipalKey(x => new { x.Id, x.ContractId });

        modelBuilder.Entity<Invoice>()
            .HasMany(e => e.CollectedFees)
            .WithOne(e => e.Invoice)
            .HasForeignKey(e => new { e.InvoiceId, e.ContractId })
            .HasPrincipalKey(x => new { x.Id, x.ContractId });

        modelBuilder.Entity<InvoiceHistory>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<CollectedFee>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Account>()
            .HasMany(e => e.SupplierContracts)
            .WithOne(e => e.AccountSupplier)
            .HasForeignKey(e => e.AccountSupplierId)
            .HasPrincipalKey(e => e.Id);

        modelBuilder.Entity<Account>()
            .HasMany(e => e.ClientContracts)
            .WithOne(e => e.AccountClient)
            .HasForeignKey(e => e.AccountClientId)
            .HasPrincipalKey(e => e.Id);

        modelBuilder.Entity<Account>()
            .HasMany(e => e.Administrators)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.IdAccount)
            .HasPrincipalKey(e => e.Id);

        modelBuilder.Entity<Contract>()
            .HasMany(e => e.Invoices)
            .WithOne(e => e.Contract)
            .HasForeignKey(e => e.ContractId)
            .HasPrincipalKey(e => e.Id);

    }
}
