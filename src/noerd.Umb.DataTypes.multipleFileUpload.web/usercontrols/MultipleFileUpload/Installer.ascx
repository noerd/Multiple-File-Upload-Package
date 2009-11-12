<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Installer.ascx.cs" Inherits="usercontrols_MultipleFileUpload_Installer" %>

<asp:Panel ID="pnlInstaller" runat="server">
    <h1>Multiple File Upload custom installer</h1>
    <p>
        This installer will
    </p>
    <ul>
        <li>Create a Multiple File Upload datatype in the developer section.</li>
        <li>Add an "Upload" tab to the Folder media type and add a property with the Multiple File Upload datatype.</li>
        <li>Register the MultipleFileUploadHandler (IHttpHandler) in the web.config file.</li>
    </ul>
    <p>
        To start the install process press the <b>Install</b> button.
    </p>
    <p>
        If you wish to perform the steps manually, press the <b>Skip</b> button and a detailed installation description will be shown.
    </p>
    <p>
        <em>Warning:</em> If you wish to uninstall the package be sure to follow the instructions in the documentation and displayed on the last page of the installer, otherwise you could end up invalidating all folders in the media section.
    </p>
    <p>
        <em>Advanced:</em> There are various extension points in the Multiple File Upload package. Please refer to the documentation included with the sourcecode for details on how to extend the functionality.
    </p>
    <p>
        The documentation can be found here <a target="_blank" href="/usercontrols/MultipleFileUpload/MultipleFileUpload.pdf">~/usercontrols/MultipleFileUpload/MultipleFileUpload.pdf</a>
    </p>
    <p>
        <asp:Button ID="btnSkip" Text="Skip" runat="server" OnClick="SkipClick" />
        <asp:Button ID="btnInstall" Text="Install" runat="server" OnClick="Install" />
    </p>
</asp:Panel>

<asp:Panel ID="pnlSkip" runat="server" Visible="false">
    <h1>Multiple File Upload Manual install</h1>
    <h3>1) Create a Multiple File Upload datatype in the developer section.</h3>
    <ol>
        <li>Go to the developer section, rightclick the <b>Data Types</b> node and select <b>Create</b></li>
        <li>Give the Data Type a name like "Multiple File Upload" and press <b>Create</b> button</li>
        <li>Click on the new Data type. In Rendercontrol drop down select <b>Multiple File Upload</b> and press the save button.</li>
    </ol>
    <p>
        <em>Advanced:</em> The Prevalue field, can be used to specify the path to a custom usercontrol as frontend for the Multiple File Upload package.
    </p>
    <h3>2) Add an "Upload" tab to the Folder media type and add a property with the Multiple File Upload datatype.</h3>
    <ol>
        <li>Go to the settings section and expand the <b>Media Types</b> folder.</li>
        <li>Select the <b>Folder</b> node and press the <b>Tabs</b> tab.</li>
        <li>In the <b>New tab</b> field write a name like "Upload" and pres the <b>New tab</b> button.</li>
        <li>Select the <b>Generic properties</b> tab and create a new property. Give it whatever name you like, put MultipleFileUpload in the <b>Alias</b> field, select the data type created in step 1 in the <b>Type</b> field and select the tab you just created in teh <b>Tab</b> field.</li>
        <li>Press the save  button.</li>
    </ol>
    <h3>3) Register the MultipleFileUploadHandler (IHttpHandler) in the web.config file.</h3>
    <ol>
        <li>Add the following line to the system.web/httpHandlers section in the web.config file.<br /></li>
    </ol>
    <pre>
        &lt;add path="MultipleFileUploadHandler.axd" verb="POST" type="noerd.Umb.DataTypes.multipleFileUpload.MultipleFileUploadHandler, noerd.Umb.DataTypes.multipleFileUpload" /&gt;
    </pre>
    <br />
</asp:Panel>

<asp:Panel ID="pnlSucces" runat="server" Visible="false">
    <h1>Multiple File Upload was succesfully installed</h1>
</asp:Panel>

<asp:Panel ID="pnlError" runat="server" Visible="false">
    <h1>An error ocured while installing the Multiple File Upload package</h1>
    To fix this error please follow these simple steps
    <h3>Register the MultipleFileUploadHandler (IHttpHandler) in the web.config file.</h3><br />
    <p>
        Add the following line to the system.web/httpHandlers section in the web.config file.<br />
    </p>
    <pre>&lt;add path="MultipleFileUploadHandler.axd" verb="POST" type="noerd.Umb.DataTypes.multipleFileUpload.MultipleFileUploadHandler, noerd.Umb.DataTypes.multipleFileUpload" /&gt;</pre>        
</asp:Panel>

<asp:Panel ID="pnlUninstall" runat="server" Visible="false">
    <h3>List of files that has been added to the umbraco instance:</h3>
    <ul>
        <li>~/bin/noerd.Umb.DataTypes.multipleFileUpload.dll</li>
        <li>~/scripts/swfobject.js</li>
        <li>~/config/MultipleFileUpload.config</li>
        <li>~/usercontrols/MultipleFileUpload/MultipleFileUpload.swf</li>
        <li>~/usercontrols/MultipleFileUpload/MultipleFileUpload.ascx</li>
        <li>~/usercontrols/MultipleFileUpload/MultipleFileUpload.ascx.cs</li>
        <li>~/usercontrols/MultipleFileUpload/Installer.ascx</li>
        <li>~/usercontrols/MultipleFileUpload/Installer.ascx.cs</li>
        <li>~/usercontrols/MultipleFileUpload/MultipleFileUpload.pdf</li>
    </ul>
    <h3>Uninstalling the Multiple File Upload package</h3><br />
    <em>Warning:</em> If you wish to uninstall the package be sure to follow the instructions below in the in the correct order, otherwise you could end up invalidating the folders in the media section.
    <ol>
        <li>Goto the settings section and expand the <b>Media Type</b> folder</li>
        <li>Select the <b>Folder</b> node and press the <b>Generic properties</b> tab and delete the property <b>"Upload multiple files"</b></li>
        <li>Goto the <b>Tabs</b> tab and delete the <b>Upload</b> tab.</li>
        <li>Go to the developer section and delete the Multiple File Upload data type.</li>
        <li>Open the web.config file, goto the system.web/httpHandlers section and delete this line:<br />
<pre>&lt;add path="MultipleFileUploadHandler.axd" verb="POST" type="noerd.Umb.DataTypes.multipleFileUpload.MultipleFileUploadHandler, noerd.Umb.DataTypes.multipleFileUpload" /&gt;</pre> </li>
        <li><b>Umbraco v4:</b> Goto the developer section and expand the Packages node and the Installed packages node. Click on the the MultipleFileUpload node and press the Uninstall package button.<br /><br />
        <b>Umbraco v3.x:</b> Remove the files listed above from the umbraco instance.</li>
    </ol>
</asp:Panel>