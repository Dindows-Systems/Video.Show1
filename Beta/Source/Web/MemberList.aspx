<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="True"
    CodeBehind="MemberList.aspx.cs" Inherits="MemberList" Title="Members" %>

<%@ Register Src="UserControls/VideoGrid.ascx" TagName="VideoGrid" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">
            function setDefaultMenu() {
        	if (typeof whichMenu == "undefined") {
        		var whichMenu = 0;
        	}
            var menuDiv = document.getElementById("menu-nav");
            var menu = menuDiv.getElementsByTagName("ul")[0]
            var menuItems = menu.getElementsByTagName("li");
            menuItems[2].className += " selected"
        };
        siteMenu.loadEvent(setDefaultMenu);

    </script>
    <style type="text/css">
    #content-frame 
    {
    	padding: 0px 20px;
    }
    h3 { margin-left: -20px; }
    
	#content-frame
	{
	    min-height: 560px;
	}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolderMain" runat="Server">
    <div id="page-content">
    <h3 class="user-name">All members</h3>
    <div class="clearFloat"></div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Panel ID="PanelMembersWithVideos" runat="server">
                        <asp:ListView ID="MemberRepeater" runat="server" OnItemDataBound="MemberRepeater_ItemDataBound">
                        <LayoutTemplate>
                            <table class="video-grid" id="itemContainer" runat="server">
                            </table>
                        </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <img id="Img1" runat="server" alt='<%# Eval("UserName") %>' class="avatar" src='<%# GetAvatarUrl(Convert.ToString(Eval("UserId"))) %>' />
                                        <div class="member-data">
                                            <p class="title">
                                                <a href='Member.aspx?memberid=<%# Eval("UserId") %>'>
                                                    <%# Eval("UserName") %></a></p>
                                            <p>
                                                <%# Eval("Videos.Count")%> videos</p>
                                        </div>
                                    </td>
                                    <td>
                                        <uc1:VideoGrid ID='MemberVideoGrid' runat="server" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
                </asp:Panel>
                <asp:Panel runat="server" ID="PanelAllMembers" Visible="false">
                    <asp:ListView ID="MemberListView" runat="server" GroupItemCount="4">
                        <LayoutTemplate>
                            <table id="Table1" runat="server" class="member-list">
                                <tr id="Tr1" runat="server">
                                    <td id="Td2" runat="server">
                                        <table id="groupContainer" runat="server">
                                        </table>
                                    </td>
                                </tr>
                                <tr id="Tr2" runat="server">
                                    <td id="Td3" runat="server" />
                                </tr>
                            </table>
                        </LayoutTemplate>
                        <GroupTemplate>
                            <tr id="itemContainer" runat="server">
                            </tr>
                        </GroupTemplate>
                        <ItemTemplate>
                            <td id="Td4" runat="server" class="MemberProfile" style="">
                                <img id="Img1" runat="server" alt='<%# Eval("UserName") %>' class="avatar" src='<%# GetAvatarUrl(Convert.ToString(Eval("UserId"))) %>' />
                                <div class="member-data">
                                    <div>
                                        <a href='Member.aspx?memberid=<%# Eval("UserId") %>'>
                                            <%# Eval("UserName") %></a><div>
                                        member since
                                        <%# Eval("aspnet_Membership.CreateDate","{0:d}")%></div>
                                    <div>
                                        <span>
                                            <%# Eval("Videos.Count")%> videos</span> | <span>
                                                <%# Eval("Favorites.Count")%> favorites</span>
                                    </div>
                                </div>
                            </td>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            No members have signed up yet.
                        </EmptyDataTemplate>
                    </asp:ListView>
                </asp:Panel>
                <hr class="separator4" />
                <div id="pagination">
                    <asp:HiddenField ID="CurrentPage" runat="server" />
                    <asp:LinkButton CssClass="PreviousButton" ID="PreviousButton" runat="server" OnClick="previous_Click">&lt;&lt; previous</asp:LinkButton>
                    <asp:LinkButton CssClass="NextButton" ID="NextButton" runat="server" OnClick="next_Click">next &gt;&gt;</asp:LinkButton>
                    <div class="clearFloat">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
