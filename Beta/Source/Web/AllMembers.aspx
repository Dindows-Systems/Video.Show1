<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="True" CodeBehind="AllMembers.aspx.cs" Inherits="VideoShow.AllMembers" Title="Tag Channel >> Members >> All Members" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolderMain" Runat="Server">
    <asp:UpdatePanel runat="server">
    <ContentTemplate>
        <asp:ListView ID="MemberListView" runat="server" GroupItemCount="4" >
            <LayoutTemplate>
            <table id="Table1" runat="server" class="member-list">
                <tr id="Tr1" runat="server">
                    <td id="Td2" runat="server">
                        <table id="groupContainer" runat="server" border="0" style="">
                        </table>
                    </td>
                </tr>
                <tr id="Tr2" runat="server">
                    <td id="Td3" runat="server" style="">
                    </td>
                </tr>
            </table>
            </LayoutTemplate>
            <grouptemplate>
                <tr id="itemContainer" runat="server">
                </tr>
            </grouptemplate>
            <ItemTemplate>
                <td id="Td4" runat="server" class="MemberProfile" style="">
                        <img border="0" alt='<%# Eval("UserName") %>' src="App_Themes/Default/Images/guy.gif" class="avatar" />
                        <div class="member-data">
                        <div>
                        <a id="A1" href='VideoList.aspx?view=member&memberid=<%# Eval("UserId") %>'><span><%# Eval("UserName") %></a></div>
                        <div>member since <%# Eval("aspnet_Membership.CreateDate","{0:d}")%></div>
                        <div>
                        <span><%# Eval("Videos.Count")%>videos</span> | <span><%# Eval("Favorites.Count")%>favorites</span>
                        </div>
                        </div>
                        </td>
            </ItemTemplate>
            <EmptyDataTemplate>
                No members have signed up yet.
            </EmptyDataTemplate>
        </asp:ListView>
            <hr class="separator3" />
            <div id="pagination">
                <asp:HiddenField ID="CurrentPage" runat="server" />
                <asp:LinkButton ID="PreviousButton" CausesValidation="false" runat="server" onclick="previous_Click">&lt;&lt; previous</asp:LinkButton>
                <asp:LinkButton ID="NextButton" CausesValidation="false" runat="server" onclick="next_Click">next &gt;&gt;</asp:LinkButton>
                <div class="clearFloat"></div>
            </div>
    </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

