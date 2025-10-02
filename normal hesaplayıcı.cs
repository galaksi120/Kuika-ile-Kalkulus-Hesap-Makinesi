
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace Kuika.ThirdPartyApisCollection
{
    using System;

public class Calculatorhp
{
    private string expression;
    private int position;

    public Calculatorhp(string expression)
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
    public string Calculate(object expr)
    {
        if (expr.Equals("e"))
        {
            return $"{Math.Exp(1)}";
        }
        else if (expr is double || expr is int)
        {
            return $"{expr}";
        }
        else if (expr is Tuple<string, object, object>)
        {
            var i1=((Tuple<string, object, object>)expr).Item2;
            var i2=((Tuple<string, object, object>)expr).Item3;
            var op = ((Tuple<string, object, object>)expr).Item1;
            if (op == "+")
            {
                return $"{Convert.ToDouble(Calculate(i1))+Convert.ToDouble(Calculate(i2))}";
            }
            else if (op == "-")
            {
                return $"{Convert.ToDouble(Calculate(i1))-Convert.ToDouble(Calculate(i2))}";
            }
            else if (op == "*")
            {
                var left = Calculate(((Tuple<string, object, object>)expr).Item2);
                var right = Calculate(((Tuple<string, object, object>)expr).Item3);
                return $"{Convert.ToDouble(left)*Convert.ToDouble(right)}";
                
            
            }
            else if (op == "neg")
            {
                var inner = ((Tuple<string, object,object>)expr).Item2;
                
                return $"{-Convert.ToDouble(Calculate(inner))}";
                
                
            }
            else if (op == "/")
            {
                var numerator = Calculate(((Tuple<string, object, object>)expr).Item2);
                var denominator = Calculate(((Tuple<string, object, object>)expr).Item3);
                
                return $"{Convert.ToDouble(numerator)/Convert.ToDouble(denominator)}";
      
                
            }
            else if (op == "sin")
            {
                return $"{Math.Sin(Convert.ToDouble(Calculate(i1)))}";
            }
            else if (op == "cos")
            {
                return $"{Math.Cos(Convert.ToDouble(Calculate(i1)))}";
            }
            else if (op == "tan")
            {
                return $"{Math.Tan(Convert.ToDouble(Calculate(i1)))}";
            }
            else if (op == "cot")
            {
                return $"{1/(Math.Tan(Convert.ToDouble(Calculate(i1))))}";
            }
            else if (op == "sec")
            {
                return $"{1/(Math.Cos(Convert.ToDouble(Calculate(i1))))}";
            }
            else if (op == "csc")
            {
                return $"{1/(Math.Sin(Convert.ToDouble(Calculate(i1))))}";
            }

            else if (op == "ln")
            {
                var argument = ((Tuple<string, object, object>)expr).Item2;
                if (Convert.ToDouble(Calculate(i1))<0)
                {
                    return "logaritma değeri 0 dan büyük olmalı";
                }
                
                return  $"{Math.Log(Convert.ToDouble(Calculate(i1)))}";
            }
            else if (op == "log")
            {
                var argument = ((Tuple<string, object, object>)expr).Item2;
                if (Convert.ToDouble(Calculate(i1))<0)
                {
                    return  "logaritma değeri 0 dan büyük olmalı";
                }
                return $"{Math.Log(Convert.ToDouble(Calculate(i1)),10)}";
            }
            else if (op == "e^")
            {
                return $"{Math.Exp(Convert.ToDouble(Calculate(i1)))}";
            }
            else if (op == "^")
            {
                var exponent = ((Tuple<string, object, object>)expr).Item2;
                return $"{Math.Pow(Convert.ToDouble(Calculate(i1)),Convert.ToDouble(Calculate(i2)))}";  // Use logarithmic differentiation for power expressions
            }
        }
        return expr.ToString();
    }

}

public class Programnormal
{
    public static string normalhesapsonuc(string expression)
    {
        
        
        Calculatorhp calculator = new Calculatorhp(expression);

        try
        {
            var parsedExpr = calculator.Parse();
            string resultcalc = calculator.Calculate(parsedExpr);
            return resultcalc;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}
}
