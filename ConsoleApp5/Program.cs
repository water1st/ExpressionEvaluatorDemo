using ConsoleApp5;
using System.Diagnostics;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;

namespace ExpressionEvaluator
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmark>();
        }
    }

    [ThreadingDiagnoser]
    [MemoryDiagnoser]
    public class Benchmark
    {
        // 测试用例
        private string[] expressions = new string[]
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


        private IEvaluator etEvaluator = new ExpressionTreeEvaluator();
        private IEvaluator rnpEvaluator = new ExpressionTreeEvaluator();

        [Benchmark(OperationsPerInvoke = 10000, Description = "逆波兰表达式实现")]
        public void RunRPNEvaluator()
        {
            foreach (var expression in expressions)
            {
                rnpEvaluator.Evaluate(expression);
            }
        }

        [Benchmark(OperationsPerInvoke = 10000, Description = "中序二叉树实现")]
        public void RunETEvaluator()
        {
            foreach (var expression in expressions)
            {
                etEvaluator.Evaluate(expression);
            }
        }
    }
}