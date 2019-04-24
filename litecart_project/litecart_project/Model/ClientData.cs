using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace litecart_project
{
  class ClientData : IEquatable<ClientData>, IComparable<ClientData>
  {
    public ClientData()
    {
    }

    public ClientData(string lastname)
    {
      LastName = lastname;
    }

    public bool Equals(ClientData other)
    {
      if (Object.ReferenceEquals(other, null))
      {
        return false;
      }

      if (Object.ReferenceEquals(this, other))
      {
        return true;
      }

      return LastName == other.LastName;

    }

    public int CompareTo(ClientData other)
    {
      if (Object.ReferenceEquals(other, null))
      {
        return 1;
      }

      return LastName.CompareTo(other.LastName);
    }

    public string FirstName { get; set; } //FirstName
    public string LastName { get; set; } //LastName
    public string Address1 { get; set; } //Address1
    public string Address2 { get; set; } //Address2
    public string Postcode { get; set; } //Postcode
    public string City { get; set; } //City
    public string Country { get; set; } //Country
    public string State { get; set; } //State
    public string Email { get; set; } //Email
    public string Phone { get; set; } //Phone
    public string Password { get; set; } //Password
    public string ConfirmPassword { get; set; } //ConfirmPassword
  }
}