﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support.Events;

using System.Drawing;
using System.Text.RegularExpressions;


namespace litecart_project
{
    [TestFixture]
    public class litecart_project_tests : TestBase
    {
        public IWebDriver driver;
        private WebDriverWait wait;
        private string baseURL;

        

        public void LoginToLitecartShop()
        {
            driver.Navigate().GoToUrl(baseURL + "/litecart/");
            //Вход под клиентом
        }


        [Test]
        public void LoginToLitecart()
        {
            app.Auth.LoginToAdmin();
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
            app.Auth.LoginToAdmin();

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


            app.Auth.LoginToAdmin();

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



            //2) на странице http://localhost/litecart/admin/?app=geo_zones&doc=geo_zones
            //зайти в каждую из стран и проверить, что зоны расположены в алфавитном порядке
            driver.Navigate().GoToUrl(baseURL + "/litecart/admin/?app=geo_zones&doc=geo_zones");

            ICollection<IWebElement> geozones = driver.FindElements(By.CssSelector("td#content tr.row"));
            foreach (IWebElement element in geozones)
            {
                IList<IWebElement> elements_td = element.FindElements(By.CssSelector("td"));
                if (elements_td.Count != 0)
                {
                    int vZone = Convert.ToInt32(elements_td.ElementAt(3).Text);
                    if (vZone > 0)
                    {
                        string link = elements_td.ElementAt(2).FindElement(By.CssSelector("a")).GetAttribute("href");

                        //переход на страницу зон
                        IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                        jse.ExecuteScript("window.open()");
                        driver.SwitchTo().Window(driver.WindowHandles[driver.WindowHandles.Count - 1]);
                        driver.Navigate().GoToUrl(link);

                        //Проверка сортировки списка стран
                        ICollection<IWebElement> tr_zones = driver.FindElements(By.CssSelector("table#table-zones.dataTable tr"));
                        List<String> lsZoneNotSorted = new List<string>();
                        for (int i = 1; i < tr_zones.Count - 1; i++)
                        {
                            IList<IWebElement> tr_zones_td = tr_zones.ElementAt(i).
                              FindElements(By.CssSelector("td select [selected='selected']"));
                            lsZoneNotSorted.Add(tr_zones_td.ElementAt(1).GetAttribute("text"));
                        }
                        List<String> lsZoneSorted = new List<string>();
                        lsZoneSorted = lsZoneNotSorted;
                        lsZoneSorted.Sort();

                        //Проверка сортировки зон внутри страны
                        Assert.AreEqual(lsZoneNotSorted, lsZoneSorted);


                        //Закрытие активного окна и возврат на страницу Countries
                        jse.ExecuteScript("window.close()");
                        driver.SwitchTo().Window(driver.WindowHandles[driver.WindowHandles.Count - 1]);
                    }

                }
            }

        }

        public void ConvertRGB(string RGB, out int r, out int g, out int b)
        {
            Regex regex = new Regex(@"rgb\((?<r>\d{1,3}), (?<g>\d{1,3}), (?<b>\d{1,3})\)");
            Match match = regex.Match(RGB);
            if (match.Success)
            {
                r = int.Parse(match.Groups["r"].Value);
                g = int.Parse(match.Groups["g"].Value);
                b = int.Parse(match.Groups["b"].Value);
            }
            else
            {
                r = 0;
                g = 0;
                b = 0;
            }

        }

