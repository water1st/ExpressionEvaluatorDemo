﻿using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ConsoleApp5
{
    public class BETEvaluator : IEvaluator
    {
        #region 操作符
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
        #endregion

        #region 优先级
        private const byte PRECEDENCE_0 = 0;
        private const byte PRECEDENCE_1 = 1;
        private const byte PRECEDENCE_2 = 2;
        private const byte PRECEDENCE_3 = 3;
        private const byte PRECEDENCE_4 = 4;
        private const byte PRECEDENCE_5 = 5;
        #endregion

        protected const int INT32_ZERO = 0;

        /// <summary>
        /// 操作符和优先级映射字典
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

                Calculate(leftResult, rightResult, node);
            }

            return node;
        }

        /// <summary>
        /// 计算
        /// </summary>
        private void Calculate(ExpressionNode left, ExpressionNode right, ExpressionNode @operator)
        {
            switch (@operator.Value)
            {
                case OPERATOR_ADD:
                    if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        @operator.Value = (decimal.Parse(left.Value) + decimal.Parse(right.Value)).ToString();
                        @operator.Type = ExpressionNodeType.Number;
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    break;
                case OPERATOR_SUBTRACT:
                    if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        @operator.Value = (decimal.Parse(left.Value) - decimal.Parse(right.Value)).ToString();
                        @operator.Type = ExpressionNodeType.Number;
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    break;
                case OPERATOR_MULTIPLY:
                    if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        @operator.Value = (decimal.Parse(left.Value) * decimal.Parse(right.Value)).ToString();
                        @operator.Type = ExpressionNodeType.Number;
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    break;
                case OPERATOR_DIVIDE:
                    if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        @operator.Value = (decimal.Parse(left.Value) / decimal.Parse(right.Value)).ToString();
                        @operator.Type = ExpressionNodeType.Number;
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    break;
                case OPERATOR_EQUAL:
                    if (left.Type == ExpressionNodeType.Datetime && right.Type == ExpressionNodeType.Datetime)
                    {
                        @operator.Value = (DateTime.Parse(left.Value) == DateTime.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Boolean && right.Type == ExpressionNodeType.Boolean)
                    {
                        @operator.Value = (bool.Parse(left.Value) == bool.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        @operator.Value = (decimal.Parse(left.Value) == decimal.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.String && right.Type == ExpressionNodeType.String)
                    {
                        @operator.Value = (left.Value == right.Value).ToString();
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    @operator.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_NOT_EQUAL:
                    if (left.Type == ExpressionNodeType.Datetime && right.Type == ExpressionNodeType.Datetime)
                    {
                        @operator.Value = (DateTime.Parse(left.Value) != DateTime.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Boolean && right.Type == ExpressionNodeType.Boolean)
                    {
                        @operator.Value = (bool.Parse(left.Value) != bool.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        @operator.Value = (decimal.Parse(left.Value) != decimal.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.String && right.Type == ExpressionNodeType.String)
                    {
                        @operator.Value = (left.Value != right.Value).ToString();
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    @operator.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_GREATER_THAN:
                    if (left.Type == ExpressionNodeType.Datetime && right.Type == ExpressionNodeType.Datetime)
                    {
                        @operator.Value = (DateTime.Parse(left.Value) > DateTime.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        @operator.Value = (decimal.Parse(left.Value) > decimal.Parse(right.Value)).ToString();
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    @operator.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_LESS_THAN:
                    if (left.Type == ExpressionNodeType.Datetime && right.Type == ExpressionNodeType.Datetime)
                    {
                        @operator.Value = (DateTime.Parse(left.Value) < DateTime.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        @operator.Value = (decimal.Parse(left.Value) < decimal.Parse(right.Value)).ToString();
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    @operator.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_GREATER_THAN_OR_EQUAL_TO:
                    if (left.Type == ExpressionNodeType.Datetime && right.Type == ExpressionNodeType.Datetime)
                    {
                        @operator.Value = (DateTime.Parse(left.Value) >= DateTime.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        @operator.Value = (decimal.Parse(left.Value) >= decimal.Parse(right.Value)).ToString();
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    @operator.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_LESS_THAN_OR_EQUAL_TO:
                    if (left.Type == ExpressionNodeType.Datetime && right.Type == ExpressionNodeType.Datetime)
                    {
                        @operator.Value = (DateTime.Parse(left.Value) <= DateTime.Parse(right.Value)).ToString();
                    }
                    else if (left.Type == ExpressionNodeType.Number && right.Type == ExpressionNodeType.Number)
                    {
                        @operator.Value = (decimal.Parse(left.Value) <= decimal.Parse(right.Value)).ToString();
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    @operator.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_CONTAIN:
                    if (left.Type == ExpressionNodeType.StringArray && (right.Type == ExpressionNodeType.String || right.Type == ExpressionNodeType.StringArray))
                    {
                        var array = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(left.Value);
                        if (right.Type == ExpressionNodeType.String)
                        {
                            @operator.Value = array.Contains(right.Value).ToString();
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
                            @operator.Value = rsl.ToString();
                        }
                        else
                        {
                            @operator.Value = false.ToString();
                        }
                    }
                    else if (left.Type == ExpressionNodeType.String && right.Type == ExpressionNodeType.String)
                    {
                        @operator.Value = (left.Value.Contains(right.Value)).ToString();
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    @operator.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_NOT_CONTAIN:
                    if (left.Type == ExpressionNodeType.StringArray && (right.Type == ExpressionNodeType.String || right.Type == ExpressionNodeType.StringArray))
                    {
                        var array = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(left.Value);
                        if (right.Type == ExpressionNodeType.String)
                        {
                            @operator.Value = (!array.Contains(right.Value)).ToString();
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
                            @operator.Value = (!rsl).ToString();
                        }
                        else
                        {
                            @operator.Value = true.ToString();
                        }
                    }
                    else if (left.Type == ExpressionNodeType.String && right.Type == ExpressionNodeType.String)
                    {
                        @operator.Value = (!left.Value.Contains(right.Value)).ToString();
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    @operator.Type = ExpressionNodeType.Boolean;
                    break;
                case OPERATOR_AND:
                    if (left.Type == ExpressionNodeType.Boolean && right.Type == ExpressionNodeType.Boolean)
                    {
                        @operator.Value = (bool.Parse(left.Value) && bool.Parse(right.Value)).ToString();
                        @operator.Type = ExpressionNodeType.Boolean;
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    break;
                case OPERATOR_OR:
                    if (left.Type == ExpressionNodeType.Boolean && right.Type == ExpressionNodeType.Boolean)
                    {
                        @operator.Value = (bool.Parse(left.Value) || bool.Parse(right.Value)).ToString();
                        @operator.Type = ExpressionNodeType.Boolean;
                    }
                    else
                    {
                        ThrowNotSupportException(left, right, @operator);
                    }
                    break;

            }
        }

        /// <summary>
        /// 抛出异常
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        private void ThrowNotSupportException(ExpressionNode left, ExpressionNode right, ExpressionNode @operator)
        {
            const string message = "不支持{0}类型数据{1}和{2}类型数据{3}进行{4}运算";
            throw new NotSupportedException(string.Format(message, left.Type, left.Value, right.Type, right.Value, @operator.ToString()));
        }

        /// <summary>
        /// 构建表达式树
        /// </summary>
        private ExpressionNode BuildTree(Queue<ExpressionNode> nodes)
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


            ExpressionNode previous = null;
            while (nodes.Count > INT32_ZERO)
            {
                var node = nodes.Dequeue();

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
                        //当()没表达式抛出异常
                        if (previous != null && previous == OPERATOR_LEFT_PARENTHESIS)
                        {
                            const string exceptionMessage = "括号内缺少表达式";
                            throw new ArgumentException(exceptionMessage);
                        }

                        //如果当前操作符是右括号)，则在操作符栈出栈，从结果栈出栈为子节点，
                        //设置完子节点后，把操作符栈压到结果栈
                        while (stack.Count > INT32_ZERO && stack.Peek() != OPERATOR_LEFT_PARENTHESIS)
                        {
                            SetChildNode();
                        }
                        //如果是左括号，则出栈并丢弃
                        if (stack.Count > INT32_ZERO && stack.Peek() == OPERATOR_LEFT_PARENTHESIS)
                        {
                            stack.Pop();
                        }
                    }
                    else
                    {
                        //如果栈顶的操作符优先级大于目前操作符，则在操作符栈出栈，
                        //从结果栈出栈为子节点，设置完子节点后，把操作符栈压到结果栈
                        while (node != OPERATOR_LEFT_PARENTHESIS && stack.Count > INT32_ZERO && precedence[node.Value] <= precedence[stack.Peek()])
                        {
                            SetChildNode();
                        }

                        //将当前操作符压到操作符栈
                        stack.Push(node);
                    }
                }

                previous = node;
            }

            while (result.Count > 1)
            {
                if (stack.Count == INT32_ZERO || result.Count < 2)
                {
                    const string errorMessage = "表达式错误，表达式缺少运算符或运算数";
                    throw new ArgumentException(errorMessage);
                }

                SetChildNode();
            }

            return result.Pop();
        }

        /// <summary>
        /// 分词和标记单词类型
        /// </summary>
        private Queue<ExpressionNode> Tokenize(string expression)
        {
            const string pattern = "[-]?\\d+\\.?\\d*|\"[^\"]*\"|True|False|true|false|\\d{4}[-/]\\d{2}[-/]\\d{2}( \\d{2}:\\d{2}:\\d{2})?|(==)|(!=)|(>=)|(<=)|(##)|(!#)|(&&)|(\\|\\|)|[\\\\+\\\\\\-\\\\*/><\\\\(\\\\)]|\\[[^\\[\\]]*\\]";
            var regex = new Regex(pattern);

            var matches = regex.Matches(expression);

            var parenthesis = INT32_ZERO;

            var result = new Queue<ExpressionNode>();
            for (var i = INT32_ZERO; i < matches.Count; i++)
            {
                ExpressionNode value = matches[i].Value;
                const char subtract = '-';
                //处理当没有使用空格分割操作符的时候，数字遇到减号会被分为负数的问题
                if (i > INT32_ZERO && value.Type == ExpressionNodeType.Number && value.Value.Length > INT32_ZERO && value.Value[INT32_ZERO] == subtract
                    && result.Count > INT32_ZERO && result.Last().Type == ExpressionNodeType.Number)
                {
                    result.Enqueue(OPERATOR_SUBTRACT);
                    value.Value = value.Value.TrimStart(subtract);
                    result.Enqueue(value);
                    continue;
                }

                //记录括号，如果左括号就加，右括号就减，最后回正就是左右括号数量对等
                if (value == OPERATOR_LEFT_PARENTHESIS)
                    parenthesis++;
                else if (value == OPERATOR_RIGHT_PARENTHESIS)
                    parenthesis--;

                result.Enqueue(value);
            }

            if (parenthesis != INT32_ZERO)
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
            private const char CHAR_DOUBLE_QUOTE = '\"';
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
                if (word is null)
                {
                    return null;
                }

                return word.ToString();
            }

            public static implicit operator ExpressionNode(string word)
            {
                if (string.IsNullOrWhiteSpace(word) || word == String.Empty)
                {
                    return null;
                }

                return new ExpressionNode(word);
            }

            public override string ToString()
            {
                return Value;
            }

            /// <summary>
            /// 设置类型
            /// </summary>
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
                return float.TryParse(Value, out _);
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
            /// 是否字符串（需要在IsDatetime()为False的分支判断）
            /// </summary>
            private bool IsString()
            {
                return (Value.StartsWith(CHAR_DOUBLE_QUOTE) && Value.EndsWith(CHAR_DOUBLE_QUOTE));
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
                try
                {
                    JsonConvert.DeserializeObject<string[]>(Value);
                    return true;
                }
                catch
                {
                    return false;
                }
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
