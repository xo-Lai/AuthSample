using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CookieAuthSample
{
    public class Config
    {

        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId="api",
                    ClientName="Mvc Client",
                    ClientUri="http://localhost:5001",
                    LogoUri="https://chocolatey.org/content/packageimages/aspnetcore-runtimepackagestore.2.0.0.png",
                    AllowRememberConsent=true,


                    AllowedGrantTypes=GrantTypes.Implicit,
                    ClientSecrets={
                        new Secret("secret".Sha256())
                    },
                    RequireConsent=true,
                    RedirectUris={"http://localhost:5001/signin-oidc"},
                    PostLogoutRedirectUris={"http://localhost:5001/signout-callback-oidc"},
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AllowedScopes={
                       IdentityServerConstants.StandardScopes.Profile,
                       IdentityServerConstants.StandardScopes.OpenId,
                       IdentityServerConstants.StandardScopes.Email
                    }
                },
                 new Client
                {
                    ClientId="mvc",
                    ClientName="Mvc Client",
                    ClientUri="http://localhost:5001",
                    LogoUri="https://chocolatey.org/content/packageimages/aspnetcore-runtimepackagestore.2.0.0.png",
                    AllowRememberConsent=true,


                    AllowedGrantTypes=GrantTypes.HybridAndClientCredentials,
                    ClientSecrets=new List<Secret>{
                        new Secret("secret".Sha256())
                    },
                    AllowOfflineAccess=true,
                    AllowAccessTokensViaBrowser=true,
                    RefreshTokenExpiration=TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime=60*60,
                    RedirectUris={"http://localhost:5001/signin-oidc"},
                    PostLogoutRedirectUris={"http://localhost:5001/signout-callback-oidc"},
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AllowedScopes={
                       IdentityServerConstants.StandardScopes.Profile,
                       IdentityServerConstants.StandardScopes.OpenId,
                       IdentityServerConstants.StandardScopes.Email,
                       IdentityServerConstants.StandardScopes.OfflineAccess,
                       "api1"
                    }
                },
            };
        }


        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile()
            };
        }
        /// <summary>
        /// api资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1","API  Applicaton")
            };
        }


        /// <summary>
        /// 测试账号
        /// </summary>
        /// <returns></returns>
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId="10000",
                    Username="jeff",
                    Password="123456"
                }
            };
        }
    }
}