        [Test]
        public void Task05_02()
        {
            LoginToLitecartShop();

            //Свойства продукта на главной странице блок = Campaigns
            ProductData ProductOnMainForm = new ProductData();
            ProductOnMainForm.Name =
              driver.FindElement(By.CssSelector("div#box-campaigns.box div.name")).GetAttribute("innerText");
            ProductOnMainForm.ProductLink =
              driver.FindElement(By.CssSelector("div#box-campaigns.box a.link")).GetAttribute("href");
            ProductOnMainForm.RegularPrice =
              driver.FindElement(By.CssSelector("div#box-campaigns.box s.regular-price")).GetAttribute("innerText");
            ProductOnMainForm.CampaignPrice =
              driver.FindElement(By.CssSelector("div#box-campaigns.box strong.campaign-price")).GetAttribute("innerText");

            ProductOnMainForm.RegularPriceColor =
              driver.FindElement(By.CssSelector("div#box-campaigns.box s.regular-price")).GetCssValue("Color");
            ProductOnMainForm.CampaignPriceColor =
              driver.FindElement(By.CssSelector("div#box-campaigns.box strong.campaign-price")).GetCssValue("Color");

            ProductOnMainForm.RegularPriceFontStrike =
              driver.FindElement(By.CssSelector("div#box-campaigns.box s.regular-price")).GetCssValue("text-decoration-line");

            ProductOnMainForm.CampaignPriceFontBold =
              driver.FindElement(By.CssSelector("div#box-campaigns.box strong.campaign-price")).GetCssValue("font-weight");

            ProductOnMainForm.RegularPriceFontSize =
              driver.FindElement(By.CssSelector("div#box-campaigns.box s.regular-price")).GetCssValue("font-size");

            ProductOnMainForm.CampaignPriceFontSize =
              driver.FindElement(By.CssSelector("div#box-campaigns.box strong.campaign-price")).GetCssValue("font-size");


            //Переход на страницу продукта
            driver.Navigate().GoToUrl(ProductOnMainForm.ProductLink);

            //Свойства продукта на странице продукта
            ProductData ProductOnProductForm = new ProductData();
            ProductOnProductForm.Name =
              driver.FindElement(By.CssSelector("div#box-product.box h1.title")).GetAttribute("innerText");
            ProductOnProductForm.ProductLink =
              driver.FindElement(By.CssSelector("div#box-product.box")).GetAttribute("baseURI");
            ProductOnProductForm.RegularPrice =
              driver.FindElement(By.CssSelector("div#box-product.box s.regular-price")).GetAttribute("innerText");
            ProductOnProductForm.CampaignPrice =
              driver.FindElement(By.CssSelector("div#box-product.box strong.campaign-price")).GetAttribute("innerText");

            ProductOnProductForm.RegularPriceColor =
              driver.FindElement(By.CssSelector("div#box-product.box div.information div.price-wrapper s.regular-price")).
              GetCssValue("Color");
            ProductOnProductForm.CampaignPriceColor =
              driver.FindElement(By.CssSelector("div#box-product.box div.information div.price-wrapper strong.campaign-price")).
              GetCssValue("Color");

            ProductOnProductForm.RegularPriceFontStrike =
              driver.FindElement(By.CssSelector("div#box-product.box div.information div.price-wrapper s.regular-price")).
              GetCssValue("text-decoration-line"); //line-through 

            ProductOnProductForm.CampaignPriceFontBold =
              driver.FindElement(By.CssSelector("div#box-product.box div.information div.price-wrapper strong.campaign-price")).
              GetCssValue("font-weight");    //font-weight = 700

            ProductOnProductForm.RegularPriceFontSize =
              driver.FindElement(By.CssSelector("div#box-product.box div.information div.price-wrapper s.regular-price")).
              GetCssValue("font-size");

            ProductOnProductForm.CampaignPriceFontSize =
              driver.FindElement(By.CssSelector("div#box-product.box div.information div.price-wrapper strong.campaign-price")).
              GetCssValue("font-size");



            //Проверки:
            //а) на главной странице и на странице товара совпадает текст названия товара
            Assert.AreEqual(ProductOnMainForm.Name, ProductOnProductForm.Name);

            //б) на главной странице и на странице товара совпадают цены(обычная и акционная)
            Assert.AreEqual(ProductOnMainForm.RegularPrice, ProductOnProductForm.RegularPrice);
            Assert.AreEqual(ProductOnMainForm.CampaignPrice, ProductOnProductForm.CampaignPrice);

            //в) обычная цена зачёркнутая и серая(можно считать, что "серый" цвет это такой,
            //у которого в RGBa представлении одинаковые значения для каналов R, G и B)
            int r, g, b;
            ConvertRGB(ProductOnProductForm.RegularPriceColor, out r, out g, out b);
            Assert.AreEqual(r, g);
            Assert.AreEqual(g, b);

            Assert.AreEqual(ProductOnMainForm.RegularPriceFontStrike, "line-through");
            Assert.AreEqual(ProductOnProductForm.RegularPriceFontStrike, "line-through");


            //г) акционная жирная и красная (можно считать, что "красный" цвет это такой,
            //у которого в RGBa представлении каналы G и B имеют нулевые значения)
            //(цвета надо проверить на каждой странице независимо, при этом цвета на разных
            //страницах могут не совпадать)
            ConvertRGB(ProductOnMainForm.CampaignPriceColor, out r, out g, out b);
            Assert.AreEqual(g, 0);
            Assert.AreEqual(b, 0);

            ConvertRGB(ProductOnProductForm.CampaignPriceColor, out r, out g, out b);
            Assert.AreEqual(g, 0);
            Assert.AreEqual(b, 0);

            Assert.AreEqual(ProductOnMainForm.CampaignPriceFontBold, "700");
            Assert.AreEqual(ProductOnProductForm.CampaignPriceFontBold, "700");

            //д) акционная цена крупнее, чем обычная (это тоже надо проверить на каждой странице независимо)
            Assert.Greater(ProductOnMainForm.CampaignPriceFontSize, ProductOnMainForm.RegularPriceFontSize);
            Assert.Greater(ProductOnProductForm.CampaignPriceFontSize, ProductOnProductForm.RegularPriceFontSize);

        }



