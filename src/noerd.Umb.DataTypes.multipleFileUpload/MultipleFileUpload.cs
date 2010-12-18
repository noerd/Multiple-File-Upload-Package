using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Schema;
using umbraco;
using umbraco.BasePages;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.media;
using umbraco.interfaces;

namespace noerd.Umb.DataTypes.multipleFileUpload
{
    /// <summary>
    /// Umbraco IDataEditor: Multiple file upload
    /// </summary>
    public class MultipleFileUpload : HtmlContainerControl, IDataEditor
    {
        // -------------------------------------------------------------------------
        // Constants
        // -------------------------------------------------------------------------

        private const string SCHEMA_NAMESPACE = "urn:MultipleFileUpload-schema";

        // -------------------------------------------------------------------------
        // Fields
        // -------------------------------------------------------------------------

        private static XmlDocument _multipleFileUploadXml;

        private readonly DefaultData _data;
        private readonly string _usercontrolPath;
        private MultipleFileUploadControl _userControl;

        // -------------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------------

        public MultipleFileUpload(IData data, string usercontrolPath)
        {
            // Get usercontrol path
            if (!String.IsNullOrEmpty(usercontrolPath))
                _usercontrolPath = usercontrolPath;
            else
                // Use default usercontrol
                _usercontrolPath = "~/usercontrols/MultipleFileUpload/MultipleFileUpload.ascx";

            _data = (DefaultData)data;

            // Get multiplefileupload config
            string configFilePath =
                HttpContext.Current.Server.MapPath(VirtualPathUtility.ToAbsolute("~/config/multiplefileupload.config"));

            if (File.Exists(configFilePath))
            {
                _multipleFileUploadXml = new XmlDocument();
                XmlReader xmlReader = null;
                try
                {
                    // Read configfile with schema
                    // Get schema from assembly
                    Stream schemaStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("noerd.multipleFileUpload.MultipleFileUpload.xsd");

                    // XmlReader settings
                    XmlReaderSettings settings = new XmlReaderSettings();
                    if (schemaStream != null)
                    {
                        settings.ValidationType = ValidationType.Schema;
                        settings.Schemas.Add(SCHEMA_NAMESPACE, XmlReader.Create(schemaStream));
                        settings.ValidationEventHandler += ConfigValidationEventHandler;
                    }

                    // Create XmlReader
                    xmlReader = XmlReader.Create(configFilePath, settings);

                    // Read xml
                    _multipleFileUploadXml.Load(xmlReader);
                }
                catch (XmlException)
                {
                    if (xmlReader != null)
                        xmlReader.Close();

                    Log(LogTypes.Error, -1, "multiplefileupload.config invalid XML");
                }
            }
            else
            {
                Log(LogTypes.Error, -1, "multiplefileupload.config not found");
            }
        }

        // -------------------------------------------------------------------------

        public static void Log(LogTypes logType, int nodeId, string message)
        {
            umbraco.BusinessLogic.Log.Add(logType, new User(0), nodeId, "Multiple file upload: " + message);

            if (logType.Equals(LogTypes.Error) || logType.Equals(LogTypes.LoginFailure))
                throw new ApplicationException(message);
        }

        // -------------------------------------------------------------------------
        // Page events
        // -------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Load the specified user control handling the gui rendering
            _userControl = (MultipleFileUploadControl)Page.LoadControl(_usercontrolPath);
            Controls.Add(_userControl);
        }

        // -------------------------------------------------------------------------

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Get content version
            Content c = Content.GetContentFromVersion(_data.Version);

