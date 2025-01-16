using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020880.DomainModels
{
    public class UserAccount
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RoleNames { get; set; }
        public string Photo { get; set; }
    }
}
