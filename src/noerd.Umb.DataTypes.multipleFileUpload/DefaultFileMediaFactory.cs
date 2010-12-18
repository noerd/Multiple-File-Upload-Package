using System.Web;
using umbraco.BasePages;
using umbraco.BusinessLogic;
using umbraco.BusinessLogic.console;
using umbraco.cms.businesslogic.media;

namespace noerd.Umb.DataTypes.multipleFileUpload
{
    /// <summary>
    /// /// The default Media factory for files.
    /// </summary>
    public class DefaultFileMediaFactory : MediaFactory
    {
        // IMediaFactory Members
        // -------------------------------------------------------------------------

        #region IMediaFactory Members

        public override Media CreateMedia(IconI parent, HttpPostedFile uploadFile)
        {
            string filename = uploadFile.FileName;

            // Create new media object
            Media media = Media.MakeNew(filename, MediaType.GetByAlias("File"),
                                      new User(0), parent.Id);

            // Get umbracoFile property
            int propertyId = media.getProperty("umbracoFile").Id;

            // Set media properties
            media.getProperty("umbracoFile").Value = VirtualPathUtility.Combine(ConstructRelativeDestPath(propertyId), filename);
            media.getProperty("umbracoBytes").Value = uploadFile.ContentLength;
            media.getProperty("umbracoExtension").Value = VirtualPathUtility.GetExtension(filename).Substring(1);

            return media;
        }

        #endregion
    }
}
