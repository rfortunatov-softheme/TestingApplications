// ConsoleApplication1.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <Windows.h>
#include <ctime>
#include "ConfigManager.h"
#include "ConfigOne.h"
#include "ConfigTwo.h"

void StopService(const wchar_t * serviceName, const wchar_t * hostName)
{
    printf("Stopping service %ws \n", serviceName);
    SC_HANDLE serviceManager = OpenSCManager(hostName, NULL, GENERIC_ALL);
    SERVICE_STATUS status;
    if (serviceManager)
    {
        SC_HANDLE service = OpenService(serviceManager, serviceName, SC_MANAGER_ALL_ACCESS);
        if (service)
        {
            while (QueryServiceStatus(service, &status))
            {
                if (status.dwCurrentState == SERVICE_RUNNING)
                {
                    break;
                }

                Sleep(100);
            }

            clock_t start = clock(), diff;            
            
            if (!ControlService(service, SERVICE_CONTROL_STOP, &status))
            {
                QueryServiceStatus(service, &status);
                diff = clock() - start;

                int msec = diff * 1000 / CLOCKS_PER_SEC;
                printf("Time taken %d seconds %d milliseconds\n", msec / 1000, msec % 1000);
            }
            else
            {                
                if (QueryServiceStatus(service, &status))
                {
                    while (SERVICE_STOPPED != status.dwCurrentState)
                    {
                        DWORD checkPoint = status.dwCheckPoint;
                        printf("Service stop checkpoint is: %d", checkPoint);
                        //dwCheckPoint contains a value incremented periodically to report progress of a long operation.  Store it.
                        Sleep(21000); //Sleep for recommended time before checking status again                        
                        if (!QueryServiceStatus(service, &status))
                        {
                            break; //couldn't check status
                        }

                        if (status.dwCheckPoint < checkPoint)
                        {
                            break; //if QueryServiceStatus didn't work for some reason, avoid infinite loop
                        }
                    } //while not running

                    diff = clock() - start;

                    int msec = diff * 1000 / CLOCKS_PER_SEC;
                    printf("Time taken %d seconds %d milliseconds\n", msec / 1000, msec % 1000);
                    CloseServiceHandle(service);
                }
            } //if able to get service handle

            CloseServiceHandle(serviceManager);
        } //if able to get svc mgr handle
    }
}

void StartService(const wchar_t * serviceName, const wchar_t * hostName)
{
    SC_HANDLE serviceManager = OpenSCManager(hostName, NULL, GENERIC_ALL);
    SERVICE_STATUS status;
    if (serviceManager)
    {
        SC_HANDLE service = OpenService(serviceManager, serviceName, SC_MANAGER_ALL_ACCESS);
        if (service)
        {
            while (QueryServiceStatus(service, &status))
            {
                if (status.dwCurrentState == SERVICE_STOPPED)
                {
                    break;
                }

                Sleep(100);
            }

            if (!ControlService(service, SERVICE_CONTROL_CONTINUE, &status) && GetLastError() != 1053)
            {
                QueryServiceStatus(service, &status);
            }
            else
            {
                if (QueryServiceStatus(service, &status))
                {
                    while (SERVICE_RUNNING != status.dwCurrentState)
                    {
                        DWORD checkPoint = status.dwCheckPoint;
                        //dwCheckPoint contains a value incremented periodically to report progress of a long operation.  Store it.
                        Sleep(10); //Sleep for recommended time before checking status again
                        if (!QueryServiceStatus(service, &status))
                        {
                            break; //couldn't check status
                        }

                        if (status.dwCheckPoint < checkPoint)
                        {
                            break; //if QueryServiceStatus didn't work for some reason, avoid infinite loop
                        }
                    } //while not running

                    CloseServiceHandle(service);
                }
            } //if able to get service handle

            CloseServiceHandle(serviceManager);
        } //if able to get svc mgr handle
    }
}

int _tmain(int argc, _TCHAR* argv[])
{    
    ConfigManager<ConfigOne> first;
    ConfigOne config;
    first.ReadConfig(&config);
    first.WriteConfig(&config);

    ConfigManager<ConfigTwo> second;
    ConfigTwo conf;
    second.ReadConfig(&conf);
    second.WriteConfig(&conf);

    return 0;
}
