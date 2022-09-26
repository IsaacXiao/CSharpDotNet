
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System;

[Route("[controller]/[action]")]
[ApiController]
public class CustomerController: ControllerBase
    {
    //没啥用的，纯粹用于测试html表单手动提交（不用swagger）
    //TODO: 考虑是否照题目的url提交试试
    [HttpPut]
    public void UpdateScore(long customer_id, decimal score,bool test = true)
        {
        Console.WriteLine("{0}\t{1}", customer_id, score);
        }

    //需求1
    [HttpPost]
    //TODO：score是偏移量，不是最终返回的score
    public async Task<IActionResult> UpdateScore(long customer_id, decimal score)
        {
        if (customer_id < 0 || score < -1000 || 1000 < score)
            return NotFound("id should be positive and score should be in range of [-1000, +1000]");
 
        if(!CustomerModel.CustomerIdExsits(customer_id))
            {
            await CustomerModel.AddRecord(new Rank(score, customer_id));
            return Ok(score);
            }
        else
            {
            await CustomerModel.RemoveRecord(customer_id);
            await CustomerModel.AddRecord(new Rank(score, customer_id));
            return Ok(score);
            }
        }

    //把所有Rank全部显示出来，便于与需求2和需求3的执行结果比对
    //浏览器开个标签页https://localhost:7292/Customer/GetAll即可
    [HttpGet]
    public async Task<List<Customer>> GetAll()
        {
        var res = await CustomerModel.GetCustomerByRange(CustomerModel.RankRange().Item1, CustomerModel.RankRange().Item2);
        return res;
        }

    //需求2
    [HttpGet]
    //TODO: 测试start和end为负数的情形
    public async Task<IActionResult> GetCustomerByRank(int start,int end)
        {
        //TODO: 调用前后打印线程ID
        if (start<CustomerModel.RankRange().Item1 || !(start<=end) || CustomerModel.RankRange().Item2<end)
            {
            return NotFound("invalid input range\t输入的区间越界");
            }
        else
            {
            var res = await CustomerModel.GetCustomerByRange(start, end);
            return Ok(res);
            }
        }

    //需求3
    [HttpGet]
    //TODO: 这里需要测试下2个都默认为0的情况、high为0的情况，low为0的情况
    public async Task<IActionResult> GetCustomerById(int id, int high = 0, int low = 0)
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
