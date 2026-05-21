using System;
using System.Text.RegularExpressions;

namespace GraphBuilder.Services;

public static class NodeCodeEvaluator
{
    private static readonly Random _random = new Random();

    /// <summary>
    /// Вычисляет код вершины и возвращает целое число — желаемый предикат (1..maxPredicate).
    /// </summary>
    public static int Evaluate(string code, int maxPredicate)
    {
        if (string.IsNullOrWhiteSpace(code))
            return new Random().Next(1, maxPredicate + 1); // по умолчанию случайно

        // Убираем пробелы
        code = code.Trim();

        // 1. Попробуем распарсить как число
        if (int.TryParse(code, out int constValue))
        {
            return Clamp(constValue, maxPredicate);
        }

        // 2. Функция random(минимум, максимум)
        var randomMatch = Regex.Match(code, @"random\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)", RegexOptions.IgnoreCase);
        if (randomMatch.Success)
        {
            int min = int.Parse(randomMatch.Groups[1].Value);
            int max = int.Parse(randomMatch.Groups[2].Value);
            if (min > max) (min, max) = (max, min);
            int value = _random.Next(min, max + 1);
            return Clamp(value, maxPredicate);
        }

        // 3. Функция choose(список) – пример: choose(1,3,2)
        var chooseMatch = Regex.Match(code, @"choose\s*\(\s*([\d\s,]+)\s*\)", RegexOptions.IgnoreCase);
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
                int idx = _random.Next(0, numbers.Count);
                return Clamp(numbers[idx], maxPredicate);
            }
        }

        // 4. Поддержка простого выражения вида "1+2", "3-1", "2*2" (без скобок)
        var exprMatch = Regex.Match(code, @"^(\d+)\s*([+\-*])\s*(\d+)$");
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

        // Если ничего не подошло – кидаем исключение (или возвращаем случайное)
        throw new ArgumentException($"Не удалось интерпретировать код: {code}");
    }

    private static int Clamp(int value, int max)
    {
        if (value < 1) return 1;
        if (value > max) return max;
        return value;
    }
}