using System;

namespace SRulesEngine.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SRuleEngineTest.Test(() => SRuleEngineTest.LambdaStockWorkFlowTest(), "股票交易工作流", 3, 10000, true);
            SRuleEngineTest.Test(() => SRuleEngineTest.LambdaStockTest(), "股票交易", 3, 100000, true);
            SRuleEngineTest.Test(() => SRuleEngineTest.CompareSmallerTest(), "股票交易", 3, 100000, true);
            SRuleEngineTest.Test(() => SRuleEngineTest.CompareGreaterTest(), "$(totalMoney) 是否大于 20", 5);
            SRuleEngineTest.Test(() => SRuleEngineTest.CompareEquals(), "$(totalMoney) 是否等于 20", 5);
            Console.WriteLine("测试结束，按任意键退出");
            Console.Read();
        }
    }
}