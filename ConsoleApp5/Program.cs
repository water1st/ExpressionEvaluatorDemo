using ConsoleApp5;

namespace ExpressionEvaluator
{
    class Program
    {
        static void Main(string[] args)
        {

            // 测试用例
            var expressions = new string[]
            {
                "1+2*3 == 5",
                "(1+2) * 3 / 3.14",
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

            var evaluator = new Evaluator();
            foreach (var expression in expressions)
            {
                var result = evaluator.Evaluate(expression);

                Console.WriteLine($"表达式 {expression} 结果为 {result}");
            }
        }


    }
}