        [Test]
        public void Task06_01()
        {
            /* "Сценарий регистрации пользователя"
            Сделайте сценарий для регистрации нового пользователя в учебном приложении litecart 
            (не в админке, а в клиентской части магазина).
            Сценарий должен состоять из следующих частей:
              1) регистрация новой учётной записи с достаточно уникальным адресом электронной почты 
              (чтобы не конфликтовало с ранее созданными пользователями, в том числе при предыдущих 
              запусках того же самого сценария),
              2) выход (logout), потому что после успешной регистрации автоматически происходит вход,
              3) повторный вход в только что созданную учётную запись,
              4) и ещё раз выход.
            В качестве страны выбирайте United States, штат произвольный. При этом формат индекса -- пять цифр.
            */

            driver.Navigate().GoToUrl(baseURL + "/litecart/");

            //Создание нового пользователя
            string lnkCreateNewClient =
              driver.FindElement(By.CssSelector("form[name='login_form'] td a")).GetAttribute("href");
            driver.Navigate().GoToUrl(lnkCreateNewClient);

            ClientData newClient = new ClientData();
            newClient.FirstName = "John";
            newClient.LastName = "Smith";
            newClient.Address1 = "Test Address 1";
            newClient.Address2 = "Test Address 2";
            newClient.Postcode = "10001"; //5 цифр
            newClient.City = "New York";
            newClient.Country = "United States";
            newClient.State = "New York";

            Random random = new Random();
            newClient.Email = string.Format("qa{0:0000}@test.com", random.Next(10000));

            newClient.Phone = "+79008006050";
            newClient.Password = "test";
            newClient.ConfirmPassword = "test";

            driver.FindElement(By.CssSelector("div#create-account.box input[name='firstname']")).
              SendKeys(newClient.FirstName);
            driver.FindElement(By.CssSelector("div#create-account.box input[name='lastname']")).
              SendKeys(newClient.LastName);
            driver.FindElement(By.CssSelector("div#create-account.box input[name='address1']")).
              SendKeys(newClient.Address1);
            driver.FindElement(By.CssSelector("div#create-account.box input[name='address2']")).
              SendKeys(newClient.Address2);
            driver.FindElement(By.CssSelector("div#create-account.box input[name='postcode']")).
              SendKeys(newClient.Postcode);
            driver.FindElement(By.CssSelector("div#create-account.box input[name='city']")).
              SendKeys(newClient.City);


            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
            IWebElement webElement =
              driver.FindElement(By.CssSelector("div#create-account.box select[name='country_code']"));
            jse.ExecuteScript("arguments[0].click();", webElement);

            new SelectElement(driver.FindElement(By.CssSelector("div#create-account.box select[name='country_code']"))).
              SelectByText(newClient.Country);
            driver.FindElement(By.CssSelector("div#create-account.box select[name='zone_code']")).Click();
            new SelectElement(driver.FindElement(By.CssSelector("div#create-account.box select[name='zone_code']"))).
              SelectByText(newClient.State);

            driver.FindElement(By.CssSelector("div#create-account.box input[name='email']")).
              SendKeys(newClient.Email);
            driver.FindElement(By.CssSelector("div#create-account.box input[name='phone']")).
              SendKeys(newClient.Phone);
            driver.FindElement(By.CssSelector("div#create-account.box input[name='password']")).
              SendKeys(newClient.Password);
            driver.FindElement(By.CssSelector("div#create-account.box input[name='confirmed_password']")).
              SendKeys(newClient.ConfirmPassword);


            //CreateAccount
            driver.FindElement(By.CssSelector("div#create-account.box button[name='create_account']")).Click();
            driver.FindElement(By.LinkText("Logout")).Click();

            //Вход под созданным пользователем
            driver.Navigate().GoToUrl(baseURL + "/litecart/");
            driver.FindElement(By.Name("email")).Clear();
            driver.FindElement(By.Name("email")).SendKeys(newClient.Email);
            driver.FindElement(By.Name("password")).Clear();
            driver.FindElement(By.Name("password")).SendKeys(newClient.Password);
            driver.FindElement(By.Name("login")).Click();

            driver.FindElement(By.LinkText("Logout")).Click();
        }

