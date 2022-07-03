﻿using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;

namespace HackerNews.UITests
{
	[TestFixture(Platform.Android)]
	[TestFixture(Platform.iOS)]
	abstract class BaseTest
	{
		readonly Platform _platform;

		IApp? _app;
		NewsPage? _newsPage;

		protected BaseTest(Platform platform) => _platform = platform;

		protected IApp App => _app ?? throw new NullReferenceException();
		protected NewsPage NewsPage => _newsPage ?? throw new NullReferenceException();

		[SetUp]
		public virtual Task BeforeEachTest()
		{
			_app = AppInitializer.StartApp(_platform);
			_newsPage = new NewsPage(App);

			App.Screenshot("App Initialized");

			return NewsPage.WaitForPageToLoad();
		}

		[Test]
		[Ignore("Only used for testing")]
		public void ReplTest() => App.Repl();
	}
}