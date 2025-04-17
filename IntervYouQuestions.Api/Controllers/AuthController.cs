using IntervYouQuestions.Api.Authentication;
using IntervYouQuestions.Api.Authentication.Dto;
using IntervYouQuestions.Api.Authentication.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web;

namespace IntervYouQuestions.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
         UserManager<AppUser> userManager,
         SignInManager<AppUser> signInManager,
         IUserService userService,
         IEmailService emailService,
         ILogger<AuthController> logger,
         InterviewModuleContext context,
         IOptions<JwtSettings> jwtSettings,
         IConfiguration configuration

        ) : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly IUserService _userService = userService;
        private readonly IEmailService _emailService = emailService;
        private readonly ILogger<AuthController> _logger = logger;
        private readonly InterviewModuleContext _context = context;
        private readonly IConfiguration _configuration = configuration;
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        [HttpPost("register/mobile")]
        public async Task<IActionResult> RegisterMobile(RegisterMobile model)
        {
            var user = new AppUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email, // Important
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = HttpUtility.UrlEncode(token);

                // Construct deep link using configured mobile app scheme
                // var confirmationLink = $"{MobileAppScheme}://email-verification?userId={user.Id}&token={encodedToken}";

                var subject = "Confirm Your Email";
                //var message = $"<h1>Welcome!</h1><p>Please confirm your email address using this link in your mobile app: <a href='{confirmationLink}'>Confirm Email</a></p><p>Or copy/paste: {confirmationLink}</p>";

                //await _emailService.SendEmailAsync(user.Email, subject, message);

                return Ok("Registration successful. Please check your email to confirm your account.");
            }
            return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
        }

        [HttpPost("register/web")]
        public async Task<IActionResult> RegisterWeb(RegisterWeb model)
        {
            var user = new AppUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = HttpUtility.UrlEncode(token); // Encode for URL query string

                var frontendBaseUrl = _configuration["FrontendUrls:FrontendBaseUrl"];
                if (string.IsNullOrEmpty(frontendBaseUrl))
                {
                    _logger.LogError("FrontendBaseUrl is not configured. Cannot generate confirmation link.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Registration succeeded but could not generate confirmation link due to server configuration.");
                }

                var confirmationLink = $"{frontendBaseUrl}/auth/verify-email?userId={user.Id}&token={encodedToken}";
                //                                         ^^^^^^ frontend route path

                _logger.LogInformation("Generated confirmation link for web registration: {Link}", confirmationLink); // Good for debugging

                var subject = "Confirm Your Email";
                var message = $"<h1>Welcome!</h1><p>Please confirm your email address by clicking this link: <a href='{confirmationLink}'>Confirm Email</a></p>";

                try
                {
                    await _emailService.SendEmailAsync(user.Email, subject, message);
                    return Ok("Registration successful. Please check your email to confirm your account.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
                    return StatusCode(StatusCodes.Status207MultiStatus, "Registration successful, but confirmation email could not be sent. Please try requesting password reset if needed, or contact support.");
                }
            }
            return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
        }


        //[HttpGet("confirm-email")]
        //public async Task<IActionResult> ConfirmEmail(string userId, string token)
        //{
        //    if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
        //    {
        //        return BadRequest("User ID and Token are required.");
        //    }

        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return BadRequest("Invalid confirmation attempt.");
        //    }

        //    var decodedToken = HttpUtility.UrlDecode(token);

        //    var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        //    if (result.Succeeded)
        //    {
        //        return Ok("Email confirmed successfully!");
        //    }

        //    _logger.LogError("Email confirmation failed for user {UserId}. Errors: {Errors}", userId, string.Join(", ", result.Errors.Select(e => e.Description)));
        //    return BadRequest("Email confirmation failed. The link may be invalid or expired.");
        //}

        // DTO for email confirmation request from frontend
        public record VerifyEmailRequestDto(string UserId, string Token);

        [HttpPost("verify-email-token")] // <<< NEW Endpoint
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyEmailToken([FromBody] VerifyEmailRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Token))
            {
                return BadRequest("User ID and Token are required.");
            }

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("VerifyEmailToken attempt for non-existent UserId: {UserId}", request.UserId);
                return NotFound("Verification failed. User not found or invalid token.");
            }

            string decodedToken;
            try
            {
                decodedToken = HttpUtility.UrlDecode(request.Token);
                if (string.IsNullOrWhiteSpace(decodedToken))
                {
                    throw new ArgumentException("Decoded token is empty or whitespace.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decode verification token for UserId: {UserId}", request.UserId);
                return BadRequest("Verification failed. Invalid token format provided.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                _logger.LogInformation("Email confirmed via VerifyEmailToken for UserId: {UserId}", request.UserId);
                return Ok("Email confirmed successfully!"); 
            }
            else
            {
                _logger.LogError("VerifyEmailToken failed for UserId {UserId}. Errors: {Errors}", request.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest("Email confirmation failed. The link may be invalid or expired.");
            }
        }


        [HttpPost("submit-password-reset")] 
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitPasswordReset([FromBody] ResetPasswordDto model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("SubmitPasswordReset attempt for non-existent email: {Email}", model.Email);
                return BadRequest("Password reset failed. Please ensure the email is correct or try the 'forgot password' process again.");
            }

            string decodedToken;
            try
            {
                decodedToken = HttpUtility.UrlDecode(model.Token);
                if (string.IsNullOrWhiteSpace(decodedToken))
                {
                    throw new ArgumentException("Decoded token is empty or whitespace.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decode password reset token for email: {Email}", model.Email);
                return BadRequest("Password reset failed. Invalid token format provided.");
            }
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation("Password reset successful via SubmitPasswordReset for user {Email}", model.Email);
                return Ok("Password has been reset successfully."); // Simple success response
            }

            _logger.LogWarning("SubmitPasswordReset failed for user {Email}. Errors: {Errors}", model.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
            return BadRequest("Password reset failed. The token may be invalid/expired or the password does not meet requirements.");
        }

        [HttpPut("update-preferences")]
        [Authorize]
        public async Task<IActionResult> UpdatePreferences(UpdatePreferences dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound("User not found");

            user.PreferredRole = dto.PreferredRole;
            user.ExperienceLevel = dto.ExperienceLevel;
            user.DailyStudyHours = dto.DailyStudyHours;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Preferences updated successfully");
        }
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            if (!user.EmailConfirmed)
            {
                return Unauthorized("Email not confirmed. Please check your inbox.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var jwtResult = _userService.GenerateJwtToken(user, roles);
                var refreshToken = new RefreshToken
                {
                    Token = AuthHelpers.GenerateRefreshToken(),
                    JwtId = jwtResult.Jti,
                    UserId = user.Id,
                    ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenValidityInDays),
                };

                await _context.RefreshTokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();

                return Ok(new AuthResponse
                {
                    AccessToken = jwtResult.Token,
                    RefreshToken = refreshToken.Token,
                    ExpiresAt = jwtResult.ExpiresAt
                });

            }
            if (result.IsLockedOut)
            {
                return Unauthorized("Account locked due to too many failed login attempts.");
            }
            return Unauthorized("Invalid credentials.");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(TokenRefreshRequest request)
        {
            var principal = _userService.GetPrincipalFromExpiredToken(request.AccessToken);
            var jti = principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

            if (storedToken == null ||
                storedToken.IsUsed ||
                storedToken.IsRevoked ||
                storedToken.ExpiryDate < DateTime.UtcNow ||
                storedToken.JwtId != jti)
            {
                return BadRequest("Invalid refresh token");
            }

            storedToken.IsUsed = true;
            _context.RefreshTokens.Update(storedToken);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            var roles = await _userManager.GetRolesAsync(user);
            var jwtResult = _userService.GenerateJwtToken(user, roles);

            var newRefreshToken = new RefreshToken
            {
                Token = AuthHelpers.GenerateRefreshToken(),
                JwtId = jwtResult.Jti,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenValidityInDays)
            };

            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                AccessToken = jwtResult.Token,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = jwtResult.ExpiresAt
            });
        }

        // Inside AuthController

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                _logger.LogWarning("Password reset request for non-existent or unconfirmed email: {Email}", model.Email);
                // Still return Ok to prevent email enumeration attacks
                return Ok("If an account exists for this email and is confirmed, a password reset link has been sent.");
            }

            var rawToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(rawToken); // Encode for URL query string

            // --- Get Frontend Base URL from Config ---
            var frontendBaseUrl = _configuration["FrontendUrls:FrontendBaseUrl"]; 
            if (string.IsNullOrEmpty(frontendBaseUrl))
            {
                _logger.LogError("FrontendBaseUrl is not configured. Cannot generate password reset link.");
                // Still return Ok to user, but log the server-side issue
                return Ok("If an account exists for this email and is confirmed, a password reset link has been sent. (Internal link generation issue occurred)");
            }

            // --- Construct link pointing to FRONTEND ---
            var encodedEmail = HttpUtility.UrlEncode(user.Email); // Also encode email just in case
            var link = $"{frontendBaseUrl}/auth/reset-password?email={encodedEmail}&token={encodedToken}";
            //                             ^^^^^^^^^^^^^^^^^^^^^^ CHOOSE your frontend route path

            _logger.LogInformation("Generated password reset link: {Link}", link);

            var subject = "Reset Your Password";
            var message = $"<h1>Password Reset</h1><p>Please reset your password by clicking this link: <a href='{link}'>Reset Password</a></p><p>This will take you to the application to set a new password.</p>";

            try
            {
                await _emailService.SendEmailAsync(user.Email, subject, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
                // Don't expose error details, but log them
            }

            return Ok("If an account exists for this email and is confirmed, a password reset link has been sent.");
        }


        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                _logger.LogWarning("ResetPassword attempt for non-existent email: {Email}", model.Email);
                return BadRequest("Password reset failed. Please try the 'forgot password' process again.");
            }

            // *** ADD THIS DECODING STEP ***
            string decodedToken;
            try
            {
                // Decode the token received from the client (Swagger/frontend)
                decodedToken = HttpUtility.UrlDecode(model.Token);
                if (string.IsNullOrWhiteSpace(decodedToken))
                {
                    _logger.LogWarning("ResetPassword attempt with empty or whitespace token after decoding for email: {Email}", model.Email);
                    return BadRequest("Password reset failed. Invalid token format provided.");
                }
            }
            catch (Exception ex) // Catch potential decoding errors
            {
                _logger.LogError(ex, "Failed to decode password reset token for email: {Email}", model.Email);
                return BadRequest("Password reset failed. Invalid token format provided.");
            }
            // *** END DECODING STEP ***


            // *** Use the DECODED token for validation ***
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation("Password reset successful for user {Email}", model.Email);
                // Optionally update security stamp or sign out other sessions here
                // await _userManager.UpdateSecurityStampAsync(user);
                return Ok("Password has been reset successfully.");
            }

            // Log errors from Identity
            _logger.LogWarning("Password reset failed for user {Email}. Errors: {Errors}", model.Email, string.Join(", ", result.Errors.Select(e => e.Description)));

            // Provide generic error to user
            return BadRequest("Password reset failed. The link may be invalid or expired.");
        }

        //[HttpGet("handle-reset-link")] // Route for the link generated by Url.Action
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public IActionResult HandlePasswordResetLink([FromQuery] string email, [FromQuery] string token)
        //{
        //    // Basic validation
        //    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        //    {
        //        _logger.LogWarning("HandlePasswordResetLink called with missing email or token.");
        //        // Consider returning a more user-friendly HTML error page if possible
        //        return BadRequest("Invalid password reset link parameters.");
        //    }

        //    _logger.LogInformation("Password reset link clicked for email {Email}", email);

           
        //    return Ok($"Password reset link processed. Please return to the application and use the provided form to enter your new password, using the email '{email}' and the token from the link you just clicked.");

            
        //}

    }


}

