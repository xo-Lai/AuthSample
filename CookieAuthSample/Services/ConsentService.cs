using CookieAuthSample.ViewModels;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookieAuthSample.Services
{
    public class ConsentService
    {

        private readonly IClientStore _clientStore;

        private readonly IResourceStore _resourceStore;

        private readonly IIdentityServerInteractionService _identityServerInteractionService;

        public ConsentService(IClientStore clientStore, IResourceStore resourceStore, IIdentityServerInteractionService identityServerInteractionService)
        {
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _identityServerInteractionService = identityServerInteractionService;
        }


        #region Private

        private ConsentViewModel CreateConsentViewModel(AuthorizationRequest request, Client client, Resources resources,InputConsentViewModel model)
        {
            var rememberConsent = model?.RememberConsent??true;
            var selectScopes = model?.ScopeConsented ?? Enumerable.Empty<string>();
            var vm = new ConsentViewModel();
            vm.ClientName = client.ClientName;
            vm.ClientLogoUrl = client.LogoUri;
            vm.ClientUrl = client.ClientUri;
            vm.RememberConsent = rememberConsent;

            vm.IdentityScopes = resources.IdentityResources.Select(i => CreateScopeViewModel(i,selectScopes.Contains(i.Name)||model==null));

            vm.ResourseScopes = resources.ApiResources.SelectMany(i => i.Scopes).Select(x => CreateScopeViewModel(x, selectScopes.Contains(x.Name) || model == null));
            return vm;
        }

        private ScopeViewModel CreateScopeViewModel(IdentityResource identityResource,bool check)
        {
            return new ScopeViewModel
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Description = identityResource.Description,
                Checked = check||identityResource.Required,
                Required = identityResource.Required,
                Emphasize = identityResource.Emphasize
            };
        }


        private ScopeViewModel CreateScopeViewModel(Scope scope,bool check)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Checked = check|| scope.Required,
                Required = scope.Required,
                Emphasize = scope.Emphasize
            };
        }

        #endregion

        public async Task<ConsentViewModel> BuildConsentViewModel(string returnUrl,InputConsentViewModel model=null)
        {

            var request = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
            if (request == null) return null;
            var client = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);
            var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);

            var vm = CreateConsentViewModel(request, client, resources,model);
            vm.ReturnUrl = returnUrl;
            return vm;
        }


        public async Task<ProcessConsentResult> ProcessConsent(InputConsentViewModel model)
        {
            ConsentResponse consentResponse = null;
            ProcessConsentResult result = new ProcessConsentResult();
            if (model.Button == "no")
            {
                consentResponse = ConsentResponse.Denied;
            }
            else if (model.Button == "yes")
            {
                if (model.ScopeConsented != null && model.ScopeConsented.Any())
                {
                    consentResponse = new ConsentResponse
                    {
                        ScopesConsented = model.ScopeConsented,
                        RememberConsent = model.RememberConsent,
                    };
                }
                result.ValidationError = "请至少一个权限";
            }
            if (consentResponse != null)
            {
                var request = await _identityServerInteractionService.GetAuthorizationContextAsync(model.ReturnUrl);
                await _identityServerInteractionService.GrantConsentAsync(request, consentResponse);
                result.RedirectUrl = model.ReturnUrl;

            }
   
            {
                var consentViewModel = await BuildConsentViewModel(model.ReturnUrl,model);
                result.ViewModel = consentViewModel;
            }
            return result;
        }
    }
}
