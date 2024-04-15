﻿// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Air.Cloud.Core.App;
using Air.Cloud.Core.Dependencies;
using Air.Cloud.Core.Extensions;
using Air.Cloud.Core.Standard.DynamicServer;
using Air.Cloud.WebApp.FriendlyException.Attributes;
using Air.Cloud.WebApp.FriendlyException.Exceptions;
using Air.Cloud.WebApp.FriendlyException.Extensions;
using Air.Cloud.WebApp.FriendlyException.Internal;
using Air.Cloud.WebApp.FriendlyException.Options;
using Air.Cloud.WebApp.FriendlyException.Providers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace Air.Cloud.WebApp.FriendlyException;

/// <summary>
/// 抛异常静态类
/// </summary>
[IgnoreScanning]
public static class Oops
{
    /// <summary>
    /// 方法错误异常特性
    /// </summary>
    private static readonly ConcurrentDictionary<MethodBase, MethodIfException> ErrorMethods;

    /// <summary>
    /// 错误代码类型
    /// </summary>
    private static readonly IEnumerable<Type> ErrorCodeTypes;

    /// <summary>
    /// 错误消息字典
    /// </summary>
    private static readonly ConcurrentDictionary<string, string> ErrorCodeMessages;

    /// <summary>
    /// 友好异常设置
    /// </summary>
    private static readonly FriendlyExceptionSettingsOptions _friendlyExceptionSettings;

    /// <summary>
    /// 构造函数
    /// </summary>
    static Oops()
    {
        ErrorMethods = new ConcurrentDictionary<MethodBase, MethodIfException>();
        _friendlyExceptionSettings = AppConfiguration.GetConfig<FriendlyExceptionSettingsOptions>("FriendlyExceptionSettings", true);
        ErrorCodeTypes = GetErrorCodeTypes();
        ErrorCodeMessages = GetErrorCodeMessages();
    }

    /// <summary>
    /// 抛出业务异常信息
    /// </summary>
    /// <param name="errorMessage">异常消息</param>
    /// <param name="args">String.Format 参数</param>
    /// <returns>异常实例</returns>
    public static AppFriendlyException Bah(string errorMessage, params object[] args)
    {
        var friendlyException = Oh(errorMessage, typeof(ValidationException), args).StatusCode(StatusCodes.Status400BadRequest);
        friendlyException.ValidationException = true;
        return friendlyException;
    }

    /// <summary>
    /// 抛出业务异常信息
    /// </summary>
    /// <param name="errorCode">错误码</param>
    /// <param name="args">String.Format 参数</param>
    /// <returns>异常实例</returns>
    public static AppFriendlyException Bah(object errorCode, params object[] args)
    {
        var friendlyException = Oh(errorCode, typeof(ValidationException), args).StatusCode(StatusCodes.Status400BadRequest);
        friendlyException.ValidationException = true;
        return friendlyException;
    }

    /// <summary>
    /// 抛出字符串异常
    /// </summary>
    /// <param name="errorMessage">异常消息</param>
    /// <param name="args">String.Format 参数</param>
    /// <returns>异常实例</returns>
    public static AppFriendlyException Oh(string errorMessage, params object[] args)
    {
        var friendlyException = new AppFriendlyException(MontageErrorMessage(errorMessage, default, args), default);

        // 处理默认配置为业务异常问题
        if (_friendlyExceptionSettings.ThrowBah == true)
        {
            friendlyException.StatusCode(StatusCodes.Status400BadRequest);
            friendlyException.ValidationException = true;
        }
        return friendlyException;
    }

    /// <summary>
    /// 抛出字符串异常
    /// </summary>
    /// <param name="errorMessage">异常消息</param>
    /// <param name="exceptionType">具体异常类型</param>
    /// <param name="args">String.Format 参数</param>
    /// <returns>异常实例</returns>
    public static AppFriendlyException Oh(string errorMessage, Type exceptionType, params object[] args)
    {
        var exceptionMessage = MontageErrorMessage(errorMessage, default, args);
        return new AppFriendlyException(exceptionMessage, default,
            Activator.CreateInstance(exceptionType, new object[] { exceptionMessage }) as Exception);
    }

    /// <summary>
    /// 抛出字符串异常
    /// </summary>
    /// <typeparam name="TException">具体异常类型</typeparam>
    /// <param name="errorMessage">异常消息</param>
    /// <param name="args">String.Format 参数</param>
    /// <returns>异常实例</returns>
    public static AppFriendlyException Oh<TException>(string errorMessage, params object[] args)
        where TException : class
    {
        return Oh(errorMessage, typeof(TException), args);
    }

