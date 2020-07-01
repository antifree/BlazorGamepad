using System;
using System.Collections.Generic;
using System.Text;

using Antifree.Blazor.GamepadAPI;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BlazorGamepadAPIServiseExtensions
    {
        public static IServiceCollection AddBlazorGamepadAPI(this IServiceCollection services) 
        { 
            return services.AddScoped<GamepadManagerResolver>();
        }
    }
}
