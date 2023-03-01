using ConsoleApp5;
using System.Diagnostics;

namespace ExpressionEvaluator
{
    class Program
    {
        static void Main(string[] args)
        {
            var multiple = 10000;
            // 测试用例
            var expressions = new string[]
            {
                "1+2*3 == 5",
                "(1+2) * 3 / 3.14 + 5 - 3",
                "1 == 2",
                "1!=2",
                "-11>=-5.6",
                "\"bob\" == \"jan\"",
                "1<2.2",
                "\"2022-11-21\" > \"2019-11-22 23:46:22\"",
                "1>=2",
                "1<=2",
                "[\"bob\",\"jack\"] ## \"bob\"",
                "[\"bob\",\"jack\"] ## \"ben\"",
                "[\"bob\",\"jack\"] !# \"bob\"",
                "[\"bob\",\"jack\"] !# \"jan\"",
                "[\"bob\",\"jack\"] !# [\"jan\"]",
                "[\"bob\",\"jack\"] ## [\"jan\"]",
            };

            var stopWatch = new Stopwatch();

            IEvaluator evaluator = new Evaluator();
            Console.WriteLine("以下转为逆波兰表达式运算");
            stopWatch.Start();
            for (int i = 0; i < multiple; i++)
            {
                foreach (var expression in expressions)
                {
                    var result = evaluator.Evaluate(expression);

                    if (i == 0)
                        Console.WriteLine($"表达式 {expression} 结果为 {result}");
                }
            }
            stopWatch.Stop();
            Console.WriteLine($"{expressions.Length * multiple}次运算耗时:{stopWatch.Elapsed}");

            Console.WriteLine();
            Console.WriteLine("以下转为表达式树运算");
            evaluator = new ExpressionTreeEvaluator();
            stopWatch.Restart();
            for (int i = 0; i < multiple; i++)
            {
                foreach (var expression in expressions)
                {
                    var result = evaluator.Evaluate(expression);
                    if (i == 0)
                        Console.WriteLine($"表达式 {expression} 结果为 {result}");
                }
            }

            stopWatch.Stop();
            Console.WriteLine($"{expressions.Length * multiple}次运算耗时:{stopWatch.Elapsed}");
        }


    }
}