    /// <summary>
    /// 抛出错误码异常
    /// </summary>
    /// <param name="errorCode">错误码</param>
    /// <param name="args">String.Format 参数</param>
    /// <returns>异常实例</returns>
    public static AppFriendlyException Oh(object errorCode, params object[] args)
    {
        var friendlyException = new AppFriendlyException(GetErrorCodeMessage(errorCode, args), errorCode);

        // 处理默认配置为业务异常问题
        if (_friendlyExceptionSettings.ThrowBah == true)
        {
            friendlyException.StatusCode(StatusCodes.Status400BadRequest);
            friendlyException.ValidationException = true;
        }
        return friendlyException;
    }

    /// <summary>
    /// 抛出错误码异常
    /// </summary>
    /// <param name="errorCode">错误码</param>
    /// <param name="exceptionType">具体异常类型</param>
    /// <param name="args">String.Format 参数</param>
    /// <returns>异常实例</returns>
    public static AppFriendlyException Oh(object errorCode, Type exceptionType, params object[] args)
    {
        var exceptionMessage = GetErrorCodeMessage(errorCode, args);
        return new AppFriendlyException(exceptionMessage, errorCode,
            Activator.CreateInstance(exceptionType, new object[] { exceptionMessage }) as Exception);
    }

    /// <summary>
    /// 抛出错误码异常
    /// </summary>
    /// <typeparam name="TException">具体异常类型</typeparam>
    /// <param name="errorCode">错误码</param>
    /// <param name="args">String.Format 参数</param>
    /// <returns>异常实例</returns>
    public static AppFriendlyException Oh<TException>(object errorCode, params object[] args)
        where TException : class
    {
        return Oh(errorCode, typeof(TException), args);
    }

    /// <summary>
    /// 获取错误码消息
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    private static string GetErrorCodeMessage(object errorCode, params object[] args)
    {
        errorCode = HandleEnumErrorCode(errorCode);

        // 获取出错的方法
        var methodIfException = GetEndPointExceptionMethod();

        // 获取当前状态码匹配异常特性
        var ifExceptionAttribute = methodIfException.IfExceptionAttributes.FirstOrDefault(u => u.ErrorCode != null && HandleEnumErrorCode(u.ErrorCode).ToString().Equals(errorCode.ToString()));

        // 获取错误码消息
        var errorCodeMessage = ifExceptionAttribute == null || string.IsNullOrWhiteSpace(ifExceptionAttribute.ErrorMessage)
            ? ErrorCodeMessages.GetValueOrDefault(errorCode.ToString()) ?? _friendlyExceptionSettings.DefaultErrorMessage
            : ifExceptionAttribute.ErrorMessage;

        // 如果所有错误码都获取不到，则找全局 [IfException] 错误
        if (string.IsNullOrWhiteSpace(errorCodeMessage))
        {
            errorCodeMessage = methodIfException.IfExceptionAttributes.FirstOrDefault(u => u.ErrorCode == null && !string.IsNullOrWhiteSpace(u.ErrorMessage))?.ErrorMessage;
        }

        // 字符串格式化
        return MontageErrorMessage(errorCodeMessage, errorCode.ToString()
            , args != null && args.Length > 0 ? args : ifExceptionAttribute?.Args);
    }

    /// <summary>
    /// 处理枚举类型错误码
    /// </summary>
    /// <param name="errorCode">错误码</param>
    /// <returns></returns>
    private static object HandleEnumErrorCode(object errorCode)
    {
        // 获取类型
        var errorType = errorCode.GetType();

        // 判断是否是内置枚举类型，如果是解析特性
        if (ErrorCodeTypes.Any(u => u == errorType))
        {
            var fieldinfo = errorType.GetField(Enum.GetName(errorType, errorCode));
            if (fieldinfo.IsDefined(typeof(ErrorCodeItemMetadataAttribute), true))
            {
                errorCode = GetErrorCodeItemMessage(fieldinfo).Key;
            }
        }

        return errorCode;
    }

    /// <summary>
    /// 获取错误代码类型
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<Type> GetErrorCodeTypes()
    {
        // 查找所有公开的枚举贴有 [ErrorCodeType] 特性的类型
        var errorCodeTypes = AppCore.EffectiveTypes
            .Where(u => u.IsDefined(typeof(ErrorCodeTypeAttribute), true) && u.IsEnum);

        // 获取错误代码提供器中定义的类型
        var errorCodeTypeProvider = AppCore.GetService<IErrorCodeTypeProvider>(AppCore.RootServices);
        if (errorCodeTypeProvider is { Definitions: not null }) errorCodeTypes = errorCodeTypes.Concat(errorCodeTypeProvider.Definitions);

        return errorCodeTypes.Distinct();
    }

