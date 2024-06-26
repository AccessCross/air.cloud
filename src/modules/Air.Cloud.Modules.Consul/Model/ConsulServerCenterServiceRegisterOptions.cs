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
using Air.Cloud.Core.Standard.ServerCenter;

namespace Air.Cloud.Modules.Consul.Model
{
    public class ConsulServerCenterServiceRegisterOptions: IServerCenterServiceRegisterOptions
    {
        public string ServiceAddress { get; set; }
        public string ServiceName { get; set; }
        public string ServiceKey { get; set; }
        public TimeSpan Timeout { get; set; }
        public TimeSpan DeregisterCriticalServiceAfter { get; set; }
        public string HealthCheckRoute { get; set; }
        public TimeSpan HealthCheckTimeStep { get; set; }
    }
}
