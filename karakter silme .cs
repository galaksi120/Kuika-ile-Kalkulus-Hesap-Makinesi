using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace Kuika.ThirdPartyApisCollection
{
     public class textekleme3
    {
        public static string eklemetin3(string text,string ekle)
        {
            
            if (ekle==";")
            {
                return text.Substring(0,text.Length-1);
            }
            else if (ekle=="$")
            {
                return text.Substring(0,text.Length-text.Length);
            }
            else
            {
                text=text+ekle;return text;
            }
