#if NET8_0_OR_GREATER
using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_Task_SafeFireAndForget_ConfigureAwaitOptions : BaseTest
{
	[SetUp]
	public void BeforeEachTest()
	{
		SafeFireAndForgetExtensions.Initialize(false);
		SafeFireAndForgetExtensions.RemoveDefaultExceptionHandling();
	}

	[TearDown]
	public void AfterEachTest()
	{
		SafeFireAndForgetExtensions.Initialize(false);
		SafeFireAndForgetExtensions.RemoveDefaultExceptionHandling();
	}

	[Test]
	public async Task SafeFireAndForget_HandledException()
	{
		//Arrange
		Exception? exception = null;

		//Act
		NoParameterDelayedNullReferenceExceptionTask().SafeFireAndForget(ConfigureAwaitOptions.None, ex => exception = ex);
		await NoParameterTask();
		await NoParameterTask();

		//Assert
		Assert.IsNotNull(exception);
	}
	
	[Test]
	public async Task SafeFireAndForget_ConfigureAwaitOptionsSuppressedThrowing()
	{
		//Arrange
		Exception? exception = null;

		//Act
		NoParameterDelayedNullReferenceExceptionTask().SafeFireAndForget(ConfigureAwaitOptions.SuppressThrowing, ex => exception = ex);
		await NoParameterTask();
		await NoParameterTask();

		//Assert
		Assert.IsNull(exception);
	}

	[Test]
	public async Task SafeFireAndForget_SetDefaultExceptionHandling_NoParams()
	{
		//Arrange
		Exception? exception = null;
		SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex => exception = ex);

		//Act
		NoParameterDelayedNullReferenceExceptionTask().SafeFireAndForget(ConfigureAwaitOptions.None, null);
		await NoParameterTask();
		await NoParameterTask();

		//Assert
		Assert.IsNotNull(exception);
	}
	
	[Test]
	public async Task SafeFireAndForget_SetDefaultExceptionHandling_ConfigureAwaitOptionsSuppressThrowing()
	{
		//Arrange
		Exception? exception = null;
		SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex => exception = ex);

		//Act
		NoParameterDelayedNullReferenceExceptionTask().SafeFireAndForget(ConfigureAwaitOptions.SuppressThrowing, null);
		await NoParameterTask();
		await NoParameterTask();

		//Assert
		Assert.IsNull(exception);
	}

	[Test]
	public async Task SafeFireAndForget_SetDefaultExceptionHandling_WithParams()
	{
		//Arrange
		Exception? exception1 = null;
		Exception? exception2 = null;
		SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex => exception1 = ex);

		//Act
		NoParameterDelayedNullReferenceExceptionTask().SafeFireAndForget(ConfigureAwaitOptions.None, ex => exception2 = ex);
		await NoParameterTask();
		await NoParameterTask();

		//Assert
		Assert.IsNotNull(exception1);
		Assert.IsNotNull(exception2);
	}
	
	[Test]
	public async Task SafeFireAndForget_SetDefaultExceptionHandling_WithParams_ConfigureAwaitOptionsSuppressThrowing()
	{
		//Arrange
		Exception? exception1 = null;
		Exception? exception2 = null;
		SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex => exception1 = ex);

		//Act
		NoParameterDelayedNullReferenceExceptionTask().SafeFireAndForget(ConfigureAwaitOptions.SuppressThrowing, ex => exception2 = ex);
		await NoParameterTask();
		await NoParameterTask();

		//Assert
		Assert.IsNull(exception1);
		Assert.IsNull(exception2);
	}

	[Test]
	public async Task SafeFireAndForget_ThreadTest()
	{
		//Arrange
		Thread? initialThread, workingThread, finalThread;
		var threadTCS = new TaskCompletionSource<Thread>();

		//Act
		initialThread = Thread.CurrentThread;

		BlockingThreadMethod().SafeFireAndForget(ConfigureAwaitOptions.None);

		finalThread = Thread.CurrentThread;

		workingThread = await threadTCS.Task;

		//Assert
		Assert.IsNotNull(initialThread);
		Assert.IsNotNull(workingThread);
		Assert.IsNotNull(finalThread);

		Assert.AreEqual(initialThread, finalThread);
		Assert.AreNotEqual(initialThread, workingThread);
		Assert.AreNotEqual(finalThread, workingThread);

		async Task BlockingThreadMethod()
		{
			await Task.Delay(100);
			threadTCS.SetResult(Thread.CurrentThread);
		}
	}

	[Test]
	public async Task SafeFireAndForget_NonAsyncMethodThreadTest()
	{
		//Arrange
		Thread initialThread, workingThread, finalThread;
		var threadTCS = new TaskCompletionSource<Thread>();

		//Act
		initialThread = Thread.CurrentThread;

		NonAsyncMethod().SafeFireAndForget(ConfigureAwaitOptions.None);

		finalThread = Thread.CurrentThread;

		workingThread = await threadTCS.Task;

		//Assert
		Assert.IsNotNull(initialThread);
		Assert.IsNotNull(workingThread);
		Assert.IsNotNull(finalThread);

		Assert.AreEqual(initialThread, finalThread);
		Assert.AreEqual(initialThread, workingThread);
		Assert.AreEqual(finalThread, workingThread);

		Task NonAsyncMethod()
		{
			threadTCS.SetResult(Thread.CurrentThread);
			return Task.FromResult(true);
		}
	}
	
	[Test]
	public async Task SafeFireAndForget_NonAsyncMethodThreadTest_ConfigureAwaitOptionsForceYielding()
	{
		//Arrange
		Thread initialThread, workingThread, finalThread;
		var threadTCS = new TaskCompletionSource<Thread>();

		//Act
		initialThread = Thread.CurrentThread;

		NonAsyncMethod().SafeFireAndForget(ConfigureAwaitOptions.ForceYielding);

		finalThread = Thread.CurrentThread;

		workingThread = await threadTCS.Task;

		//Assert
		Assert.IsNotNull(initialThread);
		Assert.IsNotNull(workingThread);
		Assert.IsNotNull(finalThread);

		Assert.AreEqual(initialThread, finalThread);
		Assert.AreEqual(initialThread, workingThread);
		Assert.AreEqual(finalThread, workingThread);

		Task NonAsyncMethod()
		{
			threadTCS.SetResult(Thread.CurrentThread);
			return Task.FromResult(true);
		}
	}
}
#endif