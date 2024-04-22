﻿// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Air.Cloud.Core.Dependencies;
using Air.Cloud.Core.Standard.DataBase.Locators;
using Air.Cloud.Core.Standard.DataBase.Model;
using Air.Cloud.DataBase.Extensions.DatabaseProvider;
using Air.Cloud.DataBase.Repositories;
using Air.Cloud.DataBase.SqlProxies.Proxies;

namespace Air.Cloud.DataBase;

/// <summary>
/// 数据库公开类
/// </summary>
[IgnoreScanning]
public static class Db
{
    /// <summary>
    /// 迁移类库名称
    /// </summary>
    internal static string MigrationAssemblyName = "Air.Database.Migrations";
    /// <summary>
    /// 获取非泛型仓储
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static IRepository GetRepository(IServiceProvider serviceProvider = default)
    {
        return AppCore.GetService<IRepository>(serviceProvider);
    }

    /// <summary>
    /// 获取实体仓储
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns>IRepository{TEntity}</returns>
    public static IRepository<TEntity> GetRepository<TEntity>(IServiceProvider serviceProvider = default)
        where TEntity : class, IPrivateEntity, new()
    {
        return AppCore.GetService<IRepository<TEntity>>(serviceProvider);
    }

    /// <summary>
    /// 获取实体仓储
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TDbContextLocator">数据库上下文定位器</typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns>IRepository{TEntity, TDbContextLocator}</returns>
    public static IRepository<TEntity, TDbContextLocator> GetRepository<TEntity, TDbContextLocator>(IServiceProvider serviceProvider = default)
        where TEntity : class, IPrivateEntity, new()
        where TDbContextLocator : class, IDbContextLocator
    {
        return AppCore.GetService<IRepository<TEntity, TDbContextLocator>>(serviceProvider);
    }

    /// <summary>
    /// 根据定位器类型获取仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="dbContextLocator"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static IPrivateRepository<TEntity> GetRepository<TEntity>(Type dbContextLocator, IServiceProvider serviceProvider = default)
         where TEntity : class, IPrivateEntity, new()
    {
        return AppCore.GetService(typeof(IRepository<,>).MakeGenericType(typeof(TEntity), dbContextLocator), serviceProvider) as IPrivateRepository<TEntity>;
    }

    /// <summary>
    /// 获取特定数据库上下文仓储
    /// </summary>
    /// <typeparam name="TDbContextLocator">数据库上下文定位器</typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static IDbRepository<TDbContextLocator> GetDbRepository<TDbContextLocator>(IServiceProvider serviceProvider = default)
        where TDbContextLocator : class, IDbContextLocator
    {
        return AppCore.GetService<IDbRepository<TDbContextLocator>>(serviceProvider);
    }

    /// <summary>
    /// 获取Sql仓储
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns>ISqlRepository</returns>
    public static ISqlRepository GetSqlRepository(IServiceProvider serviceProvider = default)
    {
        return AppCore.GetService<ISqlRepository>(serviceProvider);
    }

    /// <summary>
    /// 获取Sql仓储
    /// </summary>
    /// <typeparam name="TDbContextLocator">数据库上下文定位器</typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns>ISqlRepository{TDbContextLocator}</returns>
    public static ISqlRepository<TDbContextLocator> GetSqlRepository<TDbContextLocator>(IServiceProvider serviceProvider = default)
        where TDbContextLocator : class, IDbContextLocator
    {
        return AppCore.GetService<ISqlRepository<TDbContextLocator>>(serviceProvider);
    }

    /// <summary>
    /// 获取随机主从库仓储
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns>ISqlRepository</returns>
    public static IMSRepository GetMSRepository(IServiceProvider serviceProvider = default)
    {
        return AppCore.GetService<IMSRepository>(serviceProvider);
    }

    /// <summary>
    /// 获取随机主从库仓储
    /// </summary>
    /// <typeparam name="TMasterDbContextLocator">主库数据库上下文定位器</typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns>IMSRepository{TDbContextLocator}</returns>
    public static IMSRepository<TMasterDbContextLocator> GetMSRepository<TMasterDbContextLocator>(IServiceProvider serviceProvider = default)
        where TMasterDbContextLocator : class, IDbContextLocator
    {
        return AppCore.GetService<IMSRepository<TMasterDbContextLocator>>(serviceProvider);
    }

    /// <summary>
    /// 获取 Sql 代理
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns>ISqlRepository</returns>
    public static TSqlDispatchProxy GetSqlProxy<TSqlDispatchProxy>(IServiceProvider serviceProvider = default)
        where TSqlDispatchProxy : class, ISqlDispatchProxy
    {
        return AppCore.GetService<TSqlDispatchProxy>(serviceProvider);
    }

    /// <summary>
    /// 获取默认数据库上下文
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static DbContext GetDbContext(IServiceProvider serviceProvider = default)
    {
        return GetDbContext(typeof(MasterDbContextLocator), serviceProvider);
    }

    /// <summary>
    /// 获取特定数据库上下文
    /// </summary>
    /// <param name="dbContextLocator">数据库上下文定位器</param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static DbContext GetDbContext(Type dbContextLocator, IServiceProvider serviceProvider = default)
    {
        // 判断数据库上下文定位器是否绑定
        Penetrates.CheckDbContextLocator(dbContextLocator, out _);

        var dbContextResolve = AppCore.GetService<Func<Type, IScoped, DbContext>>(serviceProvider);
        return dbContextResolve(dbContextLocator, default);
    }

    /// <summary>
    /// 获取特定数据库上下文
    /// </summary>
    /// <typeparam name="TDbContextLocator">数据库上下文定位器</typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static DbContext GetDbContext<TDbContextLocator>(IServiceProvider serviceProvider = default)
        where TDbContextLocator : class, IDbContextLocator
    {
        return GetDbContext(typeof(TDbContextLocator), serviceProvider);
    }
}