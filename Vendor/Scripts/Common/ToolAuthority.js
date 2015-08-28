
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
                    COL_TOOL.push(data[i].id);
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
                    COL_TOOL.push(data[i].id);
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
                    COL_TOOL.push(data[i].id);
                }
            }
        }
    });
}
function setShow(panel, str) {
    Ext.Array.each(COL_TOOL, function (val) {
        var controls = panel.query('*[' + str + '=' + val + ']');
        Ext.Array.each(controls, function () { this.show(); });
    });
}

