using Amazon.EC2.Model;

namespace StratusApp.Models
{
    public class StratusUser
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public static implicit operator StratusUser(List<Instance> v)
        {
            throw new NotImplementedException();
        }
    }
}
