using Microservices.ExternalServices.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microservices
{
    static class Program
    {
        public static void Main(string[] args)
        {
            new CatShelterService(null, new AuthorizationService(), null, null, null).AddCatAsync(null, null, default).Wait();
        }
    }
}
