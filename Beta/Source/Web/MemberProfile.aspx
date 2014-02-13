<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberProfile.aspx.cs" Inherits="VideoShow.MemberProfile" MasterPageFile="~/MasterPage.master"%>
<%@ Register Src="~/UserControls/AvatarSelector.ascx" TagName="AvatarSelector" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
    <title>Member Profile</title>
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
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentPlaceHolderMain">
    <style>
        #playlist-frame { display: none; }
    </style>
    <div id="video-upload">
    <div id="Upload">
    <h2>Your Account Details</h2>
    <div id="upload-top"></div>
    <div id="upload-body">
        <asp:Label runat="server" class="results" ID="Results"></asp:Label>
        <table>
            <tr>
                <td>Email address:</td>
                <td> <asp:TextBox runat="server" ID="Email"></asp:TextBox>
                </td>
                <td> Other members cannot see your email address.</td>
            </tr>
            <tr>
                <td>Password</td>
                <td> <asp:TextBox runat="server" ID="Password"></asp:TextBox>
                </td>
                <td> Change password</td>
            </tr>
            <tr>
                <td>Screen Name</td>
                <td> <asp:Label runat="server" ID="Username"></asp:Label>
                </td>
                <td> How your name shows up to other members.</td>
            </tr>
            <tr>
                <td>
                    Shout out
                </td>
                <td>
                    <asp:TextBox ID="Description" runat="server" Rows="3" TextMode="MultiLine"></asp:TextBox>
                </td>
                <td>
                    Tell us a little about yourself.</td>
            </tr>
            <tr>
                <td>Full Name:</td>
                <td> <asp:TextBox runat="server" ID="FullName"></asp:TextBox>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>Location:</td>
                <td> <asp:TextBox runat="server" ID="Location"></asp:TextBox>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>Favorite Movie:</td>
                <td> <asp:TextBox runat="server" ID="FavoriteMovie"></asp:TextBox>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>Avatar image</td>
                <td>
                    <uc1:AvatarSelector runat="server" ID="AvatarSelector" />        
                </td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                <asp:LinkButton runat="server" ID="UpdateSubmit" OnClick="UpdateSubmit_Click" CssClass="submit-upload"><span>Save Changes</span></asp:LinkButton>
                </td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                or return to <a runat="server" href="~/Member.aspx">your member page</a>
                </td>
                <td>
                    &nbsp;</td>
            </tr>
        </table>
        </div>
        <div id="upload-bottom"></div>
        <div></div></div>
    </div>
</asp:Content>