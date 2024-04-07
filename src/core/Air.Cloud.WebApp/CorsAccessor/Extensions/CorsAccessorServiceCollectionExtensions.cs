// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Air.Cloud.Core.App;
using Air.Cloud.Core.Extensions;
using Air.Cloud.WebApp.CorsAccessor.Options;

using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Air.Cloud.WebApp.CorsAccessor.Extensions;

/// <summary>
/// 跨域访问服务拓展类
/// </summary>
[IgnoreScanning]
public static class CorsAccessorServiceCollectionExtensions
{
    /// <summary>
    /// 默认跨域导出响应头 Key
    /// </summary>
    /// <remarks>解决 ajax，XMLHttpRequest，axios 不能获取请求头问题</remarks>
    private static readonly string[] _defaultExposedHeaders = new[]
    {
        "access-token",
        "x-access-token"
    };

    /// <summary>
    /// 配置跨域
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="corsOptionsHandler"></param>
    /// <param name="corsPolicyBuilderHandler"></param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddCorsAccessor(this IServiceCollection services, Action<CorsOptions> corsOptionsHandler = default, Action<CorsPolicyBuilder> corsPolicyBuilderHandler = default)
    {
        // 添加跨域配置选项
        services.AddConfigurableOptions<CorsAccessorSettingsOptions>();

        // 获取选项
        var corsAccessorSettings = AppConfiguration.GetConfig<CorsAccessorSettingsOptions>("CorsAccessorSettings", true);

        // 添加跨域服务
        services.AddCors(options =>
        {
            // 添加策略跨域
            options.AddPolicy(name: corsAccessorSettings.PolicyName, builder =>
            {
                // 判断是否设置了来源，因为 AllowAnyOrigin 不能和 AllowCredentials一起公用
                var isNotSetOrigins = corsAccessorSettings.WithOrigins == null || corsAccessorSettings.WithOrigins.Length == 0;

                // 如果没有配置来源，则允许所有来源
                if (isNotSetOrigins) builder.AllowAnyOrigin();
                else builder.WithOrigins(corsAccessorSettings.WithOrigins)
                                  .SetIsOriginAllowedToAllowWildcardSubdomains();

                // 如果没有配置请求标头，则允许所有表头
                if (corsAccessorSettings.WithHeaders == null || corsAccessorSettings.WithHeaders.Length == 0) builder.AllowAnyHeader();
                else builder.WithHeaders(corsAccessorSettings.WithHeaders);

                // 如果没有配置任何请求谓词，则允许所有请求谓词
                if (corsAccessorSettings.WithMethods == null || corsAccessorSettings.WithMethods.Length == 0) builder.AllowAnyMethod();
                else builder.WithMethods(corsAccessorSettings.WithMethods);

                // 配置跨域凭据
                if (corsAccessorSettings.AllowCredentials == true && !isNotSetOrigins) builder.AllowCredentials();

                // 配置响应头，如果前端不能获取自定义的 header 信息，必须配置该项，默认配置了 access-token 和 x-access-token，可取消默认行为
                IEnumerable<string> exposedHeaders = corsAccessorSettings.FixedClientToken == true
                    ? _defaultExposedHeaders
                    : Array.Empty<string>();
                if (corsAccessorSettings.WithExposedHeaders != null && corsAccessorSettings.WithExposedHeaders.Length > 0)
                {
                    exposedHeaders = exposedHeaders.Concat(corsAccessorSettings.WithExposedHeaders).Distinct(StringComparer.OrdinalIgnoreCase);
                }

                if (exposedHeaders.Any()) builder.WithExposedHeaders(exposedHeaders.ToArray());

                // 设置预检过期时间，如果不设置默认为 24小时
                builder.SetPreflightMaxAge(TimeSpan.FromSeconds(corsAccessorSettings.SetPreflightMaxAge ?? 24 * 60 * 60));

                // 添加自定义配置
                corsPolicyBuilderHandler?.Invoke(builder);
            });

            // 添加自定义配置
            corsOptionsHandler?.Invoke(options);
        });

        return services;
    }
}