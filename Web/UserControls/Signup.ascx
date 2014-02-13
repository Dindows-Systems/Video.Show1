<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Signup.ascx.cs" Inherits="VideoShow.UserControls.Signup" %>
<%@ Register Src="~/UserControls/AvatarSelector.ascx" TagName="AvatarSelector" TagPrefix="uc1" %>

<asp:ScriptManagerProxy runat="server" ID="scriptManagerProxy">
    <Services>
        <asp:ServiceReference Path="~/Webservices/UserWebservice.asmx" />
    </Services>
</asp:ScriptManagerProxy>

<asp:LinkButton ID="ShowSignupPopup" CssClass="ShowSignupPopup" Text="Sign Up" OnClientClick="return false;" runat="server" />
    <div id="sign-up-dialog">
      <asp:Panel ID="SignupPanel" runat="server" DefaultButton="SubmitSignup">
        <a accesskey="" runat="server" id="CloseSignUp" class="CloseSignUp" onclick="return false;" href="/#"><span>close</span></a>
        <h3>
            First&hellip; Sign Up!</h3>
        <div id="ErrorMessage" style="color:Fuchsia;"></div>
        <label for="Username">
            Your Username</label>
        <input type="text" id="Username" />
        <label for="Email">
            Your E-mail</label>
        <input type="text" id="Email" />
        <label for="Password">
            Your Password</label>
        <input type="Password" id="Password" />
        
        <uc1:AvatarSelector runat="server" ID="AvatarSelector" />        

        <div id="finish-sign-up-span">
            <asp:LinkButton runat="server" ID="SubmitSignup" Text="Finish" OnClientClick="createUser();return false;" />
        </div>
        <iframe style="visibility:hidden;display:none;" id="LoginFrame"></iframe>
        <div class="clearFloat">
        </div>
      </asp:Panel>
    </div>

<script type="text/javascript">
//This should be a shared utility function
function bind(obj, method) 
{  
    return function() { return method.apply(obj, arguments); }
}

var username = "";
var password = "";
var email = "";

function createUser()
{
    username = $get("Username").value;
    email = $get("Email").value;
    password = $get("Password").value;
    var error = validate(username, password, email)
    var avatarId = $get("<%= AvatarSelector.GetSelectedAvatarControlId() %>").value;
    if(error == '')
        VideoShow.UserWebservice.CreateUser(username,email,password,avatarId,bind(this, this.onUserAdded));
    else
        $get("ErrorMessage").innerText = "Please fix these so we can get you signed up: " + error;
}
function validate(username, password, email)
{
    var result = '';
    if(username == '') result += 'The username was blank.';
    if(password == '') result += 'The password was blank.';
    var filter  = /^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i;
	if (!filter.test(email)) result += "The e-mail address not valid.";
	return result;
	
}
function onUserAdded(userId)
{
    if(userId.length > 0)
    {
        $get("LoginFrame").src = "/LoginRedirect.aspx?username=" + username + "&password=" + password;
        window.location.reload();
    }
    else
    {
        $get("ErrorMessage").innerText = "That didn't work at all!";
    }
}
</script>
    
<ajaxToolkit:AnimationExtender ID="SignupAnimation" runat="server" TargetControlID="CloseSignUp">
    <Animations>
    <OnClick>
        <Sequence AnimationTarget="sign-up-dialog">
          <Parallel Duration=".2" Fps="24">
            <Move Relative="false" Vertical="-430" Horizontal="-200" />
            <FadeOut />
          </Parallel>
          <ScriptAction Script="$get('SearchText').focus();" />
        </Sequence>
    </OnClick>
    </Animations>
</ajaxToolkit:AnimationExtender>
<ajaxToolkit:AnimationExtender ID="AnimationExtender1" runat="server" TargetControlID="ShowSignupPopup">
    <Animations>
        <OnClick>
            <Sequence>
              <Parallel Duration=".2" Fps="24">
                <FadeOut AnimationTarget="sign-in-panel" />
                <Move AnimationTarget="sign-in-panel" Relative="false" Vertical="-400" Horizontal="-300" />
                <Move Relative="false" Horizontal="-200" Vertical="30" AnimationTarget="sign-up-dialog" />
                <FadeIn AnimationTarget="sign-up-dialog" />
                <ScriptAction Script="$get('Username').focus();" />
              </Parallel>
            </Sequence>
        </OnClick>
    </Animations>
</ajaxToolkit:AnimationExtender>
