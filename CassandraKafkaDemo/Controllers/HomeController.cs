using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CassandraKafkaDemo.Models;
using KafkaInterface;
using Cassandra;
using Newtonsoft.Json;

namespace CassandraKafkaDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            List<PersonModel> persons = GetPersons();
            return View(persons);
        }

        public List<PersonModel> GetPersons()
        {
            var cluster = Cluster.Builder().AddContactPoint("ubuntu-server").Build();
            var session = cluster.Connect("people");
            var rs = session.Execute("SELECT * FROM persons");

            List<PersonModel> persons = new List<PersonModel>();

            foreach(var row in rs)
            {
                persons.Add(new PersonModel()
                {
                    Id = row.GetValue<Guid>("id"),
                    Name = row.GetValue<string>("name"),
                    Email = row.GetValue<string>("email"),
                    Phone = row.GetValue<int>("phone"),
                    IsActive = row.GetValue<bool>("isactive")
                });
            }

            return persons;
        }

        public IActionResult CreatePerson(string personName, string email, int phone)
        {
            ContentProducer cp = new ContentProducer("my-topic");

            var person = new PersonModel()
            {
                Id = Guid.NewGuid(),
                Name = personName,
                Email = email,
                Phone = phone,
                IsActive = true
            };

            string json = JsonConvert.SerializeObject(person);
            cp.SendMessage(json);

            var cluster = Cluster.Builder().AddContactPoint("ubuntu-server").Build();
            var session = cluster.Connect("people");

            var ps = session.Prepare("INSERT INTO persons (id, name, email, phone, isactive) VALUES (?,?,?,?,?)");
            var statement = ps.Bind(Guid.NewGuid(), personName, email, phone, true);

            var rs = session.Execute(statement);
            
            return Json(rs != null ? "success" : "error");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
