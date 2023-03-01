using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ConsoleApp5
{
    public interface IEvaluator
    {
        string Evaluate(string expression);
    }
    public class Evaluator : IEvaluator
    {
        /// <summary>
        /// 操作符和优先级
        /// </summary>
        protected static readonly IReadOnlyDictionary<string, int> precedence = new ReadOnlyDictionary<string, int>(new Dictionary<string, int>
        {
            {"(", 0},
            {")", 0},
            {"==", 1},
            {"!=", 1},
            {"##", 1},
            {"!#", 1},
            {">", 2},
            {">=", 2},
            {"<", 2},
            {"<=", 2},
            {"+", 3},
            {"-", 3},
            {"*", 4},
            {"/", 4}
        });

        /// <summary>
        /// 运算表达式
        /// </summary>
        public string Evaluate(string expression)
        {
            //分词并且按照逆波兰表达式排序
            var words = ConvertToPostfix(expression);

            //结果栈
            var stack = new Stack<Word>();

            foreach (var word in words)
            {
                // 如果不是操作符直接入栈
                if (word.Type != WordType.Operator)
                {
                    stack.Push(word);
                }
                //如果是操作符，则出栈两个操作数，并进行相应的运算，然后将结果入栈
                else
                {
                    var right = stack.Pop();
                    var left = stack.Pop();
                    var result = new Word();

                    switch (word.Value)
                    {
                        case "+":
                            if (left.Type == WordType.Number && right.Type == WordType.Number)
                            {
                                result.Value = (decimal.Parse(left.Value) + decimal.Parse(right.Value)).ToString();
                                result.Type = WordType.Number;
                            }
                            else
                            {
                                throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{word}运算");
                            }
                            break;
                        case "-":
                            if (left.Type == WordType.Number && right.Type == WordType.Number)
                            {
                                result.Value = (decimal.Parse(left.Value) - decimal.Parse(right.Value)).ToString();
                                result.Type = WordType.Number;
                            }
                            else
                            {
                                throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{word}运算");
                            }
                            break;
                        case "*":
                            if (left.Type == WordType.Number && right.Type == WordType.Number)
                            {
                                result.Value = (decimal.Parse(left.Value) * decimal.Parse(right.Value)).ToString();
                                result.Type = WordType.Number;
                            }
                            else
                            {
                                throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{word}运算");
                            }
                            break;
                        case "/":
                            if (left.Type == WordType.Number && right.Type == WordType.Number)
                            {
                                result.Value = (decimal.Parse(left.Value) / decimal.Parse(right.Value)).ToString();
                                result.Type = WordType.Number;
                            }
                            else
                            {
                                throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{word}运算");
                            }
                            break;
                        case "==":
                            if (left.Type == WordType.Datetime && right.Type == WordType.Datetime)
                            {
                                result.Value = (DateTime.Parse(left.Value) == DateTime.Parse(right.Value)).ToString();
                            }
                            else if (left.Type == WordType.Boolean && right.Type == WordType.Boolean)
                            {
                                result.Value = (bool.Parse(left.Value) == bool.Parse(right.Value)).ToString();
                            }
                            else if (left.Type == WordType.Number && right.Type == WordType.Number)
                            {
                                result.Value = (decimal.Parse(left.Value) == decimal.Parse(right.Value)).ToString();
                            }
                            else if (left.Type == WordType.String && right.Type == WordType.String)
                            {
                                result.Value = (left.Value == right.Value).ToString();
                            }
                            else
                            {
                                throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{word}运算");
                            }
                            result.Type = WordType.Boolean;
                            break;
                        case "!=":
                            if (left.Type == WordType.Datetime && right.Type == WordType.Datetime)
                            {
                                result.Value = (DateTime.Parse(left.Value) != DateTime.Parse(right.Value)).ToString();
                            }
                            else if (left.Type == WordType.Boolean && right.Type == WordType.Boolean)
                            {
                                result.Value = (bool.Parse(left.Value) != bool.Parse(right.Value)).ToString();
                            }
                            else if (left.Type == WordType.Number && right.Type == WordType.Number)
                            {
                                result.Value = (decimal.Parse(left.Value) != decimal.Parse(right.Value)).ToString();
                            }
                            else if (left.Type == WordType.String && right.Type == WordType.String)
                            {
                                result.Value = (left.Value != right.Value).ToString();
                            }
                            else
                            {
                                throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{word}运算");
                            }
                            result.Type = WordType.Boolean;
                            break;
                        case ">":
                            if (left.Type == WordType.Datetime && right.Type == WordType.Datetime)
                            {
                                result.Value = (DateTime.Parse(left.Value) > DateTime.Parse(right.Value)).ToString();
                            }
                            else if (left.Type == WordType.Number && right.Type == WordType.Number)
                            {
                                result.Value = (decimal.Parse(left.Value) > decimal.Parse(right.Value)).ToString();
                            }
                            else
                            {
                                throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{word}运算");
                            }
                            result.Type = WordType.Boolean;
                            break;
                        case "<":
                            if (left.Type == WordType.Datetime && right.Type == WordType.Datetime)
                            {
                                result.Value = (DateTime.Parse(left.Value) < DateTime.Parse(right.Value)).ToString();
                            }
                            else if (left.Type == WordType.Number && right.Type == WordType.Number)
                            {
                                result.Value = (decimal.Parse(left.Value) < decimal.Parse(right.Value)).ToString();
                            }
                            else
                            {
                                throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{word}运算");
                            }
                            result.Type = WordType.Boolean;
                            break;
                        case ">=":
                            if (left.Type == WordType.Datetime && right.Type == WordType.Datetime)
                            {
                                result.Value = (DateTime.Parse(left.Value) >= DateTime.Parse(right.Value)).ToString();
                            }
                            else if (left.Type == WordType.Number && right.Type == WordType.Number)
                            {
                                result.Value = (decimal.Parse(left.Value) >= decimal.Parse(right.Value)).ToString();
                            }
                            else
                            {
                                throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{word}运算");
                            }
                            result.Type = WordType.Boolean;
                            break;
                        case "<=":
                            if (left.Type == WordType.Datetime && right.Type == WordType.Datetime)
                            {
                                result.Value = (DateTime.Parse(left.Value) <= DateTime.Parse(right.Value)).ToString();
                            }
                            else if (left.Type == WordType.Number && right.Type == WordType.Number)
                            {
                                result.Value = (decimal.Parse(left.Value) <= decimal.Parse(right.Value)).ToString();
                            }
                            else
                            {
                                throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{word}运算");
                            }
                            result.Type = WordType.Boolean;
                            break;
                        case "##":
                            if (left.Type == WordType.StringArray && (right.Type == WordType.String || right.Type == WordType.StringArray))
                            {
                                var array = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(left.Value);
                                if (right.Type == WordType.String)
                                {
                                    result.Value = array.Contains(right.Value).ToString();
                                }
                                else if (right.Type == WordType.StringArray)
                                {
                                    var arrayRight = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(right.Value);
                                    var rsl = true;
                                    foreach (var rightString in arrayRight)
                                    {
                                        if (!rsl)
                                            break;
                                        rsl = rsl && array.Contains(rightString);
                                    }
                                    result.Value = rsl.ToString();
                                }
                                else
                                {
                                    result.Value = false.ToString();
                                }
                                result.Type = WordType.Boolean;
                            }
                            else
                            {
                                throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{word}运算");
                            }
                            break;
                        case "!#":
                            if (left.Type == WordType.StringArray && (right.Type == WordType.String || right.Type == WordType.StringArray))
                            {
                                var array = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(left.Value);
                                if (right.Type == WordType.String)
                                {
                                    result.Value = (!array.Contains(right.Value)).ToString();
                                }
                                else if (right.Type == WordType.StringArray)
                                {
                                    var arrayRight = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(right.Value);
                                    var rsl = true;
                                    foreach (var rightString in arrayRight)
                                    {
                                        if (!rsl)
                                            break;
                                        rsl = rsl && array.Contains(rightString);
                                    }
                                    result.Value = (!rsl).ToString();
                                }
                                else
                                {
                                    result.Value = true.ToString();
                                }
                                result.Type = WordType.Boolean;
                            }
                            else
                            {
                                throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{word}运算");
                            }
                            break;

                    }

                    stack.Push(result);
                }
            }

            // 最后栈中只剩下一个元素，即为最终结果
            return stack.Pop();
        }

        /// <summary>
        /// 分词并按照逆波兰表达式排序
        /// </summary>
        private IEnumerable<Word> ConvertToPostfix(string expression)
        {
            //结果队列
            var result = new Queue<Word>();
            //操作符栈
            var stack = new Stack<Word>();

            //分词和标记
            var words = Tokenize(expression);

            foreach (var word in words)
            {
                if (word.Type == WordType.Unknown)
                    continue;

                //如果不是操作符，则直接入列
                if (word.Type != WordType.Operator)
                {
                    result.Enqueue(word);
                }
                else
                {
                    if (word == ")")
                    {
                        //如果是右括号，则出栈入列直到遇到左括号
                        while (stack.Count > 0 && stack.Peek() != "(")
                        {
                            result.Enqueue(stack.Pop());
                        }
                        //如果是左括号，则出栈并丢弃
                        if (stack.Count > 0 && stack.Peek() == "(")
                        {
                            stack.Pop();
                        }
                    }
                    else
                    {
                        //如果栈顶的操作符优先级大于目前操作符，则需要出栈入列
                        while (stack.Count > 0 && precedence[word.Value] <= precedence[stack.Peek()])
                        {
                            result.Enqueue(stack.Pop());
                        }

                        //将当前操作符入栈
                        stack.Push(word);
                    }
                }
            }

            //将栈内剩余操作符入列
            while (stack.Count > 0)
            {
                result.Enqueue(stack.Pop());
            }

            return result;
        }

        /// <summary>
        /// 分词和标记单词类型
        /// </summary>
        private IEnumerable<Word> Tokenize(string expression)
        {
            var regex = new Regex("[-]?\\d+\\.?\\d*|\"[^\"]*\"|True|False|true|false|\\d{4}[-/]\\d{2}[-/]\\d{2}( \\d{2}:\\d{2}:\\d{2})?|(==)|(!=)|(>=)|(<=)|(##)|(!#)|[\\\\+\\\\-\\\\*/><\\\\(\\\\)]|\\[[^\\[\\]]*\\]");

            foreach (Match match in regex.Matches(expression))
            {
                Word world = match.Value;
                yield return world;
            }
        }


        /// <summary>
        /// 单词
        /// </summary>
        private class Word
        {
            public Word(string value)
            {
                Value = value;
                SetType();
            }

            public Word() { }

            public string Value { get; set; }
            public WordType Type { get; set; }

            public static implicit operator string(Word word)
            {
                return word.ToString();
            }

            public static implicit operator Word(string word)
            {
                return new Word(word);
            }

            public override string ToString()
            {
                return Value;
            }

            private void SetType()
            {
                if (string.IsNullOrWhiteSpace(Value) || Value == string.Empty)
                {
                    Type = WordType.Unknown;
                }
                else if (IsOperator())
                {
                    Type = WordType.Operator;
                }
                else if (IsDatetime())
                {
                    Value = Value.Trim('\"');
                    Type = WordType.Datetime;
                }
                else if (IsString())
                {
                    Value = Value.Trim('\"');
                    Type = WordType.String;
                }
                else if (IsBoolean())
                {
                    Type = WordType.Boolean;
                }
                else if (IsNumber())
                {
                    Type = WordType.Number;
                }
                else if (IsStringArray())
                {
                    Type = WordType.StringArray;
                }
                else
                {
                    Type = WordType.Unknown;
                }
            }

            /// <summary>
            /// 是否操作符
            /// </summary>
            private bool IsOperator()
            {
                return precedence.ContainsKey(Value);
            }

            /// <summary>
            /// 是否是数字（包括负数）
            /// </summary>
            private bool IsNumber()
            {
                return Regex.IsMatch(Value, @"^-?\d+(\.\d+)?$");
            }

            /// <summary>
            /// 是否日期类型(四种格式类型)
            /// </summary>
            private bool IsDatetime()
            {
                return Regex.IsMatch(Value, @"^""\d{4}(-|/)\d{2}(-|/)\d{2}( \d{2}:\d{2}:\d{2})?""$");
            }

            /// <summary>
            /// 是否字符串
            /// </summary>
            private bool IsString()
            {
                return (!IsDatetime()) && Value.StartsWith("\"") && Value.EndsWith("\"");
            }

            /// <summary>
            /// 是否布尔值
            /// </summary>
            private bool IsBoolean()
            {
                return Value == "True" || Value == "False" || Value == "true" || Value == "false";
            }

            /// <summary>
            /// 是否JSON字符串数组
            /// </summary>
            private bool IsStringArray()
            {
                return Regex.IsMatch(Value, @"^\[\s*""([^""\\]*(\\.[^""\\]*)*)""(\s*,\s*""([^""\\]*(\\.[^""\\]*)*)"")*\s*\]$");
            }
        }

        /// <summary>
        /// 单词类型
        /// </summary>
        private enum WordType
        {
            Unknown = 0,
            Number = 1,
            Boolean = 2,
            Datetime = 3,
            String = 4,
            Operator = 5,
            StringArray = 6
        }
    }


}
