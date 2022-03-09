//using Microsoft.AspNetCore.Authentication;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Threading.Tasks;

//namespace App.Models
//{
//    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
//    {       
//        public string Protect(AuthenticationTicket data)
//        {
//            if (data == null) throw new ArgumentNullException("data");

//            var token = new JwtSecurityToken(
//                "AsbtAuthenticationOptions.ISSUER",
//                "AsbtAuthenticationOptions.AUDIENCE",
//                data.Principal.Claims,
//                DateTime.Now,
//                DateTime.Now.Add(TimeSpan.FromDays(5)),                
//                new SigningCredentials(AsbtAuthenticationOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

//            var handler = new JwtSecurityTokenHandler();
//            var jwt = handler.WriteToken(token);
//            return jwt;
//        }

//        public AuthenticationTicket Unprotect(string protectedText)
//        {
//            throw new NotImplementedException();
//        }

//        public string Protect(AuthenticationTicket data, string purpose)
//        {
//            throw new NotImplementedException();
//        }

//        [return: MaybeNull]
//        public AuthenticationTicket Unprotect(string protectedText, string purpose)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
