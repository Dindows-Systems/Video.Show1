if (typeof siteMenu === "undefined") {
    var siteMenu = {
        loadEvent : function ( F ) {
            var oldonload = window.onload;
            if (typeof window.onload !== 'function') {
                window.onload = F;
            } else {
                window.onload = function () {
                    oldonload();
                    F();
                };
            }
        },
        addClass : function ( elem, name ) {
            var nameCheck = new RegExp("(^|\\s)" + name + "(\\s|$)");
            if (!nameCheck.test(elem.className)) {
                if (elem.className == "") {
                    elem.className = name;
                } else {
                    elem.className += " " + name;
                }
            }
        },
        removeClass : function ( elem, name ) {
            var modifiedClass = elem.className;
            var nameCheck = new RegExp("(^|\\s)" + name + "(\\s|$)");
            modifiedClass = modifiedClass.replace(nameCheck, "");
            elem.className = modifiedClass;
        },
        setHoverMenu : function () {
            var that = siteMenu;
            var menuDiv = document.getElementById("menu-nav");
            var menu = menuDiv.getElementsByTagName("ul")[0]
            var menuItems = menu.getElementsByTagName("li");
            var len = menuItems.length;
            for (var i = 0; i < len; i++) {
                menuItems[i].onmouseover = function() {
                    that.addClass(this, "selected2");
                    this.style.cursor = "pointer";
                };
                menuItems[i].onmouseout = function() {
                    that.removeClass(this, "selected2");
                }
            }
        }
    };
}
