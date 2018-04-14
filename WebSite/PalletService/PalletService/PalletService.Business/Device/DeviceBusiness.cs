
using PalletService.Business.SysConfig;
using PalletService.Common;
using PalletService.Model.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PalletService.Business.Device
{
    public class DeviceBusiness
    {
        public static List<SelectedDeviceModel> SelectedDeviceList = new List<SelectedDeviceModel>();

        /// <summary>
        /// 获取选择的设备名称
        /// </summary>
        /// <param name="deviceType">设备类型</param>
        /// <param name="deviceModel">设备名称</param>
        /// <returns></returns>
        public static bool GetSelectedDevice(string deviceType,out string deviceModel)
        {
            deviceModel = string.Empty;
            SelectedDeviceModel selModel = SelectedDeviceList.Find(p => p.Type.Equals(deviceType));
            if (selModel == null)
            {
                deviceModel = selModel.Model;
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 设置选择的设备
        /// </summary>
        /// <param name="deviceType">设备类型</param>
        /// <param name="deviceModel">设备名称</param>
        public static void SetSelectedDeviceList(string deviceType,string deviceModel)
        {
            if (SelectedDeviceList.Find(p => p.Type.Equals(deviceType)) == null)
            {
                SelectedDeviceModel selModel = new SelectedDeviceModel(deviceType, deviceModel);
                SelectedDeviceList.Add(selModel);
            }
            else
            {
                SelectedDeviceList.RemoveAll(p => p.Type.Equals(deviceType));
                SelectedDeviceModel selModel = new SelectedDeviceModel(deviceType, deviceModel);
                SelectedDeviceList.Add(selModel);
            }
        }

        public static void SetSelectedDevice(string deviceType,string deviceModel)
        {
            string path = GetPath(deviceType);
            string xmlFilePath = string.Format("{0}//{1}", SysConfigBusiness.ApplicationStartPath, "SysConfig.xml");
            XmlDocument xd = new XmlDocument();
            xd.Load(xmlFilePath);
            var node1 = xd.SelectSingleNode(string.Format("{0}[@Model=\"{1}\"]", path,deviceModel));
            var nodes = xd.SelectNodes(string.Format("{0}[@Selected=\"{1}\"]", path, "1"));
            bool b = false;//当前选择的节点是否已被选择
            foreach (XmlNode node in nodes)
            {
                if (!node.Attributes["Model"].Value.ToString().Equals(node1.Attributes["Model"].Value.ToString()))
                {
                    node.Attributes["Selected"].Value = "0";
                }
                else
                {
                    b = true;
                }
            }
            if (!b)
            { 
                node1.Attributes["Selected"].Value = "1";             
            }
            xd.Save(xmlFilePath);
        }

        public static void ReplaceDevice(List<DeviceModel> list,string deviceType)
        {
            string path = GetPath(deviceType);
            string xmlFilePath = string.Format("{0}//{1}", SysConfigBusiness.ApplicationStartPath, "SysConfig.xml");
            XmlDocument xd = new XmlDocument();
            xd.Load(xmlFilePath);
            XmlNodeList nodes = xd.SelectNodes(string.Format("/SysConfig/{0}", deviceType));
            if (nodes != null && nodes.Count > 0 )
            {
                foreach (XmlNode node in nodes)
                {
                    node.RemoveAll();
                }
            }
            StringBuilder sb = new StringBuilder();
            foreach(var model in list)
            {
                sb.Append(string.Format("<Device Model=\"{0}\" Name=\"{1}\" Selected=\"{2}\"></Device>", model.Model, model.Name, model.Selected == true ? 1:0));
            }
            nodes[0].InnerXml = sb.ToString();
            xd.Save(xmlFilePath);
        }


        public static List<DeviceModel> GetDevice(string deviceType)
        {
            List<DeviceModel> deviceList = new List<DeviceModel>();
            string path = GetPath(deviceType);
            if (string.IsNullOrEmpty(path))
            {
                return deviceList;
            }

            string xmlFilePath = string.Format("{0}//{1}", SysConfigBusiness.ApplicationStartPath, "SysConfig.xml");
            XmlDocument xd = new XmlDocument();
            xd.Load(xmlFilePath);
            XmlNodeList nodes = xd.SelectNodes(path);
            if (nodes == null || nodes.Count == 0)
            {
                return deviceList;
            }
            else
            {
                foreach (XmlNode node in nodes)
                {
                    DeviceModel deviceModel = new DeviceModel();
                    deviceModel.Model = node.Attributes["Model"].Value;
                    deviceModel.Name = node.Attributes["Name"] == null ? "": node.Attributes["Name"].Value;
                    deviceModel.Selected = node.Attributes["Selected"] == null ? false:(node.Attributes["Selected"].Value.Equals("1") ? true:false);
                    deviceList.Add(deviceModel);
                }
                return deviceList;
            }     
        }


        private static string GetPath(string devieType)
        {
            return string.Format("/SysConfig/{0}/Device", devieType);
        }
    }
}
