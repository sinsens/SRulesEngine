using System;
using System.CodeDom.Compiler;
using System.Reflection;

namespace SRulesEngine
{
    /// <summary>
    /// JS 表达式执行器 (eval)
    /// fork from https://www.cnblogs.com/SealedLove/archive/2008/11/20/1337745.html
    /// </summary>
    public class ExpressionEvaluator
    {
        /// <summary>
        /// 计算结果,如果表达式出错则抛出异常
        /// </summary>
        /// <param name="statement">表达式,如"1+2+3+4"</param>
        /// <returns>结果</returns>
        public static object Eval(string statement)
        {
            var result = _evaluatorType.InvokeMember("Eval", BindingFlags.InvokeMethod, null, _evaluator, new object[] { statement });
            return result;
        }

        /// <summary>
        /// 执行并返回一个 bool 值
        /// </summary>
        /// <param name="statment"></param>
        /// <returns></returns>
        public static bool EvalBoolean(string statment)
        {
            var result = ExpressionEvaluator.Eval(statment);
            return Convert.ToBoolean(result);
        }

        public static decimal EvalDecimal(string statment)
        {
            var result = ExpressionEvaluator.Eval(statment);
            return Convert.ToDecimal(result);
        }

        public static int EvalInt(string statment)
        {
            var result = ExpressionEvaluator.Eval(statment);
            return Convert.ToInt32(result);
        }

        static ExpressionEvaluator()
        {
            //构造JScript的编译驱动代码
            CodeDomProvider provider = CodeDomProvider.CreateProvider("JScript");

            CompilerParameters parameters;
            parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;

            CompilerResults results;
            results = provider.CompileAssemblyFromSource(parameters, _jscriptSource);

            Assembly assembly = results.CompiledAssembly;
            _evaluatorType = assembly.GetType("Evaluator");

            _evaluator = Activator.CreateInstance(_evaluatorType);
        }

        private static object _evaluator = null;
        private static Type _evaluatorType = null;

        /// <summary>
        /// JScript代码
        /// </summary>
        private static readonly string _jscriptSource =
            @"class Evaluator
              {
                  public function Eval(expr : String) : String
                  {
                     return eval(expr);
                  }
              }";
    }
}