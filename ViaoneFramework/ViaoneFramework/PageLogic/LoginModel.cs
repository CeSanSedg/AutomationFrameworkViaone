using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaoneFramework.PageLogic
{
    public static class LoginModel
    {

        //LOGIN TEST CASE XPATHS
        public static string strUserID = "//input[contains(@id,\"PlaceHolder_UserID\")]";
        public static string strPasswordID = "//input[contains(@id,\"PlaceHolder_password\")]";
        public static string strLoginButton = "//input[contains(@id,\"PlaceHolder_SubmitLogin\")]";
        public static string strWelcomeLogin = "//div[contains(text(), 'Welcome')]";
        public static string strMultipleSession = "//div[@id='session-multiple-modal' and contains(@style, 'display: block')]";
        public static string strMultipleSessionOK = $"{strMultipleSession}//input[@value='OK']";

        // SEARCH TEST CASE XPATHS
        public static string strSearchImage = "//a[contains(@id,\"SubMenuContentArea_btnSearch\")]";
        public static string strSearchCriteria = "//*[@id='txtSearchBox']";
        public static string strSearchButton = "//input[contains(@id,\"PageContentArea_Search\")]";
        public static string strSearchResults = "//div[contains(@id,\"claimList_DetailContainer\")]//table//tbody//tr[1]//td//table//tbody//tr//td[4]";




    }
}
