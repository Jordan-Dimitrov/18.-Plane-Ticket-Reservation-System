using EasyFly.Infrastructure.Services;
using EasyFly.Tests.Factories;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PlaneTicketReservationSystem.Pages;

namespace EasyFly.Tests.FunctionalTests
{
    internal class PlaneControllerTests
    {
        private IWebDriver _Driver;
        private PlanePage _PlanePage;

        [SetUp]
        public void Setup()
        {
            _Driver = new ChromeDriver();
            _Driver.Manage().Window.Maximize();
            _Driver.Url = Helper.RetrieveUrl();
            _PlanePage = new PlanePage(_Driver);

        }

        [Test]
        public void TestGetPlanesReturnsTrue()
        {
            _Driver.FindElement(By.LinkText("Login")).Click();

            var emailField = _Driver.FindElement(By.Id("Input_Email"));
            var passwordField = _Driver.FindElement(By.Id("Input_Password"));
            var loginButton = _Driver.FindElement(By.Id("login-submit"));

            emailField.SendKeys("admin@easyfly.com");
            passwordField.SendKeys("Admin@123");
            loginButton.Click();

            _Driver.Navigate().GoToUrl(Helper.RetrieveUrl() + "Plane/GetPlanes");

            var planes = _Driver.FindElements(By.CssSelector(".card .card-title"));
            Assert.IsTrue(_PlanePage.ArePlanesDisplayed(), "No planes are displayed on the page.");
        }

        [Test]
        public void TestCreatePlaneReturnsTrue()
        {
            _Driver.FindElement(By.LinkText("Login")).Click();

            var emailField = _Driver.FindElement(By.Id("Input_Email"));
            var passwordField = _Driver.FindElement(By.Id("Input_Password"));
            var loginButton = _Driver.FindElement(By.Id("login-submit"));

            emailField.SendKeys("admin@easyfly.com");
            passwordField.SendKeys("Admin@123");
            loginButton.Click();

            _Driver.Navigate().GoToUrl(Helper.RetrieveUrl() + "Plane/GetPlanes");
            var plane = PlaneFactory.Create();

            _PlanePage.EnterName(plane.Name);
            _PlanePage.EnterSeats(plane.AvailableSeats);
            _PlanePage.SubmitForm();

            Assert.IsTrue(_PlanePage.IsPlaneCreated(plane.Name), "Plane creation failed.");
        }

        [Test]
        public void TestCreateInvalidNameFails()
        {
            _Driver.FindElement(By.LinkText("Login")).Click();

            var emailField = _Driver.FindElement(By.Id("Input_Email"));
            var passwordField = _Driver.FindElement(By.Id("Input_Password"));
            var loginButton = _Driver.FindElement(By.Id("login-submit"));

            emailField.SendKeys("admin@easyfly.com");
            passwordField.SendKeys("Admin@123");
            loginButton.Click();

            _Driver.Navigate().GoToUrl(Helper.RetrieveUrl() + "Plane/GetPlanes");
            var plane = PlaneFactory.Create();
            plane.Name = "";

            _PlanePage.EnterName(plane.Name);
            _PlanePage.EnterSeats(plane.AvailableSeats);
            _PlanePage.SubmitForm();

            Assert.IsFalse(_PlanePage.IsPlaneCreated(plane.Name), "Plane creation with invalid data.");
        }

        [Test]
        public void TestCreateInvalidSeatCountFails()
        {
            _Driver.FindElement(By.LinkText("Login")).Click();

            var emailField = _Driver.FindElement(By.Id("Input_Email"));
            var passwordField = _Driver.FindElement(By.Id("Input_Password"));
            var loginButton = _Driver.FindElement(By.Id("login-submit"));

            emailField.SendKeys("admin@easyfly.com");
            passwordField.SendKeys("Admin@123");
            loginButton.Click();

            _Driver.Navigate().GoToUrl(Helper.RetrieveUrl() + "Plane/GetPlanes");
            var plane = PlaneFactory.Create();
            plane.AvailableSeats = -1;

            _PlanePage.EnterName(plane.Name);
            _PlanePage.EnterSeats(plane.AvailableSeats);
            _PlanePage.SubmitForm();

            Assert.IsFalse(_PlanePage.IsPlaneCreated(plane.Name), "Plane creation with invalid data.");
        }

        [Test]
        public void TestGetPlanesUnauthorizedFails()
        {
            _Driver.Navigate().GoToUrl(Helper.RetrieveUrl() + "Plane/GetPlanes");

            var planes = _Driver.FindElements(By.CssSelector(".card .card-title"));
            Assert.IsFalse(_PlanePage.ArePlanesDisplayed(), "Planes endpoint does not have authorization");
        }

        [TearDown]
        public void TearDown()
        {
            _Driver.Quit();
            _Driver.Dispose();
        }
    }
}