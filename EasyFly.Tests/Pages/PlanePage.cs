using EasyFly.Application.Dtos;
using EasyFly.Domain.Models;
using EasyFly.Infrastructure.Services;
using EasyFly.Tests.Factories;
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
        private IWebElement AvailableSeatsInput => _driver.FindElement(By.Id("AvailableSeats"));
        private IWebElement SubmitButton => _driver.FindElement(By.CssSelector("button[type='submit']"));

        public void NavigateToPlanes()
        {
            _driver.Navigate().GoToUrl(Helper.RetrieveUrl() + "Plane/GetPlanes");
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
            var planeElements = _driver.FindElements(By.CssSelector(".card-title"));
            return planeElements.Any(e => e.Text.Contains(planeName));
        }

        public bool ArePlanesDisplayed()
        {
            return _driver.FindElements(By.CssSelector(".card .card-title")).Any();
        }
    }
}