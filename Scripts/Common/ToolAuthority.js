
// ajax回调函数处理系统退出  
Ext.Ajax.on('requestcomplete', checkUserSessionStatus, this);
function checkUserSessionStatus(conn, response, options) {
    //Ext重新封装了response对象     
    if (response.getAllResponseHeaders&&response.getAllResponseHeaders().sessionstatus) {
        alert('登入過期，請重新登入！');
        window.top.location.href = "http://"+window.top.location.host;
    }
}  

//頁面按鈕權限
function ToolAuthority() {
    var ParentID = window.parent.Ext.getCmp('ContentPanel').activeTab.id;

    Ext.Ajax.request({
        url: '/FunctionGroup/GetAuthorityTool',
        method: "POST",
        params: { RowId: ParentID },
        success: function (form, action) {
            var data = Ext.decode(form.responseText);
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var btn = Ext.getCmp(data[i].id);
                    if (btn) {
                        btn.show();
                    }
                }
            }
        }
    });
}

function ToolAuthorityQuery(callback) {
    COL_TOOL = new Array();
    var ParentID = window.parent.Ext.getCmp('ContentPanel').activeTab.id;
    Ext.Ajax.request({
        url: '/FunctionGroup/GetAuthorityTool',
        method: "POST",
        params: { RowId: ParentID },
        success: function (form, action) {
            var data = Ext.decode(form.responseText);
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    COL_TOOL.push(data[i].id + "|" + data[i].isEdit);
                }
            }
        },
        callback: callback
    });
}

function ToolAuthorityQueryByUrl(url, callback) {
    COL_TOOL = new Array();
    Ext.Ajax.request({
        url: '/FunctionGroup/GetAuthorityToolByUrl',
        method: "POST",
        params: { Url: url },
        success: function (form, action) {
            var data = Ext.decode(form.responseText);
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    COL_TOOL.push(data[i].id+"|"+data[i].isEdit);
                }
            }
        },
        callback: callback
    });
}

var COL_TOOL;
function QueryToolAuthorityByUrl(url) {
    COL_TOOL = new Array();
    Ext.Ajax.request({
        url: '/FunctionGroup/GetAuthorityToolByUrl',
        method: "POST",
        async: false,
        params: { Url: url },
        success: function (form, action) {
            var data = Ext.decode(form.responseText);
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    COL_TOOL.push(data[i].id + "|" + data[i].isEdit);
                }
            }
        }
    });
}
function setShow(panel, str) {
    Ext.Array.each(COL_TOOL, function (val) {
        var arr = val.split("|");
        var controls = panel.query('*[' + str + '=' + arr[0] + ']');
        Ext.Array.each(controls, function () {
            this.show();
            if (arr[1] == 0) {
                switch (this.xtype) {
                    case "gridcolumn":
                        if (this.getEditor) {
                            this.getEditor().disable();
                        }
                        //else {
                        //    this.disable();
                        //}
                        break;
                    default:
                        this.disable();
                        break;
                }
            }
        });
    });
}

