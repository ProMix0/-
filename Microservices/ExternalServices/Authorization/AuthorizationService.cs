using Microservices.Common.Exceptions;
using Microservices.ExternalServices.Authorization.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microservices.ExternalServices.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        public Task<AuthorizationResult> AuthorizeAsync(string sessionId, CancellationToken cancellationToken)
        {
            throw new ConnectionException();
        }
    }
}
