﻿using System;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_WeakEventManager_Action : BaseTest
{
	readonly WeakEventManager _actionEventManager = new();

	event Action ActionEvent
	{
		add => _actionEventManager.AddEventHandler(value);
		remove => _actionEventManager.RemoveEventHandler(value);
	}

	[Test]
	public void WeakEventManagerAction_HandleEvent_ValidImplementation()
	{
		//Arrange
		ActionEvent += HandleDelegateTest;
		bool didEventFire = false;

		void HandleDelegateTest()
		{
			didEventFire = true;
			ActionEvent -= HandleDelegateTest;
		}

		//Act
		_actionEventManager.RaiseEvent(nameof(ActionEvent));

		//Assert
		Assert.That(didEventFire, Is.True);
	}

	[Test]
	public void WeakEventManagerAction_HandleEvent_InvalidHandleEventEventName()
	{
		//Arrange
		ActionEvent += HandleDelegateTest;
		bool didEventFire = false;

		void HandleDelegateTest() => didEventFire = true;

		//Act
		_actionEventManager.RaiseEvent(nameof(TestStringEvent));

		//Assert
		Assert.That(didEventFire, Is.False);
		ActionEvent -= HandleDelegateTest;
	}

	[Test]
	public void WeakEventManagerAction_UnassignedEvent()
	{
		//Arrange
		bool didEventFire = false;

		ActionEvent += HandleDelegateTest;
		ActionEvent -= HandleDelegateTest;
		void HandleDelegateTest() => didEventFire = true;

		//Act
		_actionEventManager.RaiseEvent(nameof(ActionEvent));

		//Assert
		Assert.That(didEventFire, Is.False);
	}

	[Test]
	public void WeakEventManagerAction_UnassignedEventManager()
	{
		//Arrange
		var unassignedEventManager = new WeakEventManager();
		bool didEventFire = false;

		ActionEvent += HandleDelegateTest;
		void HandleDelegateTest() => didEventFire = true;

		//Act
		unassignedEventManager.RaiseEvent(nameof(ActionEvent));

		//Assert
		Assert.That(didEventFire, Is.False);
		ActionEvent -= HandleDelegateTest;
	}

	[Test]
	public void WeakEventManagerAction_HandleEvent_InvalidHandleEvent()
	{
		//Arrange
		ActionEvent += HandleDelegateTest;
		bool didEventFire = false;

		void HandleDelegateTest() => didEventFire = true;

		//Act

		//Assert
		Assert.Multiple(() =>
		{
			Assert.Throws<InvalidHandleEventException>(() => _actionEventManager.RaiseEvent(this, EventArgs.Empty, nameof(ActionEvent)));
			Assert.That(didEventFire, Is.False);
		});

		ActionEvent -= HandleDelegateTest;
	}

	[Test]
	public void WeakEventManagerAction_AddEventHandler_NullHandler()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.AddEventHandler(null), "Value cannot be null.\nParameter name: handler");
	}

	[Test]
	public void WeakEventManagerAction_AddEventHandler_NullEventName()
	{
		//Arrange

		//Act

		//Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.AddEventHandler(null, null), "Value cannot be null.\nParameter name: eventName");
#pragma warning restore CS8625
	}

	[Test]
	public void WeakEventManagerAction_AddEventHandler_EmptyEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.AddEventHandler(null, string.Empty), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManagerAction_AddEventHandler_WhitespaceEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.AddEventHandler(null, " "), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManagerAction_RemoveEventHandler_NullHandler()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.RemoveEventHandler(null), "Value cannot be null.\nParameter name: handler");
	}

	[Test]
	public void WeakEventManagerAction_RemoveEventHandler_NullEventName()
	{
		//Arrange

		//Act

		//Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.RemoveEventHandler(null, null), "Value cannot be null.\nParameter name: eventName");
#pragma warning restore CS8625
	}

	[Test]
	public void WeakEventManagerAction_RemoveEventHandler_EmptyEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.RemoveEventHandler(null, string.Empty), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManagerAction_RemoveEventHandler_WhiteSpaceEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.RemoveEventHandler(null, " "), "Value cannot be null.\nParameter name: eventName");
	}
}