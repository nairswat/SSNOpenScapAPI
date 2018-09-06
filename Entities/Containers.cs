using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSNOpenScap.API.Entities
{
    public class Containers
    {
        public Containers()
        {
            System.Guid guid = System.Guid.NewGuid();
            id = guid.ToString();
        }
        
        public String id = string.Empty;

        public string ContainerName { get; set; }
    }
}
