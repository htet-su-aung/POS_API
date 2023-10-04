using System;
namespace MobileAPI.Models
{
	public class Purchase
	{
        public decimal Amount { get; set; }

        public int? ExchangeCouponId { get; set; }
    }
}

