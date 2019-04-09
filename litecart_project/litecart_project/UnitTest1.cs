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


namespace litecart_project
{
  [TestFixture]
  public class litecart_project_tests
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
      baseURL = "http://localhost";
    }


    [Test]
    public void LoginToLitecart()
    {
      driver.Navigate().GoToUrl(baseURL + "/litecart/admin/login.php");
      driver.FindElement(By.Name("username")).Clear();
      driver.FindElement(By.Name("username")).SendKeys("admin");
      driver.FindElement(By.Name("password")).Clear();
      driver.FindElement(By.Name("password")).SendKeys("admin");
      driver.FindElement(By.Name("login")).Click();

      wait.Until(ExpectedConditions.TitleIs("My Store"));
    }

    [TearDown]
    public void Stop()
    {
      driver.Quit();
      driver = null;
    }
  }
}
