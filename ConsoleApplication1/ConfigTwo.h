// <copyright company="Dell Inc.">
//     Confidential and Proprietary
//     Copyright © 2015 Dell Inc. 
//     ALL RIGHTS RESERVED.
// </copyright>
#pragma once
#include "ConfigBase.h"

class ConfigTwo : public ConfigWriter<ConfigTwo>
{
public:
    void WriteConfig(ConfigTwo * config) override;

    void ReadConfig(ConfigTwo * config) override;
};
