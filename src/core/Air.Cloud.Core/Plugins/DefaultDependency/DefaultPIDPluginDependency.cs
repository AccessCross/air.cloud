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
using Air.Cloud.Core.Plugins.PID;
using Air.Cloud.Core.Plugins.Security.MD5;
using Air.Cloud.Core.Standard.DefaultDependencies;

namespace Air.Cloud.Core.Plugins.DefaultDependency
{
    /// <summary>
    /// PID Provider
    /// </summary>
    public class DefaultPIDPluginDependency : IPIDPlugin
    {
        /// <summary>
        /// 写入PID
        /// </summary>
        /// <param name="PID">写入值 可为空 为空系统自动生成</param>
        /// <remarks>
        /// </remarks>
        public string Set(string PID = null)
        {
            if (!File.Exists(IPIDPlugin.StartPath))
            {
                File.Create(IPIDPlugin.StartPath).Close();
            }
            PID = PID ?? MD5Encryption.GetMd5By32(IPIDPlugin.StartPath);
            using (StreamWriter file = new StreamWriter(IPIDPlugin.StartPath))
            {
                file.Write(PID);
                file.Close();
            }
            return PID;
        }

        /// <summary>
        /// 读取PID
        /// </summary>
        /// <returns>PID</returns>
        public string Get()
        {
            if (File.Exists(IPIDPlugin.StartPath))
            {
                File.Delete(IPIDPlugin.StartPath);
            }
            string PID = Set();
            return PID;
        }
    }
}
