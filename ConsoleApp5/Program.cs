using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ConsoleApp5;

namespace ExpressionEvaluator
{
    class Program
    {
        static void Main(string[] args)
        {
            var bet = new BETEvaluator();
            var rpn = new RPNEvaluator();

            PrintResultToConsole("RPN", rpn);
            PrintResultToConsole("BET", bet);


            BenchmarkRunner.Run<Benchmark>();
        }

        private static void PrintResultToConsole(string name, IEvaluator evaluator)
        {
            Console.WriteLine("=============================================================================================");
            Console.WriteLine($"                                            {name}");
            Console.WriteLine("=============================================================================================");
            Console.WriteLine();

            var expressions = new string[]
            {
                        "4.1+4.1-4.1*4.1/4.1>=100",
                        "1+2*3 == 5",
                        "(1+2) * 3 / 3.14 + 5 - 3",
                        "(((1+2) * 3 / 3.14 + 5 - 3) == (4+6)) || (1/3 + (3+5) == 8)",
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
                        "1==1 && 1<2",
                        "1==1 || 2==1",
                        "1==3 || true == false",
                        "true==false && 3==1",
                        "\"梓杰的的代码又快又省内存\" ## \"代码\"",
                        "\"梓杰的代码\" ## \"垃圾代码\"",
                        "\"梓杰的代码\" !# \"垃圾代码\""
            };

            foreach (var expression in expressions)
            {
                Console.WriteLine($"表达式 {expression} 的结果为 {evaluator.Evaluate(expression)}");
            }

            Console.WriteLine();
            Console.WriteLine("=============================================================================================");
        }
    }

    [ThreadingDiagnoser]
    [MemoryDiagnoser]
    public class Benchmark
    {
        // 测试用例
        private string[] expressions = new string[]
                {
                        "4.1+4.1-4.1*4.1/4.1>=100",
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
                        "\"梓杰的的代码又快又省内存\" ## \"代码\"",
                        "\"梓杰的代码\" ## \"垃圾代码\"",
                        "\"梓杰的代码\" !# \"垃圾代码\""
                };


        private IEvaluator betEvaluator = new BETEvaluator();
        private IEvaluator rpnEvaluator = new RPNEvaluator();

        [Benchmark(OperationsPerInvoke = 10000, Description = "RPN实现")]
        public void RunRPNEvaluator()
        {
            foreach (var expression in expressions)
            {
                rpnEvaluator.Evaluate(expression);
            }
        }

        [Benchmark(OperationsPerInvoke = 10000, Description = "BET实现")]
        public void RunBETEvaluator()
        {
            foreach (var expression in expressions)
            {
                betEvaluator.Evaluate(expression);
            }
        }
    }
}