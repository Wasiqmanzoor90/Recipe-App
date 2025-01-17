using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using WebApplication1.Interface;
using WebApplication1.Models;
using WebApplication1.Service;

namespace WebApplication1.Pages;

public class LoginModel(MongoDBservice dBservice, ITokenService tokenService) : PageModel
{

    private readonly MongoDBservice dBservice = dBservice;
    private readonly ITokenService tokenService = tokenService;

    [BindProperty]
    public required Login Logininput { set; get; }
    public required string ErrorMessage { get; set; } = "";
    public required string SucessMessage { get; set; } = "";


    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {


            var finduser = await dBservice.Users.Find(u => u.Email == Logininput.Email).FirstOrDefaultAsync();
            if (finduser == null)
            {
                ErrorMessage = "User Not found";
                return Page();
            }
            if (string.IsNullOrEmpty(Logininput.Email) || string.IsNullOrEmpty(Logininput.Password))
            {
                ErrorMessage = "Email and Password are required";
                return Page();
            }
            var isvalid = BCrypt.Net.BCrypt.Verify(Logininput.Password, finduser.Password);
            if (!isvalid)
            {
                ErrorMessage = "Invalid Password";
            }

            var token = tokenService.CreateToken(finduser.Id, finduser.Email);
            HttpContext.Response.Cookies.Append(
                "TestToken",
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(10)
                }

            );
            SucessMessage = "Login Successfull";
            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {

               throw new InvalidOperationException("Something went wrong", ex);
        }
    }
}

