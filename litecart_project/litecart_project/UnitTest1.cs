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
        public IWebDriver driver;
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


        public List<String> GetCountriesFromSite()
        {
            List<String> lsCountries = new List<string>();

            ICollection<IWebElement> elements_tr = driver.FindElements(By.CssSelector("tr.row"));
            foreach (IWebElement element in elements_tr)
            {
                IList<IWebElement> elements_td = element.FindElements(By.CssSelector("td"));
                if (elements_td.Count != 0)
                {
                    string s = elements_td.ElementAt(4).Text;
                    lsCountries.Add(elements_td.ElementAt(4).Text);
                }
            }
            return lsCountries;
        }

        public void CheckSortedZone()
        {
            ICollection<IWebElement> elements_tr = driver.FindElements(By.CssSelector("tr.row"));
            foreach (IWebElement element in elements_tr)
            {
                IList<IWebElement> elements_td = element.FindElements(By.CssSelector("td"));
                if (elements_td.Count != 0)
                {
                    int vZone = Convert.ToInt32(elements_td.ElementAt(5).Text);
                    if (vZone > 0)
                    {
                        string link = elements_td.ElementAt(4).FindElement(By.CssSelector("a")).GetAttribute("href");

                        //переход на страницу зон
                        IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                        jse.ExecuteScript("window.open()");
                        driver.SwitchTo().Window(driver.WindowHandles[driver.WindowHandles.Count - 1]);
                        driver.Navigate().GoToUrl(link);

                        //таблица зон
                        ICollection<IWebElement> tr_zones = driver.FindElements(By.CssSelector("table#table-zones.dataTable tr"));
                        List<String> lsZoneNotSorted = new List<string>();
                        for (int i = 1; i < tr_zones.Count - 1; i++)
                        {
                            IList<IWebElement> tr_zones_td = tr_zones.ElementAt(i).FindElements(By.CssSelector("td"));
                            lsZoneNotSorted.Add(tr_zones_td.ElementAt(2).Text);
                        }
                        List<String> lsZoneSorted = new List<string>();
                        lsZoneSorted = lsZoneNotSorted;
                        lsZoneSorted.Sort();

                        //Проверка сортировки зон внутри страны
                        Assert.AreEqual(lsZoneNotSorted, lsZoneSorted);
                        driver.Navigate().Back();

                        //Закрытие активного окна и возврат на страницу Countries
                        jse.ExecuteScript("window.close()");
                        driver.SwitchTo().Window(driver.WindowHandles[driver.WindowHandles.Count - 1]);
                    }
                }
            }
        }


        [Test]
        public void Task05_01()
        {
            /*
            1) на странице http://localhost/litecart/admin/?app=countries&doc=countries
            а) проверить, что страны расположены в алфавитном порядке
            б) для тех стран, у которых количество зон отлично от нуля-- открыть страницу 
              этой страны и там проверить, что зоны расположены в алфавитном порядке

            2) на странице http://localhost/litecart/admin/?app=geo_zones&doc=geo_zones
            зайти в каждую из стран и проверить, что зоны расположены в алфавитном порядке
            */

            LoginToAdmin();
            driver.Navigate().GoToUrl(baseURL + "/litecart/admin/?app=countries&doc=countries");
            List<String> lcCountriesFromWeb = new List<string>();
            lcCountriesFromWeb = GetCountriesFromSite();

            List<String> lsCountriesFromDB = CountriesData.GetCountriesFromDB();

            System.Console.Out.WriteLine("lcCountriesFromWeb = " + lcCountriesFromWeb.Count);
            System.Console.Out.WriteLine("lsCountriesFromDB = " + lsCountriesFromDB.Count);

            //Проверка сортировки списка стран на странице Countries 
            Assert.AreEqual(lcCountriesFromWeb, lsCountriesFromDB); //Работает

            //Проверка сортировки зон, если они есть, внутри страны
            CheckSortedZone();
        }

        [Test]
        public void Task05_02()
        {
            /*
            1.Открыть главную страницу
            2.выбрать первый товар в блоке Campaigns
            3.Проверить следующее:
            а) на главной странице и на странице товара совпадает текст названия товара
            б) на главной странице и на странице товара совпадают цены(обычная и акционная)
            в) обычная цена зачёркнутая и серая(можно считать, что "серый" цвет это такой,
              у которого в RGBa представлении одинаковые значения для каналов R, G и B)
            г) акционная жирная и красная (можно считать, что "красный" цвет это такой,
              у которого в RGBa представлении каналы G и B имеют нулевые значения)
              (цвета надо проверить на каждой странице независимо, при этом цвета на разных
              страницах могут не совпадать)
            д) акционная цена крупнее, чем обычная (это тоже надо проверить на каждой странице независимо)
            */

            LoginToLitecartShop();

            IList<IWebElement> elements = driver.FindElements(By.CssSelector("a.link"));
            foreach (IWebElement element in elements)
            {
                //Сравнение
                //1. Получаем название, цена товара на главной
                //2. Переходим на страницу товара
                //3. Получаем название товара, цену на странице товара
                //4. Сравниваем названия и цены
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
