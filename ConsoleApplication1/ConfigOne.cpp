#include "stdafx.h"
#include "ConfigOne.h"
#include <cstdio>

void ConfigOne::WriteConfig(ConfigOne * config)
{
    printf("First config written\n");
}

void ConfigOne::ReadConfig(ConfigOne * config)
{
    printf("First config read.\n");
    printf("%s", __FUNCTION__);
}

