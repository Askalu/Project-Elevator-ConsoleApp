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
    public Task<Elevator> GetByIdAsync(int id);
    public Task Update(Elevator elevator);
}

internal class SqliteDatabaseService : IDatabaseService, IDisposable
{
    private const string DatabaseTable = "elevators";
    private const string ConnectionString = "Data Source=database.sqlite;pooling=false;";


    public async Task BootstrapDatabaseAsync()
    {
        await using var connection = new SqliteConnection(ConnectionString);
        try
        {
            var table = (await connection.QueryAsync<string>(
                    $"SELECT name FROM sqlite_master WHERE type='table' AND name='{DatabaseTable}'"))
                .FirstOrDefault();
            if (!string.IsNullOrEmpty(table) && table == DatabaseTable)
            {
                return;
            }

            const string sqlQuery =
                $"CREATE TABLE {DatabaseTable} (Id INTEGER PRIMARY KEY,ElevatorId VARCHAR(300) NULL, Location VARCHAR(300) NOT NULL, NumberOfFloors INTEGER NULL, ConnectionString VARCHAR(300) NULL)";

            await connection.ExecuteAsync(sqlQuery);
        }
        catch (Exception)
        {
            throw new Exception("could not bootstrap db");
        }
        finally
        {
            await connection.CloseAsync();
            await connection.DisposeAsync();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }


    public async Task<bool> InsertAsync(Elevator elevator)
    {
        await using var connection = new SqliteConnection(ConnectionString);

        try
        {
            var table = (await connection.QueryAsync<string>(
                    $"SELECT name FROM sqlite_master WHERE type='table' AND name='{DatabaseTable}'"))
                .FirstOrDefault();
            if (string.IsNullOrEmpty(table) || table != DatabaseTable)
                await BootstrapDatabaseAsync();


            await connection.InsertAsync(elevator);

            return true;
        }
        catch (Exception)
        {
            throw new Exception("could not insert");
        }
        finally
        {
            await connection.CloseAsync();
            await connection.DisposeAsync();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    public async Task<List<Elevator>?> GetAllAsync()
    {
        await using var connection = new SqliteConnection(ConnectionString);

        try
        {
            var table = (await connection.QueryAsync<string>(
                    $"SELECT name FROM sqlite_master WHERE type='table' AND name='{DatabaseTable}'"))
                .FirstOrDefault();
            if (string.IsNullOrEmpty(table) || table != DatabaseTable)
                await BootstrapDatabaseAsync();

            var elevators = await connection.GetAllAsync<Elevator>();

            return elevators.ToList();
        }
        catch (Exception e)
        {
            var hmm = e;
            throw new Exception("could not load all");
        }
        finally
        {
            await connection.CloseAsync();
            await connection.DisposeAsync();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    public async Task<Elevator> GetByIdAsync(int id)
    {
        await using var connection = new SqliteConnection(ConnectionString);

        try
        {
            var table = (await connection.QueryAsync<string>(
                $"SELECT name FROM sqlite_master WHERE type='table' AND name='{DatabaseTable}'")).FirstOrDefault();

            if (string.IsNullOrEmpty(table) || table != DatabaseTable)
                await BootstrapDatabaseAsync();

            var elevator = await connection.GetAsync<Elevator>(id);

            return elevator;
        }
        catch (Exception)
        {
            throw new Exception();
        }
        finally
        {
            await connection.CloseAsync();
            await connection.DisposeAsync();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    public async Task Update(Elevator elevator)
    {
        await using var connection = new SqliteConnection(ConnectionString);
        try
        {
            var table = (await connection.QueryAsync<string>(
                $"SELECT name FROM sqlite_master WHERE type='table' AND name='{DatabaseTable}'")).FirstOrDefault();

            if (string.IsNullOrEmpty(table) || table != DatabaseTable)
                await BootstrapDatabaseAsync();

            var elevatorToUpdate = await connection.GetAsync<Elevator>(elevator.Id);

            if (elevatorToUpdate is null)
            {
                throw new ArgumentException();
            }

            elevatorToUpdate.ConnectionString = elevator.ConnectionString;
            elevatorToUpdate.Location = elevator.Location;
            elevatorToUpdate.ElevatorId = elevator.ElevatorId;
            elevatorToUpdate.NumberOfFloors = elevator.NumberOfFloors;

            await connection.UpdateAsync(elevatorToUpdate);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
            await connection.DisposeAsync();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    public void Dispose()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}