        [Test]
        public void Task06_02()
        {
            /* "Сценарий добавления товара"
              Сделайте сценарий для добавления нового товара (продукта) в учебном приложении litecart (в админке).

              Для добавления товара нужно открыть меню Catalog, в правом верхнем углу нажать кнопку "Add New Product", 
              заполнить поля с информацией о товаре и сохранить.

              Достаточно заполнить только информацию на вкладках General, Information и Prices. 
              Скидки (Campains) на вкладке Prices можно не добавлять.

              Переключение между вкладками происходит не мгновенно, поэтому после переключения можно сделать 
              небольшую паузу 
              (о том, как делать более правильные ожидания, будет рассказано в следующих занятиях).

              Картинку с изображением товара нужно уложить в репозиторий вместе с кодом. При этом указывать 
              в коде полный абсолютный путь к файлу плохо, на другой машине работать не будет. 
              Надо средствами языка программирования преобразовать относительный путь в абсолютный.

              После сохранения товара нужно убедиться, что он появился в каталоге (в админке). 
              Клиентскую часть магазина можно не проверять.

            */

            app.Auth.LoginToAdmin();
            IList<IWebElement> menuElements = driver.FindElements(By.CssSelector("ul#box-apps-menu li#app- span.name"));
            menuElements.ElementAt(1).Click();

            driver.FindElement(By.LinkText("Add New Product")).Click();

            ProductData newProduct = new ProductData();
            newProduct.Name = "TestDuck";
            newProduct.Code = "rd0000";
            newProduct.Quantity = "50";
            newProduct.Manufacturer = "ACME Corp.";
            newProduct.ShortDescription = "Test Short Description";
            newProduct.Description = "Test Description";
            newProduct.PurchasePrice = "20";
            newProduct.CurrencyCode = "US Dollars";
            newProduct.Price = "10";
            newProduct.CampaignPrice = "5";


            //Информация о товаре на вкладке General
            driver.FindElement(By.LinkText("General")).Click();

            //Статус
            IList<IWebElement> statusProductElements =
              driver.FindElements(By.CssSelector("#tab-general>table>tbody>tr>td>label"));
            statusProductElements.ElementAt(0).Click();

            //Имя и код
            driver.FindElement(By.Name("name[en]")).Clear();
            driver.FindElement(By.Name("name[en]")).SendKeys(newProduct.Name);
            driver.FindElement(By.Name("code")).Clear();
            driver.FindElement(By.Name("code")).SendKeys(newProduct.Code);

            //Product Category
            IList<IWebElement> grProductCategory_Groups = driver.FindElements(By.CssSelector("div.input-wrapper"));
            foreach (IWebElement element in grProductCategory_Groups)
            {
                IList<IWebElement> grProductCategory = element.FindElements(By.CssSelector("input[type='checkbox']"));
                foreach (IWebElement elPrCateg in grProductCategory)
                {
                    string vRoot = elPrCateg.GetAttribute("data-name");
                    string vCheck = elPrCateg.GetAttribute("checked");

                    if (vRoot == "Root")
                    {
                        if (vCheck == "true")
                        {
                            elPrCateg.Click();
                        }
                        else continue;
                    }
                    else
                    {
                        if (vCheck != "true")
                        {
                            elPrCateg.Click();
                        }
                        else continue;
                    }
                }


                //Проставление чеков для Product Groups пока BREAK
                //IList<IWebElement> grProductCategory_Group = driver.FindElements(By.CssSelector("div.input-wrapper"));
                break;
            }

            //Quantity
            driver.FindElement(By.CssSelector("div.content input[name='quantity']")).Clear();
            driver.FindElement(By.CssSelector("div.content input[name='quantity']")).SendKeys(newProduct.Quantity);
            //Sold Out Status
            driver.FindElement(By.CssSelector("#tab-general select[name='sold_out_status_id']")).Click();
            new SelectElement(driver.FindElement(By.CssSelector("#tab-general select[name='sold_out_status_id']"))).
              SelectByText("Temporary sold out");
            //Upload Images
            string vPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string vFile = @"\test_yellow-duck.png";
            IWebElement vSelectFile = driver.FindElement(By.CssSelector("#tab-general input[type='file']"));
            vSelectFile.SendKeys(vPath + vFile);

            //Информация о товаре на вкладке Information
            driver.FindElement(By.LinkText("Information")).Click();
            driver.FindElement(By.CssSelector("div.content select[name='manufacturer_id']")).Click();
            new SelectElement(driver.FindElement(By.CssSelector("div.content select[name='manufacturer_id']"))).
              SelectByText(newProduct.Manufacturer);

            driver.FindElement(By.CssSelector("div.content input[name='short_description[en]']")).Clear();
            driver.FindElement(By.CssSelector("div.content input[name='short_description[en]']")).SendKeys(newProduct.ShortDescription);
            driver.FindElement(By.CssSelector("div.trumbowyg-editor")).Clear();
            driver.FindElement(By.CssSelector("div.trumbowyg-editor")).SendKeys(newProduct.Description);


            //Информация о товаре на вкладке Prices
            driver.FindElement(By.LinkText("Prices")).Click();
            driver.FindElement(By.CssSelector("div.content input[name='purchase_price']")).Clear();
            driver.FindElement(By.CssSelector("div.content input[name='purchase_price']")).SendKeys(newProduct.PurchasePrice);
            driver.FindElement(By.CssSelector("div.content select[name='purchase_price_currency_code']")).Click();
            new SelectElement(driver.FindElement(By.CssSelector("div.content select[name='purchase_price_currency_code']"))).
              SelectByText(newProduct.CurrencyCode);

            switch (newProduct.CurrencyCode)
            {
                case "US Dollars":
                    driver.FindElement(By.CssSelector("div.content input[name='prices[USD]']")).Clear();
                    driver.FindElement(By.CssSelector("div.content input[name='prices[USD]']")).SendKeys(newProduct.Price);
                    break;
                case "Euros":
                    driver.FindElement(By.CssSelector("div.content input[name='prices[EUR]']")).Clear();
                    driver.FindElement(By.CssSelector("div.content input[name='prices[EUR]']")).SendKeys(newProduct.Price);
                    break;
                default:
                    break;
            }

            driver.FindElement(By.CssSelector("div.content a[id='add-campaign']")).Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            switch (newProduct.CurrencyCode)
            {
                case "US Dollars":
                    driver.FindElement(By.CssSelector("div.content table[id='table-campaigns'] input[name*='USD']")).Clear();
                    driver.FindElement(By.CssSelector("div.content table[id='table-campaigns'] input[name*='USD']")).SendKeys(newProduct.CampaignPrice);
                    break;
                case "Euros":
                    driver.FindElement(By.CssSelector("div.content table[id='table-campaigns'] input[name*='EUR']")).Clear();
                    driver.FindElement(By.CssSelector("div.content table[id='table-campaigns'] input[name*='EUR']")).SendKeys(newProduct.CampaignPrice);
                    break;
                default:
                    break;
            }


            //Сохранение продукта
            driver.FindElement(By.CssSelector("#content form p span button[name='save']")).Click();

            //После сохранения товара нужно убедиться, что он появился в каталоге (в админке)
            IList<IWebElement> tProducts = driver.FindElements(By.CssSelector("table.dataTable tr.row td:nth-child(3) > a"));
            foreach (IWebElement tr in tProducts)
            {
                if (newProduct.Name == tr.GetAttribute("text"))
                {
                    System.Console.Out.Write("Продукт создан успешно");
                }


            }
        }

