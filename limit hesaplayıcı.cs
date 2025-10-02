
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace Kuika.ThirdPartyApisCollection
{
 public class LimitCalculator
{
    private string expression;
    private int position;
    public double xValue;

    public LimitCalculator(string expression, double xValue)
    {
        this.expression = expression.Replace(" ", "");
        this.position = 0;
    }

    public object Parse()
    {
        var result = Expr();
        if (position < expression.Length)
        {
            throw new Exception("Invalid expression!");
        }
        return result;
    }

    private object Expr()
    {
        var result = Term();
        while (position < expression.Length)
        {
            if (CurrentChar() == '+')
            {
                position++;
                result = new Tuple<string, object, object>("+", result, Term());
            }
            else if (CurrentChar() == '-')
            {
                position++;
                result = new Tuple<string, object, object>("-", result, Term());
                Console.WriteLine(result);
            }
            else
            {
                break;
            }
        }
        return result;
    }

    private object Term()
    {
        var result = Factor();
        while (position < expression.Length)
        {
            if (CurrentChar() == '*')
            {
                position++;
                result = new Tuple<string, object, object>("*", result, Factor());
            }
            else if (CurrentChar() == '/')
            {
                position++;
                var denominator = Factor();
                result = new Tuple<string, object, object>("/", result, denominator);
            }
            else if (CurrentChar() == '^')
            {
                position++;
                var exponent = Factor();
                result = new Tuple<string, object, object>("^", result, exponent);
            }
            else
            {
                break;
            }
        }
        return result;
    }
        private object Factor()
    {
        var result = BaseFactor(); // BaseFactor metodunu çağırarak, temel faktörü işleyelim
        while (position < expression.Length)
        {
            if (CurrentChar() == '^')
            {
                position++;
                result = new Tuple<string, object, object>("^", result, BaseFactor());
            }
            else
            {
                break;
            }
        }
        return result;
    }

    private object BaseFactor()
    {
        if (CurrentChar() == '-')
        {
            position++;  // '-' işaretini atla
            var nextTerm = Factor();  // Sonraki terimi al
            return new Tuple<string, object,object>("neg", nextTerm,null);  // 'neg' terimi ile dön
        }
        if (char.IsDigit(CurrentChar()) || CurrentChar() == '-')
        {
            
            return Number();
        }
        else if (CurrentChar() == '(')
        {
            position++;
            var result = Expr();
            if (CurrentChar() != ')')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;
            return result;
        }
        else if (expression.Substring(position).StartsWith("sin"))
        {
            position += 3;
            if (CurrentChar() != '(')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;
            var angle = Expr();
            if (CurrentChar() != ')')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;
            return new Tuple<string, object, object>("sin", angle, null);
        }
        else if (expression.Substring(position).StartsWith("cos"))
        {
            position += 3;
            if (CurrentChar() != '(')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;
            var angle = Expr();
            if (CurrentChar() != ')')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;
            return new Tuple<string, object, object>("cos", angle, null);
        }
        else if (expression.Substring(position).StartsWith("tan"))
        {
            position += 3;
            if (CurrentChar() != '(')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;
            var angle = Expr();
            if (CurrentChar() != ')')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;
            return new Tuple<string, object, object>("tan", angle, null);
        }
        else if (expression.Substring(position).StartsWith("cot"))
        {
            position += 3;  // Skip "cot"
            if (CurrentChar() != '(')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;  // Skip '('
            var angle = Expr();
            if (CurrentChar() != ')')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;  // Skip ')'
            return new Tuple<string, object, object>("cot", angle, null);
        }
        else if (expression.Substring(position).StartsWith("sec"))
       {
            position += 3;  // Skip "sec"
            if (CurrentChar() != '(')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;  // Skip '('
            var angle = Expr();
            if (CurrentChar() != ')')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;  // Skip ')'
            return new Tuple<string, object, object>("sec", angle, null);
        }
        else if (expression.Substring(position).StartsWith("csc"))
        {
            position += 3;  // Skip "cosec"
            if (CurrentChar() != '(')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;  // Skip '('
            var angle = Expr();
            if (CurrentChar() != ')')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;  // Skip ')'
            return new Tuple<string, object, object>("csc", angle, null);
        }

        else if (expression.Substring(position).StartsWith("ln"))
        {
            position += 2;
            if (CurrentChar() != '(')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;
            var argument = Expr();
            if (CurrentChar() != ')')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;
            return new Tuple<string, object, object>("ln", argument, null);
        }
        else if (expression.Substring(position).StartsWith("log"))
        {
            position += 3;
            if (CurrentChar() != '(')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;
            var argument = Expr();
            if (CurrentChar() != ')')
            {
                throw new Exception("Mismatched parentheses!");
            }
            position++;
            return new Tuple<string, object, object>("log", argument, null);
        }
        else if (expression.Substring(position).StartsWith("e^"))
        {
            position += 2;  // Skip "e^"
            if (CurrentChar() == '(')  // check if it is followed by parentheses
            {
                position++;
                var exponent = Expr();
                if (CurrentChar() != ')')
                {
                    throw new Exception("Mismatched parentheses!");
                }
                position++;
                return new Tuple<string, object, object>("e^", exponent, null);  // Handle e^x form with parentheses
            }
            else
            {
                var exponent = Expr();
                return new Tuple<string, object, object>("e^", exponent, null);  // Handle simple e^x form
            }
        }
        else if (CurrentChar() == 'x')
        {
            position++;
            if (CurrentChar() == '^')
            {
                position++;
                var exponent = Factor();
                return new Tuple<string, object, object>("x^", exponent, null);
            }
            return "x";
        }
        else if (CurrentChar() == 'e')
        {
            position++;
            return "e";
        }

        throw new Exception("Invalid factor at position " + position);
    }

    private double Number()
{
    int startPosition = position;
    // Eğer negatif işareti varsa
    if (CurrentChar() == '-')
    {
        position++; // Negatif işaretini geç
    }
    while (position < expression.Length && (char.IsDigit(CurrentChar()) || CurrentChar() == '.'))
    {
        position++;
    }
    return double.Parse(expression.Substring(startPosition, position - startPosition));
}

    private char CurrentChar()
    {
        if (position < expression.Length)
        {
            return expression[position];
        }
        return '\0';
    }

    // Derivative calculation
    public string Limit(object expr,double xValue)
    {
        if (expr.Equals("x"))
        {
            
            if (expr.ToString()==expression)
            {
                return $"{"x".Replace("x",xValue.ToString())}";
            }  
            return "x";
        }
        else if (expr.Equals("e"))
        {
            return $"{Math.Exp(1)}";
        }
        else if (expr is double || expr is int)
        {
            return $"{expr}";
        }
        else if (expr is Tuple<string, object, object>)
        {
            
            var op = ((Tuple<string, object, object>)expr).Item1;
            if (op == "+")
            {
                
                try
                {
                    return $"{Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString()))+Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString()))}";
                }
                catch
                {
                    return $"{Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())}+{Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString())}";
                }
            }
            else if (op == "-")
            {
                try
                {
                    
                    double a=Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString()));
                    double b=Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString()));
                    double islem=Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString()))-Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString()));
                    if ((a==Double.PositiveInfinity && b==Double.PositiveInfinity)||(a==Double.NegativeInfinity && b==Double.NegativeInfinity))
                        {
                            xValue=1e6;
                            
                            double i11=Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString()));
                            double i12=Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString()));
                            islem=i11-i12;
                        }
                        if  (islem>1e6)
                        {
                            return Double.PositiveInfinity.ToString();
                        }
                        else if  (islem<-1e6)
                        {
                            return Double.NegativeInfinity.ToString();
                        }
                        else
                        {
                            return Math.Round(islem,5).ToString();
                        }
                }
                catch
                {
                    return $"{Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())}-{Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString())}";
                }
            }
            else if (op == "*")
            {
                try
                {
                    return $"{Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString()))*Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString()))}";
                }
                catch
                {
                    return $"{Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())}*{Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString())}";
                }
            }
            else if (op == "neg")
            {
                var inner = ((Tuple<string, object,object>)expr).Item2;
                try
                {
                    return $"{-Convert.ToDouble(Limit(inner,xValue).Replace("x",xValue.ToString()))}";
                }
                catch
                {
                    return $"-{Limit(inner,xValue).Replace("x",xValue.ToString())}";
                }
                
            }
            else if (op == "/")
            {
                try
                {
                    
                    double a=Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString()));
                    double b=Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString()));
                    if (Double.IsNaN(a/b))
                    {
                        xValue=xValue+0.0001;
                        double i11=Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString()));
                        xValue=xValue-0.0001;
                        double i12=Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString()));
                        double d1=((i11-i12)/0.0001);
                        xValue=xValue+0.0001;
                        double i21=Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString()));
                        xValue=xValue-0.0001;
                        double i22=Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString()));
                        double d2=((i21-i22)/0.0001);
                        double islem=d1/d2;
                        if (Double.IsNaN(islem))
                        {
                            xValue=1e6;
                            xValue=xValue+0.0001;
                            i11=Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString()));
                            xValue=xValue-0.0001;
                            i12=Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString()));
                            d1=((i11-i12)/0.0001);
                            xValue=xValue+0.0001;
                            i21=Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString()));
                            xValue=xValue-0.0001;
                            i22=Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString()));
                            d2=((i21-i22)/0.0001);
                            islem=d1/d2;
                        }
                        if  (islem>1e6)
                        {
                            return Double.PositiveInfinity.ToString();
                        }
                        else if  (islem<-1e6)
                        {
                            return Double.NegativeInfinity.ToString();
                        }
                        else
                        {
                            return Math.Round(islem,5).ToString();
                        }
                    }
                    else
                    {
                        return $"{Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString()))/Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString()))}";
                    }
                }
                catch
                {
                    return $"{Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())}/{Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString())}";
                    
                }
                
            }
            else if (op == "sin")
            {
                
                try
                {
                    return $"{Math.Sin(Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())))}";
                }
                catch
                {
                    return $"sin({Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())})";
                }
            }
            else if (op == "cos")
            {
                try
                {
                    return $"{Math.Cos(Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())))}";
                }
                catch
                {
                    return $"cos({Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())})";
                }
            }
            else if (op == "tan")
            {
                try
                {
                    return $"{Math.Tan(Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())))}";
                }
                catch
                {
                    return $"tan({Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())})";
                }
            }
            else if (op == "cot")
            {
                try
                {
                    return $"{1/Math.Tan(Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())))}";
                }
                catch
                {
                    return $"tan({Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())})";
                }
            }
            else if (op == "sec")
            {
                try
                {
                    return $"{1/Math.Cos(Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())))}";
                }
                catch
                {
                    return $"sec({Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())})";
                }
            }
            else if (op == "csc")
            {
                try
                {
                    return $"{1/Math.Sin(Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())))}";
                }
                catch
                {
                    return $"csc({Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())})";
                }
            }

            else if (op == "ln")
            {
                try
                {
                    return $"{Math.Log(Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())))}";
                }
                catch
                {
                    return $"ln({Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())})";
                }
            }
            else if (op == "log")
            {
                try
                {
                    return $"{Math.Log(Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())),10)}";
                }
                catch
                {
                    return $"log({Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())})";
                }
            }
            else if (op == "e^")
            {
                try
                {
                    return $"{Math.Exp(Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())))}";
                }
                catch
                {
                    return $"e^({Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())})";
                }
            }
            
            else if (op == "x^")
            {
                try
                {
                    return $"{Math.Pow(Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item1,xValue).Replace("x^",xValue.ToString())),Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())))}";
                }
                catch
                {
                    return $"{Limit(((Tuple<string, object, object>)expr).Item1,xValue).Replace("x^",xValue.ToString())}^({Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())})";
                }
            }
            
            else if (op == "^")
            {
                try
                {
                    return $"{Math.Pow(Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())),Convert.ToDouble(Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString())))}";
                }
                catch
                {
                    return $"{Limit(((Tuple<string, object, object>)expr).Item2,xValue).Replace("x",xValue.ToString())}^({Limit(((Tuple<string, object, object>)expr).Item3,xValue).Replace("x",xValue.ToString())})";
                }
            }
            
        }
        return expr.ToString();
    

    }

    
   public static string limitsonuc(string expression,string xValuee)
    {
        
        double a=1; double b=0;

        double xValue = Convert.ToDouble(xValuee.ToString().Replace("∞",(a/b).ToString()));
        
        LimitCalculator calculator = new LimitCalculator(expression,xValue);

        try
        {
            var parsedExpr = calculator.Parse();
            string limit = calculator.Limit(parsedExpr,xValue);
            
            return $"{limit}";
            
            
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}
}