            // Set properties on user control
            _userControl.NodeId = c.Id;
            _userControl.Path = c.Path;
            _userControl.UmbracoUserContextID = BasePage.umbracoUserContextID;
        }

        // -------------------------------------------------------------------------
        // Eventhandler
        // -------------------------------------------------------------------------

        private static void ConfigValidationEventHandler(object sender, ValidationEventArgs e)
        {
            XmlReader xmlReader = (XmlReader)sender;
            xmlReader.Close();

            Log(LogTypes.Error, -1, "Invalid xml format in multiplefileupload.config: " + e.Message);
        }

        // -------------------------------------------------------------------------
        // Public members
        // -------------------------------------------------------------------------

        public static void HandleUpload(HttpContext context, int nodeId)
        {
            context.Response.Write(context.Request.Files.Count);

            // loop through uploaded files
            for (int j = 0; j < context.Request.Files.Count; j++)
            {
                // get parent node
                Media parentNode = new Media(nodeId);

                // get the current file
                HttpPostedFile uploadFile = context.Request.Files[j];

                // if there was a file uploded
                if (uploadFile.ContentLength > 0)
                {
                    // Get concrete MediaFactory
                    IMediaFactory factory = GetMediaFactory(uploadFile);
                    // Create media Item
                    Media media = factory.CreateMedia(parentNode, uploadFile);

                    // Get path
                    int propertyId = media.getProperty("umbracoFile").Id;
                    string path = HttpContext.Current.Server.MapPath(factory.ConstructRelativeDestPath(propertyId));

                    // Create directory
                    if (UmbracoSettings.UploadAllowDirectories)
                        Directory.CreateDirectory(path);

                    // Save file
                    string filePath = Path.Combine(path, uploadFile.FileName);
                    uploadFile.SaveAs(filePath);

                    // Close stream
                    uploadFile.InputStream.Close();

                    // Save media
                    media.Save();
                    // Genereate xml cache
                    media.XmlGenerate(new XmlDocument());
                }
            }
        }

        // IDataEditor Members
        // -------------------------------------------------------------------------

        #region IDataEditor Members

        public void Save()
        {
            // Get content version
            Content c = Content.GetContentFromVersion(_data.Version);
            
            // Process Upload
            HandleUpload(Context, c.Id);
        }

        // -------------------------------------------------------------------------

        public bool ShowLabel
        {
            get { return false; }
        }

        // -------------------------------------------------------------------------

        public bool TreatAsRichTextEditor
        {
            get { return true; }
        }

        // -------------------------------------------------------------------------

        public Control Editor
        {
            get { return this; }
        }

        #endregion

        // -------------------------------------------------------------------------
        // Private members
        // -------------------------------------------------------------------------

        private static IMediaFactory GetMediaFactory(HttpPostedFile uploadFile)
        {
            // Get extension
            string ext = uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf(".") + 1).ToLower();

            if (_multipleFileUploadXml != null)
            {
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(_multipleFileUploadXml.NameTable);
                nsmgr.AddNamespace("mfu", SCHEMA_NAMESPACE);

                // Fetch mediaFactory based on extension
                XmlNode factoryNode =
                    _multipleFileUploadXml.SelectSingleNode("/mfu:multipleFileUpload/mfu:mediaFacory [mfu:extensions/mfu:ext/text() = '" + ext + "']", nsmgr);

                // If no factory select default factory
                factoryNode = (factoryNode ??
                               _multipleFileUploadXml.SelectSingleNode(
                                   "/mfu:multipleFileUpload/mfu:mediaFacory [mfu:extensions/mfu:ext/text()='*'] [1]", nsmgr));

                if (factoryNode != null)
                {
                    // Create appropriate IMediaFactory instance
                    string assemblyName = factoryNode.SelectSingleNode("@assembly", nsmgr).Value;
                    string typeName = factoryNode.SelectSingleNode("@type", nsmgr).Value;

                    // Build full qualified type string
                    string fullQualifiedType = "";

                    XmlNode namespaceNode = factoryNode.SelectSingleNode("@namespace", nsmgr);
                    if (namespaceNode != null)
                        fullQualifiedType = namespaceNode.Value + ".";

                    fullQualifiedType += typeName + "," + assemblyName;

                    // Create type from full qualified string
                    Type type = Type.GetType(fullQualifiedType);
                    // Create instance type
                    return Activator.CreateInstance(type) as IMediaFactory;
                }
            }

            // No config
            // No matching extension
            // No default media factory configured

            // Use DefaultFileMediaFactory
            return new DefaultFileMediaFactory();
        }
    }
}
