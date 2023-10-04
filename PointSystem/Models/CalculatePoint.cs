using System;
namespace PointSystem.Models
{
	public class CalculatePoint
	{
		public required string MemberCode { get; set; }

		public List<PointItem>? Items  { get; set; }
       
    }

    public class PointItem
    {
        public int ItemId { get; set; }
        public int Qty { get; set; }
    }
}