        public string GetProdCountInProductBox()
        {
            string product_count = driver.FindElement(By.CssSelector("div#cart span.quantity")).GetAttribute("textContent");
            return product_count;
        }

        [Test]
        public void Task07_01()
        {
            /* Задание 13. Сделайте сценарий работы с корзиной

              Сделайте сценарий для добавления товаров в корзину и удаления товаров из корзины.
              1) открыть главную страницу
              2) открыть первый товар из списка
              2) добавить его в корзину (при этом может случайно добавиться товар, который там уже есть, ничего страшного)
              3) подождать, пока счётчик товаров в корзине обновится
              4) вернуться на главную страницу, повторить предыдущие шаги ещё два раза, чтобы в общей 
              сложности в корзине было 3 единицы товара
              5) открыть корзину (в правом верхнем углу кликнуть по ссылке Checkout)
              6) удалить все товары из корзины один за другим, после каждого удаления подождать, пока 
              внизу обновится таблица

            */

            //Переход на главную страницу
            driver.Navigate().GoToUrl(baseURL + "/litecart/");
            ProductData ProductOnMainForm = new ProductData();
            ProductOnMainForm.Name =
              driver.FindElement(By.CssSelector("div#box-campaigns.box div.name")).GetAttribute("innerText");
            ProductOnMainForm.ProductLink =
              driver.FindElement(By.CssSelector("div#box-campaigns.box a.link")).GetAttribute("href");

            string prodCount = GetProdCountInProductBox();
            int prodCountUpd = 0;
            //Переход на страницу товара
            driver.Navigate().GoToUrl(ProductOnMainForm.ProductLink);
            //Добавляем товар в корзину


            //Проверить есть ли на странице элемент Size для данного товара
            for (int i = 0; i < 3; i++)
            {
                if (driver.FindElements(By.CssSelector("div.buy_now form[name='buy_now_form'] select")).Count() == 0)
                {
                    driver.FindElement(
                        By.CssSelector("div#box-product.box td.quantity button[name='add_cart_product']")).Click();

                    prodCountUpd = Convert.ToInt32(prodCountUpd) + 1;
                }
                else
                {
                    driver.FindElement(By.CssSelector("div.buy_now form[name='buy_now_form'] select[name='options[Size]']")).Click();
                    new SelectElement(driver.FindElement(By.CssSelector("div.buy_now form[name='buy_now_form'] select[name='options[Size]']"))).
                      SelectByText("Small");

                    driver.FindElement(
                        By.CssSelector("div#box-product.box td.quantity button[name='add_cart_product']")).Click();

                    prodCountUpd = Convert.ToInt32(prodCountUpd) + 1;
                }

            }
            //Ждем пока счетчик товаров обновится
            //driver.Navigate().Refresh();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(50));
            IWebElement element = driver.FindElement(By.CssSelector("div#cart span.quantity"));
            wait.Until(
                ExpectedConditions.TextToBePresentInElement(
                driver.FindElement(By.CssSelector("div#cart span.quantity")),
                Convert.ToString(prodCountUpd)));


            //Переход в корзину
            string vCheckOutURL =
              driver.FindElement(By.CssSelector("div#cart a.link")).GetAttribute("href");
            driver.Navigate().GoToUrl(vCheckOutURL);

            //Удаление товара из корзины
            //Сравниваем количество товаров с тем что в таблице
            string vTableCountRec = driver.FindElement(
              By.CssSelector("#order_confirmation-wrapper > table > tbody > tr:nth-child(2) > td:nth-child(1)"))
              .GetAttribute("textContent");
            if (vTableCountRec == Convert.ToString(prodCountUpd))
            {

                for (int rec = 0; rec < Convert.ToInt32(vTableCountRec); rec++)
                {
                    prodCountUpd--;
                    driver.FindElement(By.CssSelector("div.viewport input[name='quantity']")).Clear();
                    driver.FindElement(By.CssSelector("div.viewport input[name='quantity']")).SendKeys(Convert.ToString(prodCountUpd));

                    driver.FindElement(By.CssSelector("div.viewport button[name='update_cart_item']")).Click();
                }

            }

        }

