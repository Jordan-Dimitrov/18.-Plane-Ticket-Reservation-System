using EasyFly.Application.Dtos;
using EasyFly.Infrastructure.Services;
using EasyFly.Tests.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace PlaneTicketReservationSystem.Pages
{
    public class PlanePage : BasePage
    {
        private WebDriverWait Wait => new WebDriverWait(Driver, TimeSpan.FromSeconds(10));

        public PlanePage(IWebDriver driver) : base(driver)
        {
        }

        private IWebElement NameInput => Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Name")));
        private IWebElement AvailableSeatsInput => Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("AvailableSeats")));
        private IWebElement SubmitButton => Wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[type='submit']")));

        public void NavigateToPlanes()
        {
            Driver.Navigate().GoToUrl(Helper.RetrieveUrl() + "Plane/GetPlanes");
        }

        public void CreatePlane(PlaneDto plane)
        {
            NameInput.Clear();
            NameInput.SendKeys(plane.Name);

            AvailableSeatsInput.Clear();
            AvailableSeatsInput.SendKeys(plane.AvailableSeats.ToString());

            SubmitButton.Click();
        }

        public bool IsPlaneCreated(string planeName)
        {
            var planeElements = Wait.Until(d => d.FindElements(By.CssSelector(".card-title")));
            return planeElements.Any(e => e.Text.Contains(planeName));
        }

        public bool ArePlanesDisplayed()
        {
            var planes = Wait.Until(d => d.FindElements(By.CssSelector(".card .card-title")));
            return planes.Any();
        }
    }
}
