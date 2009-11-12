using System;
using System.Web;
using umbraco.BasePages;
using umbraco.BusinessLogic;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace noerd.Umb.DataTypes.multipleFileUpload
{
    /// <summary>
    /// A IHttpHandler that handles the Mulitple File Upload post requests.
    /// </summary>
    public class MultipleFileUploadHandler : IHttpHandler
    {
        // -------------------------------------------------------------------------
        // Public members
        // -------------------------------------------------------------------------

        // IHttpHandler Members
        // -------------------------------------------------------------------------

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        // -------------------------------------------------------------------------

        public void ProcessRequest(HttpContext context)
        {
            // Get queryStrings
            string nodeIds = context.Request.QueryString["nodeId"];
            string contextId = context.Request.QueryString["contextId"];

            // Perform security check
            if (!IsValidUmbracoUserContextID(contextId))
            {
                // Log failed authentication
                MultipleFileUpload.Log(LogTypes.LoginFailure, -1, "Security: The user has no umbraco contextid, no Media elements were created");

                return;
            }

            // Check queryStrings
            if (!string.IsNullOrEmpty(contextId) && !string.IsNullOrEmpty(nodeIds))
            {
                // Parse nodeId and check for files in post
                int nodeId;
                if (int.TryParse(nodeIds, out nodeId) && context.Request.Files.Count > 0)
                {
                    try
                    {
                        // Process uploaded files
                        MultipleFileUpload.HandleUpload(context, nodeId);
                        // log succes
                        MultipleFileUpload.Log(LogTypes.New, nodeId, "Succes");
                    }
                    catch (Exception e)
                    {
                        // log error
                        MultipleFileUpload.Log(LogTypes.Error, nodeId, e.ToString());
                    }
                }
                else
                {
                    // log error
                    MultipleFileUpload.Log(LogTypes.Error, -1, "Parent node id is in incorrect format");
                }
            }
            else
            {
                // log error
                MultipleFileUpload.Log(LogTypes.Error, -1, "Incorrect querystring");
            }

            // Used as a fix for a bug in mac flash player that makes the 
            // onComplete event not fire
            HttpContext.Current.Response.Write(" ");
        }

        // -------------------------------------------------------------------------
        // Private members
        // -------------------------------------------------------------------------

        private static bool IsValidUmbracoUserContextID(string umbracoUserContextID)
        {
            if (!String.IsNullOrEmpty(umbracoUserContextID))
            {
                return BasePage.GetUserId(umbracoUserContextID) != -1;
            }

            return false;
        }

        #endregion
    }
}