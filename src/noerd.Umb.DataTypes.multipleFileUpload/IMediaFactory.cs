using System.Web;
using umbraco.BusinessLogic.console;
using umbraco.cms.businesslogic.media;

namespace noerd.Umb.DataTypes.multipleFileUpload
{
    /// <summary>
    /// An interface encapsulating a factory responsible for instantiating Umbraco Media objects.
    /// 
    /// Inherit this interface inorder to create your own custom Media Factories, then use ~/config/MultipleFileUpload.config
    /// to configure when the factory should be used based on extension.
    /// </summary>
    public interface IMediaFactory
    {
        /// <summary>
        /// Creates a new concrete Umbraco Media object.
        /// </summary>
        /// <param name="parent">The new media objects parent node.</param>
        /// <param name="uploadFile">The HttpPostedFile object posted from the Multiple File Upload user control.</param>
        /// <returns>The new media object</returns>
        Media CreateMedia(IconI parent, HttpPostedFile uploadFile);

        /// <summary>
        /// Creates a path to the directory where the uploaded file will be placed, based on the umbracoFile property id.
        /// A uploaded file in umbraco is by default contained in a folder named with the umbracoFile property id. 
        /// If the UmbracoSettings.UploadAllowDirectories is set to false, the method returns the path to the media folder.
        /// </summary>
        /// <param name="propertyId">The umbracoFile property id</param>
        /// <returns>The path to the folder where the uploaded file will be placed.</returns>
        string ConstructRelativeDestPath(int propertyId);

		/// <summary>
		/// Function for handling illegal characters in the file name.
		/// </summary>
		/// <param name="url">The uploaded file name.</param>
		/// <returns>The safe file name that the uploaded file will be saved with.</returns>
		string SafeFileName(string fileName); 
    }
}
