using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestClient
{
    /// <summary>
    /// Represents a client for a specific REST service (as described by <typeparamref name="TInterface"/>).
    /// </summary>
    /// <typeparam name="TInterface">The contract representing possible REST operations against an endpoint.</typeparam>
    public interface IRestClient<TInterface> where TInterface:class
    {
        /// <summary>
        /// Executes a call to the REST endpoint invoked on <typeparamref name="TInterface"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result expected from this operation.</typeparam>
        /// <param name="invokeRestMethod">An expression invoking a method on the <typeparamref name="TInterface"/> interface.</param>
        /// <returns>An instance of <typeparamref name="TResult"/> that may be awaited."</returns>
        Task<TResult> CallAsync<TResult>(Expression<Func<TInterface, TResult>> invokeRestMethod);

        /// <summary>
        /// Executes a call to the REST endpoint invoked on <typeparamref name="TInterface"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result expected from this operation.</typeparam>
        /// <param name="invokeRestMethod">An expression invoking a method on the <typeparamref name="TInterface"/> interface.</param>
        /// <param name="cancellationToken">A cancellation token for this operation.</param>
        /// <returns>An instance of <typeparamref name="TResult"/> that may be awaited."</returns>
        Task<TResult> CallAsync<TResult>(Expression<Func<TInterface, TResult>> invokeRestMethod, CancellationToken cancellationToken);
    }
}
