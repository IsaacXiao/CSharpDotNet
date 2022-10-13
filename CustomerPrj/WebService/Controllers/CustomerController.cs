

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System;


[Route("[controller]/[action]")]
[ApiController]
//for invalid parameter inputs
//I don't throw exception
//because returning NotFound is more user friendly 
public class CustomerController: ControllerBase
    {
    //this function is merely for test
    [HttpPut]
    public void UpdateScore(long customer_id, decimal score,bool test = true)
        {
        Console.WriteLine("{0}\t{1}", customer_id, score);
        }

    //It's more convenient to view Rank table from browser ( visualize it ) instead of breakpoint debugging
    //this function is for:
    //https://localhost:7292/Customer/GetAll
    [HttpGet]
    public async Task<List<Customer>> GetAll()
        {
        return await CustomerModel.GetCustomerByRange(CustomerModel.RankRange().lower_bound, 
                                                                                        CustomerModel.RankRange().upper_bound);
        }

    //It's more convenient to view Index table from browser ( visualize it ) instead of breakpoint debugging
    //this function is for:
    //https://localhost:7292/Customer/IndexedRank
    [HttpGet]
    public IEnumerable<IndexedRank> IndexedRank()
        {
        return CustomerModel.GetIndexedRank();
        }

    //Requirement 1
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
            //here is a point we could consider to optimize
            //there is possibility that updating score won't change ranks
            //for example, as SortedList with 3 numbers [ 1, 10, 100 ]
            //updating the number 10 to 11 does not disrupt its sorted order
            //as for merely doing a homework
            //it's not necessary to implement this secondary logic
            //because it will increase its complexity and downgrade code readability
            decimal old_score = CustomerModel.RemoveRecord(customer_id);
            CustomerModel.AddRecord(new Rank(old_score+score, customer_id));
            return Ok(old_score+score);
            }
        }

    //Requirement 2
    [HttpGet]
    public async Task<IActionResult> GetCustomerByRank(int start,int end)
        {
        if (start<CustomerModel.RankRange().lower_bound || !(start<=end) || CustomerModel.RankRange().upper_bound<end)
            {
            return NotFound("invalid input range\t");
            }
        else
            {
            var res = await CustomerModel.GetCustomerByRange(start, end);
            return Ok(res);
            }
        }

    //Requirement 3
    [HttpGet]
    public async Task<IActionResult> GetCustomerById(long customer_id, int high=0, int low=0)
        {
        if(false==CustomerModel.CustomerIdExsits(customer_id))
            { 
            return NotFound("invalid input id");
            }
        else
            {
            int start = CustomerModel.RangeIndex(customer_id) - high;
            int end = CustomerModel.RangeIndex(customer_id) + low;
            return await GetCustomerByRank(start, end);
            }
        }
    }

