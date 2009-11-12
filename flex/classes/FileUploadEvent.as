package 
{
	import flash.events.Event;		

	public class FileUploadEvent extends Event
	{
		// -------------------------------------------------------------------------
		// Fields
		// -------------------------------------------------------------------------
		private var _sender:Object;

		// -------------------------------------------------------------------------
		// Constructor
		// -------------------------------------------------------------------------
		public function FileUploadEvent(sender:Object, type:String, bubbles:Boolean = false, cancelable:Boolean = false)
		{
			super(type, bubbles, cancelable);
			_sender = sender;
		}

		// -------------------------------------------------------------------------
		// Public Interface
		// -------------------------------------------------------------------------
		public function get Sender():Object
		{
			return _sender;
		}
	}
}