
/***************************新增***********************/
onAddClick = function () {
    editFunction(null,SphinxExcludeStore);
}
/*********************編輯**********************/
onEditClick = function () {
    var row = Ext.getCmp("SphinxExcludePanel").getSelectionModel().getSelection();//獲取選中的行數
    if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], SphinxExcludeStore);
    }
}
//刪除
onDeleteClick = function () {
    var row = Ext.getCmp("SphinxExcludePanel").getSelectionModel().getSelection();
    Ext.Msg.confirm(CONFIRM, Ext.String.format("刪除選中" + row.length + "條數據?", row.length), function (btn) {
        if (btn == 'yes') {
            var rowIDs = '';
            for (var i = 0; i < row.length; i++) {
                rowIDs += row[i].data["product_id"] + ',';
            }
            Ext.Ajax.request({
                url: '/SystemKeyWord/DeleteById',//執行方法
                method: 'post',
                params: { rid: rowIDs },
                success: function (response) {
                    var result = Ext.decode(response.responseText);
                    if (result.success) {
                        Ext.Msg.alert(INFORMATION, "刪除成功!");
                        for (var i = 0; i < row.length; i++) {
                            SphinxExcludeStore.remove(row[i]);
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
function editFunction(RowID, Store) {
    var ID = 0;
    if (RowID != null) {
        ID = RowID.data["product_id"];
    }

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,//True 为 Panel 填充画面,默认为false.
        plain: true,//不在选项卡栏上显示全部背景。
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/SystemKeyWord/SphinxExcludeSave',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },//side添加一个错误图标在域的右边，鼠标悬停上面时弹出显示消息。
        //这个布局将子元素的位置与父容器大小进行关联固定. 如果容器大小改变, 所有固定的子项将按各自的anchor 规则自动被重新渲染固定.
        items: [
            {
                xtype: 'numberfield',
                fieldLabel: "商品編號",
                name: 'product_id',
                id: 'product_id',
                allowBlank: false,
                minValue: 0,
                maxValue:2147483647,
                hideTrigger: true
            }
            //,
            //{
            //    xtype: 'textareafield',
            //    fieldLabel: "活動描述",
            //    id: 'event_desc',
            //    name: 'event_desc',
            //    height: 200,
            //    allowBlank: false
            //}
        ],
        buttons: [
        {
            text: '保存',
            formBind: true,// 设置按钮与表单绑定，需在表单验证通过后才可使用
            handler: function () {
                var form = this.up('form').getForm();//沿着 ownerCt 查找匹配简单选择器的祖先容器.
                if (form.isValid()) {//这个函数会调用已经定义的校验规则来验证输入框中的值，如果通过则返回true
                    form.submit({
                        params: {
                            pid: ID,
                            productid: Ext.getCmp("product_id").getValue()
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success=="true") {
                                Ext.Msg.alert(INFORMATION, "保存成功!");
                                editWin.close();
                                SphinxExcludeStore.removeAll();
                                SphinxExcludeStore.load();
                            } else {
                                if (result.success=="-1") {
                                    Ext.Msg.alert(INFORMATION, '已存在該商品編號');
                                    Ext.getCmp("product_id").setValue("");
                                }
                               else if (result.success == "-2") {
                                   Ext.Msg.alert(INFORMATION, '不存在該商品編號');
                                   Ext.getCmp("product_id").setValue("");
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            }
                        }
                    })
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: RowID ? '修改商品' : '增加商品',
        id: 'editWin',
        iconCls: RowID ? "icon-user-edit" : "icon-user-add",
        width: 400,
        height:150,
        layout: 'fit',
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: '關閉',
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {
                if (RowID) {
                    editFrm.getForm().loadRecord(RowID); //如果是編輯的話,加載選中行數據,载入一个 Ext.data.Model 到表单中
                }
                else {
                    editFrm.getForm().reset(); //如果不是編輯的話
                }
            }
        }
    });
    editWin.show();
}

setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    return s;
}
var pageSize = 23;
Ext.define('gigade.SphinxExcludeModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "product_id", type: "int" },
        { name: "product_name", type: "string" },
        { name: "kdate", type: "string" },
        { name: "user_username", type: "string" }
    ]
});
var SphinxExcludeStore = Ext.create('Ext.data.Store', {
    model: 'gigade.SphinxExcludeModel',
    autoDestroy: true,
    autoLoad: false,
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/SystemKeyWord/GetSphinxExclude',
        actionMethods: 'post',
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
            Ext.getCmp("SphinxExcludePanel").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("SphinxExcludePanel").down('#delete').setDisabled(selections.length == 0);
        }
    }
});
Ext.onReady(function () {
    var SphinxExcludePanel = Ext.create('Ext.grid.Panel', {
        id: 'SphinxExcludePanel',
        flex: 1.8,
        store: SphinxExcludeStore,
        width: document.documentElement.clientWidth,
        columnLines: true,//顯示列線條
        frame: true,//Panel是圆角框显示
        columns: [
            { header: "商品編號", dataIndex: 'product_id', width: 100, align: 'center' },
            { header: "商品名稱", dataIndex: 'product_name', width: 250, align: 'center' },
            { header: "建立時間", dataIndex: 'kdate', width: 150, align: 'center' },
            { header: "建立者", dataIndex: 'user_username', width: 100, align: 'center' }
        ],
        tbar: [
         { xtype: 'button', text: "新增", id: 'add', iconCls: 'icon-user-add', handler: onAddClick }, '-',
         { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick }, '-',
         { xtype: 'button', text: "刪除", id: 'delete', iconCls: 'icon-user-remove', disabled: true, handler: onDeleteClick },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: SphinxExcludeStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY,
            handler: Query
        }),
        selModel: sm
    });
    SphinxExcludeStore.on('beforeload', function () {
        Ext.apply(SphinxExcludeStore.proxy.extraParams, {
            oid: Ext.getCmp('oid').getValue(),
            time_start: Ext.getCmp('start_time').getValue(),
            time_end: Ext.getCmp('end_time').getValue()
        });
    });
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 120,
        border: 0,
        bodyPadding: 13,
        width: document.documentElement.clientWidth,
        items: [
                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,
                    layout: 'hbox',
                    margin: '0 0 18 0',
                    items: [
                        {
                            xtype: 'numberfield',
                            allowBlank: true,
                            fieldLabel: "商品編號",
                            margin: '0 0 0 0',
                            labelWidth: 100,
                            width: 254,
                            id: 'oid',
                            name: 'searchcontentid',
                            minValue: 0,
                            maxValue: 2147483647,
                            hideTrigger:true,
                            listeners: {
                                specialkey: function (field, e) {
                                    if (e.getKey() == Ext.EventObject.ENTER) {
                                        Query();
                                    }
                                }
                            }
                        },
                            {
                                xtype: 'textfield',
                                allowBlank: true,
                                fieldLabel: "商品名稱",
                                margin: '0 0 0 10',
                                labelWidth: 70,
                                width: 220,
                                id: 'productname',
                                name: 'productname',
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
                    margin: '0 0 0 0',
                    items: [
                        {
                            xtype: 'displayfield',
                            value: '建立起止時間:'
                        },
                       {
                           xtype: "datefield",
                           editable: false,
                           margin: '0 0 0 14',
                           id: 'start_time',
                           name: 'start_time',
                           format: 'Y-m-d 00:00:00',
                           width: 150,
                           listeners: {
                               specialkey: function (field, e) {
                                   if (e.getKey() == Ext.EventObject.ENTER) {
                                       Query();
                                   }
                               }
                                , select: function () {
                                    if (Ext.getCmp("start_time").getValue() != null) {
                                        Ext.getCmp("end_time").setMinValue(Ext.getCmp("start_time").getValue());
                                    }
                                }
                           }
                       },
                       { xtype: 'displayfield', value: '~ ', margin: '0 0 0 10', },
                       {
                           xtype: "datefield",
                           editable: false,
                           margin: '0 0 0 10',
                           id: 'end_time',
                           name: 'end_time',
                           format: 'Y-m-d 23:59:59',
                           width: 150,
                           listeners: {
                               specialkey: function (field, e) {
                                   if (e.getKey() == Ext.EventObject.ENTER) {
                                       Query();
                                   }
                               }
                                , select: function () {
                                    if (Ext.getCmp("end_time").getValue() != null) {
                                        Ext.getCmp("start_time").setMaxValue(Ext.getCmp("end_time").getValue());
                                    }
                                }
                           }
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
                            text: SEARCH,
                            iconCls: 'icon-search',
                            id: 'btnQuery',
                            handler: Query
                        },
                         {
                             xtype: 'button',
                             text: RESET,
                             margin: '0 0 0 10',
                             id: 'btn_reset1',
                             iconCls: 'ui-icon ui-icon-reset',
                             listeners: {
                                 click: function () {
                                     frm.getForm().reset();
                                     var datetime1 = new Date();
                                     datetime1.setFullYear(2000, 1, 1);
                                     var datetime2 = new Date();
                                     datetime2.setFullYear(2100, 1, 1);
                                     Ext.getCmp("start_time").setMinValue(datetime1);
                                     Ext.getCmp("start_time").setMaxValue(datetime2);
                                     Ext.getCmp("end_time").setMinValue(datetime1);
                                     Ext.getCmp("end_time").setMaxValue(datetime2);
                                 }
                             }
                         }
                     ]
                }
        ]
    });
    function Query() {
        var falg = 0;
        var oid = Ext.getCmp('oid').getValue(); if (oid != null) { falg++; } 
        var productname = Ext.getCmp('productname').getValue().trim(); if (productname != "") { falg++; }
        var time_start = Ext.getCmp('start_time').getValue(); if (time_start != null) { falg++; }
        var time_end = Ext.getCmp('end_time').getValue(); if (time_end != null) { falg++; }
        if(falg==0)
        {
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
        SphinxExcludeStore.removeAll();
        SphinxExcludeStore.loadPage(1,
            {
                params: {
                    oid: oid,
                    time_start: time_start,
                    time_end: time_end,
                    productname:productname
                }
            });
    };
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, SphinxExcludePanel],
        renderTo: Ext.getBody(),
        autoScroll: true,//自動顯示滾動條
        listeners: {
            resize: function () {//在组件被调整大小之后触发,首次布局初始化大小时不触发
                SphinxExcludePanel.clientWidth = document.documentElement.clientWidth;
                this.doLayout();//手动强制这个容器的布局进行重新计算。大多数情况下框架自动完成刷新布局。
            }
        }
    });
});