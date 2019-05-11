using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;


namespace litecart_project
{
    public class NavigationHelper : HelperBase
    {
        private string baseURL;

        public NavigationHelper(ApplicationManager manager, string baseURL) : base(manager)
        {
            this.baseURL = baseURL;
        }


        public void AdminHomePage()
        {
            //Go to home page
            if (driver.Url == baseURL + "/litecart/admin/login.php")
            {
                return;
            }

            driver.Navigate().GoToUrl(baseURL + "/litecart/admin/login.php");
        }

        public void ShopHomePage()
        {
            //Go to home page
            if (driver.Url == baseURL + "/litecart/")
            {
                return;
            }

            driver.Navigate().GoToUrl(baseURL + "/litecart/");
        }

        public void GoToProductPage()
        {
            //Переход на страницу товара
            ProductData ProductOnMainForm = new ProductData();
            ProductOnMainForm.Name =
              driver.FindElement(By.CssSelector("div#box-campaigns.box div.name")).GetAttribute("innerText");
            ProductOnMainForm.ProductLink =
              driver.FindElement(By.CssSelector("div#box-campaigns.box a.link")).GetAttribute("href");

            driver.Navigate().GoToUrl(ProductOnMainForm.ProductLink);
        }

        public void GoToBasketPage()
        {
            string vCheckOutURL = 
                driver.FindElement(By.CssSelector("div#cart a.link")).GetAttribute("href");
            driver.Navigate().GoToUrl(vCheckOutURL);

        }


    }
}
