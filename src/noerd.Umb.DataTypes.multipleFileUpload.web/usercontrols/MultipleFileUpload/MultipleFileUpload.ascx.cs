using System;
using System.Web;
using noerd.Umb.DataTypes.multipleFileUpload;
using umbraco;

public partial class usercontrols_MultipleFileUpload_MultipleFileUpload : MultipleFileUploadControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.ClientScript.RegisterClientScriptInclude("swfobject", VirtualPathUtility.AppendTrailingSlash(UmbracoSettings.ScriptFolderPath) + "swfobject.js");
    }
}


