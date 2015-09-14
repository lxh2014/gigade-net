var pageSize = 25;
Ext.define('gigade.OrderDetailManage', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'odm_id', type: 'int' },
    { name: 'odm_user_id', type: 'int' },
    { name: 'odm_user_name', type: 'string' },
    { name: 'odm_status', type: 'int' },
    { name: 'odm_createdate', type: 'string' },
    { name: 'odm_createuser', type: 'int' },
       { name: 'user_username', type: 'string' },
    
    ]
});

var ManageStatusStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "不限", "value": "-1" },
        { "txt": "啟用", "value": "1" },
        { "txt": "未啟用", "value": "0" }
    ]
});
//獲取用戶
Ext.define('ManageUser', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "user_id", type: "int" },
        { name: "user_username", type: "string" }
    ]
});

var ManageUserStore = Ext.create('Ext.data.Store', {
    model: 'ManageUser',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/OrderManage/ManageUserStore",
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
var OrderDetailManageStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.OrderDetailManage',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetODMList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
OrderDetailManageStore.on('beforeload', function () {
    Ext.apply(OrderDetailManageStore.proxy.extraParams,
    {
        odm_status: Ext.getCmp('status').getValue(),
    });
});

Ext.onReady(function () {
    var gdList = Ext.create('Ext.grid.Panel', {
        id: 'gdList',
        store: OrderDetailManageStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 1,
        columns: [
            { header: "編號", dataIndex: 'odm_id', width: 100, align: 'center', hidden: true },
            { header: "姓名", dataIndex: 'odm_user_name', width: 150, align: 'center' },
            { header: "創建人", dataIndex: 'user_username', width: 150, align: 'center' },
            { header: "創建時間", dataIndex: 'odm_createdate', width: 150, align: 'center' },
            {
                header: "狀態", dataIndex: 'odm_status', align: 'center', hidden: false,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value==0) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.odm_id + ")'><img hidValue='1' id='img" + record.data.odm_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.odm_id + ")'><img hidValue='0' id='img" + record.data.odm_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
        {
            xtype: 'button',
            text: "新增",
            id: 'add',
            iconCls: 'icon-user-add',
            handler: addClick
        },
        '->',
        {
            xtype: 'combobox',
            fieldLabel: '狀態',
            store: ManageStatusStore,
            id: 'status',
            displayField: 'txt',
            valueField: 'value',
            editable: false,
            labelWidth:60,
            width: 180,
            displayField: 'txt',
            value:-1,
        },     
        {
            xtype: 'button',
            iconCls: 'ui-icon ui-icon-search-2',
            text: "查詢",
            handler: Query
        }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: OrderDetailManageStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdList],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdList.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    OrderDetailManageStore.load({ params: { start: 0, limit: 25 } });
})

var addClick = function () {
    var AddFrm = Ext.create('Ext.form.Panel', {
        id: 'AddFrm',
        frame: true,
        plain: true,
        layout: 'anchor',     
        labelWidth: 45,
        url: '/OrderManage/InsertODM',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 120 },
        items: [
            {
                xtype: 'combobox',
                fieldLabel: "選擇管理員",
                labelWidth: 110,
                id: 'manage_user',
                name: 'manage_user',
                width: 400,
                store: ManageUserStore,
                emptyText: '請選擇',
                allowBlank: false,             
                editable:false,              
                displayField: 'user_username',
                valueField: 'user_id'
            }
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            odm_user_id: Ext.htmlEncode(Ext.getCmp("manage_user").getValue()),
                            odm_user_name: Ext.htmlEncode(Ext.getCmp("manage_user").getRawValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (result.msg == 1) {
                                    Ext.Msg.alert(INFORMATION, "請不要重複添加!");
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, "添加成功!");
                                    OrderDetailManageStore.load();
                                    AddWin.close();
                                }
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        }, 
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            }
        }]
    });
    var AddWin = Ext.create('Ext.window.Window', {
        title: '新增權限人員',
        id: 'AddWin',
        iconCls: 'icon-user-edit',
        width: 450,
        height: 160,
        layout: 'fit',
        items: [AddFrm],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        constrain: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        margin: '5px 0px 0px 0px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: "關閉",
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('AddWin').destroy();
                        }
                        else {
                            return false;
                        }
                    });
                }
            }
        ]
    });
    AddWin.show();
}

function Query(x) {
    OrderDetailManageStore.removeAll();
    Ext.getCmp("gdList").store.loadPage(1, {
        params: {
            odm_status:Ext.getCmp('status').getValue(),
        }
    });
}
/*********************啟用/禁用**********************/
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/OrderManage/UpODMStatus",
        data: {
            "odm_id": id,
            "active": activeValue
        },
        type: "post",
        type: 'text',
        success: function (msg) {
            OrderDetailManageStore.load();
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}

