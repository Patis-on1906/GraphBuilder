using System;
using System.Text.RegularExpressions;

namespace GraphBuilder.Services;

public static class NodeCodeEvaluator
{
    private static readonly Random s_random = new Random();

    public static int Evaluate(string code, int maxPredicate)
    {
        if (string.IsNullOrWhiteSpace(code))
            return s_random.Next(1, maxPredicate + 1);

        code = code.Trim();

        // Замена N (с учётом регистра) на реальное количество исходящих дуг.
        string processedCode = Regex.Replace(code, @"\bN\b", maxPredicate.ToString(), RegexOptions.IgnoreCase);

        // Число.
        if (int.TryParse(processedCode, out int constValue))
            return Clamp(constValue, maxPredicate);

        // Функция random(минимум, максимум) – допускаем пробелы.
        var randomMatch = Regex.Match(processedCode, @"random\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)", RegexOptions.IgnoreCase);
        if (randomMatch.Success)
        {
            int min = int.Parse(randomMatch.Groups[1].Value);
            int max = int.Parse(randomMatch.Groups[2].Value);
            if (min > max) (min, max) = (max, min);
            int value = s_random.Next(min, max + 1);
            return Clamp(value, maxPredicate);
        }

        // Функция choose(...).
        var chooseMatch = Regex.Match(processedCode, @"choose\s*\(\s*([\d\s,]+)\s*\)", RegexOptions.IgnoreCase);
        if (chooseMatch.Success)
        {
            string argsStr = chooseMatch.Groups[1].Value;
            string[] parts = argsStr.Split(',');
            var numbers = new List<int>();
            foreach (var part in parts)
            {
                if (int.TryParse(part.Trim(), out int num))
                    numbers.Add(num);
            }
            if (numbers.Count > 0)
            {
                int idx = s_random.Next(0, numbers.Count);
                return Clamp(numbers[idx], maxPredicate);
            }
        }

        // Простое выражение вида "1+2", "3-1", "2*2".
        var exprMatch = Regex.Match(processedCode, @"^(\d+)\s*([+\-*])\s*(\d+)$");
        if (exprMatch.Success)
        {
            int a = int.Parse(exprMatch.Groups[1].Value);
            string op = exprMatch.Groups[2].Value;
            int b = int.Parse(exprMatch.Groups[3].Value);
            int result = op switch
            {
                "+" => a + b,
                "-" => a - b,
                "*" => a * b,
                _ => a
            };
            return Clamp(result, maxPredicate);
        }

        throw new ArgumentException($"Не удалось интерпретировать код: {code}");
    }

    private static int Clamp(int value, int max)
    {
        if (value < 1) return 1;
        if (value > max) return max;
        return value;
    }
}