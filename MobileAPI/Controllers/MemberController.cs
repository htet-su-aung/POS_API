using Microsoft.AspNetCore.Mvc;
using DataModel;
using MobileAPI.Models;
using MobileAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MobileAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemberController : ControllerBase
{
   
    private readonly PosContext _context;
    private readonly IJwtAuth _jwtAuth;

    public MemberController(
        PosContext context,
        IJwtAuth jwtAuth
        )
    {
        _context = context;
        _jwtAuth = jwtAuth;
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> Register(MemberRegister model)
    {
        try
        {

            string? lastCode = _context.TblMembers.OrderBy(x => x.MemberId).LastOrDefault()?.MemberCode;
            

            var member = new TblMember()
            {
                MemberCode = GetMemberCode(lastCode),
                MemberName = model.MemberName,
                Password = model.Password,
                MobileNumber = model.MobileNumber,
                Email = model.Email,
                Active = true,
                Verified = false,
                OtpCode = GetRandomCode()
            };
           await _context.TblMembers.AddAsync(member);
           await _context.SaveChangesAsync();
           await _context.DisposeAsync();
            return Ok("Register Success!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }


    [AllowAnonymous]
    [HttpPost("Verify")]
    public async Task<IActionResult> Verify(int memberId, string otp)
    {
        try
        {

            var member = _context.TblMembers.Where(x => x.MemberId == memberId).FirstOrDefault();

            if (member?.OtpCode == otp)
            {
                member.Verified = true;
                member.OtpCode = null;
                await _context.SaveChangesAsync();
                await _context.DisposeAsync();

                return Ok("Verify Success!");
            }
            else
            {
                _context.Dispose();
                return BadRequest("Verify Fail!");

            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }


    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(MemberLogin model)
    {
        try
        {
            var member = _context.TblMembers.Where(x => x.MemberName == model.MemberName).FirstOrDefault();
            if (member == null)
            {
                return NotFound("Member Not Found!");
            }
            else if (member.Password != model.Password)
            {
                return Unauthorized("Incorrect Password");
            }
            else if (member.Active == false )
            {
                return Forbid("Member Inactive!");
            }
            else if (member.Verified == false)
            {
                return Forbid("Member Not Verified!");
            }
            else
            {
               var token = _jwtAuth.Authenticate(member.MemberName, member.MemberCode, member.MemberId);
                return Ok(new
                {
                    message = "Login Success",
                    token
                });
            }
            
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [Authorize]
    [HttpPost("Purchase")]
    public async Task<IActionResult> Purchase(Purchase model)
    {
        try
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int memberId = Convert.ToInt32(idStr);
            string invCode;
            do
            {
                invCode = "INV-" + GetRandomCode();
            }
            while (_context.TblPurchaseHistories.Where(x => x.InvoiceNo == invCode).FirstOrDefault() != null);
            var purchase = new TblPurchaseHistory()
            {
                MemberId = memberId,
                Amount = model.Amount,
                ExchangeCouponId = model.ExchangeCouponId,
                PurchaseDate = DateTime.UtcNow,
                InvoiceNo = invCode
            };
            await _context.TblPurchaseHistories.AddAsync(purchase);
            if (model.ExchangeCouponId > 0)
            {
                var exchanged_coupon = _context.TblExchangeCoupons.Where(x => x.ExchangeCouponId == model.ExchangeCouponId).FirstOrDefault();

                if (exchanged_coupon != null)
                {
                    exchanged_coupon.IsUsed = true;
                }
            }
            await _context.SaveChangesAsync();
            await _context.DisposeAsync();
            return Ok("Purchase Success");

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [Authorize]
    [HttpGet("PurchaseHistory")]
    public  IActionResult PurchaseHistory()
    {
        try
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int memberId = Convert.ToInt32(idStr);
            var list = _context.TblPurchaseHistories.Where(x => x.MemberId == memberId)
                .Select(x =>
                new
                {
                    x.PurchaseHistoryId,
                    x.PurchaseDate,
                    x.InvoiceNo,
                    x.Amount
                }
                ).ToList();
            _context.Dispose();
            return Ok(list);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [Authorize]
    [HttpGet("GetAvailableCoupons")]
    public IActionResult GetAvailableCoupons()
    {
        try
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int memberId = Convert.ToInt32(idStr);

            var exchanged_list = _context.TblExchangeCoupons.Where(x => x.MemberId == memberId && x.IsUsed == false)
                .Select(x =>
                new
                {
                    x.ExchangeCouponId,
                    x.Coupon.CouponName,
                    x.Coupon.Amount
                }
                ).ToList();

            var member = _context.TblMembers.Where(x => x.MemberId == memberId).FirstOrDefault();

            var available_list = _context.TblCoupons.Where(x=> x.Points <= member.TotalPoints && x.Active == true)
                                 .Select(x =>
                                            new
                                            {
                                                x.CouponId,
                                                x.CouponName,
                                                x.Amount,
                                                x.Points,
                                                x.AvailableQty
                                            }
                                            ).ToList();
            _context.Dispose();
            return Ok(new
            {
                available = available_list,
                exchanged = exchanged_list
            });

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }


    [Authorize]
    [HttpPost("ExchangeCoupon")]
    public async Task<IActionResult> ExchangeCoupon(int couponId)
    {
        try
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int memberId = Convert.ToInt32(idStr);
            var coupon = _context.TblCoupons.Where(x => x.CouponId == couponId).FirstOrDefault();
            if (coupon != null)
            {
                var exchangeCoupon = new TblExchangeCoupon()
                {
                    MemberId = memberId,
                    CouponId = couponId,
                    IsUsed = false
                };
                await _context.TblExchangeCoupons.AddAsync(exchangeCoupon);
                coupon.AvailableQty -= 1;
                if (coupon.AvailableQty == 0)
                {
                    coupon.Active = false;
                }
                var member = _context.TblMembers.Where(x => x.MemberId == memberId).FirstOrDefault();
                member.TotalPoints -= coupon.Points;
                await _context.SaveChangesAsync();
                await _context.DisposeAsync();
                return Ok("Exchange Coupon Success");
            }

            else
            {
                return NotFound("Coupon Not Found!");
            }
            
            

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }


    #region Helpers
    private string GetRandomCode()
    {
        Random random = new Random();
        int randomNumber = random.Next(100000, 999999);
        return randomNumber.ToString();
    }

    private string GetMemberCode(string? lastCode)
    {
        int numericValue = 0;
        if (!string.IsNullOrEmpty(lastCode))
        {
            string numericPart = lastCode.Substring(1);
            numericValue = int.Parse(numericPart);

        }

        numericValue++;

        string newCode = "M" + numericValue.ToString("D4");

        return newCode;
    }
    #endregion

}

