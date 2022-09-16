using Microsoft.AspNetCore.Mvc;
using System.Globalization;

[Route("[controller]/[action]")]
[ApiController]
public class CustomerController: ControllerBase
    {
    //需求1
    [HttpPost]
    public Customer? UpdateScore(long id, long score)
        {
        //post请求回显不出来
        return new Customer(id, score + 100, 1);
        }
    //需求2
    [HttpGet]
    public IEnumerable<Customer> GetCustomerByRank(int start,int end)
        {
        IEnumerable<Rank> res = COMMON.LeaderBoard.Skip(start-1);
        long rank = start;

        foreach (var customer in res)
            {
            if (rank <= end)
                yield return new Customer(customer.customer_id_, customer.score_, rank++);
            }
        }
    //需求3
    [HttpGet]
    public Customer? GetCustomerById(long id,long low, long high)
        {
        //能返回客户端回显出来就意味着在代码里拿到这3个数字了
        return new Customer(id, low,high);
        }
    }

