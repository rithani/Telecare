using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SrEngineerCodingExercise.Models;
using DataAccess.BO;
using DataAccess;
using System.Web.Http.Results;

namespace SrEngineerCodingExercise.Controllers
{
    [RoutePrefix("api/v1/user")]
    public class UserController : ApiController
    {
        //TODO:  Implement calls according to README
        // First Question
        [HttpGet]
        public Rootobject GetUserById(string id)
        {
            // 1. Get the person by id
            // 2. Get the boss by boss id of the person identified in step 1
            // 3. Get the address of person identified in step 1
            // 4. Build the user object

            // 1. Get the person by id
            Person employee = GetPersonById(Convert.ToInt32(id));
            // 2. Get the boss by boss id of the person identified in step 1
            Person boss = null;
            if (employee.BossId.HasValue)
            {
                boss = GetPersonById(Convert.ToInt32(employee.BossId.Value));
            }
            // 3. Get the address of person identified in step 1
            List<StreetAddress> addresses = GetAddressesById(Convert.ToInt32(id));
            // 4. Build the user object
            User user = ConvertToUserObject(employee, boss, addresses);

            Rootobject root = new Rootobject();
            root.User = user;

            return root;
        }

        [HttpGet]
        public FormattedContentResult<Emphierarchy> GetEmphierarchy(string id)
        {
            Emphierarchy rootEmployee = new Emphierarchy();
            Person person = GetPersonById(Convert.ToInt32(id));
            //person.GivenName + " " + person.FamilyName + " (" +  person.Title + ")"
            Emphierarchy emphierarchy = new Emphierarchy() { NameAndTitle = string.Format("{0} {1} ({2})", person.GivenName, person.FamilyName, person.Title) };
            emphierarchy.Reportees = GetReporteesOf(person.Id, 0);

            return Content(HttpStatusCode.OK, emphierarchy, Configuration.Formatters.XmlFormatter);
            //return emphierarchy;
        }


        [HttpGet]
        public string GetOrgMaxLevel()
        {
            int ceoId = 0;
            foreach (Person p in PersonDataAccess.PersonList)
            {
                // BossId = Null can be CEO as well..
                if (p.Title == "CEO")
                {
                    ceoId = p.Id;
                    break;
                }
            }

            List<Emphierarchy> empHierarlist = GetReporteesOf(ceoId, 0);
            List<Emphierarchy> flattenedList = new List<Emphierarchy>();
            FlattenHierarchy(empHierarlist, flattenedList);
            // Max in LINQ
            int maxLevel = (from v in flattenedList select v.GetLevel()).Max();
            return maxLevel.ToString();
        }

        private void FlattenHierarchy(List<Emphierarchy> inputList, List<Emphierarchy> outputList)
        {
            foreach(Emphierarchy hierarchy in inputList)
            {
                outputList.Add(hierarchy);
                if (hierarchy.Reportees != null && hierarchy.Reportees.Count > 0)
                {
                    FlattenHierarchy(hierarchy.Reportees, outputList);
                }
            }
        }



        private List<Emphierarchy> GetReporteesOf(int bossId, int level)
        {
            List<Emphierarchy> empHerarList = new List<Emphierarchy>();
            foreach (Person p in PersonDataAccess.PersonList)
            {
                if (p.BossId == bossId)
                {
                    Emphierarchy reportee = new Emphierarchy { NameAndTitle = string.Format("{0} {1} ({2})", p.GivenName, p.FamilyName, p.Title) };
                    reportee.SetLevel(level + 1);
                    empHerarList.Add(reportee);
                    List<Emphierarchy> reporteesHerarList = GetReporteesOf(p.Id, level + 1);
                    if (reporteesHerarList.Count > 0)
                    {
                        reportee.Reportees = reporteesHerarList;
                    }
                }
            }
            return empHerarList;
        }

        private Person GetPersonById(int id)
        {
            Person selectedPerson = null;
            foreach (Person p in PersonDataAccess.PersonList)
            {
                if (p.Id == id)
                {
                    selectedPerson = p;
                    break;
                }
            }
            return selectedPerson;
        }


        private List<StreetAddress> GetAddressesById(int personId)
        {
            List<StreetAddress> selectedAddressess = new List<StreetAddress>();
            foreach (StreetAddress p in StreetStreetAddressData.StreetList)
            {
                if (p.PersonId == personId)
                {
                    selectedAddressess.Add(p);
                }
            }
            return selectedAddressess;
        }

        private User ConvertToUserObject(Person employee, Person boss, List<StreetAddress> addresses)
        {
            User user = new Models.User();
            user.Id = employee.Id;
            user.FirstName = employee.GivenName;
            user.LastName = employee.FamilyName;
            user.Gender = employee.Gender.ToString();
            user.DateOfBirth = employee.DateOfBirth.ToString("yyyy-MM-dd");
            user.Title = employee.Title;
            if (boss != null)
            {
                user.BossName = boss.GivenName + " " + boss.FamilyName;
            }
            List<Address> addressList = new List<Address>();
            foreach (StreetAddress strAddress in addresses)
            {
                Address address = new Address();
                address.City = strAddress.City;
                address.Id = strAddress.Id;
                address.State = strAddress.State;
                address.Street = strAddress.Street;
                address.Zip = strAddress.Zip;
                addressList.Add(address);
            }

            // From Address List (Generic List) to Address Array
            user.Addresses = addressList.ToArray();
            return user;
        }

    }
}
