using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;


namespace litecart_project
{
    public class LoginHelper : HelperBase
    {
        public LoginHelper(ApplicationManager manager) : base(manager)
        {
        }


        public void LoginToAdmin()
        {
            manager.Navigator.AdminHomePage();

            Type(By.Name("username"), "admin");
            Type(By.Name("password"), "admin");
            driver.FindElement(By.Name("login")).Click();

            //wait.Until(ExpectedConditions.TitleIs("My Store"));
        }

        public void LoginToLitecartShop()
        {
            manager.Navigator.ShopHomePage();
            //Вход под клиентом
        }
    }
}
