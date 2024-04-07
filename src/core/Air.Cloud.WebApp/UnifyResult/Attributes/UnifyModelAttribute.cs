﻿// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.


namespace Air.Cloud.WebApp.UnifyResult.Attributes;

/// <summary>
/// 规范化模型特性
/// </summary>
[IgnoreScanning, AttributeUsage(AttributeTargets.Class)]
public class UnifyModelAttribute : Attribute
{
    /// <summary>
    /// 规范化模型
    /// </summary>
    /// <param name="modelType"></param>
    public UnifyModelAttribute(Type modelType)
    {
        ModelType = modelType;
    }

    /// <summary>
    /// 模型类型（泛型）
    /// </summary>
    public Type ModelType { get; set; }
}