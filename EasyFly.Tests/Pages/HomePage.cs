using OpenQA.Selenium;

namespace EasyFly.Tests.Pages
{
    public class HomePage
    {
        private readonly IWebDriver _driver;
        public HomePage(IWebDriver driver) => _driver = driver;

        public string GetWelcomeText() => _driver.FindElement(By.ClassName("display-4")).Text;

        public void GoToLogin() => _driver.FindElement(By.LinkText("Login")).Click();

        public void GoToRegister() => _driver.FindElement(By.LinkText("Register")).Click();

        public bool IsWelcomeMessagePresent() => _driver.PageSource.Contains("Welcome");
    }
}
