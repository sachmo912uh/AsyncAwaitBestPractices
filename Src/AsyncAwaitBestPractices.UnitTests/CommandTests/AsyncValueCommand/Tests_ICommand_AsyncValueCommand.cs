﻿using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_ICommand_AsyncValueCommand : BaseAsyncValueCommandTest
{
	[TestCase(500)]
	[TestCase(default)]
	public async Task ICommand_Execute_IntParameter_Test(int parameter)
	{
		//Arrange
		ICommand command = new AsyncValueCommand<int>(IntParameterTask);
		ICommand command2 = new AsyncValueCommand<int, int>(IntParameterTask);

		//Act
		command.Execute(parameter);
		await IntParameterTask(parameter);

		command2.Execute(parameter);
		await IntParameterTask(parameter);

		//Assert

	}

	[TestCase("Hello")]
	[TestCase(default)]
	public async Task ICommand_Execute_StringParameter_Test(string parameter)
	{
		//Arrange
		ICommand command = new AsyncValueCommand<string>(StringParameterTask);
		ICommand command2 = new AsyncValueCommand<string, string>(StringParameterTask);

		//Act
		command.Execute(parameter);
		await StringParameterTask(parameter);

		command2.Execute(parameter);
		await StringParameterTask(parameter);

		//Assert

	}

	[Test]
	public void ICommand_TwoParameters_ExecuteAsync_InvalidValueTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(string), typeof(int));

		ICommand command = new AsyncValueCommand<string, string>(StringParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute(Delay));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_ExecuteAsync_InvalidValueTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(string), typeof(int));

		ICommand command = new AsyncValueCommand<string>(StringParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute(Delay));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_TwoParameters_ExecuteAsync_InvalidReferenceTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int), typeof(string));


		ICommand command = new AsyncValueCommand<int, int>(IntParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute("Hello World"));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_ExecuteAsync_InvalidReferenceTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int), typeof(string));


		ICommand command = new AsyncValueCommand<int>(IntParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute("Hello World"));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_TwoParameters_ExecuteAsync_ValueTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int));


		ICommand command = new AsyncValueCommand<int, int>(IntParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute(null));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_ExecuteAsync_ValueTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int));


		ICommand command = new AsyncValueCommand<int>(IntParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute(null));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_Parameter_CanExecuteNull_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int));

		ICommand command = new AsyncValueCommand<int, int>(IntParameterTask, CanExecuteFalse);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.CanExecute(null));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_Parameter_ExecuteNull_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int));

		ICommand command = new AsyncValueCommand<int, int>(IntParameterTask, CanExecuteFalse);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute(null));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_Parameter_CanExecuteTrue_Test()
	{
		//Arrange
		ICommand command = new AsyncValueCommand<int?>(NullableIntParameterTask, CanExecuteTrue);
		ICommand command2 = new AsyncValueCommand<int, int>(IntParameterTask, CanExecuteTrue);

		//Act

		//Assert
		Assert.True(command.CanExecute(null));
		Assert.True(command2.CanExecute(0));
	}

	[Test]
	public void ICommand_Parameter_CanExecuteFalse_Test()
	{
		//Arrange
		ICommand command = new AsyncValueCommand<int?>(NullableIntParameterTask, CanExecuteFalse);
		ICommand command2 = new AsyncValueCommand<int, int>(IntParameterTask, CanExecuteFalse);

		//Act

		//Assert
		Assert.False(command.CanExecute(null));
		Assert.False(command2.CanExecute(0));
	}

	[Test]
	public void ICommand_NoParameter_CanExecuteFalse_Test()
	{
		//Arrange
		ICommand command = new AsyncValueCommand(NoParameterTask, CanExecuteFalse);

		//Act

		//Assert
		Assert.False(command.CanExecute(null));
	}

	[Test]
	public void ICommand_Parameter_CanExecuteDynamic_Test()
	{
		//Arrange
		ICommand command = new AsyncValueCommand<int>(IntParameterTask, CanExecuteDynamic);

		//Act

		//Assert
		Assert.True(command.CanExecute(true));
		Assert.False(command.CanExecute(false));
	}

	[Test]
	public void ICommand_Parameter_CanExecuteChanged_Test()
	{
		//Arrange
		ICommand command = new AsyncValueCommand<int>(IntParameterTask, CanExecuteDynamic);

		//Act

		//Assert
		Assert.True(command.CanExecute(true));
		Assert.False(command.CanExecute(false));
	}
}