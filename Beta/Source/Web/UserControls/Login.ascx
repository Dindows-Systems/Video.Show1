<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Login.ascx.cs" Inherits="VideoShow.UserControls.Login" %>
<asp:LinkButton runat="server" ID="ShowLoginPopup" OnClientClick="return false;" Text="Sign In"></asp:LinkButton>
<ajaxToolkit:AnimationExtender ID="SignInAnimation" runat="server" TargetControlID="CloseSignIn">
    <Animations>
        <OnClick>
            <Sequence>
              <Parallel Duration=".2" Fps="24">
                <FadeOut AnimationTarget="sign-in-panel" />
                <Move AnimationTarget="sign-in-panel" Relative="false" Vertical="-400" Horizontal="-300" />
              </Parallel>
            </Sequence>
        </OnClick>
    </Animations>
</ajaxToolkit:AnimationExtender>
<ajaxToolkit:AnimationExtender ID="AnimationExtender1" runat="server" TargetControlID="ShowLoginPopup">
    <Animations>
        <OnClick>
            <Sequence>
              <Parallel Duration=".2" Fps="24">
                <Move Relative="false" Vertical="-430" Horizontal="-200" AnimationTarget="sign-up-dialog" />
                <FadeOut AnimationTarget="sign-up-dialog" />
                <FadeIn AnimationTarget="sign-in-panel" />
                <Move AnimationTarget="sign-in-panel" Relative="false" Vertical="30" Horizontal="-300" />
                <ScriptAction Script="$get('ctl00_LoginView_Login_LoginControl_UserName').focus();" />
              </Parallel>
            </Sequence>
        </OnClick>
    </Animations>
</ajaxToolkit:AnimationExtender>

<div id="sign-in-panel">
    <asp:Panel ID="LoginPanel" runat="server" CssClass="LoginDialog" DefaultButton="LoginControl$LoginLinkButton">
        <div class="button"><asp:LinkButton runat="server" id="CloseSignIn" Text="close" OnClientClick="return false;" ></asp:LinkButton></div>
        <asp:Login ID="LoginControl" runat="server" TitleText="Sign In" 
            LoginButtonText="Sign In" LoginButtonType="Link" OnLoginError="Login_LoginError">
            <layouttemplate>

                <div class="AspNet-Login" id="ctl00_LoginView_Login_LoginControl">
                    <div class="AspNet-Login-TitlePanel">
                        <span>Sign In</span>
                    </div>
                    <div class="AspNet-Login-UserPanel">
                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User Name:</asp:Label>
                                        <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" 
                                            ControlToValidate="UserName" ErrorMessage="User Name is required." 
                                            ToolTip="User Name is required." ValidationGroup="ctl00$LoginControl">*</asp:RequiredFieldValidator>
                    </div>
                    <div class="AspNet-Login-PasswordPanel">
                                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                                        <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" 
                                            ControlToValidate="Password" ErrorMessage="Password is required." 
                                            ToolTip="Password is required." ValidationGroup="ctl00$LoginControl">*</asp:RequiredFieldValidator>
                    </div>
                    <div class="AspNet-Login-RememberMePanel">
                                        <asp:CheckBox ID="RememberMe" runat="server" Text="Remember me:" />
                    </div>
                    <div class="AspNet-Login-FailurePanel">
                                        <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                        </div>
                    <div class="AspNet-Login-SubmitPanel">
                                        <div class="button">
                                        <asp:LinkButton ID="LoginLinkButton" runat="server" CommandName="Login" 
                                            ValidationGroup="ctl00$LoginControl">Sign In</asp:LinkButton>
                                        </div>
                    </div>
                </div>
                <DIV class=clearFloat></DIV>
            </layouttemplate>
        </asp:Login>
        <div class="clearFloat"></div>
    </asp:Panel>
</div>
