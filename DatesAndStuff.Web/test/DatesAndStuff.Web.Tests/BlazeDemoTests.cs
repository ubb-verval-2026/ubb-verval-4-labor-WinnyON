using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using FluentAssertions;

namespace DatesAndStuff.Web.Tests
{
    [TestFixture]
    public class BlazeDemoTests
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;
        
        [SetUp]
        public void SetupTest()
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            baseURL = "https://www.google.com/";
            verificationErrors = new StringBuilder();
        }
        
        [TearDown]
        public void TeardownTest()
        {
            try
            {
                if (driver != null)
                {
                    driver.Quit();
                    driver.Dispose();
                }
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            if (verificationErrors != null)
            {
                Assert.AreEqual("", verificationErrors.ToString());
            }
        }
        
        [Test]
        public void TheFlightSelectorTest()
        {
            driver.Navigate().GoToUrl("https://blazedemo.com/");
            driver.FindElement(By.Name("fromPort")).Click();
            new SelectElement(driver.FindElement(By.Name("fromPort"))).SelectByText("Mexico City");
            driver.FindElement(By.Name("toPort")).Click();
            new SelectElement(driver.FindElement(By.Name("toPort"))).SelectByText("Dublin");
            driver.FindElement(By.XPath("//input[@value='Find Flights']")).Click();
            
            var rows = driver.FindElements(By.CssSelector("table.table tbody tr"));
            rows.Count.Should().BeGreaterThanOrEqualTo(3, "There has to be at least 3 flights between Mexico City and Dublin.");
            
            // bonus
            double priceLimit = 500.0;
            bool foundCheapFlight = false;

            foreach (var row in rows)
            {
                var priceText = row.FindElement(By.CssSelector("td:nth-child(7)")).Text;
                priceText = priceText.Replace("$", "").Trim();

                if (double.TryParse(priceText, System.Globalization.CultureInfo.InvariantCulture, out double price))
                {
                    if (price < priceLimit)
                    {
                        foundCheapFlight = true;
                        break;
                    }
                }
            }

            if (foundCheapFlight)
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = Path.Combine(desktopPath, "cheap_flight_blazedemo.png");
                screenshot.SaveAsFile(filePath);
            
                Console.WriteLine($"Found cheap flight, screenshot saved at: {filePath}");
            }
        }
        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        
        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }
        
        private string CloseAlertAndGetItsText() {
            try {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert) {
                    alert.Accept();
                } else {
                    alert.Dismiss();
                }
                return alertText;
            } finally {
                acceptNextAlert = true;
            }
        }
    }
}