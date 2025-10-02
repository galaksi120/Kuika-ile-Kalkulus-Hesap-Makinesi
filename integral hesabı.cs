
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace Kuika.ThirdPartyApisCollection
{
class Program
{
    static string IntegrateLogarithm(double coeff, string inner)
    {
        // Eğer '*' karakteri varsa, inner'dan katsayıyı ayıklıyoruz.
        double k3 = 1;
        if (inner.Contains("-x"))
        {
            k3=-1;
        }
        if (inner.Contains("*"))
        {
            // '(' ve ')' karakterlerini temizleyip '*' ile ayırıyoruz ve ilk kısmı alıyoruz.
            k3 = double.Parse(inner.Replace("(", "").Replace(")", "").Split('*')[0]);
        }
        if (inner.Contains("/"))
        {
            double k31;double k32;
            try
            {
                k31=double.Parse(inner.Replace("(", "").Replace(")", "").Split('/')[0].Split("*")[0]);
            }
            catch
            {
                k31=1;
            }
            k32=double.Parse(inner.Replace("(", "").Replace(")", "").Split('/')[1]);
            k3=k31/k32;
        }

        // Sonuçları formatlı bir şekilde döndürüyoruz.
        return $"{coeff * 1/k3}*({inner.Replace("+-","-")})*ln({inner.Replace("+-","-")}) - {coeff * 1/k3}*({inner.Replace("+-","-")})";
    }
    static string IntegrateLogarithmbaseten(double coeff, string inner)
    {
        // Eğer '*' karakteri varsa, inner'dan katsayıyı ayıklıyoruz.
        double k3 = 1;
        if (inner.Contains("-x"))
        {
            k3=-1;
        }
        if (inner.Contains("*"))
        {
            // '(' ve ')' karakterlerini temizleyip '*' ile ayırıyoruz ve ilk kısmı alıyoruz.
            k3 = double.Parse(inner.Replace("(", "").Replace(")", "").Split('*')[0]);
        }
        if (inner.Contains("/"))
        {
            double k31;double k32;
            try
            {
                k31=double.Parse(inner.Replace("(", "").Replace(")", "").Split('/')[0].Split("*")[0]);
                
            }
            catch
            {
                k31=1;
            }
            k32=double.Parse(inner.Replace("(", "").Replace(")", "").Split('/')[1]);
            k3=k31/k32;
        }

        // Sonuçları formatlı bir şekilde döndürüyoruz.
        return $"(1/ln(10))*{coeff * 1/k3}*({inner.Replace("+-","-")})*ln({inner.Replace("+-","-")}) - {coeff * 1/k3}*({inner.Replace("+-","-")})";
    }
    
    static string IntegratePolynomial(double coeff, double exp)
    {
        // Yeni katsayı ve üssü hesapla
        double newCoeff = coeff / (exp + 1);
        double newExp = exp + 1;
        
        // Sonucu string formatında döndür
        return $"{newCoeff}*x^{newExp}";
    }
    
    static string IntegrateExponential(double coeff, string inner)
    {
        double k1 = 1;
        if (inner.Contains("-x"))
        {
            inner=inner.Replace("-x","-1*x");
        }
    
        // Eğer '*' varsa, inner'dan katsayıyı ayıklıyoruz.
        if (inner.Contains("*"))
        {
            k1 = double.Parse(inner.Replace("(", "").Replace(")", "").Split('*')[0]);
        }
        if (inner.Contains("/"))
        {
            double k11;double k12;
            try
            {
                k11=double.Parse(inner.Replace("(", "").Replace(")", "").Split('/')[0].Split("*")[0]);
                if ((inner.Replace("(", "").Replace(")", "").Split('/')[0]).Contains("-x"))
                {
                    k11=-1;
                }
            }
            catch
            {
                k11=1;
            }
            k12=double.Parse(inner.Replace("(", "").Replace(")", "").Split('/')[1]);
            k1=k11/k12;
        }
    
        // Sonucu formatlı şekilde döndür.
        return $"{coeff * 1/k1}*e^{inner.Replace("+-","-")}";
    }
    static string IntegrateTrigonometric(string inner, string funcType, double coeff = 1)
    {
        double k2 = 1;
        if (inner.Contains("-x"))
        {
            inner=inner.Replace("-x","-1*x");
        }
        
        // Eğer '*' varsa, inner'dan katsayıyı ayıklıyoruz.
        if (inner.Contains("*"))
        {
            k2 = double.Parse(inner.Replace("(", "").Replace(")", "").Split('*')[0]);
        }
        if (inner.Contains("/"))
        {
            double k21;double k22;
            try
            {
                k21=double.Parse(inner.Replace("(", "").Replace(")", "").Split('/')[0].Split("*")[0]);
            }
            catch
            {
                k21=1;
            }
            k22=double.Parse(inner.Replace("(", "").Replace(")", "").Split('/')[1]);
            k2=k21/k22;
        }

        // Trigonometrik fonksiyon türüne göre integral hesaplama
        if (funcType == "sin")
        {
            return (coeff * k2 != 1) ? $"{-coeff * 1/k2}*cos({inner.Replace("+-","-")})" : $"-cos({inner.Replace("+-","-")})";
        }
        else if (funcType == "cos")
        {
            return (coeff * k2 != 1) ? $"{coeff * 1/k2}*sin({inner.Replace("+-","-")})" : $"sin({inner.Replace("+-","-")})";
        }
        else if (funcType == "tan")
        {
            return (coeff * k2 != 1) ? $"{-coeff * 1/k2}*ln|cos({inner.Replace("+-","-")})|" : $"-ln|sec({inner.Replace("+-","-")})|";
        }
        else if (funcType == "sec")
        {
            return (coeff * k2 != 1) ? $"{coeff * 1/k2}*ln|sec({inner.Replace("+-","-")}) + tan({inner.Replace("+-","-")})|" : $"ln|sec({inner.Replace("+-","-")}) + tan({inner.Replace("+-","-")})|";
        }
       else if (funcType == "csc")
        {
            return (coeff * k2 != 1) ? $"{coeff * 1/k2}*ln|csc({inner.Replace("+-","-")}) - cot({inner.Replace("+-","-")})|" : $"ln|csc({inner.Replace("+-","-")}) - cot({inner.Replace("+-","-")})|";
        }
        else if (funcType == "cot")
        {
            return (coeff * k2 != 1) ? $"{coeff * 1/k2}*ln|sin({inner.Replace("+-","-")})|" : $"ln|sin({inner.Replace("+-","-")})|";
        }


        return null;
    }
    static string IntegrateConstant(string term)
    {
        return $"{term}*x";
    }
    static string HandleRationalFunctions(string term,string coeff1)
    {
        term = term.Replace(" ", "");  // Boşlukları temizle

        if (term == "1/x")
        {
            if (Convert.ToDouble(coeff1)==1)
                {return "ln|x|";}
            else
                {return $"{coeff1}*ln|x|";}
        }

        if (term.StartsWith("1/("))
        {
            string inner = term.Substring(2);  // '1/' kısmını atla (2. karakterden başla)
            string pinner = inner.Replace("(", "").Replace(")", "");  // Parantezleri temizle

            // 1/(a*x) durumu
            if (inner.Contains("*x"))
            {
                string coeff = pinner.Split('*')[0];  // Katsayıyı ayıklıyoruz
                return $"{coeff1}/{coeff}*ln|{pinner.Replace("+-","-")}|";
            }

            // 1/(x+b) veya 1/(x-b) durumu
            if (inner.Contains("x") && (inner.Contains("+") || inner.Contains("-")))
            {
                // 'x + b' veya 'x - b' formunda, iç kısımdan "+b" veya "-b" gibi terimi alıyoruz
                string parts = pinner.Split('x')[1].Trim();  // 'x' sonrası kısmı alıyoruz
                if (Convert.ToDouble(coeff1)==1)
                {
                    return $"ln|x {parts}|";
                }
                else
                {    return $"{coeff1}*ln|x {parts}|";  
                    
                }
                
            }

            
        }

        return null;
    }
    static (double, object) GetCoeffAndExponent(string term)
    {
        term = term.Trim();  // Boşlukları temizle

        double coeff = 1;  // Varsayılan katsayı
        if (term.Contains("*"))
        {
            string[] parts = term.Split('*');
            
            double c1,c2;
            try 
            {
                c1=Convert.ToDouble(parts[0].Replace("(","").Replace(")","").Split("/")[0]);
                c2=Convert.ToDouble(parts[0].Replace("(","").Replace(")","").Split("/")[1]);
                coeff=c1/c2;
            }
            catch 
            {
                coeff = double.Parse(parts[0].Trim());
            }
            term = parts[1].Trim();  // Katsayıdan sonraki terimi al
        }

        if (term.Contains("e^"))
        {
            string inner = term.Split(new string[] { "e^" }, StringSplitOptions.None)[1].Trim();
            return (coeff, inner);  // Inner ile beraber döner
        }
        else if (term.Contains("x"))
        {
            double exponent;
            if (term.Contains("^"))
            {   double exp1,exp2;
                string[] parts = term.Split(new string[] { "x^" }, StringSplitOptions.None);
                try 
                {
                    
                    exp1=Convert.ToDouble(parts[1].Replace("(","").Replace(")","").Split("/")[0]);
                    exp2=Convert.ToDouble(parts[1].Replace("(","").Replace(")","").Split("/")[1]);
                    exponent=exp1/exp2;
                }
                catch
                {
                    exponent = double.Parse(parts[1].Replace("(","").Replace(")","").Trim());
                }
                
            }
            else
            {
                exponent = 1;  // x için
            }
            return (coeff, exponent);
        }
        else
        {
            return (coeff, 0);  // Sabit terim
        }
    }

    static (string, string) ExtractInnerAndFuncType(string term)
    {
        string inner;
        if (term.Contains("sin("))
        {   
            try
            {
                inner = term.Substring(term.IndexOf("sin(") + 4, term.LastIndexOf(")*") - term.IndexOf("sin(") - 4);
            }
            catch
            {
                inner = term.Substring(term.IndexOf("sin(") + 4, term.LastIndexOf(")") - term.IndexOf("sin(") - 4);
            }
            return (inner, "sin");
        }
        else if (term.Contains("cos("))
        {
            try
            {
                inner = term.Substring(term.IndexOf("cos(") + 4, term.LastIndexOf(")*") - term.IndexOf("cos(") - 4);
            }
            catch
            {
                inner = term.Substring(term.IndexOf("cos(") + 4, term.LastIndexOf(")") - term.IndexOf("cos(") - 4);
            }
            
            return (inner, "cos");
        }
        else if (term.Contains("tan("))
        {
            try
            {
                inner = term.Substring(term.IndexOf("tan(") + 4, term.LastIndexOf(")*") - term.IndexOf("tan(") - 4);
            }
            catch
            {
                inner = term.Substring(term.IndexOf("tan(") + 4, term.LastIndexOf(")") - term.IndexOf("tan(") - 4);
            }
            
            return (inner, "tan");
        }
        else if (term.Contains("sec("))
        {
            try 
            {
                inner = term.Substring(term.IndexOf("sec(") + 4, term.LastIndexOf(")*") - term.IndexOf("sec(") - 4);
            }
            catch
            {
                inner = term.Substring(term.IndexOf("sec(") + 4, term.LastIndexOf(")") - term.IndexOf("sec(") - 4);
            }
            
            return (inner, "sec");
        }
        else if (term.Contains("csc("))
        {
            
            try
            {
                inner = term.Substring(term.IndexOf("csc(") + 4, term.LastIndexOf(")*") - term.IndexOf("csc(") - 4);
            }
            catch 
            {
                inner = term.Substring(term.IndexOf("csc(") + 4, term.LastIndexOf(")") - term.IndexOf("csc(") - 4);
            }
            return (inner, "csc");
        }
        else if (term.Contains("cot("))
        {
            try 
            {
                inner = term.Substring(term.IndexOf("cot(") + 4, term.LastIndexOf(")*") - term.IndexOf("cot(") - 4);
            }
            catch
            {
                inner = term.Substring(term.IndexOf("cot(") + 4, term.LastIndexOf(")") - term.IndexOf("cot(") - 4);
            }
            
            return (inner, "cot");
        }

        return (null, null);
    }
    static (double, string) ExtractCoeffAndInner(string term)
    {
        double coeff = 1;
        string inner = term;

        if (term.Contains("*"))
        {
            string[] parts = term.Split('*');
            
            double c1=1;double c2=1;
            try
            {
                c1=Convert.ToDouble(parts[0].Split("/")[0].Replace("(","").Replace(")",""));
                c1=Convert.ToDouble(parts[0].Split("/")[1].Replace("(","").Replace(")",""));
                coeff=c1/c2;
                
            }
            catch
            {
                coeff = double.Parse(parts[0].Trim());
            }
            inner = parts[1].Trim();
        }

        return (coeff, inner);
    }
    static List<string> SplitTerms(string function)
    {
        List<string> terms = new List<string>();
        string term = "";
        int parens = 0;
        double s=0;

        foreach (char ch in function)
        {
            if (ch == '+' && parens == 0)
            {
                if (!string.IsNullOrWhiteSpace(term)) // Boş terimleri eklememek için kontrol
                {
                    terms.Add(term.Trim().Replace("+-","-"));
                }
                term = "";
            }
            else
            {
                term += ch;
                Console.WriteLine(term);
                if (ch == '(')
                {
                    parens++;
                }
                else if (ch == ')')
                {
                    parens--;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(term)) // Son terimi ekle
        {
            terms.Add(term.Trim());
        }

        return terms;
    }
    
    static string IntegrateRationalPolynomial(double coeff, string inner, double exp)
    {   
        // (a*x + b)^(n+1) / (a * (n+1)) formülü
        double newExp = exp + 1;
        
        double k5 = 1;
        if ((inner.Contains("-("))&&(!inner.Contains("*")))
        {
            // '(' ve ')' karakterlerini temizleyip '*' ile ayırıyoruz ve ilk kısmı alıyoruz.
            coeff = -1;
        }
        if ((inner.Contains("(-"))&&(!inner.Contains("*")))
        {
            // '(' ve ')' karakterlerini temizleyip '*' ile ayırıyoruz ve ilk kısmı alıyoruz.
            k5 = -1;
        }
        if (inner.Contains("*"))
        {
            // '(' ve ')' karakterlerini temizleyip '*' ile ayırıyoruz ve ilk kısmı alıyoruz.
            k5 = double.Parse(inner.Replace("(", "").Replace(")", "").Split('*')[0]);
        }
        if (inner.Contains("/"))
        {
            double k51;double k52;
            try
            {
                k51=double.Parse(inner.Replace("(", "").Replace(")", "").Split('/')[0].Split("*")[0]);
            }
            catch
            {
                k51=1;
            }
            k52=double.Parse(inner.Replace("(", "").Replace(")", "").Split('/')[1]);
            k5=k51/k52;
        }
        inner=inner.Replace("-(","").Replace("(","").Replace(")","");
        return $"({inner})^{newExp} / ({k5*(1/coeff) * newExp})";
    }




    public List<string> IntegrateFunction(string function)
    {
        // Kullanıcının girdiği fonksiyonun integralini hesaplar.
        var terms = SplitTerms(function);
        var integralResult = new List<string>();

        foreach (var term in terms)
        {
            var cleanedTerm = term.Replace(" ", "").Replace("+-","-");  // Boşlukları temizle
            if (cleanedTerm.Contains("x"))
            {
                if (cleanedTerm.Contains("*e^") || cleanedTerm.StartsWith("e^")||(cleanedTerm.IndexOf("-e^")==0))  // e^ veya *e^ kontrolü
                {
                    string coeffPart, inner;
                    if (cleanedTerm.Contains("*e^"))
                    {
                        var parts = cleanedTerm.Split(new[] { "*e^" }, StringSplitOptions.None);  // *e^ ifadesine göre ayır
                        coeffPart = parts[0].Trim();  // Katsayıyı al
                        double c1; double c2;
                        try 
                        {
                            c1=Convert.ToDouble(parts[0].Trim().Replace("(","").Replace(")","").Split("/")[0]);
                            c2=Convert.ToDouble(parts[0].Trim().Replace("(","").Replace(")","").Split("/")[1]);
                            coeffPart=(c1/c2).ToString();
                        }
                        catch
                        {
                           
                            coeffPart = parts[0].Trim();
                        }
                        
                        inner = parts[1].Trim();  // e^'den sonraki kısım
                    }
                    else
                    {
                        
                        
                        coeffPart = cleanedTerm.Substring(0, cleanedTerm.IndexOf("e^")).Trim();  // Katsayıyı al
                        if (cleanedTerm.IndexOf("-e")==0)
                        {
                            Console.WriteLine("burda");
                            coeffPart=(-1).ToString();
                        }
                        inner = cleanedTerm.Split(new[] { "e^" }, 2, StringSplitOptions.None)[1].Trim();  // e^'den sonraki kısım
                    }
                    
                    double coeff = string.IsNullOrEmpty(coeffPart) || coeffPart == "*" ? 1 : double.Parse(coeffPart);
                    
                    integralResult.Add(IntegrateExponential(coeff, inner));
                }
                else if (cleanedTerm.Contains("sin(") || cleanedTerm.Contains("cos(") || cleanedTerm.Contains("tan(")|| cleanedTerm.Contains("cot(")|| cleanedTerm.Contains("sec(")|| cleanedTerm.Contains("csc("))
                {    
                    // Trigonometrik fonksiyonları ayıkla
                    var (inner, funcType) = ExtractInnerAndFuncType(cleanedTerm);
                    
                    string coeffPart = term.IndexOf(funcType) > 0 ? term.Substring(0, term.IndexOf(funcType) - 1).Trim() : "";
                    
                    string noprod = $"{funcType}({inner})*";
                    string[] splitParts = term.Split(new string[] { noprod }, StringSplitOptions.None);

                    // Loop through split parts and assign the non-empty one to coeffPart
                    foreach (var part in splitParts)
                    {
                        if (!string.IsNullOrEmpty(part)&&(cleanedTerm!=part))
                        {
                            coeffPart = part;
                            break; // Exit once the first non-empty part is found
                        }
                    }
                    
                    double coeff = 1;
                    if (cleanedTerm.IndexOf($"-{funcType}")==0)
                    {
                        coeff=-1;
                    }
                    

                    if (!string.IsNullOrEmpty(coeffPart))
                    {   double c1,c2;
                        try 
                        {
                            c1=Convert.ToDouble(coeffPart.Replace("(","").Replace(")","").Split("/")[0]);
                            c2=Convert.ToDouble(coeffPart.Replace("(","").Replace(")","").Split("/")[1]);
                            coeff=c1/c2;
                        }
                        catch
                        {
                            coeff = double.TryParse(coeffPart, out double result) ? result : 1;
                        }
                    }
                    
                    integralResult.Add(IntegrateTrigonometric(inner, funcType, coeff));
                }
                else if (cleanedTerm.StartsWith("ln") || cleanedTerm.Contains("-ln"))
                {
                    // Katsayıyı ve inner'ı ayıklama
                    string inner="";string coeffPart="";
                    string p2="";string p3="";string p1="";string p4="";
                    if (cleanedTerm.IndexOf("-ln")==0)
                    {
                        try 
                        {
                            p4=cleanedTerm.Split(")*")[1];
                        }
                        catch
                        {
                            p4="";
                        }
                        p3=cleanedTerm.Split(")*")[0]+")";
                        
                        inner=p3.Replace("-ln(","").Replace(")","").Replace("((","");
                        coeffPart=(-1).ToString();
                        
                    }
                    else if (cleanedTerm.IndexOf("ln")!=0)
                    {
                        p1 =cleanedTerm.Split("*ln")[0];
                        p2 =cleanedTerm.Split("*ln")[1];
                        inner=p2.Replace("(","").Replace(")","").Replace("((","");
                        coeffPart=p1.Replace("(","").Replace(")","");
                    }
                    
                    if (cleanedTerm.IndexOf("ln")==0)
                    {
                        try 
                        {
                            p4=cleanedTerm.Split(")*")[1];
                        }
                        catch
                        {
                            p4="";
                        }
                        p3=cleanedTerm.Split(")*")[0]+")";
                        
                        inner=p3.Replace("ln(","").Replace(")","").Replace("((","");
                        coeffPart=p4.Replace("(","").Replace(")","");
                        
                    }
                    
                    Console.WriteLine(coeffPart);
                    Console.WriteLine(inner);
                    
                    

                    // Katsayıyı kontrol et
                    double coeff = 1;  // Varsayılan katsayı
                    if (!string.IsNullOrEmpty(coeffPart))
                    {
                        if (coeffPart.Contains("/"))
                        {
                            double c1 = double.Parse(coeffPart.Split('/')[0].Trim());
                            double c2 = double.Parse(coeffPart.Split('/')[1].Trim());
                            coeff=c1/c2;
                        }
                        
                        else if (double.TryParse(coeffPart, out double result))
                        {
                            coeff = result;
                        }
                    }

                    integralResult.Add(IntegrateLogarithm(coeff, inner));
                }
                else if (cleanedTerm.StartsWith("log") || cleanedTerm.Contains("-log"))
                {
                    // Katsayıyı ve inner'ı ayıklama
                    string inner="";string coeffPart="";
                    string p2="";string p3="";string p1="";string p4="";
                    if (cleanedTerm.IndexOf("-log")==0)
                    {
                        try 
                        {
                            p4=cleanedTerm.Split(")*")[1];
                        }
                        catch
                        {
                            p4="";
                        }
                        p3=cleanedTerm.Split(")*")[0]+")";
                        
                        inner=p3.Replace("-log(","").Replace(")","").Replace("((","");
                        coeffPart=(-1).ToString();
                        
                    }
                    else if (cleanedTerm.IndexOf("log")!=0)
                    {
                        p1 =cleanedTerm.Split("*log")[0];
                        p2 =cleanedTerm.Split("*log")[1];
                        inner=p2.Replace("(","").Replace(")","").Replace("((","");
                        coeffPart=p1.Replace("(","").Replace(")","");
                    }
                    
                    if (cleanedTerm.IndexOf("log")==0)
                    {
                        try 
                        {
                            p4=cleanedTerm.Split(")*")[1];
                        }
                        catch
                        {
                            p4="";
                        }
                        p3=cleanedTerm.Split(")*")[0]+")";
                        
                        inner=p3.Replace("log(","").Replace(")","").Replace("((","");
                        coeffPart=p4.Replace("(","").Replace(")","");
                        
                    }
                    
                    Console.WriteLine(coeffPart);
                    Console.WriteLine(inner);
                    
                    

                    // Katsayıyı kontrol et
                    double coeff = 1;  // Varsayılan katsayı
                    if (!string.IsNullOrEmpty(coeffPart))
                    {
                        if (coeffPart.Contains("/"))
                        {
                            double c1 = double.Parse(coeffPart.Split('/')[0].Trim());
                            double c2 = double.Parse(coeffPart.Split('/')[1].Trim());
                            coeff=c1/c2;
                        }
                        
                        else if (double.TryParse(coeffPart, out double result))
                        {
                            coeff = result;
                        }
                    }

                    integralResult.Add(IntegrateLogarithmbaseten(coeff, inner));
                }
                else if (cleanedTerm.Contains("/")&&(!cleanedTerm.Contains("^"))&&(!cleanedTerm.Contains(")*")))  // Paydalı fonksiyonları işleme
                {
                    string coeff = cleanedTerm.Split('/')[0];
    
                    cleanedTerm = cleanedTerm.Replace(coeff, "1");
                    var result = HandleRationalFunctions(cleanedTerm,coeff);
                    if (result != null)
                    {
                        integralResult.Add(result);
                    }
                }
                else if (cleanedTerm.Contains(")^") && cleanedTerm.Contains("x"))
                {   
                    // (a*x + b)^n şeklinde bir terim
                    if (cleanedTerm.Contains("(") && cleanedTerm.Contains(")^"))
                    {   
                        double c1,c2,coeff,exp2,exp1,exp;
                        string coeffpart,inner,exponent;
                        coeffpart="";
                        // (a*x + b)^n formundaki terimleri işleme
                            try
                            {    
                                coeffpart=cleanedTerm.Replace("*(","*((").Split("*(")[0];
                                inner=cleanedTerm.Replace("*(","*((").Split("*(")[1].Split("^")[0];
                                exponent=cleanedTerm.Replace("*(","*((").Split("*(")[1].Split("^")[1];
                            }
                            catch
                            {
                                
                                if (cleanedTerm.IndexOf("(")==0)
                                {
                                    coeffpart=1.ToString();
                                }
                                else if (cleanedTerm.IndexOf("-(")==0)
                                {
                                    coeffpart=(-1).ToString();
                                }
                                inner=cleanedTerm.Split("^")[0];
                                exponent=cleanedTerm.Split("^")[1];
                            }
                            try  
                            {
                                c1=Convert.ToDouble(coeffpart.Replace("(","").Replace(")","").Split("/")[0]);
                                c2=Convert.ToDouble(coeffpart.Replace("(","").Replace(")","").Split("/")[1]);
                                coeff=c1/c2;
                                
                            }
                            catch
                            {
                                coeff=Convert.ToDouble(coeffpart);
                                
                            }
                            
                            try
                            {
                                exp1=Convert.ToDouble(exponent.Replace("(","").Replace(")","").Split("/")[0]);
                                exp2=Convert.ToDouble(exponent.Replace("(","").Replace(")","").Split("/")[1]);
                                exp=exp1/exp2;
                            }
                            catch
                            {
                                exp=Convert.ToDouble(exponent.Replace("(","").Replace(")",""));
                            }
                            integralResult.Add(IntegrateRationalPolynomial(coeff, inner, exp));
                    }
                }
                
                else if (cleanedTerm.Contains("x^")||cleanedTerm.Contains("*x")||cleanedTerm.IndexOf("x")==0||(cleanedTerm.IndexOf("-x")==0))
                {
                    
                    // Polinom veya payda kısmını işleyelim
                    var (coeff, exponent) = GetCoeffAndExponent(cleanedTerm);
                    if (cleanedTerm.IndexOf("-x")==0)
                    {
                        coeff=-1;
                    }
                    integralResult.Add(IntegratePolynomial(Convert.ToDouble(coeff), Convert.ToDouble(exponent)));
                }
               
            }
            else
            {
                // Sabit terim
                integralResult.Add(IntegrateConstant(cleanedTerm));
            }
        }

        integralResult.Add("C");  // Sabit terim
        return integralResult;
    }
}
class Programm
{
    public static string integralsonuc(string userInput )
    {
        Program program = new Program();
        
        
        List<string> integralResult = program.IntegrateFunction(userInput.Replace("-","+-"));

        // Sonucu yazdır
        return  string.Join(" + ", integralResult);
    }

}
}