        [Test]
        public void Task08_01()
        {
            /* Задание 14. Проверьте, что ссылки открываются в новом окне

            Сделайте сценарий, который проверяет, что ссылки на странице редактирования страны открываются в новом окне.
            Сценарий должен состоять из следующих частей:
            1) зайти в админку
            2) открыть пункт меню Countries (или страницу http://localhost/litecart/admin/?app=countries&doc=countries)
            3) открыть на редактирование какую-нибудь страну или начать создание новой
            4) возле некоторых полей есть ссылки с иконкой в виде квадратика со стрелкой -- они ведут на внешние страницы 
            и открываются в новом окне, именно это и нужно проверить.

            Конечно, можно просто убедиться в том, что у ссылки есть атрибут target="_blank". Но в этом упражнении требуется 
            именно кликнуть по ссылке, чтобы она открылась в новом окне, потом переключиться в новое окно, закрыть его, 
            вернуться обратно, и повторить эти действия для всех таких ссылок.

            Не забудьте, что новое окно открывается не мгновенно, поэтому требуется ожидание открытия окна.
            */

            //Вход
            app.Auth.LoginToAdmin();

            //Открываем раздел Countries
            IList<IWebElement> menuElements = driver.FindElements(By.CssSelector("ul#box-apps-menu li#app- span.name"));
            menuElements.ElementAt(2).Click();

            //Открываем страну
            ICollection<IWebElement> lsCountries =
              driver.FindElements(By.CssSelector("td#content table>tbody>tr.row td:nth-child(5) a"));
            if (lsCountries.Count != 0)
            {
                string link = lsCountries.ElementAt(0).GetAttribute("href");
                driver.Navigate().GoToUrl(link);
            }

            //Просматриваем внешные ссылки
            IList<IWebElement> ExternalLinks =
                driver.FindElements(By.CssSelector("tbody > tr > td#content a > i.fa.fa-external-link"));
            foreach (IWebElement el in ExternalLinks)
            {
                //Получаем ссылку элемента со стрелкой
                //IWebElement parent = el.FindElement(By.XPath(".."));
                //string exlnkOnCountriesForm = parent.GetAttribute("href");

                //Переход по внешней ссылке
                el.Click();
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                driver.SwitchTo().Window(driver.WindowHandles[driver.WindowHandles.Count - 1]);
                //Получаем ссылку из открытой вкладки
                string lnkOnOpenTab = driver.Url;

                //Закрытие активного окна и возврат на страницу Countries
                jse.ExecuteScript("window.close()");
            }

        }

