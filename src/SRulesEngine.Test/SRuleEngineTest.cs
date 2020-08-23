using SRulesEngine;
using SRulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace SRulesEngine.Test
{
    public class SRuleEngineTest
    {
        /// <summary>
        /// 默认的测试数
        /// </summary>
        private static int defaultCount = 100000;

        private static bool saveTestData = false;

        /// <summary>
        /// 性能测试数据目录
        /// </summary>
        private static string resultDir = AppDomain.CurrentDomain.BaseDirectory + "output/result/";

        /// <summary>
        /// 性能测试结果输出目录
        /// </summary>
        private static string resultDataDir = AppDomain.CurrentDomain.BaseDirectory + "output/data/";

        private static RulesEngine ruleEngine = RulesEngine.GetEngine();

        static SRuleEngineTest()
        {
            if (Directory.Exists(resultDir) == false)
            {
                Directory.CreateDirectory(resultDir);
            }
            if (Directory.Exists(resultDataDir) == false)
            {
                Directory.CreateDirectory(resultDataDir);
            }
        }

        /// <summary>
        /// 执行一个测试方法
        /// </summary>
        /// <param name="action"></param>
        /// <param name="testName">测试名称</param>
        /// <param name="loop">循环次数</param>
        /// <param name="defaultCount">每次执行数（仅对采用了该参数的方法有效）</param>
        /// <param name="saveTestData">保存测试数据</param>
        public static void Test(Action action, string testName = "测试名称", int loop = 1, int defaultCount = 100000, bool saveTestData = false)
        {
            SRuleEngineTest.saveTestData = saveTestData;
            SRuleEngineTest.defaultCount = defaultCount;

            var sb = new StringBuilder(4096);
            Console.WriteLine("请稍后");
            sb.AppendLine("====================================================");
            sb.AppendLine($"测试名称：{testName}，循环次数：{loop}，每次执行数：{defaultCount}");
            var stopwatchTotal = new Stopwatch();
            stopwatchTotal.Start();
            for (int i = 1; i < loop + 1; i++)
            {
                sb.AppendLine($"第 {i} 次测试开始");
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                action.Invoke();
                stopwatch.Stop();
                sb.AppendLine($"测试方法：{action.Method.Name}，第 {i} 次，耗时：{stopwatch.Elapsed.TotalMinutes} 总分，{stopwatch.Elapsed.TotalSeconds} 总秒，{stopwatch.ElapsedMilliseconds} 总毫秒");
                sb.AppendLine($"性能：{defaultCount / stopwatch.Elapsed.TotalSeconds}/QPS，{defaultCount / stopwatch.ElapsedMilliseconds}/ms");
                sb.AppendLine("---------------------------------------------");
            }
            stopwatchTotal.Stop();
            sb.AppendLine($"测试方法：{action.Method.Name}，循环次数：{loop}，总计耗时：{stopwatchTotal.Elapsed.TotalMinutes} 总分，{stopwatchTotal.Elapsed.TotalSeconds} 总秒，{stopwatchTotal.ElapsedMilliseconds} 总毫秒");
            var totalCount = defaultCount * loop;
            sb.AppendLine($"平均性能：{totalCount / stopwatchTotal.Elapsed.TotalSeconds}/QPS，{totalCount / stopwatchTotal.ElapsedMilliseconds}/ms");
            sb.AppendLine("====================================================");
            if (saveTestData)
            {
                File.WriteAllText($"{resultDir}{testName}.txt", sb.ToString());
            }
            Console.WriteLine(sb.ToString());
            Console.WriteLine("释放内存。。。");
            GC.Collect();
            Console.WriteLine("按任意键继续。。。");
            Console.ReadLine();
        }

        public static void CompareSmallerTest()
        {
            var rand = new Random();
            var rule = new Rule()
            {
                RuleExpressionType = RuleExpressionType.CompareSmaller,
                RuleCompareValue = rand.Next(20, 1000).ToString(),
                FaildMessage = "预期价位为：#(price)#，股价不匹配，取消购买",
                SucessMessage = "股价报价为：#(price)#， 当前余额：#(totalMoney)#，可买入，买入数量：#(Math.round(totalMoney*0.3/price))#，买入后余额：#(totalMoney-Math.round(Math.round(totalMoney*0.3/price)*price))#",
                RuleName = "判断股价，价格匹配就买入",
            };

            var resultList = new List<RuleResult>(defaultCount);

            for (int i = 0; i < defaultCount; i++)
            {
                var dct = new Dictionary<string, object>();
                dct["price"] = rand.Next(1, 100);
                dct["totalMoney"] = rand.Next(1, 100) * rand.Next(2, 200);
                resultList.Add(ruleEngine.Execute(rule, dct));
                rule.RuleCompareValue = rand.Next(20, 100).ToString();
            }
            if (saveTestData)
            {
                File.WriteAllText($"{resultDataDir}{MethodBase.GetCurrentMethod().Name}.txt", JsonConvert.SerializeObject(resultList));
            }
        }

        /// <summary>
        /// Lamda 表达式的股票购买
        /// </summary>
        public static void LambdaStockTest()
        {
            var rand = new Random();

            var resultList = new List<RuleResult>(defaultCount);

            for (int i = 0; i < defaultCount; i++)
            {
                var rule = new Rule()
                {
                    RuleExpressionType = RuleExpressionType.Lambda,
                    RuleExpression = $"price < expectPrice",
                    FaildMessage = "股价报价为：#(price)#，预期价位为：#(expectPrice)#，股价不匹配，取消购买",
                    SucessMessage = "股价报价为：#(price)#，预期价位为：#(expectPrice)#， 当前余额：#(totalMoney)#，可买入，购买系数：#rate#（余额的比例），买入数量：#(Math.round(totalMoney*rate/price))#，买入后余额：#(totalMoney-Math.round(totalMoney*rate/price)*price)#",
                    RuleName = "判断股价，价格匹配就买入",
                };
                var dct = new Dictionary<string, object>();
                dct["price"] = rand.Next(1, 100);
                dct["rate"] = rand.NextDouble();
                dct["expectPrice"] = rand.Next(1, 100);
                dct["totalMoney"] = rand.Next(1, 100) * rand.Next(2, 200);
                resultList.Add(ruleEngine.Execute(rule, dct));
            }
            if (saveTestData)
            {
                File.WriteAllText($"{resultDataDir}{MethodBase.GetCurrentMethod().Name}.txt", JsonConvert.SerializeObject(resultList));
            }
        }

        /// <summary>
        /// Lamda 表达式工作流的股票购买
        /// </summary>
        public static void LambdaStockWorkFlowTest()
        {
            var rand = new Random();

            var workflow = new WorkflowRules()
            {
                RuleInputs = new List<IDictionary<string, object>>(defaultCount),
                Rules = new List<Rule>(defaultCount)
            };

            for (int i = 0; i < defaultCount; i++)
            {
                var rule = new Rule()
                {
                    RuleExpressionType = RuleExpressionType.Lambda,
                    RuleExpression = $"price < expectPrice",
                    FaildMessage = "股价报价为：#(price)#，预期价位为：#(expectPrice)#，股价不匹配，取消购买",
                    SucessMessage = "股价报价为：#(price)#，预期价位为：#(expectPrice)#， 当前余额：#(totalMoney)#，可买入，购买系数：#rate#（余额的比例），买入数量：#(Math.round(totalMoney*rate/price))#，买入后余额：#(totalMoney-Math.round(totalMoney*rate/price)*price)#",
                    RuleName = "判断股价，价格匹配就买入",
                };
                var dct = new Dictionary<string, object>();
                dct["price"] = rand.Next(1, 100);
                dct["rate"] = rand.NextDouble();
                dct["expectPrice"] = rand.Next(1, 100);
                dct["totalMoney"] = rand.Next(1, 100) * rand.Next(2, 200);
                workflow.Rules.Add(rule);
                workflow.RuleInputs.Add(dct);
            }
            // 执行
            var resultList = ruleEngine.Execute(workflow);
            if (saveTestData)
            {
                File.WriteAllText($"{resultDataDir}{MethodBase.GetCurrentMethod().Name}.txt", JsonConvert.SerializeObject(resultList));
            }
        }

        public static void CompareGreaterTest()
        {
            var rule = new Rule()
            {
                RuleExpressionType = RuleExpressionType.CompareGreater,
                RuleCompareValue = "20",
                FaildMessage = "总金额 $(totalMoney*count) 小于或等于 20，可以购买",
                SucessMessage = "$(totalMoney*count) 大于 20",
                RuleName = "判断余额大于 20",
            };
            var resultList = new List<RuleResult>(defaultCount);
            var rand = new Random();
            for (int i = 0; i < defaultCount; i++)
            {
                var dct = new Dictionary<string, object>();
                dct["totalMoney"] = rand.Next(1, 100);
                dct["count"] = rand.Next(1, 100);
                resultList.Add(ruleEngine.Execute(rule, dct));
            }
            if (saveTestData)
            {
                File.WriteAllText($"{resultDataDir}{MethodBase.GetCurrentMethod().Name}.txt", JsonConvert.SerializeObject(resultList));
            }
        }

        public static void CompareEquals()
        {
            var rule = new Rule()
            {
                RuleExpressionType = RuleExpressionType.CompareEquals,
                RuleCompareValue = "20",
                FaildMessage = "$(totalMoney) 不等于 20",
                SucessMessage = "$(totalMoney) 等于 20",
                RuleName = "判断余额等于 20",
            };
            var resultList = new List<RuleResult>(defaultCount);
            var rand = new Random();
            for (int i = 0; i < defaultCount; i++)
            {
                var dct = new Dictionary<string, object>();
                dct["totalMoney"] = rand.Next(1, 100);
                dct["count"] = rand.Next(1, 100);
                resultList.Add(ruleEngine.Execute(rule, dct));
            }
            if (saveTestData)
            {
                File.WriteAllText($"{resultDataDir}{MethodBase.GetCurrentMethod().Name}.txt", JsonConvert.SerializeObject(resultList));
            }
        }

        public static void CompareContainsTest()
        {
            var rule = new Rule()
            {
                RuleExpressionType = RuleExpressionType.CompareGreater,
                RuleCompareValue = "20",
                FaildMessage = "$(totalMoney) 小于或等于 20",
                SucessMessage = "$(totalMoney) 大于 20",
                RuleName = "判断余额等于 20",
            };
            var resultList = new List<RuleResult>(defaultCount);
            var rand = new Random();
            for (int i = 0; i < defaultCount; i++)
            {
                var dct = new Dictionary<string, object>();
                dct["totalMoney"] = rand.Next(1, 100);
                dct["count"] = rand.Next(1, 100);
                resultList.Add(ruleEngine.Execute(rule, dct));
            }
            if (saveTestData)
            {
                File.WriteAllText($"{resultDataDir}{MethodBase.GetCurrentMethod().Name}.txt", JsonConvert.SerializeObject(resultList));
            }
        }

        public static void LambdaRuleExecutorTest()
        {
            var inputs = GetInput1();
            var rule = new Rule();
            rule.RuleExpressionType = RuleExpressionType.CompareEquals;
            rule.RuleCompareValue = "20";
            foreach (var item in inputs)
            {
                rule.FaildMessage = "$(totalMoney)不等于 20";
                rule.SucessMessage = "$(totalMoney) 等于 20";

                if (string.Equals(item["totalMoney"], "10"))
                {
                    rule.RuleExpressionType = RuleExpressionType.CompareSmaller;
                    rule.FaildMessage = "$(totalMoney) 大于 20";
                    rule.SucessMessage = "$(totalMoney) 小于 20";
                }
                if (string.Equals(item["totalMoney"], "40"))
                {
                    rule.RuleExpressionType = RuleExpressionType.CompareGreater;
                    rule.FaildMessage = "$(totalMoney) 小于 20";
                    rule.SucessMessage = "$(totalMoney) 大于 20";
                }
                var result = ruleEngine.Execute(rule, item);
                Console.WriteLine($"规则校验通过(?)：{result.Success}, 返回结果：{result.Message}");
            }
        }

        public static IList<IDictionary<string, object>> GetInput1()
        {
            var ls = new List<IDictionary<string, object>>();
            for (int i = 1; i < 5; i++)
            {
                var dct = new Dictionary<string, object>();
                ls.Add(dct);
                dct["totalMoney"] = i * 10;
                dct["count"] = i;
            }
            return ls;
        }
    }
}