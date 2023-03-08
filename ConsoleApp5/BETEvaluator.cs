using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ConsoleApp5
{
    public class BETEvaluator : IEvaluator
    {
        private const string OPERATOR_LEFT_PARENTHESIS = "(";
        private const string OPERATOR_RIGHT_PARENTHESIS = ")";
        private const string OPERATOR_AND = "&&";
        private const string OPERATOR_OR = "||";
        private const string OPERATOR_EQUAL = "==";
        private const string OPERATOR_NOT_EQUAL = "!=";
        private const string OPERATOR_CONTAIN = "##";
        private const string OPERATOR_NOT_CONTAIN = "!#";
        private const string OPERATOR_GREATER_THAN = ">";
        private const string OPERATOR_GREATER_THAN_OR_EQUAL_TO = ">=";
        private const string OPERATOR_LESS_THAN = "<";
        private const string OPERATOR_LESS_THAN_OR_EQUAL_TO = "<=";
        private const string OPERATOR_ADD = "+";
        private const string OPERATOR_SUBTRACT = "-";
        private const string OPERATOR_MULTIPLY = "*";
        private const string OPERATOR_DIVIDE = "/";
        private const char CHAR_DOUBLE_QUOTE = '\"';

        private const byte PRECEDENCE_0 = 0;
        private const byte PRECEDENCE_1 = 1;
        private const byte PRECEDENCE_2 = 2;
        private const byte PRECEDENCE_3 = 3;
        private const byte PRECEDENCE_4 = 4;
        private const byte PRECEDENCE_5 = 5;
        /// <summary>
        /// 操作符和优先级
        /// </summary>
        protected static readonly IReadOnlyDictionary<string, byte> precedence = new ReadOnlyDictionary<string, byte>(new Dictionary<string, byte>
        {
            // (
            {OPERATOR_LEFT_PARENTHESIS, PRECEDENCE_0},
            // )
            {OPERATOR_RIGHT_PARENTHESIS, PRECEDENCE_0},
            // &&
            {OPERATOR_AND, PRECEDENCE_1},
            // ||
            {OPERATOR_OR, PRECEDENCE_1},
            // ==
            {OPERATOR_EQUAL, PRECEDENCE_2},
            // !=
            {OPERATOR_NOT_EQUAL, PRECEDENCE_2},
            // ##
            {OPERATOR_CONTAIN, PRECEDENCE_2},
            // !#
            {OPERATOR_NOT_CONTAIN, PRECEDENCE_2},
            // >
            {OPERATOR_GREATER_THAN, PRECEDENCE_3},
            // >=
            {OPERATOR_GREATER_THAN_OR_EQUAL_TO, PRECEDENCE_3},
            // <
            {OPERATOR_LESS_THAN, PRECEDENCE_3},
            // <=
            {OPERATOR_LESS_THAN_OR_EQUAL_TO, PRECEDENCE_3},
            // +
            {OPERATOR_ADD, PRECEDENCE_4},
            // -
            {OPERATOR_SUBTRACT, PRECEDENCE_4},
            // *
            {OPERATOR_MULTIPLY, PRECEDENCE_5},
            // /
            {OPERATOR_DIVIDE, PRECEDENCE_5}
        });

        /// <summary>
        /// 计算表达式
        /// </summary>
        public string Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                const string nullExceptionMessage = "{0}不能为 null 或空白。";
                throw new ArgumentException(string.Format(nullExceptionMessage, nameof(expression)));
            }
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
                case OPERATOR_ADD:
                    if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        result.Value = (decimal.Parse(left.Value) + decimal.Parse(right.Value)).ToString();
                        result.Type = ExpressionNodeType.Number;
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    break;
                case OPERATOR_SUBTRACT:
                    if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        result.Value = (decimal.Parse(left.Value) - decimal.Parse(right.Value)).ToString();
                        result.Type = ExpressionNodeType.Number;
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    break;
                case OPERATOR_MULTIPLY:
                    if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        result.Value = (decimal.Parse(left.Value) * decimal.Parse(right.Value)).ToString();
                        result.Type = ExpressionNodeType.Number;
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    break;
                case OPERATOR_DIVIDE:
                    if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        result.Value = (decimal.Parse(left.Value) / decimal.Parse(right.Value)).ToString();
                        result.Type = ExpressionNodeType.Number;
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    break;
                case OPERATOR_EQUAL:
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
                        ThrowNotSupportException(left, right, @operator);
                    }
                    result.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_NOT_EQUAL:
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
                        ThrowNotSupportException(left, right, @operator);
                    }
                    result.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_GREATER_THAN:
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
                        ThrowNotSupportException(left, right, @operator);
                    }
                    result.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_LESS_THAN:
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
                        ThrowNotSupportException(left, right, @operator);
                    }
                    result.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_GREATER_THAN_OR_EQUAL_TO:
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
                        ThrowNotSupportException(left, right, @operator);
                    }
                    result.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_LESS_THAN_OR_EQUAL_TO:
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
                        ThrowNotSupportException(left, right, @operator);
                    }
                    result.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_CONTAIN:
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
                    }
                    else if (left.Type == ExpressionNodeType.String && right.Type == ExpressionNodeType.String)
                    {
                        result.Value = (left.Value.Contains(right.Value)).ToString();
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    result.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_NOT_CONTAIN:
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
                    }
                    else if (left.Type == ExpressionNodeType.String && right.Type == ExpressionNodeType.String)
                    {
                        result.Value = (!left.Value.Contains(right.Value)).ToString();
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    result.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_AND:
                    if (left.Type == ExpressionNodeType.Boolean && right.Type == ExpressionNodeType.Boolean)
                    {
                        result.Value = (bool.Parse(left.Value) && bool.Parse(right.Value)).ToString();
                        result.Type = ExpressionNodeType.Boolean;
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    break;
                case OPERATOR_OR:
                    if (left.Type == ExpressionNodeType.Boolean && right.Type == ExpressionNodeType.Boolean)
                    {
                        result.Value = (bool.Parse(left.Value) || bool.Parse(right.Value)).ToString();
                        result.Type = ExpressionNodeType.Boolean;
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    break;

            }

            return result;
        }

        private void ThrowNotSupportException(ExpressionNode left, ExpressionNode right, ExpressionNode @operator)
        {
            const string message = "不支持{0}类型数据{1}和{2}类型数据{3}进行{4}运算";
            throw new NotSupportedException(string.Format(message, left.Type, left.Value, right.Type, right.Value, @operator.ToString()));
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

                //如果不是操作符，则直接压到结果栈
                if (node.Type != ExpressionNodeType.Operator)
                {
                    result.Push(node);
                }
                else
                {
                    if (node == OPERATOR_RIGHT_PARENTHESIS)
                    {
                        //如果当前操作符是右括号)，则在操作符栈出栈，从结果栈出栈为子节点，
                        //设置完子节点后，把操作符栈压到结果栈
                        while (stack.Count > 0 && stack.Peek() != OPERATOR_LEFT_PARENTHESIS)
                        {
                            SetChildNode();
                        }
                        //如果是左括号，则出栈并丢弃
                        if (stack.Count > 0 && stack.Peek() == OPERATOR_LEFT_PARENTHESIS)
                        {
                            stack.Pop();
                        }
                    }
                    else
                    {
                        //如果栈顶的操作符优先级大于目前操作符，则在操作符栈出栈，
                        //从结果栈出栈为子节点，设置完子节点后，把操作符栈压到结果栈
                        while (node != OPERATOR_LEFT_PARENTHESIS && stack.Count > 0 && precedence[node.Value] <= precedence[stack.Peek()])
                        {
                            SetChildNode();
                        }

                        //将当前操作符压到操作符栈
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
            const string pattern = "[-]?\\d+\\.?\\d*|\"[^\"]*\"|True|False|true|false|\\d{4}[-/]\\d{2}[-/]\\d{2}( \\d{2}:\\d{2}:\\d{2})?|(==)|(!=)|(>=)|(<=)|(##)|(!#)|(&&)|(\\|\\|)|[\\\\+\\\\\\-\\\\*/><\\\\(\\\\)]|\\[[^\\[\\]]*\\]";
            const int zero = 0;
            var regex = new Regex(pattern);

            var matches = regex.Matches(expression);

            var parenthesis = zero;

            var result = new ExpressionNode[matches.Count];

            for (var i = zero; i < matches.Count; i++)
            {
                ExpressionNode value = matches[i].Value;
                if (value == OPERATOR_LEFT_PARENTHESIS)
                    parenthesis++;
                else if (value == OPERATOR_RIGHT_PARENTHESIS)
                    parenthesis--;

                result[i] = value;
            }

            if (parenthesis != zero)
            {
                const string exceptionMessage = "表达式错误!表达式括号数量不相等";
                throw new ArgumentException(exceptionMessage);
            }

            return result;
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
                    Value = Value.Trim(CHAR_DOUBLE_QUOTE);
                    Type = ExpressionNodeType.Datetime;
                }
                else if (IsString())
                {
                    Value = Value.Trim(CHAR_DOUBLE_QUOTE);
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
                const string pattern = @"^-?\d+(\.\d+)?$";
                return Regex.IsMatch(Value, pattern);
            }

            /// <summary>
            /// 是否日期类型(四种格式类型)
            /// </summary>
            private bool IsDatetime()
            {
                const string pattern = @"^""\d{4}(-|/)\d{2}(-|/)\d{2}( \d{2}:\d{2}:\d{2})?""$";
                return Regex.IsMatch(Value, pattern);
            }

            /// <summary>
            /// 是否字符串
            /// </summary>
            private bool IsString()
            {
                return (!IsDatetime()) && Value.StartsWith(CHAR_DOUBLE_QUOTE) && Value.EndsWith(CHAR_DOUBLE_QUOTE);
            }

            /// <summary>
            /// 是否布尔值
            /// </summary>
            private bool IsBoolean()
            {
                const string TRUE_0 = "True";
                const string FALSE_0 = "False";
                const string TRUE_1 = "true";
                const string FALSE_1 = "false";

                return Value == TRUE_0 || Value == FALSE_0 || Value == TRUE_1 || Value == FALSE_1;
            }

            /// <summary>
            /// 是否JSON字符串数组
            /// </summary>
            private bool IsStringArray()
            {
                const string pattern = @"^\[\s*""([^""\\]*(\\.[^""\\]*)*)""(\s*,\s*""([^""\\]*(\\.[^""\\]*)*)"")*\s*\]$";
                return Regex.IsMatch(Value, pattern);
            }
        }

        /// <summary>
        /// 节点类型
        /// </summary>
        private enum ExpressionNodeType : byte
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
