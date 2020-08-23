using SRulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SRulesEngine
{
    public class ResultBuilder : IRuleResultBuilder
    {
        private static readonly IExpressionCompile expressionCompile = new ExpressionCompiler();

        public RuleResult Build(Rule rule, IDictionary<string, object> input)
        {
            RuleResult result = new RuleResult() { RuleInput = input, Rule = rule, Success = rule.Success };

            try
            {
                result.Message = expressionCompile.Compile(result.Success ? rule.SucessMessage : rule.FaildMessage, input).Replace(ExpressionCompiler.MarkChar.ToString(), string.Empty);
            }
            catch (Exception ex)
            {
                result.ExceptionMessage = ex.Message + ex.StackTrace;
            }

            return result;
        }
    }
}