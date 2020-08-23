#define ENABLE_CACHE // 是否启用内存缓存，不启用就用 // 注释掉

#if ENABLE_CACHE
#warning  表达式编译器启用了内存缓存，如果想关闭它，请注释本文件头部的 #define ENABLE_CACHE 指令
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;

namespace SRulesEngine
{
    public class ExpressionCompiler : IExpressionCompile
    {
#if ENABLE_CACHE
        private static readonly MemoryCache _expressionCache = new MemoryCache($"SRulesEngine_{Guid.NewGuid()}");

        /// <summary>
        /// 缓存持续时间：分钟
        /// </summary>
        private static readonly int _expressionCacheAlive = 5;

#endif

        /// <summary>
        /// 表达式标记字符
        /// </summary>
        public static readonly char MarkChar = '#';

        //private static readonly Regex _paramReg = new Regex("(\\$\\(.*?\\))", RegexOptions.Compiled);

        public string Compile(string expr, IDictionary<string, object> input)
        {
            if (string.IsNullOrWhiteSpace(expr))
            {
                return string.Empty;
            }
            else if (input == null || input.Any() == false)
            {
                return expr;
            }

#if ENABLE_CACHE
            // 判断内存缓存
            var cacheKeyName = GetCacheKey(expr, input);
            if (_expressionCache.Contains(cacheKeyName))
            {
                return _expressionCache.Get(cacheKeyName) as string;
            }
#endif

            var tempBuilder = new StringBuilder(expr);
            foreach (var item in input)
            {
                tempBuilder.Replace(item.Key, item.Value.ToString()); // 替换表达式的值
            }
            /*
            var matches = _paramReg.Matches(tempBuilder.ToString());
            foreach (Match item in matches)
            {
                var statment = item.Value.Replace("$", "");
                // 括号补偿
                int leftbracket = statment.Count(c => c == '(');
                int rightbracket = statment.Count(c => c == ')');
                while (leftbracket > rightbracket)
                {
                    statment = string.Concat(statment, ')');
                    leftbracket -= 1;
                }
                var result = ExpressionEvaluator.Eval(statment);
                tempBuilder.Replace(statment, result.ToString());// 替换到输出文本
            }
            */

            // JS 表达式(代码)截取、替换
            var tempStr = tempBuilder.ToString();
            int startIndex = tempStr.IndexOf(MarkChar);
            int nextIndex = tempStr.IndexOf(MarkChar, startIndex + 1);
            while (nextIndex > -1)
            {
                var statment = tempStr.Substring(startIndex + 1, nextIndex - startIndex - 1);
                var result = ExpressionEvaluator.Eval(statment);
                tempBuilder.Replace(statment, result.ToString());// 替换到输出文本
                nextIndex = tempStr.IndexOf(MarkChar, nextIndex + 1);
                if (nextIndex > -1)
                {
                    startIndex = nextIndex;
                    nextIndex = tempStr.IndexOf(MarkChar, startIndex + 1);
                }
            }

#if ENABLE_CACHE
            _expressionCache.AddOrGetExisting(cacheKeyName, tempBuilder.ToString(), DateTime.Now.AddMinutes(_expressionCacheAlive));
#endif
            // 清理标记字符
            tempBuilder.Replace(MarkChar.ToString(), string.Empty);
            return tempBuilder.ToString();
        }

#if ENABLE_CACHE

        private static string GetCacheKey(string expr, IDictionary<string, object> input)
        {
            var keyname = $"{expr}_{string.Join("_", input.Select(item => $"{item.Key}#{item.Value}"))}";
            return keyname;
        }

#endif
    }
}