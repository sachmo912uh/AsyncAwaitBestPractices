﻿using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests
{
    public class Tests_ICommand : BaseTest
    {
        [TestCase(500)]
        [TestCase(default)]
        public async Task ICommand_Execute_IntParameter_Test(int parameter)
        {
            //Arrange
            ICommand command = new AsyncCommand<int>(IntParameterTask);

            //Act
            command?.Execute(parameter);
            await NoParameterTask();

            //Assert

        }

        [TestCase("Hello")]
        [TestCase(default)]
        public async Task ICommand_Execute_StringParameter_Test(string parameter)
        {
            //Arrange
            ICommand command = new AsyncCommand<string>(StringParameterTask);

            //Act
            command?.Execute(parameter);
            await NoParameterTask();

            //Assert

        }

        [Test]
        public async Task ICommand_ExecuteAsync_InvalidValueTypeParameter_Test()
        {
            //Arrange
            InvalidCommandParameterException actualInvalidCommandParameterException = null;
            InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(string), typeof(int));

            ICommand command = new AsyncCommand<string>(StringParameterTask);

            //Act
            try
            {
                command?.Execute(Delay);
                await NoParameterTask();
                await NoParameterTask();
            }
            catch (InvalidCommandParameterException e)
            {
                actualInvalidCommandParameterException = e;
            }

            //Assert
            Assert.IsNotNull(actualInvalidCommandParameterException);
            Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException.Message);
        }

        [Test]
        public async Task ICommand_ExecuteAsync_InvalidReferenceTypeParameter_Test()
        {
            //Arrange
            InvalidCommandParameterException actualInvalidCommandParameterException = null;
            InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int), typeof(string));


            ICommand command = new AsyncCommand<int>(IntParameterTask);

            //Act
            try
            {
                command.Execute("Hello World");
                await NoParameterTask();
                await NoParameterTask();
            }
            catch (InvalidCommandParameterException e)
            {
                actualInvalidCommandParameterException = e;
            }

            //Assert
            Assert.IsNotNull(actualInvalidCommandParameterException);
            Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException.Message);
        }

        [Test]
        public void ICommand_Parameter_CanExecuteTrue_Test()
        {
            //Arrange
            ICommand command = new AsyncCommand<int>(IntParameterTask, canExecute: CanExecuteTrue);

            //Act

            //Assert
            Assert.IsTrue(command.CanExecute(null));
        }

        [Test]
        public void ICommand_Parameter_CanExecuteFalse_Test()
        {
            //Arrange
            ICommand command = new AsyncCommand<int>(IntParameterTask, canExecute: CanExecuteFalse);

            //Act

            //Assert
            Assert.False(command.CanExecute(null));
        }

        [Test]
        public void ICommand_NoParameter_CanExecuteTrue_Test()
        {
            //Arrange
            ICommand command = new AsyncCommand(NoParameterTask, canExecute: CanExecuteTrue);

            //Act

            //Assert
            Assert.IsTrue(command.CanExecute(null));
        }

        [Test]
        public void ICommand_NoParameter_CanExecuteFalse_Test()
        {
            //Arrange
            ICommand command = new AsyncCommand(NoParameterTask, canExecute: CanExecuteFalse);

            //Act

            //Assert
            Assert.False(command.CanExecute(null));
        }
    }
}
