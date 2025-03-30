using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace EasyFly.Tests.Pages
{
    public class HomePage : BasePage
    {
        public HomePage(IWebDriver driver) : base(driver)
        {
        }

        public string GetWelcomeText()
        {
            var welcomeElement = Wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("display-4")));
            return welcomeElement.Text;
        }

        public void GoToLogin()
        {
            var loginButton = Wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Login")));
            loginButton.Click();
        }

        public void GoToRegister()
        {
            var registerButton = Wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Register")));
            registerButton.Click();
        }

        public bool IsWelcomeMessagePresent()
        {
            return Wait.Until(driver => driver.PageSource.Contains("Welcome"));
        }
    }
}
