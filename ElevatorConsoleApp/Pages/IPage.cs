namespace ElevatorConsoleApp.Pages;
internal interface IPage
{
    public Task DoWork(CancellationToken cancellationToken);
}
