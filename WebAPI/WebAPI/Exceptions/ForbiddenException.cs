using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message)
        {
            
        }
    }
}