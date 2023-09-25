using AutomationLibrary;
using AventStack.ExtentReports;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViaoneFramework.PageLogic;
using ViaoneFramework.Utils;

namespace ViaoneFramework.PageModel
{
    public static class ClsLogin
    {

        public static bool fnLoginData(string pstrSetNo)
        {
            bool blResult = true;
            ClsData objData = new ClsData();
            objData.fnLoadFile(ClsUtils.fnSetupDriverFile(), "LogInData");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    string strMessage;
                    bool blCondition;

                    //Accept Cookies
                    ClsWebElements.fnWaitToLoadPage();
                    fnEnterCredentails(objData.fnGetValue("User", ""), objData.fnGetValue("Password", ""));
                    //Verify if login is successfully.
                    blCondition = ClsWebElements.fnWaitUntilElementVisible(LoginModel.strWelcomeLogin, TimeSpan.FromSeconds(30));
                    strMessage = blCondition ? "was done successfully" : "was not compleated as expected.";
                    ClsReportResult.fnLog("", $"The Viaone Login {strMessage}", blCondition ? Status.Pass : Status.Fail, true);
                    //Verify if alert is displayed
                    if (ClsWebElements.fnWaitUntilElementVisible(LoginModel.strMultipleSession, TimeSpan.FromSeconds(8)))
                    {
                        ClsReportResult.fnLog("", $"The multiple sessions modal is displayed.", Status.Info, true);
                        ClsWebElements.fnClick(ClsWebElements.fnGetWebElement(LoginModel.strMultipleSessionOK), "OK Button", false);
                        ClsWebElements.fnWaitUntilElementHidden(LoginModel.strMultipleSession, TimeSpan.FromSeconds(8));
                    }
                }
            }
            return blResult;
        }

        private static void fnEnterCredentails(string strUser, string strValue)
        {
            ClsWebElements.fnWaitUntilElementVisible(LoginModel.strUserID, TimeSpan.FromSeconds(10));
            ClsWebElements.fnWaitUntilElementVisible(LoginModel.strPasswordID, TimeSpan.FromSeconds(10));
            ClsWebElements.fnCustomSendKeys(ClsWebElements.fnGetWebElement(LoginModel.strUserID), "UserID", strUser, false);
            ClsWebElements.fnCustomSendKeys(ClsWebElements.fnGetWebElement(LoginModel.strPasswordID), "Password", strValue, false);
            ClsWebElements.fnWaitUntilElementClickable(LoginModel.strLoginButton, TimeSpan.FromSeconds(10));
            ClsWebElements.fnClick(ClsWebElements.fnGetWebElement(LoginModel.strLoginButton), "Login Button", false);
            ClsWebElements.fnWaitToLoadPage();
        }




    }
}
