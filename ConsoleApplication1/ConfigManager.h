// <copyright company="Dell Inc.">
//     Confidential and Proprietary
//     Copyright © 2015 Dell Inc. 
//     ALL RIGHTS RESERVED.
// </copyright>
#pragma once
#include "ConfigBase.h"

template<class T>
class ConfigManager
{
public:
    void WriteConfig(ConfigWriter<T> * writer);

    void ReadConfig(ConfigWriter<T> * writer);
};

template <class T>
void ConfigManager<T>::WriteConfig(ConfigWriter<T> * writer)
{
    writer->WriteConfig((T*)writer);
}

template <class T>
void ConfigManager<T>::ReadConfig(ConfigWriter<T> * writer)
{
    writer->ReadConfig((T*)writer);
}