using SRulesEngine.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SRulesEngine
{
    public class RulesEngine
    {
        private static readonly IExpressionCompile _expressionCompile;
        private static readonly RulesEngine _engine;
        private RuleContext RuleContext;

        static RulesEngine()
        {
            _expressionCompile = new ExpressionCompiler();
            _engine = new RulesEngine();
        }

        public static RulesEngine GetEngine()
        {
            return _engine;
        }

        public RulesEngine()
        {
            RuleContext = new RuleContext();
        }

        public RulesEngine(Rule rule, IDictionary<string, object> input)
        {
            RuleContext = new RuleContext(rule, input);
        }

        public RuleResult Execute()
        {
            var result = RuleContext.Execute();
            return result;
        }

        public RuleResult Execute(IDictionary<string, object> input)
        {
            var result = RuleContext.Execute(input);
            return result;
        }

        public RuleResult Execute(Rule rule, IDictionary<string, object> input)
        {
            var result = RuleContext.Execute(rule, input);
            return result;
        }

        /// <summary>
        /// 任意输入匹配
        /// </summary>
        /// <param name="rule">指定规则</param>
        /// <param name="inputs">数据集</param>
        /// <returns></returns>
        public RuleResult ExecuteAny(Rule rule, IList<IDictionary<string, object>> inputs)
        {
            foreach (var input in inputs)
            {
                var result = RuleContext.Execute(rule, input);
                if (result.Success)
                {
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// 任意规则匹配
        /// </summary>
        /// <param name="rules">规则集</param>
        /// <param name="input">输入数据</param>
        /// <returns></returns>
        public RuleResult ExecuteAny(IList<Rule> rules, IDictionary<string, object> input)
        {
            foreach (var rule in rules)
            {
                var result = RuleContext.Execute(rule, input);
                if (result.Success)
                {
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// 数据匹配所有规则
        /// </summary>
        /// <param name="rules">规则集</param>
        /// <param name="input">输入数据</param>
        /// <returns></returns>
        public IList<RuleResult> Execute(IList<Rule> rules, IDictionary<string, object> input)
        {
            var results = new List<RuleResult>();
            foreach (var rule in rules)
            {
                var result = RuleContext.Execute(rule, input);
                results.Add(result);
            }
            return results;
        }

        public string Execute(string expr, IDictionary<string, object> input)
        {
            var result = _expressionCompile.Compile(expr, input);
            return result;
        }

        /// <summary>
        /// 批量执行工作集
        /// </summary>
        /// <param name="workflowRules"></param>
        /// <returns></returns>
        public IList<RuleResult> Execute(WorkflowRules workflowRules)
        {
            if (workflowRules.Rules == null || workflowRules.RuleInputs == null)
            {
                throw new ArgumentNullException("规则集或数据集不能为 null");
            }
            if (workflowRules.Rules.Count != workflowRules.RuleInputs.Count)
            {
                throw new ArgumentOutOfRangeException($"规则集与数据集数量不一致：{workflowRules.Rules.Count}!={workflowRules.RuleInputs.Count}");
            }
            int length = workflowRules.Rules.Count;
            for (int i = 0; i < length; i++)
            {
                workflowRules.RuleResults.Add(Execute(workflowRules.Rules[i], workflowRules.RuleInputs[i]));
            }
            return workflowRules.RuleResults;
        }
    }
}