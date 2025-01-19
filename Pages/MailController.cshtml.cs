using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using WebApplication1.Interface;
using WebApplication1.Models;
using WebApplication1.Service;

namespace WebApplication1.Pages
{
    public class MailControllerModel(MongoDBservice dBservice, IEmail email) : PageModel
    {

        private readonly MongoDBservice _dbservice = dBservice;
        private readonly IEmail _email = email;
        [BindProperty]
        public required Email Req {get;set;}

        public string ErrorMessage {get; set;}="";
           public string SucessMessage {get; set;}="";

        public void OnGet()
        {
        }

public async Task<IActionResult> OnPostAsync()
{
    try
    {
        

     var finduser = await _dbservice.Users.Find(u => u.Email.ToString() == Req.UserEmail).FirstOrDefaultAsync();

            if (finduser == null)
            {
                return BadRequest(new { message = "User not found" });

            }

            var random = new Random();
            var plainOtp = random.Next(1000, 9999).ToString();

            // Hash the OTP using BCrypt
            var hashedOtp = BCrypt.Net.BCrypt.HashPassword(plainOtp);

            // Save hashed OTP and expiry to the database
            finduser.Otp = hashedOtp;
            finduser.OtpExpiry = DateTime.UtcNow.AddMinutes(10); // OTP valid for 3 minutes
            await _dbservice.Users.ReplaceOneAsync(u => u.Id == finduser.Id, finduser);
             await _email.SendEmailAsync(Req.UserEmail, "OTP for Password Reset", $"Your OTP is {plainOtp}", false);
             SucessMessage ="Sucess";
            return RedirectToPage("/Verifyotp", new { id = finduser.Id.ToString() });

    }
    catch (Exception ex)
    {
        
        throw new InvalidOperationException("faild to send", ex);
    }
}

    }
}
