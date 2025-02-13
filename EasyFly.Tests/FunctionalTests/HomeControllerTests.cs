using EasyFly.Infrastructure.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;

namespace EasyFly.Tests.FunctionalTests
{
    internal class HomeControllerTests
    {
        private IWebDriver _Driver;

        [SetUp]
        public void Setup()
        {
            _Driver = new ChromeDriver();
            _Driver.Manage().Window.Maximize();
            _Driver.Url = Helper.RetrieveUrl();
        }

        [Test]
        public void TestIndex()
        {
            var item = _Driver.FindElement(By.ClassName("display-4"));
            Assert.AreEqual("Welcome", item.Text);
        }

        [Test]
        public void TestLoginPasses()
        {
            _Driver.FindElement(By.LinkText("Login")).Click();

            var emailField = _Driver.FindElement(By.Id("Input_Email"));
            var passwordField = _Driver.FindElement(By.Id("Input_Password"));
            var loginButton = _Driver.FindElement(By.Id("login-submit"));

            emailField.SendKeys("admin@easyfly.com");
            passwordField.SendKeys("Admin@123");
            loginButton.Click();

            Assert.IsTrue(_Driver.PageSource.Contains("Welcome"));
        }

        [Test]
        public void TestLoginFails()
        {
            _Driver.FindElement(By.LinkText("Login")).Click();

            var emailField = _Driver.FindElement(By.Id("Input_Email"));
            var passwordField = _Driver.FindElement(By.Id("Input_Password"));
            var loginButton = _Driver.FindElement(By.Id("login-submit"));

            emailField.SendKeys("admin@easyfly.com");
            passwordField.SendKeys("Admin@1233");
            loginButton.Click();

            Assert.IsFalse(_Driver.PageSource.Contains("Welcome"));
        }

        [Test]
        public void TestRegisterPasses()
        {
            _Driver.FindElement(By.LinkText("Register")).Click();
            Random rng = new Random();

            var emailField = _Driver.FindElement(By.Id("Input_Email"));
            var passwordField = _Driver.FindElement(By.Id("Input_Password"));
            var confirmPasswordField = _Driver.FindElement(By.Id("Input_ConfirmPassword"));
            var registerButton = _Driver.FindElement(By.Id("registerSubmit"));

            emailField.SendKeys($"newuser@examp{rng.Next(1,1230)}le.com");
            passwordField.SendKeys("Admin@123");
            confirmPasswordField.SendKeys("Admin@123");
            registerButton.Click();

            Assert.IsTrue(_Driver.PageSource.Contains("Register confirmation"));
        }

        [Test]
        public void TestRegisterFails()
        {
            _Driver.FindElement(By.LinkText("Register")).Click();
            Random rng = new Random();

            var emailField = _Driver.FindElement(By.Id("Input_Email"));
            var passwordField = _Driver.FindElement(By.Id("Input_Password"));
            var confirmPasswordField = _Driver.FindElement(By.Id("Input_ConfirmPassword"));
            var registerButton = _Driver.FindElement(By.Id("registerSubmit"));

            emailField.SendKeys($"admin@easyfly.com");
            passwordField.SendKeys("Admin@123");
            confirmPasswordField.SendKeys("Admin@123");
            registerButton.Click();

            Assert.IsFalse(_Driver.PageSource.Contains("Register confirmation"));
        }

        [TearDown]
        public void TearDown()
        {
            _Driver.Quit();
            _Driver.Dispose();
        }
    }
}