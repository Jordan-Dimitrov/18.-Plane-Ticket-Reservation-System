using OpenQA.Selenium;

namespace PlaneTicketReservationSystem.Pages
{
    public class PlanePage
    {
        private readonly IWebDriver _driver;

        public PlanePage(IWebDriver driver)
        {
            _driver = driver;
        }

        private IWebElement NameInput => _driver.FindElement(By.Id("Name"));
        private IWebElement AvailableSeats => _driver.FindElement(By.Id("AvailableSeats"));
        private IWebElement SubmitButton => _driver.FindElement(By.CssSelector("button[type='submit']"));

        public void EnterName(string name)
        {
            NameInput.SendKeys(name);
        }
        public void EnterSeats(int seats)
        {
            AvailableSeats.SendKeys(seats.ToString());
        }

        public void SubmitForm()
        {
            SubmitButton.Click();
        }

        public bool IsPlaneCreated(string planeName)
        {
            var planeElements = _driver.FindElements(By.CssSelector(".card-title"));
            return planeElements.Any(e => e.Text.Contains(planeName));
        }

        public bool ArePlanesDisplayed()
        {
            var planes = _driver.FindElements(By.CssSelector(".card .card-title"));
            return planes.Any();
        }
    }
}