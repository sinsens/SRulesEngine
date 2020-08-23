using System.Collections.Generic;

namespace SRulesEngine.Models
{
    /// <summary>
    ///并行处理的工作流
    /// </summary>
    public class WorkflowRules
    {
        public WorkflowRules()
        {
            RuleResults = new List<RuleResult>(5);
        }

        /// <summary>
        /// 工作集名称
        /// </summary>
        public string WorkflowName { get; set; }

        /// <summary>
        /// 规则集，必须与输入集一一对应
        /// </summary>
        public IList<Rule> Rules { get; set; }

        /// <summary>
        ///输入集，必须与规则集一一对应
        /// </summary>
        public IList<IDictionary<string, object>> RuleInputs { get; set; }

        public IList<RuleResult> RuleResults { get; set; }
    }
}