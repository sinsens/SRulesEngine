#define ENABLE_CACHE // 是否启用内存缓存，不启用就用 // 注释掉

#if ENABLE_CACHE
#warning  表达式编译器启用了内存缓存，如果想关闭它，请注释本文件头部的 #define ENABLE_CACHE 指令
#endif

using System;
using System.Collections.Generic;
using System.IO;
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
        /// 是否开启缓存
        /// </summary>
        private static readonly string enableCache = "disabled";

        /// <summary>
        /// 缓存持续时间：秒
        /// </summary>
        private static readonly int CacheAlive = 60;

        private static bool EnableCache
        {
            get { return enableCache != "disabled"; }
        }

#endif

        /// <summary>
        /// 表达式标记字符
        /// </summary>
        public static readonly char MarkChar = '#';

        // 本来打算用正则提取， 奈何功力不够，提取不出支持复杂公式的表达式
        //private static readonly Regex _paramReg = new Regex("(\\$\\(.*?\\))", RegexOptions.Compiled);

        /// <summary>
        /// 配置文件名称
        /// </summary>
        private static readonly string _configfilename;

        static ExpressionCompiler()
        {
            _configfilename = "Compiler.ini";
            // 读取配置，如果存在的话
            if (File.Exists(_configfilename))
            {
                var configLines = File.ReadAllLines(_configfilename);
                var configDict = new Dictionary<string, string>();
                foreach (var item in configLines)
                {
                    var configline = item.Trim();
                    if (string.IsNullOrWhiteSpace(configline) || configline.StartsWith("#"))
                    {
                        continue;
                    }
                    var config = configline.Split('=');
                    if (config.Length < 2)
                    {
                        continue;
                    }
                    configDict.Add(config[0].Trim(), config[1].Trim());
                }
                if (configDict.Any())
                {
                    foreach (var item in configDict)
                    {
                        var mb = typeof(ExpressionCompiler).GetField(item.Key, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.NonPublic);
                        if (mb != null)
                        {
                            try
                            {
                                object value = item.Value;
                                int result;
                                if (int.TryParse(item.Value, out result))
                                {
                                    mb.SetValue(new ExpressionCompiler(), result);
                                }
                                else
                                {
                                    mb.SetValue(new ExpressionCompiler(), value);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

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
            var cacheKeyName = GetCacheKey(expr, input);

            if (EnableCache && CacheAlive > 0)
            {
                // 判断内存缓存
                if (_expressionCache.Contains(cacheKeyName))
                {
                    return _expressionCache.Get(cacheKeyName) as string;
                }
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

            // JS 表达式(代码)截取、替换，这里性能不够好，得优化
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
            if (EnableCache && CacheAlive > 0)
            {
                _expressionCache.AddOrGetExisting(cacheKeyName, tempBuilder.ToString(), DateTime.Now.AddSeconds(CacheAlive));
            }
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