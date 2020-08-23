using SRulesEngine.Models;
using System;
using System.Collections.Generic;

namespace SRulesEngine.RuleExecutors
{
    /// <summary>
    /// Lamda 表达式执行器
    /// </summary>
    public class LambdaRuleExecutor : IRuleExecutor
    {
        private static readonly IExpressionCompile expressionCompiler = new ExpressionCompiler();
        private static readonly IRuleResultBuilder ruleResultBuilder = new ResultBuilder();

        public RuleResult Execute(Rule rule, IDictionary<string, object> input)
        {
            var compileExpr = expressionCompiler.Compile(rule.RuleExpression, input);
            var obj = ExpressionEvaluator.Eval(compileExpr);
            rule.Success = Convert.ToBoolean(obj);
            RuleResult result = ruleResultBuilder.Build(rule, input);
            return result;
        }
    }
}