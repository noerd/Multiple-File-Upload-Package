using System.Web;
using System.Web.UI;

namespace noerd.Umb.DataTypes.multipleFileUpload
{
    /// <summary>
    /// A superclass for the the Multiple File Upload user control.
    /// Responsible for recieving information about current media folder node, user context id and node id path 
    /// from the Multiple File Upload IDataTypeEditor.
    /// 
    /// A custom Multiple File Upload user control must inherit this class inorder to be instatiated by the 
    /// Multiple File Upload IDataTypeEditor. The information must be passed on as querystrings to the 
    /// MultipleFileUploadHandler. Use the FullMultipleFileUploadHandlerURL getter to get the full URL inclusive 
    /// required querystring variables.
    /// </summary>
    public abstract class MultipleFileUploadControl : UserControl
    {
        // -------------------------------------------------------------------------
        // Fields
        // -------------------------------------------------------------------------

        private int _nodeId = -1;
        private string _umbracoUserContextId = "";
        private string _path = "";

        // -------------------------------------------------------------------------
        // Public members
        // -------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the id of the media folder node, where the uploaded files should be placed
        /// </summary>
        public int NodeId
        {
            get { return _nodeId; }
            set { _nodeId = value; }
        }

        // -------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the umbraco user context id
        /// </summary>
        public string UmbracoUserContextID
        {
            get { return _umbracoUserContextId; }
            set { _umbracoUserContextId = value; }
        }

        // -------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the path of nodes id from the root to the folder. Used for updating the tree view in umbraco GUI
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        // -------------------------------------------------------------------------

        /// <summary>
        /// Gets the full url to post the file to including the required querystring variables.
        /// </summary>
        public string FullMultipleFileUploadHandlerURL
        {
            get
            {
                return string.Format(VirtualPathUtility.ToAbsolute("~/MultipleFileUploadHandler.axd") + "?nodeId={0}&contextId={1}", NodeId, UmbracoUserContextID);
            }
            
        }

        // -------------------------------------------------------------------------

        /// <summary>
        /// Gets the url to post the file.
        /// </summary>
        public string MultipleFileUploadHandlerURL
        {
            get
            {
                return VirtualPathUtility.ToAbsolute(string.Format("~/MultipleFileUploadHandler.axd"));
            }
        }
    }
}
