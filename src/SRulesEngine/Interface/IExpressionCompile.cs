using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRulesEngine
{
    /// <summary>
    /// 表达式编译器
    /// </summary>
    public interface IExpressionCompile
    {
        /// <summary>
        /// 编译结果
        /// </summary>
        /// <param name="expr">表达式</param>
        /// <param name="input">输入参数</param>
        /// <returns></returns>
        string Compile(string expr, IDictionary<string, object> input);
    }
}
