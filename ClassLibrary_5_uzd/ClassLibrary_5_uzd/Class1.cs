using System;
using System.IO;
using System.Management;
using Microsoft.Management.Infrastructure;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Linq;

namespace ClassLibrary_5_uzd
{
    /// <summary>
    /// Public class used for calling 2 functions with WMI object
    /// </summary>

    public class Machine
    {
        public IDictionary<string, object> SystemInfo()
        {
            IDictionary<string, object> mainObj = new Dictionary<string, object>(); //Creating main big object of Dictionary type
            Console.WriteLine("Getting machine name");
            mainObj.Add("MachineName", System.Environment.MachineName); //Getting machine name

            System.Management.ManagementObjectSearcher disk = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");  //Getting information
            Console.WriteLine("Reading Win32_DiskDrive");

            IDictionary<string, List<Object>> temp = new Dictionary<string, List<Object>>();    //Creating new dictionary to store disk information
            int counter = 0;
            foreach (ManagementObject n in disk.Get())
            {
                Console.WriteLine("Getting information about Disk" + counter);
                temp.Add("Disk" + counter, new List<object> { "Model of drive: " + n["Model"] });   //Manufacturer's model number of the disk drive. 
                temp["Disk" + counter].Add("Name of drive: " + n["Name"]);  //Label by which the object is known. When subclassed, the property can be overridden to be a key property. 
                temp["Disk" + counter].Add("Size of drive: " + (Convert.ToInt64(n["Size"]) / 1048576) + "MB");  //Size of the disk drive.
                                                                                                                //It is calculated by multiplying the total number of cylinders,
                                                                                                                //tracks in each cylinder, sectors in each track, and bytes in each sector.
                temp["Disk" + counter].Add("Drive is working: " + n["Status"]); //Current status of the object.
                                                                                //Various operational and nonoperational statuses can be defined.
                temp["Disk" + counter].Add("Number of partitions: " + n["Partitions"]); //Number of partitions on this physical disk drive that are recognized by the operating system.

                counter++;
            }
            mainObj.Add("Disks", temp); //Adding to the main object

            ManagementObjectSearcher processor = new ManagementObjectSearcher("select * from Win32_Processor");
            Console.WriteLine("Reading Win32_Processor");
            List<Object> processorInfo = new List<Object>();
            Console.WriteLine("Getting information about processor");
            foreach (ManagementObject obj in processor.Get())
            {
                processorInfo.Add(obj["Name"]);
                processorInfo.Add("Device ID: " + obj["DeviceID"]); //Unique identifier of a processor on the system. 
                var availability = obj["Availability"];
                string response = "Availability of processor: ";
                availability = Convert.ToInt32(availability);   //Converting to integer, so switch case can work
                //Checks for response number and just converts it to more useful text response (these are available in microsoft site)
                switch (availability)
                {
                    case 1:
                        response += "Other";
                        break;
                    case 2:
                        response += "Unknown";
                        break;
                    case 3:
                        response += "Running/Full power";
                        break;
                    case 4:
                        response += "Warning";
                        break;
                    case 5:
                        response += "In Test";
                        break;
                    case 6:
                        response += "Not Applicable";
                        break;
                    case 7:
                        response += "Power Off";
                        break;
                    default:
                        response += "Processor returned unexpected code " + availability + ". Please check what it means on the internet!";
                        break;
                }
                processorInfo.Add(response);
                var cpuStatus = obj["CpuStatus"];   //Current status of the processor.
                                                    //Status changes indicate processor usage, but not the physical condition of the processor. 
                response = "Status of CPU: ";
                cpuStatus = Convert.ToInt32(cpuStatus);
                switch (cpuStatus)
                {
                    case 0:
                        response += "Unknown";
                        break;
                    case 1:
                        response += "CPU Enabled";
                        break;
                    case 2:
                        response += "CPU Disabled by User via BIOS Setup";
                        break;
                    case 3:
                        response += "CPU Disabled By BIOS (POST Error)";
                        break;
                    case 4:
                        response += "CPU is Idle";
                        break;
                    case 5:
                        response += "Reserved";
                        break;
                    case 6:
                        response += "Reserved";
                        break;
                    case 7:
                        response += "Unknown";
                        break;
                    default:
                        response += "Unknown status returned: " + cpuStatus;
                        break;
                }
                processorInfo.Add(response);

                processorInfo.Add("Current clock speed: " + obj["CurrentClockSpeed"] + " MHz"); //Returns current processor clock speed in Mhz
                processorInfo.Add("CPU load percentage: " + obj["LoadPercentage"] + "%");   //Load capacity of each processor, averaged to the last second.
                                                                                            //Processor loading refers to the total computing burden for each processor at one time.
                processorInfo.Add("Number of cores: " + obj["NumberOfCores"]);  //Tries to read the number of cores in processor
                processorInfo.Add("Number of logical processors: " + obj["NumberOfLogicalProcessors"]);
                processorInfo.Add("Number of threads: " + obj["ThreadCount"]);
                processorInfo.Add("Current status of CPU: " + obj["Status"]);   //Current status of an object.
                processorInfo.Add("Processor ID: " + obj["ProcessorId"]);   //Processor information that describes the processor features.
            }
            mainObj.Add("Processor", processorInfo);

            System.Management.ManagementObjectSearcher RAM = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            Console.WriteLine("Reading Win32_OperatingSystem");
            List<Object> ramInfo = new List<Object>();
            Console.WriteLine("Getting information about RAM");
            foreach (var n in RAM.Get())
            {
                ramInfo.Add("Free RAM: " + Convert.ToInt64(n["FreePhysicalMemory"]) / 1024 + "MB"); //Number, in kilobytes, of physical memory currently unused and available.
                ramInfo.Add("Free virtual memory: " + Convert.ToInt64(n["FreeVirtualMemory"]) / 1024 + "MB");   //Number, in kilobytes, of virtual memory currently unused and available.
                ramInfo.Add("Total virtal memory: " + Convert.ToInt64(n["TotalVirtualMemorySize"]) / 1024 + "MB");//Number, in kilobytes, of virtual memory.
                                                                                                                  //For example, this may be calculated by adding the amount of total RAM to the amount of paging space.
                ramInfo.Add("Total RAM: " + Convert.ToInt64(n["TotalVisibleMemorySize"]) / 1024 + "MB");    //Total amount, in kilobytes, of physical memory available to the operating system.
                                                                                                            //This value does not necessarily indicate the true amount of physical memory,
                                                                                                            //but what is reported to the operating system as available to it.
            }
            mainObj.Add("RAM", ramInfo);
            Console.WriteLine("DONE");
            return mainObj; //Returns big main object
        }
        public IEnumerable<dynamic> GetEvents(string programName)
        {
            Console.WriteLine("Creating Application log object");
            EventLog myLogs = new EventLog("Application");  //Opens Application logs
            Console.WriteLine("Trying to find entries of specifc program...");
            var entries = myLogs.Entries.Cast<EventLogEntry>()  //Casts entries to EventLogEntry type
                                        .Where(log => log.Source == programName)    //Locates entries with provided name
                                        .Select(log => new  //Creates new List with anonymous type of string, string, DateTime
                                        {
                                            log.Source,
                                            log.Message,
                                            log.TimeWritten
                                        }).Take(10).ToList().Cast<dynamic>();   //Takes 10, converts them to a list and casts it for easier returning
            return entries;
        }
    }
}
