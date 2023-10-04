using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointSystem.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PointSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PointController : ControllerBase
    {
        private readonly PosContext _context;

        public PointController(
            PosContext context
            )
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("Calculate")]
        public async Task<IActionResult> Calculate(CalculatePoint model)
        {
            try
            {
                int points = CalculatePoints(model.Items);
                if (points > 0)
                {
                 await AddPoints(model.MemberCode, points);
                }
                await _context.DisposeAsync();

                return Ok("Points Added!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        private async Task AddPoints(string memberCode, int points)
        {
            var member = _context.TblMembers.Where(x => x.MemberCode == memberCode).FirstOrDefault();
            if (member == null)
            {
                throw new Exception("Invalid Member");
            }
            else
            {
                member.TotalPoints += points;
               await _context.SaveChangesAsync();
            }

        }
        private int CalculatePoints(List<PointItem> items)
        {
            int points = 0;

            var itemIds = items.Select(item => item.ItemId).ToList();

            var validItems = _context.TblItems
                .Where(x => itemIds.Contains(x.ItemId) && !x.Alcohol)
                .ToList();

            foreach (var validItem in validItems)
            {
                var pointItem = items.FirstOrDefault(x => x.ItemId == validItem.ItemId);
              
                    var total = validItem.Price * pointItem.Qty;
                    int point_per_item = (int)Math.Floor(total / 10);
                    points += point_per_item;
                
            }

            return points;
        }

    }
}

