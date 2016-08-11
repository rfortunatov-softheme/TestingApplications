// <copyright company="Dell Inc.">
//     Confidential and Proprietary
//     Copyright © 2015 Dell Inc. 
//     ALL RIGHTS RESERVED.
// </copyright>
#pragma once

template <class T>
class ConfigWriter
{
public:
    virtual void WriteConfig(T * config) = 0;

    virtual void ReadConfig(T * config) = 0;
};
