using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support.Events;


namespace CSharp_Example
{
  [TestFixture]
  public class MyFirstTest
  {
    private IWebDriver driver;
    private WebDriverWait wait;
    private string baseURL;

    [SetUp]
    public void Start()
    {
      InternetExplorerOptions options = new InternetExplorerOptions();
      options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
      driver = new InternetExplorerDriver(options);
      wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }


    [Test]
    public void TestMethod1()
    {
      baseURL = "https://www.google.com";
      driver.Navigate().GoToUrl(baseURL);
      driver.FindElement(By.Name("q")).Click();
      driver.FindElement(By.Name("q")).Clear();
      driver.FindElement(By.Name("q")).SendKeys("тест");
      driver.FindElement(By.Name("btnK")).Click();
      wait.Until(ExpectedConditions.TitleIs("тест - Поиск в Google"));
    }

    [TearDown]
    public void Stop()
    {
      driver.Quit();
      driver = null;
    }
  }
}
