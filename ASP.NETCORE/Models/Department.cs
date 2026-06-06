using System.Text.Json.Serialization;

namespace ASP.NETCORE.Models
{
    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public List<Student>? Students { get; set; }
            = [];
    }
}
