<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecentViews.aspx.cs" Inherits="VideoShow.RecentViews" MasterPageFile="~/MasterPage.master" %>

<%@ Register src="UserControls/VideoGrid.ascx" tagname="VideoGrid" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <title>Recently viewed videos</title>
    <script type="text/javascript">
            function setDefaultMenu() {
        	if (typeof whichMenu == "undefined") {
        		var whichMenu = 0;
        	}
            var menuDiv = document.getElementById("menu-nav");
            var menu = menuDiv.getElementsByTagName("ul")[0]
            var menuItems = menu.getElementsByTagName("li");
            menuItems[4].className += " selected"
        };
        siteMenu.loadEvent(setDefaultMenu);

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolderMain" Runat="Server">
    <div id="page-content">
        <h3 class="user-name">Recent Views</asp:Label></h3>
        <div class="clearFloat">
        </div>
        <!-- Where the Silverlight plug-in will go-->
        <uc1:VideoGrid ID="RecentViewsVideoGrid" runat="server" />
    </div>
</asp:Content>

