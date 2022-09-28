
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
    //用于测试和调试html表单手动提交（不用vs自带的swagger）
    [HttpPut]
    public void UpdateScore(long customer_id, decimal score,bool test = true)
        {
        Console.WriteLine("{0}\t{1}", customer_id, score);
        }

    //用于测试和调试（不用vs自带的swagger）
    //把所有Rank及相关记录全部显示出来，便于与需求2和需求3的执行结果比对
    //浏览器开标签页https://localhost:7292/Customer/GetAll比断点调试更直观
    [HttpGet]
    public async Task<List<Customer>> GetAll()
        {
        return await CustomerModel.GetCustomerByRange(CustomerModel.RankRange().lower_bound, 
                                                                                        CustomerModel.RankRange().upper_bound);
        }

    //用于测试和调试（不用vs自带的swagger）
    //把所有建了索引Index的Rank全部显示出来，便于与需求2和需求3的执行结果比对
    //浏览器开标签页https://localhost:7292/Customer/IndexedRank比断点调试更直观
    [HttpGet]
    public IEnumerable<IndexedRank> IndexedRank()
        {
        return CustomerModel.GetIndexedRank();
        }

    //需求1
    [HttpPost]
    public IActionResult UpdateScore(long customer_id, decimal score)
        {
        //对做前端开发的同事和用户而言
        //用NotFound返回非法参数传入的信息要比抛异常更友好
        if (customer_id <= Config.INVALID_CUSTOMERID || score < Config.SCORE_DECREASE || Config.SCORE_INCREASE < score)
            return NotFound("id should be positive and score should be in range of [-1000, +1000]");
 
        if(!CustomerModel.CustomerIdExsits(customer_id))
            {
            CustomerModel.AddRecord(new Rank(score, customer_id));
            return Ok(score);
            }
        else
            {
            decimal old_score = CustomerModel.RemoveRecord(customer_id);
            CustomerModel.AddRecord(new Rank(old_score+score, customer_id));
            return Ok(old_score+score);
            }
        }

    //需求2
    [HttpGet]
    public async Task<IActionResult> GetCustomerByRank(int start,int end)
        {
        //对做前端开发的同事和用户而言
        //用NotFound返回非法参数传入的信息要比抛异常更友好
        if (start<CustomerModel.RankRange().lower_bound || !(start<=end) || CustomerModel.RankRange().upper_bound<end)
            {
            return NotFound("invalid input range\t输入的区间越界");
            }
        else
            {
            Console.WriteLine("Before Query-ThreadId = " + Thread.CurrentThread.ManagedThreadId);
            var res = await CustomerModel.GetCustomerByRange(start, end);
            Console.WriteLine("After Query-ThreadId = " + Thread.CurrentThread.ManagedThreadId);
            return Ok(res);
            }
        }

    //需求3
    [HttpGet]
    //测试通过2个都默认为0的情况、high为0的情况，low为0的情况
    //以及high为负数的情况、low为负数的情况
    public async Task<IActionResult> GetCustomerById(int customer_id, int high=0, int low=0)
        {
        //对做前端开发的同事和用户而言
        //用NotFound返回非法参数传入的信息要比抛异常更友好
        if(!CustomerModel.CustomerIdExsits(customer_id))
            { 
            return NotFound("invalid input id or range\t输入的id有误");
            }
        else
            {
            int start = CustomerModel.RangeIndex(customer_id) - high;
            int end = CustomerModel.RangeIndex(customer_id) + low;
            return await GetCustomerByRank(start, end);
            }
        }
    }
