using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace EasyFly.Tests.Pages
{
    public abstract class BasePage
    {
        protected WebDriverWait Wait { get; set; }
        protected readonly IWebDriver Driver;
        private const int Timeout = 20;
        public BasePage(IWebDriver driver)
        {
            Driver = driver;
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(Timeout));
        }
    }
}
