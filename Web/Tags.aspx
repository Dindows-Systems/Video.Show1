<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tags.aspx.cs" Inherits="VideoShow.Tags" MasterPageFile="~/MasterPage.master" %>

<%@ Register src="UserControls/VideoGrid.ascx" tagname="VideoGrid" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <title>Video.Show</title>
    <script type="text/javascript">
            function setDefaultMenu() {
        	if (typeof whichMenu == "undefined") {
        		var whichMenu = 0;
        	}
            var menuDiv = document.getElementById("menu-nav");
            var menu = menuDiv.getElementsByTagName("ul")[0]
            var menuItems = menu.getElementsByTagName("li");
            menuItems[1].className += " selected"
        };
        siteMenu.loadEvent(setDefaultMenu);

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolderMain" Runat="Server">
    <div id="page-content">
        <h3 class="user-name"><asp:Label ID="AllVideosLabel" runat="server" Visible="true">All videos</asp:Label><asp:HyperLink ID="AllVideosLink" runat="server" Visible="false" NavigateUrl="~/Tags.aspx">All videos</asp:HyperLink><asp:Label ID="TagLabel" runat="server"></asp:Label></h3>
        <div class="clearFloat">
        </div>
        <!-- Where the Silverlight plug-in will go-->
        <uc1:VideoGrid ID="TagsVideoGrid" runat="server" />
    </div>
</asp:Content>
