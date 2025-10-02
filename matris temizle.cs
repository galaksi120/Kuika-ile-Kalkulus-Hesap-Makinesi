
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace Kuika.ThirdPartyApisCollection
{
    public class matrisclean
    {
        public static string matristemizleme (string matris1)
        { 
            return matris1.Substring(0,matris1.Length-matris1.Length);
        }
    }
}
