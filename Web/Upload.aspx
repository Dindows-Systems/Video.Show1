<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="Upload.aspx.cs" Inherits="Upload" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
    <title>Upload</title>
    <script type="text/javascript">
            function setDefaultMenu() {
        	if (typeof whichMenu == "undefined") {
        		var whichMenu = 0;
        	}
            var menuDiv = document.getElementById("menu-nav");
            var menu = menuDiv.getElementsByTagName("ul")[0]
            var menuItems = menu.getElementsByTagName("li");
            menuItems[menuItems.length-1].className += " selected"
        };
        siteMenu.loadEvent(setDefaultMenu);
    </script>
    <style type="text/css">
        #frame #main #content-frame
        {
            background: none;	
        }
    </style>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentPlaceHolderMain">
    <div id="video-upload">
    <div id="Upload">
    <h2>Upload Videos</h2>
    <div id="upload-top"></div>
    <div id="upload-body">
        <asp:Label runat="server" class="results" ID="Results"></asp:Label>
        <asp:PlaceHolder ID="UploadForm" runat="server">
        <table>
            <tr>
                <td>Video File</td>
                <td><asp:FileUpload runat="server" ID="fileUpload"/></td>
            </tr>
            <tr>
                <td>Title</td>
                <td> <asp:TextBox runat="server" ID="VideoTitle" Columns="80"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>Description</td>
                <td> 
                    <asp:TextBox runat="server" ID="Description" 
            Rows="10" TextMode="MultiLine" Columns="60"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>Tags</td>
                <td> <asp:TextBox runat="server" ID="Tags" Columns="80"></asp:TextBox>
                    <div class="Tip">Enter tags separated by commas: <em>fish, scuba, vacation</em></div>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                <asp:LinkButton runat="server" ID="UploadSubmit" OnClick="UploadSubmit_Click" CssClass="submit-upload"><span>Upload</span></asp:LinkButton></td>
            </tr>
        </table>
        </asp:PlaceHolder>
        <asp:LinkButton runat="server" ID="UploadAnother" Text="Upload Another Video" 
            Visible="false" onclick="UploadAnother_Click" />
        </div>
        <div id="upload-bottom"></div>
        <div></div></div>
    </div>
</asp:Content>