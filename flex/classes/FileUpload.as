package 
{
	import flash.events.Event;
	import flash.events.HTTPStatusEvent;
	import flash.events.IOErrorEvent;
	import flash.events.MouseEvent;
	import flash.events.ProgressEvent;
	import flash.events.SecurityErrorEvent;
	import flash.net.FileReference;
	import flash.net.URLRequest;

	import mx.containers.HBox;
	import mx.containers.VBox;
	import mx.controls.Button;
	import mx.controls.ProgressBar;
	import mx.controls.ProgressBarMode;
	import mx.controls.Text;
	import mx.core.ScrollPolicy;	

	public class FileUpload extends VBox
	{
		// -------------------------------------------------------------------------
		// Fields
		// -------------------------------------------------------------------------
		private var _bar:ProgressBar;
		private var _file:FileReference;
		private var _nameText:Text;
		private var _uploaded:Boolean;
		private var _uploading:Boolean;
		private var _bytesUploaded:uint;
		private var _uploadUrl:String;		private var _nodeId:String;		private var _contextId:String;		private var _button:Button;

		// -------------------------------------------------------------------------
		// Constructor
		// -------------------------------------------------------------------------
		public function FileUpload(file:FileReference, uploadUrl:String, nodeId:String, contextId:String)
		{
			super();

			_file = file;
			_uploadUrl = uploadUrl;
			_nodeId = nodeId;
			_contextId = contextId;
			
			_uploaded = false;	
			_uploading = false;
			_bytesUploaded = 0;
			
			setStyle("backgroundColor", "#eeeeee");
			setStyle("paddingBottom", "10");
			setStyle("paddingTop", "10");
			setStyle("paddingLeft", "10");
			verticalScrollPolicy = ScrollPolicy.OFF;
			
			_file.addEventListener(Event.COMPLETE, OnUploadComplete);
			_file.addEventListener(ProgressEvent.PROGRESS, OnUploadProgressChanged);
			_file.addEventListener(HTTPStatusEvent.HTTP_STATUS, OnHttpError);
			_file.addEventListener(IOErrorEvent.IO_ERROR, OnIOError);
			_file.addEventListener(SecurityErrorEvent.SECURITY_ERROR, OnSecurityError);
			
			var hbox:HBox = new HBox();
			
			_nameText = new Text();
			_nameText.text = _file.name + " - " + FormatSize(_file.size);
			
			addChild(_nameText);
						
			_bar = new ProgressBar();
			_bar.mode = ProgressBarMode.MANUAL;
			_bar.label = "Uploaded 0%";
			_bar.width = 275;			
			hbox.addChild(_bar);
			
			_button = new Button();
			_button.label = "Remove";
			hbox.addChild(_button);			
			
			_button.addEventListener(MouseEvent.CLICK, OnRemoveButtonClicked);
			addChild(hbox);
			
			addEventListener(Event.ADDED_TO_STAGE, AddResizeListener);			addEventListener(Event.ADDED, OnAdded);
		}

		// -------------------------------------------------------------------------
		// Public Interface
		// -------------------------------------------------------------------------
		public function get IsUploading():Boolean
		{
			return _uploading;
		}

		// -------------------------------------------------------------------------
		public function get IsUploaded():Boolean
		{
			return _uploaded;
		}

		// -------------------------------------------------------------------------
		public function get BytesUploaded():uint
		{
			return _bytesUploaded;
		}

		// -------------------------------------------------------------------------
		public function get UploadUrl():String
		{
			return _uploadUrl;
		}

		// -------------------------------------------------------------------------
		public function set UploadUrl(uploadUrl:String):void
		{
			_uploadUrl = uploadUrl;
		}

		// -------------------------------------------------------------------------
		public function get FileSize():uint
		{
			var size:uint = 0;
			try
			{
				size = _file.size;
			}
			catch (err:Error) 
			{
				size = 0;
			}
			return size;
		}

		// -------------------------------------------------------------------------
		public function Upload():void
		{
			_uploading = true;
			_bytesUploaded = 0;
			
			var urlRequest:URLRequest = new URLRequest();
			urlRequest.url = _uploadUrl + "?nodeId=" + _nodeId + "&contextId=" + _contextId;
			
			_file.upload(urlRequest);
		}

		// -------------------------------------------------------------------------
		public function CancelUpload():void
		{
			_uploading = false;
			_file.cancel();
		}

		// -------------------------------------------------------------------------
		public static function FormatSize(size:uint):String
		{
			if(size < 1024)
		        return PadSize(int(size * 100) / 100) + " bytes";
			if(size < 1048576)
		        return PadSize(int((size / 1024) * 100) / 100) + "KB";
			if(size < 1073741824)
		       return PadSize(int((size / 1048576) * 100) / 100) + "MB";
			
			return PadSize(int((size / 1073741824) * 100) / 100) + "GB";
		}

		// -------------------------------------------------------------------------
		public static function FormatPercent(percent:Number):String
		{
			percent = int(percent);
			return String(percent);
		}

		// -------------------------------------------------------------------------
		public static function PadSize(size:Number):String
		{
			var temp:String = String(size);
			var index:int = temp.lastIndexOf(".");
			
			if(index == -1)
				return temp + ".00";
			else if(index == temp.length - 2)
				return temp + "0";
			else
				return temp;
		}

		// -------------------------------------------------------------------------
		// Private Interface
		// -------------------------------------------------------------------------
		private function OnRemoveButtonClicked(event:Event):void
		{
			if(_uploading)
				_file.cancel();
			
			dispatchEvent(new FileUploadEvent(this, "FileRemoved"));
		}

		// -------------------------------------------------------------------------
		private function OnUploadComplete(event:Event):void
		{
			_uploading = false;
			_uploaded = true;
			
			dispatchEvent(new FileUploadEvent(this, "UploadComplete"));
		}

		// -------------------------------------------------------------------------
		private function OnHttpError(event:HTTPStatusEvent):void
		{
			dispatchEvent(event);
		}

		// -------------------------------------------------------------------------
		private function OnIOError(event:IOErrorEvent):void
		{
			dispatchEvent(event);
		}

		// -------------------------------------------------------------------------
		private function OnSecurityError(event:SecurityErrorEvent):void
		{
			dispatchEvent(event);
		}

		// -------------------------------------------------------------------------
		private function OnUploadProgressChanged(event:ProgressEvent):void
		{
			var bytesUploaded:uint = event.bytesLoaded - _bytesUploaded;
			
			_bytesUploaded = event.bytesLoaded;
			_bar.setProgress(event.bytesLoaded, event.bytesTotal);
			_bar.label = "Uploaded " + FormatPercent(_bar.percentComplete) + "%";
			
			dispatchEvent(new FileUploadProgressChangedEvent(this, bytesUploaded, "UploadProgressChanged"));			
		}

		// -------------------------------------------------------------------------
		private function OnAdded(evt:Event):void
		{
			OnResize();
		}

		// -------------------------------------------------------------------------
		private function AddResizeListener(evt:Event):void
		{
			stage.addEventListener(Event.RESIZE, OnResize);
		}	

		// -------------------------------------------------------------------------
		private function OnResize(evt:Event = null):void
		{
			_bar.width = width - 120;
		}	
	}
}