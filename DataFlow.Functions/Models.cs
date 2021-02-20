using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DataFlow.Functions {

    public class DataEntry {

        [JsonProperty("id")]
        public string Id { get; set; }

        public int Index { get; set; }

        public Guid Guid { get; set; }

        public bool IsActive { get; set; }

        public string Balance { get; set; }

        public string Picture { get; set; }

        public int Age { get; set; }

        public string EyeColor { get; set; }

        public Name Name { get; set; }

        public string Company { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string About { get; set; }

        public string Registered { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string[] Tags { get; set; }

        public int[] Range { get; set; }

        public Friend[] Friends { get; set; }

        public string Greeting { get; set; }

        public string FavoriteFruit { get; set; }
    }

    public class Name {

        public string First { get; set; }

        public string Last { get; set; }
    }

    public class Friend {

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
