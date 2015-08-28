
Ext.Loader.setConfig({ enabled: true });

Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector',
    'Ext.ux.CheckColumn'
]);
var pageSize = 25;
Ext.define('gigade.MailGroup', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "row_id", type: "int" },
        { name: "group_name", type: "string" },
        { name: "remark", type: "string" },
        { name: "status", type: "int" },
        { name: "group_code", type: "string" },
                { name: "callid", type: "int" },
    ]
});
var CallidForm;
var MailGroupStore = Ext.create('Ext.data.Store', {
    model: 'gigade.MailGroup',
    autoLoad:true,
    proxy: {
        type: 'ajax',
        url: '/MailGroup/MailGroupList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

Ext.define('gigade.MailMapUser', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "user_name", type: "string" },
          { name: "nameemail", type: "string" },
        { name: "row_id", type: "int" }]
});

var MailMapStore = Ext.create('Ext.data.Store', {
    model: 'gigade.MailMapUser',
    autoLoad:true,
    proxy: {
        type: 'ajax',
        url: '/MailGroup/MailUserList?pagers=0',
        reader: {
            type: 'json',
            root: 'data'
        }
    },
});


var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            var row = Ext.getCmp("MailGroup").getSelectionModel().getSelection();
            Ext.getCmp("MailGroup").down('#edit').setDisabled(selections.length == 0);
            ////Ext.getCmp("MailGroup").down('#remove').setDisabled(selections.length == 0);
            Ext.getCmp("MailGroup").down('#callid').setDisabled(selections.length == 0);
            if (row != "") {
                if (row[0].data.status == 0) {
                    Ext.getCmp("MailGroup").down('#edit').setDisabled(true);
                    Ext.getCmp("MailGroup").down('#callid').setDisabled(true);
                }
            }
        }
    }
});

Ext.onReady(function () {
    var MailGroup = Ext.create('Ext.grid.Panel', {
        id: 'MailGroup',
        store: MailGroupStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "群組名稱", dataIndex: 'group_name', width: 150, align: 'center' },
            { header: "群組編碼", dataIndex: 'group_code', width: 150, align: 'center' },
            { header: "人數", dataIndex: 'callid', align: '120', align: 'center' },
            { header: "備註", dataIndex: 'remark', align: '120', align: 'center' },
             {
                 header: "啓用", dataIndex: 'status', align: '120', align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value == 1) {
                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.row_id + ")'><img hidValue='0' id='img" + record.data.row_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                     }
                     else {
                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.row_id + ")'><img hidValue='1' id='img" + record.data.row_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                     }
                 }
             },
        ],
        tbar: [
            { xtype: 'button', text: "新增", id: 'add',  iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },
            { xtype: 'button', text: "編輯", id: 'edit',  iconCls: 'ui-icon ui-icon-user-edit', disabled: true, handler: onEditClick },
         //   { xtype: 'button', text: "刪除", id: 'remove',  iconCls: 'ui-icon ui-icon-user-delete', disabled: true, handler: onRemoveClick },
            { xtype: 'button', text: "人員管理", id: 'callid', iconCls: 'ui-icon ui-icon-user-suite', disabled: true, handler: onCallidClick },
            //{
            //    xtype: 'button', text: "刷新", id: 'refresh', handler: function () {
            //        window.location.reload(true);
            //    }
            //},
            '->',
            { text: ' ' }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: MailGroupStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: "當前顯示記錄" + ': {0} - {1}' + "總計" + ': {2}',
            emptyMsg: "沒有記錄可以顯示"
        }),
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
        items: [MailGroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                MailGroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    MailGroupStore.load({ params: { start: 0, limit: 25 } });
});

//*********新增********//
onAddClick = function () {
    editFunction(null, MailGroupStore);
}

//*********編輯********//
onEditClick = function () {
    var row = Ext.getCmp("MailGroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息","沒有選擇一行！");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行！");
    } else if (row[0].data.status == 1) {
        editFunction(row[0], MailGroupStore);
    }
    else {
        Ext.Msg.alert("提示信息","請先啓用此群組");
    }
}

//*********刪除********//
onRemoveClick = function () {
    var row = Ext.getCmp("MailGroup").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert("提示信息","沒有選擇一行！");
    }
    else {
        Ext.Msg.confirm("確認信息", Ext.String.format("刪除選中 {0} 條數據？", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.row_id + '|';
                }
                Ext.Ajax.request({
                    url: '/MailGroup/DeleteMailGroup',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form,action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert("提示信息", "刪除成功！");
                        }
                        else {
                            Ext.Msg.alert("提示信息", "刪除失敗！");
                        }
                        MailGroupStore.load();
                    },
                    failure: function () {
                        Ext.Msg.alert("提示信息", "刪除失敗！");
                        MailGroupStore.load();
                    }
                });
            }
        });
    }
}

//******更改狀態******//
function UpdateActive(row_id) {
    var activeValue = $("#img" + row_id).attr("hidValue");
    $.ajax({
        url: "/MailGroup/UpMailGroupStatus",
        data: {
            "id": row_id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                MailGroupStore.load();
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                MailGroupStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert("提示信息", "操作失敗");
            MailGroupStore.load();
        }
    });
}

//*********人員管理********//

onCallidClick = function () {
    var row = Ext.getCmp("MailGroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行！");
    }
    else if (row[0].data.status == 1) {
        var groupId = Ext.getCmp('MailGroup').getSelectionModel().getSelection()[0].data.row_id;
        Ext.Ajax.request({
            url: '/MailGroup/QueryUserById',
            params: { groupId: groupId },
            success: function (response) {
                var a = response.responseText;
                var arr = a.split(",");
                if (!CallidForm) {
                    CallidForm = Ext.create('widget.window', {
                        title: "人員管理", closable: true,
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
                            toListTitle: '已選',
                            fromListTitle: '可選',
                            height: document.documentElement.clientHeight / 2 - 100,
                            store: MailMapStore,
                            displayField: 'nameemail',
                            valueField: 'row_id',
                            allowBlank: true,
                            msgTarget: 'side'
                        }, {
                            xtype: 'textfield',
                            name: 'groupId',
                            hidden: true
                        }],
                        fbar: [{
                            xtype: 'button',
                            text: "重置",
                            id: 'reset',
                            handler: function () {
                                Ext.getCmp("itemselector-field").reset();
                                return false;
                            }
                        },
                    {
                        xtype: 'button',
                        text: "保存",
                        id: 'save',
                        handler: function () {
                            var callid = Ext.getCmp("itemselector-field").getValue();
                            Ext.Ajax.request({
                                url: '/MailGroup/AddCallid',
                                params: { groupId: Ext.getCmp('MailGroup').getSelectionModel().getSelection()[0].data.row_id, callid: callid },
                                success: function (response, opts) {
                                    var result = eval("(" + response.responseText + ")");
                                    Ext.Msg.alert("提示信息", result.msg);
                                    CallidForm.hide();
                                    MailGroupStore.load();
                                },
                                failure: function (response) {
                                    var result = eval("(" + response.responseText + ")");
                                    Ext.Msg.alert("提示信息", result.msg);
                                }
                            });
                        }
                    }]
                    });
                }
                // alert('nnnn');
                CallidForm.show();
                Ext.getCmp("itemselector-field").setValue(arr);
            },
            failure: function (response) {
                var resText = eval("(" + response.responseText + ")");
                alert(resText.rpackCode);
            }
        });
    }
    else {
        Ext.Msg.alert("提示信息", "請先啓用此群組");
    }
}