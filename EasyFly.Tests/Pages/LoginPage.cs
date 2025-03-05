using OpenQA.Selenium;

namespace EasyFly.Tests.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        public LoginPage(IWebDriver driver) => _driver = driver;

        public void Login(string email, string password)
        {
            _driver.FindElement(By.Id("Input_Email")).SendKeys(email);
            _driver.FindElement(By.Id("Input_Password")).SendKeys(password);
            _driver.FindElement(By.Id("login-submit")).Click();
        }
        public void LoginAsAdmin()
        {
            Login("admin@easyfly.com", "Admin@123");
        }
    }
}
