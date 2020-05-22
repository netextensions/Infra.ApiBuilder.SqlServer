using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetExtensions.Infra.SqlServer;
using Newtonsoft.Json.Converters;

namespace NetExtensions.Infra.ApiBuilder.SqlServer
{
    public static class ApiBuilderExtension
    {
        public static IServiceCollection AddApiBuilder<TContext, TAutoMapper, TMediatR>(this IServiceCollection services, string connectionSetting,
             string swaggerTitle = null,
            string swaggerDescription = null,
            string swaggerVersion = null)
            where TContext : DbContext
        {
            services.AddSwashbuckle(swaggerTitle, swaggerDescription, swaggerVersion);
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()));
            services.AddSqlServerDb<TContext>(connectionSetting);
            services.AddAutoMapper(typeof(TAutoMapper).Assembly);
            services.AddMediatR(typeof(TMediatR).Assembly);
            return services;
        }
        public static IServiceCollection AddApiBuilder<TContext, TAutoMapperMediatR>(this IServiceCollection services, string connectionSetting,
            string swaggerTitle = null,
            string swaggerDescription = null,
            string swaggerVersion = null)
            where TContext : DbContext
        {
            return services.AddApiBuilder<TContext, TAutoMapperMediatR, TAutoMapperMediatR>(connectionSetting, swaggerTitle, swaggerDescription, swaggerVersion);
        }

        public static IApplicationBuilder UseApiBuilder(this IApplicationBuilder app, IWebHostEnvironment env, string swaggerName = null,
            string swaggerEndpoint = null, bool useSerilogMiddleware = true)
        {
            app.AddSwashbuckle(swaggerName, swaggerEndpoint);
            app.AddSerilogRequestLogging(useSerilogMiddleware);
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            return app;
        }
    }
}
