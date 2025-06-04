﻿using Application.Services.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class Extension
{
    public static void AddServices(this IServiceCollection services)
    {     
        services.AddHostedService<SFTPConfiguration>();
    }
}