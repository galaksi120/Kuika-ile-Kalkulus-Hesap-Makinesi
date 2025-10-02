
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace Kuika.ThirdPartyApisCollection
{
     class matrisfarkiislem
{
    public static string matriscikarmasonuc(string matris1Str,string matris2Str)
    {
        

        // Stringleri matrislere dönüştür
        var matris1 = StringdenMatriseDonustur(matris1Str);
        var matris2 = StringdenMatriseDonustur(matris2Str);

        // Matrislerin toplamını hesapla
        
        
        var fark = MatrisCikarma(matris1, matris2);
            
        return MatrisYazdir(fark, matris1);
        
        
    }

    static List<List<string>> StringdenMatriseDonustur(string metin)
    {
        var l = metin.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var matris = new List<List<string>>();
        foreach (var i in l)
        {
            matris.Add(new List<string>(i.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)));
        }
        return matris;
    }

    static List<double> MatrisCikarma(List<List<string>> matris1, List<List<string>> matris2)
    {
        // Matrislerin boyutlarının aynı olup olmadığını kontrol et
        if (matris1.Count != matris2.Count || matris1[0].Count != matris2[0].Count)
        {
            throw new ArgumentException("Matrisler aynı boyutta olmalıdır.");
        }

        var toplam = new List<double>();
        for (int i = 0; i < matris1.Count; i++)
        {
            for (int j = 0; j < matris1[0].Count; j++)
            {
                toplam.Add(double.Parse(matris1[i][j]) - double.Parse(matris2[i][j]));
            }
        }

        return toplam;
    }

    static string MatrisYazdir(List<double> toplam, List<List<string>> matris1)
    {
        int satir = matris1.Count;
        int sutun = matris1[0].Count;
        var tablo = new List<List<double>>();

        for (int i = 0; i < satir; i++)
        {
            var row = toplam.GetRange(i * sutun, sutun);
            tablo.Add(row);
        }

        string txt = "";
        foreach (var i in tablo)
        {
            txt += string.Join(" ", i) + "\n";
        }
        return txt;
    }
}
}
