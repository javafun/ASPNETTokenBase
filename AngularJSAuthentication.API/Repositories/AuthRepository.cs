using AngularJSAuthentication.API.Entities;
using AngularJSAuthentication.API.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularJSAuthentication.API.Repositories
{
    public class AuthRepository : IDisposable
    {
        private AuthContext _ctx;
        private UserManager<IdentityUser> _userManager;

        public AuthRepository()
        {
            _ctx = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        }

        public async Task<IdentityResult> RegisterUserAsync(UserModel userModel)
        {
            IdentityUser user = new IdentityUser(userModel.UserName);

            var result = await _userManager.CreateAsync(user, userModel.Password);

            return result;
        }

        public async Task<IdentityUser> FindUser(string username, string password)
        {
            IdentityUser user = await _userManager.FindAsync(username, password);

            return user;
        }

        public Client FindClient(string clientId)
        {
            return _ctx.Clients.Find(clientId);
        }

        public async Task<bool> AddRefreshToken(RefreshToken refreshToken)
        {
            var existingToken = _ctx.RefreshTokens.SingleOrDefault(x =>
                    x.Subject == refreshToken.Subject
                    && x.ClientId == refreshToken.ClientId);

            if (existingToken != null)
            {
                await RemoveRefreshToken(existingToken);
            }

            _ctx.RefreshTokens.Add(refreshToken);

            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            if(refreshToken!= null)
            {
                _ctx.RefreshTokens.Remove(refreshToken);

                return await _ctx.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken existingToken)
        {
            _ctx.RefreshTokens.Remove(existingToken);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            return await _ctx.RefreshTokens.FindAsync(refreshTokenId);
        }

        public IEnumerable<RefreshToken> GetAllRefershTokens()
        {
            return _ctx.RefreshTokens;
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }
    }
}
