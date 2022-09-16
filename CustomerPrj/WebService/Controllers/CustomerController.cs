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
        for(int rank = start; rank <= end; rank++)
            {
            Rank item = (Rank)(COMMON.LeaderBoard.GetKey(rank-1));
            yield return new Customer(item.customer_id_, item.score_, rank);
            }
        }
    //需求3
    [HttpGet]
    public IEnumerable<Customer> GetCustomerById(int id, int high, int low)
        {
        int start = COMMON.Indexed[id] - high;
        int end = COMMON.Indexed[id] + low;

        return GetCustomerByRank(start, end);
        }
    }

