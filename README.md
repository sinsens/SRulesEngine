# SRulesEngine
 一个简单的 C# 规则引擎，支持 JavaScript 表达式规则验证。
 
 A simple rules engine, writen by C#, support javascript expression with JScript compiler

 ## 用法

 示例：

 Example:
 ```C#

var rand = new Random();
var rule = new Rule()
{
    RuleExpressionType = RuleExpressionType.Lambda,
    RuleExpression = $"price < expectPrice",
    FaildMessage = "股价报价为：#(price)#，预期价位为：#(expectPrice)#，股价不匹配，取消购买",
    SucessMessage = "股价报价为：#(price)#，预期价位为：#(expectPrice)#， 当前余额：#(totalMoney)#，可买入，购买系数：#rate#（余额的比例），买入数量：#(Math.round(totalMoney*rate/price))#，买入后余额：#(totalMoney-Math.round(totalMoney*rate/price)*price)#",
    RuleName = "判断股价，价格匹配就买入",
};
var input = new Dictionary<string, object>();
input["price"] = rand.Next(1, 100);
input["rate"] = rand.NextDouble();
input["expectPrice"] = rand.Next(1, 100);
input["totalMoney"] = rand.Next(1, 100) * rand.Next(2, 200);

var result = (ruleEngine.Execute(rule, input));

 ```

返回结果（需要手动序列化）：

output result (Serialize by your code):
```json
{
	"Success": false,
	"Message": "股价报价为：74，预期价位为：6，股价不匹配，取消购买",
	"Rule": {
		"RuleName": "判断股价，价格匹配就买入",
		"RuleExpression": "price < expectPrice",
		"Success": false,
		"RuleExpressionType": 0,
		"SucessMessage": "股价报价为：#(price)#，预期价位为：#(expectPrice)#， 当前余额：#(totalMoney)#，可买入，购买系数：#rate#（余额的比例），买入数量：#(Math.round(totalMoney*rate/price))#，买入后余额：#(totalMoney-Math.round(totalMoney*rate/price)*price)#",
		"FaildMessage": "股价报价为：#(price)#，预期价位为：#(expectPrice)#，股价不匹配，取消购买",
		"Operator": null,
		"RuleOrder": 0,
		"RuleCompareValue": null,
		"Rules": null
	},
	"RuleInput": {
		"price": 74,
		"rate": 0.69069951059794965,
		"expectPrice": 6,
		"totalMoney": 5525
	},
	"ExceptionMessage": null
}
```