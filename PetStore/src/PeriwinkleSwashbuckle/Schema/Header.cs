using Swashbuckle.Swagger.Model;

namespace Periwinkle.Swashbuckle
{
    public class PartialHeader : PartialSchema
    {
        public PartialHeader()
        {
            required = false;
        }

        public string name { get; set; }

        public bool? required { get; set; }
    }
}
