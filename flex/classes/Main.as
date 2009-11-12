package 
{
	import flash.events.Event;
	import flash.events.HTTPStatusEvent;
	import flash.events.IOErrorEvent;
	import flash.events.MouseEvent;
	import flash.events.SecurityErrorEvent;
	import flash.net.FileReferenceList;
	import flash.net.URLRequest;
	import flash.net.navigateToURL;

	import mx.containers.HBox;
	import mx.containers.VBox;
	import mx.controls.Alert;
	import mx.controls.Button;
	import mx.controls.ProgressBar;
	import mx.controls.Text;
	import mx.core.Application;
	import mx.events.FlexEvent;	

	public class Main extends Application
	{
		// -------------------------------------------------------------------------
		// Fields
		// -------------------------------------------------------------------------
		public var fileContainer:VBox;
		public var fileUploadBox:VBox;
		public var uploadStats:HBox;
		public var totalFiles:Text;
		public var totalSize:Text;
		public var totalProgressBar:ProgressBar;
		public var browseButton:Button;
		public var clearButton:Button;
		public var uploadButton:Button;
		public var cancelButton:Button;
		public var mytext:Text;
		// -------------------------------------------------------------------------
		private var _fileRefList:FileReferenceList;
		private var _totalSize:Number;
		private var _uploadedBytes:Number;
		private var _currentUpload:FileUpload;

		// -------------------------------------------------------------------------
		// Constructor
		// -------------------------------------------------------------------------
		public function Main()
		{
			super();
			addEventListener(FlexEvent.CREATION_COMPLETE, OnLoad);
		}

		// -------------------------------------------------------------------------
		// Private Interface
		// -------------------------------------------------------------------------
		private function OnLoad(event:Event):void
		{
			_fileRefList = new FileReferenceList();
			_totalSize = 0;
			_uploadedBytes = 0;
			
			_fileRefList.addEventListener(Event.SELECT, OnSelect);
			
			browseButton.addEventListener(MouseEvent.CLICK, OnAddFilesClicked);
			clearButton.addEventListener(MouseEvent.CLICK, OnClearFilesClicked);
			uploadButton.addEventListener(MouseEvent.CLICK, OnUploadFilesClicked);
			cancelButton.addEventListener(MouseEvent.CLICK, OnCancelClicked);
			
			addEventListener(Event.ADDED, AddResizeListener);
		}

		// -------------------------------------------------------------------------
		private function OnAddFilesClicked(event:Event):void
		{
			//fileRefList.browse("*.jpg; *.jpeg; *.gif; *.png; *.zip");
			_fileRefList.browse();
		}

		// -------------------------------------------------------------------------
		private function OnClearFilesClicked(event:Event):void
		{			
			if(_currentUpload != null)
				_currentUpload.CancelUpload();

			fileUploadBox.removeAllChildren();

			SetLabels();

			_uploadedBytes = 0;
			_totalSize = 0;
			_currentUpload == null;
		}

		// -------------------------------------------------------------------------
		private function OnUploadFilesClicked(event:Event):void
		{
			var fileUploadArray:Array = fileUploadBox.getChildren();
			var fileUploading:Boolean = false;			
			_currentUpload = null;
			
			uploadButton.visible = false;
			cancelButton.visible = true;

			for(var x:uint = 0;x < fileUploadArray.length;x++)
			{
				if(!FileUpload(fileUploadArray[x]).IsUploaded)
				{
					fileUploading = true;
					_currentUpload = FileUpload(fileUploadArray[x]);
					_currentUpload.Upload();
					break;
				}
			}	

			if(!fileUploading)
			{
				OnCancelClicked(null);
				
				var completeFunction:String = Application.application.parameters.completeFunction;

				if(completeFunction != null && completeFunction != "")							
					navigateToURL(new URLRequest("javascript:" + completeFunction), "_self");
			}
		}

		// -------------------------------------------------------------------------
		private function OnCancelClicked(event:Event):void
		{
			if(_currentUpload != null)
			{
				_currentUpload.CancelUpload();
				_uploadedBytes -= _currentUpload.BytesUploaded;
				_currentUpload = null;					
			}

			SetLabels();
			
			uploadButton.visible = true;
			cancelButton.visible = false;
		}

		// -------------------------------------------------------------------------
		private function OnSelect(event:Event):void
		{
			var uploadPage:String = Application.application.parameters.uploadPage;			var nodeId:String = Application.application.parameters.nodeId;			var contextId:String = Application.application.parameters.contextId;

			for(var i:uint = 0;i < _fileRefList.fileList.length;i++)
			{
				var fu:FileUpload = new FileUpload(_fileRefList.fileList[i], uploadPage, nodeId, contextId);	
				fu.percentWidth = 100;				
				fu.addEventListener("FileRemoved", OnFileRemoved);	
				fu.addEventListener("UploadComplete", OnFileUploadComplete);
				fu.addEventListener("UploadProgressChanged", OnFileUploadProgressChanged);
				fu.addEventListener(HTTPStatusEvent.HTTP_STATUS, OnHttpError);
				fu.addEventListener(IOErrorEvent.IO_ERROR, OnIOError);
				fu.addEventListener(SecurityErrorEvent.SECURITY_ERROR, OnSecurityError);
				fileUploadBox.addChild(fu);					
			}

			SetLabels();
		}

		// -------------------------------------------------------------------------
		private function OnFileRemoved(event:FileUploadEvent):void
		{
			_uploadedBytes -= FileUpload(event.Sender).BytesUploaded;
			fileUploadBox.removeChild(FileUpload(event.Sender));				

			SetLabels();

			if(_currentUpload == FileUpload(event.Sender))
				OnUploadFilesClicked(null);
		}

		// -------------------------------------------------------------------------
		private function OnFileUploadComplete(event:FileUploadEvent):void
		{
			_currentUpload == null;
			OnUploadFilesClicked(null);
		}

		// -------------------------------------------------------------------------
		private function OnHttpError(event:HTTPStatusEvent):void
		{
			Alert.show("There has been an HTTP Error: status code " + event.status);
		}

		// -------------------------------------------------------------------------
		private function OnIOError(event:IOErrorEvent):void
		{
			Alert.show("There has been an I/O Error: " + event.text);
		}

		// -------------------------------------------------------------------------
		private function OnSecurityError(event:SecurityErrorEvent):void
		{
			Alert.show("There has been a Security Error: " + event.text);
		}

		// -------------------------------------------------------------------------
		private function OnFileUploadProgressChanged(event:FileUploadProgressChangedEvent):void
		{
			_uploadedBytes += Number(event.BytesUploaded);	
			
			SetProgressBar();
		}

		// -------------------------------------------------------------------------
		private function SetProgressBar():void
		{
			totalProgressBar.setProgress(_uploadedBytes, _totalSize);			
			totalProgressBar.label = "Uploaded " + FileUpload.FormatPercent(totalProgressBar.percentComplete) + "% - " + FileUpload.FormatSize(_uploadedBytes) + " of " + FileUpload.FormatSize(_totalSize);
		}

		// -------------------------------------------------------------------------
		private function SetLabels():void
		{
			var fileUploadArray:Array = fileUploadBox.getChildren();
		
			if(fileUploadArray.length > 0)
			{
				totalFiles.text = String(fileUploadArray.length);
				_totalSize = 0;
		
				for(var x:uint = 0;x < fileUploadArray.length;x++)
				{
					_totalSize += FileUpload(fileUploadArray[x]).FileSize;
				}
		
				totalSize.text = FileUpload.FormatSize(_totalSize);
		
				SetProgressBar();
		
				clearButton.visible = uploadButton.visible = totalProgressBar.visible = uploadStats.visible = true;					
			}
			else
			{
				clearButton.visible = uploadButton.visible = totalProgressBar.visible = uploadStats.visible = false;					
			}
		}

		// -------------------------------------------------------------------------
		private function AddResizeListener(evt:Event):void
		{
			stage.addEventListener(Event.RESIZE, OnResize);
			OnResize();
		}	

		// -------------------------------------------------------------------------
		private function OnResize(evt:Event = null):void
		{
			totalProgressBar.width = fileContainer.width - 50;
			totalProgressBar.validateNow();
		}		
	}
}