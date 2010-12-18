using System;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using umbraco.BasePages;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.datatype.controls;
using umbraco.cms.businesslogic.media;
using umbraco.DataLayer;

public partial class usercontrols_MultipleFileUpload_Installer : UserControl
{
    // -------------------------------------------------------------------------
    // Constants
    // -------------------------------------------------------------------------

    private const string DATATYPE_NAME = "Multiple File Upload";
    private static readonly Guid DATATYPE_UID = new Guid("ACCB9911-CD81-4B17-AF3F-446CEB1DBF0D");

    // -------------------------------------------------------------------------
    // Page events
    // -------------------------------------------------------------------------

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        pnlInstaller.Visible = true;
        pnlSkip.Visible = false;
        pnlSucces.Visible = false;
        pnlError.Visible = false;
        pnlUninstall.Visible = false;
    }

    // -------------------------------------------------------------------------
    // Button events
    // -------------------------------------------------------------------------

    protected void SkipClick(object sender, EventArgs e)
    {
        pnlInstaller.Visible = false;
        pnlSkip.Visible = true;
        pnlUninstall.Visible = true;
    }

    // -------------------------------------------------------------------------

    protected void Install(object sender, EventArgs e)
    {
        pnlInstaller.Visible = false;

        // Get MultipleFileUpload DataType
        DataTypeDefinition ddMultipleFileUpload = null;
        foreach (DataTypeDefinition dt in DataTypeDefinition.GetAll())
        {
            if (dt.DataType != null && dt.DataType.Id.Equals(DATATYPE_UID))
                ddMultipleFileUpload = dt;
        }

        if (ddMultipleFileUpload != null)
        {
            // Add tab to Folder Media type
            MediaType mediaType = MediaType.GetByAlias("Folder");
            ContentType.TabI uploadTab = null;
            foreach (ContentType.TabI tab in mediaType.getVirtualTabs)
            {
                if (tab.Caption.Equals("Upload"))
                    uploadTab = tab;
            }

            if (uploadTab == null)
            {
                int tabId = mediaType.AddVirtualTab("Upload");
                mediaType.AddPropertyType(ddMultipleFileUpload, "MultipleFileUpload", "Upload multiple files");
                mediaType.SetTabOnPropertyType(mediaType.getPropertyType("MultipleFileUpload"), tabId);
                mediaType.Save();
            }
        }

        pnlUninstall.Visible = true;
    }
}
