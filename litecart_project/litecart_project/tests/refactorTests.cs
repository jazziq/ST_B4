using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace litecart_project
{
    class refactorTests : TestBase
    {
        [Test]
        public void Task10_01()
        {
            /* Задание 19. Реализовать многослойную архитектуру

             Переделайте созданный в задании 13 сценарий для добавления товаров в корзину 
             и удаления товаров из корзины, чтобы он использовал многослойную архитектуру.

             А именно, выделите вспомогательные классы 
             - для работы с главной страницей (откуда выбирается товар), 
             - для работы со страницей товара (откуда происходит добавление товара в корзину), 
             - со страницей корзины (откуда происходит удаление), 
             и реализуйте сценарий, который не напрямую обращается к операциям Selenium, 
             а оперирует вышеперечисленными объектами-страницами.
            */

            //Переход на главную страницу
            app.Auth.LoginToLitecartShop();
            
            //Переход на страницу товара
            app.Navigator.GoToProductPage();

            //Добавление продукта в корзину (количество добавляемого товара)
            int prodCountUpd = app.Product.AddProductToBasket(3);

            //Переход в корзину
            app.Navigator.GoToBasketPage();

            //Удаление товара из корзины
            app.Product.RemoveProductFromBasket(prodCountUpd);
        }
    }
       
}
