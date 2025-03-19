using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project212.Models;

namespace Project212
{
    public static class UserSession
    {
        public static Account? CurrentUser { get; set; }
    }

}