    /// <summary>
    /// 获取所有错误消息
    /// </summary>
    /// <returns></returns>
    private static ConcurrentDictionary<string, string> GetErrorCodeMessages()
    {
        var defaultErrorCodeMessages = new ConcurrentDictionary<string, string>();

        // 查找所有 [ErrorCodeType] 类型中的 [ErrorCodeMetadata] 元数据定义
        var errorCodeMessages = ErrorCodeTypes.SelectMany(u => u.GetFields().Where(u => u.IsDefined(typeof(ErrorCodeItemMetadataAttribute))))
            .Select(u => GetErrorCodeItemMessage(u))
           .ToDictionary(u => u.Key.ToString(), u => u.Value);

        defaultErrorCodeMessages.Concat(errorCodeMessages);

        // 加载配置文件状态码
        var errorCodeMessageSettings = AppConfiguration.GetConfig<ErrorCodeMessageSettingsOptions>("ErrorCodeMessageSettings", true);
        if (errorCodeMessageSettings is { Definitions: not null })
        {
            // 获取所有参数大于1的配置
            var fitErrorCodes = errorCodeMessageSettings.Definitions
                .Where(u => u.Length > 1)
                .ToDictionary(u => u[0].ToString(), u => FixErrorCodeSettingMessage(u));

            defaultErrorCodeMessages.Concat(fitErrorCodes);
        }

        return defaultErrorCodeMessages;
    }

    /// <summary>
    /// 处理异常配置数据
    /// </summary>
    /// <param name="errorCodes">错误消息配置对象</param>
    /// <remarks>
    /// 方式：数组第一个元素为错误码，第二个参数为错误消息，剩下的参数为错误码格式化字符串
    /// </remarks>
    /// <returns></returns>
    private static string FixErrorCodeSettingMessage(object[] errorCodes)
    {
        var args = errorCodes.Skip(2).ToArray();
        var errorMessage = errorCodes[1].ToString();
        return string.Format(errorMessage, args);
    }

    /// <summary>
    /// 获取堆栈中顶部抛异常方法
    /// </summary>
    /// <returns></returns>
    private static MethodIfException GetEndPointExceptionMethod()
    {
        // 获取调用堆栈信息
        var stackTrace = EnhancedStackTrace.Current();

        // 获取出错的堆栈信息，在 web 请求中获取控制器或动态API的方法，除外获取第一个出错的方法
        var stackFrame = stackTrace.FirstOrDefault(u => typeof(ControllerBase).IsAssignableFrom(u.MethodInfo.DeclaringType) || typeof(IDynamicService).IsAssignableFrom(u.MethodInfo.DeclaringType))
            ?? stackTrace.FirstOrDefault(u => u.GetMethod().DeclaringType.Namespace != typeof(Oops).Namespace);

        // 获取出错的方法
        var errorMethod = stackFrame.MethodInfo.MethodBase;

        // 判断是否已经缓存过该方法，避免重复解析
        var isCached = ErrorMethods.TryGetValue(errorMethod, out var methodIfException);
        if (isCached) return methodIfException;

        // 获取堆栈中所有的 [IfException] 特性
        var ifExceptionAttributes = stackTrace
            .Where(u => u.MethodInfo.MethodBase != null && u.MethodInfo.MethodBase.IsDefined(typeof(IfExceptionAttribute), true))
            .SelectMany(u => u.MethodInfo.MethodBase.GetCustomAttributes<IfExceptionAttribute>(true));

        // 组装方法异常对象
        methodIfException = new MethodIfException
        {
            ErrorMethod = errorMethod,
            IfExceptionAttributes = ifExceptionAttributes
        };

        // 存入缓存
        ErrorMethods.TryAdd(errorMethod, methodIfException);

        return methodIfException;
    }

    /// <summary>
    /// 获取错误代码消息实体
    /// </summary>
    /// <param name="fieldInfo">字段对象</param>
    /// <returns>(object key, object value)</returns>
    private static (object Key, string Value) GetErrorCodeItemMessage(FieldInfo fieldInfo)
    {
        var errorCodeItemMetadata = fieldInfo.GetCustomAttribute<ErrorCodeItemMetadataAttribute>();
        return (errorCodeItemMetadata.ErrorCode ?? fieldInfo.Name, string.Format(errorCodeItemMetadata.ErrorMessage, errorCodeItemMetadata.Args));
    }

    /// <summary>
    /// 获取错误码字符串
    /// </summary>
    /// <param name="errorMessage"></param>
    /// <param name="errorCode"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    private static string MontageErrorMessage(string errorMessage, string errorCode, params object[] args)
    {
        // 判断是否隐藏错误码
        var msg = (_friendlyExceptionSettings.HideErrorCode == true || string.IsNullOrWhiteSpace(errorCode)
            ? string.Empty
            : $"[{errorCode}] ") + errorMessage;

        return string.Format(msg, args);
    }
}