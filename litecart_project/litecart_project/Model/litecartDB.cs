using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB;

namespace litecart_project
{
  public class LitecartDB : LinqToDB.Data.DataConnection
  {
    public LitecartDB() : base("litecart") { }
    public ITable<CountriesData> Countries { get { return GetTable<CountriesData>(); } }
  }
}
