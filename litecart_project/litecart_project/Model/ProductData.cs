using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;


namespace litecart_project
{
  class ProductData : IEquatable<ProductData>, IComparable<ProductData>
  {
    public ProductData()
    {
    }

    public ProductData(string name)
    {
      Name = name;
    }

    public bool Equals(ProductData other)
    {
      if (Object.ReferenceEquals(other, null))
      {
        return false;
      }

      if (Object.ReferenceEquals(this, other))
      {
        return true;
      }

      return Name == other.Name;

    }

    public int CompareTo(ProductData other)
    {
      if (Object.ReferenceEquals(other, null))
      {
        return 1;
      }

      return Name.CompareTo(other.Name);
    }

    public string Name { get; set; } //Наименование продукта
    public string Code { get; set; } //Код продукта
    public string Quantity { get; set; } //Количество продукта
    public string ProductLink { get; set; } //Ссылка на страницу продукта
    public string RegularPrice { get; set; } //Постоянная цена
    public string CampaignPrice { get; set; } //Цена со скидкой
    public string RegularPriceColor { get; set; } //Цвет постоянной цены
    public string RegularPriceFontSize { get; set; } //Размер шрифта постоянной цены
    public string CampaignPriceColor { get; set; } //Цвет цены со скидкой
    public string CampaignPriceFontSize { get; set; } //Размер шрифта цены со скидкой
    public string RegularPriceFontStrike { get; set; } //Зачеркнутый шрифт для постоянной цены
    public string CampaignPriceFontStrike { get; set; } //Зачеркнутый шрифт для цены со скидкой
    public string RegularPriceFontBold { get; set; } //Шрифт для постоянной цены жирный
    public string CampaignPriceFontBold { get; set; } //Шрифт для цены со скидкой жирный


  }
}
