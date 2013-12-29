using umbraco;
using umbraco.cms.businesslogic.media;
using umbraco.BusinessLogic.console;
using System.Web;
using System;
using System.Text.RegularExpressions;

namespace noerd.Umb.DataTypes.multipleFileUpload
{
    /// <summary>
    /// An abstract implementation of ImediaFactory. 
    /// Responsible for the implementation of common functionality in the ConstructRelativeDestPath() method.
    /// </summary>
    public abstract class MediaFactory : IMediaFactory
    {
        // -------------------------------------------------------------------------
        // Constants
        // -------------------------------------------------------------------------

        private const string MEDIA_PATH = "~/media/";

        // -------------------------------------------------------------------------
        // Public members
        // -------------------------------------------------------------------------

        public abstract Media CreateMedia(IconI parent, HttpPostedFile uploadFile);

        // -------------------------------------------------------------------------

        public string ConstructRelativeDestPath(int propertyId)
        {
            if (UmbracoSettings.UploadAllowDirectories)
            {
                string path = VirtualPathUtility.Combine(MEDIA_PATH, propertyId.ToString());
                path = VirtualPathUtility.AppendTrailingSlash(path);

                return VirtualPathUtility.ToAbsolute(path);
            }

            return VirtualPathUtility.ToAbsolute(MEDIA_PATH);
        }

		public string SafeFileName(string fileName)
		{
			if (!String.IsNullOrEmpty(fileName))
				return Regex.Replace(fileName, @"[^a-zA-Z0-9\-\.\/\:]{1}", "_");
			else
				return String.Empty;
		}
    }
}
