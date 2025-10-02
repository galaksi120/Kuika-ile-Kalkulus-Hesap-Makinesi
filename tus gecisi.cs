using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace Kuika.ThirdPartyApisCollection
{
     public class isaret
    {
        public static string ekleisaret(string text)
        {
            
            if (text=="?")
            {
                return "}" ;
            }
            else
            {
                return "?" ;
            }

        }
    }
}
