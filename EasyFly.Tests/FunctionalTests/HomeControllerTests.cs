using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace EasyFly.Tests.FunctionalTests
{
    internal class HomeControllerTests
    {
        private IWebDriver _Driver;
        public HomeControllerTests()
        {
        }

        [SetUp]
        public void Setup()
        {
            _Driver = new ChromeDriver();
            _Driver.Manage().Window.Maximize();
            _Driver.Url = "https://localhost:8081/";

        }

        [Test]
        public void TestIndex()
        {
            var item = _Driver.FindElement(By.ClassName("display-4"));
            Assert.AreEqual("Welcome", item.Text);
        }

        [TearDown]
        public void TearDown()
        {
            _Driver.Quit();
            _Driver.Dispose();
        }
    }
}
