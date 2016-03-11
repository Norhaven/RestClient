using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestClient
{
    public interface IRestClient<TInterface> where TInterface:class
    {
        Task<TResult> CallAsync<TResult>(Expression<Func<TInterface, TResult>> invokeRestMethod, CancellationToken cancellationToken);
    }
}