        [Test]
        public void Task09_01()
        {
            /* Задание 17. Проверьте отсутствие сообщений в логе браузера
              Сделайте сценарий, который проверяет, не появляются ли в логе браузера сообщения 
              при открытии страниц в учебном приложении, а именно -- страниц товаров 
              в каталоге в административной панели.

              Сценарий должен состоять из следующих частей:
              1) зайти в админку
              2) открыть каталог, категорию, которая содержит товары (страница http://localhost/litecart/admin/?app=catalog&doc=catalog&category_id=1)
              3) последовательно открывать страницы товаров и проверять, не появляются ли в логе браузера сообщения (любого уровня)
            */

            //Вход
            app.Auth.LoginToAdmin();

            //Открываем раздел Countries
            IList<IWebElement> menuElements = driver.FindElements(By.CssSelector("ul#box-apps-menu li#app- span.name"));
            menuElements.ElementAt(1).Click();

            //Просмотр папок 
            while (driver.
              FindElements(By.CssSelector("td#content > form > table > tbody > tr.row td:nth-child(3) i.fa.fa-folder")).
              Count > 0)
            {
                IList<IWebElement> foldersElements = driver.
                  FindElements(By.CssSelector("td#content > form > table > tbody > tr.row td:nth-child(3) i.fa.fa-folder"));

                foreach (IWebElement el in foldersElements)
                {
                    el.FindElement(By.XPath("../a")).Click();
                    //driver.Navigate().Refresh();
                }
            }


            //Ссылки продуктов
            IList<IWebElement> ProductLinks =
                driver.FindElements(By.CssSelector("td#content > form > table > tbody > tr.row td:nth-child(3) a"));
            foreach (IWebElement pl in ProductLinks)
            {
                //Ссылка на страницу продукта
                string lnkOnOpenTab = pl.GetAttribute("href");
                string productName = pl.GetAttribute("text");

                if (lnkOnOpenTab.Contains("product") == true)
                {
                    //Переход на страницу продукта
                    IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                    jse.ExecuteScript("window.open()");
                    driver.SwitchTo().Window(driver.WindowHandles[driver.WindowHandles.Count - 1]);
                    driver.Navigate().GoToUrl(lnkOnOpenTab);

                    System.Console.Out.WriteLine("Product Name: " + productName);
                    System.Console.Out.WriteLine("Product URL: " + lnkOnOpenTab);
                    System.Console.Out.WriteLine("START -----------------------------");

                    //Просматриваем логи
                    foreach (LogEntry l in driver.Manage().Logs.GetLog("browser"))
                    {
                        System.Console.Out.WriteLine("Errors: " + l);
                    }
                    System.Console.Out.WriteLine("END -----------------------------" + "\n");


                    //Закрытие активного окна и возврат на страницу Countries
                    jse.ExecuteScript("window.close()");
                    driver.SwitchTo().Window(driver.WindowHandles[driver.WindowHandles.Count - 1]);
                }

            }


        }


     }


}
