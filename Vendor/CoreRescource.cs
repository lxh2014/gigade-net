﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL.gigade.Mgr.Impl;
using System.Resources;
using System.Reflection;
namespace Vendor
{
    public class CoreResource : IResource
    {
        /// <summary>
        /// 资源文件类型，即类名
        /// </summary>
        private string cagetory;

        private ResourceManager manager;


        /// <summary>
        /// 初始化，创建 ResourceManager 对象
        /// </summary>
        public CoreResource(string resourceCagetory)
        {
            if (string.IsNullOrEmpty(resourceCagetory))
                cagetory = "ProductItemMap";
            else
                cagetory = resourceCagetory;

            //利用反射来动态获取对应的资源文件类
            manager = (ResourceManager)System.Type.GetType
                    ("Resources." + cagetory).GetProperty("ResourceManager",
                    BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null);

        }

        /// <summary>
        /// 根据KEY以获取相应的资源
        /// </summary>
        /// <param name="key">资源的 key</param>
        /// <returns></returns>
        public string GetResource(string key)
        {
            try
            {
                return manager.GetString(key);
            }
            catch (System.Resources.MissingManifestResourceException)
            {
                return "";
            }
        }
    }
}