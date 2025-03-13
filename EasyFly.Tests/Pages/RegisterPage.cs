using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace EasyFly.Tests.Pages
{
    public class RegisterPage : BasePage
    {
        public RegisterPage(IWebDriver driver) : base(driver)
        {

        }

        public void Register(string email, string password)
        {
            var emailField = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_Email")));
            emailField.Clear();
            emailField.SendKeys(email);

            var passwordField = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_Password")));
            passwordField.Clear();
            passwordField.SendKeys(password);

            var confirmPasswordField = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_ConfirmPassword")));
            confirmPasswordField.Clear();
            confirmPasswordField.SendKeys(password);

            var submitButton = Wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("registerSubmit")));
            submitButton.Click();
        }

        public string RegisterWithRandomEmail(string password)
        {
            string email = $"newuser{new Random().Next(1, 1230)}@example.com";
            Register(email, password);
            return email;
        }

        public bool IsRegistrationSuccessful()
        {
            return Wait.Until(driver => driver.PageSource.Contains("Register confirmation"));
        }
    }
}
