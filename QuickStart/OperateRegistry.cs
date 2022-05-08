using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickStart
{
    internal class OperateRegistry
    {
        public static RegistryKey GetRegistryKeyByHelper()
        {
            return GetRegistryKeyByHelper(null, false);
        }

        public static RegistryKey GetRegistryKeyByHelper(string keyPath)
        {
            return GetRegistryKeyByHelper(keyPath, false);
        }

        public static RegistryKey GetRegistryKeyByHelper(string keyPath, bool writable)
        {
            RegistryKey localMachineRegistry
                = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser,
                                          System.Environment.Is64BitOperatingSystem
                                              ? RegistryView.Registry64
                                              : RegistryView.Registry32);

            return string.IsNullOrEmpty(keyPath)
                ? localMachineRegistry
                : localMachineRegistry.OpenSubKey(keyPath, writable);
        }

        public static bool isExistItemKeyName(RegistryKey registryKey, String subKeyName)
        {
            bool bExists = false;
            string[] subValueNames = registryKey.GetValueNames();

            foreach (string _itemName in subValueNames)
            {
                //判断子项的名称
                if (_itemName == subKeyName)
                {
                    bExists = true;
                    break;
                }
            }

            return bExists;
        }
    }
}
