var pageSize = 24;
Ext.define('gigade.DeliveryStorePlace', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "dsp_id", type: "int" },
        { name: "dsp_name", type: "string" },
        { name: "dsp_address", type: "string" },
        { name: "dsp_telephone", type: "string" },
        { name: "big", type: "string" },
        { name: "parameterName", type: "string" },
        { name: "dsp_note", type: "string" },
        { name: "create_username", type: "string" },
        { name: "create_time", type: "string" },
        { name: "modify_username", type: "string" },
        { name: "modify_time", type: "string" },
        { name: "dsp_status", type: "int" },
        { name: "dsp_big_code", type: "string" },
        { name: "dsp_deliver_store", type: "string" }
    ]
});
var deliveryStorePlaceStore = Ext.create('Ext.data.Store', {
    model: 'gigade.DeliveryStorePlace',
    autoDestroy: true,
    autoLoad: true,
    pageSize:pageSize,
    proxy: {
        type: 'ajax',
        url: '/DeliveryStore/GetDeliveryStorePlaceList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

Ext.define('gigade.t_zip_code', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "big", type: "string" },
    { name: "bigcode", type: "string" }
    ]
});
var t_zip_codeStore = Ext.create('Ext.data.Store', {
    model: 'gigade.t_zip_code',
    autoDestroy: true,
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/DeliveryStore/GetTZipCodeList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
}); 
Ext.define('gigade.DspDeliverStore', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ParameterCode", type: "string" },
        { name: "parameterName", type: "string" }
    ]
});
var DspDeliverStore = Ext.create('Ext.data.Store', {
    model: 'gigade.DspDeliverStore',
    autoDestroy: true,
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/DeliveryStore/GetDspDeliverStoreList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdAccum").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdAccum").down('#delete').setDisabled(selections.length == 0);
        }
    }
});
function Query() {
    var falg = 0;
    var dsp_name = Ext.getCmp('dsp_name_serch').getValue().trim(); if (dsp_name != '') { falg++; }
    var dsp_big_code = Ext.getCmp('dsp_big_code_serch').getValue(); if (dsp_big_code != '0') { falg++; }
    var dsp_deliver_store = Ext.getCmp('dsp_deliver_store_serch').getValue(); if (dsp_deliver_store != '0') { falg++; }
    if (falg == 0) {
        Ext.Msg.alert("提示", "請輸入營業所名稱或選擇查詢條件");
        return false;
    }
    deliveryStorePlaceStore.removeAll();
    Ext.getCmp("gdAccum").store.loadPage(1,
        {
            params: {
                dsp_name: dsp_name,
                dsp_big_code: dsp_big_code,
                dsp_deliver_store: dsp_deliver_store
            }
        });
}
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',//anchor固定
        height: 90,
        border: 0,
        bodyPadding: 13,
        width: document.documentElement.clientWidth,
        items: [
                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,
                    layout: 'hbox',
                    items: [
                            {
                                xtype: 'textfield',
                                fieldLabel: "營業所名稱",
                                allowBlank: true,//可以為空
                                editable: false,//阻止直接在表单项的文本框中输入字符
                                id: 'dsp_name_serch',
                                name: 'dsp_name_serch',
                                labelWidth: 80,
                                width: 210,
                                listeners: {
                                    specialkey: function (field, e) {
                                        if (e.getKey() == Ext.EventObject.ENTER) {
                                            Query();
                                        }
                                    }
                                }
                            },
                    {
                        xtype: 'combobox',
                        fieldLabel: "區域",
                        margin: '0 0 0 10',
                        allowBlank: true,//可以為空
                        editable: false,//阻止直接在表单项的文本框中输入字符
                        id: 'dsp_big_code_serch',
                        name: 'dsp_big_code_serch',
                        store: t_zip_codeStore,
                        displayField: 'big',
                        valueField: 'bigcode',
                        labelWidth: 60,
                        width: 160,
                        value: '0',
                        queryMode: 'local',
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                            {
                                xtype: 'combobox',
                                fieldLabel: "所屬公司",
                                margin: '0 0 0 10',
                                allowBlank: true,//可以為空
                                editable: false,//阻止直接在表单项的文本框中输入字符
                                id: 'dsp_deliver_store_serch',
                                name: 'dsp_deliver_store_serch',
                                store: DspDeliverStore,
                                displayField: 'parameterName',
                                valueField: 'ParameterCode',
                                labelWidth: 60,
                                value:'0',
                                queryMode: 'local',
                                listeners: {
                                    specialkey: function (field, e) {
                                        if (e.getKey() == Ext.EventObject.ENTER) {
                                            Query();
                                        }
                                    }
                                }
                            }
                    ]
                },
                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,
                    layout: 'hbox',
                    items: [
                         {
                             xtype: 'button',
                             text: SEARCH,
                             iconCls: 'icon-search',
                             id: 'btnQuery',
                             handler: Query
                         }
                         , {
                             xtype: 'button',
                             text: '重置',
                             id: 'btn_reset',
                             iconCls: 'ui-icon ui-icon-reset',
                             margin: '0 0 0 10',
                             listeners:
                             {
                                 click: function () {
                                     frm.getForm().reset();
                                 }
                             }
                         }
                    ]
                }
        ]
    });
    deliveryStorePlaceStore.on('beforeload', function () {
        Ext.apply(deliveryStorePlaceStore.proxy.extraParams, {
            dsp_name: Ext.getCmp('dsp_name_serch').getValue().trim(),
            dsp_big_code:Ext.getCmp('dsp_big_code_serch').getValue(),
            dsp_deliver_store: Ext.getCmp('dsp_deliver_store_serch').getValue()
        });
    });
    var gdAccum = Ext.create('Ext.grid.Panel', {
        id: 'gdAccum',
        flex: 1.8,
        store: deliveryStorePlaceStore,
        width: document.documentElement.clientWidth,
        columnLines: true,//顯示列線條
        frame: true,//Panel是圆角框显示
        columns: [{ header: "編號", dataIndex: 'dsp_id', width: 70, align: 'center' },
                { header: "營業所名稱", dataIndex: 'dsp_name', width: 130, align: 'center' },
                { header: "營業所地址", dataIndex: 'dsp_address', width: 250, align: 'center' },
                { header: "營業所電話", dataIndex: 'dsp_telephone', width: 120, align: 'center' },
                { header: "區域", dataIndex: 'big', width: 50, align: 'center' },
                { header: "所屬公司", dataIndex: 'parameterName', width: 150, align: 'center' },
                { header: "備註", dataIndex: 'dsp_note', width: 150, align: 'center' },
                { header: "創建人", dataIndex: 'create_username', width: 100, align: 'center' },
                { header: "創建時間", dataIndex: 'create_time', width: 200, align: 'center' },
                { header: "更新人", dataIndex: 'modify_username', width: 100, align: 'center' },
                { header: "更新時間", dataIndex: 'modify_time', width: 200, align: 'center' }
        ],
        tbar: [
           { xtype: 'button', text: "新增", id: 'add', iconCls: 'icon-user-add', handler: onAddClick }, '-',
           { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick }, '-',
           { xtype: 'button', text: "刪除", id: 'delete', iconCls: 'icon-user-remove', disabled: true, handler: onDeleteClick }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: deliveryStorePlaceStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        selModel: sm
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, gdAccum],
        renderTo: Ext.getBody(),
        autoScroll: true,//自動顯示滾動條
        listeners: {
            resize: function () {//在组件被调整大小之后触发,首次布局初始化大小时不触发
                gdAccum.clientWidth = document.documentElement.clientWidth;
                this.doLayout();//手动强制这个容器的布局进行重新计算。大多数情况下框架自动完成刷新布局。
            }
        }
    });
});
/***************************新增***********************/
onAddClick = function () {
    editFunction(null, deliveryStorePlaceStore);
}
/*********************編輯**********************/
onEditClick = function () {
    var row = Ext.getCmp("gdAccum").getSelectionModel().getSelection();//獲取選中的行數
    if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], deliveryStorePlaceStore);
    }
}
//刪除
onDeleteClick = function () {
    var row = Ext.getCmp("gdAccum").getSelectionModel().getSelection();
    Ext.Msg.confirm(CONFIRM, Ext.String.format("刪除選中" + row.length + "條數據?", row.length), function (btn) {
        if (btn == 'yes') {
            var rowIDs = '';
            for (var i = 0; i < row.length; i++) {
                rowIDs += row[i].data["dsp_id"] + ',';
            }
            Ext.Ajax.request({
                url: '/DeliveryStore/DeleteDeliveryStorePlaceByIds',//執行方法
                method: 'post',
                params: { rid: rowIDs },
                success: function (response) {
                    var result = Ext.decode(response.responseText);
                    if (result.success) {
                        Ext.Msg.alert(INFORMATION, "刪除成功!");
                        for (var i = 0; i < row.length; i++) {
                            deliveryStorePlaceStore.remove(row[i]);
                        }
                    }
                    else {
                        Ext.Msg.alert(INFORMATION, "無法刪除!");
                    }
                },
                failure: function () {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            });
        }
    });

}