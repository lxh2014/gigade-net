var pageSize = 25;
Ext.define('gigade.Ipod', {
    extend: 'Ext.data.Model',
    fields: [
     { name: "row_id", type: "int" },
     { name: "po_id", type: "string" },//採購單單號
     { name: "po_type_desc", type: "string" },//採購單別描述
     { name: "erp_id", type: "string" },//品號
     { name: "bkord_allow", type: "string" },
     { name: "cde_dt_incr", type: "int" },
     { name: "cde_dt_var", type: "int" },
     { name: "cde_dt_shp", type: "int" },
     { name: "pwy_dte_ctl", type: "string" },
     { name: "qty_ord", type: "int" },//下單採購量
     { name: "qty_damaged", type: "int" },//不允收量
     { name: "qty_claimed", type: "int" },//允收量
     { name: "promo_invs_flg", type: "string" },
     { name: "req_cost", type: "string" },
     { name: "off_invoice", type: "string" },
     { name: "new_cost", type: "string" },
     { name: "freight_price", type: "int" },
     { name: "prod_id", type: "string" },
     { name: "product_id", type: "int" },
     { name: "product_name", type: "string" },//商品名稱
     { name: "create_user", type: "string" },
     { name: "user_username", type: "string" },
     { name: "create_dtim", type: "string" },
     { name: "change_dtim", type: "string" },
     { name: "user_username", type: "string" },
     { name: "parameterName", type: "string" },
     { name: "plst_id", type: "string" },
     { name: "spec", type: "string" },//規格
     { name: "item_stock", type: "string" }//庫存
    ]
});
var accumAmountStore = Ext.create('Ext.data.Store', {
    model: 'gigade.accumAmount',
    autoDestroy: true,
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/AccumAmount/GetEventId',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

var DateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "活動開始", "value": "0" },
        { "txt": "活動結束", "value": "1" },
        { "txt": "創建時間", "value": "2" }
    ]
});

var accumAmountStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.accumAmount',
    proxy: {
        type: 'ajax',
        url: '/AccumAmount/AccumAmountList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
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
    var oid = Ext.getCmp('oid').getValue(); if (oid != null) { falg++; }
    var productname = Ext.getCmp('productname').getValue().trim(); if (productname != "") { falg++; }
    var time_start = Ext.getCmp('time_start').getValue(); if (time_start != null) { falg++; }
    var time_end = Ext.getCmp('time_end').getValue(); if (time_end != null) { falg++; }
    if (falg == 0) {
        Ext.Msg.alert("提示", "請輸入查詢條件");
        return false;
    }
    if (oid != null) {
        if (oid < 1) {
            Ext.Msg.alert("提示", "商品編號必須大於0");
            return false;
        }
    }
    if (time_start != null && time_end == null) {
        Ext.Msg.alert("提示", "請輸入結束時間");
        return false;
    }
    if (time_end != null && time_start == null) {
        Ext.Msg.alert("提示", "請輸入開始時間");
        return false;
    }
    accumAmountStore.removeAll();
    Ext.getCmp("gdAccum").store.loadPage(1,
        {
            params: {
                name: Ext.getCmp('ddlSel').getValue(),
                searchcontent: Ext.getCmp('selcontent').getValue(),
                time_start: Ext.getCmp('time_start').getValue(),
                time_end: Ext.getCmp('time_end').getValue(),
                //radio1: Ext.getCmp('radio1').getValue(),
                //radio2: Ext.getCmp('radio2').getValue(),
                ddlSel: Ext.getCmp('ddSelName').getValue(),
                dateleibie: Ext.getCmp('dateleibie').getValue()
            }
        });
}
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',//anchor固定
        height: 110,
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
                                xtype: 'combobox',
                                fieldLabel: "活動名稱",
                                //margin: '0 0 0 0',
                                allowBlank: true,//可以為空
                                editable: false,//阻止直接在表单项的文本框中输入字符
                                id: 'ddlSel',
                                name: 'ddlSel',
                                store: accumAmountStoreDL,
                                displayField: 'event_name',
                                valueField: 'event_name',
                                //typeAhead: false,
                                //forceSelection: false,//true时，所选择的值限制在一个列表中的值，false时，允许用户设置任意的文本字段。
                                emptyText: '請選擇',
                                labelWidth: 80,
                                width: 210,
                                queryMode: 'remote',//請求數據模式，local：读取本地数据 remote：读取远程数据
                                lastQuery: ''//匹配字符串的值用于过滤store。删除这个属性来强制重新查询。
                                //,listeners: {
                                //    beforequery: function (e) {
                                //        accumAmountStoreDL.removeAll();
                                //        accumAmountStoreDL.load();
                                //    }
                                //}
                            },
                    {
                        xtype: 'numberfield',
                        fieldLabel: "商品編號",
                        name: 'product_id',
                        id: 'product_id',
                        allowBlank: false,
                        minValue: 0,
                        maxValue: 2147483647,
                        hideTrigger: true
                    },
                            , {
                                xtype: 'textfield',
                                allowBlank: true,
                                fieldLabel: "活動描述",
                                margin: '0 0 0 15',
                                labelWidth: 80,
                                width: 210,
                                id: 'selcontent',
                                name: 'searchcontent'
                            }
                    ]
                },
                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,
                    layout: 'hbox',
                    margin: '0 0 0 0',
                    items: [
                        {
                            xtype: 'datefield',
                            fieldLabel: "開始時間",
                            labelWidth: 80,
                            width: 210,
                            id: 'time_start',
                            name: 'time_start',
                            margin: '0 0 0 0',
                            format: 'Y-m-d 00:00:00',
                            editable: false,
                            value: Tomorrow(1),
                            vtype: 'daterange',
                            endDateField: 'end_time',
                            listeners: {
                                select: function () {
                                    var startTime = Ext.getCmp("time_start");
                                    var endTime = Ext.getCmp("time_end");
                                    if (endTime.getValue() < startTime.getValue()) {
                                        Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間!");
                                        startTime.setValue(new Date(endTime.getValue()));

                                    }

                                }
                                // select: function () {
                                //    if (Ext.getCmp("time_start").getValue() != null) {
                                //        Ext.getCmp("end_time").setMinValue(Ext.getCmp("time_start").getValue());
                                //    }
                                //}

                            }
                        },
                        {
                            xtype: 'label',
                            forId: 'myFieldId',
                            text: '~',
                            margin: '0 0 0 5'
                        },
                        {
                            xtype: 'datefield',
                            fieldLabel: "開始時間",
                            labelWidth: 80,
                            width: 210,
                            id: 'time_end',
                            name: 'time_end',
                            margin: '0 0 0 6',
                            format: 'Y-m-d 23:59:59',
                            editable: false,
                            value: Tomorrow(new Date().getDate()),
                            vtype: 'daterange',
                            startDateField: 'start_time',
                            listeners: {
                                select: function () {
                                    var startTime = Ext.getCmp("time_start");
                                    var endTime = Ext.getCmp("time_end");
                                    if (endTime.getValue() < startTime.getValue()) {
                                        Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間!");
                                        endTime.setValue(new Date(startTime.getValue()));

                                    }
                                }
                                //select: function () {
                                //    if (Ext.getCmp("time_end").getValue() != null) {
                                //        Ext.getCmp("time_start").setMaxValue(Ext.getCmp("time_end").getValue()) ;
                                //    }
                                //}
, specialkey: function (field, e) {
    if (e.getKey() == Ext.EventObject.ENTER) {
        Query();
    }
}

                            }
                        },
                           //{
                           //    xtype: 'label',
                           //    forId: 'tj',
                           //    text: '時間類別:',
                           //    margin: '2 0 0 5'
                           //},
                           //{
                           //    xtype: 'radiofield',
                           //    boxLabel: '活動開始',
                           //    name: 'lei',
                           //    margin: '1 5 0 5',
                           //    id: 'radio1',
                           //    checked: true
                           //},
                           //{
                           //    xtype: 'radiofield',
                           //    boxLabel: '活動結束',
                           //    name: 'lei',
                           //    margin: '1 5 0 5',
                           //    id: 'radio2'
                           //}

                            {
                                xtype: 'combobox',
                                fieldLabel: "時間類別",
                                margin: '0 0 0 10',
                                allowBlank: true,//可以為空
                                editable: false,//阻止直接在表单项的文本框中输入字符
                                id: 'dateleibie',
                                name: 'dateleibie',
                                store: DateStore,
                                displayField: 'txt',
                                valueField: 'value',
                                //typeAhead: false,
                                //forceSelection: false,//true时，所选择的值限制在一个列表中的值，false时，允许用户设置任意的文本字段。
                                emptyText: '請選擇',
                                labelWidth: 60,
                                width: 160,
                                queryMode: 'local',//請求數據模式，local：读取本地数据 remote：读取远程数据
                                value: 0,
                                lastQuery: ''
                                //,listeners: {
                                //    beforequery: function (e) {
                                //        accumAmountStoreDL.removeAll();
                                //        accumAmountStoreDL.load();
                                //    }
                                //}
                            }
                    ]
                },

                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,//如果设置为 true, 则 field 容器自动将其包含的所有属性域的校验错误组合为单个错误信息, 并显示到 配置的 msgTarget 上. 默认值 false.
                    layout: 'hbox',
                    margin: '10 0 0 0',
                    items:
                     [
                        {
                            xtype: 'button',
                            margin: '0 10 0 10',
                            iconCls: 'icon-search',
                            text: "查詢",
                            handler: Query
                        },
                        {
                            xtype: 'button',
                            text: '重置',
                            id: 'btn_reset',
                            iconCls: 'ui-icon ui-icon-reset',
                            listeners:
                            {
                                click: function () {

                                    Ext.getCmp('ddlSel').setValue(0);//查詢條件
                                    Ext.getCmp('selcontent').setValue("");//查詢條件
                                    Ext.getCmp('time_start').setValue(Tomorrow(1));
                                    Ext.getCmp('time_end').setValue(Tomorrow(new Date().getDate()));
                                    //frm.getForm().reset();
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
            dsp_big_code: Ext.getCmp('dsp_big_code_serch').getValue(),
            dsp_deliver_store: Ext.getCmp('dsp_deliver_store_serch').getValue()
        });
    });
    var gdAccum = Ext.create('Ext.grid.Panel', {
        id: 'gdAccum',
        flex: 1.8,
        store: accumAmountStore,
        width: document.documentElement.clientWidth,
        columnLines: true,//顯示列線條
        frame: true,//Panel是圆角框显示
        columns: [{ header: "活動編號", dataIndex: 'event_id', width: 70, align: 'center' },
            { header: "折扣", dataIndex: 'accum_amount', width: 50, align: 'center' },
              { header: "活動開始時間", dataIndex: 'event_start_time', width: 150, align: 'center' },
                { header: "活動結束時間", dataIndex: 'event_end_time', width: 150, align: 'center' },
                  { header: "活動描述", dataIndex: 'event_desc', width: 250, align: 'center' },
                    { header: "活動名稱", dataIndex: 'event_name', width: 150, align: 'center' },
                    {
                        header: "具體活動開始時間", dataIndex: 'event_desc_start', width: 150, align: 'center',
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            if (value == '0001-01-01 00:00:00') {
                                return "null";
                            } else { return value; }
                        }
                    },
                    {
                        header: "具體活動結束時間", dataIndex: 'event_desc_end', width: 150, align: 'center',
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            if (value == '0001-01-01 00:00:00') {
                                return "null";
                            } else { return value; }
                        }
                    },
                    { header: "創建人", dataIndex: 'event_create_user', width: 50, align: 'center' },
                    { header: "創建時間", dataIndex: 'event_create_time', width: 150, align: 'center' },
                    {
                        header: "活動狀態", dataIndex: 'event_status', width: 70, align: 'center',
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//拦截者模式，用于改变渲染到单元格的值和样式
                            if (value == 1) {
                                return "<a href='javascript:void(0);' onclick='UpdateStatus(" + record.data.event_id + ")'><img hidValue='0' id='img" + record.data.event_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                            } else {
                                return "<a href='javascript:void(0);' onclick='UpdateStatus(" + record.data.event_id + ")'><img hidValue='1' id='img" + record.data.event_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                            }
                        }
                    }
        ],
        tbar: [
           { xtype: 'button', text: "新增", id: 'add', iconCls: 'icon-user-add', handler: onAddClick }, '-',
           { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick }, '-',
           { xtype: 'button', text: "刪除", id: 'delete', iconCls: 'icon-user-remove', disabled: true, handler: onDeleteClick },
           //'->',//使用右对齐容器,等同 { xtype: 'tbfill' }
           { xtype: 'tbfill' },
            {
                xtype: 'combobox',
                //editable: false,
                fieldLabel: "活動編號",
                labelWidth: 70,
                width: 150,
                id: 'ddSelName',
                //lastQuery: "",
                name: 'searchcontenttb',
                store: accumAmountStoreDL,
                displayField: 'event_id',
                valueField: 'event_id',
                emptyText: '請選擇...',
                value: 0,
                lastQuery: ''
            },
           {
               xtype: 'button',
               text: SEARCH,
               iconCls: 'icon-search',
               id: 'btnQuery',
               handler: Query
           },
          {
              xtype: 'button',
              text: RESET,
              id: 'btn_reset1',
              iconCls: 'ui-icon ui-icon-reset',
              listeners: {
                  click: function () {
                      Ext.getCmp("ddSelName").setValue("");
                  }
              }
          }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: accumAmountStore,
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
    //ToolAuthority();
    accumAmountStore.load({ params: { start: 0, limit: 25 } });
});
/***************************新增***********************/
onAddClick = function () {
    editFunction(null, accumAmountStore);
}
/*********************編輯**********************/
onEditClick = function () {
    var row = Ext.getCmp("gdAccum").getSelectionModel().getSelection();//獲取選中的行數
    if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], accumAmountStore);
    }
}
//更改狀態
function UpdateStatus(id) {
    var statusValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/AccumAmount/UpdateStats",
        data: {
            "id": id,
            "status": statusValue
        },
        type: "post",
        type: 'text',
        success: function (msg) {
            if (statusValue == 1) {
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
//刪除
onDeleteClick = function () {
    var row = Ext.getCmp("gdAccum").getSelectionModel().getSelection();
    Ext.Msg.confirm(CONFIRM, Ext.String.format("刪除選中" + row.length + "條數據?", row.length), function (btn) {
        if (btn == 'yes') {
            var rowIDs = '';
            for (var i = 0; i < row.length; i++) {
                rowIDs += row[i].data["event_id"] + ',';
            }
            Ext.Ajax.request({
                url: '/AccumAmount/DeleteById',//執行方法
                method: 'post',
                params: { rid: rowIDs },
                success: function (response) {
                    var result = Ext.decode(response.responseText);
                    if (result.success) {
                        Ext.Msg.alert(INFORMATION, "刪除成功!");
                        //accumAmountStore.loadPage(1);
                        for (var i = 0; i < row.length; i++) {
                            accumAmountStore.remove(row[i]);
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