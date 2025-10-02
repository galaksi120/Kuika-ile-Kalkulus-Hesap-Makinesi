
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace Kuika.ThirdPartyApisCollection
{
    class matrisintersiislem
{
    public static string matristerssonuc(string matris1Str)
    {
        

        // Stringleri matrislere dönüştür
        List<List<double>> matris1 = StringdenMatriseDonustur(matris1Str);

        // Matrislerin tersini hesapla
        
        
        var ters = GaussEliminationInverse(matris1);
        
        return MatrisYazdir(ters);
        
        
    }

    static List<List<double>> StringdenMatriseDonustur(string metin)
    {
        string[] l = metin.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        List<List<double>> matris = new List<List<double>>();

        foreach (var i in l)
        {
            var row = i.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<double> rowList = new List<double>();
            foreach (var item in row)
            {
                rowList.Add(double.Parse(item));
            }
            matris.Add(rowList);
        }
        return matris;
    }

    static string MatrisYazdir(List<List<double>> sonuc)
    {
        string txt = "";
        foreach (var satir in sonuc)  // Her satırı tek tek dolaş
        {
            foreach (var sayi in satir)  // Satırdaki her sayıyı dolaş
            {
                // Sayıyı 2 ondalıklı formatta yazdırıyoruz
                txt += sayi.ToString("F4") + " ";
            }
            // Satır sonu ekliyoruz
            txt = txt.TrimEnd() + "\n";  // Sonundaki gereksiz boşluğu temizle ve satır sonu ekle
        }
        return txt;
    }

    static List<List<double>> GaussEliminationInverse(List<List<double>> matrix)
    {
        int n = matrix.Count;

        // Birim matris oluştur
        List<List<double>> identityMatrix = new List<List<double>>();
        for (int i = 0; i < n; i++)
        {
            List<double> row = new List<double>();
            for (int j = 0; j < n; j++)
            {
                row.Add(i == j ? 1 : 0);
            }
            identityMatrix.Add(row);
        }

        // Genişletilmiş matris: [matris | birim matris]
        List<List<double>> augmentedMatrix = new List<List<double>>();
        for (int i = 0; i < n; i++)
        {
            List<double> row = new List<double>(matrix[i]);
            row.AddRange(identityMatrix[i]);
            augmentedMatrix.Add(row);
        }

        // Gauss eliminasyonu işlemi
        for (int i = 0; i < n; i++)
        {
            // Eğer pivot sıfırsa, sıralama yap
            if (augmentedMatrix[i][i] == 0)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (augmentedMatrix[j][i] != 0)
                    {
                        var temp = augmentedMatrix[i];
                        augmentedMatrix[i] = augmentedMatrix[j];
                        augmentedMatrix[j] = temp;
                        break;
                    }
                }
            }

            // Pivot elemanını 1 yap
            double pivot = augmentedMatrix[i][i];
            for (int k = 0; k < 2 * n; k++)
            {
                augmentedMatrix[i][k] /= pivot;
            }

            // Diğer satırlarda sıfır yapmak
            for (int j = 0; j < n; j++)
            {
                if (j != i)
                {
                    double factor = augmentedMatrix[j][i];
                    for (int k = 0; k < 2 * n; k++)
                    {
                        augmentedMatrix[j][k] -= factor * augmentedMatrix[i][k];
                    }
                }
            }
        }

        // Ters matrisi ayıkla
        List<List<double>> inverseMatrix = new List<List<double>>();
        for (int i = 0; i < n; i++)
        {
            List<double> row = new List<double>();
            for (int j = n; j < 2 * n; j++)
            {
                row.Add(augmentedMatrix[i][j]);
            }
            inverseMatrix.Add(row);
        }

        return inverseMatrix;
    }
}
}
