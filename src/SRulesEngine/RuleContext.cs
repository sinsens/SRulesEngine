using SRulesEngine.Models;
using SRulesEngine.RuleExecutors;
using System.Collections.Generic;
using System.Linq;

namespace SRulesEngine
{
    internal class RuleContext
    {
        private static readonly Dictionary<string, IRuleExecutor> containers;

        private IRuleExecutor executor;
        private Rule rule;
        private IDictionary<string, object> input;

        static RuleContext()
        {
            containers = new Dictionary<string, IRuleExecutor>(5);
        }

        public RuleContext()
        {
        }

        public RuleContext(IRuleExecutor ruleExecutor)
        {
            this.executor = ruleExecutor;
        }

        public RuleContext(Rule rule)
        {
            this.rule = rule;
        }

        public RuleContext(Rule rule, IDictionary<string, object> input)
        {
            this.rule = rule;
            this.input = input;
            this.executor = GetRuleExecutor(rule);
        }

        public RuleResult Execute()
        {
            var result = executor.Execute(this.rule, this.input);
            return result;
        }

        public RuleResult Execute(Rule rule, IDictionary<string, object> input)
        {
            var exe = GetRuleExecutor(rule);
            var result = exe.Execute(rule, input);
            return result;
        }

        public RuleResult Execute(IDictionary<string, object> input)
        {
            RuleResult ruleResult;
            if (this.executor != null)
            {
                ruleResult = executor.Execute(this.rule, input);
                return ruleResult;
            }
            else
            {
                var exe = GetRuleExecutor(rule);
                ruleResult = exe.Execute(this.rule, input);
            }

            return ruleResult;
        }

        public static IRuleExecutor GetRuleExecutor(Rule rule)
        {
            if (containers.ContainsKey(rule.RuleExpressionType.ToString()))
            {
                return containers[rule.RuleExpressionType.ToString()];
            }
            IRuleExecutor ruleExecutor = null;
            switch (rule.RuleExpressionType)
            {
                default:
                    ruleExecutor = new LambdaRuleExecutor();
                    break;
                /// 定义的其他类型
                case RuleExpressionType.CompareNotContains:
                case RuleExpressionType.CompareContains:
                case RuleExpressionType.CompareEquals:
                case RuleExpressionType.CompareNotEquals:
                case RuleExpressionType.ValueNullOrEmpty:
                case RuleExpressionType.ValueExists:
                case RuleExpressionType.CompareGreater:
                case RuleExpressionType.CompareSmaller:
                    if (containers.Any())
                    {
                        var excutor = containers.First(exe => exe.Value.GetType() == typeof(CompareRuleExecutor));
                        if (excutor.Value != null)
                            return excutor.Value;
                    }
                    ruleExecutor = new CompareRuleExecutor();
                    break;
            }
            if (containers.ContainsKey(rule.RuleExpressionType.ToString()))
            {
                return containers[rule.RuleExpressionType.ToString()];
            }
            containers.Add(rule.RuleExpressionType.ToString(), ruleExecutor);
            return ruleExecutor;
        }
    }
}