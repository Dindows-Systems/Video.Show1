<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="True" CodeBehind="View.aspx.cs" Inherits="View" Title="Untitled Page" %>
<%@ Register src="UserControls/VideoGrid.ascx" tagname="VideoGrid" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <title>Video.Show</title>
    <style type="text/css">
        /* Fix for Firefox */
        html:not([lang*=""]):not(:only-child) #content-frame
        {
	        height: 725px;	
	        position: relative;
        }
        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolderMain" Runat="Server">
    <!--
    //The AjaxControlToolkit Scripts are included to allow direct use of AjaxControlToolkit Animations from Javascript.
    //These can be removed if an Animation control is needed elsewhere on the page.
    -->
    <asp:ScriptManagerProxy runat="server" ID="ScriptManagerProxy">
        <Services>
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/Silverlight/Player/BasePlayer/BasePlayer.js" />
            <asp:ScriptReference Path="~/Silverlight/Player/BasePlayer/PlayerStrings.js" />
            <asp:ScriptReference Path="~/Silverlight/Player/BasePlayer/ExtendedPlayer.js" />
            <asp:ScriptReference Path="~/Silverlight/Player/Player.js" />
            <asp:ScriptReference Assembly="AjaxControlToolkit" Name="AjaxControlToolkit.Common.Common.js" />
            <asp:ScriptReference Assembly="AjaxControlToolkit" Name="AjaxControlToolkit.Compat.Timer.Timer.js" />
            <asp:ScriptReference Assembly="AjaxControlToolkit" Name="AjaxControlToolkit.Animation.Animations.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <div id="PlayerContainer">
        <div id="video-media-header">
            <img alt="<%= SelectedVideo.aspnet_User.UserName %>" class="avatar" src='<%= GetOwnerAvatar(SelectedVideo.VideoId) %>' />
            <h3 class="user-name"><%= SelectedVideo.Title %></h3>
            <div class="member-data"><a href="Member.aspx?MemberId=<%= SelectedVideo.aspnet_User.UserId %>"><%= SelectedVideo.aspnet_User.UserName %></a></div>
                               
            <div class="clearFloat"></div>
            <ul id="dashboard-links">
                <li><span id="number-of-video-views"><%= SelectedVideo.Views.Count %> view(s)</span></li>
                <li> | </li>
                <li>Posted on <span id="posted-date-span"><%= SelectedVideo.DatePublished.ToShortDateString() %></span></li>
            </ul>
        </div>
        <div id="comment-area-header">
        
        <asp:LinkButton runat="server" ID="DeleteVideo" class="delete-video" 
                    Text="Delete" Visible="false" onclick="DeleteVideo_Click"><span>Delete Video</span></asp:LinkButton>
            <ajaxToolkit:ConfirmButtonExtender ID="cbe" runat="server" TargetControlID="DeleteVideo" ConfirmText="This video will be gone forever, and we all might miss it. Are you sure you want to send it to video heaven?" /> 
            <asp:LoginView ID="LoginView1" runat="server">
                <AnonymousTemplate>
                    <span id="login-request-span">Sign In to comment or favorite a video.</span>
                </AnonymousTemplate>
                <LoggedInTemplate>
                    <div class="button"><a href="#" onclick="player.showCommentForm()">Add Comment</a></div>
                    <div class="button"><a id="ToggleFavoriteLink" href="#" onclick="player.toggleFavorite()">Favorite</a></div>
                </LoggedInTemplate>
            </asp:LoginView>
        </div>
        <div class="clearFloat"></div>
        <div id="Player">
            <div id="TabContainer">

            </div>
            <div style="width:900px;height:425px;">
                <div id="Player_SilverlightContainer">
	                <script type="text/javascript">
                        var player = new PlayerControl();
	                </script>
                </div>
            </div>
            <div id="VideoDescription"></div>
            <div id="VideoTags"></div>
            <div id="VideoFavorites"></div>
        </div>

    <div id="commentForm">
        <table>
            <tr>
                <td>
                    <textarea id="CommentText" cols="20" rows="5"></textarea>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="AddComment" class="button"><a href="#" onclick="player.SubmitComment()">Submit Comment</a></div>
                    <div id="CancelComment" class="button"><a href="#" onclick="player.hideCommentForm()">Cancel</a></div>
                </td>
            </tr>
        </table>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="contentPlaceHolderPlaylist" runat="server">
    <div id="video-strip-nav">
        <div class="more-videos"><span>More videos by <%= SelectedVideo.aspnet_User.UserName %>: </span></div>
    </div>
    <uc2:VideoGrid ID="PlaylistVideoGrid" runat="server" />
</asp:Content>

