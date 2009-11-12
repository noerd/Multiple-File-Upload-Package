<%@ Import Namespace="umbraco"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MultipleFileUpload.ascx.cs" Inherits="usercontrols_MultipleFileUpload_MultipleFileUpload" %>

<div class="width:100%;height:400px;" id="multipleFileUpload"></div>

<script type="text/javascript">
    /* <![CDATA[ */
    var uploadUI = new SWFObject("/usercontrols/MultipleFileUpload/MultipleFileUpload.swf", "multipleFileUploadFlash", "100%", "400", 9, "#FFFFFF");
    uploadUI.addVariable("uploadPage", "<%= MultipleFileUploadHandlerURL %>");
    uploadUI.addVariable("nodeId", "<%= NodeId %>");    
    uploadUI.addVariable("contextId", "<%= UmbracoUserContextID %>");
    uploadUI.addVariable("completeFunction", "parent.top.syncTree('<%= Path %>');");
    uploadUI.write("multipleFileUpload");
    /* ]]> */
</script>

<%--
<!-- If you plan to develop your own IMediaFactory implementations you will
get better error feedback by uncommenting the FileUpload WebControl beneath. -->
<asp:FileUpload ID="fileUpload" runat="server"/>
--%>
