///////////////////////////////////////////////////////////////////////////////
//
//  ExtendedPlayer
//
//  This extends the base player class, you may override the base player
//  member functions or add additional player functionality here. Here 
//  we show a volume slider if you hover near the mute audio button.
//
///////////////////////////////////////////////////////////////////////////////
Type.registerNamespace('ExtendedPlayer');

ExtendedPlayer.Player = function(domElement)
{
    ExtendedPlayer.Player.initializeBase(this, [domElement]);    
}

ExtendedPlayer.Player.prototype =  {
    xamlInitialize: function() {    
        ExtendedPlayer.Player.callBaseMethod(this, 'xamlInitialize');    
        this._hoverVolumeControl = new ExtendedPlayer.MouseOverControl(this.get_element(), "VolumeControl");
    },

    xamlDispose: function() {
		if (this._hoverVolumeControl) this._hoverVolumeControl.dispose();
		this._hoverVolumeControl = null;
		ExtendedPlayer.Player.callBaseMethod(this, 'xamlDispose');    				
    }
}
ExtendedPlayer.Player.registerClass('ExtendedPlayer.Player', EePlayer.Player);

ExtendedPlayer.MouseOverControl = function(host, nameElement) {
    // plays animations on mouse enter/leave
    this._element = host.content.findName(nameElement);
    this._t1 = this._element.addEventListener("mouseEnter", Function.createDelegate(this, this._mouseEnter));
    this._t2 = this._element.addEventListener("mouseLeave", Function.createDelegate(this, this._mouseLeave));
    this._enter = host.content.findName(nameElement + "_MouseEnter");
    this._leave = host.content.findName(nameElement + "_MouseLeave");
}
ExtendedPlayer.MouseOverControl.prototype = {
    dispose: function() {
        this._element.removeEventListener("mouseEnter", this._t1);
        this._element.removeEventListener("mouseLeave", this._t2);
        this._enter = null;
        this._leave = null;
        this._element = null;
    },
    _mouseEnter: function() {
        this._enter.begin();
    },
    _mouseLeave: function() {
        this._leave.begin();
    }
}
ExtendedPlayer.MouseOverControl.registerClass("ExtendedPlayer.MouseOverControl");

