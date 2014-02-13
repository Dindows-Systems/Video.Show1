<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeBehind="Member.aspx.cs" Inherits="VideoShow.Member" %>

<%@ Register src="UserControls/VideoGrid.ascx" tagname="VideoGrid" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <title>VideoShow</title>
    <script type="text/javascript">
            function setDefaultMenu() {
        	if (typeof whichMenu == "undefined") {
        		var whichMenu = 0;
        	}
            var menuDiv = document.getElementById("menu-nav");
            var menu = menuDiv.getElementsByTagName("ul")[0]
            var menuItems = menu.getElementsByTagName("li");
            menuItems[menuItems.length-3].className += " selected"
        };
        siteMenu.loadEvent(setDefaultMenu);

    </script>
    <style type="text/css">
        #playlist-frame
        {
            display: none;	
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolderMain" Runat="Server">
    <div id="page-content">
        <div id="user-dashboard">
            <img class="avatar" src="<%= GetAvatar() %>" />
            <h3 class="user-name">
                <%= GetUserName() %></h3>
            <ul id="dashboard-links">
                <li><a href="?memberid=<%= MemberId %>"><%= GetVideoCount() %>&nbsp;Videos</a>
                    </li>
                <li>| </li>
                <li><a href="?view=favorite&memberid=<%= MemberId %>"><%= GetFavoriteCount() %>&nbsp;Favorites</a></li>
                <li runat="server" ID="EditAccountLinkContainer">| <a href="MemberProfile.aspx">Edit account</a></li>
            </ul>
            <asp:PlaceHolder ID="ShowUpload" runat="server">
                <a runat="server" id="UploadVideo" href="Upload.aspx" class="upload-video"><span>Upload a video</span></a>
            </asp:PlaceHolder>
            <div class="clearFloat">
            </div>
        </div>
        <span id="user-tagline"><em><%= VideoShow.UserProfile.GetUserProfile(GetUserName()).Description %></em></span>
        <div id="user-information">
            <ul runat="server" id="UserInformationList" />
        </div>
        <!-- Where the Silverlight plug-in will go-->
        <uc1:VideoGrid ID="MemberVideoGrid" runat="server" />
    </div>
</asp:Content>
