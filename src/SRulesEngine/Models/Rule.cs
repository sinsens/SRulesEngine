using System.Collections.Generic;

namespace SRulesEngine.Models
{
    /// <summary>
    /// 规则
    /// </summary>
    public class Rule
    {
        public Rule()
        { }

        /// <summary>
        /// 规则名称
        /// </summary>
        public string RuleName { get; set; }

        /// <summary>
        /// 规则表达式
        /// </summary>
        public string RuleExpression { get; set; }

        public bool Success { get; set; }

        /// <summary>
        /// 表达式类型
        /// </summary>
        public RuleExpressionType? RuleExpressionType { get; set; }

        /// <summary>
        /// 成功结果，可带表达式
        /// </summary>
        public string SucessMessage { get; set; }

        /// <summary>
        /// 失败结果，可带表达式
        /// </summary>
        public string FaildMessage { get; set; }

        public string Operator { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int RuleOrder { get; set; }

        /// <summary>
        /// 比较值
        /// </summary>
        public string RuleCompareValue { get; set; }

        /// <summary>
        /// 绑定的子规则
        /// </summary>
        public IList<Rule> Rules { get; set; }
    }

    public enum RuleExpressionType
    {
        /// <summary>
        /// Lambda 动态表达式
        /// </summary>
        Lambda = 0,

        /// <summary>
        /// 有值
        /// </summary>
        ValueExists = 1,

        /// <summary>
        /// 无值
        /// </summary>
        ValueNullOrEmpty = 2,

        // 通用文本类
        /// <summary>
        /// 相等（文自动类型）
        /// </summary>
        CompareEquals = 10,

        /// <summary>
        /// 不相等（自动类型）
        /// </summary>
        CompareNotEquals = 11,

        /// <summary>
        /// 包含（文本类型比较）
        /// </summary>
        CompareContains = 12,

        /// <summary>
        /// 不包含（文本类型比较）
        /// </summary>
        CompareNotContains = 13,

        // 数值型
        /// <summary>
        /// 大于（数值型）
        /// </summary>
        CompareGreater = 20,

        /// <summary>
        /// 小于（数值型）
        /// </summary>
        CompareSmaller = 21
    }
}