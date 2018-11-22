# AsyncAwaitBestPractices

Extensions for `System.Threading.Tasks.Task`:
- `AsyncAwaitBestPractices` 
  - Contains an `SafeFireAndForget`, an extension method to safely fire-and-forget a `Task`
  - [Usage instructions below](#AsyncAwaitBestPractices)
- `AsyncAwaitBestPractices.MVVM`
  - Conatains `AsyncCommand` and `IAsyncCommand : ICommand` to safely be used asynchronously with `Task`
  - [Usage instructions below](#AsyncAwaitBestPractices.MVVM)

# Setup

* Available on NuGet: https://www.nuget.org/packages/AsyncAwaitBestPractices/ [![NuGet](https://img.shields.io/nuget/v/AsyncAwaitBestPractices.svg?label=NuGet)](https://www.nuget.org/packages/AsyncAwaitBestPractices/)

* Available on NuGet: https://www.nuget.org/packages/AsyncAwaitBestPractices.MVVM/ [![NuGet](https://img.shields.io/nuget/v/AsyncAwaitBestPractices.MVVM.svg?label=NuGet)](https://www.nuget.org/packages/AsyncAwaitBestPractices.MVVM/)

# Usage

## AsyncAwaitBestPractices
An extension method to safely fire-and-forget a Task

```csharp
void HandleButtonTapped(object sender, EventArgs e)
{
    // Allows the async Task method to safely run on a different thread while not awaiting its completion
    ExampleAsyncMethod().SafeFireAndForget();
    
    // The code continues here while `ExampleAsyncMethod()` is running on a different thread
    // ...
}

async Task ExampleAsyncMethod()
{
    await Task.Delay(1000);
}
```

## AsyncAwaitBestPractices.MVVM
Includes AsyncCommand and IAsyncCommand which allows ICommand to safely be used asynchronously with Task

```csharp
public class ExampleClass
{
    public ExampleClass()
    {
        ExampleAsyncCommand = new AsyncCommand(ExampleAsyncMethod);
        ExampleAsyncIntCommand = new AsyncCommand<int>(ExampleAsyncMethodWithIntParameter);
        ExampleAsyncExceptionCommand = new AsyncCommand(ExampleAsyncMethodWithException, onException: ex => Console.WriteLine(ex.Message));
        ExampleAsyncCommandNotReturningToTheCallingThread = new AsyncCommand(ExampleAsyncMethod, continueOnCapturedContext:false);
    }
    
    public IAsyncCommand ExampleAsyncCommand { get; }
    public IAsyncCommand ExampleAsyncIntCommand { get; }
    public IAsyncCommand ExampleAsyncExceptionCommand { get; }
    public IAsyncCommand ExampleAsyncCommandNotReturningToTheCallingThread { get; }

    async Task ExampleAsyncMethod()
    {
        await Task.Delay(1000);
    }
  
    async Task ExampleAsyncMethodWithIntParameter(int parameter)
    {
        await Task.Delay(parameter);
    }
    
    async Task ExampleAsyncMethodWithException()
    {
        await Task.Delay(1000);
        throw new Exception();
    }
    
    void ExecuteCommands()
    {
        ExampleAsyncCommand.Execute(null);
        ExampleAsyncIntCommand.Execute(1000);
        ExampleAsyncExceptionCommand.Execute(null);
        ExampleAsyncCommandNotReturningToTheCallingThread.Execute(null);
    }
}
```
