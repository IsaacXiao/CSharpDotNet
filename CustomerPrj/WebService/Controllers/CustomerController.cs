

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System;


[Route("[controller]/[action]")]
[ApiController]
//这个类在处理非法输入数据时不是抛异常而是返回NotFound
//这样对做前端开发的同事和用户而言都更加友好
public class CustomerController: ControllerBase
    {
    //用于测试和调试html表单手动提交（不用vs自带的swagger）
    [HttpPut]
    public void UpdateScore(long customer_id, decimal score,bool test = true)
        {
        Console.WriteLine("{0}\t{1}", customer_id, score);
        }

    //用于测试和调试（不用vs自带的swagger）
    //浏览器开标签页https://localhost:7292/Customer/GetAll比断点调试更直观
    [HttpGet]
    public async Task<List<Customer>> GetAll()
        {
        return await CustomerModel.GetCustomerByRange(CustomerModel.RankRange().lower_bound, 
                                                                                        CustomerModel.RankRange().upper_bound);
        }

    //用于测试和调试（不用vs自带的swagger）
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
        if (customer_id <= Config.INVALID_CUSTOMERID || score < Config.SCORE_DECREASE || Config.SCORE_INCREASE < score)
            return NotFound("id should be positive and score should be in range of [-1000, +1000]");

        if(false==CustomerModel.CustomerIdExsits(customer_id))
            {
            CustomerModel.AddRecord(new Rank(score, customer_id));
            return Ok(score);
            }
        else
            {
            //这里有个可以考虑优化的地方
            //更新Score有可能不会导致排名发生变化
            //比如 [ 1, 10, 100 ] 这3个数字把10更新为11不会导致前后排名变化
            //写起来有点麻烦，纯粹为做题就免了
            decimal old_score = CustomerModel.RemoveRecord(customer_id);
            CustomerModel.AddRecord(new Rank(old_score+score, customer_id));
            return Ok(old_score+score);
            }
        }

    //需求2
    [HttpGet]
    public async Task<IActionResult> GetCustomerByRank(int start,int end)
        {
        if (start<CustomerModel.RankRange().lower_bound || !(start<=end) || CustomerModel.RankRange().upper_bound<end)
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
    //测试通过2个都默认为0的情况、high为0的情况，low为0的情况
    //以及high为负数的情况、low为负数的情况
    public async Task<IActionResult> GetCustomerById(long customer_id, int high=0, int low=0)
        {
        if(false==CustomerModel.CustomerIdExsits(customer_id))
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

