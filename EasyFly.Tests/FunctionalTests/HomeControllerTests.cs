using EasyFly.Infrastructure.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using EasyFly.Tests.Pages;

namespace EasyFly.Tests.FunctionalTests
{
    internal class HomeControllerTests
    {
        private IWebDriver _driver;
        private HomePage _homePage;
        private LoginPage _loginPage;
        private RegisterPage _registerPage;

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _driver.Url = Helper.RetrieveUrl();

            _homePage = new HomePage(_driver);
            _loginPage = new LoginPage(_driver);
            _registerPage = new RegisterPage(_driver);
        }

        [Test]
        public void TestIndex()
        {
            Assert.AreEqual("Welcome", _homePage.GetWelcomeText());
        }

        [Test]
        public void TestLoginPasses()
        {
            _homePage.GoToLogin();
            _loginPage.Login("admin@easyfly.com", "Admin@123");
            Assert.IsTrue(_homePage.IsWelcomeMessagePresent());
        }

        [Test]
        public void TestLoginFails()
        {
            _homePage.GoToLogin();
            _loginPage.Login("admin@easyfly.com", "Admin@1233");
            Assert.IsFalse(_homePage.IsWelcomeMessagePresent());
        }

        [Test]
        public void TestRegisterPasses()
        {
            _homePage.GoToRegister();
            string email = _registerPage.RegisterWithRandomEmail("Admin@123");
            Assert.IsTrue(_registerPage.IsRegistrationSuccessful());
        }

        [Test]
        public void TestRegisterFails()
        {
            _homePage.GoToRegister();
            _registerPage.Register("admin@easyfly.com", "Admin@123");
            Assert.IsFalse(_registerPage.IsRegistrationSuccessful());
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}