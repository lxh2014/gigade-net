/*
* 文件名稱 :Fgroup.js
* 文件功能描述 :群組管理
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/02
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

Ext.Loader.setConfig({ enabled: true });

Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
//Ext.Loader.setPath('Ext.ux', '../../../Scripts/Views/ProductParticulars');

Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector',
    'Ext.ux.CheckColumn'
]);
var CallidForm;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.Fgroup', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "rowid", type: "string" },
        { name: "groupName", type: "string" },
        { name: "groupCode", type: "string" },
        { name: "callid", type: "string" },
        { name: "remark", type: "string" }]
});

var FgroupStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.Fgroup',
    proxy: {
        type: 'ajax',
        url: '/Fgroup/QueryAll',
        reader: {
            type: 'json',
            root: 'items'
        }
    },
    autoLoad: true
});

//使用者Model
Ext.define('gigade.ManageUser', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "name", type: "string" },
        { name: "callid", type: "string" }]
});

var ManageUserStore = Ext.create('Ext.data.Store', {
    autoDestroy: false,
    model: 'gigade.ManageUser',
    proxy: {
        type: 'ajax',
        url: '/Fgroup/QueryCallid',
        reader: {
            type: 'json',
            root: 'items'
        }
    },
    autoLoad: true
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdFgroup").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdFgroup").down('#remove').setDisabled(selections.length == 0);
            Ext.getCmp("gdFgroup").down('#auth').setDisabled(selections.length == 0);
            Ext.getCmp("gdFgroup").down('#callid').setDisabled(selections.length == 0);
        }
    }
});

Ext.onReady(function () {
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: FgroupStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: 'ID', dataIndex: 'rowid', align: 'center', hidden: true, menuDisabled: true, sortable: false },
            { header: GROUPNAME, dataIndex: 'groupName', width: 200, align: 'center', menuDisabled: true, sortable: false },
            { header: GROUPCODE, dataIndex: 'groupCode', width: 200, align: 'center', menuDisabled: true, sortable: false },
            { header: CALLID, dataIndex: 'callid', align: '120', align: 'center', menuDisabled: true, sortable: false },
            { header: REMARK, dataIndex: 'remark', align: '120', align: 'left', menuDisabled: true, sortable: false, flex: 1 }],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: true, iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', hidden: true, iconCls: 'ui-icon ui-icon-user-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', text: REMOVE, id: 'remove', hidden: true, iconCls: 'ui-icon ui-icon-user-delete', disabled: true, handler: onRemoveClick },
            { xtype: 'button', text: TOOL_AUTHORITY, id: 'auth', hidden: true, iconCls: 'ui-icon ui-icon-key', disabled: true, handler: onAuthClick },
            { xtype: 'button', text: TOOL_CALLID, id: 'callid', hidden: true, iconCls: 'ui-icon ui-icon-user-suite', disabled: true, handler: onCallidClick },
            { xtype: 'button', text: OUT_GROUP_AUTHORITY, id: 'grouplimit', handler: ExportGroupMsg },//匯出群組權限
            '->',
            { text: ' ' }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        selModel: sm
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdFgroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});





/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    addWin.show();
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], FgroupStore);
    }
}

/*************************************************************************************刪除*************************************************************************************************/
onRemoveClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.rowid + '|';
                }
                Ext.Ajax.request({
                    url: '/Fgroup/Delete',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, result.msg);
                        if (result.success) {
                            FgroupStore.load();
                        }
                    },
                    failure: function () {
                    }
                });
            }
        });
    }
}

/***********************************************************************************權限分配***********************************************************************************************/
onAuthClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        groupAuthority(row[0].data.rowid);

    }
}

/***********************************************************************************人員管理***********************************************************************************************/
onCallidClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        var groupId = Ext.getCmp('gdFgroup').getSelectionModel().getSelection()[0].data.rowid;
        Ext.Ajax.request({
            url: '/Fgroup/QueryCallidById',
            params: { groupId: groupId },
            success: function (response) {
                var a = response.responseText;
                var arr = a.split(",");
                if (!CallidForm) {
                    CallidForm = Ext.create('widget.window', {
                        title: TOOL_CALLID, closable: true,
                        closeAction: 'hide',
                        modal: true,
                        width: 500,
                        minWidth: 500,
                        height: document.documentElement.clientHeight / 2,
                        layout: 'fit',
                        bodyStyle: 'padding:5px;',
                        items: [{
                            xtype: 'itemselector',
                            name: 'itemselector',
                            id: 'itemselector-field',
                            toListTitle: HAVE_SELECT,//已選
                            fromListTitle: CAN_SELECT,//可選
                            height: document.documentElement.clientHeight / 2 - 100,
                            store: ManageUserStore,
                            displayField: 'name',
                            valueField: 'callid',
                            allowBlank: false,
                            msgTarget: 'side'
                        }, {
                            xtype: 'textfield',
                            name: 'groupId',
                            hidden: true
                        }],
                        fbar: [{
                            xtype: 'button',
                            text: RESET,
                            id: 'reset',
                            handler: function () {
                                Ext.getCmp("itemselector-field").reset();
                                return false;
                            }
                        },
                    {
                        xtype: 'button',
                        text: SAVE,
                        id: 'save',
                        handler: function () {
                            Ext.Ajax.request({
                                url: '/Fgroup/AddCallid',
                                params: { groupId: Ext.getCmp('gdFgroup').getSelectionModel().getSelection()[0].data.rowid, callid: Ext.getCmp("itemselector-field").getValue() },
                                success: function (response, opts) {
                                    var result = eval("(" + response.responseText + ")");
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                    CallidForm.hide();
                                },
                                failure: function (response) {
                                    var result = eval("(" + response.responseText + ")");
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                }
                            });
                        }
                    }]
                    });
                }

                CallidForm.show();
                Ext.getCmp("itemselector-field").setValue(arr);
            },
            failure: function (response) {
                var resText = eval("(" + response.responseText + ")");
                alert(resText.rpackCode);
            }
        });
    }
}

ExportGroupMsg = function ()
{
    window.open("/Fgroup/ExportGroupLimit");
}