using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
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

        public void LoginToAdmin()
        {
            driver.Navigate().GoToUrl(baseURL + "/litecart/admin/login.php");
            driver.FindElement(By.Name("username")).Clear();
            driver.FindElement(By.Name("username")).SendKeys("admin");
            driver.FindElement(By.Name("password")).Clear();
            driver.FindElement(By.Name("password")).SendKeys("admin");
            driver.FindElement(By.Name("login")).Click();

            //wait.Until(ExpectedConditions.TitleIs("My Store"));
        }

        public void LoginToLitecartShop()
        {
            driver.Navigate().GoToUrl(baseURL + "/litecart/");
        }


        [SetUp]
        public void Start()
        {
            //InternetExplorerOptions options = new InternetExplorerOptions();
            //options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
            //driver = new InternetExplorerDriver(options);

            ChromeOptions options = new ChromeOptions();
            options.BinaryLocation = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            options.AddArgument("start-maximized");
            driver = new ChromeDriver(options);

            //FirefoxOptions options = new FirefoxOptions();
            //options.BrowserExecutableLocation = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
            //options.UseLegacyImplementation = true;
            //options.AddArgument("start-maximized");
            //driver = new FirefoxDriver(options);


            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            // wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            baseURL = "http://localhost";
        }


        [Test]
        public void LoginToLitecart()
        {
            LoginToAdmin();
        }

        public bool IsElementPresent(IWebDriver driver, By locator)
        {
            try
            {
                driver.FindElement(locator);
                return true;
            }
            catch (NoSuchElementException ex)
            {
                return false;
            }
        }


        [Test]
        public void Task04_01()
        {
            LoginToAdmin();

            IList<IWebElement> root_elem = driver.FindElements(By.CssSelector("ul#box-apps-menu li#app-"));
            int count_root_el = root_elem.Count;

            for (int i = 0; i < count_root_el; i++)
            {
                IList<IWebElement> rootelements = driver.FindElements(By.CssSelector("ul#box-apps-menu li#app-"));
                rootelements.ElementAt(i).Click();

                IList<IWebElement> child_el = driver.FindElements(By.CssSelector("ul.docs li"));
                int count_child_el = child_el.Count;
                if (child_el.Count != 0)
                {
                    for (int j = 0; j < count_child_el; j++)
                    {
                        IList<IWebElement> childnodes = driver.FindElements(By.CssSelector("ul.docs li"));
                        childnodes.ElementAt(j).Click();
                        Debug.WriteLine("TagName child_h1 = " + driver.FindElement(By.TagName("h1")).Text);
                        Assert.True(IsElementPresent(driver, By.TagName("h1")));
                    }
                }

                Debug.WriteLine("TagName child_h1 = " + driver.FindElement(By.TagName("h1")).Text);
                Assert.True(IsElementPresent(driver, By.TagName("h1")));
            }

        }

        [Test]
        public void Task04_02()
        {
            LoginToLitecartShop();

            IList<IWebElement> elements = driver.FindElements(By.CssSelector("li.product"));
            foreach (IWebElement element in elements)
            {
                IList<IWebElement> stickers = element.FindElements(By.CssSelector("div.sticker"));
                if (stickers.Count != 0)
                {
                    Assert.AreEqual(1, stickers.Count);
                }
            }
        }


        [TearDown]
        public void Stop()
        {
            driver.Quit();
            driver = null;
        }
    }
}
