using AutomationLibrary;
using AventStack.ExtentReports;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViaoneFramework.PageModel;
using ViaoneFramework.Utils;

namespace ViaoneFramework.TestCases
{
    public class ClsTestCases
    {
        readonly ClsData objData = new ClsData();

        public string strTestCase;
        public bool blStop;
        public bool blStatus;
        public static bool blCookiesAccepted;
        public DateTime TCStartDate;
        public DateTime TCEndDate;
        public string TCExecutionTime;
        public bool blLogDeploy = false;


        [OneTimeSetUp]
        public void BeforeClass()
        {
            blStop = ClsReportResult.fnExtentSetup();
            if (!blStop)
                AfterClass();
        }

        public void SetupTest(string pstrTestCase)
        {
            ClsReportResult.objTest = ClsReportResult.objExtent.CreateTest(pstrTestCase);
            ClsWebBrowser.fnInitBrowser(TestContext.Parameters["GI_BrowserName"]);
        }

        [Test]
        [Category("Login Suite")]
        public void TC_VerifyTwoFactorAuthentication()
        { GetAndExecuteTestCase("Success Login"); }




        public void GetAndExecuteTestCase(string pstrTestName)
        {
            bool blTestFound = false;
            objData.fnLoadFile(ClsUtils.fnSetupDriverFile(), TestContext.Parameters["GI_DataDriverSheet"]);
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                if (blTestFound) { break; }
                TCStartDate = DateTime.Now;
                objData.CurrentRow = intRow;

                if (objData.fnGetValue("Description", "").Trim() == pstrTestName)
                {
                    //Get Suite Name
                    if (ClsReportResult.TestPlanSuite == "") { ClsReportResult.TestPlanSuite = objData.fnGetValue("Execution Set", ""); }

                    //Print Result on Pipeline
                    SetupTest($"{objData.fnGetValue("Execution Set", "")} - {pstrTestName}");
                    //Clean Constants
                    ClsReportResult.TC_Status = true;
                    ClsReportResult.isWarning = false;
                    blCookiesAccepted = false;
                    blStatus = true;
                    blTestFound = true;

                    //Get Functions to be executed
                    var arrFunctions = objData.fnGetValue("Funcions").Split(';').Where(act => !string.IsNullOrEmpty(act)).ToList();
                    string[] arrValue = objData.fnGetValue("Values").Split(';');
                    int intCount = -1;

                    // Using try to catch any unhandled exception at Test Level
                    ClsUtils.TryExecute(
                        () =>
                        {
                            ClsWebBrowser.fnNavigateToUrl(ClsUtils.fnGetURLEnv(TestContext.Parameters["GI_TestEnvironment"]));
                            ClsWebElements.fnWaitToLoadPage();

                            //Iterate and select function
                            foreach (var item in arrFunctions)
                            {
                                if (!blStatus) break; //TODO: stop using since we will start using AssertFail
                                intCount++;
                                var TempValue = "";
                                if (intCount < arrValue.Length && blStatus && arrValue[intCount] != "")
                                { TempValue = arrValue[intCount].Split('=')[1]; }
                                switch (item.ToUpper())
                                {
                                    case "LOGIN":
                                        blStatus = ClsLogin.fnLoginData(TempValue);
                                        break;
                                    default:
                                        ClsReportResult.fnLog("Data Driven Test", "The action: " + item.ToString() + " does not exist.", Status.Fail, false);
                                        blStatus = false;
                                        break;
                                }
                            }
                        },
                        (exception) =>
                        {
                            if (!exception.Message.Contains("Hardstop defined:"))
                            {
                                //Manage WebElements/Selenium Exceptions
                                var errorMessage = exception.Message.Contains("ERR_NAME_NOT_RESOLVED") ? "The URL cannot be opened as expected. Verify that environment is running up." : $"Unhandled Exception at Scenario '{strTestCase}'";
                                ClsWebElements.fnExceptionHandling(exception, errorMessage);
                            }
                        }
                    );


                    //Check Status
                    TCEndDate = DateTime.Now;
                    TimeSpan ExecTime = TCEndDate - TCStartDate;
                    if (ClsReportResult.TC_Status)
                    {
                        if (ClsReportResult.isWarning)
                        {
                            objData.fnSaveValue(ClsUtils.fnSetupDriverFile(), "TestCases", "Status", intRow, "Warning");
                            objData.fnSaveValue(ClsUtils.fnSetupDriverFile(), "TestCases", "ExecTime", intRow, ClsUtils.GetTCExecutionTimeFormat(ExecTime.ToString(@"hh\:mm\:ss")));
                            Assert.Warn("The scenario was executed with a warning flag");
                        }
                        else
                        {
                            objData.fnSaveValue(ClsUtils.fnSetupDriverFile(), "TestCases", "Status", intRow, "Pass");
                            objData.fnSaveValue(ClsUtils.fnSetupDriverFile(), "TestCases", "ExecTime", intRow, ClsUtils.GetTCExecutionTimeFormat(ExecTime.ToString(@"hh\:mm\:ss")));
                        }
                    }
                    else
                    {
                        objData.fnSaveValue(ClsUtils.fnSetupDriverFile(), "TestCases", "Status", intRow, "Fail");
                        objData.fnSaveValue(ClsUtils.fnSetupDriverFile(), "TestCases", "ExecTime", intRow, ClsUtils.GetTCExecutionTimeFormat(ExecTime.ToString(@"hh\:mm\:ss")));
                        CloseTest();
                        Assert.Fail("Hardstop defined:");
                    }

                    CloseTest();
                }
            }

            if (!blTestFound)
            {
                SetupTest(pstrTestName);
                ClsReportResult.fnLog("Data Driven Test", $"The Test Case: ({objData.fnGetValue("Execution Set", "")} - {pstrTestName}) was not found in the driver and cannot be executed", Status.Fail, false);
                blStatus = false;
                CloseTest();
            }
        }



        public void CloseTest()
        {
            ClsWebBrowser.fnCloseBrowser();
            ClsReportResult.fnExtentClose();
        }

        [OneTimeTearDown]
        public void AfterClass()
        {
            try
            {
                Trace.Flush();
                ClsReportResult.objExtent.Flush();
            }
            catch (Exception objException)
            {
                Console.WriteLine(objException.Message);
            }
        }



    }

}
