using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunstallBL.Helpers;

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var token = JWTHelper.GetToken("58F5D7947E9B7E68CDB00B5DF1E3679693CA14D8A7A9F14C8795CCE0A5", "testUser");

            //var Username = JWTHelper.DecodeToken(token);
            

            Console.Read();
        }
    }
}
