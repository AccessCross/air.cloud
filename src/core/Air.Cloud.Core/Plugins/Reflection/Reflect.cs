// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Reflection;
using System.Runtime.Loader;

namespace Air.Cloud.Core.Plugins.Reflection;

/// <summary>
/// 内部反射静态类
/// </summary>
public static class Reflect
{
    /// <summary>
    /// 获取入口程序集
    /// </summary>
    /// <returns></returns>
    public static Assembly GetEntryAssembly()
    {
        return Assembly.GetEntryAssembly();
    }

    /// <summary>
    /// 根据程序集名称获取运行时程序集
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public static Assembly GetAssembly(string assemblyName)
    {
        try
        {
            return AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
        }
        catch (Exception)
        {
            return default;
        }

    }
    /// <summary>
    /// 根据路径加载程序集
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Assembly LoadAssembly(string path)
    {
        if (!File.Exists(path)) return default;
        return Assembly.LoadFrom(path);
    }

    /// <summary>
    /// 通过流加载程序集
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static Assembly LoadAssembly(MemoryStream assembly)
    {
        return Assembly.Load(assembly.ToArray());
    }

    /// <summary>
    /// 根据程序集名称、类型完整限定名获取运行时类型
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="typeFullName"></param>
    /// <returns></returns>
    public static Type GetType(string assemblyName, string typeFullName)
    {
        return GetAssembly(assemblyName).GetType(typeFullName);
    }

    /// <summary>
    /// 根据程序集和类型完全限定名获取运行时类型
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="typeFullName"></param>
    /// <returns></returns>
    public static Type GetType(Assembly assembly, string typeFullName)
    {
        return assembly.GetType(typeFullName);
    }

    /// <summary>
    /// 根据程序集和类型完全限定名获取运行时类型
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="typeFullName"></param>
    /// <returns></returns>
    public static Type GetType(MemoryStream assembly, string typeFullName)
    {
        return LoadAssembly(assembly).GetType(typeFullName);
    }

    /// <summary>
    /// 获取程序集名称
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static string GetAssemblyName(Assembly assembly)
    {
        return assembly.GetName().Name;
    }

    /// <summary>
    /// 获取程序集名称
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetAssemblyName(Type type)
    {
        return GetAssemblyName(type.GetTypeInfo());
    }

    /// <summary>
    /// 获取程序集名称
    /// </summary>
    /// <param name="typeInfo"></param>
    /// <returns></returns>
    public static string GetAssemblyName(TypeInfo typeInfo)
    {
        return GetAssemblyName(typeInfo.Assembly);
    }

    /// <summary>
    /// 加载程序集类型，支持格式：程序集;网站类型命名空间
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Type GetStringType(string str)
    {
        var typeDefinitions = str.Split(";");
        return GetType(typeDefinitions[0], typeDefinitions[1]);
    }
}