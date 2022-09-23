using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

[Route("[controller]/[action]")]
[ApiController]
public class CustomerController: ControllerBase
    {
    //没啥用的，纯粹用于测试html表单手动提交
    [HttpPut]
    public void UpdateScore(long customer_id, decimal score,bool test = true)
        {
        Console.WriteLine("{0}\t{1}", customer_id, score);
        }

    //需求1
    [HttpPost]
    public async Task<decimal> UpdateScore(long customer_id, decimal score)
        {
        Console.WriteLine("{0}\t{1}", customer_id, score);
        Console.WriteLine("Before UpdateScore-ThreadId = " + Thread.CurrentThread.ManagedThreadId);
        await CustomerModel.UpdateScore(customer_id, score);
        Console.WriteLine("After UpdateScore-ThreadId = " + Thread.CurrentThread.ManagedThreadId);
        return score;
        }

    //把所有Rank全部显示出来，便于与需求2和需求3的执行结果比对
    //浏览器开个标签页https://localhost:7292/Customer/GetAll即可
    [HttpGet]
    public async Task<List<Customer>> GetAll()
        {
        Console.WriteLine("Before GetAll-ThreadId = " + Thread.CurrentThread.ManagedThreadId);
        var res = await CustomerModel.GetCustomerByRank(CustomerModel.RankRange().Item1, CustomerModel.RankRange().Item2);
        Console.WriteLine("After GetAll-ThreadId = " + Thread.CurrentThread.ManagedThreadId);
        return res;
        }

    //需求2
    [HttpGet]
    public async Task<IActionResult> GetCustomerByRank(int start,int end)
        {
        if (start<CustomerModel.RankRange().Item1 || !(start<=end) || CustomerModel.RankRange().Item2<end)
            {
            return NotFound("invalid input range\t输入的区间越界");
            }
        else
            {
            var res = await CustomerModel.GetCustomerByRank(start, end);
            return Ok(res);
            }
        }

    //需求3
    [HttpGet]
    public async Task<IActionResult> GetCustomerById(int id, int high, int low)
        {
        if(!CustomerModel.CustomerIdExsits(id))
            { 
            return NotFound("invalid input id or range\t输入的id有误");
            }
        else
            {
            int start = CustomerModel.RangeIndex(id) - high;
            int end = CustomerModel.RangeIndex(id) + low;
            var res = await GetCustomerByRank(start, end);
            return res;
            }
        }
}
