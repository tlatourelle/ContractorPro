using ContractorPro.Application.Identity;
using ContractorPro.Infrastructure;
using ContractorPro.Infrastructure.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace ContractorPro.Application.Tests.Identity;

public sealed class ProvisioningServiceTests
{
    [Fact]
    public async Task ProvisionNewUser_CreatesContractorOwnerAndAuthIdentity()
    {
        await using var fixture = await TestDbFixture.CreateAsync();
        var service = new ProvisioningService(fixture.DbContext);

        var result = await service.ProvisionOrLoadAsync(new ProvisioningRequest(
            Provider: "google",
            ProviderSubject: "sub-new-1",
            Email: "ryan@example.com",
            DisplayName: "Ryan Rivers"));

        Assert.NotEqual(Guid.Empty, result.UserId);
        Assert.NotEqual(Guid.Empty, result.ContractorId);
        Assert.NotEqual(Guid.Empty, result.TeamMemberId);

        Assert.Equal(1, await fixture.DbContext.Users.CountAsync());
        Assert.Equal(1, await fixture.DbContext.Contractors.CountAsync());
        Assert.Equal(1, await fixture.DbContext.TeamMembers.CountAsync());
        Assert.Equal(1, await fixture.DbContext.AuthIdentities.CountAsync());

        var teamMember = await fixture.DbContext.TeamMembers.SingleAsync();
        Assert.True(teamMember.IsOwner);
        Assert.Equal("owner", teamMember.Role);
    }

    [Fact]
    public async Task ProvisionExistingUser_DoesNotCreateDuplicateContractor()
    {
        await using var fixture = await TestDbFixture.CreateAsync();
        var service = new ProvisioningService(fixture.DbContext);

        var first = await service.ProvisionOrLoadAsync(new ProvisioningRequest(
            Provider: "google",
            ProviderSubject: "sub-existing-1",
            Email: "same@example.com",
            DisplayName: "Same User"));

        var second = await service.ProvisionOrLoadAsync(new ProvisioningRequest(
            Provider: "google",
            ProviderSubject: "sub-existing-1",
            Email: "same@example.com",
            DisplayName: "Same User"));

        Assert.Equal(first.UserId, second.UserId);
        Assert.Equal(first.ContractorId, second.ContractorId);
        Assert.Equal(first.TeamMemberId, second.TeamMemberId);

        Assert.Equal(1, await fixture.DbContext.Users.CountAsync());
        Assert.Equal(1, await fixture.DbContext.Contractors.CountAsync());
        Assert.Equal(1, await fixture.DbContext.TeamMembers.CountAsync());
        Assert.Equal(1, await fixture.DbContext.AuthIdentities.CountAsync());
    }

    [Fact]
    public async Task ProvisionNewUser_RollsBackOnFailure()
    {
        await using var fixture = await TestDbFixture.CreateAsync(new ThrowOnSecondSaveChangesInterceptor());
        var service = new ProvisioningService(fixture.DbContext);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.ProvisionOrLoadAsync(new ProvisioningRequest(
            Provider: "google",
            ProviderSubject: "sub-fail-1",
            Email: "fail@example.com",
            DisplayName: "Rollback Candidate")));

        Assert.Equal(0, await fixture.DbContext.Users.CountAsync());
        Assert.Equal(0, await fixture.DbContext.Contractors.CountAsync());
        Assert.Equal(0, await fixture.DbContext.TeamMembers.CountAsync());
        Assert.Equal(0, await fixture.DbContext.AuthIdentities.CountAsync());
    }

    private sealed class ThrowOnSecondSaveChangesInterceptor : SaveChangesInterceptor
    {
        private int _saveCount;

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            _saveCount++;
            if (_saveCount == 2)
            {
                throw new InvalidOperationException("Simulated failure after first save.");
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }

    private sealed class TestDbFixture : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;

        public ContractorProDbContext DbContext { get; }

        private TestDbFixture(SqliteConnection connection, ContractorProDbContext dbContext)
        {
            _connection = connection;
            DbContext = dbContext;
        }

        public static async Task<TestDbFixture> CreateAsync(SaveChangesInterceptor? interceptor = null)
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();

            var optionsBuilder = new DbContextOptionsBuilder<ContractorProDbContext>()
                .UseSqlite(connection);

            if (interceptor is not null)
            {
                optionsBuilder.AddInterceptors(interceptor);
            }

            var dbContext = new ContractorProDbContext(optionsBuilder.Options);
            await dbContext.Database.EnsureCreatedAsync();

            return new TestDbFixture(connection, dbContext);
        }

        public async ValueTask DisposeAsync()
        {
            await DbContext.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
