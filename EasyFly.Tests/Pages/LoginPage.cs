using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace EasyFly.Tests.Pages
{
    public class LoginPage : BasePage
    {
        public LoginPage(IWebDriver driver) : base(driver)
        {

        }

        public void Login(string email, string password)
        {
            var emailField = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_Email")));
            emailField.Clear();
            emailField.SendKeys(email);

            var passwordField = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_Password")));
            passwordField.Clear();
            passwordField.SendKeys(password);

            var loginButton = Wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("login-submit")));
            loginButton.Click();
        }
        public void LoginAsAdmin()
        {
            Login("admin@easyfly.com", "Admin@123");
        }
    }
}
