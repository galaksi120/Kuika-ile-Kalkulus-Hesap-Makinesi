using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace Kuika.ThirdPartyApisCollection
{
     class matriscarpimi
{
    // String'i matrise dönüştüren fonksiyon
    static List<List<double>> StringdenMatriseDonustur(string metin)
    {
        string[] satirlar = metin.Split(new string[] { "\n" }, StringSplitOptions.None);
        List<List<double>> matris = new List<List<double>>();

        foreach (string satir in satirlar)
        {
            if (string.IsNullOrWhiteSpace(satir))
                continue;
            string[] elemanlar = satir.Split();
            List<double> satirListesi = new List<double>();
            foreach (string eleman in elemanlar)
            {
                satirListesi.Add(double.Parse(eleman));
            }
            matris.Add(satirListesi);
        }
        return matris;
    }

    // Matris yazdırma fonksiyonu
    static string MatrisYazdir(List<List<double>> sonuc)
    {
        string txt = "";
        foreach (var satir in sonuc)
        {
            txt += string.Join(" ", satir) + "\n";
        }
        return txt;
    }

    // Matris çarpımı fonksiyonu
    static List<List<double>> MatrisCarpimi(List<List<double>> matris1, List<List<double>> matris2)
    {
        // A'nın sütun sayısı ile B'nin satır sayısı eşit olmalı
        if (matris1[0].Count != matris2.Count)
        {
            throw new InvalidOperationException("Matrislerin boyutları çarpılmaya uygun değil.");
        }

        // Sonuç matrisini oluştur (A'nın satır sayısı, B'nin sütun sayısı kadar)
        List<List<double>> sonuc = new List<List<double>>();
        for (int i = 0; i < matris1.Count; i++)
        {
            sonuc.Add(new List<double>(new double[matris2[0].Count]));
        }

        // Matris çarpımını gerçekleştir
        for (int i = 0; i < matris1.Count; i++)  // A matrisinin satırları
        {
            for (int j = 0; j < matris2[0].Count; j++)  // B matrisinin sütunları
            {
                for (int k = 0; k < matris1[0].Count; k++)  // A matrisinin sütunları (B matrisinin satırları)
                {
                    sonuc[i][j] += matris1[i][k] * matris2[k][j];
                }
            }
        }

        return sonuc;
    }

    public static string matriscarpimisonuc(string matris1Str ,string matris2Str)
    {
        

        // Stringleri matrislere dönüştür
        var matris1 = StringdenMatriseDonustur(matris1Str);
        var matris2 = StringdenMatriseDonustur(matris2Str);

        // Matrislerin çarpımını hesapla
        
        
        var carpim = MatrisCarpimi(matris1, matris2);
        // Sonucu yazdır
            
        return MatrisYazdir(carpim);
        
        
    }
}
}
