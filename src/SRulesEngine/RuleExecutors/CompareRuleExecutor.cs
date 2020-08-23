using SRulesEngine.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace SRulesEngine.RuleExecutors
{
    public class CompareRuleExecutor : IRuleExecutor
    {
        private static readonly IRuleResultBuilder ruleResultBuilder = new ResultBuilder();

        public RuleResult Execute(Rule rule, IDictionary<string, object> input)
        {
            var value = input == null || input.Any() == false ? null : input.First().Value;
            var inputValue = value?.ToString();
            switch (rule.RuleExpressionType)
            {
                case RuleExpressionType.CompareNotContains:
                    rule.Success = inputValue.Contains(rule.RuleCompareValue) == false;
                    break;

                case RuleExpressionType.CompareContains:
                    rule.Success = inputValue.Contains(rule.RuleCompareValue);
                    break;

                case RuleExpressionType.CompareEquals:
                    // 可能是数值类型的，这里做个判断
                    rule.Success = CompareEquals(rule.RuleCompareValue, value);
                    break;

                case RuleExpressionType.CompareNotEquals:
                    rule.Success = string.Equals(rule.RuleCompareValue, inputValue) == false;
                    break;

                case RuleExpressionType.ValueNullOrEmpty:
                    rule.Success = string.IsNullOrWhiteSpace(inputValue);
                    break;

                case RuleExpressionType.ValueExists:
                    rule.Success = value != null;
                    break;

                case RuleExpressionType.CompareGreater:
                    rule.Success = CompareNumber(rule.RuleCompareValue, value) == 1;
                    break;

                case RuleExpressionType.CompareSmaller:
                    rule.Success = CompareNumber(rule.RuleCompareValue, value) == 2;
                    break;

                default:
                    throw new NotImplementedException($"不支持的类型：{rule.RuleExpressionType}");
            }

            RuleResult result = ruleResultBuilder.Build(rule, input);

            return result;
        }

        /// <summary>
        /// 比较两个文本的大小
        /// </summary>
        /// <param name="compareValue"></param>
        /// <param name="input"></param>
        /// <returns>
        /// (compareValue == input) => 0, (compareValue > input) => 1, () => 2
        /// </returns>
        public int CompareNumber(string compareValue, object input)
        {
            try
            {
                var inputType = input?.GetType();
                if (typeof(decimal) == inputType)
                {
                    decimal dA = decimal.Parse(compareValue);
                    decimal dB = decimal.Parse(input?.ToString());
                    if (dA == dB) return 0;
                    if (dA > dB) return 1;
                    return 2;
                }
                if (typeof(long) == inputType)
                {
                    long dA = long.Parse(compareValue);
                    long dB = long.Parse(input?.ToString());
                    if (dA == dB) return 0;
                    if (dA > dB) return 1;
                    return 2;
                }
                if (typeof(int) == inputType)
                {
                    int dA = int.Parse(compareValue);
                    int dB = int.Parse(input?.ToString());
                    if (dA == dB) return 0;
                    if (dA > dB) return 1;
                    return 2;
                }
                if (typeof(float) == inputType)
                {
                    float dA = float.Parse(compareValue);
                    float dB = float.Parse(input?.ToString());
                    if (dA == dB) return 0;
                    if (dA > dB) return 1;
                    return 2;
                }
                if (typeof(double) == inputType)
                {
                    double dA = double.Parse(compareValue);
                    double dB = double.Parse(input?.ToString());
                    if (dA == dB) return 0;
                    if (dA > dB) return 1;
                    return 2;
                }
                throw new ArgumentException($"暂不支持该类型进行比较，参数：{input}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 相等比较
        /// </summary>
        /// <param name="compareValue"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool CompareEquals(string compareValue, object input)
        {
            var inputType = input?.GetType();
            if (typeof(string) == inputType)
            {
                return string.Equals(compareValue, input);
            }
            return CompareNumber(compareValue, input?.ToString()) == 0;
        }
    }
}