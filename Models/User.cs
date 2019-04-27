using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SrEngineerCodingExercise.Models
{
    public class Rootobject
    {
        public User User { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BossName { get; set; }
        public string Title { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public Address[] Addresses { get; set; }
    }

    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        //public string ZipCode { get; set; }
    }

}