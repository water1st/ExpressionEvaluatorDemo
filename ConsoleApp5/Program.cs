using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ConsoleApp5;

namespace ExpressionEvaluator
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<Benchmark>();


            string[] expressions = new string[]
                {
                        //"1+2*3 == 5",
                        //"(1+2) * 3 / 3.14 + 5 - 3",
                        //"1 == 2",
                        //"1!=2",
                        //"-11>=-5.6",
                        //"\"bob\" == \"jan\"",
                        //"1<2.2",
                        //"\"2022-11-21\" > \"2019-11-22 23:46:22\"",
                        //"1>=2",
                        //"1<=2",
                        //"[\"bob\",\"jack\"] ## \"bob\"",
                        //"[\"bob\",\"jack\"] ## \"ben\"",
                        //"[\"bob\",\"jack\"] !# \"bob\"",
                        //"[\"bob\",\"jack\"] !# \"jan\"",
                        //"[\"bob\",\"jack\"] !# [\"jan\"]",
                        //"[\"bob\",\"jack\"] ## [\"jan\"]",
                        "1==1 && 1<2",
                        "1==1 || 2==1",
                        "1==3 || true == false",
                        "true==false && 3==1"
                };

            var e = new Evaluator();
            foreach (var ex in expressions)
            {
                Console.WriteLine($"表达式 {ex} 的结果为 {e.Evaluate(ex)}");

            }
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

        [Benchmark(OperationsPerInvoke = 10000, Description = "RPN实现")]
        public void RunRPNEvaluator()
        {
            foreach (var expression in expressions)
            {
                rnpEvaluator.Evaluate(expression);
            }
        }

        [Benchmark(OperationsPerInvoke = 10000, Description = "BST实现")]
        public void RunETEvaluator()
        {
            foreach (var expression in expressions)
            {
                etEvaluator.Evaluate(expression);
            }
        }
    }
}