using System;
using Autofac;
using DBTM.Application;
using DBTM.Application.Views;
using DBTM.UI;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.UI
{
    [TestFixture]
    public class MainWindowControllerTests
    {
        private ILifetimeScope _appContainer;
        private ILifetimeScope _requestContainer;
        private IMainWindowView _view;
        private MainWindowController _controller;

        [SetUp]
        public void Setup()
        {
            _appContainer = MockRepository.GenerateMock<ILifetimeScope>();
            _requestContainer = MockRepository.GenerateMock<ILifetimeScope>();
            _view = MockRepository.GenerateMock<IMainWindowView>();

            _controller = new TestableMainWindowController(_appContainer, _view);
        }

        [TearDown]
        public void TearDown()
        {
            _appContainer.VerifyAllExpectations();
            _requestContainer.VerifyAllExpectations();
            _view.VerifyAllExpectations();
        }

        [Test]
        public void StartCreatesRequestContainerInitializesWindow()
        {
            _appContainer.Expect(c => c.BeginLifetimeScope()).Return(_requestContainer);
            _view.Expect(v => v.Initialize());

            _controller.Start();
        }

        [Test]
        public void StopDisposesRequestContainer()
        {
            _appContainer.Expect(c => c.BeginLifetimeScope()).Return(_requestContainer);
            _view.Expect(v => v.Initialize());

            _requestContainer.Expect(c => c.Dispose());

            _controller.Start();
            _controller.Stop();
        }



        private class TestableMainWindowController : MainWindowController
        {
            private readonly IMainWindowView _mainWindowView;

            public TestableMainWindowController(ILifetimeScope applicationContainer, IMainWindowView mainWindowView) : base(applicationContainer)
            {
                _mainWindowView = mainWindowView;
                ResolveMainWindowViewFunc = c => _mainWindowView;
            }
        }
    }
}