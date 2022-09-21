using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Runtime.CompilerServices;

[Route("[controller]/[action]")]
[ApiController]
public class CustomerController: ControllerBase
    {
    //需求1
    [HttpPost]
    public void UpdateScore(long customer_id, decimal score)
        {
        Rank new_rank = new Rank(score,customer_id);
        if(!CustomerModel.indexed_rank_byid_.ContainsKey(customer_id))
            {
            CustomerModel.leaderboard_.Add(new_rank, null);
            }
        else
            {
            int old_index = CustomerModel.indexed_rank_byid_[customer_id]-1;
            CustomerModel.leaderboard_.RemoveAt(old_index);
            CustomerModel.leaderboard_.Add(new_rank, null);
            CustomerModel.indexed_rank_byid_.TryAdd(new_rank.customerId_,CustomerModel.leaderboard_.IndexOfKey(new_rank)+1);
            }
        }
    //需求2
    [HttpGet]
    public IEnumerable<Customer> GetCustomerByRank(int start,int end)
        {
        for(int rank = start; rank <= end; rank++)
            {
            Rank item = (Rank)(CustomerModel.leaderboard_.GetKey(rank-1));
            yield return new Customer(item.customerId_, item.score_, rank);
            }
        }
    //需求3
    [HttpGet]
    public IEnumerable<Customer> GetCustomerById(int id, int high, int low)
    {
        int start = CustomerModel.indexed_rank_byid_[id] - high;
        int end = CustomerModel.indexed_rank_byid_[id] + low;
        return GetCustomerByRank(start, end);
    }
}
