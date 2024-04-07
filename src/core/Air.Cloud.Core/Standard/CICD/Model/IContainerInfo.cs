﻿/*
 * Copyright (c) 2024 星曳数据
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * This file is provided under the Mozilla Public License Version 2.0,
 * and the "NO WARRANTY" clause of the MPL is hereby expressly
 * acknowledged.
 */
namespace Air.Cloud.Core.Standard.CICD.Model
{
    /// <summary>
    /// 容器信息
    /// </summary>
    public interface IContainerInfo
    {
        /// <summary>
        /// 容器ID
        /// </summary>
        public string ContainerId { get; set; }
        /// <summary>
        /// 容器名称
        /// </summary>
        public string ContainerName { get; set; }
        /// <summary>
        /// 容器标识
        /// </summary>
        public string Tag { get; set; }
    }
}
