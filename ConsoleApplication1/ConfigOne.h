// <copyright company="Dell Inc.">
//     Confidential and Proprietary
//     Copyright © 2015 Dell Inc. 
//     ALL RIGHTS RESERVED.
// </copyright>
#pragma once
#include "ConfigBase.h"

class ConfigOne : public ConfigWriter<ConfigOne>
{
public:
    void WriteConfig(ConfigOne * config) override;

    void ReadConfig(ConfigOne * config) override;
};
