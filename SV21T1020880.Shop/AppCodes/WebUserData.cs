using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SV21T1020880.Shop
{
    public class WebUserData
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";

        /// <summary>
        /// Danh sách các Claim
        /// </summary>
        public List<Claim> Claims
        {
            get
            {
                return new List<Claim>
                {
                    new Claim(nameof(UserId), UserId),
                    new Claim(nameof(UserName), UserName),
                    new Claim(nameof(DisplayName), DisplayName)
                };
            }
        }

        /// <summary>
        /// Tạo ClaimsPrincipal dựa trên thông tin của người dùng
        /// </summary>
        public ClaimsPrincipal CreatePrincipal()
        {
            var identity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        /// <summary>
        /// Tạo đối tượng WebUserData từ ClaimsPrincipal
        /// </summary>
        public static WebUserData FromClaimsPrincipal(ClaimsPrincipal principal)
        {
            if (principal?.Identity == null || !principal.Identity.IsAuthenticated)
                return null;

            var claims = principal.Claims;
            return new WebUserData
            {
                UserId = claims.FirstOrDefault(c => c.Type == nameof(UserId))?.Value ?? "",
                UserName = claims.FirstOrDefault(c => c.Type == nameof(UserName))?.Value ?? "",
                DisplayName = claims.FirstOrDefault(c => c.Type == nameof(DisplayName))?.Value ?? ""
            };
        }
    }
}
