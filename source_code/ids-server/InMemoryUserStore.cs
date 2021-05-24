
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace idsserver
{

    public class InMemoryUserStore<TUser> : IUserLoginStore<TUser>,
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
          where TUser : IdentityUser
    {
        private readonly IMemoryCache cache;
        private const string InternalLoginProvider = "[AspNetUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        private const string RecoveryCodeTokenName = "RecoveryCodes";
        public InMemoryUserStore(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public IQueryable<TUser> Users
        {
            get
            {
                var db = this.UsersDb();
                return db.Values.AsQueryable();
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
            var db = this.UsersDb();
            db.Add(user.Id, user);
            Save(db);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            var db = this.UsersDb();
            db.Remove(user.Id);
            Save(db);
            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {

            return Task.FromResult(this.UsersDb().Values.FirstOrDefault(x => x.NormalizedEmail == normalizedEmail));
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.UsersDb().Values.FirstOrDefault(x => x.Id == userId));
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.UsersDb().Values.FirstOrDefault(x => x.NormalizedUserName == normalizedUserName));
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AccessFailedCount);
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
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            // throw new System.NotImplementedException();
            IList<string> result = new List<string>();
            return Task.FromResult(result);
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        private IdentityUserToken<string> FindToken(TUser user, string loginProvider, string name)
        {
            var db = this.UserTokens();
            var token = db.FirstOrDefault(x =>
                x.UserId == user.Id && x.Name == name && x.LoginProvider == loginProvider);
            return token;
        }

        public Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            var db = this.UserTokens();
            var token = FindToken(user, loginProvider, name);
            return Task.FromResult(token?.Value);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
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
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
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
            var db = this.UserTokens();
            var token = FindToken(user, loginProvider, name);
            if (token != null)
            {
                db = db.Where(x => x.UserId != user.Id && x.LoginProvider != loginProvider && x.Name != name).ToList();
            }
            this.SaveTokens(db);
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
            user.AccessFailedCount = 0;
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
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            var db = UserTokens();
            var token = FindToken(user, loginProvider, name);
            if (token == null)
            {
                db.Add(new IdentityUserToken<string>
                {
                    UserId = user.Id,
                    LoginProvider = loginProvider,
                    Name = name,
                    Value = value

                });
            }
            else
            {
                token.Value = value;
            }

            SaveTokens(db);
            return Task.CompletedTask;
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            var db = UsersDb();
            var _user = db.Values.ToList().Find(x => x.NormalizedUserName == user.NormalizedUserName);
            if (_user == null)
            {
                // add new 
                user.Id = Guid.NewGuid().ToString();
                db[user.Id] = user;
                Save(db);
            }
            else
            {
                // update
                db[_user.Id] = user;
                Save(db);
            }
            return Task.FromResult(IdentityResult.Success);
        }

        private Dictionary<string, TUser> UsersDb()
        {
            var db = this.cache.Get<Dictionary<string, TUser>>("users");
            if (db == null)
            {
                this.cache.Set("users", new Dictionary<string, TUser>());
                return new Dictionary<string, TUser>();
            }
            return db;
        }
        private void Save(Dictionary<string, TUser> db)
        {
            this.cache.Set("users", db);
        }
        private List<IdentityUserToken<string>> UserTokens()
        {
            var db = this.cache.Get<List<IdentityUserToken<string>>>("tokens");
            if (db == null)
            {
                this.cache.Set("tokens", new List<IdentityUserToken<string>>());
                return new List<IdentityUserToken<string>>();
            }
            return db;
        }
        private void SaveTokens(List<IdentityUserToken<string>> db)
        {
            this.cache.Set("tokens", db);
        }
    }
}