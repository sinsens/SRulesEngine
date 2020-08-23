using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRulesEngine.Models
{
    /// <summary>
    /// 执行结果
    /// </summary>
    public class RuleResult
    {
        public RuleResult()
        { }

        /// <summary>
        /// 执行结果
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 返回的信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 绑定的规则
        /// </summary>
        public Rule Rule { get; set; }

        /// <summary>
        /// 输入参数
        /// </summary>
        public IDictionary<string, object> RuleInput { get; set; }

        public string ExceptionMessage { get; internal set; }
    }
}