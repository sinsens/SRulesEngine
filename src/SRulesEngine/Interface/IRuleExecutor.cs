using SRulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRulesEngine
{
    /// <summary>
    /// 规则执行器
    /// </summary>
    public interface IRuleExecutor
    {
        /// <summary>
        /// 执行规则
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        RuleResult Execute(Rule rule, IDictionary<string, object> input);
    }
}
