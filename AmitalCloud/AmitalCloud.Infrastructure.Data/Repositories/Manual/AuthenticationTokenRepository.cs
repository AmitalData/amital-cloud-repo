using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class AuthenticationTokenRepository : Repository<AuthenticationToken>
    {
        IGlobalContext currentContext;
        public AuthenticationTokenRepository(IGlobalContext context) : base(context)
        {
            currentContext = context;
        }
        public AuthenticationTokenRepository() : this(GlobalContext.GetContext())
        {
        }
        public AuthenticationTokenRepository(int tenant) : this(GlobalContext.GetContext())
        {
        }
        public IGlobalContext context
        {
            get { return currentContext; }
        }
        public AuthenticationToken GetSingleToken(string token)
        {
            if (!string.IsNullOrEmpty(token) && !token.Contains("+"))
            {
                token = token.Replace(" ", "+");
            }
            return GetSingle(a => a.Token == token);
        }
        public static AuthenticationToken GetSingleTokenFromCache(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;
            if (!string.IsNullOrEmpty(token) && !token.Contains("+"))
            {
                token = token.Replace(" ", "+");
            }
            string cacheKey = $"Token_({token})";
            AuthenticationToken authenticationToken = (AuthenticationToken)CacheManager.CacheWrapper.Get(cacheKey);
            if (authenticationToken != null)
            {
                if (authenticationToken.ExpirationDate != null && authenticationToken.ExpirationDate < DateTime.Now)
                {
                    throw new AutenticationException("Session expired. Please log in again");
                }
                return authenticationToken;
            }
            else
            {
                IGlobalContext context = GlobalContext.GetContext();
                authenticationToken = (from a in context.AuthenticationTokens
                                       where a.Token == token
                                       select a).FirstOrDefault();
                if (authenticationToken != null)
                {
                    if (authenticationToken.ExpirationDate != null && authenticationToken.ExpirationDate < DateTime.Now)
                    {
                        throw new AutenticationException("Session expired. Please log in again");
                    }
                    CacheManager.CacheWrapper.Insert(cacheKey, authenticationToken, null, DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                }
                return authenticationToken;
            }
        }
        public class AutenticationException : Exception
        {
            public AutenticationException(string message) : base(message) { }
        }
    }
}
