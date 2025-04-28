using EasyFly.Infrastructure.Services;
using EasyFly.Tests.Factories;
using EasyFly.Tests.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PlaneTicketReservationSystem.Pages;

namespace EasyFly.Tests.FunctionalTests
{
    internal class PlaneControllerTests
    {
        private IWebDriver _Driver;
        private PlanePage _PlanePage;
        private LoginPage _LoginPage;
        private HomePage _HomePage;

        [SetUp]
        public void Setup()
        {
            _Driver = new ChromeDriver();
            _Driver.Manage().Window.Maximize();
            _Driver.Url = Helper.RetrieveUrl();
            _PlanePage = new PlanePage(_Driver);
            _LoginPage = new LoginPage(_Driver);
            _HomePage = new HomePage(_Driver);
        }

        [Test]
        public void TestGetPlanesPasses()
        {
            _HomePage.GoToLogin();
            _LoginPage.LoginAsAdmin();
            _PlanePage.NavigateToPlanes();

            Assert.IsTrue(_PlanePage.ArePlanesDisplayed(), "No planes are displayed on the page.");
        }

        [Test]
        public void TestCreatePlanePasses()
        {
            _HomePage.GoToLogin();
            _LoginPage.LoginAsAdmin();
            _PlanePage.NavigateToPlanes();

            var plane = PlaneFactory.Create();
            _PlanePage.CreatePlane(plane);

            Assert.IsTrue(_PlanePage.IsPlaneCreated(plane.Name), "Plane creation failed.");
        }

        [Test]
        public void TestCreateInvalidNameFails()
        {
            _HomePage.GoToLogin();
            _LoginPage.LoginAsAdmin();
            _PlanePage.NavigateToPlanes();

            var plane = PlaneFactory.Create();
            plane.Name = "";
            _PlanePage.CreatePlane(plane);

            Assert.IsFalse(_PlanePage.IsPlaneCreated(plane.Name), "Plane creation with an invalid name succeeded.");
        }

        [Test]
        public void TestCreateInvalidSeatCountFails()
        {
            _HomePage.GoToLogin();
            _LoginPage.LoginAsAdmin();
            _PlanePage.NavigateToPlanes();

            var plane = PlaneFactory.Create();
            plane.AvailableSeats = -1;
            _PlanePage.CreatePlane(plane);

            Assert.IsFalse(_PlanePage.IsPlaneCreated(plane.Name), "Plane creation with an invalid seat count succeeded.");
        }

        [Test]
        public void TestGetPlanesUnauthorizedFails()
        {
            _PlanePage.NavigateToPlanes();

            Assert.IsFalse(_PlanePage.ArePlanesDisplayed(), "Planes endpoint does not require authentication.");
        }

        [TearDown]
        public void TearDown()
        {
            _Driver.Quit();
            _Driver.Dispose();
        }
    }
}