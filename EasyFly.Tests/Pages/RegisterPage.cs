using OpenQA.Selenium;
using System;

namespace EasyFly.Tests.Pages
{
    public class RegisterPage
    {
        private readonly IWebDriver _driver;
        public RegisterPage(IWebDriver driver) => _driver = driver;

        public void Register(string email, string password)
        {
            _driver.FindElement(By.Id("Input_Email")).SendKeys(email);
            _driver.FindElement(By.Id("Input_Password")).SendKeys(password);
            _driver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys(password);
            _driver.FindElement(By.Id("registerSubmit")).Click();
        }

        public string RegisterWithRandomEmail(string password)
        {
            string email = $"newuser{new Random().Next(1, 1230)}@example.com";
            Register(email, password);
            return email;
        }

        public bool IsRegistrationSuccessful() => _driver.PageSource.Contains("Register confirmation");
    }
}
