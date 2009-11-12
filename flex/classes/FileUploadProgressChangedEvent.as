package
{

	public class FileUploadProgressChangedEvent extends FileUploadEvent
	{
		// -------------------------------------------------------------------------
		// Fields
		// -------------------------------------------------------------------------
		private var _bytesUploaded:uint;

		// -------------------------------------------------------------------------
		// Constructor
		// -------------------------------------------------------------------------
		public function FileUploadProgressChangedEvent(sender:Object,bytesUploaded:uint,type:String,bubbles:Boolean = false,cancelable:Boolean = false)
		{
			super(sender, type, bubbles, cancelable);
			_bytesUploaded = bytesUploaded;
		}

		// -------------------------------------------------------------------------
		// Public Interface
		// -------------------------------------------------------------------------
		public function get BytesUploaded():Object
		{
			return _bytesUploaded;
		}
	}
}