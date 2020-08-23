using SRulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRulesEngine
{
    public interface IRuleResultBuilder
    {
        /// <summary>
        /// 生成规则结果
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        RuleResult Build(Rule rule, IDictionary<string, object> input);
    }
}