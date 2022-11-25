using Dapper;
using Dapper.Contrib.Extensions;
using ElevatorConsoleApp.Models;
using Microsoft.Data.Sqlite;

namespace ElevatorConsoleApp.Services;
internal interface IDatabaseService
{
    public Task BootstrapDatabaseAsync();
    public Task<bool> InsertAsync(Elevator elevator);
    public Task<List<Elevator>?> GetAllAsync();
    public Task<Elevator> GetByIdAsync();
}

internal class SqliteDatabaseService : IDatabaseService
{
    private const string DatabaseTable = "elevators";


    public async Task BootstrapDatabaseAsync()
    {
        try
        {
            await using var connection = new SqliteConnection("Data Source=database.sqlite;");

            var table = (await connection.QueryAsync<string>(
                    $"SELECT name FROM sqlite_master WHERE type='table' AND name='{DatabaseTable}'"))
                .FirstOrDefault();
            if (!string.IsNullOrEmpty(table) && table == DatabaseTable)
            {
                return;
            }

            const string sqlQuery = $"CREATE TABLE {DatabaseTable} (Id INT NULL,ElevatorId VARCHAR(300) NULL, Location VARCHAR(300) NOT NULL, NumberOfFloors INT NULL, ConnectionString VARCHAR(300) NULL)";

            await connection.ExecuteAsync(sqlQuery);

        }
        catch (Exception)
        {
            throw new Exception("could not bootstrap db");
        }
    }

    public async Task<bool> InsertAsync(Elevator elevator)
    {
        try
        {
            await using var connection = new SqliteConnection("Data Source=database.sqlite;");

            var table = (await connection.QueryAsync<string>(
                    $"SELECT name FROM sqlite_master WHERE type='table' AND name='{DatabaseTable}'"))
                .FirstOrDefault();
            if (string.IsNullOrEmpty(table) || table != DatabaseTable)
            {
                return false;
            }

            await connection.InsertAsync(elevator);

            return true;
        }
        catch (Exception)
        {
            throw new Exception("could not insert");
        }
    }

    public async Task<List<Elevator>?> GetAllAsync()
    {
        try
        {
            await using var connection = new SqliteConnection("Data Source=database.sqlite;");

            var table = (await connection.QueryAsync<string>(
                    $"SELECT name FROM sqlite_master WHERE type='table' AND name='{DatabaseTable}'"))
                .FirstOrDefault();
            if (string.IsNullOrEmpty(table) || table != DatabaseTable)
            {
                return null;
            }

            var elevators = await connection.GetAllAsync<Elevator>();

            return elevators.ToList();
        }
        catch (Exception)
        {
            throw new Exception("could not load all");
        }
    }

    public Task<Elevator> GetByIdAsync()
    {
        throw new NotImplementedException();
    }
}