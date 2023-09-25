using AutomationLibrary;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaoneFramework.Utils
{
    public static class ClsUtils
    {

        public static string fnGetDriverDirectory()
        {
            string strFullDriverDir = "";
            var strDriverPath = (ClsUtils.fnSetupDriverFile()).Split('\\');
            for (int intFolder = 0; intFolder < strDriverPath.Length - 1; intFolder++)
            {
                strFullDriverDir = strFullDriverDir + strDriverPath[intFolder] + @"\";
            }
            return strFullDriverDir;
        }


        public static string fnSetupDriverFile()
        {
            var newPath = TestContext.Parameters["GI_DataDriverPath"] + TestContext.Parameters["GI_DataDriverName"];
            if (!File.Exists(newPath)) { Assert.Fail($"Hardstop defined: The file \"{newPath}\" not exist or was not found in the path."); }
            return newPath;
        }



        /// <summary>
        /// Function to return true if no exception is throwed when executing
        /// </summary>
        /// <param name="action">Function to execute</param>
        /// <returns>True if no exceptions thrown</returns>
        public static bool TryExecute(this Action action)
        {
            try
            {
                action.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Function to manage exceptions throwed when executing
        /// </summary>
        /// <param name="action">Function to execute</param>
        /// <param name="customExceptionHandler">
        /// Optional - Custom function to manage exceptions
        /// If this parameter is not provided default handling will be executed
        /// </param>
        public static void TryExecute(this Action action, Action<Exception> customExceptionHandler)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                //Stack Trace
                ClsVariables.fnAddStackTrace(ex.Message);
                customExceptionHandler.Invoke(ex);
                ClsReportResult.TC_Status = false;
            }
        }

        /// <summary>
        /// Returns url for environment specified
        /// </summary>
        /// <param name="pstrEnv"></param>
        /// <returns></returns>
        public static string fnGetURLEnv(string pstrEnv)
        {
            string URL = "";
            switch (pstrEnv.ToUpper())
            {
                case "VIAONEQA":
                    URL = ConfigurationManager.AppSettings["ViaoneQA"];
                    break;
                case "VIAONEUAT":
                    URL = ConfigurationManager.AppSettings["ViaoneUAT"];
                    break;
                default:
                    URL = "";
                    break;
            }
            return URL;
        }


        /// <summary>
        /// Retunrs the a date on hh mm ss, example: 9 hrs, 15 min, 01 seg
        /// </summary>
        /// <param name="pDateTime"></param>
        /// <returns></returns>
        public static string GetTCExecutionTimeFormat(string pDateTime)
        {
            var strTimeSplit = pDateTime.Split(':');
            var strMin = strTimeSplit[1].StartsWith("0") ? strTimeSplit[1].Remove(0, 1) : strTimeSplit[1];
            string strtime = strTimeSplit[0] != "00" ? $"{strTimeSplit[0]} hrs, {strMin} min, {strTimeSplit[2]} seg" : $"{strMin} min, {strTimeSplit[2]} seg";
            return strtime;
        }




    }
}
