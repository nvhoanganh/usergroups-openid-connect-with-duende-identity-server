
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace idsserver
{

    public class MySqlUserStore<TUser> : IUserLoginStore<TUser>,
          IUserClaimStore<TUser>,
          IUserRoleStore<TUser>,
          IUserPasswordStore<TUser>,
          IUserSecurityStampStore<TUser>,
          IQueryableUserStore<TUser>,
          IUserEmailStore<TUser>,
          IUserPhoneNumberStore<TUser>,
          IUserTwoFactorStore<TUser>,
          IUserLockoutStore<TUser>,
          IUserStore<TUser>,

        IUserAuthenticationTokenStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        IUserTwoFactorRecoveryCodeStore<TUser>
          where TUser : UserAuth
    {
        private readonly MysqlApplicationDbContext userDbContext;
        private const string InternalLoginProvider = "[AspNetUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        private const string RecoveryCodeTokenName = "RecoveryCodes";
        public MySqlUserStore(MysqlApplicationDbContext cache)
        {
            this.userDbContext = cache;
        }

        public IQueryable<TUser> Users
        {
            get
            {
                var db = this.userDbContext.UserAuth.Select(x => (TUser)x).AsQueryable();
                return db;
            }
        }

        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            // throw new System.NotImplementedException();
            return Task.CompletedTask;
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public virtual async Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken)
        {
            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
            if (mergedCodes.Length > 0)
            {
                return mergedCodes.Split(';').Length;
            }
            return 0;
        }

        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            var db = this.userDbContext.UserAuth;
            db.Add(user);
            this.userDbContext.SaveChanges();

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            var db = this.userDbContext.UserAuth;
            var _user = this.userDbContext.UserAuth.FirstOrDefault(x => x.UserAuthId == user.UserAuthId);
            if (_user != null)
            {
                this.userDbContext.Remove(_user);
                this.userDbContext.SaveChanges();
            }
            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {

            return Task.FromResult((TUser)this.userDbContext.UserAuth.FirstOrDefault(x => x.Email.ToUpper() == normalizedEmail));
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return Task.FromResult((TUser)this.userDbContext.UserAuth.FirstOrDefault(x => x.UserAuthId == userId));
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult((TUser)this.userDbContext.UserAuth.FirstOrDefault(x => x.UserName.ToUpper() == normalizedUserName));
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            // return Task.FromResult(user.AccessFailedCount);
            // todo: support
            return Task.FromResult(0);

        }

        public Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
            => GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);

        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            IList<Claim> result = new List<Claim>();
            return Task.FromResult(result);
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            // return Task.FromResult(user.EmailConfirmed);
            // todo: support
            return Task.FromResult(true);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            // return Task.FromResult(user.LockoutEnabled);
            // todo: support
            return Task.FromResult(false);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            // return Task.FromResult(user.LockoutEnd);
            // todo: support
            throw new System.NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email.ToUpper());
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName.ToUpper());
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.HashedPassword);
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult("");
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            // todo: support
            return Task.FromResult(true);
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            // throw new System.NotImplementedException();
            IList<string> result = new List<string>();
            return Task.FromResult(result);
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            // todo: support
            return Task.FromResult("");
        }

        private IdentityUserToken<string> FindToken(TUser user, string loginProvider, string name)
        {
            if (name == AuthenticatorKeyTokenName)
            {
                // this is stored in the same table
                var _user = this.userDbContext.UserAuth.FirstOrDefault(x => x.UserAuthId == user.UserAuthId);
                return new IdentityUserToken<string>
                {
                    UserId = user.UserAuthId,
                    LoginProvider = loginProvider,
                    Name = name,
                    Value = _user.TwoFaToken
                };
            }

            // for now we only support 2FA
            throw new NotSupportedException();
        }

        public Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            if (name == AuthenticatorKeyTokenName)
            {
                // this is stored in the userauth table
                var _user = this.userDbContext.UserAuth.FirstOrDefault(x => x.UserAuthId == user.UserAuthId);
                if (_user != null)
                    return Task.FromResult(_user.TwoFaToken);
            }
            return Task.FromResult("");
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Has2Fa);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserAuthId);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.HashedPassword != null);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            // user.AccessFailedCount++;
            // return Task.FromResult(user.AccessFailedCount);
            // todo: support
            return Task.FromResult(0);

        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
            var splitCodes = mergedCodes.Split(';');
            if (splitCodes.Contains(code))
            {
                var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
                await ReplaceCodesAsync(user, updatedCodes, cancellationToken);
                return true;
            }
            return false;
        }

        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            if (name == AuthenticatorKeyTokenName)
            {
                var _user = FindUser(user);
                if (_user != null)
                {
                    _user.TwoFaToken = null;
                    this.userDbContext.SaveChanges();
                }
            }
            return Task.CompletedTask;
        }


        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            var mergedCodes = string.Join(";", recoveryCodes);
            return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            // user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
            => SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            // user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            // user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            // user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.Email = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.UserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.HashedPassword = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            // user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            // user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            // user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            if (name == AuthenticatorKeyTokenName)
            {
                UserAuth _user = FindUser(user);
                if (_user != null)
                {
                    _user.TwoFaToken = value;
                    this.userDbContext.SaveChanges();
                }
            }

            // don't need to support recovery codes for now
            return Task.CompletedTask;
        }

        private UserAuth FindUser(TUser user)
        {
            return this.userDbContext.UserAuth.FirstOrDefault(x => x.UserAuthId == user.UserAuthId);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.Has2Fa = enabled;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            var _user = this.userDbContext.UserAuth.FirstOrDefault(x => x.UserAuthId == user.UserAuthId);
            if (_user == null)
            {
                // add new 
                user.UserAuthId = Guid.NewGuid().ToString();
                this.userDbContext.Add(user);
                this.userDbContext.SaveChanges();
            }
            else
            {
                // what fields can user change?
                _user.Has2Fa = user.Has2Fa;
                _user.HashedPassword = user.HashedPassword;
                this.userDbContext.SaveChanges();
            }
            return Task.FromResult(IdentityResult.Success);
        }

        private List<IdentityUserToken<string>> UserTokens()
        {
            return new List<IdentityUserToken<string>>();
        }
        private void SaveTokens(List<IdentityUserToken<string>> db)
        {
            return;
        }
    }
}