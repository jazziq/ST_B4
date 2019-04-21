using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using LinqToDB.Mapping;


namespace litecart_project
{
    [Table(Name = "lc_countries")]

    public class CountriesData : IEquatable<CountriesData>, IComparable<CountriesData>
    {

        public CountriesData()
        {
        }

        public CountriesData(string name)
        {
            Name = name;
        }

        public bool Equals(CountriesData other)
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

        public int CompareTo(CountriesData other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return 1;
            }

            return Name.CompareTo(other.Name);
        }


        [Column(Name = "id"), PrimaryKey, Identity]
        public string Id { get; set; } //Идентификатор страны

        [Column(Name = "name")]
        public string Name { get; set; } //Имя страны

        [Column(Name = "iso_code_2")]
        public string Code { get; set; } //Код страны



        public static List<String> GetCountriesFromDB()
        {
            using (LitecartDB db = new LitecartDB())
            {
                return (from g in db.Countries
                        orderby g.Name ascending
                        select g.Name
                        ).ToList();
            }
        }


    }
}
