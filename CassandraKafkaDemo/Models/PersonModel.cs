using System;

namespace CassandraKafkaDemo.Models
{
    public class PersonModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Phone { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
