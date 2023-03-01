using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ConsoleApp5
{
    public class ExpressionTreeEvaluator : IEvaluator
    {
        /// <summary>
        /// 操作符和优先级
        /// </summary>
        protected static readonly IReadOnlyDictionary<string, int> precedence = new ReadOnlyDictionary<string, int>(new Dictionary<string, int>
        {
            {"(", 0},
            {")", 0},
            {"&&", 1},
            {"||", 1},
            {"==", 2},
            {"!=", 2},
            {"##", 2},
            {"!#", 2},
            {">", 3},
            {">=", 3},
            {"<", 3},
            {"<=", 3},
            {"+", 4},
            {"-", 4},
            {"*", 5},
            {"/", 5}
        });

        /// <summary>
        /// 计算表达式
        /// </summary>
        public string Evaluate(string expression)
        {
            //分词
            var tokens = Tokenize(expression);

            //构建表达式树
            var root = BuildTree(tokens);

            //运算表达式树的值
            var result = EvaluateTree(root);

            return result;
        }

        /// <summary>
        /// 计算表达式树的值
        /// </summary>
        private ExpressionNode EvaluateTree(ExpressionNode node)
        {
            if (node.Type == ExpressionNodeType.Operator)
            {
                var leftResult = EvaluateTree(node.Left);
                var rightResult = EvaluateTree(node.Right);

                return Calculate(leftResult, rightResult, node);
            }
            else
            {
                return node;
            }
        }

        /// <summary>
        /// 计算
        /// </summary>
        private ExpressionNode Calculate(ExpressionNode left, ExpressionNode right, ExpressionNode @operator)
        {
            var result = new ExpressionNode();

            switch (@operator.Value)
            {
                case "+":
                    if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        result.Value = (decimal.Parse(left.Value) + decimal.Parse(right.Value)).ToString();
                        result.Type = ExpressionNodeType.Number;
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{@operator}运算");
                    }
                    break;
                case "-":
                    if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        result.Value = (decimal.Parse(left.Value) - decimal.Parse(right.Value)).ToString();
                        result.Type = ExpressionNodeType.Number;
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{@operator}运算");
                    }
                    break;
                case "*":
                    if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        result.Value = (decimal.Parse(left.Value) * decimal.Parse(right.Value)).ToString();
                        result.Type = ExpressionNodeType.Number;
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{@operator}运算");
                    }
                    break;
                case "/":
                    if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        result.Value = (decimal.Parse(left.Value) / decimal.Parse(right.Value)).ToString();
                        result.Type = ExpressionNodeType.Number;
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{@operator}运算");
                    }
                    break;
                case "==":
                    if (left.Type == ExpressionNodeType.Datetime && right.Type == ExpressionNodeType.Datetime)
                    {
                        result.Value = (DateTime.Parse(left.Value) == DateTime.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Boolean && right.Type == ExpressionNodeType.Boolean)
                    {
                        result.Value = (bool.Parse(left.Value) == bool.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        result.Value = (decimal.Parse(left.Value) == decimal.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.String && right.Type == ExpressionNodeType.String)
                    {
                        result.Value = (left.Value == right.Value).ToString();
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{@operator}运算");
                    }
                    result.Type = ExpressionNodeType.Boolean;
                    break;
                case "!=":
                    if (left.Type == ExpressionNodeType.Datetime && right.Type == ExpressionNodeType.Datetime)
                    {
                        result.Value = (DateTime.Parse(left.Value) != DateTime.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Boolean && right.Type == ExpressionNodeType.Boolean)
                    {
                        result.Value = (bool.Parse(left.Value) != bool.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        result.Value = (decimal.Parse(left.Value) != decimal.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.String && right.Type == ExpressionNodeType.String)
                    {
                        result.Value = (left.Value != right.Value).ToString();
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{@operator}运算");
                    }
                    result.Type = ExpressionNodeType.Boolean;
                    break;
                case ">":
                    if (left.Type == ExpressionNodeType.Datetime && right.Type == ExpressionNodeType.Datetime)
                    {
                        result.Value = (DateTime.Parse(left.Value) > DateTime.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        result.Value = (decimal.Parse(left.Value) > decimal.Parse(right.Value)).ToString();
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{@operator}运算");
                    }
                    result.Type = ExpressionNodeType.Boolean;
                    break;
                case "<":
                    if (left.Type == ExpressionNodeType.Datetime && right.Type == ExpressionNodeType.Datetime)
                    {
                        result.Value = (DateTime.Parse(left.Value) < DateTime.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        result.Value = (decimal.Parse(left.Value) < decimal.Parse(right.Value)).ToString();
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{@operator}运算");
                    }
                    result.Type = ExpressionNodeType.Boolean;
                    break;
                case ">=":
                    if (left.Type == ExpressionNodeType.Datetime && right.Type == ExpressionNodeType.Datetime)
                    {
                        result.Value = (DateTime.Parse(left.Value) >= DateTime.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        result.Value = (decimal.Parse(left.Value) >= decimal.Parse(right.Value)).ToString();
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{@operator}运算");
                    }
                    result.Type = ExpressionNodeType.Boolean;
                    break;
                case "<=":
                    if (left.Type == ExpressionNodeType.Datetime && right.Type == ExpressionNodeType.Datetime)
                    {
                        result.Value = (DateTime.Parse(left.Value) <= DateTime.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        result.Value = (decimal.Parse(left.Value) <= decimal.Parse(right.Value)).ToString();
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{@operator}运算");
                    }
                    result.Type = ExpressionNodeType.Boolean;
                    break;
                case "##":
                    if (left.Type == ExpressionNodeType.StringArray && (right.Type == ExpressionNodeType.String || right.Type == ExpressionNodeType.StringArray))
                    {
                        var array = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(left.Value);
                        if (right.Type == ExpressionNodeType.String)
                        {
                            result.Value = array.Contains(right.Value).ToString();
                        }
                        else if (right.Type == ExpressionNodeType.StringArray)
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
                        result.Type = ExpressionNodeType.Boolean;
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{@operator}运算");
                    }
                    break;
                case "!#":
                    if (left.Type == ExpressionNodeType.StringArray && (right.Type == ExpressionNodeType.String || right.Type == ExpressionNodeType.StringArray))
                    {
                        var array = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(left.Value);
                        if (right.Type == ExpressionNodeType.String)
                        {
                            result.Value = (!array.Contains(right.Value)).ToString();
                        }
                        else if (right.Type == ExpressionNodeType.StringArray)
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
                        result.Type = ExpressionNodeType.Boolean;
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{@operator}运算");
                    }
                    break;
                case "&&":
                    if (left.Type == ExpressionNodeType.Boolean && right.Type == ExpressionNodeType.Boolean)
                    {
                        result.Value = (bool.Parse(left.Value) && bool.Parse(right.Value)).ToString();
                        result.Type = ExpressionNodeType.Boolean;
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{@operator}运算");
                    }
                    break;
                case "||":
                    if (left.Type == ExpressionNodeType.Boolean && right.Type == ExpressionNodeType.Boolean)
                    {
                        result.Value = (bool.Parse(left.Value) || bool.Parse(right.Value)).ToString();
                        result.Type = ExpressionNodeType.Boolean;
                    }
                    else
                    {
                        throw new InvalidOperationException($"不支持{left.Type}类型数据{left.Value}和{right.Type}类型数据{right.Value}进行{@operator}运算");
                    }
                    break;

            }

            return result;
        }

        /// <summary>
        /// 构建表达式树
        /// </summary>
        private ExpressionNode BuildTree(IEnumerable<ExpressionNode> nodes)
        {
            //结果栈
            var result = new Stack<ExpressionNode>();
            //操作符栈
            var stack = new Stack<ExpressionNode>();

            void SetChildNode()
            {
                var parent = stack.Pop();
                var right = result.Pop();
                var left = result.Pop();

                parent.Left = left;
                parent.Right = right;

                result.Push(parent);
            }

            foreach (var node in nodes)
            {
                if (node.Type == ExpressionNodeType.Unknown)
                    continue;

                //如果不是操作符，则直接入列
                if (node.Type != ExpressionNodeType.Operator)
                {
                    result.Push(node);
                }
                else
                {
                    if (node == ")")
                    {
                        //如果是右括号，则出栈入列直到遇到左括号
                        while (stack.Count > 0 && stack.Peek() != "(")
                        {
                            SetChildNode();
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
                        while (stack.Count > 0 && precedence[node.Value] <= precedence[stack.Peek()])
                        {
                            SetChildNode();
                        }

                        //将当前操作符入栈
                        stack.Push(node);
                    }
                }
            }

            while (result.Count > 1)
            {
                SetChildNode();
            }

            return result.Pop();
        }

        /// <summary>
        /// 分词和标记单词类型
        /// </summary>
        private IEnumerable<ExpressionNode> Tokenize(string expression)
        {
            var regex = new Regex("[-]?\\d+\\.?\\d*|\"[^\"]*\"|True|False|true|false|\\d{4}[-/]\\d{2}[-/]\\d{2}( \\d{2}:\\d{2}:\\d{2})?|(==)|(!=)|(>=)|(<=)|(##)|(!#)|(&&)|(\\|\\|)|[\\\\+\\\\\\-\\\\*/><\\\\(\\\\)]|\\[[^\\[\\]]*\\]");

            foreach (Match match in regex.Matches(expression))
            {
                yield return match.Value;
            }
        }

        /// <summary>
        /// 节点
        /// </summary>
        private class ExpressionNode
        {
            public ExpressionNode(string value)
            {
                Value = value;
                SetType();
            }

            public ExpressionNode() { }

            public string Value { get; set; }
            public ExpressionNode Left { get; set; }
            public ExpressionNode Right { get; set; }
            public ExpressionNodeType Type { get; set; }

            public static implicit operator string(ExpressionNode word)
            {
                return word.ToString();
            }

            public static implicit operator ExpressionNode(string word)
            {
                return new ExpressionNode(word);
            }

            public override string ToString()
            {
                return Value;
            }

            private void SetType()
            {
                if (string.IsNullOrWhiteSpace(Value) || Value == string.Empty)
                {
                    Type = ExpressionNodeType.Unknown;
                }
                else if (IsOperator())
                {
                    Type = ExpressionNodeType.Operator;
                }
                else if (IsDatetime())
                {
                    Value = Value.Trim('\"');
                    Type = ExpressionNodeType.Datetime;
                }
                else if (IsString())
                {
                    Value = Value.Trim('\"');
                    Type = ExpressionNodeType.String;
                }
                else if (IsBoolean())
                {
                    Type = ExpressionNodeType.Boolean;
                }
                else if (IsNumber())
                {
                    Type = ExpressionNodeType.Number;
                }
                else if (IsStringArray())
                {
                    Type = ExpressionNodeType.StringArray;
                }
                else
                {
                    Type = ExpressionNodeType.Unknown;
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
        /// 节点类型
        /// </summary>
        private enum ExpressionNodeType
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
