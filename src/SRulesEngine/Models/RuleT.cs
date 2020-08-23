using System.Collections.Generic;

namespace SRulesEngine.Models
{
    /// <summary>
    /// 规则，泛型版本，需要制定参数类型
    /// </summary>
    public class RuleT<T>
    {
        public RuleT()
        {
            RuleInput = new Dictionary<string, T>();
        }

        /// <summary>
        /// 规则名称
        /// </summary>
        public string RuleName { get; set; }

        /// <summary>
        /// 规则表达式
        /// </summary>
        public string RuleExpression { get; set; }

        public bool IsSucess { get; set; }

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

        /// <summary>
        /// 解析后的输入结果
        /// </summary>
        public IDictionary<string, T> RuleInput { get; set; }

        public T GetInputValue(string name)
        {
            T value = default(T);
            if (RuleInput.ContainsKey(name))
            {
                value = RuleInput[name];
            }
            return value;
        }
    }
}