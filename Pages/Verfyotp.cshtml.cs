using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using WebApplication1.Models;
using WebApplication1.Service;

namespace WebApplication1.Pages;

    public class VerifyOtpModel(MongoDBservice dBservice) : PageModel
    {
 private readonly MongoDBservice _dbservice=dBservice;

        public string ErrorMessage { get; set; } = "";
        public string SucessMessage { get; set; } = "";
        public required Otpp Req { get; set; } // Ensure that this is properly initialized
        public required string UserId { get; set; } // Capture the ID from the URL

        // Constructor that injects dependencies


        public void OnGet(string id)
        {
            // Capture the ID from the URL
            UserId = id;

            // For debugging, log the ID
            Console.WriteLine($"User ID: {UserId}");

            if (string.IsNullOrEmpty(UserId))
            {
                ErrorMessage = "Invalid user ID.";
            }
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    ErrorMessage = "Invalid user ID.";
                    return Page();
                }

                // For debugging purposes
                Console.WriteLine($"Received ID in OnPost: {id}");

                // Ensure Req is not null before using it
                if (Req == null)
                {
                    ErrorMessage = "Request data is missing.";
                    return Page();
                }

                // Find the user by ID (no need to convert to ObjectId)
                var findUser = await _dbservice.Users.Find(u => u.Id.ToString() == id).FirstOrDefaultAsync();

                // Check if user is found
                if (findUser == null)
                {
                    ErrorMessage = "User not found.";
                    return Page();
                }

                // Ensure OTP is valid
                if (string.IsNullOrEmpty(Req.Otp))
                {
                    ErrorMessage = "OTP is missing.";
                    return Page();
                }

                // Validate OTP (ensure Req.Otp is not null)
                var isOtpValid = BCrypt.Net.BCrypt.Verify(Req.Otp.ToString(), findUser.Otp);
                if (!isOtpValid)
                {
                    ErrorMessage = "Invalid OTP.";
                    return Page();
                }

                // Ensure passwords match
                if (Req.Pass != Req.Confpass)
                {
                    ErrorMessage = "Password and confirm password don't match.";
                    return Page();
                }

                // Update password and clear OTP
                findUser.Password = BCrypt.Net.BCrypt.HashPassword(Req.Pass);
                findUser.Otp = null;
                findUser.OtpExpiry = null;

                // Update the user document in MongoDB
                await _dbservice.Users.ReplaceOneAsync(u => u.Id == findUser.Id, findUser);

                SucessMessage = "Password reset successfully!";
                return RedirectToPage("/Verifyotp", new { id = id });
            }
            catch (Exception ex)
            {
                // Catch the exception and display the error message
                ErrorMessage = $"An error occurred: {ex.Message}";
                return Page();
            }
        }
    }

