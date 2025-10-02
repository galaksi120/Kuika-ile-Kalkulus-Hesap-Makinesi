
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace Kuika.ThirdPartyApisCollection
{
     public class DerivativeCalculator
{
    private string expression;
    private int position;

    public DerivativeCalculator(string expression)
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
        if (char.IsDigit(CurrentChar()) || CurrentChar() == '-')
        {
            try
            {
                if (char.IsDigit(CurrentChar()) || CurrentChar() == '-')
                {
                    return Number();
                }
                
            }
            catch
            {
                if (CurrentChar() == '-')
                {
                    position++;  // '-' işaretini atla
                    var nextTerm = Factor();  // Sonraki terimi al
                    return new Tuple<string, object,object>("neg", nextTerm,null); 
                }
            }
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
    public string Derivative(object expr)
    {
        if (expr.Equals("x"))
        {
            return "1";
        }
        else if (expr.Equals("e"))
        {
            return "0";
        }
        else if (expr is double || expr is int|| (!(expr).ToString().Contains("x")))
        {
            return "0";
        }
        else if (expr is Tuple<string, object, object>)
        {
            var op = ((Tuple<string, object, object>)expr).Item1;
            if (op == "+")
            {
                return $"{CheckPlus(Derivative(((Tuple<string, object, object>)expr).Item2),Derivative(((Tuple<string, object, object>)expr).Item3))}";
            }
            else if (op == "-")
            {
                return $"{CheckMinus(Derivative(((Tuple<string, object, object>)expr).Item2),Derivative(((Tuple<string, object, object>)expr).Item3))}";
            }
            else if (op == "*")
            {
                var left = Derivative(((Tuple<string, object, object>)expr).Item2);
                var right = Derivative(((Tuple<string, object, object>)expr).Item3);
                
                if (((Tuple<string, object, object>)expr).Item3 is double)
                {
                    return $"({Check(left, right)})";
                }
                return $"{CheckPlus(Check(left,PrettyPrint(((Tuple<string, object, object>)expr).Item3)),Check(PrettyPrint(((Tuple<string, object, object>)expr).Item2),right))}";
            }
            else if (op == "neg")
            {
                var inner = ((Tuple<string, object,object>)expr).Item2;
                
                return $"-{Derivative(inner)}";
                
                
            }
            else if (op == "/")
            {
                var numerator = Derivative(((Tuple<string, object, object>)expr).Item2);
                var denominator = Derivative(((Tuple<string, object, object>)expr).Item3);
                var p1 = PrettyPrint(((Tuple<string, object, object>)expr).Item2);
                var p2 = PrettyPrint(((Tuple<string, object, object>)expr).Item3);
                Console.WriteLine(p2);
                try
                {
                    return $"{Convert.ToDouble(CheckMinus(Check(numerator,p2), Check(p1, denominator))) / Convert.ToDouble(Check(p2,p2))}";
                }
                catch
                {
                    try
                    {
                        return $"{CheckMinus(Check(numerator,p2), Check(p1, denominator))} / {Check(p2,p2)}";
                    }
                    catch
                    {
                        return $"({CheckMinus(Check(numerator,p2), Check(p1, denominator))}) / ({Check(p2, p2)})";
                    }
                    
                }
                
            }
            else if (op == "sin")
            {
                return $"{Derivative(((Tuple<string, object, object>)expr).Item2)}*cos({PrettyPrint(((Tuple<string, object, object>)expr).Item2)})";
            }
            else if (op == "cos")
            {
                return $"(-{Derivative(((Tuple<string, object, object>)expr).Item2)}*sin({PrettyPrint(((Tuple<string, object, object>)expr).Item2)})) ";
            }
            else if (op == "tan")
            {
                return $"{Derivative(((Tuple<string, object, object>)expr).Item2)}*sec({PrettyPrint(((Tuple<string, object, object>)expr).Item2)})^2 ";
            }
            else if (op == "cot")
            {
                return $"{Derivative(((Tuple<string, object, object>)expr).Item2)}*(-csc({PrettyPrint(((Tuple<string, object, object>)expr).Item2)})^2)";
            }
            else if (op == "sec")
            {
                return $"{Derivative(((Tuple<string, object, object>)expr).Item2)}*sec({PrettyPrint(((Tuple<string, object, object>)expr).Item2)})*tan({PrettyPrint(((Tuple<string, object, object>)expr).Item2)})";
            }
            else if (op == "csc")
            {
                return $"{Derivative(((Tuple<string, object, object>)expr).Item2)}*(-csc({PrettyPrint(((Tuple<string, object, object>)expr).Item2)})*cot({PrettyPrint(((Tuple<string, object, object>)expr).Item2)}))";
            }

            else if (op == "ln")
            {
                var argument = ((Tuple<string, object, object>)expr).Item2;
                var derivativeOfArgument = Derivative(argument);
                return $"{derivativeOfArgument} / {PrettyPrint(argument)}";
            }
            else if (op == "log")
            {
                var argument = ((Tuple<string, object, object>)expr).Item2;
                var derivativeOfArgument = Derivative(argument);
                return $"{derivativeOfArgument} / ({PrettyPrint(argument)} * ln(10))";
            }
            else if (op == "e^")
            {
                return $"{Derivative(((Tuple<string, object, object>)expr).Item2)}*e^{PrettyPrint(((Tuple<string, object, object>)expr).Item2)} ";
            }
            else if (op == "x^")
            {
                var exponent = ((Tuple<string, object, object>)expr).Item2;
                return $"{PrettyPrint(exponent)}*x^{Convert.ToDouble(PrettyPrint(exponent))-1} ";  
            }
        }
        return expr.ToString();
    }

    public string PrettyPrint(object expr)
    {
        if (expr is Tuple<string, object, object>)
        {
            var op = ((Tuple<string, object, object>)expr).Item1;
            if (op == "+")
            {
                return $"({PrettyPrint(((Tuple<string, object, object>)expr).Item2)} + {PrettyPrint(((Tuple<string, object, object>)expr).Item3)})";
            }
            else if (op == "-")
            {
                return $"({PrettyPrint(((Tuple<string, object, object>)expr).Item2)} - {PrettyPrint(((Tuple<string, object, object>)expr).Item3)})";
            }

            else if (op == "*")
            {
                return $"({PrettyPrint(((Tuple<string, object, object>)expr).Item2)} * {PrettyPrint(((Tuple<string, object, object>)expr).Item3)})";
            }
            else if (op == "/")
            {
                return $"({PrettyPrint(((Tuple<string, object, object>)expr).Item2)} / {PrettyPrint(((Tuple<string, object, object>)expr).Item3)})";
            }
            
            else if (op == "sin")
            {
                return $"sin({PrettyPrint(((Tuple<string, object, object>)expr).Item2)})";
            }
            else if (op == "cos")
            {
                return $"cos({PrettyPrint(((Tuple<string, object, object>)expr).Item2)})";
            }
            else if (op == "tan")
            {
                return $"tan({PrettyPrint(((Tuple<string, object, object>)expr).Item2)})";
            }
            else if (op == "cot")
            {
                return $"cot({PrettyPrint(((Tuple<string, object, object>)expr).Item2)})";
            }
            else if (op == "sec")
            {
                return $"sec({PrettyPrint(((Tuple<string, object, object>)expr).Item2)})";
            }
            else if (op == "csc")
            {
                return $"csc({PrettyPrint(((Tuple<string, object, object>)expr).Item2)})";
            }

            else if (op == "ln")
            {
                return $"ln({PrettyPrint(((Tuple<string, object, object>)expr).Item2)})";
            }
            else if (op == "log")
            {
                return $"log({PrettyPrint(((Tuple<string, object, object>)expr).Item2)})";
            }
            else if (op == "e^")
            {
                return $"e^{PrettyPrint(((Tuple<string, object, object>)expr).Item2)}";
            }
            else if (op == "x^")
            {
                return $"x^{PrettyPrint(((Tuple<string, object, object>)expr).Item2)}";
            }
        }
        return expr.ToString();
    }
    
        private string IsFloat(object expr)
    {
        try
        {
            
            return $"{Convert.ToDouble(expr)}";
        }
        catch
        {
            return "false";
        }
    }

    private object Check(object expr1, object expr2)
    {
        if ((IsFloat(expr1.ToString().Replace("(","").Replace(")","")))!="false")
        {
            expr1=IsFloat(expr1.ToString().Replace("(","").Replace(")",""));
        }
        if ((IsFloat(expr2.ToString().Replace("(","").Replace(")","")))!="false")
        {
            expr2=IsFloat(expr2.ToString().Replace("(","").Replace(")",""));
        }
        try
        {
            return $"{Convert.ToDouble(expr1)*Convert.ToDouble(expr2)}";
    
        }
        catch 
        {
            if ((IsFloat(expr1).ToString()==IsFloat(0).ToString()) || (IsFloat(expr2).ToString()==IsFloat(0).ToString()))
            {
                return "0";
            }
            else if (IsFloat(expr1).ToString()==IsFloat("1").ToString())
            {    
                return expr2;
                
            }
            else if (IsFloat(expr2).ToString()==IsFloat("1").ToString())
            {    
                return expr1;
                
            }
            else if (expr1.ToString()==expr2.ToString())
            {
                return $"{expr1}^2";
            }
            else
            {
                return $"{expr1}*{expr2}";
            }
            
        }
    }

    private object CheckMinus(object expr1, object expr2)
    {
        if ((IsFloat(expr1.ToString().Replace("(","").Replace(")","")))!="false")
        {
            expr1=IsFloat(expr1.ToString().Replace("(","").Replace(")",""));
        }
        if ((IsFloat(expr2.ToString().Replace("(","").Replace(")","")))!="false")
        {
            expr2=IsFloat(expr2.ToString().Replace("(","").Replace(")",""));
        }
        try
        {
            return $"{Convert.ToDouble(expr1)-Convert.ToDouble(expr2)}";
    
        }
        catch 
        {
            
            if (IsFloat(expr1).ToString()==IsFloat("0").ToString())
            {    
                return $"-{expr2}";
                
            }
            else if (IsFloat(expr2).ToString()==IsFloat("0").ToString())
            {    
                return expr1;
                
            }
            else
            {
                return $"{expr1}-{expr2}";
            }
            
        }
    }

    private object CheckPlus(object expr1, object expr2)
    {
        if ((IsFloat(expr1.ToString().Replace("(","").Replace(")","")))!="false")
        {
            expr1=IsFloat(expr1.ToString().Replace("(","").Replace(")",""));
        }
        if ((IsFloat(expr2.ToString().Replace("(","").Replace(")","")))!="false")
        {
            expr2=IsFloat(expr2.ToString().Replace("(","").Replace(")",""));
        }
        try
        {
            return $"{Convert.ToDouble(expr1)+Convert.ToDouble(expr2)}";
    
        }
        catch 
        {
            
            if (IsFloat(expr1).ToString()==IsFloat("0").ToString())
            {    
                return $"{expr2}";
                
            }
            else if (IsFloat(expr2).ToString()==IsFloat("0").ToString())
            {    
                return expr1;
                
            }
            else
            {
                return $"{expr1}+{expr2}";
            }
            
        }
    }
    
}

public class turevprog
{
    public static string turevsonuc(string expression)
    {
        
        DerivativeCalculator calculator = new DerivativeCalculator(expression);

        try
        {
            var parsedExpr = calculator.Parse();
            string derivative = calculator.Derivative(parsedExpr);
            return derivative;
        }
        catch (Exception ex)
        {
            return $" {ex.Message}";
        }
    }
}
}
