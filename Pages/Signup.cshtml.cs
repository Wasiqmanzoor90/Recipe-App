using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using WebApplication1.Models;
using WebApplication1.Service;

namespace WebApplication1.Pages
{
    public class SignupModel(MongoDBservice dBservice) : PageModel
    {
        private readonly MongoDBservice _dbservice = dBservice;
        [BindProperty]
        public required User Newuser { set; get; }

        public required string ErrorMessage{set; get;}= "";
        public required string SucesssMessage { set; get; }="";
        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            try
            {


                var finduser = await _dbservice.Users.Find(u => u.Email == Newuser.Email).FirstOrDefaultAsync();
                if (finduser != null)
                {
                    ErrorMessage = "User already exist";
                    return Page();
                }
                if (string.IsNullOrEmpty(Newuser.Username) || string.IsNullOrEmpty(Newuser.Email) || string.IsNullOrEmpty(Newuser.Password))
                {
                    ErrorMessage = "Please fill all fields";
                    return Page();
                }
                Newuser.Password = BCrypt.Net.BCrypt.HashPassword(Newuser.Password);
                await _dbservice.Users.InsertOneAsync(Newuser);
                SucesssMessage = "User created successfully";
                return RedirectToPage("/Signup");

            }
            catch (Exception ex)
            {

                throw new InvalidOperationException("Server error", ex);
            }

        }
    }
}
