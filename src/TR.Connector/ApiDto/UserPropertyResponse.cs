using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TR.Connector.ApiDto
{
    internal class UserPropertyData
    {
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string telephoneNumber { get; set; }
        public bool isLead { get; set; }
        public string login { get; set; }
        public string status { get; set; }
    }

    internal class UserPropertyResponse
    {
        public UserPropertyData data { get; set; }
        public bool success { get; set; }
        public object errorText { get; set; }
        public int count { get; set; }
    }
}
