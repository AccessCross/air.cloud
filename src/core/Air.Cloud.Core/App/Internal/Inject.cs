﻿// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Air.Cloud.Core.Standard;

using System.Diagnostics;
using System.Reflection;

namespace Air.Cloud.Core.App.Internal;

/// <summary>
/// 跨平台 Inject
/// </summary>
public static class Inject
{
    /// <summary>
    /// 创建初始服务集合
    /// </summary>
    /// <param name="configureLogging">配置日志</param>
    /// <returns></returns>
    public static IServiceCollection Create(Action<ILoggingBuilder> configureLogging = default)
    {
        // 监听全局异常
        AppDomain.CurrentDomain.UnhandledException += AppStandardRealization.AppDomainExceptionHandlerStandard.OnException;

        // 创建配置构建器
        var configurationBuilder = new ConfigurationBuilder();

        // 存储配置对象
        // var configuration = InternalApp.InnerConfiguration = configurationBuilder.Build();
        var configuration = InternalApp.Configuration;

        // 创建服务对象和存储服务提供器
        var services = InternalApp.InternalServices = new ServiceCollection();

        // 添加默认控制台日志处理程序
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
            loggingBuilder.AddConsole(); // 将日志输出到控制台
            configureLogging?.Invoke(loggingBuilder);
        });

        // 初始化应用服务
        services.AddApplication();

        return services;
    }
}