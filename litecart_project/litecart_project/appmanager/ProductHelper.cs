using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;


namespace litecart_project
{
    public class ProductHelper : HelperBase
    {
        public ProductHelper(ApplicationManager manager) : base(manager)
        {
        }

        public string GetProdCountInProductBox()
        {
            string product_count = driver.FindElement(By.CssSelector("div#cart span.quantity")).GetAttribute("textContent");
            return product_count;
        }

        public int AddProductToBasket(int vAddProductCount)
        {
            //Добавляем товар в корзину
            //Проверить есть ли на странице элемент Size для данного товара
            string prodCount = GetProdCountInProductBox();
            int prodCountUpd = 0;
            for (int i = 0; i < vAddProductCount; i++)
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

            return prodCountUpd;
        }

        public void RemoveProductFromBasket(int prodCountUpd)
        {
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


    }
}
